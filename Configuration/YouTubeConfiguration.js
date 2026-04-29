define([
    'baseView',
    'loading',
    'toast',
    'emby-input',
    'emby-button',
    'emby-checkbox'
], function (BaseView, loading, toast) {
    'use strict';

    const pluginId = 'B2C3D4E5-F6A7-4B5C-9D0E-1F2A3B4C5D6E';
    const defaultDonateUrl = 'https://paypal.me/eliasbruno123';
    const trendingRegions = [
        ['AF', 'Afghanistan'], ['AX', 'Aland Islands'], ['AL', 'Albania'], ['DZ', 'Algeria'],
        ['AS', 'American Samoa'], ['AD', 'Andorra'], ['AO', 'Angola'], ['AI', 'Anguilla'],
        ['AQ', 'Antarctica'], ['AG', 'Antigua and Barbuda'], ['AR', 'Argentina'], ['AM', 'Armenia'],
        ['AW', 'Aruba'], ['AU', 'Australia'], ['AT', 'Austria'], ['AZ', 'Azerbaijan'],
        ['BS', 'Bahamas'], ['BH', 'Bahrain'], ['BD', 'Bangladesh'], ['BB', 'Barbados'],
        ['BY', 'Belarus'], ['BE', 'Belgium'], ['BZ', 'Belize'], ['BJ', 'Benin'],
        ['BM', 'Bermuda'], ['BT', 'Bhutan'], ['BO', 'Bolivia'], ['BQ', 'Bonaire, Sint Eustatius and Saba'],
        ['BA', 'Bosnia and Herzegovina'], ['BW', 'Botswana'], ['BV', 'Bouvet Island'], ['BR', 'Brazil'],
        ['IO', 'British Indian Ocean Territory'], ['BN', 'Brunei Darussalam'], ['BG', 'Bulgaria'], ['BF', 'Burkina Faso'],
        ['BI', 'Burundi'], ['CV', 'Cabo Verde'], ['KH', 'Cambodia'], ['CM', 'Cameroon'],
        ['CA', 'Canada'], ['KY', 'Cayman Islands'], ['CF', 'Central African Republic'], ['TD', 'Chad'],
        ['CL', 'Chile'], ['CN', 'China'], ['CX', 'Christmas Island'], ['CC', 'Cocos (Keeling) Islands'],
        ['CO', 'Colombia'], ['KM', 'Comoros'], ['CG', 'Congo'], ['CD', 'Congo, Democratic Republic of the'],
        ['CK', 'Cook Islands'], ['CR', 'Costa Rica'], ['CI', "Cote d'Ivoire"], ['HR', 'Croatia'],
        ['CU', 'Cuba'], ['CW', 'Curacao'], ['CY', 'Cyprus'], ['CZ', 'Czechia'],
        ['DK', 'Denmark'], ['DJ', 'Djibouti'], ['DM', 'Dominica'], ['DO', 'Dominican Republic'],
        ['EC', 'Ecuador'], ['EG', 'Egypt'], ['SV', 'El Salvador'], ['GQ', 'Equatorial Guinea'],
        ['ER', 'Eritrea'], ['EE', 'Estonia'], ['SZ', 'Eswatini'], ['ET', 'Ethiopia'],
        ['FK', 'Falkland Islands'], ['FO', 'Faroe Islands'], ['FJ', 'Fiji'], ['FI', 'Finland'],
        ['FR', 'France'], ['GF', 'French Guiana'], ['PF', 'French Polynesia'], ['TF', 'French Southern Territories'],
        ['GA', 'Gabon'], ['GM', 'Gambia'], ['GE', 'Georgia'], ['DE', 'Germany'],
        ['GH', 'Ghana'], ['GI', 'Gibraltar'], ['GR', 'Greece'], ['GL', 'Greenland'],
        ['GD', 'Grenada'], ['GP', 'Guadeloupe'], ['GU', 'Guam'], ['GT', 'Guatemala'],
        ['GG', 'Guernsey'], ['GN', 'Guinea'], ['GW', 'Guinea-Bissau'], ['GY', 'Guyana'],
        ['HT', 'Haiti'], ['HM', 'Heard Island and McDonald Islands'], ['VA', 'Holy See'], ['HN', 'Honduras'],
        ['HK', 'Hong Kong'], ['HU', 'Hungary'], ['IS', 'Iceland'], ['IN', 'India'],
        ['ID', 'Indonesia'], ['IR', 'Iran'], ['IQ', 'Iraq'], ['IE', 'Ireland'],
        ['IM', 'Isle of Man'], ['IL', 'Israel'], ['IT', 'Italy'], ['JM', 'Jamaica'],
        ['JP', 'Japan'], ['JE', 'Jersey'], ['JO', 'Jordan'], ['KZ', 'Kazakhstan'],
        ['KE', 'Kenya'], ['KI', 'Kiribati'], ['KP', 'Korea, Democratic People\'s Republic of'], ['KR', 'Korea, Republic of'],
        ['KW', 'Kuwait'], ['KG', 'Kyrgyzstan'], ['LA', 'Lao People\'s Democratic Republic'], ['LV', 'Latvia'],
        ['LB', 'Lebanon'], ['LS', 'Lesotho'], ['LR', 'Liberia'], ['LY', 'Libya'],
        ['LI', 'Liechtenstein'], ['LT', 'Lithuania'], ['LU', 'Luxembourg'], ['MO', 'Macao'],
        ['MG', 'Madagascar'], ['MW', 'Malawi'], ['MY', 'Malaysia'], ['MV', 'Maldives'],
        ['ML', 'Mali'], ['MT', 'Malta'], ['MH', 'Marshall Islands'], ['MQ', 'Martinique'],
        ['MR', 'Mauritania'], ['MU', 'Mauritius'], ['YT', 'Mayotte'], ['MX', 'Mexico'],
        ['FM', 'Micronesia'], ['MD', 'Moldova'], ['MC', 'Monaco'], ['MN', 'Mongolia'],
        ['ME', 'Montenegro'], ['MS', 'Montserrat'], ['MA', 'Morocco'], ['MZ', 'Mozambique'],
        ['MM', 'Myanmar'], ['NA', 'Namibia'], ['NR', 'Nauru'], ['NP', 'Nepal'],
        ['NL', 'Netherlands'], ['NC', 'New Caledonia'], ['NZ', 'New Zealand'], ['NI', 'Nicaragua'],
        ['NE', 'Niger'], ['NG', 'Nigeria'], ['NU', 'Niue'], ['NF', 'Norfolk Island'],
        ['MK', 'North Macedonia'], ['MP', 'Northern Mariana Islands'], ['NO', 'Norway'], ['OM', 'Oman'],
        ['PK', 'Pakistan'], ['PW', 'Palau'], ['PS', 'Palestine, State of'], ['PA', 'Panama'],
        ['PG', 'Papua New Guinea'], ['PY', 'Paraguay'], ['PE', 'Peru'], ['PH', 'Philippines'],
        ['PN', 'Pitcairn'], ['PL', 'Poland'], ['PT', 'Portugal'], ['PR', 'Puerto Rico'],
        ['QA', 'Qatar'], ['RE', 'Reunion'], ['RO', 'Romania'], ['RU', 'Russian Federation'],
        ['RW', 'Rwanda'], ['BL', 'Saint Barthelemy'], ['SH', 'Saint Helena, Ascension and Tristan da Cunha'], ['KN', 'Saint Kitts and Nevis'],
        ['LC', 'Saint Lucia'], ['MF', 'Saint Martin'], ['PM', 'Saint Pierre and Miquelon'], ['VC', 'Saint Vincent and the Grenadines'],
        ['WS', 'Samoa'], ['SM', 'San Marino'], ['ST', 'Sao Tome and Principe'], ['SA', 'Saudi Arabia'],
        ['SN', 'Senegal'], ['RS', 'Serbia'], ['SC', 'Seychelles'], ['SL', 'Sierra Leone'],
        ['SG', 'Singapore'], ['SX', 'Sint Maarten'], ['SK', 'Slovakia'], ['SI', 'Slovenia'],
        ['SB', 'Solomon Islands'], ['SO', 'Somalia'], ['ZA', 'South Africa'], ['GS', 'South Georgia and the South Sandwich Islands'],
        ['SS', 'South Sudan'], ['ES', 'Spain'], ['LK', 'Sri Lanka'], ['SD', 'Sudan'],
        ['SR', 'Suriname'], ['SJ', 'Svalbard and Jan Mayen'], ['SE', 'Sweden'], ['CH', 'Switzerland'],
        ['SY', 'Syrian Arab Republic'], ['TW', 'Taiwan'], ['TJ', 'Tajikistan'], ['TZ', 'Tanzania'],
        ['TH', 'Thailand'], ['TL', 'Timor-Leste'], ['TG', 'Togo'], ['TK', 'Tokelau'],
        ['TO', 'Tonga'], ['TT', 'Trinidad and Tobago'], ['TN', 'Tunisia'], ['TR', 'Turkey'],
        ['TM', 'Turkmenistan'], ['TC', 'Turks and Caicos Islands'], ['TV', 'Tuvalu'], ['UG', 'Uganda'],
        ['UA', 'Ukraine'], ['AE', 'United Arab Emirates'], ['GB', 'United Kingdom'], ['US', 'United States'],
        ['UM', 'United States Minor Outlying Islands'], ['UY', 'Uruguay'], ['UZ', 'Uzbekistan'], ['VU', 'Vanuatu'],
        ['VE', 'Venezuela'], ['VN', 'Viet Nam'], ['VG', 'Virgin Islands, British'], ['VI', 'Virgin Islands, U.S.'],
        ['WF', 'Wallis and Futuna'], ['EH', 'Western Sahara'], ['YE', 'Yemen'], ['ZM', 'Zambia'],
        ['ZW', 'Zimbabwe']
    ];

    function client() {
        return window.ApiClient || (window.Dashboard && Dashboard.getCurrentApiClient && Dashboard.getCurrentApiClient());
    }

    function field(view, id) {
        return view.querySelector('#' + id);
    }

    function value(view, id) {
        const el = field(view, id);
        return el ? (el.value || '') : '';
    }

    function setValue(view, id, val) {
        const el = field(view, id);
        if (el) {
            el.value = val == null ? '' : val;
        }
    }

    function checked(view, id) {
        const el = field(view, id);
        return !!(el && el.checked);
    }

    function setChecked(view, id, val) {
        const el = field(view, id);
        if (el) {
            el.checked = !!val;
        }
    }

    function numberValue(view, id, fallback, min, max) {
        let parsed = parseInt(value(view, id), 10);
        if (Number.isNaN(parsed)) {
            parsed = fallback;
        }
        return Math.min(Math.max(parsed, min), max);
    }

    function setStatus(view, text) {
        setText(view, 'youtubeConfigurationStatus', text);
    }

    function setText(view, id, text) {
        const el = field(view, id);
        if (el) {
            el.textContent = text || '';
        }
    }

    function splitEntries(text) {
        return (text || '')
            .split(/[,\n\r;]+/)
            .map(item => item.trim())
            .filter(item => item.length > 0);
    }

    function uniqueEntries(entries) {
        const seen = {};
        const result = [];

        entries.forEach(item => {
            const key = item.toLowerCase();
            if (!seen[key]) {
                seen[key] = true;
                result.push(item);
            }
        });

        return result;
    }

    function classifySavedItem(item) {
        if (item.indexOf('@') === 0) {
            return 'Handle';
        }
        if (item.indexOf('UC') === 0) {
            return 'Channel';
        }
        if (item.indexOf('PL') === 0 || item.indexOf('UU') === 0 || item.indexOf('OL') === 0 || item.indexOf('WL') === 0) {
            return 'Playlist';
        }
        return 'Search';
    }

    function setDonateButton(view) {
        const button = field(view, 'btnDonate');
        if (button) {
            button.href = defaultDonateUrl;
        }
    }

    function populateTrendingRegions(view) {
        const select = field(view, 'selectTrendingRegion');
        if (!select || select.youtubeRegionsLoaded) {
            return;
        }

        select.youtubeRegionsLoaded = true;
        select.innerHTML = '';

        const defaultOption = document.createElement('option');
        defaultOption.value = '';
        defaultOption.textContent = 'YouTube default';
        select.appendChild(defaultOption);

        trendingRegions.forEach(region => {
            const option = document.createElement('option');
            option.value = region[0];
            option.textContent = region[1] + ' (' + region[0] + ')';
            select.appendChild(option);
        });
    }

    function bindTabNavigation(view) {
        const navButtons = view.querySelectorAll('.nav-button');
        const pages = {
            settingsPage: field(view, 'settingsPage'),
            setupGuidePage: field(view, 'setupGuidePage')
        };

        navButtons.forEach(button => {
            button.addEventListener('click', e => {
                e.preventDefault();
                const target = button.getAttribute('data-target');

                navButtons.forEach(item => item.classList.remove('ui-btn-active'));
                button.classList.add('ui-btn-active');

                Object.keys(pages).forEach(key => {
                    if (pages[key]) {
                        pages[key].classList.toggle('hide', key !== target);
                    }
                });
            });
        });
    }

    function showToast(type, text) {
        if (typeof toast === 'function') {
            toast({ type: type, text: text });
        }
    }

    return class extends BaseView {
        constructor(view, params) {
            super(view, params);
            this.config = {};
            this.savedItems = [];
            this.watchLaterItems = [];
        }

        onResume(options) {
            super.onResume(options);
            populateTrendingRegions(this.view);
            this.bindEventListeners(this.view);
            this.loadData(this.view);
        }

        bindEventListeners(view) {
            if (view.youtubeConfigurationBound) {
                return;
            }

            view.youtubeConfigurationBound = true;
            bindTabNavigation(view);

            const form = view.querySelector('.youtubeConfigurationForm');
            if (form) {
                form.addEventListener('submit', (e) => {
                    e.preventDefault();
                    this.saveData(view);
                    return false;
                });
            }

            this.bindEntryControls(
                view,
                'txtSavedItemEntry',
                'btnAddSavedItem',
                'savedItemsList',
                () => this.savedItems,
                value => { this.savedItems = value; },
                item => classifySavedItem(item));

            this.bindEntryControls(
                view,
                'txtWatchLaterEntry',
                'btnAddWatchLaterItem',
                'watchLaterItemsList',
                () => this.watchLaterItems,
                value => { this.watchLaterItems = value; },
                () => 'Playlist');
        }

        bindEntryControls(view, inputId, buttonId, listId, getItems, setItems, classify) {
            const input = field(view, inputId);
            const button = field(view, buttonId);
            const add = () => {
                const newItems = splitEntries(value(view, inputId));
                if (newItems.length === 0) {
                    return;
                }

                setItems(uniqueEntries(getItems().concat(newItems)));
                setValue(view, inputId, '');
                this.renderEntryList(view, listId, getItems, setItems, classify);
                this.syncEntryFields(view);
            };

            if (button) {
                button.addEventListener('click', add);
            }

            if (input) {
                input.addEventListener('keydown', e => {
                    if (e.key === 'Enter') {
                        e.preventDefault();
                        add();
                    }
                });
            }
        }

        renderEntryList(view, listId, getItems, setItems, classify) {
            const list = field(view, listId);
            if (!list) {
                return;
            }

            list.innerHTML = '';
            const items = getItems();

            if (items.length === 0) {
                const empty = document.createElement('div');
                empty.className = 'fieldDescription emptyListNote';
                empty.textContent = 'No entries added yet.';
                list.appendChild(empty);
                return;
            }

            const header = document.createElement('div');
            header.className = 'fieldDescription';
            header.style.width = '100%';
            header.style.marginTop = '0';
            header.innerHTML = items.length + ' entr' + (items.length === 1 ? 'y' : 'ies');
            list.appendChild(header);

            items.forEach(item => {
                const chip = document.createElement('div');
                chip.className = 'itemChip';

                const text = document.createElement('span');
                text.className = 'itemChipText';
                text.textContent = item;
                chip.appendChild(text);

                const badge = document.createElement('span');
                badge.className = 'entryBadge';
                badge.textContent = classify(item);
                chip.appendChild(badge);

                const remove = document.createElement('button');
                remove.type = 'button';
                remove.className = 'removeEntryButton';
                remove.title = 'Remove "' + item + '"';
                remove.setAttribute('aria-label', 'Remove ' + item);
                remove.textContent = '×';
                remove.addEventListener('click', () => {
                    setItems(getItems().filter(existing => existing !== item));
                    this.renderEntryList(view, listId, getItems, setItems, classify);
                    this.syncEntryFields(view);
                });
                chip.appendChild(remove);

                list.appendChild(chip);
            });
        }

        syncEntryFields(view) {
            setValue(view, 'txtSavedItems', this.savedItems.join(', '));
            setValue(view, 'txtWatchLaterPlaylist', this.watchLaterItems.join(', '));
        }

        renderAllEntryLists(view) {
            this.renderEntryList(
                view,
                'savedItemsList',
                () => this.savedItems,
                value => { this.savedItems = value; },
                item => classifySavedItem(item));

            this.renderEntryList(
                view,
                'watchLaterItemsList',
                () => this.watchLaterItems,
                value => { this.watchLaterItems = value; },
                () => 'Playlist');
        }

        loadData(view) {
            const apiClient = client();
            if (!apiClient) {
                setStatus(view, 'Could not find the Emby API client.');
                return;
            }

            loading.show();
            apiClient.getPluginConfiguration(pluginId).then(config => {
                this.config = config || {};
                config = this.config;

                setValue(view, 'txtApiKey', config.ApiKey);
                this.savedItems = uniqueEntries(splitEntries(config.SavedItems));
                this.watchLaterItems = uniqueEntries(splitEntries(config.WatchLaterPlaylist));
                this.syncEntryFields(view);
                this.renderAllEntryLists(view);
                setChecked(view, 'chkShowTrending', config.ShowTrending !== false);
                setChecked(view, 'chkShowCategories', config.ShowCategories !== false);
                setChecked(view, 'chkShowRecentlyAdded', !!config.ShowRecentlyAdded);
                setChecked(view, 'chkShowLiveFolders', !!config.ShowLiveFolders);
                setChecked(view, 'chkShowShorts', config.HideShorts === true ? false : config.ShowShorts !== false);
                setValue(view, 'selectTrendingRegion', config.TrendingRegion);
                setValue(view, 'selectTrendingCategory', config.TrendingCategory === '0' ? '' : (config.TrendingCategory || ''));
                setChecked(view, 'chkShowLikeCount', config.ShowLikeCount !== false);
                setChecked(view, 'chkShowCommentCount', !!config.ShowCommentCount);
                setValue(view, 'selectChannelSortBy', config.ChannelSortBy || 'date');
                setValue(view, 'numMaxChannelVideos', config.MaxChannelVideos || 50);
                setValue(view, 'numMaxSearchVideos', config.MaxSearchVideos || 50);
                setValue(view, 'numRecentlyAddedPerChannel', config.RecentlyAddedPerChannel || 10);
                setValue(view, 'numWatchLaterPollMinutes', config.WatchLaterPollMinutes || 3);
                setText(view, 'txtQuotaStatus', config.QuotaStatus || 'Quota usage will appear after the first plugin load.');
                setDonateButton(view);
                setStatus(view, '');
                loading.hide();
            }, error => {
                loading.hide();
                console.error('Error loading YouTube configuration:', error);
                setStatus(view, 'Could not load plugin settings.');
                showToast('error', 'Could not load YouTube settings');
            });
        }

        saveData(view) {
            const apiClient = client();
            if (!apiClient) {
                setStatus(view, 'Could not find the Emby API client.');
                return;
            }

            const config = this.config || {};
            config.ApiKey = value(view, 'txtApiKey').trim();
            this.syncEntryFields(view);
            config.SavedItems = this.savedItems.join(', ');
            config.WatchLaterPlaylist = this.watchLaterItems.join(', ');
            config.ShowTrending = checked(view, 'chkShowTrending');
            config.ShowCategories = checked(view, 'chkShowCategories');
            config.ShowRecentlyAdded = checked(view, 'chkShowRecentlyAdded');
            config.ShowLiveFolders = checked(view, 'chkShowLiveFolders');
            config.ShowShorts = checked(view, 'chkShowShorts');
            config.HideShorts = false;
            config.TrendingRegion = value(view, 'selectTrendingRegion').trim().toUpperCase();
            config.TrendingCategory = value(view, 'selectTrendingCategory').trim();
            config.ShowLikeCount = checked(view, 'chkShowLikeCount');
            config.ShowCommentCount = checked(view, 'chkShowCommentCount');
            config.ChannelSortBy = value(view, 'selectChannelSortBy') || 'date';
            config.MaxChannelVideos = numberValue(view, 'numMaxChannelVideos', 50, 1, 150);
            config.MaxSearchVideos = numberValue(view, 'numMaxSearchVideos', 50, 1, 150);
            config.RecentlyAddedPerChannel = numberValue(view, 'numRecentlyAddedPerChannel', 10, 1, 25);
            config.WatchLaterPollMinutes = numberValue(view, 'numWatchLaterPollMinutes', 3, 1, 60);
            config.Donate = defaultDonateUrl;
            delete config.QuotaStatus;
            delete config.ShortsEnabled;

            loading.show();
            apiClient.updatePluginConfiguration(pluginId, config).then(result => {
                this.config = config;
                setDonateButton(view);
                setStatus(view, 'Settings saved. The YouTube channel will refresh shortly.');
                loading.hide();
                showToast('success', 'YouTube settings saved');

                if (window.Dashboard && Dashboard.processPluginConfigurationUpdateResult) {
                    Dashboard.processPluginConfigurationUpdateResult(result);
                }
            }, error => {
                loading.hide();
                console.error('Error saving YouTube configuration:', error);
                setStatus(view, 'Could not save plugin settings.');
                showToast('error', 'Could not save YouTube settings');
            });
        }
    };
});
