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

    function formatNumber(n) {
        const value = Number(n) || 0;
        try { return value.toLocaleString(); }
        catch (e) { return String(value); }
    }

    function formatResetCountdown(seconds) {
        const total = Math.max(0, Math.floor(Number(seconds) || 0));
        if (total <= 0) return 'now';
        const h = Math.floor(total / 3600);
        const m = Math.floor((total % 3600) / 60);
        if (h >= 1) return h + 'h ' + m + 'm';
        return m + 'm';
    }

    function renderQuota(view, config) {
        const used = Number(config.QuotaUsedToday) || 0;
        const limit = Number(config.QuotaDailyLimit) || 10000;
        const lifetime = Number(config.QuotaLifetime) || 0;
        const resetSec = Number(config.QuotaResetSeconds) || 0;
        const pct = limit > 0 ? Math.min(100, Math.max(0, (used / limit) * 100)) : 0;

        setText(view, 'quotaNumber', formatNumber(used));
        setText(view, 'quotaTotal', formatNumber(limit));
        setText(view, 'quotaPercent', pct.toFixed(1) + '%');
        setText(view, 'quotaLifetime', 'Lifetime: ' + formatNumber(lifetime) + ' units');

        const reset = field(view, 'quotaReset');
        if (reset) {
            reset.textContent = '⏱ Reset in ' + formatResetCountdown(resetSec);
            reset.classList.toggle('quotaResetWarn', resetSec > 0 && resetSec < 1800);
        }

        const bar = field(view, 'quotaBar');
        const fill = field(view, 'quotaBarFill');
        if (fill) fill.style.width = pct.toFixed(2) + '%';
        if (bar) {
            bar.classList.remove('warn', 'danger');
            if (pct >= 90) bar.classList.add('danger');
            else if (pct >= 70) bar.classList.add('warn');
        }
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

    function pluginUrl(path) {
        const apiClient = client();
        if (apiClient && apiClient.getUrl) {
            return apiClient.getUrl(path);
        }

        return path.charAt(0) === '/' ? path : '/' + path;
    }

    function setGuideImages(view) {
        const images = view.querySelectorAll('[data-guide-image]');
        images.forEach(img => {
            const name = img.getAttribute('data-guide-image');
            if (name && !img.src) {
                img.src = pluginUrl('YouTubePlugin/GuideImage/' + encodeURIComponent(name));
            }
        });
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

    // Compares two version strings ("2.0.9.8") numerically. Returns >0 if
    // a > b, <0 if a < b, 0 if equal. Tolerates a leading "v" and missing
    // tail components.
    function compareVersions(a, b) {
        const norm = v => String(v || '').replace(/^v/i, '').split('.').map(n => parseInt(n, 10) || 0);
        const av = norm(a);
        const bv = norm(b);
        const len = Math.max(av.length, bv.length);
        for (let i = 0; i < len; i++) {
            const d = (av[i] || 0) - (bv[i] || 0);
            if (d !== 0) return d;
        }
        return 0;
    }

    // Cache the latest-release lookup briefly in sessionStorage so we don't
    // hammer GitHub on every page open, but still pick up new releases fast.
    function fetchLatestGithubVersion() {
        const cacheKey = 'ytPluginLatestRelease';
        // Only cache positive results — never cache "no release found"
        // (otherwise a single hiccup hides updates for the whole session).
        try {
            const cached = JSON.parse(sessionStorage.getItem(cacheKey) || 'null');
            if (cached && cached.v && (Date.now() - cached.t) < 60 * 1000) {
                return Promise.resolve(cached.v);
            }
        } catch (e) { /* ignore */ }

        const repo = 'eliasbruno124-dev/Emby-Youtube-Plugin';
        const headers = { 'Accept': 'application/vnd.github+json' };
        const opts = { headers, cache: 'no-store' };
        const bust = '?_=' + Date.now();

        // /releases/latest only returns releases marked as "latest". Fall back
        // to /releases?per_page=1 if the tag was published without that flag
        // (or if the user is using a draft / pre-release).
        return fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)
            .then(r => {
                if (!r.ok) {
                    console.log('[YT] /releases/latest returned', r.status);
                    return null;
                }
                return r.json();
            })
            .then(json => json && (json.tag_name || json.name))
            .catch(err => { console.log('[YT] /releases/latest failed', err); return null; })
            .then(tag => {
                if (tag) return tag;
                return fetch('https://api.github.com/repos/' + repo + '/releases' + bust + '&per_page=5', opts)
                    .then(r => r.ok ? r.json() : null)
                    .then(arr => {
                        if (!Array.isArray(arr) || arr.length === 0) return null;
                        // Prefer non-draft, anything else if all are drafts.
                        const pick = arr.find(x => x && !x.draft) || arr[0];
                        return pick.tag_name || pick.name || null;
                    })
                    .catch(err => { console.log('[YT] /releases fallback failed', err); return null; });
            })
            .then(tag => {
                if (!tag) {
                    console.log('[YT] No GitHub release found for update check');
                    return null;
                }
                console.log('[YT] Latest GitHub tag:', tag);
                try { sessionStorage.setItem(cacheKey, JSON.stringify({ v: tag, t: Date.now() })); } catch (e) { }
                return tag;
            });
    }

    function renderPluginCredits(view, apiClient) {
        const versionEl = view.querySelector('#ytPluginVersion');
        const badgeEl = view.querySelector('#ytUpdateBadge');
        if (!versionEl) return;

        const showUpdateBadge = (latest) => {
            if (!badgeEl) return;
            badgeEl.textContent = 'Update available: ' + String(latest).replace(/^v/i, '');
            // Inline overrides win over the HTML's inline display:none.
            badgeEl.style.cssText = 'display:inline-block;padding:2px 10px;border-radius:999px;background:#d12c2c;color:#fff;font-weight:700;font-size:0.8em;letter-spacing:0.02em;';
        };

        const finish = (currentVersion) => {
            versionEl.textContent = currentVersion || 'unknown';
            console.log('[YT] Installed plugin version =', currentVersion);
            fetchLatestGithubVersion().then(latest => {
                if (!latest) return;
                if (!currentVersion) {
                    // Couldn't determine installed version — show the badge
                    // anyway so the user at least sees a release exists.
                    console.log('[YT] No installed version, showing badge for', latest);
                    showUpdateBadge(latest);
                    return;
                }
                const cmp = compareVersions(latest, currentVersion);
                console.log('[YT] Version check installed=' + currentVersion + ' latest=' + latest + ' cmp=' + cmp);
                if (cmp > 0) showUpdateBadge(latest);
            });
        };

        const findPlugin = (plugins) => {
            if (!Array.isArray(plugins)) return null;
            // Match by Id first (case-insensitive). Some Emby builds drop
            // dashes from the GUID, so compare normalized too.
            const wantA = pluginId.toLowerCase();
            const wantB = wantA.replace(/-/g, '');
            const byId = plugins.find(p => {
                const id = ((p && p.Id) || '').toLowerCase();
                return id === wantA || id.replace(/-/g, '') === wantB;
            });
            if (byId) return byId;
            // Fall back to matching by name.
            return plugins.find(p =>
                p && typeof p.Name === 'string'
                && p.Name.toLowerCase().indexOf('youtube') >= 0);
        };

        if (apiClient && typeof apiClient.getInstalledPlugins === 'function') {
            apiClient.getInstalledPlugins().then(plugins => {
                const me = findPlugin(plugins);
                finish(me ? me.Version : null);
            }, err => { console.log('[YT] getInstalledPlugins failed', err); finish(null); });
        } else {
            finish(null);
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
            setGuideImages(this.view);
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
                renderQuota(view, config);
                setDonateButton(view);
                setStatus(view, '');
                loading.hide();
                renderPluginCredits(view, apiClient);
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
            delete config.QuotaUsedToday;
            delete config.QuotaDailyLimit;
            delete config.QuotaResetSeconds;
            delete config.QuotaLifetime;
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
