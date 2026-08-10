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
        const seen = new Set();
        const result = [];

        entries.forEach(item => {
            if (!seen.has(item)) {
                seen.add(item);
                result.push(item);
            }
        });

        return result;
    }

    function setConfigurationControlsDisabled(view, disabled) {
        const form = view.querySelector('.youtubeConfigurationForm');
        if (!form) {
            return;
        }
        form.querySelectorAll('input, select, textarea, button').forEach(control => {
            control.disabled = !!disabled;
        });
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

    function renderQuotaBucket(view, prefix, usedValue, limitValue, lifetimeValue, unit) {
        const used = Number(usedValue) || 0;
        const limit = Number(limitValue) || 1;
        const lifetime = Number(lifetimeValue) || 0;
        const pct = limit > 0 ? Math.min(100, Math.max(0, (used / limit) * 100)) : 0;

        setText(view, prefix + 'Number', formatNumber(used));
        setText(view, prefix + 'Total', formatNumber(limit));
        setText(view, prefix + 'Percent', pct.toFixed(1) + '%');
        setText(view, prefix + 'Lifetime', 'Locally tracked: ' + formatNumber(lifetime) + ' ' + unit);

        const bar = field(view, prefix + 'Bar');
        const fill = field(view, prefix + 'BarFill');
        if (fill) fill.style.width = pct.toFixed(2) + '%';
        if (bar) {
            bar.classList.remove('warn', 'danger');
            if (pct >= 90) bar.classList.add('danger');
            else if (pct >= 70) bar.classList.add('warn');
        }
    }

    function renderQuota(view, config) {
        renderQuotaBucket(
            view,
            'quotaSearch',
            config.QuotaSearchCallsToday,
            config.QuotaSearchDailyLimit || 100,
            config.QuotaTotalSearchCalls,
            'calls');
        renderQuotaBucket(
            view,
            'quotaOther',
            config.QuotaOtherUnitsToday,
            config.QuotaOtherDailyLimit || 10000,
            config.QuotaTotalOtherUnits,
            'units');

        const resetSec = Number(config.QuotaResetSeconds) || 0;
        const reset = field(view, 'quotaReset');
        if (reset) {
            reset.textContent = '⏱ Reset in ' + formatResetCountdown(resetSec);
            reset.classList.toggle('quotaResetWarn', resetSec > 0 && resetSec < 1800);
        }
    }

    function classifySavedItem(item) {
        if (item.indexOf('@') === 0) {
            return item.length > 1 && !/\s/.test(item) ? 'Handle' : 'Invalid ID';
        }
        if (item.indexOf('UC') === 0) {
            return /^UC[A-Za-z0-9_-]{22}$/.test(item) ? 'Channel' : 'Invalid ID';
        }
        if (item.indexOf('WL') === 0) {
            return 'Private / unsupported';
        }
        if (item.indexOf('PL') === 0 || item.indexOf('UU') === 0 || item.indexOf('OL') === 0) {
            return /^(PL|UU|OL)[A-Za-z0-9_-]{19,}$/.test(item) ? 'Playlist' : 'Invalid ID';
        }
        return 'Search';
    }

    function classifyAutoPlaylistItem(item) {
        if (item.indexOf('WL') === 0) {
            return 'Private / unsupported';
        }
        if (/^(PL|UU|OL)[A-Za-z0-9_-]{19,}$/.test(item)) {
            return 'Playlist';
        }
        return 'Invalid ID';
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
            if (name && !img.getAttribute('src')) {
                img.src = pluginUrl('YouTubePlugin/GuideImage/' + encodeURIComponent(name));
            }
        });
    }

    function populateTrendingRegions(view, regionResult, selectedValue) {
        const select = field(view, 'selectTrendingRegion');
        if (!select) {
            return false;
        }

        const selected = String(selectedValue || '').trim().toUpperCase();
        const lookupSucceeded = !!(regionResult && regionResult.LookupSucceeded);
        const regions = (regionResult && regionResult.Regions) || [];
        select.innerHTML = '';

        const defaultOption = document.createElement('option');
        defaultOption.value = '';
        defaultOption.textContent = 'YouTube default';
        select.appendChild(defaultOption);

        const normalized = Array.isArray(regions) ? regions : [];
        normalized.forEach(region => {
            const option = document.createElement('option');
            option.value = region.Code;
            option.textContent = region.Name + ' (' + region.Code + ')';
            select.appendChild(option);
        });

        const selectedIsSupported = normalized.some(region => region.Code === selected);
        if (selected && !selectedIsSupported && !lookupSucceeded) {
            const savedOption = document.createElement('option');
            savedOption.value = selected;
            savedOption.textContent = 'Saved value (' + selected + ')';
            select.appendChild(savedOption);
        }
        select.value = selectedIsSupported || !lookupSucceeded ? selected : '';
        return !!selected && lookupSucceeded && !selectedIsSupported;
    }

    function fetchTrendingRegions(apiClient) {
        if (!apiClient || typeof apiClient.ajax !== 'function') {
            return Promise.resolve({ LookupSucceeded: false, Regions: [] });
        }

        return apiClient.ajax({
            type: 'GET',
            url: apiClient.getUrl('YouTubePlugin/Regions'),
            dataType: 'json'
        }).then(payload => {
            if (typeof payload === 'string') {
                try { payload = JSON.parse(payload); }
                catch (e) { return { LookupSucceeded: false, Regions: [] }; }
            }

            const source = (payload && (payload.Regions || payload.regions)) || [];
            if (!Array.isArray(source)) {
                return { LookupSucceeded: false, Regions: [] };
            }
            const seen = new Set();
            const regions = source.map(region => ({
                Code: String((region && (region.Code || region.code)) || '').trim().toUpperCase(),
                Name: String((region && (region.Name || region.name)) || '').trim()
            })).filter(region => {
                if (!/^[A-Z]{2}$/.test(region.Code) || !region.Name || seen.has(region.Code)) {
                    return false;
                }
                seen.add(region.Code);
                return true;
            }).sort((a, b) => a.Name.localeCompare(b.Name));
            return {
                LookupSucceeded: !!(payload && (payload.LookupSucceeded || payload.lookupSucceeded))
                    && regions.length > 0,
                Regions: regions
            };
        }).catch(error => {
            console.log('[YT] Supported-region lookup failed:', error);
            return { LookupSucceeded: false, Regions: [] };
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

                if (target === 'setupGuidePage') {
                    setGuideImages(view);
                }
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
                    console.log('[YT] Installed version unavailable; update comparison skipped. Latest published tag:', latest);
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
            this._saveInFlight = false;
            this._pendingSave = null;
            this._saveRevision = 0;
            this._loadRevision = 0;
            this._hasLoadedConfiguration = false;
            this._regionResult = { LookupSucceeded: false, Regions: [] };
            this._regionLookupRevision = 0;
            this._regionLookupApiKey = '';
        }

        onResume(options) {
            super.onResume(options);
            populateTrendingRegions(
                this.view,
                this._regionResult,
                value(this.view, 'selectTrendingRegion'));
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
                    if (this._autoSaveReady) {
                        this.saveData(view);
                    }
                    return false;
                });
                // Auto-save: there is no Save button. Any control change persists
                // immediately (debounced via scheduleAutoSave). Programmatic value
                // changes during loadData don't fire these events, and the
                // _autoSaveReady guard blocks saves until the initial load is done.
                form.addEventListener('change', () => this.scheduleAutoSave(view));
                form.addEventListener('input', (e) => {
                    // The "add entry" boxes aren't persisted config fields —
                    // their text only matters once Add is pressed — so typing
                    // in them must not schedule a save.
                    const id = e.target && e.target.id;
                    if (id === 'txtSavedItemEntry' || id === 'txtWatchLaterEntry') {
                        return;
                    }
                    this.scheduleAutoSave(view);
                });
            }

            this.bindEntryControls(
                view,
                'txtSavedItemEntry',
                'btnAddSavedItem',
                'savedItemsList',
                () => this.savedItems,
                value => { this.savedItems = value; },
                item => classifySavedItem(item),
                item => {
                    const kind = classifySavedItem(item);
                    return kind !== 'Invalid ID' && kind !== 'Private / unsupported';
                },
                'Private or malformed YouTube IDs were not added.');

            this.bindEntryControls(
                view,
                'txtWatchLaterEntry',
                'btnAddWatchLaterItem',
                'watchLaterItemsList',
                () => this.watchLaterItems,
                value => { this.watchLaterItems = value; },
                item => classifyAutoPlaylistItem(item),
                item => classifyAutoPlaylistItem(item) === 'Playlist',
                'Only public or unlisted PL, UU or OL playlist IDs can be auto-refreshed.');
        }

        bindEntryControls(view, inputId, buttonId, listId, getItems, setItems, classify, validate, invalidMessage) {
            const input = field(view, inputId);
            const button = field(view, buttonId);
            const add = () => {
                const newItems = splitEntries(value(view, inputId));
                if (newItems.length === 0) {
                    return;
                }

                const acceptedItems = validate ? newItems.filter(validate) : newItems;
                if (acceptedItems.length !== newItems.length) {
                    setStatus(view, invalidMessage || 'Some entries were not valid.');
                    showToast('error', invalidMessage || 'Some entries were not valid');
                }
                if (acceptedItems.length === 0) {
                    return;
                }

                setItems(uniqueEntries(getItems().concat(acceptedItems)));
                setValue(view, inputId, '');
                this.renderEntryList(view, listId, getItems, setItems, classify);
                this.syncEntryFields(view);
                this.scheduleAutoSave(view);
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
                    this.scheduleAutoSave(view);
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
                item => classifyAutoPlaylistItem(item));
        }

        refreshTrendingRegions(view, apiClient, loadRevision, apiKeyAtRequest) {
            const lookupRevision = ++this._regionLookupRevision;
            const selectedBeforeLookup = value(view, 'selectTrendingRegion');
            this._regionLookupApiKey = String(apiKeyAtRequest || '').trim();

            return fetchTrendingRegions(apiClient).then(regionResult => {
                if (lookupRevision !== this._regionLookupRevision
                    || (loadRevision != null && loadRevision !== this._loadRevision)
                    || value(view, 'selectTrendingRegion') !== selectedBeforeLookup) {
                    return;
                }

                if (regionResult.LookupSucceeded) {
                    this._regionResult = regionResult;
                }

                // A transient refresh failure must not discard a previously
                // validated region list from this page instance.
                const displayResult = regionResult.LookupSucceeded
                    ? regionResult
                    : this._regionResult;
                const resetUnsupported = populateTrendingRegions(
                    view,
                    displayResult,
                    selectedBeforeLookup);
                if (resetUnsupported) {
                    setStatus(view, 'The saved region is not supported by YouTube and was reset to the default.');
                    this.scheduleAutoSave(view);
                }
            });
        }

        loadData(view) {
            this._autoSaveReady = false;
            setConfigurationControlsDisabled(view, true);
            const apiClient = client();
            if (!apiClient) {
                if (this._autoSaveTimer) {
                    clearTimeout(this._autoSaveTimer);
                    this._autoSaveTimer = null;
                }
                setStatus(view, 'Could not find the Emby API client.');
                return;
            }

            // Preserve a debounced edit before reloading, then wait until the
            // serialized save queue is empty. A stale GET must never repaint
            // the form while an older POST is still being committed.
            if (this._autoSaveTimer) {
                clearTimeout(this._autoSaveTimer);
                this._autoSaveTimer = null;
                this.saveData(view);
            }
            const loadRevision = ++this._loadRevision;
            const requestWhenIdle = () => {
                if (loadRevision !== this._loadRevision) {
                    return;
                }
                if (this._saveInFlight || this._pendingSave) {
                    setTimeout(requestWhenIdle, 100);
                    return;
                }

                const saveRevisionAtRequest = this._saveRevision;
                loading.show();
                apiClient.getPluginConfiguration(pluginId).then(loadedConfig => {
                    if (loadRevision !== this._loadRevision) {
                        loading.hide();
                        return;
                    }
                    if (this._saveInFlight
                        || this._pendingSave
                        || saveRevisionAtRequest !== this._saveRevision) {
                        loading.hide();
                        setTimeout(requestWhenIdle, 100);
                        return;
                    }
                    this.config = loadedConfig || {};
                    const config = this.config;

                    setValue(view, 'txtApiKey', config.ApiKey);
                    this.savedItems = uniqueEntries(splitEntries(config.SavedItems));
                    this.watchLaterItems = uniqueEntries(splitEntries(config.WatchLaterPlaylist));
                    this.syncEntryFields(view);
                    this.renderAllEntryLists(view);
                    setChecked(view, 'chkShowRootFoldersAtTopLevel', !!config.ShowRootFoldersAtTopLevel);
                    setChecked(view, 'chkShowTrending', config.ShowTrending !== false);
                    setChecked(view, 'chkShowCategories', config.ShowCategories !== false);
                    setChecked(view, 'chkShowShorts', config.HideShorts === true ? false : config.ShowShorts !== false);
                    const resetFromKnownRegions = populateTrendingRegions(
                        view,
                        this._regionResult,
                        config.TrendingRegion);
                    setValue(view, 'selectTrendingCategory', config.TrendingCategory === '0' ? '' : (config.TrendingCategory || ''));
                    setChecked(view, 'chkShowLikeCount', config.ShowLikeCount !== false);
                    setChecked(view, 'chkShowCommentCount', !!config.ShowCommentCount);
                    setValue(view, 'selectChannelSortBy', config.ChannelSortBy || 'date');
                    setValue(view, 'numMaxChannelVideos', config.MaxChannelVideos || 50);
                    setValue(view, 'numMaxSearchVideos', config.MaxSearchVideos || 50);
                    setValue(view, 'numWatchLaterPollMinutes', config.WatchLaterPollMinutes || 3);
                    renderQuota(view, config);
                    setDonateButton(view);
                    setStatus(view, '');
                    loading.hide();
                    renderPluginCredits(view, apiClient);
                    // Initial values are in place; enable auto-save for real edits.
                    this._hasLoadedConfiguration = true;
                    this._autoSaveReady = true;
                    setConfigurationControlsDisabled(view, false);

                    if (resetFromKnownRegions) {
                        setStatus(view, 'The saved region is not supported by YouTube and was reset to the default.');
                        this.scheduleAutoSave(view);
                    }

                    // Region discovery is deliberately independent of the
                    // main config load. A slow or unreachable YouTube API must
                    // never keep the whole settings page disabled.
                    this.refreshTrendingRegions(view, apiClient, loadRevision, config.ApiKey);
                }, error => {
                    if (loadRevision !== this._loadRevision) {
                        loading.hide();
                        return;
                    }
                    loading.hide();
                    console.error('Error loading YouTube configuration:', error);
                    this._autoSaveReady = this._hasLoadedConfiguration;
                    setConfigurationControlsDisabled(view, !this._hasLoadedConfiguration);
                    setStatus(
                        view,
                        this._hasLoadedConfiguration
                            ? 'Could not refresh plugin settings; keeping the last loaded values.'
                            : 'Could not load plugin settings. Reopen this page to retry.');
                    showToast('error', 'Could not load YouTube settings');
                });
            };
            requestWhenIdle();
        }

        saveData(view) {
            const apiClient = client();
            if (!apiClient) {
                setStatus(view, 'Could not find the Emby API client.');
                return;
            }

            const config = Object.assign({}, this.config || {});
            config.ApiKey = value(view, 'txtApiKey').trim();
            this.syncEntryFields(view);
            config.SavedItems = this.savedItems.join(', ');
            config.WatchLaterPlaylist = this.watchLaterItems.join(', ');
            config.ShowRootFoldersAtTopLevel = checked(view, 'chkShowRootFoldersAtTopLevel');
            config.ShowTrending = checked(view, 'chkShowTrending');
            config.ShowCategories = checked(view, 'chkShowCategories');
            config.ShowShorts = checked(view, 'chkShowShorts');
            config.HideShorts = false;
            config.TrendingRegion = value(view, 'selectTrendingRegion').trim().toUpperCase();
            config.TrendingCategory = value(view, 'selectTrendingCategory').trim();
            config.ShowLikeCount = checked(view, 'chkShowLikeCount');
            config.ShowCommentCount = checked(view, 'chkShowCommentCount');
            config.ChannelSortBy = value(view, 'selectChannelSortBy') || 'date';
            config.MaxChannelVideos = numberValue(view, 'numMaxChannelVideos', 50, 1, 150);
            config.MaxSearchVideos = numberValue(view, 'numMaxSearchVideos', 50, 1, 150);
            config.WatchLaterPollMinutes = numberValue(view, 'numWatchLaterPollMinutes', 3, 1, 60);
            config.Donate = defaultDonateUrl;
            delete config.ShowRecentlyAdded;
            delete config.ShowLiveFolders;
            delete config.RecentlyAddedPerChannel;
            delete config.QuotaStatus;
            delete config.QuotaUsedToday;
            delete config.QuotaDailyLimit;
            delete config.QuotaResetSeconds;
            delete config.QuotaLifetime;
            delete config.QuotaSearchCallsToday;
            delete config.QuotaSearchDailyLimit;
            delete config.QuotaOtherUnitsToday;
            delete config.QuotaOtherDailyLimit;
            delete config.QuotaTotalSearchCalls;
            delete config.QuotaTotalOtherUnits;
            delete config.ShortsEnabled;

            this._pendingSave = {
                apiClient,
                config,
                revision: ++this._saveRevision,
                view
            };
            this.flushSaveQueue();
        }

        flushSaveQueue() {
            if (this._saveInFlight || !this._pendingSave) {
                return;
            }

            const pending = this._pendingSave;
            this._pendingSave = null;
            this._saveInFlight = true;

            // Save through the plugin's own endpoint. Requests are serialized and
            // edits made while one is in flight collapse into the newest snapshot,
            // so a slow older response can never overwrite newer settings.
            const view = pending.view;
            setStatus(view, 'Saving…');
            pending.apiClient.ajax({
                type: 'POST',
                url: pending.apiClient.getUrl('YouTubePlugin/SaveConfiguration'),
                data: JSON.stringify(pending.config),
                contentType: 'application/json'
            }).then(() => {
                this.config = pending.config;
                setDonateButton(view);
                if (!this._pendingSave && pending.revision === this._saveRevision) {
                    setStatus(view, 'Saved automatically.');
                    const savedApiKey = String(pending.config.ApiKey || '').trim();
                    if (savedApiKey !== this._regionLookupApiKey) {
                        this.refreshTrendingRegions(view, pending.apiClient, null, savedApiKey);
                    }
                }
            }, error => {
                console.error('Error saving YouTube configuration:', error);
                if (!this._pendingSave && pending.revision === this._saveRevision) {
                    setStatus(view, 'Could not save plugin settings.');
                    showToast('error', 'Could not save YouTube settings');
                }
            }).then(() => {
                this._saveInFlight = false;
                this.flushSaveQueue();
            }, error => {
                console.error('Error finalizing YouTube configuration save:', error);
                this._saveInFlight = false;
                this.flushSaveQueue();
            });
        }

        scheduleAutoSave(view) {
            // Debounce so typing in a text field doesn't fire a save per keystroke.
            if (!this._autoSaveReady) {
                return;
            }
            if (this._autoSaveTimer) {
                clearTimeout(this._autoSaveTimer);
            }
            this._autoSaveTimer = setTimeout(() => {
                this._autoSaveTimer = null;
                this.saveData(view);
            }, 600);
        }
    };
});
