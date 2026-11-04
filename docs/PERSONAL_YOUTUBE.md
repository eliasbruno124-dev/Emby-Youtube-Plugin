# Personal YouTube settings

Administrators can enable **Personal API keys** and **Personal subscriptions**
separately in the plugin settings. Both are off by default; existing shared keys
and channels stay unchanged.

Users open their avatar menu in Emby Web, choose **App settings**, then
**My YouTube** under their personal settings to save an API key or add channels
by `@handle` or `UC…` channel ID. Version 2.0.12.1 fixes the missing settings entry
in 2.0.12.0. The resulting content uses normal Emby channel folders, including in native clients. Reload the client after the first
refresh. The settings page itself is a web page, not a custom TV settings screen.

**Own user** keeps added channels in that user's My YouTube area. **Shared** also
adds them to the common YouTube catalog. Removing one user's subscription does
not remove a channel that another user or the administrator still follows.
These are plugin subscriptions, not a connection to a Google account.

An enabled personal key is used for that user's area. Without one, the global
key is used. A failing personal key does not silently consume the global key's
quota. The shared catalog always uses the global key. The existing quota panel
estimates global-key requests only.

Disabling either option keeps its saved values but stops using and editing them.
Personal keys are not returned by the settings API. Encrypted profile storage
and its private key remain in Emby's data directory, like other plugin settings;
removing the DLL does not erase those files.

Native visibility is checked against the server's actual query and DTO methods.
Runtime-hook and SQL tests use Emby Server 4.10.2.0, Linux x64. They do not replace
physical TV-client tests. Compatible method structures are accepted
without a machine-specific binary pin; Windows and macOS have not been runtime
tested. Recheck personal browsing after an Emby upgrade. This is separation of
user views, not DRM or protection from the server administrator. Removing the
plugin or upgrading to a runtime where even its presentation hooks no longer
bind can leave existing materialized folders without this visibility filter.

The personal-settings and upload-cache ideas were reviewed from
[lilfire's fork](https://github.com/lilfire/Emby-Youtube-Plugin/tree/25515a35db945842b648aa06af6e9dd992088f2d)
and adapted to this branch's native streaming. Its external download worker and
private media-library machinery are not included.
