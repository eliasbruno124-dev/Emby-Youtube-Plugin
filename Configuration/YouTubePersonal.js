define(['baseView', 'emby-input', 'emby-button'], function (BaseView) {
    'use strict';

    function client() {
        return window.ApiClient || (window.Dashboard && window.Dashboard.getCurrentApiClient && window.Dashboard.getCurrentApiClient());
    }

    function field(view, id) {
        return view.querySelector('#' + id);
    }

    function text(view, id, value) {
        const element = field(view, id);
        if (element) element.textContent = value || '';
    }

    function normalizeUserId(value) {
        return String(value || '').replace(/-/g, '').toLowerCase();
    }

    function context() {
        try {
            const api = client();
            if (!api || typeof api.getCurrentUserId !== 'function' || typeof api.ajax !== 'function' || typeof api.getUrl !== 'function') return null;
            const userId = normalizeUserId(api.getCurrentUserId());
            if (!/^[a-f0-9]{32}$/.test(userId)) return null;
            return { api: api, userId: userId, url: api.getUrl('YouTubePlugin/MySettings') };
        } catch (error) {
            return null;
        }
    }

    function sameContext(left, right) {
        return !!(left && right && left.api === right.api && left.userId === right.userId && left.url === right.url);
    }

    function validChannel(value) {
        return typeof value === 'string' && (/^UC[A-Za-z0-9_-]{22}$/.test(value)
            || /^@[A-Za-z0-9_][A-Za-z0-9_.-]{1,28}[A-Za-z0-9_]$/.test(value));
    }

    function channelIdentity(value) {
        return value.charAt(0) === '@' ? value.toLowerCase() : value;
    }

    function readSettings(payload, expectedUserId) {
        if (typeof payload === 'string') payload = JSON.parse(payload);
        if (!payload || normalizeUserId(payload.UserId) !== expectedUserId
            || !Number.isSafeInteger(payload.Revision) || payload.Revision < 0
            || typeof payload.HasApiKey !== 'boolean'
            || typeof payload.EnablePersonalApiKeys !== 'boolean'
            || typeof payload.EnablePersonalSubscriptions !== 'boolean'
            || (payload.PersonalChannelVisibility !== 'OwnUser' && payload.PersonalChannelVisibility !== 'Shared')
            || !Array.isArray(payload.Channels) || payload.Channels.length > 100 || !payload.Channels.every(validChannel)) {
            throw new Error('Invalid personal settings response');
        }
        // Keep only the public contract, even if a faulty server includes extra fields.
        return {
            Revision: payload.Revision,
            HasApiKey: payload.HasApiKey,
            Channels: Array.from(new Set(payload.Channels)),
            EnablePersonalApiKeys: payload.EnablePersonalApiKeys,
            EnablePersonalSubscriptions: payload.EnablePersonalSubscriptions,
            PersonalChannelVisibility: payload.PersonalChannelVisibility
        };
    }

    return class extends BaseView {
        constructor(view, params) {
            super(view, params);
            this.requestedUserId = normalizeUserId(params && (params.userId || params.userid));
            this.settings = null;
            this.identity = null;
            this.sequence = 0;
            this.active = false;
            this.busy = false;
        }

        onResume(options) {
            super.onResume(options);
            this.active = true;
            this.bindEvents();
            return this.loadData();
        }

        onPause() {
            this.active = false;
            this.sequence++;
            this.reset();
            if (super.onPause) super.onPause.apply(this, arguments);
        }

        bindEvents() {
            if (this.bound) return;
            this.bound = true;
            const view = this.view;
            view.querySelector('.youtubePersonalChannelsForm').addEventListener('submit', event => {
                event.preventDefault();
                this.addChannel();
            });
            view.querySelector('.youtubePersonalKeyForm').addEventListener('submit', event => {
                event.preventDefault();
                this.saveKey();
            });
            field(view, 'btnClearPersonalKey').addEventListener('click', () => this.saveData({ ClearApiKey: true }));
        }

        clearKey() {
            field(this.view, 'txtPersonalApiKey').value = '';
        }

        reset() {
            this.settings = null;
            this.identity = null;
            this.busy = false;
            this.clearKey();
            field(this.view, 'txtPersonalChannel').value = '';
            this.render();
        }

        isCurrent(identity, sequence) {
            if (!this.active || sequence !== this.sequence) return false;
            if (sameContext(identity, context())) return true;
            this.sequence++;
            this.reset();
            text(this.view, 'youtubePersonalStatus', 'Your account or server changed. Reopen this page.');
            return false;
        }

        render() {
            const view = this.view;
            const settings = this.settings;
            const allowChannels = !!(settings && settings.EnablePersonalSubscriptions && !this.busy);
            const allowKeys = !!(settings && settings.EnablePersonalApiKeys && !this.busy);
            ['txtPersonalChannel', 'btnAddPersonalChannel'].forEach(id => { field(view, id).disabled = !allowChannels; });
            ['txtPersonalApiKey', 'btnSavePersonalKey'].forEach(id => { field(view, id).disabled = !allowKeys; });
            field(view, 'btnClearPersonalKey').disabled = !allowKeys || !settings.HasApiKey;
            text(view, 'personalKeyPolicy', !settings ? '' : settings.EnablePersonalApiKeys
                ? 'Your key is private. Without one, the server key is used.'
                : 'Personal API keys are disabled by your administrator. Saved keys are kept but not used.');
            text(view, 'personalKeyStatus', !settings ? '' : settings.HasApiKey ? 'A personal key is saved.' : 'No personal key saved.');
            text(view, 'personalChannelPolicy', !settings ? '' : !settings.EnablePersonalSubscriptions
                ? 'Personal channels are disabled by your administrator. Your saved list is kept.'
                : settings.PersonalChannelVisibility === 'Shared'
                    ? 'Your channels are visible to all users. Only you can change your list.'
                    : 'Your channel list is visible only to you.');
            const list = field(view, 'personalChannelsList');
            while (list.firstChild) list.removeChild(list.firstChild);
            if (!settings) return;
            if (!settings.Channels.length) {
                const empty = document.createElement('p');
                empty.className = 'fieldDescription';
                empty.textContent = 'No channels added yet.';
                list.appendChild(empty);
            }
            settings.Channels.forEach(channel => {
                const row = document.createElement('div');
                row.className = 'personalChannel';
                const label = document.createElement('span');
                label.textContent = channel;
                row.appendChild(label);
                const remove = document.createElement('button');
                remove.type = 'button';
                remove.className = 'raised emby-button';
                remove.textContent = 'Remove';
                remove.setAttribute('aria-label', 'Remove ' + channel);
                remove.disabled = !allowChannels;
                remove.addEventListener('click', () => {
                    if (this.settings) this.saveData({ Channels: this.settings.Channels.filter(item => item !== channel) });
                });
                row.appendChild(remove);
                list.appendChild(row);
            });
        }

        loadData() {
            const sequence = ++this.sequence;
            this.reset();
            const identity = context();
            if (!identity) {
                text(this.view, 'youtubePersonalStatus', 'Sign in to manage your YouTube settings.');
                return Promise.resolve();
            }
            if (this.requestedUserId && this.requestedUserId !== identity.userId) {
                text(this.view, 'youtubePersonalStatus', 'Open your own user settings to manage your YouTube channels and key.');
                return Promise.resolve();
            }
            this.identity = identity;
            this.busy = true;
            text(this.view, 'youtubePersonalStatus', 'Loading…');
            return Promise.resolve().then(() => {
                if (!this.isCurrent(identity, sequence)) return null;
                return identity.api.ajax({ type: 'GET', url: identity.url, dataType: 'json' });
            }).then(payload => {
                if (!this.isCurrent(identity, sequence)) return;
                this.settings = readSettings(payload, identity.userId);
                this.busy = false;
                this.clearKey();
                this.render();
                text(this.view, 'youtubePersonalStatus', '');
            }).catch(() => {
                if (!this.isCurrent(identity, sequence)) return;
                this.reset();
                text(this.view, 'youtubePersonalStatus', 'Could not load your settings. Reopen this page to retry.');
            });
        }

        addChannel() {
            const channel = field(this.view, 'txtPersonalChannel').value.trim();
            if (!validChannel(channel)) {
                text(this.view, 'youtubePersonalStatus', 'Enter a UC channel ID or an @handle (3–30 letters, numbers, dots, hyphens or underscores).');
                return Promise.resolve();
            }
            if (!this.settings) return Promise.resolve();
            if (this.settings.Channels.some(item => channelIdentity(item) === channelIdentity(channel))) {
                text(this.view, 'youtubePersonalStatus', 'This channel is already in your list.');
                return Promise.resolve();
            }
            if (this.settings.Channels.length >= 100) {
                text(this.view, 'youtubePersonalStatus', 'You can save up to 100 channels. Remove one before adding another.');
                return Promise.resolve();
            }
            return this.saveData({ Channels: this.settings.Channels.concat(channel) });
        }

        saveKey() {
            const key = field(this.view, 'txtPersonalApiKey').value.trim();
            if (!key) {
                text(this.view, 'youtubePersonalStatus', 'Enter a new key, or choose Remove key.');
                return Promise.resolve();
            }
            if (!/^[A-Za-z0-9_-]{1,256}$/.test(key)) {
                this.clearKey();
                text(this.view, 'youtubePersonalStatus', 'Enter a valid YouTube Data API key.');
                return Promise.resolve();
            }
            return this.saveData({ ApiKey: key });
        }

        saveData(change) {
            const identity = this.identity;
            if (!this.isCurrent(identity, this.sequence) || !this.settings || this.busy) return Promise.resolve();
            const hasChannels = Object.prototype.hasOwnProperty.call(change, 'Channels');
            const hasKey = Object.prototype.hasOwnProperty.call(change, 'ApiKey');
            const clearKey = change.ClearApiKey === true;
            if ((hasChannels && !this.settings.EnablePersonalSubscriptions)
                || ((hasKey || clearKey) && !this.settings.EnablePersonalApiKeys)) return Promise.resolve();
            // Separate actions send only the capability they mutate. Disabled values are preserved.
            const body = { ExpectedRevision: this.settings.Revision };
            if (hasChannels) body.Channels = change.Channels;
            if (hasKey) body.ApiKey = change.ApiKey;
            if (clearKey) body.ClearApiKey = true;
            if (!hasChannels && !hasKey && !clearKey) return Promise.resolve();
            const sequence = ++this.sequence;
            this.busy = true;
            this.clearKey();
            this.render();
            text(this.view, 'youtubePersonalStatus', 'Saving…');
            return Promise.resolve().then(() => {
                if (!this.isCurrent(identity, sequence)) return null;
                return identity.api.ajax({
                    type: 'POST', url: identity.url, data: JSON.stringify(body), contentType: 'application/json', dataType: 'json'
                });
            }).then(payload => {
                if (!this.isCurrent(identity, sequence)) return;
                this.settings = readSettings(payload, identity.userId);
                this.busy = false;
                this.clearKey();
                if (hasChannels) field(this.view, 'txtPersonalChannel').value = '';
                this.render();
                text(this.view, 'youtubePersonalStatus', 'Saved.');
            }).catch(error => {
                if (!this.isCurrent(identity, sequence)) return;
                this.reset();
                const status = Number(error && (error.status || error.statusCode));
                // A conflict or changed permission needs a fresh revision, not a blind retry.
                if (status === 409 || status === 403 || status === 401) {
                    text(this.view, 'youtubePersonalStatus', 'Your settings or permissions changed. Reopen this page before saving again.');
                } else {
                    text(this.view, 'youtubePersonalStatus', 'Could not save your settings. Reopen this page to check before trying again.');
                }
            });
        }
    };
});
