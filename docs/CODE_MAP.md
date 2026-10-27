# Code map

Generated syntax inventory. Start with [the reviewed change guide](CODE_GUIDE.md) and [maintenance commands](CODE_MAP_MAINTENANCE.md).

Run `node tools/code-map.mjs check` before trusting this snapshot. A generated map is not a compiler-bound call graph or a runtime capability test.

Scope: 97 files; 7341 syntax declarations/HTML anchors.

C# includes namespace/type/member/local/parameter declarations and anonymous functions. JavaScript includes functions, classes/methods, bindings and parameters. HTML records IDs, script/style blocks and resource references. Compiler-synthesized members and external Emby bundle code are not repository declarations.

C# constant JavaScript payload coordinates point to the owning constant; decoded payload coordinates are retained in JSON. Other dashboard replacement strings are fragments owned by their C# patch methods.

| File | Role | Entries |
|---|---|---:|
| [.gitattributes](#file-618cd5b83d62060b) | Repository support | 0 |
| [.gitignore](#file-bc37d034bad56458) | Repository support | 0 |
| [AGENTS.md](#file-a54ff182c7e8acf5) | Documentation/resource | 0 |
| [ChannelContentFlags.cs](#file-3fbcb901338e79fb) | Plugin Compile source (SDK default) | 29 |
| [Configuration/YouTubeConfiguration.html](#file-3265de6859f23663) | EmbeddedResource | 50 |
| [Configuration/YouTubeConfiguration.js](#file-44888eb7c4d56aef) | EmbeddedResource | 357 |
| [Configuration/guide/step1.png](#file-5f31de67e5ef8dbe) | EmbeddedResource | 0 |
| [Configuration/guide/step2.png](#file-ed44b448e7d3eaa3) | EmbeddedResource | 0 |
| [Configuration/guide/step34.png](#file-8b116519690d79c6) | EmbeddedResource | 0 |
| [Configuration/guide/step5.png](#file-d5cc51fd66e047cb) | EmbeddedResource | 0 |
| [Configuration/guide/step6.png](#file-6e5d5c4e6b423e11) | EmbeddedResource | 0 |
| [Configuration/guide/step7.png](#file-795fc5e8d5423b25) | EmbeddedResource | 0 |
| [Configuration/guide/step8.png](#file-7e925ff01358802b) | EmbeddedResource | 0 |
| [DashboardYouTubePlayerInterceptor.cs](#file-d1ed9d5e47f4e374) | Plugin Compile source (SDK default) | 269 |
| [EmbeddedDependencyLoader.cs](#file-07323f6dc3f3a1d5) | Plugin Compile source (SDK default) | 23 |
| [Emby-Youtube-Plugin.sln](#file-8194c8953ddc1453) | Build definition | 0 |
| [Emby.YouTubePlugin.csproj](#file-8c9377fd54ef39d4) | Build definition | 0 |
| [FolderIcons.cs](#file-072a7f211747c67e) | Plugin Compile source (SDK default) | 15 |
| [GuideImageService.cs](#file-24d40baec16f6f30) | Plugin Compile source (SDK default) | 18 |
| [LICENSE](#file-c693279643b8cd5d) | Repository support | 0 |
| [ManagedResolver/Compatibility.cs](#file-813f15c5a6f705bb) | Plugin Compile source (SDK default) | 64 |
| [ManagedResolver/Integration.md](#file-69aa28ead9070c54) | Documentation/resource | 0 |
| [ManagedResolver/License.txt](#file-8c416880d43abc64) | EmbeddedResource | 0 |
| [ManagedResolver/Upstream.json](#file-273e736a61de1668) | Repository support | 0 |
| [ManagedResolver/vendored/Bridge/IStreamData.cs](#file-88ffe9d079ed07b0) | Plugin Compile source (SDK default) | 36 |
| [ManagedResolver/vendored/Bridge/PlayerResponse.cs](#file-55b1644a548a74e8) | Plugin Compile source (SDK default) | 89 |
| [ManagedResolver/vendored/Bridge/ThumbnailData.cs](#file-0cd0bad381695bc3) | Plugin Compile source (SDK default) | 6 |
| [ManagedResolver/vendored/Exceptions/VideoUnavailableException.cs](#file-95c161f9b0da96af) | Plugin Compile source (SDK default) | 3 |
| [ManagedResolver/vendored/Exceptions/VideoUnplayableException.cs](#file-f9669ca26d62c40d) | Plugin Compile source (SDK default) | 3 |
| [ManagedResolver/vendored/Exceptions/YoutubeExplodeException.cs](#file-697e007577a4a1c3) | Plugin Compile source (SDK default) | 3 |
| [ManagedResolver/vendored/Utils/Json.cs](#file-2db2dc2c42455d05) | Plugin Compile source (SDK default) | 22 |
| [ManagedResolver/vendored/Utils/Protobuf.cs](#file-98e8c5ef86faa4ef) | Plugin Compile source (SDK default) | 34 |
| [ManagedResolver/vendored/Utils/UrlEx.cs](#file-e3e506b64848f0af) | Plugin Compile source (SDK default) | 36 |
| [ManagedResolver/vendored/Videos/VideoController.cs](#file-3d45031c9e99dad6) | Plugin Compile source (SDK default) | 28 |
| [ManagedResolver/vendored/Videos/VideoId.cs](#file-c4d93fe674f77ff2) | Plugin Compile source (SDK default) | 40 |
| [PlaybackIntentInterceptor.cs](#file-6ff472c354325e8c) | Plugin Compile source (SDK default) | 475 |
| [Plugin.cs](#file-5a2f4e90167632d0) | Plugin Compile source (SDK default) | 78 |
| [PluginConfiguration.cs](#file-224cbfdbaba3a683) | Plugin Compile source (SDK default) | 87 |
| [PluginEntryPoint.cs](#file-d27f2eafc5169b60) | Plugin Compile source (SDK default) | 693 |
| [PluginFileLog.cs](#file-adad5fbc257fb6bf) | Plugin Compile source (SDK default) | 19 |
| [QuotaTracker.cs](#file-942e26b5e5820665) | Plugin Compile source (SDK default) | 113 |
| [README.md](#file-b335630551682c19) | Documentation/resource | 0 |
| [YouTubeAdaptiveSourceBudget.cs](#file-03de83be30141261) | Plugin Compile source (SDK default) | 195 |
| [YouTubeApi.cs](#file-7011a74936995935) | Plugin Compile source (SDK default) | 377 |
| [YouTubeCaptionMetadata.cs](#file-914fdb518473aaa9) | C# source excluded from plugin Compile | 102 |
| [YouTubeCaptionNormalizer.cs](#file-179571d4992d8567) | Plugin Compile source (SDK default) | 89 |
| [YouTubeCaptionOffGuard.cs](#file-1dd14218ee040c7f) | Plugin Compile source (SDK default) | 121 |
| [YouTubeChannel.Aggregates.cs](#file-f2ed4ae35969d6d0) | Plugin Compile source (SDK default) | 27 |
| [YouTubeChannel.DirectPlayback.cs](#file-d1cc636f42c38fb0) | Plugin Compile source (SDK default) | 363 |
| [YouTubeChannel.Enrichment.cs](#file-d51ff2da196617b0) | Plugin Compile source (SDK default) | 130 |
| [YouTubeChannel.Folders.cs](#file-3cf86b8d83708a69) | Plugin Compile source (SDK default) | 108 |
| [YouTubeChannel.Live.cs](#file-fd75d22d5169819c) | Plugin Compile source (SDK default) | 30 |
| [YouTubeChannel.Mapping.cs](#file-b5a9b427d9c6cb61) | Plugin Compile source (SDK default) | 95 |
| [YouTubeChannel.MediaFolders.cs](#file-11faad7ba4aab64e) | Plugin Compile source (SDK default) | 130 |
| [YouTubeChannel.Shorts.cs](#file-b6bd071cf0a75be9) | Plugin Compile source (SDK default) | 148 |
| [YouTubeChannel.State.cs](#file-bd6961656d4c3822) | Plugin Compile source (SDK default) | 52 |
| [YouTubeChannel.cs](#file-aa77154cbf6e5636) | Plugin Compile source (SDK default) | 47 |
| [YouTubeChapterSync.cs](#file-90d85cea05c050fd) | Plugin Compile source (SDK default) | 75 |
| [YouTubeConfigSaveService.cs](#file-afb9dc3e97044d5d) | Plugin Compile source (SDK default) | 117 |
| [YouTubeDirectHttp.cs](#file-1922be3cbc99a179) | Plugin Compile source (SDK default) | 181 |
| [YouTubeDirectManifest.cs](#file-5a4b4574992dd577) | Plugin Compile source (SDK default) | 148 |
| [YouTubeDirectMetadata.cs](#file-5ec1b64fd6d6c34f) | Plugin Compile source (SDK default) | 136 |
| [YouTubeDirectModels.cs](#file-db78dbee2ac97cd9) | Plugin Compile source (SDK default) | 5 |
| [YouTubeDirectQualityPolicy.cs](#file-2b6757d8b9192715) | Plugin Compile source (SDK default) | 12 |
| [YouTubeDirectRangeProxy.cs](#file-538c7a88a964bb3f) | Plugin Compile source (SDK default) | 198 |
| [YouTubeDirectSubtitles.cs](#file-8dd75b168cf4c866) | Plugin Compile source (SDK default) | 198 |
| [YouTubeEmbed.html](#file-39b78aa97731c824) | EmbeddedResource | 71 |
| [YouTubeEpisodeMetadataProvider.cs](#file-973493e8a800cabd) | Plugin Compile source (SDK default) | 16 |
| [YouTubeExternalId.cs](#file-0fc93e92e0c0f80b) | Plugin Compile source (SDK default) | 7 |
| [YouTubeHomeSectionManager.cs](#file-2bdd885ca109f825) | Plugin Compile source (SDK default) | 415 |
| [YouTubeHttpClientFactory.cs](#file-086dc9043b3c2586) | Plugin Compile source (SDK default) | 26 |
| [YouTubeImageProvider.cs](#file-7d33b8b54f4ebb57) | Plugin Compile source (SDK default) | 84 |
| [YouTubeManagedHttp.cs](#file-bc134ac40739676b) | Plugin Compile source (SDK default) | 88 |
| [YouTubeManagedResolver.cs](#file-d1109fcde8c29d7a) | Plugin Compile source (SDK default) | 182 |
| [YouTubeRequestBackoff.cs](#file-7a9c7096d206cd40) | Plugin Compile source (SDK default) | 27 |
| [YouTubeSessionCommandRouter.cs](#file-8ca449cb8064e640) | Plugin Compile source (SDK default) | 36 |
| [YouTubeSortNameRepairer.cs](#file-ff64b0c5af701f8e) | Plugin Compile source (SDK default) | 57 |
| [YouTubeSourceBudget.cs](#file-28aab84e271eec1a) | Plugin Compile source (SDK default) | 92 |
| [YouTubeStreamResolver.cs](#file-a5337fff5b60437f) | Plugin Compile source (SDK default) | 146 |
| [docs/CODE_GUIDE.md](#file-8a63fe1dcd96b301) | Documentation/resource | 0 |
| [docs/CODE_MAP_MAINTENANCE.md](#file-24c05485c45ad84d) | Documentation/resource | 0 |
| [docs/code-map.flows.json](#file-31e2cff7937d621c) | Documentation/resource | 0 |
| [docs/screenshots/.gitkeep](#file-51ccab4fa46b4e7f) | Documentation/resource | 0 |
| [docs/screenshots/Root Folder.png](#file-73dd066abffa11bb) | Documentation/resource | 0 |
| [docs/screenshots/channel folder.png](#file-5a6060bd29b474db) | Documentation/resource | 0 |
| [docs/screenshots/setup 1.png](#file-51471e3d6d535ee4) | Documentation/resource | 0 |
| [docs/screenshots/setup 2.png](#file-b31749773496f6d1) | Documentation/resource | 0 |
| [docs/screenshots/setup 3.png](#file-31c7ba2a78fa4783) | Documentation/resource | 0 |
| [docs/screenshots/setup 4.png](#file-991a05bbbd648bf8) | Documentation/resource | 0 |
| [docs/screenshots/setup 5.png](#file-c296f1b827fce9f3) | Documentation/resource | 0 |
| [docs/screenshots/trending videos.png](#file-ca941b12bd6e7d3d) | Documentation/resource | 0 |
| [thumb.png](#file-8c560e4ae292c462) | EmbeddedResource + Content | 0 |
| [tools/CodeMap/CodeMap.csproj](#file-f877f7ea17f46f5d) | Build definition | 0 |
| [tools/CodeMap/Program.cs](#file-bf979a122640e0a8) | C# source excluded from plugin Compile | 98 |
| [tools/code-map.config.json](#file-ed1a99afe3defdab) | Development tooling | 0 |
| [tools/code-map.mjs](#file-f02e27bb31214f09) | Development tooling | 322 |
| [tools/package.json](#file-fd2585028b28d5cf) | Development tooling | 0 |

<a id="file-618cd5b83d62060b"></a>
## [.gitattributes](../.gitattributes)

Repository support; 66 bytes.

<a id="file-bc37d034bad56458"></a>
## [.gitignore](../.gitignore)

Repository support; 102 bytes.

<a id="file-a54ff182c7e8acf5"></a>
## [AGENTS.md](../AGENTS.md)

Documentation/resource; 1241 bytes; 8 lines.

<a id="file-3fbcb901338e79fb"></a>
## [ChannelContentFlags.cs](../ChannelContentFlags.cs)

Plugin Compile source (SDK default); 1538 bytes; 52 lines.

<details>
<summary>All 29 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [4:1](../ChannelContentFlags.cs#L4) |
| type | Emby.YouTubePlugin.ChannelContentFlags | [6:5](../ChannelContentFlags.cs#L6) |
| field | Emby.YouTubePlugin.ChannelContentFlags.States | [8:83](../ChannelContentFlags.cs#L8) |
| method | Emby.YouTubePlugin.ChannelContentFlags.Get | [10:9](../ChannelContentFlags.cs#L10) |
| parameter | Emby.YouTubePlugin.ChannelContentFlags.Get.channelId | [10:48](../ChannelContentFlags.cs#L10) |
| pattern-variable | Emby.YouTubePlugin.ChannelContentFlags.Get.state | [12:58](../ChannelContentFlags.cs#L12) |
| method | Emby.YouTubePlugin.ChannelContentFlags.SetHasShorts | [15:9](../ChannelContentFlags.cs#L15) |
| parameter | Emby.YouTubePlugin.ChannelContentFlags.SetHasShorts.channelId | [15:41](../ChannelContentFlags.cs#L15) |
| parameter | Emby.YouTubePlugin.ChannelContentFlags.SetHasShorts.hasShorts | [15:59](../ChannelContentFlags.cs#L15) |
| anonymous-function | Emby.YouTubePlugin.ChannelContentFlags.SetHasShorts.anonymous@536 | [17:31](../ChannelContentFlags.cs#L17) |
| parameter | Emby.YouTubePlugin.ChannelContentFlags.SetHasShorts.anonymous@536.state | [17:31](../ChannelContentFlags.cs#L17) |
| method | Emby.YouTubePlugin.ChannelContentFlags.SetHasLive | [24:9](../ChannelContentFlags.cs#L24) |
| parameter | Emby.YouTubePlugin.ChannelContentFlags.SetHasLive.channelId | [24:39](../ChannelContentFlags.cs#L24) |
| parameter | Emby.YouTubePlugin.ChannelContentFlags.SetHasLive.hasLive | [24:57](../ChannelContentFlags.cs#L24) |
| anonymous-function | Emby.YouTubePlugin.ChannelContentFlags.SetHasLive.anonymous@796 | [26:31](../ChannelContentFlags.cs#L26) |
| parameter | Emby.YouTubePlugin.ChannelContentFlags.SetHasLive.anonymous@796.state | [26:31](../ChannelContentFlags.cs#L26) |
| method | Emby.YouTubePlugin.ChannelContentFlags.Update | [33:9](../ChannelContentFlags.cs#L33) |
| parameter | Emby.YouTubePlugin.ChannelContentFlags.Update.channelId | [33:36](../ChannelContentFlags.cs#L33) |
| parameter | Emby.YouTubePlugin.ChannelContentFlags.Update.update | [33:54](../ChannelContentFlags.cs#L33) |
| anonymous-function | Emby.YouTubePlugin.ChannelContentFlags.Update.anonymous@1240 | [42:17](../ChannelContentFlags.cs#L42) |
| parameter | Emby.YouTubePlugin.ChannelContentFlags.Update.anonymous@1240._ | [42:17](../ChannelContentFlags.cs#L42) |
| anonymous-function | Emby.YouTubePlugin.ChannelContentFlags.Update.anonymous@1318 | [43:17](../ChannelContentFlags.cs#L43) |
| parameter | Emby.YouTubePlugin.ChannelContentFlags.Update.anonymous@1318._ | [43:18](../ChannelContentFlags.cs#L43) |
| parameter | Emby.YouTubePlugin.ChannelContentFlags.Update.anonymous@1318.existing | [43:21](../ChannelContentFlags.cs#L43) |
| type | Emby.YouTubePlugin.ChannelContentState | [47:5](../ChannelContentFlags.cs#L47) |
| parameter | Emby.YouTubePlugin.ChannelContentState.HasShorts | [48:9](../ChannelContentFlags.cs#L48) |
| parameter | Emby.YouTubePlugin.ChannelContentState.HasLive | [49:9](../ChannelContentFlags.cs#L49) |
| parameter | Emby.YouTubePlugin.ChannelContentState.ShortsCheckedAt | [50:9](../ChannelContentFlags.cs#L50) |
| parameter | Emby.YouTubePlugin.ChannelContentState.LiveCheckedAt | [51:9](../ChannelContentFlags.cs#L51) |

</details>

<a id="file-3265de6859f23663"></a>
## [Configuration/YouTubeConfiguration.html](../Configuration/YouTubeConfiguration.html)

EmbeddedResource; 37233 bytes; 756 lines.

Resource/controller references:

- Line [1](../Configuration/YouTubeConfiguration.html#L1): data-controller = `configurationpage?name=YouTubeConfigurationjs`
- Line [345](../Configuration/YouTubeConfiguration.html#L345): href = `#`
- Line [347](../Configuration/YouTubeConfiguration.html#L347): href = `#`
- Line [633](../Configuration/YouTubeConfiguration.html#L633): href = `https://paypal.me/eliasbruno123`
- Line [635](../Configuration/YouTubeConfiguration.html#L635): src = `https://img.shields.io/badge/Donate%20with-PayPal-0070BA?style=for-the-badge&logo=paypal&logoColor=white`
- Line [649](../Configuration/YouTubeConfiguration.html#L649): href = `https://github.com/eliasbruno124-dev/Emby-Youtube-Plugin`
- Line [676](../Configuration/YouTubeConfiguration.html#L676): href = `https://console.cloud.google.com/apis/library/youtube.googleapis.com`
- Line [679](../Configuration/YouTubeConfiguration.html#L679): href = `https://console.cloud.google.com/apis/credentials`
- Line [691](../Configuration/YouTubeConfiguration.html#L691): data-guide-image = `step1.png`
- Line [700](../Configuration/YouTubeConfiguration.html#L700): data-guide-image = `step2.png`
- Line [710](../Configuration/YouTubeConfiguration.html#L710): data-guide-image = `step34.png`
- Line [719](../Configuration/YouTubeConfiguration.html#L719): data-guide-image = `step5.png`
- Line [739](../Configuration/YouTubeConfiguration.html#L739): data-guide-image = `step7.png`
- Line [749](../Configuration/YouTubeConfiguration.html#L749): data-guide-image = `step8.png`

<details>
<summary>All 50 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| style-block | style@4 | [4:5](../Configuration/YouTubeConfiguration.html#L4) |
| dom-id | div.youtubeConfigurationRoot | [341:9](../Configuration/YouTubeConfiguration.html#L341) |
| dom-id | div.settingsPage | [351:13](../Configuration/YouTubeConfiguration.html#L351) |
| dom-id | span.quotaSearchNumber | [364:37](../Configuration/YouTubeConfiguration.html#L364) |
| dom-id | span.quotaSearchTotal | [366:37](../Configuration/YouTubeConfiguration.html#L366) |
| dom-id | div.quotaSearchPercent | [369:33](../Configuration/YouTubeConfiguration.html#L369) |
| dom-id | div.quotaSearchBar | [371:29](../Configuration/YouTubeConfiguration.html#L371) |
| dom-id | div.quotaSearchBarFill | [372:33](../Configuration/YouTubeConfiguration.html#L372) |
| dom-id | span.quotaReset | [375:33](../Configuration/YouTubeConfiguration.html#L375) |
| dom-id | span.quotaSearchLifetime | [376:33](../Configuration/YouTubeConfiguration.html#L376) |
| dom-id | span.quotaOtherNumber | [383:37](../Configuration/YouTubeConfiguration.html#L383) |
| dom-id | span.quotaOtherTotal | [385:37](../Configuration/YouTubeConfiguration.html#L385) |
| dom-id | div.quotaOtherPercent | [388:33](../Configuration/YouTubeConfiguration.html#L388) |
| dom-id | div.quotaOtherBar | [390:29](../Configuration/YouTubeConfiguration.html#L390) |
| dom-id | div.quotaOtherBarFill | [391:33](../Configuration/YouTubeConfiguration.html#L391) |
| dom-id | span.quotaOtherLifetime | [395:33](../Configuration/YouTubeConfiguration.html#L395) |
| dom-id | input.txtApiKey | [404:29](../Configuration/YouTubeConfiguration.html#L404) |
| dom-id | input.txtSavedItems | [416:33](../Configuration/YouTubeConfiguration.html#L416) |
| dom-id | input.txtSavedItemEntry | [418:37](../Configuration/YouTubeConfiguration.html#L418) |
| dom-id | button.btnAddSavedItem | [420:37](../Configuration/YouTubeConfiguration.html#L420) |
| dom-id | div.savedItemsList | [431:33](../Configuration/YouTubeConfiguration.html#L431) |
| dom-id | input.txtWatchLaterPlaylist | [437:33](../Configuration/YouTubeConfiguration.html#L437) |
| dom-id | input.txtWatchLaterEntry | [439:37](../Configuration/YouTubeConfiguration.html#L439) |
| dom-id | button.btnAddWatchLaterItem | [441:37](../Configuration/YouTubeConfiguration.html#L441) |
| dom-id | div.watchLaterItemsList | [450:33](../Configuration/YouTubeConfiguration.html#L450) |
| dom-id | input.numWatchLaterPollMinutes | [456:33](../Configuration/YouTubeConfiguration.html#L456) |
| dom-id | input.chkDirectPlayback | [468:29](../Configuration/YouTubeConfiguration.html#L468) |
| dom-id | input.numDirectMaxHeight | [478:29](../Configuration/YouTubeConfiguration.html#L478) |
| dom-id | input.numDirectH264MaxHeight | [483:29](../Configuration/YouTubeConfiguration.html#L483) |
| dom-id | input.chkShowRootFoldersAtTopLevel | [496:37](../Configuration/YouTubeConfiguration.html#L496) |
| dom-id | div.homeRowsDescription | [500:33](../Configuration/YouTubeConfiguration.html#L500) |
| dom-id | input.chkShowTrending | [507:37](../Configuration/YouTubeConfiguration.html#L507) |
| dom-id | div.trendingDescription | [511:33](../Configuration/YouTubeConfiguration.html#L511) |
| dom-id | input.chkShowCategories | [517:37](../Configuration/YouTubeConfiguration.html#L517) |
| dom-id | div.categoriesDescription | [521:33](../Configuration/YouTubeConfiguration.html#L521) |
| dom-id | input.chkShowShorts | [527:37](../Configuration/YouTubeConfiguration.html#L527) |
| dom-id | div.shortsDescription | [531:33](../Configuration/YouTubeConfiguration.html#L531) |
| dom-id | select.selectTrendingRegion | [543:33](../Configuration/YouTubeConfiguration.html#L543) |
| dom-id | select.selectTrendingCategory | [552:33](../Configuration/YouTubeConfiguration.html#L552) |
| dom-id | select.selectChannelSortBy | [573:33](../Configuration/YouTubeConfiguration.html#L573) |
| dom-id | input.chkShowLikeCount | [588:33](../Configuration/YouTubeConfiguration.html#L588) |
| dom-id | input.chkShowCommentCount | [592:33](../Configuration/YouTubeConfiguration.html#L592) |
| dom-id | input.numMaxChannelVideos | [607:33](../Configuration/YouTubeConfiguration.html#L607) |
| dom-id | input.numMaxSearchVideos | [616:33](../Configuration/YouTubeConfiguration.html#L616) |
| dom-id | a.btnDonate | [633:29](../Configuration/YouTubeConfiguration.html#L633) |
| dom-id | p.youtubeConfigurationStatus | [645:21](../Configuration/YouTubeConfiguration.html#L645) |
| dom-id | div.ytPluginCredits | [647:21](../Configuration/YouTubeConfiguration.html#L647) |
| dom-id | span.ytPluginVersion | [661:47](../Configuration/YouTubeConfiguration.html#L661) |
| dom-id | span.ytUpdateBadge | [662:25](../Configuration/YouTubeConfiguration.html#L662) |
| dom-id | div.setupGuidePage | [669:13](../Configuration/YouTubeConfiguration.html#L669) |

</details>

<a id="file-44888eb7c4d56aef"></a>
## [Configuration/YouTubeConfiguration.js](../Configuration/YouTubeConfiguration.js)

EmbeddedResource; 38224 bytes; 917 lines.

<details>
<summary>All 357 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| function | define callback@113 | [8:4](../Configuration/YouTubeConfiguration.js#L8) |
| parameter | define callback@113.BaseView | [8:14](../Configuration/YouTubeConfiguration.js#L8) |
| parameter | define callback@113.loading | [8:24](../Configuration/YouTubeConfiguration.js#L8) |
| parameter | define callback@113.toast | [8:33](../Configuration/YouTubeConfiguration.js#L8) |
| variable | define callback@113.pluginId | [11:11](../Configuration/YouTubeConfiguration.js#L11) |
| variable | define callback@113.defaultDonateUrl | [12:11](../Configuration/YouTubeConfiguration.js#L12) |
| variable | define callback@113.directMinimumHeight | [13:11](../Configuration/YouTubeConfiguration.js#L13) |
| variable | define callback@113.directMaximumHeight | [14:11](../Configuration/YouTubeConfiguration.js#L14) |
| function | define callback@113.client | [15:5](../Configuration/YouTubeConfiguration.js#L15) |
| function | define callback@113.field | [19:5](../Configuration/YouTubeConfiguration.js#L19) |
| parameter | define callback@113.field.view | [19:20](../Configuration/YouTubeConfiguration.js#L19) |
| parameter | define callback@113.field.id | [19:26](../Configuration/YouTubeConfiguration.js#L19) |
| function | define callback@113.value | [23:5](../Configuration/YouTubeConfiguration.js#L23) |
| parameter | define callback@113.value.view | [23:20](../Configuration/YouTubeConfiguration.js#L23) |
| parameter | define callback@113.value.id | [23:26](../Configuration/YouTubeConfiguration.js#L23) |
| variable | define callback@113.value.el | [24:15](../Configuration/YouTubeConfiguration.js#L24) |
| function | define callback@113.setValue | [28:5](../Configuration/YouTubeConfiguration.js#L28) |
| parameter | define callback@113.setValue.view | [28:23](../Configuration/YouTubeConfiguration.js#L28) |
| parameter | define callback@113.setValue.id | [28:29](../Configuration/YouTubeConfiguration.js#L28) |
| parameter | define callback@113.setValue.val | [28:33](../Configuration/YouTubeConfiguration.js#L28) |
| variable | define callback@113.setValue.el | [29:15](../Configuration/YouTubeConfiguration.js#L29) |
| function | define callback@113.checked | [35:5](../Configuration/YouTubeConfiguration.js#L35) |
| parameter | define callback@113.checked.view | [35:22](../Configuration/YouTubeConfiguration.js#L35) |
| parameter | define callback@113.checked.id | [35:28](../Configuration/YouTubeConfiguration.js#L35) |
| variable | define callback@113.checked.el | [36:15](../Configuration/YouTubeConfiguration.js#L36) |
| function | define callback@113.setChecked | [40:5](../Configuration/YouTubeConfiguration.js#L40) |
| parameter | define callback@113.setChecked.view | [40:25](../Configuration/YouTubeConfiguration.js#L40) |
| parameter | define callback@113.setChecked.id | [40:31](../Configuration/YouTubeConfiguration.js#L40) |
| parameter | define callback@113.setChecked.val | [40:35](../Configuration/YouTubeConfiguration.js#L40) |
| variable | define callback@113.setChecked.el | [41:15](../Configuration/YouTubeConfiguration.js#L41) |
| function | define callback@113.numberValue | [47:5](../Configuration/YouTubeConfiguration.js#L47) |
| parameter | define callback@113.numberValue.view | [47:26](../Configuration/YouTubeConfiguration.js#L47) |
| parameter | define callback@113.numberValue.id | [47:32](../Configuration/YouTubeConfiguration.js#L47) |
| parameter | define callback@113.numberValue.fallback | [47:36](../Configuration/YouTubeConfiguration.js#L47) |
| parameter | define callback@113.numberValue.min | [47:46](../Configuration/YouTubeConfiguration.js#L47) |
| parameter | define callback@113.numberValue.max | [47:51](../Configuration/YouTubeConfiguration.js#L47) |
| variable | define callback@113.numberValue.parsed | [48:13](../Configuration/YouTubeConfiguration.js#L48) |
| function | define callback@113.clampDirectHeight | [55:5](../Configuration/YouTubeConfiguration.js#L55) |
| parameter | define callback@113.clampDirectHeight.rawValue | [55:32](../Configuration/YouTubeConfiguration.js#L55) |
| parameter | define callback@113.clampDirectHeight.fallbackValue | [55:42](../Configuration/YouTubeConfiguration.js#L55) |
| variable | define callback@113.clampDirectHeight.parsed | [56:15](../Configuration/YouTubeConfiguration.js#L56) |
| variable | define callback@113.clampDirectHeight.fallback | [57:15](../Configuration/YouTubeConfiguration.js#L57) |
| variable | define callback@113.clampDirectHeight.height | [60:15](../Configuration/YouTubeConfiguration.js#L60) |
| function | define callback@113.setStatus | [66:5](../Configuration/YouTubeConfiguration.js#L66) |
| parameter | define callback@113.setStatus.view | [66:24](../Configuration/YouTubeConfiguration.js#L66) |
| parameter | define callback@113.setStatus.text | [66:30](../Configuration/YouTubeConfiguration.js#L66) |
| function | define callback@113.setText | [70:5](../Configuration/YouTubeConfiguration.js#L70) |
| parameter | define callback@113.setText.view | [70:22](../Configuration/YouTubeConfiguration.js#L70) |
| parameter | define callback@113.setText.id | [70:28](../Configuration/YouTubeConfiguration.js#L70) |
| parameter | define callback@113.setText.text | [70:32](../Configuration/YouTubeConfiguration.js#L70) |
| variable | define callback@113.setText.el | [71:15](../Configuration/YouTubeConfiguration.js#L71) |
| function | define callback@113.splitEntries | [77:5](../Configuration/YouTubeConfiguration.js#L77) |
| parameter | define callback@113.splitEntries.text | [77:27](../Configuration/YouTubeConfiguration.js#L77) |
| function | define callback@113.splitEntries.(text \|\| '')             .split(/[,\n\r;]+/)             .map callback@2324 | [80:18](../Configuration/YouTubeConfiguration.js#L80) |
| parameter | define callback@113.splitEntries.(text \|\| '')             .split(/[,\n\r;]+/)             .map callback@2324.item | [80:18](../Configuration/YouTubeConfiguration.js#L80) |
| function | define callback@113.splitEntries.(text \|\| '')             .split(/[,\n\r;]+/)             .map(item =&gt; item.trim())             .filter callback@2365 | [81:21](../Configuration/YouTubeConfiguration.js#L81) |
| parameter | define callback@113.splitEntries.(text \|\| '')             .split(/[,\n\r;]+/)             .map(item =&gt; item.trim())             .filter callback@2365.item | [81:21](../Configuration/YouTubeConfiguration.js#L81) |
| function | define callback@113.uniqueEntries | [84:5](../Configuration/YouTubeConfiguration.js#L84) |
| parameter | define callback@113.uniqueEntries.entries | [84:28](../Configuration/YouTubeConfiguration.js#L84) |
| variable | define callback@113.uniqueEntries.seen | [85:15](../Configuration/YouTubeConfiguration.js#L85) |
| variable | define callback@113.uniqueEntries.result | [86:15](../Configuration/YouTubeConfiguration.js#L86) |
| function | define callback@113.uniqueEntries.entries.forEach callback@2520 | [88:25](../Configuration/YouTubeConfiguration.js#L88) |
| parameter | define callback@113.uniqueEntries.entries.forEach callback@2520.item | [88:25](../Configuration/YouTubeConfiguration.js#L88) |
| function | define callback@113.setConfigurationControlsDisabled | [98:5](../Configuration/YouTubeConfiguration.js#L98) |
| parameter | define callback@113.setConfigurationControlsDisabled.view | [98:47](../Configuration/YouTubeConfiguration.js#L98) |
| parameter | define callback@113.setConfigurationControlsDisabled.disabled | [98:53](../Configuration/YouTubeConfiguration.js#L98) |
| variable | define callback@113.setConfigurationControlsDisabled.form | [99:15](../Configuration/YouTubeConfiguration.js#L99) |
| function | define callback@113.setConfigurationControlsDisabled.form.querySelectorAll('input, select, textarea, button').forEach callback@2947 | [103:74](../Configuration/YouTubeConfiguration.js#L103) |
| parameter | define callback@113.setConfigurationControlsDisabled.form.querySelectorAll('input, select, textarea, button').forEach callback@2947.control | [103:74](../Configuration/YouTubeConfiguration.js#L103) |
| function | define callback@113.formatNumber | [108:5](../Configuration/YouTubeConfiguration.js#L108) |
| parameter | define callback@113.formatNumber.n | [108:27](../Configuration/YouTubeConfiguration.js#L108) |
| variable | define callback@113.formatNumber.value | [109:15](../Configuration/YouTubeConfiguration.js#L109) |
| catch-variable | define callback@113.formatNumber.e | [111:16](../Configuration/YouTubeConfiguration.js#L111) |
| function | define callback@113.formatResetCountdown | [114:5](../Configuration/YouTubeConfiguration.js#L114) |
| parameter | define callback@113.formatResetCountdown.seconds | [114:35](../Configuration/YouTubeConfiguration.js#L114) |
| variable | define callback@113.formatResetCountdown.total | [115:15](../Configuration/YouTubeConfiguration.js#L115) |
| variable | define callback@113.formatResetCountdown.h | [117:15](../Configuration/YouTubeConfiguration.js#L117) |
| variable | define callback@113.formatResetCountdown.m | [118:15](../Configuration/YouTubeConfiguration.js#L118) |
| function | define callback@113.renderQuotaBucket | [123:5](../Configuration/YouTubeConfiguration.js#L123) |
| parameter | define callback@113.renderQuotaBucket.view | [123:32](../Configuration/YouTubeConfiguration.js#L123) |
| parameter | define callback@113.renderQuotaBucket.prefix | [123:38](../Configuration/YouTubeConfiguration.js#L123) |
| parameter | define callback@113.renderQuotaBucket.usedValue | [123:46](../Configuration/YouTubeConfiguration.js#L123) |
| parameter | define callback@113.renderQuotaBucket.limitValue | [123:57](../Configuration/YouTubeConfiguration.js#L123) |
| parameter | define callback@113.renderQuotaBucket.lifetimeValue | [123:69](../Configuration/YouTubeConfiguration.js#L123) |
| parameter | define callback@113.renderQuotaBucket.unit | [123:84](../Configuration/YouTubeConfiguration.js#L123) |
| variable | define callback@113.renderQuotaBucket.used | [124:15](../Configuration/YouTubeConfiguration.js#L124) |
| variable | define callback@113.renderQuotaBucket.limit | [125:15](../Configuration/YouTubeConfiguration.js#L125) |
| variable | define callback@113.renderQuotaBucket.lifetime | [126:15](../Configuration/YouTubeConfiguration.js#L126) |
| variable | define callback@113.renderQuotaBucket.pct | [127:15](../Configuration/YouTubeConfiguration.js#L127) |
| variable | define callback@113.renderQuotaBucket.bar | [134:15](../Configuration/YouTubeConfiguration.js#L134) |
| variable | define callback@113.renderQuotaBucket.fill | [135:15](../Configuration/YouTubeConfiguration.js#L135) |
| function | define callback@113.renderQuota | [144:5](../Configuration/YouTubeConfiguration.js#L144) |
| parameter | define callback@113.renderQuota.view | [144:26](../Configuration/YouTubeConfiguration.js#L144) |
| parameter | define callback@113.renderQuota.config | [144:32](../Configuration/YouTubeConfiguration.js#L144) |
| variable | define callback@113.renderQuota.resetSec | [160:15](../Configuration/YouTubeConfiguration.js#L160) |
| variable | define callback@113.renderQuota.reset | [161:15](../Configuration/YouTubeConfiguration.js#L161) |
| function | define callback@113.isSupportedPlaylistId | [168:5](../Configuration/YouTubeConfiguration.js#L168) |
| parameter | define callback@113.isSupportedPlaylistId.item | [168:36](../Configuration/YouTubeConfiguration.js#L168) |
| function | define callback@113.classifySavedItem | [172:5](../Configuration/YouTubeConfiguration.js#L172) |
| parameter | define callback@113.classifySavedItem.item | [172:32](../Configuration/YouTubeConfiguration.js#L172) |
| function | define callback@113.classifyAutoPlaylistItem | [188:5](../Configuration/YouTubeConfiguration.js#L188) |
| parameter | define callback@113.classifyAutoPlaylistItem.item | [188:39](../Configuration/YouTubeConfiguration.js#L188) |
| function | define callback@113.setDonateButton | [198:5](../Configuration/YouTubeConfiguration.js#L198) |
| parameter | define callback@113.setDonateButton.view | [198:30](../Configuration/YouTubeConfiguration.js#L198) |
| variable | define callback@113.setDonateButton.button | [199:15](../Configuration/YouTubeConfiguration.js#L199) |
| function | define callback@113.pluginUrl | [205:5](../Configuration/YouTubeConfiguration.js#L205) |
| parameter | define callback@113.pluginUrl.path | [205:24](../Configuration/YouTubeConfiguration.js#L205) |
| variable | define callback@113.pluginUrl.apiClient | [206:15](../Configuration/YouTubeConfiguration.js#L206) |
| function | define callback@113.setGuideImages | [214:5](../Configuration/YouTubeConfiguration.js#L214) |
| parameter | define callback@113.setGuideImages.view | [214:29](../Configuration/YouTubeConfiguration.js#L214) |
| variable | define callback@113.setGuideImages.images | [215:15](../Configuration/YouTubeConfiguration.js#L215) |
| function | define callback@113.setGuideImages.images.forEach callback@6811 | [216:24](../Configuration/YouTubeConfiguration.js#L216) |
| parameter | define callback@113.setGuideImages.images.forEach callback@6811.img | [216:24](../Configuration/YouTubeConfiguration.js#L216) |
| variable | define callback@113.setGuideImages.images.forEach callback@6811.name | [217:19](../Configuration/YouTubeConfiguration.js#L217) |
| function | define callback@113.populateTrendingRegions | [224:5](../Configuration/YouTubeConfiguration.js#L224) |
| parameter | define callback@113.populateTrendingRegions.view | [224:38](../Configuration/YouTubeConfiguration.js#L224) |
| parameter | define callback@113.populateTrendingRegions.regionResult | [224:44](../Configuration/YouTubeConfiguration.js#L224) |
| parameter | define callback@113.populateTrendingRegions.selectedValue | [224:58](../Configuration/YouTubeConfiguration.js#L224) |
| variable | define callback@113.populateTrendingRegions.select | [225:15](../Configuration/YouTubeConfiguration.js#L225) |
| variable | define callback@113.populateTrendingRegions.selected | [230:15](../Configuration/YouTubeConfiguration.js#L230) |
| variable | define callback@113.populateTrendingRegions.lookupSucceeded | [231:15](../Configuration/YouTubeConfiguration.js#L231) |
| variable | define callback@113.populateTrendingRegions.regions | [232:15](../Configuration/YouTubeConfiguration.js#L232) |
| variable | define callback@113.populateTrendingRegions.defaultOption | [235:15](../Configuration/YouTubeConfiguration.js#L235) |
| variable | define callback@113.populateTrendingRegions.normalized | [240:15](../Configuration/YouTubeConfiguration.js#L240) |
| function | define callback@113.populateTrendingRegions.normalized.forEach callback@7804 | [241:28](../Configuration/YouTubeConfiguration.js#L241) |
| parameter | define callback@113.populateTrendingRegions.normalized.forEach callback@7804.region | [241:28](../Configuration/YouTubeConfiguration.js#L241) |
| variable | define callback@113.populateTrendingRegions.normalized.forEach callback@7804.option | [242:19](../Configuration/YouTubeConfiguration.js#L242) |
| variable | define callback@113.populateTrendingRegions.selectedIsSupported | [248:15](../Configuration/YouTubeConfiguration.js#L248) |
| function | define callback@113.populateTrendingRegions.normalized.some callback@8095 | [248:53](../Configuration/YouTubeConfiguration.js#L248) |
| parameter | define callback@113.populateTrendingRegions.normalized.some callback@8095.region | [248:53](../Configuration/YouTubeConfiguration.js#L248) |
| variable | define callback@113.populateTrendingRegions.savedOption | [250:19](../Configuration/YouTubeConfiguration.js#L250) |
| function | define callback@113.fetchTrendingRegions | [259:5](../Configuration/YouTubeConfiguration.js#L259) |
| parameter | define callback@113.fetchTrendingRegions.apiClient | [259:35](../Configuration/YouTubeConfiguration.js#L259) |
| function | define callback@113.fetchTrendingRegions.apiClient.ajax({             type: 'GET',             url: apiClient.getUrl('YouTubePlugin/Regions'),             dataType: 'json'         }).then callback@8955 | [268:17](../Configuration/YouTubeConfiguration.js#L268) |
| parameter | define callback@113.fetchTrendingRegions.apiClient.ajax({             type: 'GET',             url: apiClient.getUrl('YouTubePlugin/Regions'),             dataType: 'json'         }).then callback@8955.payload | [268:17](../Configuration/YouTubeConfiguration.js#L268) |
| catch-variable | define callback@113.fetchTrendingRegions.apiClient.ajax({             type: 'GET',             url: apiClient.getUrl('YouTubePlugin/Regions'),             dataType: 'json'         }).then callback@8955.e | [271:24](../Configuration/YouTubeConfiguration.js#L271) |
| variable | define callback@113.fetchTrendingRegions.apiClient.ajax({             type: 'GET',             url: apiClient.getUrl('YouTubePlugin/Regions'),             dataType: 'json'         }).then callback@8955.source | [274:19](../Configuration/YouTubeConfiguration.js#L274) |
| variable | define callback@113.fetchTrendingRegions.apiClient.ajax({             type: 'GET',             url: apiClient.getUrl('YouTubePlugin/Regions'),             dataType: 'json'         }).then callback@8955.seen | [278:19](../Configuration/YouTubeConfiguration.js#L278) |
| variable | define callback@113.fetchTrendingRegions.apiClient.ajax({             type: 'GET',             url: apiClient.getUrl('YouTubePlugin/Regions'),             dataType: 'json'         }).then callback@8955.regions | [279:19](../Configuration/YouTubeConfiguration.js#L279) |
| function | define callback@113.fetchTrendingRegions.apiClient.ajax({             type: 'GET',             url: apiClient.getUrl('YouTubePlugin/Regions'),             dataType: 'json'         }).then callback@8955.source.map callback@9442 | [279:40](../Configuration/YouTubeConfiguration.js#L279) |
| parameter | define callback@113.fetchTrendingRegions.apiClient.ajax({             type: 'GET',             url: apiClient.getUrl('YouTubePlugin/Regions'),             dataType: 'json'         }).then callback@8955.source.map callback@9442.region | [279:40](../Configuration/YouTubeConfiguration.js#L279) |
| function | define callback@113.fetchTrendingRegions.apiClient.ajax({             type: 'GET',             url: apiClient.getUrl('YouTubePlugin/Regions'),             dataType: 'json'         }).then callback@8955.source.map(region =&gt; ({                 Code: String((region && (region.Code \|\| region.code)) \|\| '').trim().toUpperCase(),                 Name: String((region && (region.Name \|\| region.name)) \|\| '').trim()             })).filter callback@9661 | [282:24](../Configuration/YouTubeConfiguration.js#L282) |
| parameter | define callback@113.fetchTrendingRegions.apiClient.ajax({             type: 'GET',             url: apiClient.getUrl('YouTubePlugin/Regions'),             dataType: 'json'         }).then callback@8955.source.map(region =&gt; ({                 Code: String((region && (region.Code \|\| region.code)) \|\| '').trim().toUpperCase(),                 Name: String((region && (region.Name \|\| region.name)) \|\| '').trim()             })).filter callback@9661.region | [282:24](../Configuration/YouTubeConfiguration.js#L282) |
| function | define callback@113.fetchTrendingRegions.apiClient.ajax({             type: 'GET',             url: apiClient.getUrl('YouTubePlugin/Regions'),             dataType: 'json'         }).then callback@8955.source.map(region =&gt; ({                 Code: String((region && (region.Code \|\| region.code)) \|\| '').trim().toUpperCase(),                 Name: String((region && (region.Name \|\| region.name)) \|\| '').trim()             })).filter(region =&gt; {                 if (!/^[A-Z]{2}$/.test(region.Code) \|\| !region.Name \|\| seen.has(region.Code)) {                     return false;                 }                 seen.add(region.Code);                 return true;             }).sort callback@9909 | [288:21](../Configuration/YouTubeConfiguration.js#L288) |
| parameter | define callback@113.fetchTrendingRegions.apiClient.ajax({             type: 'GET',             url: apiClient.getUrl('YouTubePlugin/Regions'),             dataType: 'json'         }).then callback@8955.source.map(region =&gt; ({                 Code: String((region && (region.Code \|\| region.code)) \|\| '').trim().toUpperCase(),                 Name: String((region && (region.Name \|\| region.name)) \|\| '').trim()             })).filter(region =&gt; {                 if (!/^[A-Z]{2}$/.test(region.Code) \|\| !region.Name \|\| seen.has(region.Code)) {                     return false;                 }                 seen.add(region.Code);                 return true;             }).sort callback@9909.a | [288:22](../Configuration/YouTubeConfiguration.js#L288) |
| parameter | define callback@113.fetchTrendingRegions.apiClient.ajax({             type: 'GET',             url: apiClient.getUrl('YouTubePlugin/Regions'),             dataType: 'json'         }).then callback@8955.source.map(region =&gt; ({                 Code: String((region && (region.Code \|\| region.code)) \|\| '').trim().toUpperCase(),                 Name: String((region && (region.Name \|\| region.name)) \|\| '').trim()             })).filter(region =&gt; {                 if (!/^[A-Z]{2}$/.test(region.Code) \|\| !region.Name \|\| seen.has(region.Code)) {                     return false;                 }                 seen.add(region.Code);                 return true;             }).sort callback@9909.b | [288:25](../Configuration/YouTubeConfiguration.js#L288) |
| function | define callback@113.fetchTrendingRegions.apiClient.ajax({             type: 'GET',             url: apiClient.getUrl('YouTubePlugin/Regions'),             dataType: 'json'         }).then(payload =&gt; {             if (typeof payload === 'string') {                 try { payload = JSON.parse(payload); }                 catch (e) { return { LookupSucceeded: false, Regions: [] }; }             }              const source = (payload && (payload.Regions \|\| payload.regions)) \|\| [];             if (!Array.isArray(source)) {                 return { LookupSucceeded: false, Regions: [] };             }             const seen = new Set();             const regions = source.map(region =&gt; ({                 Code: String((region && (region.Code \|\| region.code)) \|\| '').trim().toUpperCase(),                 Name: String((region && (region.Name \|\| region.name)) \|\| '').trim()             })).filter(region =&gt; {                 if (!/^[A-Z]{2}$/.test(region.Code) \|\| !region.Name \|\| seen.has(region.Code)) {                     return false;                 }                 seen.add(region.Code);                 return true;             }).sort((a, b) =&gt; a.Name.localeCompare(b.Name));             return {                 LookupSucceeded: !!(payload && (payload.LookupSucceeded \|\| payload.lookupSucceeded))                     && regions.length &gt; 0,                 Regions: regions             };         }).catch callback@10180 | [294:18](../Configuration/YouTubeConfiguration.js#L294) |
| parameter | define callback@113.fetchTrendingRegions.apiClient.ajax({             type: 'GET',             url: apiClient.getUrl('YouTubePlugin/Regions'),             dataType: 'json'         }).then(payload =&gt; {             if (typeof payload === 'string') {                 try { payload = JSON.parse(payload); }                 catch (e) { return { LookupSucceeded: false, Regions: [] }; }             }              const source = (payload && (payload.Regions \|\| payload.regions)) \|\| [];             if (!Array.isArray(source)) {                 return { LookupSucceeded: false, Regions: [] };             }             const seen = new Set();             const regions = source.map(region =&gt; ({                 Code: String((region && (region.Code \|\| region.code)) \|\| '').trim().toUpperCase(),                 Name: String((region && (region.Name \|\| region.name)) \|\| '').trim()             })).filter(region =&gt; {                 if (!/^[A-Z]{2}$/.test(region.Code) \|\| !region.Name \|\| seen.has(region.Code)) {                     return false;                 }                 seen.add(region.Code);                 return true;             }).sort((a, b) =&gt; a.Name.localeCompare(b.Name));             return {                 LookupSucceeded: !!(payload && (payload.LookupSucceeded \|\| payload.lookupSucceeded))                     && regions.length &gt; 0,                 Regions: regions             };         }).catch callback@10180.error | [294:18](../Configuration/YouTubeConfiguration.js#L294) |
| function | define callback@113.bindTabNavigation | [300:5](../Configuration/YouTubeConfiguration.js#L300) |
| parameter | define callback@113.bindTabNavigation.view | [300:32](../Configuration/YouTubeConfiguration.js#L300) |
| variable | define callback@113.bindTabNavigation.navButtons | [301:15](../Configuration/YouTubeConfiguration.js#L301) |
| variable | define callback@113.bindTabNavigation.pages | [302:15](../Configuration/YouTubeConfiguration.js#L302) |
| function | define callback@113.bindTabNavigation.navButtons.forEach callback@10622 | [307:28](../Configuration/YouTubeConfiguration.js#L307) |
| parameter | define callback@113.bindTabNavigation.navButtons.forEach callback@10622.button | [307:28](../Configuration/YouTubeConfiguration.js#L307) |
| function | define callback@113.bindTabNavigation.navButtons.forEach callback@10622.button.addEventListener callback@10679 | [308:46](../Configuration/YouTubeConfiguration.js#L308) |
| parameter | define callback@113.bindTabNavigation.navButtons.forEach callback@10622.button.addEventListener callback@10679.e | [308:46](../Configuration/YouTubeConfiguration.js#L308) |
| variable | define callback@113.bindTabNavigation.navButtons.forEach callback@10622.button.addEventListener callback@10679.target | [310:23](../Configuration/YouTubeConfiguration.js#L310) |
| function | define callback@113.bindTabNavigation.navButtons.forEach callback@10622.button.addEventListener callback@10679.navButtons.forEach callback@10825 | [312:36](../Configuration/YouTubeConfiguration.js#L312) |
| parameter | define callback@113.bindTabNavigation.navButtons.forEach callback@10622.button.addEventListener callback@10679.navButtons.forEach callback@10825.item | [312:36](../Configuration/YouTubeConfiguration.js#L312) |
| function | define callback@113.bindTabNavigation.navButtons.forEach callback@10622.button.addEventListener callback@10679.Object.keys(pages).forEach callback@10973 | [315:44](../Configuration/YouTubeConfiguration.js#L315) |
| parameter | define callback@113.bindTabNavigation.navButtons.forEach callback@10622.button.addEventListener callback@10679.Object.keys(pages).forEach callback@10973.key | [315:44](../Configuration/YouTubeConfiguration.js#L315) |
| function | define callback@113.showToast | [328:5](../Configuration/YouTubeConfiguration.js#L328) |
| parameter | define callback@113.showToast.type | [328:24](../Configuration/YouTubeConfiguration.js#L328) |
| parameter | define callback@113.showToast.text | [328:30](../Configuration/YouTubeConfiguration.js#L328) |
| function | define callback@113.compareVersions | [337:5](../Configuration/YouTubeConfiguration.js#L337) |
| parameter | define callback@113.compareVersions.a | [337:30](../Configuration/YouTubeConfiguration.js#L337) |
| parameter | define callback@113.compareVersions.b | [337:33](../Configuration/YouTubeConfiguration.js#L337) |
| variable | define callback@113.compareVersions.norm | [338:15](../Configuration/YouTubeConfiguration.js#L338) |
| function | define callback@113.compareVersions.norm | [338:22](../Configuration/YouTubeConfiguration.js#L338) |
| parameter | define callback@113.compareVersions.norm.v | [338:22](../Configuration/YouTubeConfiguration.js#L338) |
| function | define callback@113.compareVersions.norm.String(v \|\| '').replace(/^v/i, '').split('.').map callback@11717 | [338:77](../Configuration/YouTubeConfiguration.js#L338) |
| parameter | define callback@113.compareVersions.norm.String(v \|\| '').replace(/^v/i, '').split('.').map callback@11717.n | [338:77](../Configuration/YouTubeConfiguration.js#L338) |
| variable | define callback@113.compareVersions.av | [339:15](../Configuration/YouTubeConfiguration.js#L339) |
| variable | define callback@113.compareVersions.bv | [340:15](../Configuration/YouTubeConfiguration.js#L340) |
| variable | define callback@113.compareVersions.len | [341:15](../Configuration/YouTubeConfiguration.js#L341) |
| variable | define callback@113.compareVersions.i | [342:18](../Configuration/YouTubeConfiguration.js#L342) |
| variable | define callback@113.compareVersions.d | [343:19](../Configuration/YouTubeConfiguration.js#L343) |
| function | define callback@113.fetchLatestGithubVersion | [351:5](../Configuration/YouTubeConfiguration.js#L351) |
| variable | define callback@113.fetchLatestGithubVersion.cacheKey | [352:15](../Configuration/YouTubeConfiguration.js#L352) |
| variable | define callback@113.fetchLatestGithubVersion.cached | [356:19](../Configuration/YouTubeConfiguration.js#L356) |
| catch-variable | define callback@113.fetchLatestGithubVersion.e | [360:18](../Configuration/YouTubeConfiguration.js#L360) |
| variable | define callback@113.fetchLatestGithubVersion.repo | [362:15](../Configuration/YouTubeConfiguration.js#L362) |
| variable | define callback@113.fetchLatestGithubVersion.headers | [363:15](../Configuration/YouTubeConfiguration.js#L363) |
| variable | define callback@113.fetchLatestGithubVersion.opts | [364:15](../Configuration/YouTubeConfiguration.js#L364) |
| variable | define callback@113.fetchLatestGithubVersion.bust | [365:15](../Configuration/YouTubeConfiguration.js#L365) |
| function | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then callback@13242 | [371:19](../Configuration/YouTubeConfiguration.js#L371) |
| parameter | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then callback@13242.r | [371:19](../Configuration/YouTubeConfiguration.js#L371) |
| function | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then(r =&gt; {                 if (!r.ok) {                     console.log('[YT] /releases/latest returned', r.status);                     return null;                 }                 return r.json();             })             .then callback@13472 | [378:19](../Configuration/YouTubeConfiguration.js#L378) |
| parameter | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then(r =&gt; {                 if (!r.ok) {                     console.log('[YT] /releases/latest returned', r.status);                     return null;                 }                 return r.json();             })             .then callback@13472.json | [378:19](../Configuration/YouTubeConfiguration.js#L378) |
| function | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then(r =&gt; {                 if (!r.ok) {                     console.log('[YT] /releases/latest returned', r.status);                     return null;                 }                 return r.json();             })             .then(json =&gt; json && (json.tag_name \|\| json.name))             .catch callback@13537 | [379:20](../Configuration/YouTubeConfiguration.js#L379) |
| parameter | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then(r =&gt; {                 if (!r.ok) {                     console.log('[YT] /releases/latest returned', r.status);                     return null;                 }                 return r.json();             })             .then(json =&gt; json && (json.tag_name \|\| json.name))             .catch callback@13537.err | [379:20](../Configuration/YouTubeConfiguration.js#L379) |
| function | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then(r =&gt; {                 if (!r.ok) {                     console.log('[YT] /releases/latest returned', r.status);                     return null;                 }                 return r.json();             })             .then(json =&gt; json && (json.tag_name \|\| json.name))             .catch(err =&gt; { console.log('[YT] /releases/latest failed', err); return null; })             .then callback@13630 | [380:19](../Configuration/YouTubeConfiguration.js#L380) |
| parameter | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then(r =&gt; {                 if (!r.ok) {                     console.log('[YT] /releases/latest returned', r.status);                     return null;                 }                 return r.json();             })             .then(json =&gt; json && (json.tag_name \|\| json.name))             .catch(err =&gt; { console.log('[YT] /releases/latest failed', err); return null; })             .then callback@13630.tag | [380:19](../Configuration/YouTubeConfiguration.js#L380) |
| function | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then(r =&gt; {                 if (!r.ok) {                     console.log('[YT] /releases/latest returned', r.status);                     return null;                 }                 return r.json();             })             .then(json =&gt; json && (json.tag_name \|\| json.name))             .catch(err =&gt; { console.log('[YT] /releases/latest failed', err); return null; })             .then callback@13630.fetch('https://api.github.com/repos/' + repo + '/releases' + bust + '&per_page=5', opts)                     .then callback@13814 | [383:27](../Configuration/YouTubeConfiguration.js#L383) |
| parameter | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then(r =&gt; {                 if (!r.ok) {                     console.log('[YT] /releases/latest returned', r.status);                     return null;                 }                 return r.json();             })             .then(json =&gt; json && (json.tag_name \|\| json.name))             .catch(err =&gt; { console.log('[YT] /releases/latest failed', err); return null; })             .then callback@13630.fetch('https://api.github.com/repos/' + repo + '/releases' + bust + '&per_page=5', opts)                     .then callback@13814.r | [383:27](../Configuration/YouTubeConfiguration.js#L383) |
| function | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then(r =&gt; {                 if (!r.ok) {                     console.log('[YT] /releases/latest returned', r.status);                     return null;                 }                 return r.json();             })             .then(json =&gt; json && (json.tag_name \|\| json.name))             .catch(err =&gt; { console.log('[YT] /releases/latest failed', err); return null; })             .then callback@13630.fetch('https://api.github.com/repos/' + repo + '/releases' + bust + '&per_page=5', opts)                     .then(r =&gt; r.ok ? r.json() : null)                     .then callback@13869 | [384:27](../Configuration/YouTubeConfiguration.js#L384) |
| parameter | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then(r =&gt; {                 if (!r.ok) {                     console.log('[YT] /releases/latest returned', r.status);                     return null;                 }                 return r.json();             })             .then(json =&gt; json && (json.tag_name \|\| json.name))             .catch(err =&gt; { console.log('[YT] /releases/latest failed', err); return null; })             .then callback@13630.fetch('https://api.github.com/repos/' + repo + '/releases' + bust + '&per_page=5', opts)                     .then(r =&gt; r.ok ? r.json() : null)                     .then callback@13869.arr | [384:27](../Configuration/YouTubeConfiguration.js#L384) |
| variable | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then(r =&gt; {                 if (!r.ok) {                     console.log('[YT] /releases/latest returned', r.status);                     return null;                 }                 return r.json();             })             .then(json =&gt; json && (json.tag_name \|\| json.name))             .catch(err =&gt; { console.log('[YT] /releases/latest failed', err); return null; })             .then callback@13630.fetch('https://api.github.com/repos/' + repo + '/releases' + bust + '&per_page=5', opts)                     .then(r =&gt; r.ok ? r.json() : null)                     .then callback@13869.pick | [387:31](../Configuration/YouTubeConfiguration.js#L387) |
| function | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then(r =&gt; {                 if (!r.ok) {                     console.log('[YT] /releases/latest returned', r.status);                     return null;                 }                 return r.json();             })             .then(json =&gt; json && (json.tag_name \|\| json.name))             .catch(err =&gt; { console.log('[YT] /releases/latest failed', err); return null; })             .then callback@13630.fetch('https://api.github.com/repos/' + repo + '/releases' + bust + '&per_page=5', opts)                     .then(r =&gt; r.ok ? r.json() : null)                     .then callback@13869.arr.find callback@14084 | [387:47](../Configuration/YouTubeConfiguration.js#L387) |
| parameter | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then(r =&gt; {                 if (!r.ok) {                     console.log('[YT] /releases/latest returned', r.status);                     return null;                 }                 return r.json();             })             .then(json =&gt; json && (json.tag_name \|\| json.name))             .catch(err =&gt; { console.log('[YT] /releases/latest failed', err); return null; })             .then callback@13630.fetch('https://api.github.com/repos/' + repo + '/releases' + bust + '&per_page=5', opts)                     .then(r =&gt; r.ok ? r.json() : null)                     .then callback@13869.arr.find callback@14084.x | [387:47](../Configuration/YouTubeConfiguration.js#L387) |
| function | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then(r =&gt; {                 if (!r.ok) {                     console.log('[YT] /releases/latest returned', r.status);                     return null;                 }                 return r.json();             })             .then(json =&gt; json && (json.tag_name \|\| json.name))             .catch(err =&gt; { console.log('[YT] /releases/latest failed', err); return null; })             .then callback@13630.fetch('https://api.github.com/repos/' + repo + '/releases' + bust + '&per_page=5', opts)                     .then(r =&gt; r.ok ? r.json() : null)                     .then(arr =&gt; {                         if (!Array.isArray(arr) \|\| arr.length === 0) return null;                         // Prefer non-draft, anything else if all are drafts.                         const pick = arr.find(x =&gt; x && !x.draft) \|\| arr[0];                         return pick.tag_name \|\| pick.name \|\| null;                     })                     .catch callback@14232 | [390:28](../Configuration/YouTubeConfiguration.js#L390) |
| parameter | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then(r =&gt; {                 if (!r.ok) {                     console.log('[YT] /releases/latest returned', r.status);                     return null;                 }                 return r.json();             })             .then(json =&gt; json && (json.tag_name \|\| json.name))             .catch(err =&gt; { console.log('[YT] /releases/latest failed', err); return null; })             .then callback@13630.fetch('https://api.github.com/repos/' + repo + '/releases' + bust + '&per_page=5', opts)                     .then(r =&gt; r.ok ? r.json() : null)                     .then(arr =&gt; {                         if (!Array.isArray(arr) \|\| arr.length === 0) return null;                         // Prefer non-draft, anything else if all are drafts.                         const pick = arr.find(x =&gt; x && !x.draft) \|\| arr[0];                         return pick.tag_name \|\| pick.name \|\| null;                     })                     .catch callback@14232.err | [390:28](../Configuration/YouTubeConfiguration.js#L390) |
| function | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then(r =&gt; {                 if (!r.ok) {                     console.log('[YT] /releases/latest returned', r.status);                     return null;                 }                 return r.json();             })             .then(json =&gt; json && (json.tag_name \|\| json.name))             .catch(err =&gt; { console.log('[YT] /releases/latest failed', err); return null; })             .then(tag =&gt; {                 if (tag) return tag;                 return fetch('https://api.github.com/repos/' + repo + '/releases' + bust + '&per_page=5', opts)                     .then(r =&gt; r.ok ? r.json() : null)                     .then(arr =&gt; {                         if (!Array.isArray(arr) \|\| arr.length === 0) return null;                         // Prefer non-draft, anything else if all are drafts.                         const pick = arr.find(x =&gt; x && !x.draft) \|\| arr[0];                         return pick.tag_name \|\| pick.name \|\| null;                     })                     .catch(err =&gt; { console.log('[YT] /releases fallback failed', err); return null; });             })             .then callback@14343 | [392:19](../Configuration/YouTubeConfiguration.js#L392) |
| parameter | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then(r =&gt; {                 if (!r.ok) {                     console.log('[YT] /releases/latest returned', r.status);                     return null;                 }                 return r.json();             })             .then(json =&gt; json && (json.tag_name \|\| json.name))             .catch(err =&gt; { console.log('[YT] /releases/latest failed', err); return null; })             .then(tag =&gt; {                 if (tag) return tag;                 return fetch('https://api.github.com/repos/' + repo + '/releases' + bust + '&per_page=5', opts)                     .then(r =&gt; r.ok ? r.json() : null)                     .then(arr =&gt; {                         if (!Array.isArray(arr) \|\| arr.length === 0) return null;                         // Prefer non-draft, anything else if all are drafts.                         const pick = arr.find(x =&gt; x && !x.draft) \|\| arr[0];                         return pick.tag_name \|\| pick.name \|\| null;                     })                     .catch(err =&gt; { console.log('[YT] /releases fallback failed', err); return null; });             })             .then callback@14343.tag | [392:19](../Configuration/YouTubeConfiguration.js#L392) |
| catch-variable | define callback@113.fetchLatestGithubVersion.fetch('https://api.github.com/repos/' + repo + '/releases/latest' + bust, opts)             .then(r =&gt; {                 if (!r.ok) {                     console.log('[YT] /releases/latest returned', r.status);                     return null;                 }                 return r.json();             })             .then(json =&gt; json && (json.tag_name \|\| json.name))             .catch(err =&gt; { console.log('[YT] /releases/latest failed', err); return null; })             .then(tag =&gt; {                 if (tag) return tag;                 return fetch('https://api.github.com/repos/' + repo + '/releases' + bust + '&per_page=5', opts)                     .then(r =&gt; r.ok ? r.json() : null)                     .then(arr =&gt; {                         if (!Array.isArray(arr) \|\| arr.length === 0) return null;                         // Prefer non-draft, anything else if all are drafts.                         const pick = arr.find(x =&gt; x && !x.draft) \|\| arr[0];                         return pick.tag_name \|\| pick.name \|\| null;                     })                     .catch(err =&gt; { console.log('[YT] /releases fallback failed', err); return null; });             })             .then callback@14343.e | [398:109](../Configuration/YouTubeConfiguration.js#L398) |
| function | define callback@113.renderPluginCredits | [403:5](../Configuration/YouTubeConfiguration.js#L403) |
| parameter | define callback@113.renderPluginCredits.view | [403:34](../Configuration/YouTubeConfiguration.js#L403) |
| parameter | define callback@113.renderPluginCredits.apiClient | [403:40](../Configuration/YouTubeConfiguration.js#L403) |
| variable | define callback@113.renderPluginCredits.versionEl | [404:15](../Configuration/YouTubeConfiguration.js#L404) |
| variable | define callback@113.renderPluginCredits.badgeEl | [405:15](../Configuration/YouTubeConfiguration.js#L405) |
| variable | define callback@113.renderPluginCredits.showUpdateBadge | [408:15](../Configuration/YouTubeConfiguration.js#L408) |
| function | define callback@113.renderPluginCredits.showUpdateBadge | [408:33](../Configuration/YouTubeConfiguration.js#L408) |
| parameter | define callback@113.renderPluginCredits.showUpdateBadge.latest | [408:34](../Configuration/YouTubeConfiguration.js#L408) |
| variable | define callback@113.renderPluginCredits.finish | [415:15](../Configuration/YouTubeConfiguration.js#L415) |
| function | define callback@113.renderPluginCredits.finish | [415:24](../Configuration/YouTubeConfiguration.js#L415) |
| parameter | define callback@113.renderPluginCredits.finish.currentVersion | [415:25](../Configuration/YouTubeConfiguration.js#L415) |
| function | define callback@113.renderPluginCredits.finish.fetchLatestGithubVersion().then callback@15622 | [418:45](../Configuration/YouTubeConfiguration.js#L418) |
| parameter | define callback@113.renderPluginCredits.finish.fetchLatestGithubVersion().then callback@15622.latest | [418:45](../Configuration/YouTubeConfiguration.js#L418) |
| variable | define callback@113.renderPluginCredits.finish.fetchLatestGithubVersion().then callback@15622.cmp | [424:23](../Configuration/YouTubeConfiguration.js#L424) |
| variable | define callback@113.renderPluginCredits.findPlugin | [430:15](../Configuration/YouTubeConfiguration.js#L430) |
| function | define callback@113.renderPluginCredits.findPlugin | [430:28](../Configuration/YouTubeConfiguration.js#L430) |
| parameter | define callback@113.renderPluginCredits.findPlugin.plugins | [430:29](../Configuration/YouTubeConfiguration.js#L430) |
| variable | define callback@113.renderPluginCredits.findPlugin.wantA | [434:19](../Configuration/YouTubeConfiguration.js#L434) |
| variable | define callback@113.renderPluginCredits.findPlugin.wantB | [435:19](../Configuration/YouTubeConfiguration.js#L435) |
| variable | define callback@113.renderPluginCredits.findPlugin.byId | [436:19](../Configuration/YouTubeConfiguration.js#L436) |
| function | define callback@113.renderPluginCredits.findPlugin.plugins.find callback@16527 | [436:39](../Configuration/YouTubeConfiguration.js#L436) |
| parameter | define callback@113.renderPluginCredits.findPlugin.plugins.find callback@16527.p | [436:39](../Configuration/YouTubeConfiguration.js#L436) |
| variable | define callback@113.renderPluginCredits.findPlugin.plugins.find callback@16527.id | [437:23](../Configuration/YouTubeConfiguration.js#L437) |
| function | define callback@113.renderPluginCredits.findPlugin.plugins.find callback@16796 | [442:33](../Configuration/YouTubeConfiguration.js#L442) |
| parameter | define callback@113.renderPluginCredits.findPlugin.plugins.find callback@16796.p | [442:33](../Configuration/YouTubeConfiguration.js#L442) |
| function | define callback@113.renderPluginCredits.apiClient.getInstalledPlugins().then callback@17056 | [448:50](../Configuration/YouTubeConfiguration.js#L448) |
| parameter | define callback@113.renderPluginCredits.apiClient.getInstalledPlugins().then callback@17056.plugins | [448:50](../Configuration/YouTubeConfiguration.js#L448) |
| variable | define callback@113.renderPluginCredits.apiClient.getInstalledPlugins().then callback@17056.me | [449:23](../Configuration/YouTubeConfiguration.js#L449) |
| function | define callback@113.renderPluginCredits.apiClient.getInstalledPlugins().then callback@17180 | [451:16](../Configuration/YouTubeConfiguration.js#L451) |
| parameter | define callback@113.renderPluginCredits.apiClient.getInstalledPlugins().then callback@17180.err | [451:16](../Configuration/YouTubeConfiguration.js#L451) |
| class | define callback@113.anonymous-class@17331 | [457:12](../Configuration/YouTubeConfiguration.js#L457) |
| method | define callback@113.anonymous-class@17331.constructor | [458:9](../Configuration/YouTubeConfiguration.js#L458) |
| parameter | define callback@113.anonymous-class@17331.constructor.view | [458:21](../Configuration/YouTubeConfiguration.js#L458) |
| parameter | define callback@113.anonymous-class@17331.constructor.params | [458:27](../Configuration/YouTubeConfiguration.js#L458) |
| method | define callback@113.anonymous-class@17331.onResume | [473:9](../Configuration/YouTubeConfiguration.js#L473) |
| parameter | define callback@113.anonymous-class@17331.onResume.options | [473:18](../Configuration/YouTubeConfiguration.js#L473) |
| method | define callback@113.anonymous-class@17331.bindEventListeners | [483:9](../Configuration/YouTubeConfiguration.js#L483) |
| parameter | define callback@113.anonymous-class@17331.bindEventListeners.view | [483:28](../Configuration/YouTubeConfiguration.js#L483) |
| variable | define callback@113.anonymous-class@17331.bindEventListeners.form | [491:19](../Configuration/YouTubeConfiguration.js#L491) |
| function | define callback@113.anonymous-class@17331.bindEventListeners.form.addEventListener callback@18580 | [493:49](../Configuration/YouTubeConfiguration.js#L493) |
| parameter | define callback@113.anonymous-class@17331.bindEventListeners.form.addEventListener callback@18580.e | [493:50](../Configuration/YouTubeConfiguration.js#L493) |
| function | define callback@113.anonymous-class@17331.bindEventListeners.form.addEventListener callback@19173 | [504:49](../Configuration/YouTubeConfiguration.js#L504) |
| function | define callback@113.anonymous-class@17331.bindEventListeners.form.addEventListener callback@19256 | [505:48](../Configuration/YouTubeConfiguration.js#L505) |
| parameter | define callback@113.anonymous-class@17331.bindEventListeners.form.addEventListener callback@19256.e | [505:49](../Configuration/YouTubeConfiguration.js#L505) |
| variable | define callback@113.anonymous-class@17331.bindEventListeners.form.addEventListener callback@19256.id | [509:27](../Configuration/YouTubeConfiguration.js#L509) |
| function | define callback@113.anonymous-class@17331.bindEventListeners.this.bindEntryControls callback@19938 | [522:17](../Configuration/YouTubeConfiguration.js#L522) |
| function | define callback@113.anonymous-class@17331.bindEventListeners.this.bindEntryControls callback@19977 | [523:17](../Configuration/YouTubeConfiguration.js#L523) |
| parameter | define callback@113.anonymous-class@17331.bindEventListeners.this.bindEntryControls callback@19977.value | [523:17](../Configuration/YouTubeConfiguration.js#L523) |
| function | define callback@113.anonymous-class@17331.bindEventListeners.this.bindEntryControls callback@20032 | [524:17](../Configuration/YouTubeConfiguration.js#L524) |
| parameter | define callback@113.anonymous-class@17331.bindEventListeners.this.bindEntryControls callback@20032.item | [524:17](../Configuration/YouTubeConfiguration.js#L524) |
| function | define callback@113.anonymous-class@17331.bindEventListeners.this.bindEntryControls callback@20081 | [525:17](../Configuration/YouTubeConfiguration.js#L525) |
| parameter | define callback@113.anonymous-class@17331.bindEventListeners.this.bindEntryControls callback@20081.item | [525:17](../Configuration/YouTubeConfiguration.js#L525) |
| variable | define callback@113.anonymous-class@17331.bindEventListeners.this.bindEntryControls callback@20081.kind | [526:27](../Configuration/YouTubeConfiguration.js#L526) |
| function | define callback@113.anonymous-class@17331.bindEventListeners.this.bindEntryControls callback@20515 | [536:17](../Configuration/YouTubeConfiguration.js#L536) |
| function | define callback@113.anonymous-class@17331.bindEventListeners.this.bindEntryControls callback@20559 | [537:17](../Configuration/YouTubeConfiguration.js#L537) |
| parameter | define callback@113.anonymous-class@17331.bindEventListeners.this.bindEntryControls callback@20559.value | [537:17](../Configuration/YouTubeConfiguration.js#L537) |
| function | define callback@113.anonymous-class@17331.bindEventListeners.this.bindEntryControls callback@20619 | [538:17](../Configuration/YouTubeConfiguration.js#L538) |
| parameter | define callback@113.anonymous-class@17331.bindEventListeners.this.bindEntryControls callback@20619.item | [538:17](../Configuration/YouTubeConfiguration.js#L538) |
| function | define callback@113.anonymous-class@17331.bindEventListeners.this.bindEntryControls callback@20675 | [539:17](../Configuration/YouTubeConfiguration.js#L539) |
| parameter | define callback@113.anonymous-class@17331.bindEventListeners.this.bindEntryControls callback@20675.item | [539:17](../Configuration/YouTubeConfiguration.js#L539) |
| method | define callback@113.anonymous-class@17331.bindEntryControls | [543:9](../Configuration/YouTubeConfiguration.js#L543) |
| parameter | define callback@113.anonymous-class@17331.bindEntryControls.view | [543:27](../Configuration/YouTubeConfiguration.js#L543) |
| parameter | define callback@113.anonymous-class@17331.bindEntryControls.inputId | [543:33](../Configuration/YouTubeConfiguration.js#L543) |
| parameter | define callback@113.anonymous-class@17331.bindEntryControls.buttonId | [543:42](../Configuration/YouTubeConfiguration.js#L543) |
| parameter | define callback@113.anonymous-class@17331.bindEntryControls.listId | [543:52](../Configuration/YouTubeConfiguration.js#L543) |
| parameter | define callback@113.anonymous-class@17331.bindEntryControls.getItems | [543:60](../Configuration/YouTubeConfiguration.js#L543) |
| parameter | define callback@113.anonymous-class@17331.bindEntryControls.setItems | [543:70](../Configuration/YouTubeConfiguration.js#L543) |
| parameter | define callback@113.anonymous-class@17331.bindEntryControls.classify | [543:80](../Configuration/YouTubeConfiguration.js#L543) |
| parameter | define callback@113.anonymous-class@17331.bindEntryControls.validate | [543:90](../Configuration/YouTubeConfiguration.js#L543) |
| parameter | define callback@113.anonymous-class@17331.bindEntryControls.invalidMessage | [543:100](../Configuration/YouTubeConfiguration.js#L543) |
| variable | define callback@113.anonymous-class@17331.bindEntryControls.input | [544:19](../Configuration/YouTubeConfiguration.js#L544) |
| variable | define callback@113.anonymous-class@17331.bindEntryControls.button | [545:19](../Configuration/YouTubeConfiguration.js#L545) |
| variable | define callback@113.anonymous-class@17331.bindEntryControls.add | [546:19](../Configuration/YouTubeConfiguration.js#L546) |
| function | define callback@113.anonymous-class@17331.bindEntryControls.add | [546:25](../Configuration/YouTubeConfiguration.js#L546) |
| variable | define callback@113.anonymous-class@17331.bindEntryControls.add.newItems | [547:23](../Configuration/YouTubeConfiguration.js#L547) |
| variable | define callback@113.anonymous-class@17331.bindEntryControls.add.acceptedItems | [552:23](../Configuration/YouTubeConfiguration.js#L552) |
| function | define callback@113.anonymous-class@17331.bindEntryControls.input.addEventListener callback@22162 | [573:51](../Configuration/YouTubeConfiguration.js#L573) |
| parameter | define callback@113.anonymous-class@17331.bindEntryControls.input.addEventListener callback@22162.e | [573:51](../Configuration/YouTubeConfiguration.js#L573) |
| method | define callback@113.anonymous-class@17331.renderEntryList | [582:9](../Configuration/YouTubeConfiguration.js#L582) |
| parameter | define callback@113.anonymous-class@17331.renderEntryList.view | [582:25](../Configuration/YouTubeConfiguration.js#L582) |
| parameter | define callback@113.anonymous-class@17331.renderEntryList.listId | [582:31](../Configuration/YouTubeConfiguration.js#L582) |
| parameter | define callback@113.anonymous-class@17331.renderEntryList.getItems | [582:39](../Configuration/YouTubeConfiguration.js#L582) |
| parameter | define callback@113.anonymous-class@17331.renderEntryList.setItems | [582:49](../Configuration/YouTubeConfiguration.js#L582) |
| parameter | define callback@113.anonymous-class@17331.renderEntryList.classify | [582:59](../Configuration/YouTubeConfiguration.js#L582) |
| variable | define callback@113.anonymous-class@17331.renderEntryList.list | [583:19](../Configuration/YouTubeConfiguration.js#L583) |
| variable | define callback@113.anonymous-class@17331.renderEntryList.items | [589:19](../Configuration/YouTubeConfiguration.js#L589) |
| variable | define callback@113.anonymous-class@17331.renderEntryList.empty | [592:23](../Configuration/YouTubeConfiguration.js#L592) |
| variable | define callback@113.anonymous-class@17331.renderEntryList.header | [599:19](../Configuration/YouTubeConfiguration.js#L599) |
| function | define callback@113.anonymous-class@17331.renderEntryList.items.forEach callback@23265 | [606:27](../Configuration/YouTubeConfiguration.js#L606) |
| parameter | define callback@113.anonymous-class@17331.renderEntryList.items.forEach callback@23265.item | [606:27](../Configuration/YouTubeConfiguration.js#L606) |
| variable | define callback@113.anonymous-class@17331.renderEntryList.items.forEach callback@23265.chip | [607:23](../Configuration/YouTubeConfiguration.js#L607) |
| variable | define callback@113.anonymous-class@17331.renderEntryList.items.forEach callback@23265.text | [610:23](../Configuration/YouTubeConfiguration.js#L610) |
| variable | define callback@113.anonymous-class@17331.renderEntryList.items.forEach callback@23265.badge | [615:23](../Configuration/YouTubeConfiguration.js#L615) |
| variable | define callback@113.anonymous-class@17331.renderEntryList.items.forEach callback@23265.remove | [620:23](../Configuration/YouTubeConfiguration.js#L620) |
| function | define callback@113.anonymous-class@17331.renderEntryList.items.forEach callback@23265.remove.addEventListener callback@24154 | [626:50](../Configuration/YouTubeConfiguration.js#L626) |
| function | define callback@113.anonymous-class@17331.renderEntryList.items.forEach callback@23265.remove.addEventListener callback@24154.getItems().filter callback@24209 | [627:48](../Configuration/YouTubeConfiguration.js#L627) |
| parameter | define callback@113.anonymous-class@17331.renderEntryList.items.forEach callback@23265.remove.addEventListener callback@24154.getItems().filter callback@24209.existing | [627:48](../Configuration/YouTubeConfiguration.js#L627) |
| method | define callback@113.anonymous-class@17331.syncEntryFields | [638:9](../Configuration/YouTubeConfiguration.js#L638) |
| parameter | define callback@113.anonymous-class@17331.syncEntryFields.view | [638:25](../Configuration/YouTubeConfiguration.js#L638) |
| method | define callback@113.anonymous-class@17331.renderAllEntryLists | [643:9](../Configuration/YouTubeConfiguration.js#L643) |
| parameter | define callback@113.anonymous-class@17331.renderAllEntryLists.view | [643:29](../Configuration/YouTubeConfiguration.js#L643) |
| function | define callback@113.anonymous-class@17331.renderAllEntryLists.this.renderEntryList callback@24899 | [647:17](../Configuration/YouTubeConfiguration.js#L647) |
| function | define callback@113.anonymous-class@17331.renderAllEntryLists.this.renderEntryList callback@24938 | [648:17](../Configuration/YouTubeConfiguration.js#L648) |
| parameter | define callback@113.anonymous-class@17331.renderAllEntryLists.this.renderEntryList callback@24938.value | [648:17](../Configuration/YouTubeConfiguration.js#L648) |
| function | define callback@113.anonymous-class@17331.renderAllEntryLists.this.renderEntryList callback@24993 | [649:17](../Configuration/YouTubeConfiguration.js#L649) |
| parameter | define callback@113.anonymous-class@17331.renderAllEntryLists.this.renderEntryList callback@24993.item | [649:17](../Configuration/YouTubeConfiguration.js#L649) |
| function | define callback@113.anonymous-class@17331.renderAllEntryLists.this.renderEntryList callback@25139 | [654:17](../Configuration/YouTubeConfiguration.js#L654) |
| function | define callback@113.anonymous-class@17331.renderAllEntryLists.this.renderEntryList callback@25183 | [655:17](../Configuration/YouTubeConfiguration.js#L655) |
| parameter | define callback@113.anonymous-class@17331.renderAllEntryLists.this.renderEntryList callback@25183.value | [655:17](../Configuration/YouTubeConfiguration.js#L655) |
| function | define callback@113.anonymous-class@17331.renderAllEntryLists.this.renderEntryList callback@25243 | [656:17](../Configuration/YouTubeConfiguration.js#L656) |
| parameter | define callback@113.anonymous-class@17331.renderAllEntryLists.this.renderEntryList callback@25243.item | [656:17](../Configuration/YouTubeConfiguration.js#L656) |
| method | define callback@113.anonymous-class@17331.refreshTrendingRegions | [659:9](../Configuration/YouTubeConfiguration.js#L659) |
| parameter | define callback@113.anonymous-class@17331.refreshTrendingRegions.view | [659:32](../Configuration/YouTubeConfiguration.js#L659) |
| parameter | define callback@113.anonymous-class@17331.refreshTrendingRegions.apiClient | [659:38](../Configuration/YouTubeConfiguration.js#L659) |
| parameter | define callback@113.anonymous-class@17331.refreshTrendingRegions.loadRevision | [659:49](../Configuration/YouTubeConfiguration.js#L659) |
| parameter | define callback@113.anonymous-class@17331.refreshTrendingRegions.apiKeyAtRequest | [659:63](../Configuration/YouTubeConfiguration.js#L659) |
| variable | define callback@113.anonymous-class@17331.refreshTrendingRegions.lookupRevision | [660:19](../Configuration/YouTubeConfiguration.js#L660) |
| variable | define callback@113.anonymous-class@17331.refreshTrendingRegions.selectedBeforeLookup | [661:19](../Configuration/YouTubeConfiguration.js#L661) |
| function | define callback@113.anonymous-class@17331.refreshTrendingRegions.fetchTrendingRegions(apiClient).then callback@25653 | [664:57](../Configuration/YouTubeConfiguration.js#L664) |
| parameter | define callback@113.anonymous-class@17331.refreshTrendingRegions.fetchTrendingRegions(apiClient).then callback@25653.regionResult | [664:57](../Configuration/YouTubeConfiguration.js#L664) |
| variable | define callback@113.anonymous-class@17331.refreshTrendingRegions.fetchTrendingRegions(apiClient).then callback@25653.displayResult | [677:23](../Configuration/YouTubeConfiguration.js#L677) |
| variable | define callback@113.anonymous-class@17331.refreshTrendingRegions.fetchTrendingRegions(apiClient).then callback@25653.resetUnsupported | [680:23](../Configuration/YouTubeConfiguration.js#L680) |
| method | define callback@113.anonymous-class@17331.loadData | [691:9](../Configuration/YouTubeConfiguration.js#L691) |
| parameter | define callback@113.anonymous-class@17331.loadData.view | [691:18](../Configuration/YouTubeConfiguration.js#L691) |
| variable | define callback@113.anonymous-class@17331.loadData.apiClient | [694:19](../Configuration/YouTubeConfiguration.js#L694) |
| variable | define callback@113.anonymous-class@17331.loadData.loadRevision | [712:19](../Configuration/YouTubeConfiguration.js#L712) |
| variable | define callback@113.anonymous-class@17331.loadData.requestWhenIdle | [713:19](../Configuration/YouTubeConfiguration.js#L713) |
| function | define callback@113.anonymous-class@17331.loadData.requestWhenIdle | [713:37](../Configuration/YouTubeConfiguration.js#L713) |
| variable | define callback@113.anonymous-class@17331.loadData.requestWhenIdle.saveRevisionAtRequest | [722:23](../Configuration/YouTubeConfiguration.js#L722) |
| function | define callback@113.anonymous-class@17331.loadData.requestWhenIdle.apiClient.getPluginConfiguration(pluginId).then callback@28199 | [724:65](../Configuration/YouTubeConfiguration.js#L724) |
| parameter | define callback@113.anonymous-class@17331.loadData.requestWhenIdle.apiClient.getPluginConfiguration(pluginId).then callback@28199.loadedConfig | [724:65](../Configuration/YouTubeConfiguration.js#L724) |
| variable | define callback@113.anonymous-class@17331.loadData.requestWhenIdle.apiClient.getPluginConfiguration(pluginId).then callback@28199.config | [737:27](../Configuration/YouTubeConfiguration.js#L737) |
| variable | define callback@113.anonymous-class@17331.loadData.requestWhenIdle.apiClient.getPluginConfiguration(pluginId).then callback@28199.resetFromKnownRegions | [748:27](../Configuration/YouTubeConfiguration.js#L748) |
| function | define callback@113.anonymous-class@17331.loadData.requestWhenIdle.apiClient.getPluginConfiguration(pluginId).then callback@31814 | [781:20](../Configuration/YouTubeConfiguration.js#L781) |
| parameter | define callback@113.anonymous-class@17331.loadData.requestWhenIdle.apiClient.getPluginConfiguration(pluginId).then callback@31814.error | [781:20](../Configuration/YouTubeConfiguration.js#L781) |
| method | define callback@113.anonymous-class@17331.saveData | [801:9](../Configuration/YouTubeConfiguration.js#L801) |
| parameter | define callback@113.anonymous-class@17331.saveData.view | [801:18](../Configuration/YouTubeConfiguration.js#L801) |
| variable | define callback@113.anonymous-class@17331.saveData.apiClient | [802:19](../Configuration/YouTubeConfiguration.js#L802) |
| variable | define callback@113.anonymous-class@17331.saveData.config | [808:19](../Configuration/YouTubeConfiguration.js#L808) |
| method | define callback@113.anonymous-class@17331.flushSaveQueue | [858:9](../Configuration/YouTubeConfiguration.js#L858) |
| variable | define callback@113.anonymous-class@17331.flushSaveQueue.pending | [863:19](../Configuration/YouTubeConfiguration.js#L863) |
| variable | define callback@113.anonymous-class@17331.flushSaveQueue.view | [870:19](../Configuration/YouTubeConfiguration.js#L870) |
| function | define callback@113.anonymous-class@17331.flushSaveQueue.pending.apiClient.ajax({                 type: 'POST',                 url: pending.apiClient.getUrl('YouTubePlugin/SaveConfiguration'),                 data: JSON.stringify(pending.config),                 contentType: 'application/json'             }).then callback@36523 | [877:21](../Configuration/YouTubeConfiguration.js#L877) |
| variable | define callback@113.anonymous-class@17331.flushSaveQueue.pending.apiClient.ajax({                 type: 'POST',                 url: pending.apiClient.getUrl('YouTubePlugin/SaveConfiguration'),                 data: JSON.stringify(pending.config),                 contentType: 'application/json'             }).then callback@36523.savedApiKey | [882:27](../Configuration/YouTubeConfiguration.js#L882) |
| function | define callback@113.anonymous-class@17331.flushSaveQueue.pending.apiClient.ajax({                 type: 'POST',                 url: pending.apiClient.getUrl('YouTubePlugin/SaveConfiguration'),                 data: JSON.stringify(pending.config),                 contentType: 'application/json'             }).then callback@37066 | [887:16](../Configuration/YouTubeConfiguration.js#L887) |
| parameter | define callback@113.anonymous-class@17331.flushSaveQueue.pending.apiClient.ajax({                 type: 'POST',                 url: pending.apiClient.getUrl('YouTubePlugin/SaveConfiguration'),                 data: JSON.stringify(pending.config),                 contentType: 'application/json'             }).then callback@37066.error | [887:16](../Configuration/YouTubeConfiguration.js#L887) |
| function | define callback@113.anonymous-class@17331.flushSaveQueue.pending.apiClient.ajax({                 type: 'POST',                 url: pending.apiClient.getUrl('YouTubePlugin/SaveConfiguration'),                 data: JSON.stringify(pending.config),                 contentType: 'application/json'             }).then(() =&gt; {                 this.config = pending.config;                 setDonateButton(view);                 if (!this._pendingSave && pending.revision === this._saveRevision) {                     setStatus(view, 'Saved automatically.');                     const savedApiKey = String(pending.config.ApiKey \|\| '').trim();                     if (savedApiKey !== this._regionLookupApiKey) {                         this.refreshTrendingRegions(view, pending.apiClient, null, savedApiKey);                     }                 }             }, error =&gt; {                 console.error('Error saving YouTube configuration:', error);                 if (!this._pendingSave && pending.revision === this._saveRevision) {                     setStatus(view, 'Could not save plugin settings.');                     showToast('error', 'Could not save YouTube settings');                 }             }).then callback@37424 | [893:21](../Configuration/YouTubeConfiguration.js#L893) |
| function | define callback@113.anonymous-class@17331.flushSaveQueue.pending.apiClient.ajax({                 type: 'POST',                 url: pending.apiClient.getUrl('YouTubePlugin/SaveConfiguration'),                 data: JSON.stringify(pending.config),                 contentType: 'application/json'             }).then(() =&gt; {                 this.config = pending.config;                 setDonateButton(view);                 if (!this._pendingSave && pending.revision === this._saveRevision) {                     setStatus(view, 'Saved automatically.');                     const savedApiKey = String(pending.config.ApiKey \|\| '').trim();                     if (savedApiKey !== this._regionLookupApiKey) {                         this.refreshTrendingRegions(view, pending.apiClient, null, savedApiKey);                     }                 }             }, error =&gt; {                 console.error('Error saving YouTube configuration:', error);                 if (!this._pendingSave && pending.revision === this._saveRevision) {                     setStatus(view, 'Could not save plugin settings.');                     showToast('error', 'Could not save YouTube settings');                 }             }).then callback@37530 | [896:16](../Configuration/YouTubeConfiguration.js#L896) |
| parameter | define callback@113.anonymous-class@17331.flushSaveQueue.pending.apiClient.ajax({                 type: 'POST',                 url: pending.apiClient.getUrl('YouTubePlugin/SaveConfiguration'),                 data: JSON.stringify(pending.config),                 contentType: 'application/json'             }).then(() =&gt; {                 this.config = pending.config;                 setDonateButton(view);                 if (!this._pendingSave && pending.revision === this._saveRevision) {                     setStatus(view, 'Saved automatically.');                     const savedApiKey = String(pending.config.ApiKey \|\| '').trim();                     if (savedApiKey !== this._regionLookupApiKey) {                         this.refreshTrendingRegions(view, pending.apiClient, null, savedApiKey);                     }                 }             }, error =&gt; {                 console.error('Error saving YouTube configuration:', error);                 if (!this._pendingSave && pending.revision === this._saveRevision) {                     setStatus(view, 'Could not save plugin settings.');                     showToast('error', 'Could not save YouTube settings');                 }             }).then callback@37530.error | [896:16](../Configuration/YouTubeConfiguration.js#L896) |
| method | define callback@113.anonymous-class@17331.scheduleAutoSave | [903:9](../Configuration/YouTubeConfiguration.js#L903) |
| parameter | define callback@113.anonymous-class@17331.scheduleAutoSave.view | [903:26](../Configuration/YouTubeConfiguration.js#L903) |
| function | define callback@113.anonymous-class@17331.scheduleAutoSave.setTimeout callback@38082 | [911:46](../Configuration/YouTubeConfiguration.js#L911) |

</details>

<a id="file-5f31de67e5ef8dbe"></a>
## [Configuration/guide/step1.png](../Configuration/guide/step1.png)

EmbeddedResource; 83617 bytes.

<a id="file-ed44b448e7d3eaa3"></a>
## [Configuration/guide/step2.png](../Configuration/guide/step2.png)

EmbeddedResource; 113394 bytes.

<a id="file-8b116519690d79c6"></a>
## [Configuration/guide/step34.png](../Configuration/guide/step34.png)

EmbeddedResource; 176004 bytes.

<a id="file-d5cc51fd66e047cb"></a>
## [Configuration/guide/step5.png](../Configuration/guide/step5.png)

EmbeddedResource; 103635 bytes.

<a id="file-6e5d5c4e6b423e11"></a>
## [Configuration/guide/step6.png](../Configuration/guide/step6.png)

EmbeddedResource; 67648 bytes.

<a id="file-795fc5e8d5423b25"></a>
## [Configuration/guide/step7.png](../Configuration/guide/step7.png)

EmbeddedResource; 35167 bytes.

<a id="file-7e925ff01358802b"></a>
## [Configuration/guide/step8.png](../Configuration/guide/step8.png)

EmbeddedResource; 67506 bytes.

<a id="file-d1ed9d5e47f4e374"></a>
## [DashboardYouTubePlayerInterceptor.cs](../DashboardYouTubePlayerInterceptor.cs)

Plugin Compile source (SDK default); 67192 bytes; 1168 lines.

<details>
<summary>All 269 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [11:1](../DashboardYouTubePlayerInterceptor.cs#L11) |
| type | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor | [13:5](../DashboardYouTubePlayerInterceptor.cs#L13) |
| field | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.HarmonyId | [15:30](../DashboardYouTubePlayerInterceptor.cs#L15) |
| field | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchMarker | [16:30](../DashboardYouTubePlayerInterceptor.cs#L16) |
| field | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.AppJsPatchMarker | [17:30](../DashboardYouTubePlayerInterceptor.cs#L17) |
| field | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.VideoOsdPatchMarker | [18:30](../DashboardYouTubePlayerInterceptor.cs#L18) |
| field | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PlaybackManagerPatchMarker | [19:30](../DashboardYouTubePlayerInterceptor.cs#L19) |
| field | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Sync | [20:40](../DashboardYouTubePlayerInterceptor.cs#L20) |
| field | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor._harmony | [21:32](../DashboardYouTubePlayerInterceptor.cs#L21) |
| field | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor._harmonyType | [22:30](../DashboardYouTubePlayerInterceptor.cs#L22) |
| field | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor._patchedResponseLogsRemaining | [23:28](../DashboardYouTubePlayerInterceptor.cs#L23) |
| field | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor._optionalPatchLogsRemaining | [24:28](../DashboardYouTubePlayerInterceptor.cs#L24) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Install | [26:9](../DashboardYouTubePlayerInterceptor.cs#L26) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Install.webAppServiceType | [35:25](../DashboardYouTubePlayerInterceptor.cs#L35) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Install.dashboardResourceType | [36:25](../DashboardYouTubePlayerInterceptor.cs#L36) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Install.harmonyAssembly | [43:25](../DashboardYouTubePlayerInterceptor.cs#L43) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Install.harmonyType | [44:25](../DashboardYouTubePlayerInterceptor.cs#L44) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Install.harmonyMethodType | [45:25](../DashboardYouTubePlayerInterceptor.cs#L45) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Install.prefixMethod | [52:25](../DashboardYouTubePlayerInterceptor.cs#L52) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Install.method | [60:25](../DashboardYouTubePlayerInterceptor.cs#L60) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Install.prefix | [72:25](../DashboardYouTubePlayerInterceptor.cs#L72) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Install.harmony | [73:25](../DashboardYouTubePlayerInterceptor.cs#L73) |
| catch-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Install.ex | [85:23](../DashboardYouTubePlayerInterceptor.cs#L85) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Uninstall | [92:9](../DashboardYouTubePlayerInterceptor.cs#L92) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Uninstall.unpatchAll | [98:25](../DashboardYouTubePlayerInterceptor.cs#L98) |
| anonymous-function | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Uninstall.unpatchAll.anonymous@4510 | [99:41](../DashboardYouTubePlayerInterceptor.cs#L99) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Uninstall.unpatchAll.anonymous@4510.m | [99:41](../DashboardYouTubePlayerInterceptor.cs#L99) |
| catch-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Uninstall.ex | [106:23](../DashboardYouTubePlayerInterceptor.cs#L106) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetDashboardResourcePrefix | [113:9](../DashboardYouTubePlayerInterceptor.cs#L113) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetDashboardResourcePrefix.__instance | [113:56](../DashboardYouTubePlayerInterceptor.cs#L113) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetDashboardResourcePrefix.__0 | [113:75](../DashboardYouTubePlayerInterceptor.cs#L113) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetDashboardResourcePrefix.__result | [113:87](../DashboardYouTubePlayerInterceptor.cs#L113) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetDashboardResourcePrefix.resourceName | [119:21](../DashboardYouTubePlayerInterceptor.cs#L119) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetDashboardResourcePrefix.diagRequest | [126:25](../DashboardYouTubePlayerInterceptor.cs#L126) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetDashboardResourcePrefix.diagResultFactory | [127:25](../DashboardYouTubePlayerInterceptor.cs#L127) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetDashboardResourcePrefix.patchedBytes | [145:24](../DashboardYouTubePlayerInterceptor.cs#L145) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetDashboardResourcePrefix.request | [156:21](../DashboardYouTubePlayerInterceptor.cs#L156) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetDashboardResourcePrefix.resultFactory | [157:21](../DashboardYouTubePlayerInterceptor.cs#L157) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetDashboardResourcePrefix.result | [162:21](../DashboardYouTubePlayerInterceptor.cs#L162) |
| catch-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetDashboardResourcePrefix.ex | [179:19](../DashboardYouTubePlayerInterceptor.cs#L179) |
| field | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor._diagLogsRemaining | [186:28](../DashboardYouTubePlayerInterceptor.cs#L186) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.IsDiagResource | [188:9](../DashboardYouTubePlayerInterceptor.cs#L188) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.IsDiagResource.resourceName | [188:44](../DashboardYouTubePlayerInterceptor.cs#L188) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.IsDiagResource.normalized | [190:17](../DashboardYouTubePlayerInterceptor.cs#L190) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.LogDiagIfMatch | [194:9](../DashboardYouTubePlayerInterceptor.cs#L194) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.LogDiagIfMatch.service | [194:44](../DashboardYouTubePlayerInterceptor.cs#L194) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.LogDiagIfMatch.resourceName | [194:60](../DashboardYouTubePlayerInterceptor.cs#L194) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.LogDiagIfMatch.request | [204:25](../DashboardYouTubePlayerInterceptor.cs#L204) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.LogDiagIfMatch.msg | [205:25](../DashboardYouTubePlayerInterceptor.cs#L205) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.NoCacheHeaders | [215:9](../DashboardYouTubePlayerInterceptor.cs#L215) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.DescribeRequest | [223:9](../DashboardYouTubePlayerInterceptor.cs#L223) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.DescribeRequest.request | [223:47](../DashboardYouTubePlayerInterceptor.cs#L223) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.DescribeRequest.client | [228:17](../DashboardYouTubePlayerInterceptor.cs#L228) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.DescribeRequest.device | [229:17](../DashboardYouTubePlayerInterceptor.cs#L229) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.DescribeRequest.ip | [230:17](../DashboardYouTubePlayerInterceptor.cs#L230) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.DescribeRequest.userAgent | [234:17](../DashboardYouTubePlayerInterceptor.cs#L234) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.RequestValue | [238:9](../DashboardYouTubePlayerInterceptor.cs#L238) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.RequestValue.request | [238:45](../DashboardYouTubePlayerInterceptor.cs#L238) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.RequestValue.name | [238:63](../DashboardYouTubePlayerInterceptor.cs#L238) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Shorten | [243:9](../DashboardYouTubePlayerInterceptor.cs#L243) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Shorten.value | [243:39](../DashboardYouTubePlayerInterceptor.cs#L243) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.Shorten.maxLength | [243:53](../DashboardYouTubePlayerInterceptor.cs#L243) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.TryReadAndPatchResource | [251:9](../DashboardYouTubePlayerInterceptor.cs#L251) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.TryReadAndPatchResource.service | [251:53](../DashboardYouTubePlayerInterceptor.cs#L251) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.TryReadAndPatchResource.resourceName | [251:69](../DashboardYouTubePlayerInterceptor.cs#L251) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.TryReadAndPatchResource.patchedBytes | [251:90](../DashboardYouTubePlayerInterceptor.cs#L251) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.TryReadAndPatchResource.dashboardPath | [255:17](../DashboardYouTubePlayerInterceptor.cs#L255) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.TryReadAndPatchResource.normalizedResourceName | [259:17](../DashboardYouTubePlayerInterceptor.cs#L259) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.TryReadAndPatchResource.dashboardFullPath | [260:17](../DashboardYouTubePlayerInterceptor.cs#L260) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.TryReadAndPatchResource.resourceFullPath | [261:17](../DashboardYouTubePlayerInterceptor.cs#L261) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.TryReadAndPatchResource.original | [271:17](../DashboardYouTubePlayerInterceptor.cs#L271) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.TryReadAndPatchResource.patched | [272:17](../DashboardYouTubePlayerInterceptor.cs#L272) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.TryReadEmbeddedPlayer | [284:9](../DashboardYouTubePlayerInterceptor.cs#L284) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.TryReadEmbeddedPlayer.content | [284:51](../DashboardYouTubePlayerInterceptor.cs#L284) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.TryReadEmbeddedPlayer.assembly | [289:21](../DashboardYouTubePlayerInterceptor.cs#L289) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.TryReadEmbeddedPlayer.resourceName | [290:21](../DashboardYouTubePlayerInterceptor.cs#L290) |
| anonymous-function | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.TryReadEmbeddedPlayer.resourceName.anonymous@12287 | [291:37](../DashboardYouTubePlayerInterceptor.cs#L291) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.TryReadEmbeddedPlayer.resourceName.anonymous@12287.name | [291:37](../DashboardYouTubePlayerInterceptor.cs#L291) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.TryReadEmbeddedPlayer.stream | [295:27](../DashboardYouTubePlayerInterceptor.cs#L295) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.TryReadEmbeddedPlayer.buffer | [299:27](../DashboardYouTubePlayerInterceptor.cs#L299) |
| catch-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.TryReadEmbeddedPlayer.ex | [304:19](../DashboardYouTubePlayerInterceptor.cs#L304) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchResource | [311:9](../DashboardYouTubePlayerInterceptor.cs#L311) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchResource.normalizedResourceName | [311:45](../DashboardYouTubePlayerInterceptor.cs#L311) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchResource.source | [311:76](../DashboardYouTubePlayerInterceptor.cs#L311) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchVideoOsd | [334:9](../DashboardYouTubePlayerInterceptor.cs#L334) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchVideoOsd.source | [334:45](../DashboardYouTubePlayerInterceptor.cs#L334) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchVideoOsd.pauseStartBefore | [340:26](../DashboardYouTubePlayerInterceptor.cs#L340) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchVideoOsd.pauseStartAfter | [342:26](../DashboardYouTubePlayerInterceptor.cs#L342) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchVideoOsd.exitGateBefore | [344:26](../DashboardYouTubePlayerInterceptor.cs#L344) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchVideoOsd.exitGateAfter | [346:26](../DashboardYouTubePlayerInterceptor.cs#L346) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchVideoOsd.stopBefore | [348:26](../DashboardYouTubePlayerInterceptor.cs#L348) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchVideoOsd.stopAfter | [350:26](../DashboardYouTubePlayerInterceptor.cs#L350) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchVideoOsd.patched | [353:17](../DashboardYouTubePlayerInterceptor.cs#L353) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchAppJs | [361:9](../DashboardYouTubePlayerInterceptor.cs#L361) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchAppJs.source | [361:42](../DashboardYouTubePlayerInterceptor.cs#L361) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchAppJs.patched | [363:17](../DashboardYouTubePlayerInterceptor.cs#L363) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchAppJs.registrationPrefix | [367:30](../DashboardYouTubePlayerInterceptor.cs#L367) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchAppJs.registrationPrefixWithIos | [369:30](../DashboardYouTubePlayerInterceptor.cs#L369) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchAppJs.replacement | [371:21](../DashboardYouTubePlayerInterceptor.cs#L371) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchAppJs.registrationAnchor | [377:21](../DashboardYouTubePlayerInterceptor.cs#L377) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchAppJs.token | [404:17](../DashboardYouTubePlayerInterceptor.cs#L404) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchPlaybackManager | [450:9](../DashboardYouTubePlayerInterceptor.cs#L450) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchPlaybackManager.source | [450:52](../DashboardYouTubePlayerInterceptor.cs#L450) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchPlaybackManager.currentBefore | [460:26](../DashboardYouTubePlayerInterceptor.cs#L460) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchPlaybackManager.currentAfter | [462:26](../DashboardYouTubePlayerInterceptor.cs#L462) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchPlaybackManager.before | [468:26](../DashboardYouTubePlayerInterceptor.cs#L468) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchPlaybackManager.after | [470:26](../DashboardYouTubePlayerInterceptor.cs#L470) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchPlaybackManager.patched | [473:17](../DashboardYouTubePlayerInterceptor.cs#L473) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchIframePlayer | [483:9](../DashboardYouTubePlayerInterceptor.cs#L483) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchIframePlayer.source | [483:49](../DashboardYouTubePlayerInterceptor.cs#L483) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchIframePlayer.patched | [488:17](../DashboardYouTubePlayerInterceptor.cs#L488) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchIframePlayer.currentStart | [494:26](../DashboardYouTubePlayerInterceptor.cs#L494) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchIframePlayer.currentReady | [524:26](../DashboardYouTubePlayerInterceptor.cs#L524) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchIframeTeardown | [635:9](../DashboardYouTubePlayerInterceptor.cs#L635) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchIframeTeardown.source | [635:51](../DashboardYouTubePlayerInterceptor.cs#L635) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchIframeTeardown.patched | [639:17](../DashboardYouTubePlayerInterceptor.cs#L639) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchIframeTeardown.replacements | [640:17](../DashboardYouTubePlayerInterceptor.cs#L640) |
| tuple-element | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchIframeTeardown.replacements.Before | [640:37](../DashboardYouTubePlayerInterceptor.cs#L640) |
| tuple-element | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchIframeTeardown.replacements.After | [640:52](../DashboardYouTubePlayerInterceptor.cs#L640) |
| iteration-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchIframeTeardown.replacement | [663:13](../DashboardYouTubePlayerInterceptor.cs#L663) |
| function | IframeTeardownHelper.ytPluginDisposePlayback20260908 | [672:1](../DashboardYouTubePlayerInterceptor.cs#L672) |
| variable | IframeTeardownHelper.ytPluginDisposePlayback20260908.player | [672:9](../DashboardYouTubePlayerInterceptor.cs#L672) |
| variable | IframeTeardownHelper.ytPluginDisposePlayback20260908.timer | [672:9](../DashboardYouTubePlayerInterceptor.cs#L672) |
| variable | IframeTeardownHelper.ytPluginDisposePlayback20260908.signal | [672:9](../DashboardYouTubePlayerInterceptor.cs#L672) |
| variable | IframeTeardownHelper.ytPluginDisposePlayback20260908.reject | [672:9](../DashboardYouTubePlayerInterceptor.cs#L672) |
| variable | IframeTeardownHelper.ytPluginDisposePlayback20260908.frames | [672:21](../DashboardYouTubePlayerInterceptor.cs#L672) |
| field | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.IframeTeardownHelper | [672:30](../DashboardYouTubePlayerInterceptor.cs#L672) |
| catch-variable | IframeTeardownHelper.ytPluginDisposePlayback20260908.e | [672:32](../DashboardYouTubePlayerInterceptor.cs#L672) |
| catch-variable | IframeTeardownHelper.ytPluginDisposePlayback20260908.e | [672:38](../DashboardYouTubePlayerInterceptor.cs#L672) |
| parameter | IframeTeardownHelper.ytPluginDisposePlayback20260908.instance | [672:42](../DashboardYouTubePlayerInterceptor.cs#L672) |
| variable | IframeTeardownHelper.ytPluginDisposePlayback20260908.resize | [672:43](../DashboardYouTubePlayerInterceptor.cs#L672) |
| variable | IframeTeardownHelper.ytPluginDisposePlayback20260908.dlg | [672:46](../DashboardYouTubePlayerInterceptor.cs#L672) |
| variable | IframeTeardownHelper.ytPluginDisposePlayback20260908.abort | [672:47](../DashboardYouTubePlayerInterceptor.cs#L672) |
| variable | IframeTeardownHelper.ytPluginDisposePlayback20260908.active | [672:47](../DashboardYouTubePlayerInterceptor.cs#L672) |
| parameter | IframeTeardownHelper.ytPluginDisposePlayback20260908.triggerStopped | [672:51](../DashboardYouTubePlayerInterceptor.cs#L672) |
| catch-variable | IframeTeardownHelper.ytPluginDisposePlayback20260908.e | [672:56](../DashboardYouTubePlayerInterceptor.cs#L672) |
| catch-variable | IframeTeardownHelper.ytPluginDisposePlayback20260908.e | [672:60](../DashboardYouTubePlayerInterceptor.cs#L672) |
| catch-variable | IframeTeardownHelper.ytPluginDisposePlayback20260908.e | [672:67](../DashboardYouTubePlayerInterceptor.cs#L672) |
| variable | IframeTeardownHelper.ytPluginDisposePlayback20260908.i | [672:67](../DashboardYouTubePlayerInterceptor.cs#L672) |
| catch-variable | IframeTeardownHelper.ytPluginDisposePlayback20260908.e | [672:72](../DashboardYouTubePlayerInterceptor.cs#L672) |
| catch-variable | IframeTeardownHelper.ytPluginDisposePlayback20260908.e | [672:124](../DashboardYouTubePlayerInterceptor.cs#L672) |
| catch-variable | IframeTeardownHelper.ytPluginDisposePlayback20260908.e | [672:125](../DashboardYouTubePlayerInterceptor.cs#L672) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchWebViewPlayer | [698:9](../DashboardYouTubePlayerInterceptor.cs#L698) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchWebViewPlayer.source | [698:50](../DashboardYouTubePlayerInterceptor.cs#L698) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchWebViewPlayer.patched | [703:17](../DashboardYouTubePlayerInterceptor.cs#L703) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchWebViewPlayer.currentPlay | [789:26](../DashboardYouTubePlayerInterceptor.cs#L789) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.InjectPlayerPatchHelpers | [865:9](../DashboardYouTubePlayerInterceptor.cs#L865) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.InjectPlayerPatchHelpers.source | [865:54](../DashboardYouTubePlayerInterceptor.cs#L865) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.InjectPlayerPatchHelpers.moduleName | [865:73](../DashboardYouTubePlayerInterceptor.cs#L865) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.InjectPlayerPatchHelpers.cssLoader | [870:26](../DashboardYouTubePlayerInterceptor.cs#L870) |
| function | legacyAbortHelper.getSignalRejectReason | [880:1](../DashboardYouTubePlayerInterceptor.cs#L880) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.InjectPlayerPatchHelpers.legacyAbortHelper | [880:26](../DashboardYouTubePlayerInterceptor.cs#L880) |
| parameter | legacyAbortHelper.getSignalRejectReason.signal | [880:32](../DashboardYouTubePlayerInterceptor.cs#L880) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchLocalPlayerFlag | [895:9](../DashboardYouTubePlayerInterceptor.cs#L895) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchLocalPlayerFlag.source | [895:50](../DashboardYouTubePlayerInterceptor.cs#L895) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchLocalPlayerFlag.moduleName | [895:69](../DashboardYouTubePlayerInterceptor.cs#L895) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchLocalPlayerFlag.patched | [900:17](../DashboardYouTubePlayerInterceptor.cs#L900) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchCanPlayItem | [921:9](../DashboardYouTubePlayerInterceptor.cs#L921) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchCanPlayItem.source | [921:46](../DashboardYouTubePlayerInterceptor.cs#L921) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchCanPlayItem.moduleName | [921:65](../DashboardYouTubePlayerInterceptor.cs#L921) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PatchCanPlayItem.patched | [926:17](../DashboardYouTubePlayerInterceptor.cs#L926) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.ApplyIframeHostOptionsWrap | [955:9](../DashboardYouTubePlayerInterceptor.cs#L955) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.ApplyIframeHostOptionsWrap.source | [955:56](../DashboardYouTubePlayerInterceptor.cs#L955) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.ApplyIframeHostOptionsWrap.startOld | [957:26](../DashboardYouTubePlayerInterceptor.cs#L957) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.ApplyIframeHostOptionsWrap.startNew | [958:26](../DashboardYouTubePlayerInterceptor.cs#L958) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.ApplyIframeHostOptionsWrap.legacyEndOld | [959:26](../DashboardYouTubePlayerInterceptor.cs#L959) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.ApplyIframeHostOptionsWrap.legacyEndNew | [960:26](../DashboardYouTubePlayerInterceptor.cs#L960) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.ApplyIframeHostOptionsWrap.currentEndOld | [961:26](../DashboardYouTubePlayerInterceptor.cs#L961) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.ApplyIframeHostOptionsWrap.currentEndNew | [962:26](../DashboardYouTubePlayerInterceptor.cs#L962) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.ApplyIframeHostOptionsWrap.endOld | [978:17](../DashboardYouTubePlayerInterceptor.cs#L978) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.ApplyIframeHostOptionsWrap.endNew | [983:17](../DashboardYouTubePlayerInterceptor.cs#L983) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.ReplaceOptional | [1001:9](../DashboardYouTubePlayerInterceptor.cs#L1001) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.ReplaceOptional.source | [1001:45](../DashboardYouTubePlayerInterceptor.cs#L1001) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.ReplaceOptional.oldValue | [1001:64](../DashboardYouTubePlayerInterceptor.cs#L1001) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.ReplaceOptional.newValue | [1001:81](../DashboardYouTubePlayerInterceptor.cs#L1001) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.ReplaceRequired | [1010:9](../DashboardYouTubePlayerInterceptor.cs#L1010) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.ReplaceRequired.source | [1010:45](../DashboardYouTubePlayerInterceptor.cs#L1010) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.ReplaceRequired.oldValue | [1010:64](../DashboardYouTubePlayerInterceptor.cs#L1010) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.ReplaceRequired.newValue | [1010:81](../DashboardYouTubePlayerInterceptor.cs#L1010) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.ReplaceRequired.label | [1010:98](../DashboardYouTubePlayerInterceptor.cs#L1010) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.IsTargetResource | [1022:9](../DashboardYouTubePlayerInterceptor.cs#L1022) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.IsTargetResource.resourceName | [1022:46](../DashboardYouTubePlayerInterceptor.cs#L1022) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.IsTargetResource.normalized | [1027:17](../DashboardYouTubePlayerInterceptor.cs#L1027) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.IsVideoOsdResource | [1036:9](../DashboardYouTubePlayerInterceptor.cs#L1036) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.IsVideoOsdResource.resourceName | [1036:48](../DashboardYouTubePlayerInterceptor.cs#L1036) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.IsPlaybackManagerResource | [1040:9](../DashboardYouTubePlayerInterceptor.cs#L1040) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.IsPlaybackManagerResource.resourceName | [1040:55](../DashboardYouTubePlayerInterceptor.cs#L1040) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.IsEmbedResource | [1044:9](../DashboardYouTubePlayerInterceptor.cs#L1044) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.IsEmbedResource.resourceName | [1044:45](../DashboardYouTubePlayerInterceptor.cs#L1044) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.IsEmbedResource.normalized | [1046:17](../DashboardYouTubePlayerInterceptor.cs#L1046) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.NormalizeResourceName | [1056:9](../DashboardYouTubePlayerInterceptor.cs#L1056) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.NormalizeResourceName.resourceName | [1056:53](../DashboardYouTubePlayerInterceptor.cs#L1056) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.NormalizeResourceName.normalized | [1058:17](../DashboardYouTubePlayerInterceptor.cs#L1058) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.NormalizeResourceName.queryStart | [1063:17](../DashboardYouTubePlayerInterceptor.cs#L1063) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetContentType | [1067:9](../DashboardYouTubePlayerInterceptor.cs#L1067) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetContentType.resourceName | [1067:46](../DashboardYouTubePlayerInterceptor.cs#L1067) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetContentType.normalized | [1069:17](../DashboardYouTubePlayerInterceptor.cs#L1069) |
| property | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PluginVersion | [1077:9](../DashboardYouTubePlayerInterceptor.cs#L1077) |
| field | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.DashboardPatchRevision | [1080:30](../DashboardYouTubePlayerInterceptor.cs#L1080) |
| property | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PluginCacheQueryPart | [1082:9](../DashboardYouTubePlayerInterceptor.cs#L1082) |
| function | PlayerPatchHelpers.ytPluginPatch20260606 | [1085:1](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| field | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.PlayerPatchHelpers | [1085:30](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| function | PlayerPatchHelpers.ytPluginStartSeconds20260606 | [1085:43](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| parameter | PlayerPatchHelpers.ytPluginStartSeconds20260606.params | [1085:81](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| variable | PlayerPatchHelpers.ytPluginStartSeconds20260606.raw | [1085:93](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| variable | PlayerPatchHelpers.ytPluginStartSeconds20260606.match | [1085:200](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| function | PlayerPatchHelpers.ytPluginCanPlayItem20260606 | [1085:359](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| parameter | PlayerPatchHelpers.ytPluginCanPlayItem20260606.item | [1085:396](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| variable | PlayerPatchHelpers.ytPluginCanPlayItem20260606.yt | [1085:410](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| variable | PlayerPatchHelpers.ytPluginCanPlayItem20260606.sources | [1085:469](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| variable | PlayerPatchHelpers.ytPluginCanPlayItem20260606.i | [1085:534](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| variable | PlayerPatchHelpers.ytPluginCanPlayItem20260606.source | [1085:564](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| variable | PlayerPatchHelpers.ytPluginCanPlayItem20260606.path | [1085:586](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| variable | PlayerPatchHelpers.ytPluginCanPlayItem20260606.path | [1085:700](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| catch-variable | PlayerPatchHelpers.ytPluginCanPlayItem20260606.e | [1085:770](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| function | PlayerPatchHelpers.ytPluginPlayerVars20260822 | [1085:783](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| parameter | PlayerPatchHelpers.ytPluginPlayerVars20260822.playerVars | [1085:819](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| parameter | PlayerPatchHelpers.ytPluginPlayerVars20260822.startSeconds | [1085:830](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| variable | PlayerPatchHelpers.ytPluginPlayerVars20260822.result | [1085:848](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| function | PlayerPatchHelpers.ytPluginForceCaptionsOff20260822 | [1085:1106](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| parameter | PlayerPatchHelpers.ytPluginForceCaptionsOff20260822.player | [1085:1148](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| variable | PlayerPatchHelpers.ytPluginForceCaptionsOff20260822.applied | [1085:1221](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| catch-variable | PlayerPatchHelpers.ytPluginForceCaptionsOff20260822.e | [1085:1380](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| catch-variable | PlayerPatchHelpers.ytPluginForceCaptionsOff20260822.e | [1085:1480](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| function | PlayerPatchHelpers.ytPluginStopCaptionGuard20260822 | [1085:1551](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| parameter | PlayerPatchHelpers.ytPluginStopCaptionGuard20260822.instance | [1085:1593](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| variable | PlayerPatchHelpers.ytPluginStopCaptionGuard20260822.timers | [1085:1631](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| variable | PlayerPatchHelpers.ytPluginStopCaptionGuard20260822.i | [1085:1678](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| catch-variable | PlayerPatchHelpers.ytPluginStopCaptionGuard20260822.e | [1085:1906](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| function | PlayerPatchHelpers.ytPluginStartCaptionGuard20260822 | [1085:1911](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| parameter | PlayerPatchHelpers.ytPluginStartCaptionGuard20260822.instance | [1085:1954](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| parameter | PlayerPatchHelpers.ytPluginStartCaptionGuard20260822.player | [1085:1963](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| variable | PlayerPatchHelpers.ytPluginStartCaptionGuard20260822.keep | [1085:2109](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| function | PlayerPatchHelpers.ytPluginStartCaptionGuard20260822.keep | [1085:2114](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| function | PlayerPatchHelpers.ytPluginStartCaptionGuard20260822.[100,500,1500,4000].map callback@2325 | [1085:2326](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| parameter | PlayerPatchHelpers.ytPluginStartCaptionGuard20260822.[100,500,1500,4000].map callback@2325.delay | [1085:2335](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| catch-variable | PlayerPatchHelpers.ytPluginStartCaptionGuard20260822.e | [1085:2433](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| function | PlayerPatchHelpers.ytPluginDiag20260606 | [1085:2438](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| parameter | PlayerPatchHelpers.ytPluginDiag20260606.m | [1085:2468](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| catch-variable | PlayerPatchHelpers.ytPluginDiag20260606.e | [1085:2573](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| function | PlayerPatchHelpers.ytPluginHostOptions20260617 | [1085:2578](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| variable | PlayerPatchHelpers.ytPluginHostOptions20260617.ua | [1085:2625](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| catch-variable | PlayerPatchHelpers.ytPluginHostOptions20260617.e | [1085:2860](../DashboardYouTubePlayerInterceptor.cs#L1085) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetProperty | [1098:9](../DashboardYouTubePlayerInterceptor.cs#L1098) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetProperty.source | [1098:44](../DashboardYouTubePlayerInterceptor.cs#L1098) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetProperty.name | [1098:59](../DashboardYouTubePlayerInterceptor.cs#L1098) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetStringProperty | [1105:9](../DashboardYouTubePlayerInterceptor.cs#L1105) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetStringProperty.source | [1105:50](../DashboardYouTubePlayerInterceptor.cs#L1105) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetStringProperty.name | [1105:65](../DashboardYouTubePlayerInterceptor.cs#L1105) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetField | [1110:9](../DashboardYouTubePlayerInterceptor.cs#L1110) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetField.source | [1110:41](../DashboardYouTubePlayerInterceptor.cs#L1110) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.GetField.name | [1110:56](../DashboardYouTubePlayerInterceptor.cs#L1110) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.FindLoadedType | [1117:9](../DashboardYouTubePlayerInterceptor.cs#L1117) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.FindLoadedType.fullName | [1117:45](../DashboardYouTubePlayerInterceptor.cs#L1117) |
| iteration-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.FindLoadedType.assembly | [1119:13](../DashboardYouTubePlayerInterceptor.cs#L1119) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.FindLoadedType.assembly.type | [1123:25](../DashboardYouTubePlayerInterceptor.cs#L1123) |
| method | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.InvokeHarmonyPatch | [1136:9](../DashboardYouTubePlayerInterceptor.cs#L1136) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.InvokeHarmonyPatch.harmony | [1136:48](../DashboardYouTubePlayerInterceptor.cs#L1136) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.InvokeHarmonyPatch.original | [1136:64](../DashboardYouTubePlayerInterceptor.cs#L1136) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.InvokeHarmonyPatch.prefix | [1136:85](../DashboardYouTubePlayerInterceptor.cs#L1136) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.InvokeHarmonyPatch.patchMethod | [1138:17](../DashboardYouTubePlayerInterceptor.cs#L1138) |
| anonymous-function | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.InvokeHarmonyPatch.patchMethod.anonymous@66107 | [1140:33](../DashboardYouTubePlayerInterceptor.cs#L1140) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.InvokeHarmonyPatch.patchMethod.anonymous@66107.m | [1140:33](../DashboardYouTubePlayerInterceptor.cs#L1140) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.InvokeHarmonyPatch.patchMethod.anonymous@66107.parameters | [1145:25](../DashboardYouTubePlayerInterceptor.cs#L1145) |
| anonymous-function | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.InvokeHarmonyPatch.patchMethod.anonymous@66107.anonymous@66458 | [1148:46](../DashboardYouTubePlayerInterceptor.cs#L1148) |
| parameter | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.InvokeHarmonyPatch.patchMethod.anonymous@66107.anonymous@66458.p | [1148:46](../DashboardYouTubePlayerInterceptor.cs#L1148) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.InvokeHarmonyPatch.patchParameters | [1154:17](../DashboardYouTubePlayerInterceptor.cs#L1154) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.InvokeHarmonyPatch.args | [1155:17](../DashboardYouTubePlayerInterceptor.cs#L1155) |
| local-variable | Emby.YouTubePlugin.DashboardYouTubePlayerInterceptor.InvokeHarmonyPatch.i | [1157:22](../DashboardYouTubePlayerInterceptor.cs#L1157) |

</details>

<a id="file-07323f6dc3f3a1d5"></a>
## [EmbeddedDependencyLoader.cs](../EmbeddedDependencyLoader.cs)

Plugin Compile source (SDK default); 3407 bytes; 88 lines.

<details>
<summary>All 23 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [8:1](../EmbeddedDependencyLoader.cs#L8) |
| type | Emby.YouTubePlugin.EmbeddedDependencyLoader | [13:5](../EmbeddedDependencyLoader.cs#L13) |
| field | Emby.YouTubePlugin.EmbeddedDependencyLoader.HarmonyAssemblyName | [15:30](../EmbeddedDependencyLoader.cs#L15) |
| field | Emby.YouTubePlugin.EmbeddedDependencyLoader.HarmonyResourceName | [16:30](../EmbeddedDependencyLoader.cs#L16) |
| field | Emby.YouTubePlugin.EmbeddedDependencyLoader._registered | [18:28](../EmbeddedDependencyLoader.cs#L18) |
| field | Emby.YouTubePlugin.EmbeddedDependencyLoader._harmonyAssembly | [19:34](../EmbeddedDependencyLoader.cs#L19) |
| method | Emby.YouTubePlugin.EmbeddedDependencyLoader.Register | [21:9](../EmbeddedDependencyLoader.cs#L21) |
| method | Emby.YouTubePlugin.EmbeddedDependencyLoader.LoadHarmonyAssembly | [32:9](../EmbeddedDependencyLoader.cs#L32) |
| method | Emby.YouTubePlugin.EmbeddedDependencyLoader.ResolveFromAppDomain | [41:9](../EmbeddedDependencyLoader.cs#L41) |
| parameter | Emby.YouTubePlugin.EmbeddedDependencyLoader.ResolveFromAppDomain.sender | [41:55](../EmbeddedDependencyLoader.cs#L41) |
| parameter | Emby.YouTubePlugin.EmbeddedDependencyLoader.ResolveFromAppDomain.args | [41:71](../EmbeddedDependencyLoader.cs#L41) |
| method | Emby.YouTubePlugin.EmbeddedDependencyLoader.ResolveFromLoadContext | [46:9](../EmbeddedDependencyLoader.cs#L46) |
| parameter | Emby.YouTubePlugin.EmbeddedDependencyLoader.ResolveFromLoadContext.context | [46:57](../EmbeddedDependencyLoader.cs#L46) |
| parameter | Emby.YouTubePlugin.EmbeddedDependencyLoader.ResolveFromLoadContext.assemblyName | [46:86](../EmbeddedDependencyLoader.cs#L46) |
| method | Emby.YouTubePlugin.EmbeddedDependencyLoader.Resolve | [51:9](../EmbeddedDependencyLoader.cs#L51) |
| parameter | Emby.YouTubePlugin.EmbeddedDependencyLoader.Resolve.assemblyName | [51:42](../EmbeddedDependencyLoader.cs#L51) |
| local-variable | Emby.YouTubePlugin.EmbeddedDependencyLoader.Resolve.requested | [56:17](../EmbeddedDependencyLoader.cs#L56) |
| local-variable | Emby.YouTubePlugin.EmbeddedDependencyLoader.Resolve.alreadyLoaded | [62:17](../EmbeddedDependencyLoader.cs#L62) |
| anonymous-function | Emby.YouTubePlugin.EmbeddedDependencyLoader.Resolve.alreadyLoaded.anonymous@2347 | [63:33](../EmbeddedDependencyLoader.cs#L63) |
| parameter | Emby.YouTubePlugin.EmbeddedDependencyLoader.Resolve.alreadyLoaded.anonymous@2347.a | [63:33](../EmbeddedDependencyLoader.cs#L63) |
| local-variable | Emby.YouTubePlugin.EmbeddedDependencyLoader.Resolve.executingAssembly | [70:17](../EmbeddedDependencyLoader.cs#L70) |
| local-variable | Emby.YouTubePlugin.EmbeddedDependencyLoader.Resolve.resource | [71:23](../EmbeddedDependencyLoader.cs#L71) |
| local-variable | Emby.YouTubePlugin.EmbeddedDependencyLoader.Resolve.buffer | [78:23](../EmbeddedDependencyLoader.cs#L78) |

</details>

<a id="file-8194c8953ddc1453"></a>
## [Emby-Youtube-Plugin.sln](../Emby-Youtube-Plugin.sln)

Build definition; 1106 bytes; 24 lines.

<a id="file-8c9377fd54ef39d4"></a>
## [Emby.YouTubePlugin.csproj](../Emby.YouTubePlugin.csproj)

Build definition; 2340 bytes; 45 lines.

<a id="file-072a7f211747c67e"></a>
## [FolderIcons.cs](../FolderIcons.cs)

Plugin Compile source (SDK default); 2986 bytes; 62 lines.

<details>
<summary>All 15 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [3:1](../FolderIcons.cs#L3) |
| type | Emby.YouTubePlugin.FolderIcons | [9:5](../FolderIcons.cs#L9) |
| field | Emby.YouTubePlugin.FolderIcons.Cdn | [11:30](../FolderIcons.cs#L11) |
| field | Emby.YouTubePlugin.FolderIcons.WatchLater | [13:39](../FolderIcons.cs#L13) |
| field | Emby.YouTubePlugin.FolderIcons.Trending | [14:39](../FolderIcons.cs#L14) |
| field | Emby.YouTubePlugin.FolderIcons.Categories | [15:39](../FolderIcons.cs#L15) |
| field | Emby.YouTubePlugin.FolderIcons.RecentlyAdded | [16:39](../FolderIcons.cs#L16) |
| field | Emby.YouTubePlugin.FolderIcons.Videos | [17:39](../FolderIcons.cs#L17) |
| field | Emby.YouTubePlugin.FolderIcons.Shorts | [18:39](../FolderIcons.cs#L18) |
| field | Emby.YouTubePlugin.FolderIcons.Live | [19:39](../FolderIcons.cs#L19) |
| field | Emby.YouTubePlugin.FolderIcons.Search | [20:39](../FolderIcons.cs#L20) |
| field | Emby.YouTubePlugin.FolderIcons.CategoryMap | [23:60](../FolderIcons.cs#L23) |
| method | Emby.YouTubePlugin.FolderIcons.ForCategory | [59:9](../FolderIcons.cs#L59) |
| parameter | Emby.YouTubePlugin.FolderIcons.ForCategory.categoryId | [59:42](../FolderIcons.cs#L59) |
| pattern-variable | Emby.YouTubePlugin.FolderIcons.ForCategory.url | [60:57](../FolderIcons.cs#L60) |

</details>

<a id="file-24d40baec16f6f30"></a>
## [GuideImageService.cs](../GuideImageService.cs)

Plugin Compile source (SDK default); 1669 bytes; 52 lines.

<details>
<summary>All 18 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [5:1](../GuideImageService.cs#L5) |
| type | Emby.YouTubePlugin.GetYouTubeGuideImage | [7:5](../GuideImageService.cs#L7) |
| property | Emby.YouTubePlugin.GetYouTubeGuideImage.Name | [10:9](../GuideImageService.cs#L10) |
| accessor | Emby.YouTubePlugin.GetYouTubeGuideImage.Name.get | [10:31](../GuideImageService.cs#L10) |
| accessor | Emby.YouTubePlugin.GetYouTubeGuideImage.Name.set | [10:36](../GuideImageService.cs#L10) |
| type | Emby.YouTubePlugin.GuideImageService | [13:5](../GuideImageService.cs#L13) |
| property | Emby.YouTubePlugin.GuideImageService.Request | [15:9](../GuideImageService.cs#L15) |
| accessor | Emby.YouTubePlugin.GuideImageService.Request.get | [15:35](../GuideImageService.cs#L15) |
| accessor | Emby.YouTubePlugin.GuideImageService.Request.set | [15:40](../GuideImageService.cs#L15) |
| method | Emby.YouTubePlugin.GuideImageService.Get | [17:9](../GuideImageService.cs#L17) |
| parameter | Emby.YouTubePlugin.GuideImageService.Get.request | [17:27](../GuideImageService.cs#L17) |
| local-variable | Emby.YouTubePlugin.GuideImageService.Get.name | [19:17](../GuideImageService.cs#L19) |
| local-variable | Emby.YouTubePlugin.GuideImageService.Get.resourceName | [27:17](../GuideImageService.cs#L27) |
| local-variable | Emby.YouTubePlugin.GuideImageService.Get.stream | [28:17](../GuideImageService.cs#L28) |
| method | Emby.YouTubePlugin.GuideImageService.NotFound | [39:9](../GuideImageService.cs#L39) |
| method | Emby.YouTubePlugin.GuideImageService.SetResponse | [45:9](../GuideImageService.cs#L45) |
| parameter | Emby.YouTubePlugin.GuideImageService.SetResponse.contentType | [45:34](../GuideImageService.cs#L45) |
| parameter | Emby.YouTubePlugin.GuideImageService.SetResponse.statusCode | [45:54](../GuideImageService.cs#L45) |

</details>

<a id="file-c693279643b8cd5d"></a>
## [LICENSE](../LICENSE)

Repository support; 1021 bytes.

<a id="file-813f15c5a6f705bb"></a>
## [ManagedResolver/Compatibility.cs](../ManagedResolver/Compatibility.cs)

Plugin Compile source (SDK default); 3327 bytes; 56 lines.

<details>
<summary>All 64 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | PowerKit.Extensions | [9:1](../ManagedResolver/Compatibility.cs#L9) |
| type | PowerKit.Extensions.ManagedCompatibility | [11:5](../ManagedResolver/Compatibility.cs#L11) |
| method | PowerKit.Extensions.ManagedCompatibility.Pipe | [13:9](../ManagedResolver/Compatibility.cs#L13) |
| type-parameter | PowerKit.Extensions.ManagedCompatibility.Pipe.T | [13:38](../ManagedResolver/Compatibility.cs#L13) |
| type-parameter | PowerKit.Extensions.ManagedCompatibility.Pipe.TResult | [13:41](../ManagedResolver/Compatibility.cs#L13) |
| parameter | PowerKit.Extensions.ManagedCompatibility.Pipe.value | [13:50](../ManagedResolver/Compatibility.cs#L13) |
| parameter | PowerKit.Extensions.ManagedCompatibility.Pipe.map | [13:64](../ManagedResolver/Compatibility.cs#L13) |
| method | PowerKit.Extensions.ManagedCompatibility.NullIfWhiteSpace | [14:9](../ManagedResolver/Compatibility.cs#L14) |
| parameter | PowerKit.Extensions.ManagedCompatibility.NullIfWhiteSpace.value | [14:50](../ManagedResolver/Compatibility.cs#L14) |
| method | PowerKit.Extensions.ManagedCompatibility.SubstringAfter | [15:9](../ManagedResolver/Compatibility.cs#L15) |
| parameter | PowerKit.Extensions.ManagedCompatibility.SubstringAfter.value | [15:47](../ManagedResolver/Compatibility.cs#L15) |
| parameter | PowerKit.Extensions.ManagedCompatibility.SubstringAfter.separator | [15:66](../ManagedResolver/Compatibility.cs#L15) |
| local-variable | PowerKit.Extensions.ManagedCompatibility.SubstringAfter.index | [16:15](../ManagedResolver/Compatibility.cs#L16) |
| method | PowerKit.Extensions.ManagedCompatibility.SubstringUntil | [17:9](../ManagedResolver/Compatibility.cs#L17) |
| parameter | PowerKit.Extensions.ManagedCompatibility.SubstringUntil.value | [17:47](../ManagedResolver/Compatibility.cs#L17) |
| parameter | PowerKit.Extensions.ManagedCompatibility.SubstringUntil.separator | [17:66](../ManagedResolver/Compatibility.cs#L17) |
| local-variable | PowerKit.Extensions.ManagedCompatibility.SubstringUntil.index | [18:15](../ManagedResolver/Compatibility.cs#L18) |
| method | PowerKit.Extensions.ManagedCompatibility.WhereNotNull | [19:9](../ManagedResolver/Compatibility.cs#L19) |
| type-parameter | PowerKit.Extensions.ManagedCompatibility.WhereNotNull.T | [19:53](../ManagedResolver/Compatibility.cs#L19) |
| parameter | PowerKit.Extensions.ManagedCompatibility.WhereNotNull.source | [19:56](../ManagedResolver/Compatibility.cs#L19) |
| anonymous-function | PowerKit.Extensions.ManagedCompatibility.WhereNotNull.anonymous@1125 | [19:117](../ManagedResolver/Compatibility.cs#L19) |
| parameter | PowerKit.Extensions.ManagedCompatibility.WhereNotNull.anonymous@1125.value | [19:117](../ManagedResolver/Compatibility.cs#L19) |
| method | PowerKit.Extensions.ManagedCompatibility.Index | [20:9](../ManagedResolver/Compatibility.cs#L20) |
| tuple-element | PowerKit.Extensions.ManagedCompatibility.Index.Index | [20:38](../ManagedResolver/Compatibility.cs#L20) |
| tuple-element | PowerKit.Extensions.ManagedCompatibility.Index.Item | [20:49](../ManagedResolver/Compatibility.cs#L20) |
| type-parameter | PowerKit.Extensions.ManagedCompatibility.Index.T | [20:64](../ManagedResolver/Compatibility.cs#L20) |
| parameter | PowerKit.Extensions.ManagedCompatibility.Index.source | [20:67](../ManagedResolver/Compatibility.cs#L20) |
| anonymous-function | PowerKit.Extensions.ManagedCompatibility.Index.anonymous@1262 | [20:112](../ManagedResolver/Compatibility.cs#L20) |
| parameter | PowerKit.Extensions.ManagedCompatibility.Index.anonymous@1262.value | [20:113](../ManagedResolver/Compatibility.cs#L20) |
| parameter | PowerKit.Extensions.ManagedCompatibility.Index.anonymous@1262.index | [20:120](../ManagedResolver/Compatibility.cs#L20) |
| type | PowerKit.Extensions.ManagedNumeric | [23:5](../ManagedResolver/Compatibility.cs#L23) |
| method | PowerKit.Extensions.ManagedNumeric.ParseLongOrNull | [25:9](../ManagedResolver/Compatibility.cs#L25) |
| parameter | PowerKit.Extensions.ManagedNumeric.ParseLongOrNull.value | [25:47](../ManagedResolver/Compatibility.cs#L25) |
| parameter | PowerKit.Extensions.ManagedNumeric.ParseLongOrNull.provider | [25:61](../ManagedResolver/Compatibility.cs#L25) |
| pattern-variable | PowerKit.Extensions.ManagedNumeric.ParseLongOrNull.parsed | [26:75](../ManagedResolver/Compatibility.cs#L26) |
| method | PowerKit.Extensions.ManagedNumeric.ParseDoubleOrNull | [27:9](../ManagedResolver/Compatibility.cs#L27) |
| parameter | PowerKit.Extensions.ManagedNumeric.ParseDoubleOrNull.value | [27:51](../ManagedResolver/Compatibility.cs#L27) |
| parameter | PowerKit.Extensions.ManagedNumeric.ParseDoubleOrNull.provider | [27:65](../ManagedResolver/Compatibility.cs#L27) |
| pattern-variable | PowerKit.Extensions.ManagedNumeric.ParseDoubleOrNull.parsed | [28:77](../ManagedResolver/Compatibility.cs#L28) |
| namespace | JsonExtensions | [32:1](../ManagedResolver/Compatibility.cs#L32) |
| type | JsonExtensions.Json | [34:5](../ManagedResolver/Compatibility.cs#L34) |
| method | JsonExtensions.Json.Parse | [36:9](../ManagedResolver/Compatibility.cs#L36) |
| parameter | JsonExtensions.Json.Parse.source | [36:43](../ManagedResolver/Compatibility.cs#L36) |
| local-variable | JsonExtensions.Json.Parse.document | [37:21](../ManagedResolver/Compatibility.cs#L37) |
| method | JsonExtensions.Json.TryParse | [38:9](../ManagedResolver/Compatibility.cs#L38) |
| parameter | JsonExtensions.Json.TryParse.source | [38:47](../ManagedResolver/Compatibility.cs#L38) |
| namespace | JsonExtensions.Reading | [43:1](../ManagedResolver/Compatibility.cs#L43) |
| type | JsonExtensions.Reading.ManagedJsonReading | [45:5](../ManagedResolver/Compatibility.cs#L45) |
| method | JsonExtensions.Reading.ManagedJsonReading.GetPropertyOrNull | [47:9](../ManagedResolver/Compatibility.cs#L47) |
| parameter | JsonExtensions.Reading.ManagedJsonReading.GetPropertyOrNull.value | [47:56](../ManagedResolver/Compatibility.cs#L47) |
| parameter | JsonExtensions.Reading.ManagedJsonReading.GetPropertyOrNull.name | [47:80](../ManagedResolver/Compatibility.cs#L47) |
| pattern-variable | JsonExtensions.Reading.ManagedJsonReading.GetPropertyOrNull.found | [48:91](../ManagedResolver/Compatibility.cs#L48) |
| method | JsonExtensions.Reading.ManagedJsonReading.GetStringOrNull | [49:9](../ManagedResolver/Compatibility.cs#L49) |
| parameter | JsonExtensions.Reading.ManagedJsonReading.GetStringOrNull.value | [49:49](../ManagedResolver/Compatibility.cs#L49) |
| method | JsonExtensions.Reading.ManagedJsonReading.GetInt32OrNull | [50:9](../ManagedResolver/Compatibility.cs#L50) |
| parameter | JsonExtensions.Reading.ManagedJsonReading.GetInt32OrNull.value | [50:45](../ManagedResolver/Compatibility.cs#L50) |
| pattern-variable | JsonExtensions.Reading.ManagedJsonReading.GetInt32OrNull.parsed | [50:141](../ManagedResolver/Compatibility.cs#L50) |
| method | JsonExtensions.Reading.ManagedJsonReading.GetInt64OrNull | [51:9](../ManagedResolver/Compatibility.cs#L51) |
| parameter | JsonExtensions.Reading.ManagedJsonReading.GetInt64OrNull.value | [51:46](../ManagedResolver/Compatibility.cs#L51) |
| pattern-variable | JsonExtensions.Reading.ManagedJsonReading.GetInt64OrNull.parsed | [51:143](../ManagedResolver/Compatibility.cs#L51) |
| method | JsonExtensions.Reading.ManagedJsonReading.GetBooleanOrNull | [52:9](../ManagedResolver/Compatibility.cs#L52) |
| parameter | JsonExtensions.Reading.ManagedJsonReading.GetBooleanOrNull.value | [52:48](../ManagedResolver/Compatibility.cs#L52) |
| method | JsonExtensions.Reading.ManagedJsonReading.EnumerateArrayOrNull | [53:9](../ManagedResolver/Compatibility.cs#L53) |
| parameter | JsonExtensions.Reading.ManagedJsonReading.EnumerateArrayOrNull.value | [53:72](../ManagedResolver/Compatibility.cs#L53) |

</details>

<a id="file-69aa28ead9070c54"></a>
## [ManagedResolver/Integration.md](../ManagedResolver/Integration.md)

Documentation/resource; 2932 bytes; 45 lines.

<a id="file-8c416880d43abc64"></a>
## [ManagedResolver/License.txt](../ManagedResolver/License.txt)

EmbeddedResource; 1074 bytes.

<a id="file-273e736a61de1668"></a>
## [ManagedResolver/Upstream.json](../ManagedResolver/Upstream.json)

Repository support; 5405 bytes; 118 lines.

<a id="file-88ffe9d079ed07b0"></a>
## [ManagedResolver/vendored/Bridge/IStreamData.cs](../ManagedResolver/vendored/Bridge/IStreamData.cs)

Plugin Compile source (SDK default); 644 bytes; 38 lines.

<details>
<summary>All 36 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | YoutubeExplode.Bridge | [1:1](../ManagedResolver/vendored/Bridge/IStreamData.cs#L1) |
| type | YoutubeExplode.Bridge.IStreamData | [3:1](../ManagedResolver/vendored/Bridge/IStreamData.cs#L3) |
| property | YoutubeExplode.Bridge.IStreamData.Itag | [5:5](../ManagedResolver/vendored/Bridge/IStreamData.cs#L5) |
| accessor | YoutubeExplode.Bridge.IStreamData.Itag.get | [5:17](../ManagedResolver/vendored/Bridge/IStreamData.cs#L5) |
| property | YoutubeExplode.Bridge.IStreamData.Url | [7:5](../ManagedResolver/vendored/Bridge/IStreamData.cs#L7) |
| accessor | YoutubeExplode.Bridge.IStreamData.Url.get | [7:19](../ManagedResolver/vendored/Bridge/IStreamData.cs#L7) |
| property | YoutubeExplode.Bridge.IStreamData.Signature | [9:5](../ManagedResolver/vendored/Bridge/IStreamData.cs#L9) |
| accessor | YoutubeExplode.Bridge.IStreamData.Signature.get | [9:25](../ManagedResolver/vendored/Bridge/IStreamData.cs#L9) |
| property | YoutubeExplode.Bridge.IStreamData.SignatureParameter | [11:5](../ManagedResolver/vendored/Bridge/IStreamData.cs#L11) |
| accessor | YoutubeExplode.Bridge.IStreamData.SignatureParameter.get | [11:34](../ManagedResolver/vendored/Bridge/IStreamData.cs#L11) |
| property | YoutubeExplode.Bridge.IStreamData.ContentLength | [13:5](../ManagedResolver/vendored/Bridge/IStreamData.cs#L13) |
| accessor | YoutubeExplode.Bridge.IStreamData.ContentLength.get | [13:27](../ManagedResolver/vendored/Bridge/IStreamData.cs#L13) |
| property | YoutubeExplode.Bridge.IStreamData.Bitrate | [15:5](../ManagedResolver/vendored/Bridge/IStreamData.cs#L15) |
| accessor | YoutubeExplode.Bridge.IStreamData.Bitrate.get | [15:21](../ManagedResolver/vendored/Bridge/IStreamData.cs#L15) |
| property | YoutubeExplode.Bridge.IStreamData.Container | [17:5](../ManagedResolver/vendored/Bridge/IStreamData.cs#L17) |
| accessor | YoutubeExplode.Bridge.IStreamData.Container.get | [17:25](../ManagedResolver/vendored/Bridge/IStreamData.cs#L17) |
| property | YoutubeExplode.Bridge.IStreamData.AudioCodec | [19:5](../ManagedResolver/vendored/Bridge/IStreamData.cs#L19) |
| accessor | YoutubeExplode.Bridge.IStreamData.AudioCodec.get | [19:26](../ManagedResolver/vendored/Bridge/IStreamData.cs#L19) |
| property | YoutubeExplode.Bridge.IStreamData.AudioLanguageCode | [21:5](../ManagedResolver/vendored/Bridge/IStreamData.cs#L21) |
| accessor | YoutubeExplode.Bridge.IStreamData.AudioLanguageCode.get | [21:33](../ManagedResolver/vendored/Bridge/IStreamData.cs#L21) |
| property | YoutubeExplode.Bridge.IStreamData.AudioLanguageName | [23:5](../ManagedResolver/vendored/Bridge/IStreamData.cs#L23) |
| accessor | YoutubeExplode.Bridge.IStreamData.AudioLanguageName.get | [23:33](../ManagedResolver/vendored/Bridge/IStreamData.cs#L23) |
| property | YoutubeExplode.Bridge.IStreamData.IsAudioLanguageDefault | [25:5](../ManagedResolver/vendored/Bridge/IStreamData.cs#L25) |
| accessor | YoutubeExplode.Bridge.IStreamData.IsAudioLanguageDefault.get | [25:36](../ManagedResolver/vendored/Bridge/IStreamData.cs#L25) |
| property | YoutubeExplode.Bridge.IStreamData.VideoCodec | [27:5](../ManagedResolver/vendored/Bridge/IStreamData.cs#L27) |
| accessor | YoutubeExplode.Bridge.IStreamData.VideoCodec.get | [27:26](../ManagedResolver/vendored/Bridge/IStreamData.cs#L27) |
| property | YoutubeExplode.Bridge.IStreamData.VideoQualityLabel | [29:5](../ManagedResolver/vendored/Bridge/IStreamData.cs#L29) |
| accessor | YoutubeExplode.Bridge.IStreamData.VideoQualityLabel.get | [29:33](../ManagedResolver/vendored/Bridge/IStreamData.cs#L29) |
| property | YoutubeExplode.Bridge.IStreamData.VideoWidth | [31:5](../ManagedResolver/vendored/Bridge/IStreamData.cs#L31) |
| accessor | YoutubeExplode.Bridge.IStreamData.VideoWidth.get | [31:23](../ManagedResolver/vendored/Bridge/IStreamData.cs#L31) |
| property | YoutubeExplode.Bridge.IStreamData.VideoHeight | [33:5](../ManagedResolver/vendored/Bridge/IStreamData.cs#L33) |
| accessor | YoutubeExplode.Bridge.IStreamData.VideoHeight.get | [33:24](../ManagedResolver/vendored/Bridge/IStreamData.cs#L33) |
| property | YoutubeExplode.Bridge.IStreamData.IsVideoUpscaled | [35:5](../ManagedResolver/vendored/Bridge/IStreamData.cs#L35) |
| accessor | YoutubeExplode.Bridge.IStreamData.IsVideoUpscaled.get | [35:28](../ManagedResolver/vendored/Bridge/IStreamData.cs#L35) |
| property | YoutubeExplode.Bridge.IStreamData.VideoFramerate | [37:5](../ManagedResolver/vendored/Bridge/IStreamData.cs#L37) |
| accessor | YoutubeExplode.Bridge.IStreamData.VideoFramerate.get | [37:27](../ManagedResolver/vendored/Bridge/IStreamData.cs#L37) |

</details>

<a id="file-55b1644a548a74e8"></a>
## [ManagedResolver/vendored/Bridge/PlayerResponse.cs](../ManagedResolver/vendored/Bridge/PlayerResponse.cs)

Plugin Compile source (SDK default); 10377 bytes; 286 lines.

<details>
<summary>All 89 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | YoutubeExplode.Bridge | [12:1](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L12) |
| type | YoutubeExplode.Bridge.PlayerResponse | [14:1](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L14) |
| parameter | YoutubeExplode.Bridge.PlayerResponse.content | [14:39](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L14) |
| property | YoutubeExplode.Bridge.PlayerResponse.Snapshot | [16:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L16) |
| property | YoutubeExplode.Bridge.PlayerResponse.Playability | [19:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L19) |
| property | YoutubeExplode.Bridge.PlayerResponse.PlayabilityStatus | [21:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L21) |
| property | YoutubeExplode.Bridge.PlayerResponse.PlayabilityError | [24:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L24) |
| property | YoutubeExplode.Bridge.PlayerResponse.IsAvailable | [26:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L26) |
| property | YoutubeExplode.Bridge.PlayerResponse.IsPlayable | [30:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L30) |
| property | YoutubeExplode.Bridge.PlayerResponse.Details | [33:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L33) |
| property | YoutubeExplode.Bridge.PlayerResponse.Title | [35:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L35) |
| property | YoutubeExplode.Bridge.PlayerResponse.ChannelId | [37:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L37) |
| property | YoutubeExplode.Bridge.PlayerResponse.Author | [39:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L39) |
| property | YoutubeExplode.Bridge.PlayerResponse.UploadDate | [41:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L41) |
| property | YoutubeExplode.Bridge.PlayerResponse.Duration | [48:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L48) |
| anonymous-function | YoutubeExplode.Bridge.PlayerResponse.Duration.anonymous@1712 | [52:20](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L52) |
| parameter | YoutubeExplode.Bridge.PlayerResponse.Duration.anonymous@1712.s | [52:20](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L52) |
| property | YoutubeExplode.Bridge.PlayerResponse.Thumbnails | [55:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L55) |
| anonymous-function | YoutubeExplode.Bridge.PlayerResponse.Thumbnails.anonymous@2046 | [60:22](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L60) |
| parameter | YoutubeExplode.Bridge.PlayerResponse.Thumbnails.anonymous@2046.j | [60:22](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L60) |
| property | YoutubeExplode.Bridge.PlayerResponse.Keywords | [64:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L64) |
| anonymous-function | YoutubeExplode.Bridge.PlayerResponse.Keywords.anonymous@2275 | [68:22](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L68) |
| parameter | YoutubeExplode.Bridge.PlayerResponse.Keywords.anonymous@2275.j | [68:22](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L68) |
| property | YoutubeExplode.Bridge.PlayerResponse.Description | [73:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L73) |
| property | YoutubeExplode.Bridge.PlayerResponse.ViewCount | [75:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L75) |
| anonymous-function | YoutubeExplode.Bridge.PlayerResponse.ViewCount.anonymous@2612 | [79:20](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L79) |
| parameter | YoutubeExplode.Bridge.PlayerResponse.ViewCount.anonymous@2612.s | [79:20](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L79) |
| property | YoutubeExplode.Bridge.PlayerResponse.PreviewVideoId | [81:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L81) |
| anonymous-function | YoutubeExplode.Bridge.PlayerResponse.PreviewVideoId.anonymous@3959 | [108:19](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L108) |
| parameter | YoutubeExplode.Bridge.PlayerResponse.PreviewVideoId.anonymous@3959.s | [108:19](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L108) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamingData | [111:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L111) |
| property | YoutubeExplode.Bridge.PlayerResponse.DashManifestUrl | [113:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L113) |
| property | YoutubeExplode.Bridge.PlayerResponse.HlsManifestUrl | [116:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L116) |
| property | YoutubeExplode.Bridge.PlayerResponse.Streams | [119:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L119) |
| accessor | YoutubeExplode.Bridge.PlayerResponse.Streams.get | [121:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L121) |
| local-variable | YoutubeExplode.Bridge.PlayerResponse.Streams.get.result | [123:17](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L123) |
| local-variable | YoutubeExplode.Bridge.PlayerResponse.Streams.get.muxedStreams | [125:17](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L125) |
| anonymous-function | YoutubeExplode.Bridge.PlayerResponse.Streams.get.muxedStreams.anonymous@4657 | [128:26](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L128) |
| parameter | YoutubeExplode.Bridge.PlayerResponse.Streams.get.muxedStreams.anonymous@4657.j | [128:26](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L128) |
| local-variable | YoutubeExplode.Bridge.PlayerResponse.Streams.get.adaptiveStreams | [133:17](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L133) |
| anonymous-function | YoutubeExplode.Bridge.PlayerResponse.Streams.get.adaptiveStreams.anonymous@4942 | [136:26](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L136) |
| parameter | YoutubeExplode.Bridge.PlayerResponse.Streams.get.adaptiveStreams.anonymous@4942.j | [136:26](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L136) |
| property | YoutubeExplode.Bridge.PlayerResponse.ClosedCaptionTracks | [145:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L145) |
| anonymous-function | YoutubeExplode.Bridge.PlayerResponse.ClosedCaptionTracks.anonymous@5413 | [151:22](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L151) |
| parameter | YoutubeExplode.Bridge.PlayerResponse.ClosedCaptionTracks.anonymous@5413.j | [151:22](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L151) |
| type | YoutubeExplode.Bridge.PlayerResponse | [156:1](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L156) |
| type | YoutubeExplode.Bridge.PlayerResponse.ClosedCaptionTrackData | [158:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L158) |
| parameter | YoutubeExplode.Bridge.PlayerResponse.ClosedCaptionTrackData.content | [158:41](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L158) |
| property | YoutubeExplode.Bridge.PlayerResponse.ClosedCaptionTrackData.Url | [161:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L161) |
| property | YoutubeExplode.Bridge.PlayerResponse.ClosedCaptionTrackData.LanguageCode | [163:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L163) |
| property | YoutubeExplode.Bridge.PlayerResponse.ClosedCaptionTrackData.LanguageName | [165:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L165) |
| anonymous-function | YoutubeExplode.Bridge.PlayerResponse.ClosedCaptionTrackData.LanguageName.anonymous@6101 | [171:26](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L171) |
| parameter | YoutubeExplode.Bridge.PlayerResponse.ClosedCaptionTrackData.LanguageName.anonymous@6101.j | [171:26](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L171) |
| property | YoutubeExplode.Bridge.PlayerResponse.ClosedCaptionTrackData.IsAutoGenerated | [175:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L175) |
| type | YoutubeExplode.Bridge.PlayerResponse | [184:1](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L184) |
| type | YoutubeExplode.Bridge.PlayerResponse.StreamData | [186:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L186) |
| parameter | YoutubeExplode.Bridge.PlayerResponse.StreamData.content | [186:29](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L186) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.Itag | [189:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L189) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.CipherData | [191:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L191) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.Url | [198:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L198) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.Signature | [202:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L202) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.SignatureParameter | [204:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L204) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.ContentLength | [206:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L206) |
| anonymous-function | YoutubeExplode.Bridge.PlayerResponse.StreamData.ContentLength.anonymous@7461 | [210:24](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L210) |
| parameter | YoutubeExplode.Bridge.PlayerResponse.StreamData.ContentLength.anonymous@7461.s | [210:24](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L210) |
| anonymous-function | YoutubeExplode.Bridge.PlayerResponse.StreamData.ContentLength.anonymous@7556 | [211:26](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L211) |
| parameter | YoutubeExplode.Bridge.PlayerResponse.StreamData.ContentLength.anonymous@7556.s | [211:26](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L211) |
| anonymous-function | YoutubeExplode.Bridge.PlayerResponse.StreamData.ContentLength.anonymous@7665 | [213:24](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L213) |
| parameter | YoutubeExplode.Bridge.PlayerResponse.StreamData.ContentLength.anonymous@7665.s | [213:24](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L213) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.Bitrate | [215:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L215) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.MimeType | [217:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L217) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.Container | [219:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L219) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.IsAudioOnly | [221:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L221) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.Codecs | [224:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L224) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.AudioCodec | [226:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L226) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.AudioLanguageCode | [229:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L229) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.AudioLanguageName | [236:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L236) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.IsAudioLanguageDefault | [242:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L242) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.VideoCodec | [248:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L248) |
| accessor | YoutubeExplode.Bridge.PlayerResponse.StreamData.VideoCodec.get | [250:13](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L250) |
| local-variable | YoutubeExplode.Bridge.PlayerResponse.StreamData.VideoCodec.get.codec | [252:21](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L252) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.VideoQualityLabel | [262:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L262) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.VideoWidth | [265:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L265) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.VideoHeight | [267:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L267) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.IsVideoUpscaled | [269:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L269) |
| property | YoutubeExplode.Bridge.PlayerResponse.StreamData.VideoFramerate | [279:9](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L279) |
| type | YoutubeExplode.Bridge.PlayerResponse | [283:1](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L283) |
| method | YoutubeExplode.Bridge.PlayerResponse.Parse | [285:5](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L285) |
| parameter | YoutubeExplode.Bridge.PlayerResponse.Parse.raw | [285:40](../ManagedResolver/vendored/Bridge/PlayerResponse.cs#L285) |

</details>

<a id="file-0cd0bad381695bc3"></a>
## [ManagedResolver/vendored/Bridge/ThumbnailData.cs](../ManagedResolver/vendored/Bridge/ThumbnailData.cs)

Plugin Compile source (SDK default); 385 bytes; 14 lines.

<details>
<summary>All 6 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | YoutubeExplode.Bridge | [4:1](../ManagedResolver/vendored/Bridge/ThumbnailData.cs#L4) |
| type | YoutubeExplode.Bridge.ThumbnailData | [6:1](../ManagedResolver/vendored/Bridge/ThumbnailData.cs#L6) |
| parameter | YoutubeExplode.Bridge.ThumbnailData.content | [6:30](../ManagedResolver/vendored/Bridge/ThumbnailData.cs#L6) |
| property | YoutubeExplode.Bridge.ThumbnailData.Url | [9:5](../ManagedResolver/vendored/Bridge/ThumbnailData.cs#L9) |
| property | YoutubeExplode.Bridge.ThumbnailData.Width | [11:5](../ManagedResolver/vendored/Bridge/ThumbnailData.cs#L11) |
| property | YoutubeExplode.Bridge.ThumbnailData.Height | [13:5](../ManagedResolver/vendored/Bridge/ThumbnailData.cs#L13) |

</details>

<a id="file-95c161f9b0da96af"></a>
## [ManagedResolver/vendored/Exceptions/VideoUnavailableException.cs](../ManagedResolver/vendored/Exceptions/VideoUnavailableException.cs)

Plugin Compile source (SDK default); 221 bytes; 6 lines.

<details>
<summary>All 3 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | YoutubeExplode.Exceptions | [1:1](../ManagedResolver/vendored/Exceptions/VideoUnavailableException.cs#L1) |
| type | YoutubeExplode.Exceptions.VideoUnavailableException | [6:1](../ManagedResolver/vendored/Exceptions/VideoUnavailableException.cs#L6) |
| parameter | YoutubeExplode.Exceptions.VideoUnavailableException.message | [6:40](../ManagedResolver/vendored/Exceptions/VideoUnavailableException.cs#L6) |

</details>

<a id="file-f9669ca26d62c40d"></a>
## [ManagedResolver/vendored/Exceptions/VideoUnplayableException.cs](../ManagedResolver/vendored/Exceptions/VideoUnplayableException.cs)

Plugin Compile source (SDK default); 218 bytes; 6 lines.

<details>
<summary>All 3 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | YoutubeExplode.Exceptions | [1:1](../ManagedResolver/vendored/Exceptions/VideoUnplayableException.cs#L1) |
| type | YoutubeExplode.Exceptions.VideoUnplayableException | [6:1](../ManagedResolver/vendored/Exceptions/VideoUnplayableException.cs#L6) |
| parameter | YoutubeExplode.Exceptions.VideoUnplayableException.message | [6:39](../ManagedResolver/vendored/Exceptions/VideoUnplayableException.cs#L6) |

</details>

<a id="file-697e007577a4a1c3"></a>
## [ManagedResolver/vendored/Exceptions/YoutubeExplodeException.cs](../ManagedResolver/vendored/Exceptions/YoutubeExplodeException.cs)

Plugin Compile source (SDK default); 216 bytes; 8 lines.

<details>
<summary>All 3 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | YoutubeExplode.Exceptions | [3:1](../ManagedResolver/vendored/Exceptions/YoutubeExplodeException.cs#L3) |
| type | YoutubeExplode.Exceptions.YoutubeExplodeException | [8:1](../ManagedResolver/vendored/Exceptions/YoutubeExplodeException.cs#L8) |
| parameter | YoutubeExplode.Exceptions.YoutubeExplodeException.message | [8:38](../ManagedResolver/vendored/Exceptions/YoutubeExplodeException.cs#L8) |

</details>

<a id="file-2db2dc2c42455d05"></a>
## [ManagedResolver/vendored/Utils/Json.cs](../ManagedResolver/vendored/Utils/Json.cs)

Plugin Compile source (SDK default); 2173 bytes; 76 lines.

<details>
<summary>All 22 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | YoutubeExplode.Utils | [7:1](../ManagedResolver/vendored/Utils/Json.cs#L7) |
| type | YoutubeExplode.Utils.Json | [9:1](../ManagedResolver/vendored/Utils/Json.cs#L9) |
| method | YoutubeExplode.Utils.Json.Extract | [11:5](../ManagedResolver/vendored/Utils/Json.cs#L11) |
| parameter | YoutubeExplode.Utils.Json.Extract.source | [11:34](../ManagedResolver/vendored/Utils/Json.cs#L11) |
| local-variable | YoutubeExplode.Utils.Json.Extract.buffer | [13:13](../ManagedResolver/vendored/Utils/Json.cs#L13) |
| local-variable | YoutubeExplode.Utils.Json.Extract.depth | [15:13](../ManagedResolver/vendored/Utils/Json.cs#L15) |
| local-variable | YoutubeExplode.Utils.Json.Extract.isInsideString | [16:13](../ManagedResolver/vendored/Utils/Json.cs#L16) |
| pattern-variable | YoutubeExplode.Utils.Json.Extract.i | [20:23](../ManagedResolver/vendored/Utils/Json.cs#L20) |
| pattern-variable | YoutubeExplode.Utils.Json.Extract.ch | [20:26](../ManagedResolver/vendored/Utils/Json.cs#L20) |
| local-variable | YoutubeExplode.Utils.Json.Extract.prev | [22:17](../ManagedResolver/vendored/Utils/Json.cs#L22) |
| method | YoutubeExplode.Utils.Json.Parse | [44:5](../ManagedResolver/vendored/Utils/Json.cs#L44) |
| parameter | YoutubeExplode.Utils.Json.Parse.source | [44:37](../ManagedResolver/vendored/Utils/Json.cs#L44) |
| method | YoutubeExplode.Utils.Json.TryParse | [46:5](../ManagedResolver/vendored/Utils/Json.cs#L46) |
| parameter | YoutubeExplode.Utils.Json.TryParse.source | [46:41](../ManagedResolver/vendored/Utils/Json.cs#L46) |
| method | YoutubeExplode.Utils.Json.Escape | [48:5](../ManagedResolver/vendored/Utils/Json.cs#L48) |
| parameter | YoutubeExplode.Utils.Json.Escape.value | [48:33](../ManagedResolver/vendored/Utils/Json.cs#L48) |
| local-variable | YoutubeExplode.Utils.Json.Escape.buffer | [50:13](../ManagedResolver/vendored/Utils/Json.cs#L50) |
| iteration-variable | YoutubeExplode.Utils.Json.Escape.ch | [52:9](../ManagedResolver/vendored/Utils/Json.cs#L52) |
| method | YoutubeExplode.Utils.Json.Encode | [71:5](../ManagedResolver/vendored/Utils/Json.cs#L71) |
| parameter | YoutubeExplode.Utils.Json.Encode.value | [71:33](../ManagedResolver/vendored/Utils/Json.cs#L71) |
| method | YoutubeExplode.Utils.Json.Encode | [74:5](../ManagedResolver/vendored/Utils/Json.cs#L74) |
| parameter | YoutubeExplode.Utils.Json.Encode.value | [74:33](../ManagedResolver/vendored/Utils/Json.cs#L74) |

</details>

<a id="file-98e8c5ef86faa4ef"></a>
## [ManagedResolver/vendored/Utils/Protobuf.cs](../ManagedResolver/vendored/Utils/Protobuf.cs)

Plugin Compile source (SDK default); 3909 bytes; 135 lines.

<details>
<summary>All 34 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | YoutubeExplode.Utils | [5:1](../ManagedResolver/vendored/Utils/Protobuf.cs#L5) |
| type | YoutubeExplode.Utils.Protobuf | [7:1](../ManagedResolver/vendored/Utils/Protobuf.cs#L7) |
| method | YoutubeExplode.Utils.Protobuf.IsLenField | [9:5](../ManagedResolver/vendored/Utils/Protobuf.cs#L9) |
| parameter | YoutubeExplode.Utils.Protobuf.IsLenField.tag | [9:36](../ManagedResolver/vendored/Utils/Protobuf.cs#L9) |
| method | YoutubeExplode.Utils.Protobuf.TryReadVarint | [11:5](../ManagedResolver/vendored/Utils/Protobuf.cs#L11) |
| parameter | YoutubeExplode.Utils.Protobuf.TryReadVarint.data | [11:41](../ManagedResolver/vendored/Utils/Protobuf.cs#L11) |
| parameter | YoutubeExplode.Utils.Protobuf.TryReadVarint.i | [11:54](../ManagedResolver/vendored/Utils/Protobuf.cs#L11) |
| local-variable | YoutubeExplode.Utils.Protobuf.TryReadVarint.value | [13:13](../ManagedResolver/vendored/Utils/Protobuf.cs#L13) |
| local-variable | YoutubeExplode.Utils.Protobuf.TryReadVarint.shift | [14:13](../ManagedResolver/vendored/Utils/Protobuf.cs#L14) |
| local-variable | YoutubeExplode.Utils.Protobuf.TryReadVarint.b | [17:17](../ManagedResolver/vendored/Utils/Protobuf.cs#L17) |
| method | YoutubeExplode.Utils.Protobuf.WriteVarint | [32:5](../ManagedResolver/vendored/Utils/Protobuf.cs#L32) |
| parameter | YoutubeExplode.Utils.Protobuf.WriteVarint.buffer | [32:36](../ManagedResolver/vendored/Utils/Protobuf.cs#L32) |
| parameter | YoutubeExplode.Utils.Protobuf.WriteVarint.value | [32:55](../ManagedResolver/vendored/Utils/Protobuf.cs#L32) |
| method | YoutubeExplode.Utils.Protobuf.TryReadString | [43:5](../ManagedResolver/vendored/Utils/Protobuf.cs#L43) |
| parameter | YoutubeExplode.Utils.Protobuf.TryReadString.data | [43:42](../ManagedResolver/vendored/Utils/Protobuf.cs#L43) |
| parameter | YoutubeExplode.Utils.Protobuf.TryReadString.i | [43:55](../ManagedResolver/vendored/Utils/Protobuf.cs#L43) |
| local-variable | YoutubeExplode.Utils.Protobuf.TryReadString.length | [45:13](../ManagedResolver/vendored/Utils/Protobuf.cs#L45) |
| local-variable | YoutubeExplode.Utils.Protobuf.TryReadString.result | [52:13](../ManagedResolver/vendored/Utils/Protobuf.cs#L52) |
| method | YoutubeExplode.Utils.Protobuf.TryDeserializeMap | [62:5](../ManagedResolver/vendored/Utils/Protobuf.cs#L62) |
| parameter | YoutubeExplode.Utils.Protobuf.TryDeserializeMap.data | [62:75](../ManagedResolver/vendored/Utils/Protobuf.cs#L62) |
| local-variable | YoutubeExplode.Utils.Protobuf.TryDeserializeMap.result | [64:13](../ManagedResolver/vendored/Utils/Protobuf.cs#L64) |
| local-variable | YoutubeExplode.Utils.Protobuf.TryDeserializeMap.i | [66:13](../ManagedResolver/vendored/Utils/Protobuf.cs#L66) |
| local-variable | YoutubeExplode.Utils.Protobuf.TryDeserializeMap.outerTag | [69:17](../ManagedResolver/vendored/Utils/Protobuf.cs#L69) |
| local-variable | YoutubeExplode.Utils.Protobuf.TryDeserializeMap.entryLen | [77:17](../ManagedResolver/vendored/Utils/Protobuf.cs#L77) |
| local-variable | YoutubeExplode.Utils.Protobuf.TryDeserializeMap.entryEnd | [82:17](../ManagedResolver/vendored/Utils/Protobuf.cs#L82) |
| local-variable | YoutubeExplode.Utils.Protobuf.TryDeserializeMap.key | [87:17](../ManagedResolver/vendored/Utils/Protobuf.cs#L87) |
| local-variable | YoutubeExplode.Utils.Protobuf.TryDeserializeMap.value | [88:17](../ManagedResolver/vendored/Utils/Protobuf.cs#L88) |
| local-variable | YoutubeExplode.Utils.Protobuf.TryDeserializeMap.j | [89:17](../ManagedResolver/vendored/Utils/Protobuf.cs#L89) |
| local-variable | YoutubeExplode.Utils.Protobuf.TryDeserializeMap.fieldTag | [92:21](../ManagedResolver/vendored/Utils/Protobuf.cs#L92) |
| local-variable | YoutubeExplode.Utils.Protobuf.TryDeserializeMap.fieldNum | [100:21](../ManagedResolver/vendored/Utils/Protobuf.cs#L100) |
| local-variable | YoutubeExplode.Utils.Protobuf.TryDeserializeMap.str | [102:21](../ManagedResolver/vendored/Utils/Protobuf.cs#L102) |
| method | YoutubeExplode.Utils.Protobuf.TryDeserializeMap | [123:5](../ManagedResolver/vendored/Utils/Protobuf.cs#L123) |
| parameter | YoutubeExplode.Utils.Protobuf.TryDeserializeMap.base64 | [123:75](../ManagedResolver/vendored/Utils/Protobuf.cs#L123) |
| local-variable | YoutubeExplode.Utils.Protobuf.TryDeserializeMap.bytes | [127:17](../ManagedResolver/vendored/Utils/Protobuf.cs#L127) |

</details>

<a id="file-e3e506b64848f0af"></a>
## [ManagedResolver/vendored/Utils/UrlEx.cs](../ManagedResolver/vendored/Utils/UrlEx.cs)

Plugin Compile source (SDK default); 2702 bytes; 84 lines.

<details>
<summary>All 36 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | YoutubeExplode.Utils | [8:1](../ManagedResolver/vendored/Utils/UrlEx.cs#L8) |
| type | YoutubeExplode.Utils.UrlEx | [10:1](../ManagedResolver/vendored/Utils/UrlEx.cs#L10) |
| method | YoutubeExplode.Utils.UrlEx.EnumerateQueryParameters | [12:5](../ManagedResolver/vendored/Utils/UrlEx.cs#L12) |
| parameter | YoutubeExplode.Utils.UrlEx.EnumerateQueryParameters.url | [12:87](../ManagedResolver/vendored/Utils/UrlEx.cs#L12) |
| local-variable | YoutubeExplode.Utils.UrlEx.EnumerateQueryParameters.query | [14:13](../ManagedResolver/vendored/Utils/UrlEx.cs#L14) |
| iteration-variable | YoutubeExplode.Utils.UrlEx.EnumerateQueryParameters.parameter | [16:9](../ManagedResolver/vendored/Utils/UrlEx.cs#L16) |
| local-variable | YoutubeExplode.Utils.UrlEx.EnumerateQueryParameters.parameter.key | [18:17](../ManagedResolver/vendored/Utils/UrlEx.cs#L18) |
| local-variable | YoutubeExplode.Utils.UrlEx.EnumerateQueryParameters.parameter.value | [19:17](../ManagedResolver/vendored/Utils/UrlEx.cs#L19) |
| method | YoutubeExplode.Utils.UrlEx.GetQueryParameters | [28:5](../ManagedResolver/vendored/Utils/UrlEx.cs#L28) |
| parameter | YoutubeExplode.Utils.UrlEx.GetQueryParameters.url | [28:74](../ManagedResolver/vendored/Utils/UrlEx.cs#L28) |
| anonymous-function | YoutubeExplode.Utils.UrlEx.GetQueryParameters.anonymous@888 | [29:52](../ManagedResolver/vendored/Utils/UrlEx.cs#L29) |
| parameter | YoutubeExplode.Utils.UrlEx.GetQueryParameters.anonymous@888.kvp | [29:52](../ManagedResolver/vendored/Utils/UrlEx.cs#L29) |
| anonymous-function | YoutubeExplode.Utils.UrlEx.GetQueryParameters.anonymous@904 | [29:68](../ManagedResolver/vendored/Utils/UrlEx.cs#L29) |
| parameter | YoutubeExplode.Utils.UrlEx.GetQueryParameters.anonymous@904.kvp | [29:68](../ManagedResolver/vendored/Utils/UrlEx.cs#L29) |
| method | YoutubeExplode.Utils.UrlEx.TryGetQueryParameter | [31:5](../ManagedResolver/vendored/Utils/UrlEx.cs#L31) |
| parameter | YoutubeExplode.Utils.UrlEx.TryGetQueryParameter.url | [31:71](../ManagedResolver/vendored/Utils/UrlEx.cs#L31) |
| parameter | YoutubeExplode.Utils.UrlEx.TryGetQueryParameter.key | [31:83](../ManagedResolver/vendored/Utils/UrlEx.cs#L31) |
| iteration-variable | YoutubeExplode.Utils.UrlEx.TryGetQueryParameter.parameter | [33:9](../ManagedResolver/vendored/Utils/UrlEx.cs#L33) |
| method | YoutubeExplode.Utils.UrlEx.TryGetQueryParameterValue | [42:5](../ManagedResolver/vendored/Utils/UrlEx.cs#L42) |
| parameter | YoutubeExplode.Utils.UrlEx.TryGetQueryParameterValue.url | [42:53](../ManagedResolver/vendored/Utils/UrlEx.cs#L42) |
| parameter | YoutubeExplode.Utils.UrlEx.TryGetQueryParameterValue.key | [42:65](../ManagedResolver/vendored/Utils/UrlEx.cs#L42) |
| method | YoutubeExplode.Utils.UrlEx.ContainsQueryParameter | [45:5](../ManagedResolver/vendored/Utils/UrlEx.cs#L45) |
| parameter | YoutubeExplode.Utils.UrlEx.ContainsQueryParameter.url | [45:47](../ManagedResolver/vendored/Utils/UrlEx.cs#L45) |
| parameter | YoutubeExplode.Utils.UrlEx.ContainsQueryParameter.key | [45:59](../ManagedResolver/vendored/Utils/UrlEx.cs#L45) |
| method | YoutubeExplode.Utils.UrlEx.RemoveQueryParameter | [48:5](../ManagedResolver/vendored/Utils/UrlEx.cs#L48) |
| parameter | YoutubeExplode.Utils.UrlEx.RemoveQueryParameter.url | [48:47](../ManagedResolver/vendored/Utils/UrlEx.cs#L48) |
| parameter | YoutubeExplode.Utils.UrlEx.RemoveQueryParameter.key | [48:59](../ManagedResolver/vendored/Utils/UrlEx.cs#L48) |
| local-variable | YoutubeExplode.Utils.UrlEx.RemoveQueryParameter.urlBuilder | [53:13](../ManagedResolver/vendored/Utils/UrlEx.cs#L53) |
| local-variable | YoutubeExplode.Utils.UrlEx.RemoveQueryParameter.queryBuilder | [54:13](../ManagedResolver/vendored/Utils/UrlEx.cs#L54) |
| iteration-variable | YoutubeExplode.Utils.UrlEx.RemoveQueryParameter.parameter | [56:9](../ManagedResolver/vendored/Utils/UrlEx.cs#L56) |
| method | YoutubeExplode.Utils.UrlEx.SetQueryParameter | [73:5](../ManagedResolver/vendored/Utils/UrlEx.cs#L73) |
| parameter | YoutubeExplode.Utils.UrlEx.SetQueryParameter.url | [73:44](../ManagedResolver/vendored/Utils/UrlEx.cs#L73) |
| parameter | YoutubeExplode.Utils.UrlEx.SetQueryParameter.key | [73:56](../ManagedResolver/vendored/Utils/UrlEx.cs#L73) |
| parameter | YoutubeExplode.Utils.UrlEx.SetQueryParameter.value | [73:68](../ManagedResolver/vendored/Utils/UrlEx.cs#L73) |
| local-variable | YoutubeExplode.Utils.UrlEx.SetQueryParameter.urlWithoutParameter | [75:13](../ManagedResolver/vendored/Utils/UrlEx.cs#L75) |
| local-variable | YoutubeExplode.Utils.UrlEx.SetQueryParameter.hasOtherParameters | [76:13](../ManagedResolver/vendored/Utils/UrlEx.cs#L76) |

</details>

<a id="file-3d45031c9e99dad6"></a>
## [ManagedResolver/vendored/Videos/VideoController.cs](../ManagedResolver/vendored/Videos/VideoController.cs)

Plugin Compile source (SDK default); 4351 bytes; 131 lines.

<details>
<summary>All 28 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | YoutubeExplode.Videos | [11:1](../ManagedResolver/vendored/Videos/VideoController.cs#L11) |
| type | YoutubeExplode.Videos.VideoController | [13:1](../ManagedResolver/vendored/Videos/VideoController.cs#L13) |
| parameter | YoutubeExplode.Videos.VideoController.http | [13:32](../ManagedResolver/vendored/Videos/VideoController.cs#L13) |
| field | YoutubeExplode.Videos.VideoController._visitorData | [15:21](../ManagedResolver/vendored/Videos/VideoController.cs#L15) |
| property | YoutubeExplode.Videos.VideoController.Http | [17:5](../ManagedResolver/vendored/Videos/VideoController.cs#L17) |
| accessor | YoutubeExplode.Videos.VideoController.Http.get | [17:33](../ManagedResolver/vendored/Videos/VideoController.cs#L17) |
| method | YoutubeExplode.Videos.VideoController.ResolveVisitorDataAsync | [19:5](../ManagedResolver/vendored/Videos/VideoController.cs#L19) |
| parameter | YoutubeExplode.Videos.VideoController.ResolveVisitorDataAsync.cancellationToken | [20:9](../ManagedResolver/vendored/Videos/VideoController.cs#L20) |
| local-variable | YoutubeExplode.Videos.VideoController.ResolveVisitorDataAsync.request | [26:19](../ManagedResolver/vendored/Videos/VideoController.cs#L26) |
| local-variable | YoutubeExplode.Videos.VideoController.ResolveVisitorDataAsync.response | [38:19](../ManagedResolver/vendored/Videos/VideoController.cs#L38) |
| local-variable | YoutubeExplode.Videos.VideoController.ResolveVisitorDataAsync.jsonString | [42:13](../ManagedResolver/vendored/Videos/VideoController.cs#L42) |
| local-variable | YoutubeExplode.Videos.VideoController.ResolveVisitorDataAsync.json | [46:13](../ManagedResolver/vendored/Videos/VideoController.cs#L46) |
| local-variable | YoutubeExplode.Videos.VideoController.ResolveVisitorDataAsync.value | [49:13](../ManagedResolver/vendored/Videos/VideoController.cs#L49) |
| method | YoutubeExplode.Videos.VideoController.GetPlayerResponseForVisionOsAsync | [58:5](../ManagedResolver/vendored/Videos/VideoController.cs#L58) |
| parameter | YoutubeExplode.Videos.VideoController.GetPlayerResponseForVisionOsAsync.videoId | [59:9](../ManagedResolver/vendored/Videos/VideoController.cs#L59) |
| parameter | YoutubeExplode.Videos.VideoController.GetPlayerResponseForVisionOsAsync.visitorData | [60:9](../ManagedResolver/vendored/Videos/VideoController.cs#L60) |
| parameter | YoutubeExplode.Videos.VideoController.GetPlayerResponseForVisionOsAsync.cancellationToken | [61:9](../ManagedResolver/vendored/Videos/VideoController.cs#L61) |
| local-variable | YoutubeExplode.Videos.VideoController.GetPlayerResponseForVisionOsAsync.request | [64:19](../ManagedResolver/vendored/Videos/VideoController.cs#L64) |
| local-variable | YoutubeExplode.Videos.VideoController.GetPlayerResponseForVisionOsAsync.response | [99:19](../ManagedResolver/vendored/Videos/VideoController.cs#L99) |
| local-variable | YoutubeExplode.Videos.VideoController.GetPlayerResponseForVisionOsAsync.playerResponse | [102:13](../ManagedResolver/vendored/Videos/VideoController.cs#L102) |
| method | YoutubeExplode.Videos.VideoController.GetPlayerResponseAsync | [115:5](../ManagedResolver/vendored/Videos/VideoController.cs#L115) |
| parameter | YoutubeExplode.Videos.VideoController.GetPlayerResponseAsync.videoId | [116:9](../ManagedResolver/vendored/Videos/VideoController.cs#L116) |
| parameter | YoutubeExplode.Videos.VideoController.GetPlayerResponseAsync.signatureTimestamp | [117:9](../ManagedResolver/vendored/Videos/VideoController.cs#L117) |
| parameter | YoutubeExplode.Videos.VideoController.GetPlayerResponseAsync.cancellationToken | [118:9](../ManagedResolver/vendored/Videos/VideoController.cs#L118) |
| local-variable | YoutubeExplode.Videos.VideoController.GetPlayerResponseAsync.visitorData | [123:13](../ManagedResolver/vendored/Videos/VideoController.cs#L123) |
| method | YoutubeExplode.Videos.VideoController.GetPlayerResponseAsync | [127:5](../ManagedResolver/vendored/Videos/VideoController.cs#L127) |
| parameter | YoutubeExplode.Videos.VideoController.GetPlayerResponseAsync.videoId | [128:9](../ManagedResolver/vendored/Videos/VideoController.cs#L128) |
| parameter | YoutubeExplode.Videos.VideoController.GetPlayerResponseAsync.cancellationToken | [129:9](../ManagedResolver/vendored/Videos/VideoController.cs#L129) |

</details>

<a id="file-c4d93fe674f77ff2"></a>
## [ManagedResolver/vendored/Videos/VideoId.cs](../ManagedResolver/vendored/Videos/VideoId.cs)

Plugin Compile source (SDK default); 3892 bytes; 111 lines.

<details>
<summary>All 40 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | YoutubeExplode.Videos | [7:1](../ManagedResolver/vendored/Videos/VideoId.cs#L7) |
| type | YoutubeExplode.Videos.VideoId | [12:1](../ManagedResolver/vendored/Videos/VideoId.cs#L12) |
| parameter | YoutubeExplode.Videos.VideoId.value | [12:40](../ManagedResolver/vendored/Videos/VideoId.cs#L12) |
| property | YoutubeExplode.Videos.VideoId.Value | [17:5](../ManagedResolver/vendored/Videos/VideoId.cs#L17) |
| accessor | YoutubeExplode.Videos.VideoId.Value.get | [17:27](../ManagedResolver/vendored/Videos/VideoId.cs#L17) |
| method | YoutubeExplode.Videos.VideoId.ToString | [20:5](../ManagedResolver/vendored/Videos/VideoId.cs#L20) |
| type | YoutubeExplode.Videos.VideoId | [23:1](../ManagedResolver/vendored/Videos/VideoId.cs#L23) |
| method | YoutubeExplode.Videos.VideoId.IsValid | [25:5](../ManagedResolver/vendored/Videos/VideoId.cs#L25) |
| parameter | YoutubeExplode.Videos.VideoId.IsValid.videoId | [25:33](../ManagedResolver/vendored/Videos/VideoId.cs#L25) |
| anonymous-function | YoutubeExplode.Videos.VideoId.IsValid.anonymous@593 | [26:45](../ManagedResolver/vendored/Videos/VideoId.cs#L26) |
| parameter | YoutubeExplode.Videos.VideoId.IsValid.anonymous@593.c | [26:45](../ManagedResolver/vendored/Videos/VideoId.cs#L26) |
| method | YoutubeExplode.Videos.VideoId.TryNormalize | [28:5](../ManagedResolver/vendored/Videos/VideoId.cs#L28) |
| parameter | YoutubeExplode.Videos.VideoId.TryNormalize.videoIdOrUrl | [28:41](../ManagedResolver/vendored/Videos/VideoId.cs#L28) |
| local-function | YoutubeExplode.Videos.VideoId.TryNormalize.TryExtractId | [30:9](../ManagedResolver/vendored/Videos/VideoId.cs#L30) |
| parameter | YoutubeExplode.Videos.VideoId.TryNormalize.TryExtractId.url | [30:37](../ManagedResolver/vendored/Videos/VideoId.cs#L30) |
| parameter | YoutubeExplode.Videos.VideoId.TryNormalize.TryExtractId.pattern | [30:49](../ManagedResolver/vendored/Videos/VideoId.cs#L30) |
| local-variable | YoutubeExplode.Videos.VideoId.TryNormalize.TryExtractId.id | [32:17](../ManagedResolver/vendored/Videos/VideoId.cs#L32) |
| method | YoutubeExplode.Videos.VideoId.TryParse | [70:5](../ManagedResolver/vendored/Videos/VideoId.cs#L70) |
| parameter | YoutubeExplode.Videos.VideoId.TryParse.videoIdOrUrl | [70:37](../ManagedResolver/vendored/Videos/VideoId.cs#L70) |
| anonymous-function | YoutubeExplode.Videos.VideoId.TryParse.anonymous@2517 | [71:42](../ManagedResolver/vendored/Videos/VideoId.cs#L71) |
| parameter | YoutubeExplode.Videos.VideoId.TryParse.anonymous@2517.id | [71:42](../ManagedResolver/vendored/Videos/VideoId.cs#L71) |
| method | YoutubeExplode.Videos.VideoId.Parse | [77:5](../ManagedResolver/vendored/Videos/VideoId.cs#L77) |
| parameter | YoutubeExplode.Videos.VideoId.Parse.videoIdOrUrl | [77:33](../ManagedResolver/vendored/Videos/VideoId.cs#L77) |
| conversion | YoutubeExplode.Videos.VideoId.implicit operator VideoId | [84:5](../ManagedResolver/vendored/Videos/VideoId.cs#L84) |
| parameter | YoutubeExplode.Videos.VideoId.implicit operator VideoId.videoIdOrUrl | [84:45](../ManagedResolver/vendored/Videos/VideoId.cs#L84) |
| conversion | YoutubeExplode.Videos.VideoId.implicit operator string | [89:5](../ManagedResolver/vendored/Videos/VideoId.cs#L89) |
| parameter | YoutubeExplode.Videos.VideoId.implicit operator string.videoId | [89:44](../ManagedResolver/vendored/Videos/VideoId.cs#L89) |
| type | YoutubeExplode.Videos.VideoId | [92:1](../ManagedResolver/vendored/Videos/VideoId.cs#L92) |
| method | YoutubeExplode.Videos.VideoId.Equals | [95:5](../ManagedResolver/vendored/Videos/VideoId.cs#L95) |
| parameter | YoutubeExplode.Videos.VideoId.Equals.other | [95:24](../ManagedResolver/vendored/Videos/VideoId.cs#L95) |
| method | YoutubeExplode.Videos.VideoId.Equals | [99:5](../ManagedResolver/vendored/Videos/VideoId.cs#L99) |
| parameter | YoutubeExplode.Videos.VideoId.Equals.obj | [99:33](../ManagedResolver/vendored/Videos/VideoId.cs#L99) |
| pattern-variable | YoutubeExplode.Videos.VideoId.Equals.other | [99:64](../ManagedResolver/vendored/Videos/VideoId.cs#L99) |
| method | YoutubeExplode.Videos.VideoId.GetHashCode | [102:5](../ManagedResolver/vendored/Videos/VideoId.cs#L102) |
| operator | YoutubeExplode.Videos.VideoId.operator == | [107:5](../ManagedResolver/vendored/Videos/VideoId.cs#L107) |
| parameter | YoutubeExplode.Videos.VideoId.operator ==.left | [107:36](../ManagedResolver/vendored/Videos/VideoId.cs#L107) |
| parameter | YoutubeExplode.Videos.VideoId.operator ==.right | [107:50](../ManagedResolver/vendored/Videos/VideoId.cs#L107) |
| operator | YoutubeExplode.Videos.VideoId.operator != | [110:5](../ManagedResolver/vendored/Videos/VideoId.cs#L110) |
| parameter | YoutubeExplode.Videos.VideoId.operator !=.left | [110:36](../ManagedResolver/vendored/Videos/VideoId.cs#L110) |
| parameter | YoutubeExplode.Videos.VideoId.operator !=.right | [110:50](../ManagedResolver/vendored/Videos/VideoId.cs#L110) |

</details>

<a id="file-6ff472c354325e8c"></a>
## [PlaybackIntentInterceptor.cs](../PlaybackIntentInterceptor.cs)

Plugin Compile source (SDK default); 73326 bytes; 1649 lines.

<details>
<summary>All 475 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [18:1](../PlaybackIntentInterceptor.cs#L18) |
| type | Emby.YouTubePlugin.PlaybackIntent | [22:5](../PlaybackIntentInterceptor.cs#L22) |
| parameter | Emby.YouTubePlugin.PlaybackIntent.ItemId | [23:9](../PlaybackIntentInterceptor.cs#L23) |
| parameter | Emby.YouTubePlugin.PlaybackIntent.UserId | [24:9](../PlaybackIntentInterceptor.cs#L24) |
| parameter | Emby.YouTubePlugin.PlaybackIntent.DeviceId | [25:9](../PlaybackIntentInterceptor.cs#L25) |
| parameter | Emby.YouTubePlugin.PlaybackIntent.StartTimeTicks | [26:9](../PlaybackIntentInterceptor.cs#L26) |
| parameter | Emby.YouTubePlugin.PlaybackIntent.CapturedUtc | [27:9](../PlaybackIntentInterceptor.cs#L27) |
| type | Emby.YouTubePlugin.PlaybackInfoPatchContext | [29:5](../PlaybackIntentInterceptor.cs#L29) |
| parameter | Emby.YouTubePlugin.PlaybackInfoPatchContext.ItemId | [30:9](../PlaybackIntentInterceptor.cs#L30) |
| parameter | Emby.YouTubePlugin.PlaybackInfoPatchContext.Client | [31:9](../PlaybackIntentInterceptor.cs#L31) |
| parameter | Emby.YouTubePlugin.PlaybackInfoPatchContext.DeviceName | [32:9](../PlaybackIntentInterceptor.cs#L32) |
| parameter | Emby.YouTubePlugin.PlaybackInfoPatchContext.UserAgent | [33:9](../PlaybackIntentInterceptor.cs#L33) |
| parameter | Emby.YouTubePlugin.PlaybackInfoPatchContext.IpAddress | [34:9](../PlaybackIntentInterceptor.cs#L34) |
| parameter | Emby.YouTubePlugin.PlaybackInfoPatchContext.MaxStreamingBitrate | [35:9](../PlaybackIntentInterceptor.cs#L35) |
| parameter | Emby.YouTubePlugin.PlaybackInfoPatchContext.StartTimeTicks | [36:9](../PlaybackIntentInterceptor.cs#L36) |
| parameter | Emby.YouTubePlugin.PlaybackInfoPatchContext.HasStartTimeTicks | [37:9](../PlaybackIntentInterceptor.cs#L37) |
| parameter | Emby.YouTubePlugin.PlaybackInfoPatchContext.IsPlayback | [38:9](../PlaybackIntentInterceptor.cs#L38) |
| parameter | Emby.YouTubePlugin.PlaybackInfoPatchContext.IsLikelyIos | [39:9](../PlaybackIntentInterceptor.cs#L39) |
| parameter | Emby.YouTubePlugin.PlaybackInfoPatchContext.IsLikelyNativeAndroid | [40:9](../PlaybackIntentInterceptor.cs#L40) |
| parameter | Emby.YouTubePlugin.PlaybackInfoPatchContext.IsLikelyNativeTheater | [41:9](../PlaybackIntentInterceptor.cs#L41) |
| type | Emby.YouTubePlugin.PlaybackInfoNormalizationResult | [43:5](../PlaybackIntentInterceptor.cs#L43) |
| parameter | Emby.YouTubePlugin.PlaybackInfoNormalizationResult.Changed | [44:9](../PlaybackIntentInterceptor.cs#L44) |
| parameter | Emby.YouTubePlugin.PlaybackInfoNormalizationResult.NativeDirectPlayDisabled | [45:9](../PlaybackIntentInterceptor.cs#L45) |
| type | Emby.YouTubePlugin.PlaybackIntentInterceptor | [49:5](../PlaybackIntentInterceptor.cs#L49) |
| field | Emby.YouTubePlugin.PlaybackIntentInterceptor.HarmonyId | [51:30](../PlaybackIntentInterceptor.cs#L51) |
| field | Emby.YouTubePlugin.PlaybackIntentInterceptor.IntentTtl | [52:42](../PlaybackIntentInterceptor.cs#L52) |
| field | Emby.YouTubePlugin.PlaybackIntentInterceptor.YouTubeVideoIdRegex | [53:39](../PlaybackIntentInterceptor.cs#L53) |
| field | Emby.YouTubePlugin.PlaybackIntentInterceptor.Intents | [56:78](../PlaybackIntentInterceptor.cs#L56) |
| field | Emby.YouTubePlugin.PlaybackIntentInterceptor.Sync | [57:40](../PlaybackIntentInterceptor.cs#L57) |
| field | Emby.YouTubePlugin.PlaybackIntentInterceptor._harmony | [58:32](../PlaybackIntentInterceptor.cs#L58) |
| field | Emby.YouTubePlugin.PlaybackIntentInterceptor._harmonyType | [59:30](../PlaybackIntentInterceptor.cs#L59) |
| field | Emby.YouTubePlugin.PlaybackIntentInterceptor._lastCleanupUtc | [60:33](../PlaybackIntentInterceptor.cs#L60) |
| field | Emby.YouTubePlugin.PlaybackIntentInterceptor._playbackInfoLogsRemaining | [61:28](../PlaybackIntentInterceptor.cs#L61) |
| field | Emby.YouTubePlugin.PlaybackIntentInterceptor._mediaSourceNormalizeLogsRemaining | [62:28](../PlaybackIntentInterceptor.cs#L62) |
| field | Emby.YouTubePlugin.PlaybackIntentInterceptor._startStampSuppressLogsRemaining | [63:28](../PlaybackIntentInterceptor.cs#L63) |
| field | Emby.YouTubePlugin.PlaybackIntentInterceptor._legacyTabletSourceLogsRemaining | [64:28](../PlaybackIntentInterceptor.cs#L64) |
| field | Emby.YouTubePlugin.PlaybackIntentInterceptor._requestSubtitleSuppressLogsRemaining | [65:28](../PlaybackIntentInterceptor.cs#L65) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.DiscardCapturedIntents | [67:9](../PlaybackIntentInterceptor.cs#L67) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.Install | [72:9](../PlaybackIntentInterceptor.cs#L72) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.Install.mediaInfoServiceType | [83:25](../PlaybackIntentInterceptor.cs#L83) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.Install.harmonyAssembly | [92:25](../PlaybackIntentInterceptor.cs#L92) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.Install.harmonyType | [93:25](../PlaybackIntentInterceptor.cs#L93) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.Install.harmonyMethodType | [94:25](../PlaybackIntentInterceptor.cs#L94) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.Install.postfixMethod | [101:25](../PlaybackIntentInterceptor.cs#L101) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.Install.prefixMethod | [103:25](../PlaybackIntentInterceptor.cs#L103) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.Install.prefix | [111:25](../PlaybackIntentInterceptor.cs#L111) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.Install.postfix | [112:25](../PlaybackIntentInterceptor.cs#L112) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.Install.harmony | [113:25](../PlaybackIntentInterceptor.cs#L113) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.Install.patched | [120:25](../PlaybackIntentInterceptor.cs#L120) |
| catch-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.Install.ex | [146:23](../PlaybackIntentInterceptor.cs#L146) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.Uninstall | [153:9](../PlaybackIntentInterceptor.cs#L153) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.Uninstall.unpatchAll | [161:25](../PlaybackIntentInterceptor.cs#L161) |
| anonymous-function | Emby.YouTubePlugin.PlaybackIntentInterceptor.Uninstall.unpatchAll.anonymous@7239 | [162:41](../PlaybackIntentInterceptor.cs#L162) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.Uninstall.unpatchAll.anonymous@7239.m | [162:41](../PlaybackIntentInterceptor.cs#L162) |
| catch-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.Uninstall.ex | [170:23](../PlaybackIntentInterceptor.cs#L170) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsume | [177:9](../PlaybackIntentInterceptor.cs#L177) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsume.userId | [177:41](../PlaybackIntentInterceptor.cs#L177) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsume.itemId | [177:57](../PlaybackIntentInterceptor.cs#L177) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsume.deviceId | [177:70](../PlaybackIntentInterceptor.cs#L177) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsume.intent | [177:88](../PlaybackIntentInterceptor.cs#L177) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeForResume | [183:9](../PlaybackIntentInterceptor.cs#L183) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeForResume.userId | [184:13](../PlaybackIntentInterceptor.cs#L184) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeForResume.itemId | [185:13](../PlaybackIntentInterceptor.cs#L185) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeForResume.deviceId | [186:13](../PlaybackIntentInterceptor.cs#L186) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeForResume.intent | [187:13](../PlaybackIntentInterceptor.cs#L187) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeForResume.ambiguous | [188:13](../PlaybackIntentInterceptor.cs#L188) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeCore | [194:9](../PlaybackIntentInterceptor.cs#L194) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeCore.userId | [195:13](../PlaybackIntentInterceptor.cs#L195) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeCore.itemId | [196:13](../PlaybackIntentInterceptor.cs#L196) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeCore.deviceId | [197:13](../PlaybackIntentInterceptor.cs#L197) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeCore.intent | [198:13](../PlaybackIntentInterceptor.cs#L198) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeCore.ambiguous | [199:13](../PlaybackIntentInterceptor.cs#L199) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeCore.normalizedUserId | [206:17](../PlaybackIntentInterceptor.cs#L206) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeCore.normalizedItemId | [207:17](../PlaybackIntentInterceptor.cs#L207) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeCore.normalizedDeviceId | [208:17](../PlaybackIntentInterceptor.cs#L208) |
| anonymous-function | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeCore.anonymous@9335 | [213:41](../PlaybackIntentInterceptor.cs#L213) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeCore.anonymous@9335.candidate | [213:41](../PlaybackIntentInterceptor.cs#L213) |
| iteration-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeCore.kvp | [235:13](../PlaybackIntentInterceptor.cs#L235) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeCore.kvp.candidate | [237:21](../PlaybackIntentInterceptor.cs#L237) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeKey | [259:9](../PlaybackIntentInterceptor.cs#L259) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeKey.key | [259:43](../PlaybackIntentInterceptor.cs#L259) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeKey.intent | [259:55](../PlaybackIntentInterceptor.cs#L259) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.RemoveIntentAliases | [272:9](../PlaybackIntentInterceptor.cs#L272) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.RemoveIntentAliases.consumed | [272:49](../PlaybackIntentInterceptor.cs#L272) |
| iteration-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.RemoveIntentAliases.pair | [274:13](../PlaybackIntentInterceptor.cs#L274) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchPublicMethod | [281:9](../PlaybackIntentInterceptor.cs#L281) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchPublicMethod.harmony | [282:13](../PlaybackIntentInterceptor.cs#L282) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchPublicMethod.prefix | [283:13](../PlaybackIntentInterceptor.cs#L283) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchPublicMethod.postfix | [284:13](../PlaybackIntentInterceptor.cs#L284) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchPublicMethod.serviceType | [285:13](../PlaybackIntentInterceptor.cs#L285) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchPublicMethod.methodName | [286:13](../PlaybackIntentInterceptor.cs#L286) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchPublicMethod.requestTypeName | [287:13](../PlaybackIntentInterceptor.cs#L287) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchPublicMethod.requestType | [289:17](../PlaybackIntentInterceptor.cs#L289) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchPublicMethod.method | [297:17](../PlaybackIntentInterceptor.cs#L297) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.InvokeHarmonyPatch | [314:9](../PlaybackIntentInterceptor.cs#L314) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.InvokeHarmonyPatch.harmony | [314:48](../PlaybackIntentInterceptor.cs#L314) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.InvokeHarmonyPatch.original | [314:64](../PlaybackIntentInterceptor.cs#L314) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.InvokeHarmonyPatch.prefix | [314:85](../PlaybackIntentInterceptor.cs#L314) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.InvokeHarmonyPatch.postfix | [314:100](../PlaybackIntentInterceptor.cs#L314) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.InvokeHarmonyPatch.patchMethod | [316:17](../PlaybackIntentInterceptor.cs#L316) |
| anonymous-function | Emby.YouTubePlugin.PlaybackIntentInterceptor.InvokeHarmonyPatch.patchMethod.anonymous@13349 | [318:33](../PlaybackIntentInterceptor.cs#L318) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.InvokeHarmonyPatch.patchMethod.anonymous@13349.m | [318:33](../PlaybackIntentInterceptor.cs#L318) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.InvokeHarmonyPatch.patchMethod.anonymous@13349.parameters | [323:25](../PlaybackIntentInterceptor.cs#L323) |
| anonymous-function | Emby.YouTubePlugin.PlaybackIntentInterceptor.InvokeHarmonyPatch.patchMethod.anonymous@13349.anonymous@13700 | [326:46](../PlaybackIntentInterceptor.cs#L326) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.InvokeHarmonyPatch.patchMethod.anonymous@13349.anonymous@13700.p | [326:46](../PlaybackIntentInterceptor.cs#L326) |
| anonymous-function | Emby.YouTubePlugin.PlaybackIntentInterceptor.InvokeHarmonyPatch.patchMethod.anonymous@13349.anonymous@13819 | [327:46](../PlaybackIntentInterceptor.cs#L327) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.InvokeHarmonyPatch.patchMethod.anonymous@13349.anonymous@13819.p | [327:46](../PlaybackIntentInterceptor.cs#L327) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.InvokeHarmonyPatch.patchParameters | [333:17](../PlaybackIntentInterceptor.cs#L333) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.InvokeHarmonyPatch.args | [334:17](../PlaybackIntentInterceptor.cs#L334) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.InvokeHarmonyPatch.i | [336:22](../PlaybackIntentInterceptor.cs#L336) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.FindLoadedType | [349:9](../PlaybackIntentInterceptor.cs#L349) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.FindLoadedType.fullName | [349:45](../PlaybackIntentInterceptor.cs#L349) |
| iteration-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.FindLoadedType.assembly | [352:13](../PlaybackIntentInterceptor.cs#L352) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.FindLoadedType.assembly.type | [354:23](../PlaybackIntentInterceptor.cs#L354) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizePlaybackInfoPrefix | [371:9](../PlaybackIntentInterceptor.cs#L371) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizePlaybackInfoPrefix.__instance | [371:56](../PlaybackIntentInterceptor.cs#L371) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizePlaybackInfoPrefix.__0 | [371:75](../PlaybackIntentInterceptor.cs#L371) |
| catch-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizePlaybackInfoPrefix.ex | [377:19](../PlaybackIntentInterceptor.cs#L377) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.PreferNativeYouTubeAudio | [383:9](../PlaybackIntentInterceptor.cs#L383) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.PreferNativeYouTubeAudio.requestDto | [383:54](../PlaybackIntentInterceptor.cs#L383) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PreferNativeYouTubeAudio.property | [385:17](../PlaybackIntentInterceptor.cs#L385) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PreferNativeYouTubeAudio.profile | [386:98](../PlaybackIntentInterceptor.cs#L386) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PreferNativeYouTubeAudio.changed | [388:35](../PlaybackIntentInterceptor.cs#L388) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PreferNativeYouTubeAudio.clone | [389:17](../PlaybackIntentInterceptor.cs#L389) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PreferNativeYouTubeAudio.i | [390:22](../PlaybackIntentInterceptor.cs#L390) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PreferNativeYouTubeAudio.candidate | [392:21](../PlaybackIntentInterceptor.cs#L392) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PreferNativeYouTubeAudio.codecs | [397:21](../PlaybackIntentInterceptor.cs#L397) |
| anonymous-function | Emby.YouTubePlugin.PlaybackIntentInterceptor.PreferNativeYouTubeAudio.codecs.anonymous@16904 | [397:69](../PlaybackIntentInterceptor.cs#L397) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.PreferNativeYouTubeAudio.codecs.anonymous@16904.c | [397:69](../PlaybackIntentInterceptor.cs#L397) |
| anonymous-function | Emby.YouTubePlugin.PlaybackIntentInterceptor.PreferNativeYouTubeAudio.anonymous@16961 | [398:32](../PlaybackIntentInterceptor.cs#L398) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.PreferNativeYouTubeAudio.anonymous@16961.c | [398:32](../PlaybackIntentInterceptor.cs#L398) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PreferNativeYouTubeAudio.index | [399:21](../PlaybackIntentInterceptor.cs#L399) |
| anonymous-function | Emby.YouTubePlugin.PlaybackIntentInterceptor.PreferNativeYouTubeAudio.index.anonymous@17044 | [399:53](../PlaybackIntentInterceptor.cs#L399) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.PreferNativeYouTubeAudio.index.anonymous@17044.c | [399:53](../PlaybackIntentInterceptor.cs#L399) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PreferNativeYouTubeAudio.copy | [402:21](../PlaybackIntentInterceptor.cs#L402) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PreferNativeYouTubeAudio.local | [410:17](../PlaybackIntentInterceptor.cs#L410) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest | [415:9](../PlaybackIntentInterceptor.cs#L415) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.service | [415:64](../PlaybackIntentInterceptor.cs#L415) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.requestDto | [415:80](../PlaybackIntentInterceptor.cs#L415) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.directItemId | [424:21](../PlaybackIntentInterceptor.cs#L424) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.directQuery | [430:25](../PlaybackIntentInterceptor.cs#L430) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.maximumStreamingBitrate | [439:25](../PlaybackIntentInterceptor.cs#L439) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.subtitle | [449:25](../PlaybackIntentInterceptor.cs#L449) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.querySubtitle | [450:25](../PlaybackIntentInterceptor.cs#L450) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.currentSession | [451:25](../PlaybackIntentInterceptor.cs#L451) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.serviceRequest | [464:17](../PlaybackIntentInterceptor.cs#L464) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.itemId | [465:17](../PlaybackIntentInterceptor.cs#L465) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.videoId | [469:17](../PlaybackIntentInterceptor.cs#L469) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.previousAudioDtoValue | [487:17](../PlaybackIntentInterceptor.cs#L487) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.previousAudioQueryValue | [488:17](../PlaybackIntentInterceptor.cs#L488) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.audioDtoChanged | [489:17](../PlaybackIntentInterceptor.cs#L489) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.audioQueryChanged | [490:17](../PlaybackIntentInterceptor.cs#L490) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.currentDtoValue | [494:17](../PlaybackIntentInterceptor.cs#L494) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.currentQueryValue | [495:17](../PlaybackIntentInterceptor.cs#L495) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.dtoChanged | [496:17](../PlaybackIntentInterceptor.cs#L496) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.queryChanged | [497:17](../PlaybackIntentInterceptor.cs#L497) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SanitizeYouTubePlaybackInfoRequest.isNativeAndroidTablet | [499:17](../PlaybackIntentInterceptor.cs#L499) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoSubtitleSuppressed | [526:9](../PlaybackIntentInterceptor.cs#L526) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoSubtitleSuppressed.request | [527:13](../PlaybackIntentInterceptor.cs#L527) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoSubtitleSuppressed.itemId | [528:13](../PlaybackIntentInterceptor.cs#L528) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoSubtitleSuppressed.videoId | [529:13](../PlaybackIntentInterceptor.cs#L529) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoSubtitleSuppressed.previousDtoValue | [530:13](../PlaybackIntentInterceptor.cs#L530) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoSubtitleSuppressed.previousQueryValue | [531:13](../PlaybackIntentInterceptor.cs#L531) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoSubtitleSuppressed.dtoChanged | [532:13](../PlaybackIntentInterceptor.cs#L532) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoSubtitleSuppressed.queryChanged | [533:13](../PlaybackIntentInterceptor.cs#L533) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoSubtitleSuppressed.previousAudioDtoValue | [534:13](../PlaybackIntentInterceptor.cs#L534) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoSubtitleSuppressed.previousAudioQueryValue | [535:13](../PlaybackIntentInterceptor.cs#L535) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoSubtitleSuppressed.audioDtoChanged | [536:13](../PlaybackIntentInterceptor.cs#L536) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoSubtitleSuppressed.audioQueryChanged | [537:13](../PlaybackIntentInterceptor.cs#L537) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoSubtitleSuppressed.isNativeAndroidTablet | [538:13](../PlaybackIntentInterceptor.cs#L538) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoSubtitleSuppressed.client | [544:17](../PlaybackIntentInterceptor.cs#L544) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoSubtitleSuppressed.deviceName | [545:17](../PlaybackIntentInterceptor.cs#L545) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoSubtitleSuppressed.userAgent | [546:17](../PlaybackIntentInterceptor.cs#L546) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoSubtitleSuppressed.previousDtoText | [547:17](../PlaybackIntentInterceptor.cs#L547) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoSubtitleSuppressed.previousAudioDtoText | [550:17](../PlaybackIntentInterceptor.cs#L550) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoSubtitleSuppressed.clientKind | [553:17](../PlaybackIntentInterceptor.cs#L553) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveRequestUserId | [558:9](../PlaybackIntentInterceptor.cs#L558) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveRequestUserId.request | [558:52](../PlaybackIntentInterceptor.cs#L558) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveRequestUserId.requestDto | [558:71](../PlaybackIntentInterceptor.cs#L558) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveRequestUserId.userId | [560:17](../PlaybackIntentInterceptor.cs#L560) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveRequestUserId.deviceId | [575:17](../PlaybackIntentInterceptor.cs#L575) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveRequestUserId.sessionManager | [579:17](../PlaybackIntentInterceptor.cs#L579) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveUniqueDeviceUserId | [583:9](../PlaybackIntentInterceptor.cs#L583) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveUniqueDeviceUserId.sessions | [583:57](../PlaybackIntentInterceptor.cs#L583) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveUniqueDeviceUserId.deviceId | [583:93](../PlaybackIntentInterceptor.cs#L583) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveUniqueDeviceUserId.userId | [588:21](../PlaybackIntentInterceptor.cs#L588) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveUniqueDeviceUserId.userInternalId | [589:18](../PlaybackIntentInterceptor.cs#L589) |
| iteration-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveUniqueDeviceUserId.session | [590:13](../PlaybackIntentInterceptor.cs#L590) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveUniqueDeviceUserId.session.candidateUserId | [598:21](../PlaybackIntentInterceptor.cs#L598) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfoPostfix | [614:9](../PlaybackIntentInterceptor.cs#L614) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfoPostfix.__instance | [614:56](../PlaybackIntentInterceptor.cs#L614) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfoPostfix.__0 | [614:75](../PlaybackIntentInterceptor.cs#L614) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfoPostfix.__result | [614:87](../PlaybackIntentInterceptor.cs#L614) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfoPostfix.itemId | [620:25](../PlaybackIntentInterceptor.cs#L620) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfoPostfix.videoId | [621:25](../PlaybackIntentInterceptor.cs#L621) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfoPostfix.context | [626:21](../PlaybackIntentInterceptor.cs#L626) |
| catch-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfoPostfix.ex | [630:19](../PlaybackIntentInterceptor.cs#L630) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.RequireDirectPlaybackInfoAsync | [636:9](../PlaybackIntentInterceptor.cs#L636) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.RequireDirectPlaybackInfoAsync.responseTask | [636:74](../PlaybackIntentInterceptor.cs#L636) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.RequireDirectPlaybackInfoAsync.videoId | [636:101](../PlaybackIntentInterceptor.cs#L636) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.RequireDirectPlaybackInfoAsync.itemId | [636:117](../PlaybackIntentInterceptor.cs#L636) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.RequireDirectPlaybackInfoAsync.response | [638:17](../PlaybackIntentInterceptor.cs#L638) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.RequireDirectPlaybackInfoAsync.verified | [639:17](../PlaybackIntentInterceptor.cs#L639) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfo | [645:9](../PlaybackIntentInterceptor.cs#L645) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfo.service | [645:70](../PlaybackIntentInterceptor.cs#L645) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfo.requestDto | [645:86](../PlaybackIntentInterceptor.cs#L645) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfo.serviceRequest | [651:17](../PlaybackIntentInterceptor.cs#L651) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfo.itemId | [652:17](../PlaybackIntentInterceptor.cs#L652) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfo.userId | [653:17](../PlaybackIntentInterceptor.cs#L653) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfo.isPlayback | [657:17](../PlaybackIntentInterceptor.cs#L657) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfo.startTimeTicks | [661:17](../PlaybackIntentInterceptor.cs#L661) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfo.hasStartTimeTicks | [663:17](../PlaybackIntentInterceptor.cs#L663) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfo.effectiveStartTimeTicks | [664:17](../PlaybackIntentInterceptor.cs#L664) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfo.deviceId | [671:21](../PlaybackIntentInterceptor.cs#L671) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfo.intent | [672:21](../PlaybackIntentInterceptor.cs#L672) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfo.client | [685:17](../PlaybackIntentInterceptor.cs#L685) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfo.deviceName | [686:17](../PlaybackIntentInterceptor.cs#L686) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfo.userAgent | [687:17](../PlaybackIntentInterceptor.cs#L687) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfo.ipAddress | [688:17](../PlaybackIntentInterceptor.cs#L688) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CapturePlaybackInfo.maxBitrate | [692:17](../PlaybackIntentInterceptor.cs#L692) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoCapture | [708:9](../PlaybackIntentInterceptor.cs#L708) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoCapture.request | [708:52](../PlaybackIntentInterceptor.cs#L708) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoCapture.itemId | [708:71](../PlaybackIntentInterceptor.cs#L708) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoCapture.startTimeTicks | [708:86](../PlaybackIntentInterceptor.cs#L708) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoCapture.isLikelyIos | [713:17](../PlaybackIntentInterceptor.cs#L713) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoCapture.isLikelyNativeAndroid | [714:17](../PlaybackIntentInterceptor.cs#L714) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoCapture.isLikelyNativeTheater | [715:17](../PlaybackIntentInterceptor.cs#L715) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoCapture.client | [720:17](../PlaybackIntentInterceptor.cs#L720) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoCapture.deviceName | [721:17](../PlaybackIntentInterceptor.cs#L721) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoCapture.maxBitrate | [722:17](../PlaybackIntentInterceptor.cs#L722) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoCapture.clientKind | [723:17](../PlaybackIntentInterceptor.cs#L723) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubePlaybackInfoAsync | [732:9](../PlaybackIntentInterceptor.cs#L732) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubePlaybackInfoAsync.responseTask | [732:73](../PlaybackIntentInterceptor.cs#L732) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubePlaybackInfoAsync.context | [732:100](../PlaybackIntentInterceptor.cs#L732) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubePlaybackInfoAsync.response | [734:17](../PlaybackIntentInterceptor.cs#L734) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubePlaybackInfoAsync.videoId | [737:21](../PlaybackIntentInterceptor.cs#L737) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubePlaybackInfoAsync.youTubeVideoId | [746:21](../PlaybackIntentInterceptor.cs#L746) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubePlaybackInfoAsync.normalized | [747:21](../PlaybackIntentInterceptor.cs#L747) |
| catch-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubePlaybackInfoAsync.ex | [764:19](../PlaybackIntentInterceptor.cs#L764) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoNormalization | [772:9](../PlaybackIntentInterceptor.cs#L772) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoNormalization.context | [772:58](../PlaybackIntentInterceptor.cs#L772) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoNormalization.result | [772:92](../PlaybackIntentInterceptor.cs#L772) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoNormalization.clientKind | [778:17](../PlaybackIntentInterceptor.cs#L778) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogPlaybackInfoNormalization.nativePart | [785:17](../PlaybackIntentInterceptor.cs#L785) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.StampYouTubeStartTime | [792:9](../PlaybackIntentInterceptor.cs#L792) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.StampYouTubeStartTime.response | [793:13](../PlaybackIntentInterceptor.cs#L793) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.StampYouTubeStartTime.startTimeTicks | [794:13](../PlaybackIntentInterceptor.cs#L794) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.StampYouTubeStartTime.includePlayerHints | [795:13](../PlaybackIntentInterceptor.cs#L795) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.StampYouTubeStartTime.mediaSources | [797:17](../PlaybackIntentInterceptor.cs#L797) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.StampYouTubeStartTime.changed | [803:17](../PlaybackIntentInterceptor.cs#L803) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.StampYouTubeStartTime.seconds | [804:17](../PlaybackIntentInterceptor.cs#L804) |
| iteration-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.StampYouTubeStartTime.mediaSource | [805:13](../PlaybackIntentInterceptor.cs#L805) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.StampYouTubeStartTime.mediaSource.pathProp | [810:21](../PlaybackIntentInterceptor.cs#L810) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.StampYouTubeStartTime.mediaSource.path | [815:21](../PlaybackIntentInterceptor.cs#L815) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.StampYouTubeStartTime.mediaSource.stampedPath | [816:21](../PlaybackIntentInterceptor.cs#L816) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.ShouldSuppressYouTubeStartStamp | [835:9](../PlaybackIntentInterceptor.cs#L835) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.ShouldSuppressYouTubeStartStamp.context | [835:61](../PlaybackIntentInterceptor.cs#L835) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyNativeAndroidTablet | [838:9](../PlaybackIntentInterceptor.cs#L838) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyNativeAndroidTablet.context | [838:57](../PlaybackIntentInterceptor.cs#L838) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyNativeAndroidTabletRequest | [861:9](../PlaybackIntentInterceptor.cs#L861) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyNativeAndroidTabletRequest.request | [861:64](../PlaybackIntentInterceptor.cs#L861) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyNativeAndroidTabletRequest.client | [866:17](../PlaybackIntentInterceptor.cs#L866) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyNativeAndroidTabletRequest.deviceName | [867:17](../PlaybackIntentInterceptor.cs#L867) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyNativeAndroidTabletRequest.userAgent | [868:17](../PlaybackIntentInterceptor.cs#L868) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyFireTvOrAndroidTv | [886:9](../PlaybackIntentInterceptor.cs#L886) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyFireTvOrAndroidTv.client | [887:13](../PlaybackIntentInterceptor.cs#L887) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyFireTvOrAndroidTv.deviceName | [888:13](../PlaybackIntentInterceptor.cs#L888) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyFireTvOrAndroidTv.userAgent | [889:13](../PlaybackIntentInterceptor.cs#L889) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.GetContextualYouTubeWatchUrl | [903:9](../PlaybackIntentInterceptor.cs#L903) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.GetContextualYouTubeWatchUrl.videoId | [903:60](../PlaybackIntentInterceptor.cs#L903) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.GetContextualYouTubeWatchUrl.context | [903:76](../PlaybackIntentInterceptor.cs#L903) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogNativeAndroidTabletLegacySource | [911:9](../PlaybackIntentInterceptor.cs#L911) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogNativeAndroidTabletLegacySource.context | [911:64](../PlaybackIntentInterceptor.cs#L911) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogNativeAndroidTabletLegacySource.videoId | [911:98](../PlaybackIntentInterceptor.cs#L911) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogYouTubeStartStampSuppressed | [921:9](../PlaybackIntentInterceptor.cs#L921) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogYouTubeStartStampSuppressed.context | [921:60](../PlaybackIntentInterceptor.cs#L921) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.LogYouTubeStartStampSuppressed.seconds | [927:17](../PlaybackIntentInterceptor.cs#L927) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.NormalizeYouTubeMediaSources | [932:9](../PlaybackIntentInterceptor.cs#L932) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.NormalizeYouTubeMediaSources.response | [933:13](../PlaybackIntentInterceptor.cs#L933) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.NormalizeYouTubeMediaSources.youTubeVideoId | [934:13](../PlaybackIntentInterceptor.cs#L934) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.NormalizeYouTubeMediaSources.context | [935:13](../PlaybackIntentInterceptor.cs#L935) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.NormalizeYouTubeMediaSources.mediaSources | [939:17](../PlaybackIntentInterceptor.cs#L939) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.NormalizeYouTubeMediaSources.changed | [945:17](../PlaybackIntentInterceptor.cs#L945) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.NormalizeYouTubeMediaSources.nativeDirectPlayDisabled | [946:17](../PlaybackIntentInterceptor.cs#L946) |
| iteration-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.NormalizeYouTubeMediaSources.mediaSource | [947:13](../PlaybackIntentInterceptor.cs#L947) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.NormalizeYouTubeMediaSources.mediaSource.pathProp | [952:21](../PlaybackIntentInterceptor.cs#L952) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.NormalizeYouTubeMediaSources.mediaSource.path | [954:21](../PlaybackIntentInterceptor.cs#L954) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.NormalizeYouTubeMediaSources.mediaSource.videoId | [960:21](../PlaybackIntentInterceptor.cs#L960) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.NormalizeYouTubeMediaSources.mediaSource.mediaSourceChanged | [971:21](../PlaybackIntentInterceptor.cs#L971) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.NormalizeYouTubeMediaSources.mediaSource.normalizedPath | [977:25](../PlaybackIntentInterceptor.cs#L977) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryResolveYouTubeVideoId | [1055:9](../PlaybackIntentInterceptor.cs#L1055) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryResolveYouTubeVideoId.itemId | [1055:57](../PlaybackIntentInterceptor.cs#L1055) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryResolveYouTubeVideoId.library | [1062:21](../PlaybackIntentInterceptor.cs#L1062) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryResolveYouTubeVideoId.item | [1066:21](../PlaybackIntentInterceptor.cs#L1066) |
| catch-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryResolveYouTubeVideoId.ex | [1069:19](../PlaybackIntentInterceptor.cs#L1069) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveBaseItem | [1076:9](../PlaybackIntentInterceptor.cs#L1076) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveBaseItem.library | [1076:50](../PlaybackIntentInterceptor.cs#L1076) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveBaseItem.itemId | [1076:75](../PlaybackIntentInterceptor.cs#L1076) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveBaseItem.methods | [1080:17](../PlaybackIntentInterceptor.cs#L1080) |
| anonymous-function | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveBaseItem.methods.anonymous@51956 | [1082:24](../PlaybackIntentInterceptor.cs#L1082) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveBaseItem.methods.anonymous@51956.m | [1082:24](../PlaybackIntentInterceptor.cs#L1082) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveBaseItem.numericId | [1085:99](../PlaybackIntentInterceptor.cs#L1085) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveBaseItem.longMethod | [1087:21](../PlaybackIntentInterceptor.cs#L1087) |
| anonymous-function | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveBaseItem.longMethod.anonymous@52228 | [1087:57](../PlaybackIntentInterceptor.cs#L1087) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveBaseItem.longMethod.anonymous@52228.m | [1087:57](../PlaybackIntentInterceptor.cs#L1087) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveBaseItem.guidId | [1092:47](../PlaybackIntentInterceptor.cs#L1092) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveBaseItem.guidMethod | [1094:21](../PlaybackIntentInterceptor.cs#L1094) |
| anonymous-function | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveBaseItem.guidMethod.anonymous@52561 | [1094:57](../PlaybackIntentInterceptor.cs#L1094) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveBaseItem.guidMethod.anonymous@52561.m | [1094:57](../PlaybackIntentInterceptor.cs#L1094) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveBaseItem.stringMethod | [1099:17](../PlaybackIntentInterceptor.cs#L1099) |
| anonymous-function | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveBaseItem.stringMethod.anonymous@52820 | [1099:55](../PlaybackIntentInterceptor.cs#L1099) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.ResolveBaseItem.stringMethod.anonymous@52820.m | [1099:55](../PlaybackIntentInterceptor.cs#L1099) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.ExtractYouTubeVideoId | [1106:9](../PlaybackIntentInterceptor.cs#L1106) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.ExtractYouTubeVideoId.value | [1106:54](../PlaybackIntentInterceptor.cs#L1106) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.ExtractYouTubeVideoId.match | [1111:17](../PlaybackIntentInterceptor.cs#L1111) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubeUrl | [1115:9](../PlaybackIntentInterceptor.cs#L1115) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubeUrl.path | [1116:13](../PlaybackIntentInterceptor.cs#L1116) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubeUrl.startSeconds | [1117:13](../PlaybackIntentInterceptor.cs#L1117) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubeUrl.includePlayerHints | [1118:13](../PlaybackIntentInterceptor.cs#L1118) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubeUrl.uri | [1121:67](../PlaybackIntentInterceptor.cs#L1121) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubeUrl.queryParts | [1127:17](../PlaybackIntentInterceptor.cs#L1127) |
| anonymous-function | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubeUrl.queryParts.anonymous@53868 | [1129:24](../PlaybackIntentInterceptor.cs#L1129) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubeUrl.queryParts.anonymous@53868.p | [1129:24](../PlaybackIntentInterceptor.cs#L1129) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubeUrl.queryParts.anonymous@53868.keyEnd | [1131:25](../PlaybackIntentInterceptor.cs#L1131) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubeUrl.queryParts.anonymous@53868.key | [1132:25](../PlaybackIntentInterceptor.cs#L1132) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubeUrl.secondsText | [1157:21](../PlaybackIntentInterceptor.cs#L1157) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.PatchYouTubeUrl.builder | [1162:17](../PlaybackIntentInterceptor.cs#L1162) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetStringPropertyToNull | [1170:9](../PlaybackIntentInterceptor.cs#L1170) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetStringPropertyToNull.source | [1170:53](../PlaybackIntentInterceptor.cs#L1170) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetStringPropertyToNull.name | [1170:68](../PlaybackIntentInterceptor.cs#L1170) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetStringPropertyToNull.prop | [1172:17](../PlaybackIntentInterceptor.cs#L1172) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsYouTubeLiveItem | [1183:9](../PlaybackIntentInterceptor.cs#L1183) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsYouTubeLiveItem.itemId | [1183:47](../PlaybackIntentInterceptor.cs#L1183) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsYouTubeLiveItem.library | [1190:21](../PlaybackIntentInterceptor.cs#L1190) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsYouTubeLiveItem.item | [1191:21](../PlaybackIntentInterceptor.cs#L1191) |
| catch-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsYouTubeLiveItem.ex | [1197:19](../PlaybackIntentInterceptor.cs#L1197) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetPropertyToNull | [1204:9](../PlaybackIntentInterceptor.cs#L1204) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetPropertyToNull.source | [1204:47](../PlaybackIntentInterceptor.cs#L1204) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetPropertyToNull.name | [1204:62](../PlaybackIntentInterceptor.cs#L1204) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetPropertyToNull.prop | [1206:17](../PlaybackIntentInterceptor.cs#L1206) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.ClearListProperty | [1223:9](../PlaybackIntentInterceptor.cs#L1223) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.ClearListProperty.source | [1223:47](../PlaybackIntentInterceptor.cs#L1223) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.ClearListProperty.name | [1223:62](../PlaybackIntentInterceptor.cs#L1223) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.ClearListProperty.prop | [1225:17](../PlaybackIntentInterceptor.cs#L1225) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.ClearListProperty.list | [1226:53](../PlaybackIntentInterceptor.cs#L1226) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsYouTubeHost | [1240:9](../PlaybackIntentInterceptor.cs#L1240) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsYouTubeHost.host | [1240:43](../PlaybackIntentInterceptor.cs#L1240) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNullableLongProperty | [1245:9](../PlaybackIntentInterceptor.cs#L1245) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNullableLongProperty.source | [1245:53](../PlaybackIntentInterceptor.cs#L1245) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNullableLongProperty.name | [1245:68](../PlaybackIntentInterceptor.cs#L1245) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNullableLongProperty.value | [1245:81](../PlaybackIntentInterceptor.cs#L1245) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNullableLongProperty.prop | [1247:17](../PlaybackIntentInterceptor.cs#L1247) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNullableIntProperty | [1255:9](../PlaybackIntentInterceptor.cs#L1255) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNullableIntProperty.source | [1255:52](../PlaybackIntentInterceptor.cs#L1255) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNullableIntProperty.name | [1255:67](../PlaybackIntentInterceptor.cs#L1255) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNullableIntProperty.value | [1255:80](../PlaybackIntentInterceptor.cs#L1255) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNullableIntProperty.prop | [1257:17](../PlaybackIntentInterceptor.cs#L1257) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNullableIntProperty.current | [1267:17](../PlaybackIntentInterceptor.cs#L1267) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNullableIntProperty.currentInt | [1268:32](../PlaybackIntentInterceptor.cs#L1268) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetBoolProperty | [1278:9](../PlaybackIntentInterceptor.cs#L1278) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetBoolProperty.source | [1278:45](../PlaybackIntentInterceptor.cs#L1278) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetBoolProperty.name | [1278:60](../PlaybackIntentInterceptor.cs#L1278) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetBoolProperty.value | [1278:73](../PlaybackIntentInterceptor.cs#L1278) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetBoolProperty.prop | [1280:17](../PlaybackIntentInterceptor.cs#L1280) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetBoolProperty.current | [1287:17](../PlaybackIntentInterceptor.cs#L1287) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetBoolProperty.currentBool | [1288:33](../PlaybackIntentInterceptor.cs#L1288) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNumberProperty | [1295:9](../PlaybackIntentInterceptor.cs#L1295) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNumberProperty.source | [1295:47](../PlaybackIntentInterceptor.cs#L1295) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNumberProperty.name | [1295:62](../PlaybackIntentInterceptor.cs#L1295) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNumberProperty.value | [1295:75](../PlaybackIntentInterceptor.cs#L1295) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNumberProperty.prop | [1297:17](../PlaybackIntentInterceptor.cs#L1297) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNumberProperty.current | [1301:17](../PlaybackIntentInterceptor.cs#L1301) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNumberProperty.currentInt | [1304:36](../PlaybackIntentInterceptor.cs#L1304) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.SetNumberProperty.currentLong | [1313:37](../PlaybackIntentInterceptor.cs#L1313) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.GetServiceRequest | [1323:9](../PlaybackIntentInterceptor.cs#L1323) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.GetServiceRequest.service | [1323:52](../PlaybackIntentInterceptor.cs#L1323) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.GetDeviceId | [1330:9](../PlaybackIntentInterceptor.cs#L1330) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.GetDeviceId.request | [1330:44](../PlaybackIntentInterceptor.cs#L1330) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.GetRequestValue | [1345:9](../PlaybackIntentInterceptor.cs#L1345) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.GetRequestValue.request | [1345:48](../PlaybackIntentInterceptor.cs#L1345) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.GetRequestValue.name | [1345:67](../PlaybackIntentInterceptor.cs#L1345) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.TrySetRequestCollectionValue | [1353:9](../PlaybackIntentInterceptor.cs#L1353) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TrySetRequestCollectionValue.collection | [1353:58](../PlaybackIntentInterceptor.cs#L1353) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TrySetRequestCollectionValue.name | [1353:78](../PlaybackIntentInterceptor.cs#L1353) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TrySetRequestCollectionValue.value | [1353:91](../PlaybackIntentInterceptor.cs#L1353) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TrySetRequestCollectionValue.collectionType | [1360:21](../PlaybackIntentInterceptor.cs#L1360) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TrySetRequestCollectionValue.indexer | [1361:21](../PlaybackIntentInterceptor.cs#L1361) |
| anonymous-function | Emby.YouTubePlugin.PlaybackIntentInterceptor.TrySetRequestCollectionValue.indexer.anonymous@62403 | [1363:37](../PlaybackIntentInterceptor.cs#L1363) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TrySetRequestCollectionValue.indexer.anonymous@62403.p | [1363:37](../PlaybackIntentInterceptor.cs#L1363) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TrySetRequestCollectionValue.indexer.anonymous@62403.indexParameters | [1365:29](../PlaybackIntentInterceptor.cs#L1365) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TrySetRequestCollectionValue.current | [1372:25](../PlaybackIntentInterceptor.cs#L1372) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TrySetRequestCollectionValue.setMethod | [1383:21](../PlaybackIntentInterceptor.cs#L1383) |
| anonymous-function | Emby.YouTubePlugin.PlaybackIntentInterceptor.TrySetRequestCollectionValue.setMethod.anonymous@63326 | [1385:37](../PlaybackIntentInterceptor.cs#L1385) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TrySetRequestCollectionValue.setMethod.anonymous@63326.m | [1385:37](../PlaybackIntentInterceptor.cs#L1385) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TrySetRequestCollectionValue.setMethod.anonymous@63326.parameters | [1390:29](../PlaybackIntentInterceptor.cs#L1390) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeCompatibleKey | [1410:9](../PlaybackIntentInterceptor.cs#L1410) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeCompatibleKey.key | [1411:13](../PlaybackIntentInterceptor.cs#L1411) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeCompatibleKey.deviceId | [1412:13](../PlaybackIntentInterceptor.cs#L1412) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeCompatibleKey.intent | [1413:13](../PlaybackIntentInterceptor.cs#L1413) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryConsumeCompatibleKey.candidate | [1415:53](../PlaybackIntentInterceptor.cs#L1415) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryRemoveRequestCollectionValue | [1444:9](../PlaybackIntentInterceptor.cs#L1444) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryRemoveRequestCollectionValue.collection | [1444:61](../PlaybackIntentInterceptor.cs#L1444) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryRemoveRequestCollectionValue.name | [1444:81](../PlaybackIntentInterceptor.cs#L1444) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryRemoveRequestCollectionValue.dictionary | [1451:47](../PlaybackIntentInterceptor.cs#L1451) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryRemoveRequestCollectionValue.collectionType | [1460:21](../PlaybackIntentInterceptor.cs#L1460) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryRemoveRequestCollectionValue.indexer | [1461:21](../PlaybackIntentInterceptor.cs#L1461) |
| anonymous-function | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryRemoveRequestCollectionValue.indexer.anonymous@66089 | [1463:37](../PlaybackIntentInterceptor.cs#L1463) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryRemoveRequestCollectionValue.indexer.anonymous@66089.p | [1463:37](../PlaybackIntentInterceptor.cs#L1463) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryRemoveRequestCollectionValue.indexer.anonymous@66089.indexParameters | [1465:29](../PlaybackIntentInterceptor.cs#L1465) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryRemoveRequestCollectionValue.current | [1470:21](../PlaybackIntentInterceptor.cs#L1470) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryRemoveRequestCollectionValue.removeMethod | [1474:21](../PlaybackIntentInterceptor.cs#L1474) |
| anonymous-function | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryRemoveRequestCollectionValue.removeMethod.anonymous@66718 | [1476:37](../PlaybackIntentInterceptor.cs#L1476) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryRemoveRequestCollectionValue.removeMethod.anonymous@66718.m | [1476:37](../PlaybackIntentInterceptor.cs#L1476) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryRemoveRequestCollectionValue.removeMethod.anonymous@66718.parameters | [1481:29](../PlaybackIntentInterceptor.cs#L1481) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyIosClient | [1499:9](../PlaybackIntentInterceptor.cs#L1499) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyIosClient.request | [1499:47](../PlaybackIntentInterceptor.cs#L1499) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyIosClient.deviceName | [1501:17](../PlaybackIntentInterceptor.cs#L1501) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyIosClient.userAgent | [1502:17](../PlaybackIntentInterceptor.cs#L1502) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyNativeAndroidClient | [1511:9](../PlaybackIntentInterceptor.cs#L1511) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyNativeAndroidClient.request | [1511:57](../PlaybackIntentInterceptor.cs#L1511) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyNativeAndroidClient.client | [1513:17](../PlaybackIntentInterceptor.cs#L1513) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyNativeTheaterClient | [1517:9](../PlaybackIntentInterceptor.cs#L1517) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyNativeTheaterClient.request | [1517:57](../PlaybackIntentInterceptor.cs#L1517) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyNativeTheaterClient.client | [1519:17](../PlaybackIntentInterceptor.cs#L1519) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyNativeTheaterClient.deviceName | [1520:17](../PlaybackIntentInterceptor.cs#L1520) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyNativeTheaterClient.userAgent | [1521:17](../PlaybackIntentInterceptor.cs#L1521) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyBrowserEngineClient | [1538:9](../PlaybackIntentInterceptor.cs#L1538) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.IsLikelyBrowserEngineClient.userAgent | [1538:57](../PlaybackIntentInterceptor.cs#L1538) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.ContainsIgnoreCase | [1548:9](../PlaybackIntentInterceptor.cs#L1548) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.ContainsIgnoreCase.value | [1548:48](../PlaybackIntentInterceptor.cs#L1548) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.ContainsIgnoreCase.needle | [1548:63](../PlaybackIntentInterceptor.cs#L1548) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.Shorten | [1552:9](../PlaybackIntentInterceptor.cs#L1552) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.Shorten.value | [1552:39](../PlaybackIntentInterceptor.cs#L1552) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.Shorten.maxLength | [1552:53](../PlaybackIntentInterceptor.cs#L1552) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.GetStringProperty | [1560:9](../PlaybackIntentInterceptor.cs#L1560) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.GetStringProperty.source | [1560:50](../PlaybackIntentInterceptor.cs#L1560) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.GetStringProperty.name | [1560:65](../PlaybackIntentInterceptor.cs#L1560) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadLongProperty | [1567:9](../PlaybackIntentInterceptor.cs#L1567) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadLongProperty.source | [1567:50](../PlaybackIntentInterceptor.cs#L1567) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadLongProperty.name | [1567:65](../PlaybackIntentInterceptor.cs#L1567) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadLongProperty.value | [1569:17](../PlaybackIntentInterceptor.cs#L1569) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadIntProperty | [1575:9](../PlaybackIntentInterceptor.cs#L1575) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadIntProperty.source | [1575:48](../PlaybackIntentInterceptor.cs#L1575) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadIntProperty.name | [1575:63](../PlaybackIntentInterceptor.cs#L1575) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadIntProperty.value | [1577:17](../PlaybackIntentInterceptor.cs#L1577) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadInt | [1583:9](../PlaybackIntentInterceptor.cs#L1583) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadInt.value | [1583:40](../PlaybackIntentInterceptor.cs#L1583) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadInt.intValue | [1587:21](../PlaybackIntentInterceptor.cs#L1587) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadInt.longValue | [1588:22](../PlaybackIntentInterceptor.cs#L1588) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadInt.text | [1589:24](../PlaybackIntentInterceptor.cs#L1589) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadInt.parsed | [1589:113](../PlaybackIntentInterceptor.cs#L1589) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadLong | [1594:9](../PlaybackIntentInterceptor.cs#L1594) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadLong.value | [1594:42](../PlaybackIntentInterceptor.cs#L1594) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadLong.longValue | [1598:22](../PlaybackIntentInterceptor.cs#L1598) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadLong.intValue | [1599:21](../PlaybackIntentInterceptor.cs#L1599) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadLong.text | [1600:24](../PlaybackIntentInterceptor.cs#L1600) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadLong.parsed | [1600:114](../PlaybackIntentInterceptor.cs#L1600) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadBoolProperty | [1605:9](../PlaybackIntentInterceptor.cs#L1605) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadBoolProperty.source | [1605:50](../PlaybackIntentInterceptor.cs#L1605) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadBoolProperty.name | [1605:65](../PlaybackIntentInterceptor.cs#L1605) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadBoolProperty.value | [1607:17](../PlaybackIntentInterceptor.cs#L1607) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadBool | [1613:9](../PlaybackIntentInterceptor.cs#L1613) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadBool.value | [1613:42](../PlaybackIntentInterceptor.cs#L1613) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadBool.boolValue | [1617:22](../PlaybackIntentInterceptor.cs#L1617) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadBool.text | [1618:24](../PlaybackIntentInterceptor.cs#L1618) |
| pattern-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.TryReadBool.parsed | [1618:62](../PlaybackIntentInterceptor.cs#L1618) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.CleanupExpired | [1623:9](../PlaybackIntentInterceptor.cs#L1623) |
| local-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CleanupExpired.now | [1627:17](../PlaybackIntentInterceptor.cs#L1627) |
| iteration-variable | Emby.YouTubePlugin.PlaybackIntentInterceptor.CleanupExpired.kvp | [1632:13](../PlaybackIntentInterceptor.cs#L1632) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.MakeKey | [1639:9](../PlaybackIntentInterceptor.cs#L1639) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.MakeKey.userId | [1639:39](../PlaybackIntentInterceptor.cs#L1639) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.MakeKey.itemId | [1639:54](../PlaybackIntentInterceptor.cs#L1639) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.MakeKey.deviceId | [1639:69](../PlaybackIntentInterceptor.cs#L1639) |
| method | Emby.YouTubePlugin.PlaybackIntentInterceptor.Normalize | [1644:9](../PlaybackIntentInterceptor.cs#L1644) |
| parameter | Emby.YouTubePlugin.PlaybackIntentInterceptor.Normalize.value | [1644:41](../PlaybackIntentInterceptor.cs#L1644) |

</details>

<a id="file-5a2f4e90167632d0"></a>
## [Plugin.cs](../Plugin.cs)

Plugin Compile source (SDK default); 11710 bytes; 266 lines.

<details>
<summary>All 78 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [15:1](../Plugin.cs#L15) |
| type | Emby.YouTubePlugin.Plugin | [17:5](../Plugin.cs#L17) |
| field | Emby.YouTubePlugin.Plugin.PluginVersion | [19:40](../Plugin.cs#L19) |
| property | Emby.YouTubePlugin.Plugin.Name | [22:9](../Plugin.cs#L22) |
| property | Emby.YouTubePlugin.Plugin.Description | [23:9](../Plugin.cs#L23) |
| property | Emby.YouTubePlugin.Plugin.Id | [24:9](../Plugin.cs#L24) |
| property | Emby.YouTubePlugin.Plugin.Instance | [26:9](../Plugin.cs#L26) |
| accessor | Emby.YouTubePlugin.Plugin.Instance.get | [26:42](../Plugin.cs#L26) |
| accessor | Emby.YouTubePlugin.Plugin.Instance.set | [26:47](../Plugin.cs#L26) |
| property | Emby.YouTubePlugin.Plugin.LibraryDbPath | [27:9](../Plugin.cs#L27) |
| accessor | Emby.YouTubePlugin.Plugin.LibraryDbPath.get | [27:47](../Plugin.cs#L27) |
| accessor | Emby.YouTubePlugin.Plugin.LibraryDbPath.set | [27:52](../Plugin.cs#L27) |
| property | Emby.YouTubePlugin.Plugin.CachePath | [28:9](../Plugin.cs#L28) |
| accessor | Emby.YouTubePlugin.Plugin.CachePath.get | [28:43](../Plugin.cs#L28) |
| accessor | Emby.YouTubePlugin.Plugin.CachePath.set | [28:48](../Plugin.cs#L28) |
| property | Emby.YouTubePlugin.Plugin.DataPath | [29:9](../Plugin.cs#L29) |
| accessor | Emby.YouTubePlugin.Plugin.DataPath.get | [29:42](../Plugin.cs#L29) |
| accessor | Emby.YouTubePlugin.Plugin.DataPath.set | [29:47](../Plugin.cs#L29) |
| property | Emby.YouTubePlugin.Plugin.SystemConfigurationFilePath | [30:9](../Plugin.cs#L30) |
| accessor | Emby.YouTubePlugin.Plugin.SystemConfigurationFilePath.get | [30:61](../Plugin.cs#L30) |
| accessor | Emby.YouTubePlugin.Plugin.SystemConfigurationFilePath.set | [30:66](../Plugin.cs#L30) |
| property | Emby.YouTubePlugin.Plugin.AppHost | [31:9](../Plugin.cs#L31) |
| accessor | Emby.YouTubePlugin.Plugin.AppHost.get | [31:51](../Plugin.cs#L31) |
| accessor | Emby.YouTubePlugin.Plugin.AppHost.set | [31:56](../Plugin.cs#L31) |
| property | Emby.YouTubePlugin.Plugin.PluginLogger | [32:9](../Plugin.cs#L32) |
| accessor | Emby.YouTubePlugin.Plugin.PluginLogger.get | [32:49](../Plugin.cs#L32) |
| accessor | Emby.YouTubePlugin.Plugin.PluginLogger.set | [32:54](../Plugin.cs#L32) |
| constructor | Emby.YouTubePlugin.Plugin.Plugin | [34:9](../Plugin.cs#L34) |
| parameter | Emby.YouTubePlugin.Plugin.Plugin.applicationPaths | [35:13](../Plugin.cs#L35) |
| parameter | Emby.YouTubePlugin.Plugin.Plugin.xmlSerializer | [36:13](../Plugin.cs#L36) |
| method | Emby.YouTubePlugin.Plugin.ResolveService | [45:9](../Plugin.cs#L45) |
| type-parameter | Emby.YouTubePlugin.Plugin.ResolveService.T | [45:43](../Plugin.cs#L45) |
| parameter | Emby.YouTubePlugin.Plugin.ResolveService.host | [45:46](../Plugin.cs#L45) |
| local-variable | Emby.YouTubePlugin.Plugin.ResolveService.resolve | [51:21](../Plugin.cs#L51) |
| anonymous-function | Emby.YouTubePlugin.Plugin.ResolveService.resolve.anonymous@2231 | [53:37](../Plugin.cs#L53) |
| parameter | Emby.YouTubePlugin.Plugin.ResolveService.resolve.anonymous@2231.m | [53:37](../Plugin.cs#L53) |
| catch-variable | Emby.YouTubePlugin.Plugin.ResolveService.ex | [56:19](../Plugin.cs#L56) |
| method | Emby.YouTubePlugin.Plugin.InitializeApplicationHost | [63:9](../Plugin.cs#L63) |
| parameter | Emby.YouTubePlugin.Plugin.InitializeApplicationHost.applicationHost | [63:56](../Plugin.cs#L63) |
| method | Emby.YouTubePlugin.Plugin.TryInitLogger | [69:9](../Plugin.cs#L69) |
| parameter | Emby.YouTubePlugin.Plugin.TryInitLogger.applicationHost | [69:43](../Plugin.cs#L69) |
| local-variable | Emby.YouTubePlugin.Plugin.TryInitLogger.factory | [74:17](../Plugin.cs#L74) |
| property | Emby.YouTubePlugin.Plugin.Options | [80:9](../Plugin.cs#L80) |
| property | Emby.YouTubePlugin.Plugin.ThumbImageFormat | [82:9](../Plugin.cs#L82) |
| method | Emby.YouTubePlugin.Plugin.GetPages | [84:9](../Plugin.cs#L84) |
| method | Emby.YouTubePlugin.Plugin.SaveConfiguration | [104:9](../Plugin.cs#L104) |
| local-variable | Emby.YouTubePlugin.Plugin.SaveConfiguration.contentChanged | [115:17](../Plugin.cs#L115) |
| catch-variable | Emby.YouTubePlugin.Plugin.SaveConfiguration.ex | [126:19](../Plugin.cs#L126) |
| catch-variable | Emby.YouTubePlugin.Plugin.SaveConfiguration.ex | [139:19](../Plugin.cs#L139) |
| catch-variable | Emby.YouTubePlugin.Plugin.SaveConfiguration.ex | [142:19](../Plugin.cs#L142) |
| anonymous-function | Emby.YouTubePlugin.Plugin.SaveConfiguration.anonymous@6469 | [147:49](../Plugin.cs#L147) |
| catch-variable | Emby.YouTubePlugin.Plugin.SaveConfiguration.anonymous@6469.ex | [150:23](../Plugin.cs#L150) |
| method | Emby.YouTubePlugin.Plugin.GetPluginInfo | [154:9](../Plugin.cs#L154) |
| local-variable | Emby.YouTubePlugin.Plugin.GetPluginInfo.info | [156:17](../Plugin.cs#L156) |
| local-variable | Emby.YouTubePlugin.Plugin.GetPluginInfo.assemblyDir | [157:17](../Plugin.cs#L157) |
| local-variable | Emby.YouTubePlugin.Plugin.GetPluginInfo.thumbPath | [158:17](../Plugin.cs#L158) |
| local-variable | Emby.YouTubePlugin.Plugin.GetPluginInfo.imageTag | [162:21](../Plugin.cs#L162) |
| local-variable | Emby.YouTubePlugin.Plugin.GetPluginInfo.pngBytes | [168:21](../Plugin.cs#L168) |
| local-variable | Emby.YouTubePlugin.Plugin.GetPluginInfo.imageDataUrl | [169:21](../Plugin.cs#L169) |
| method | Emby.YouTubePlugin.Plugin.GetThumbImage | [176:9](../Plugin.cs#L176) |
| local-variable | Emby.YouTubePlugin.Plugin.GetThumbImage.type | [178:17](../Plugin.cs#L178) |
| local-variable | Emby.YouTubePlugin.Plugin.GetThumbImage.stream | [179:17](../Plugin.cs#L179) |
| method | Emby.YouTubePlugin.Plugin.InitializePaths | [183:9](../Plugin.cs#L183) |
| parameter | Emby.YouTubePlugin.Plugin.InitializePaths.applicationPaths | [183:45](../Plugin.cs#L183) |
| local-variable | Emby.YouTubePlugin.Plugin.InitializePaths.candidate | [189:21](../Plugin.cs#L189) |
| local-variable | Emby.YouTubePlugin.Plugin.InitializePaths.cacheDir | [193:21](../Plugin.cs#L193) |
| catch-variable | Emby.YouTubePlugin.Plugin.InitializePaths.ex | [197:19](../Plugin.cs#L197) |
| method | Emby.YouTubePlugin.Plugin.NormalizeConfiguration | [203:9](../Plugin.cs#L203) |
| parameter | Emby.YouTubePlugin.Plugin.NormalizeConfiguration.config | [203:52](../Plugin.cs#L203) |
| method | Emby.YouTubePlugin.Plugin.NormalizeSort | [235:9](../Plugin.cs#L235) |
| parameter | Emby.YouTubePlugin.Plugin.NormalizeSort.sortBy | [235:45](../Plugin.cs#L235) |
| method | Emby.YouTubePlugin.Plugin.IsRegionCodeShape | [249:9](../Plugin.cs#L249) |
| parameter | Emby.YouTubePlugin.Plugin.IsRegionCodeShape.value | [249:47](../Plugin.cs#L249) |
| method | Emby.YouTubePlugin.Plugin.SetStringPropertyIfExists | [254:9](../Plugin.cs#L254) |
| parameter | Emby.YouTubePlugin.Plugin.SetStringPropertyIfExists.target | [254:55](../Plugin.cs#L254) |
| parameter | Emby.YouTubePlugin.Plugin.SetStringPropertyIfExists.propertyName | [254:70](../Plugin.cs#L254) |
| parameter | Emby.YouTubePlugin.Plugin.SetStringPropertyIfExists.value | [254:91](../Plugin.cs#L254) |
| local-variable | Emby.YouTubePlugin.Plugin.SetStringPropertyIfExists.property | [256:17](../Plugin.cs#L256) |

</details>

<a id="file-224cbfdbaba3a683"></a>
## [PluginConfiguration.cs](../PluginConfiguration.cs)

Plugin Compile source (SDK default); 4106 bytes; 119 lines.

<details>
<summary>All 87 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [4:1](../PluginConfiguration.cs#L4) |
| type | Emby.YouTubePlugin.PluginConfiguration | [7:5](../PluginConfiguration.cs#L7) |
| property | Emby.YouTubePlugin.PluginConfiguration.ApiKey | [9:9](../PluginConfiguration.cs#L9) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.ApiKey.get | [9:32](../PluginConfiguration.cs#L9) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.ApiKey.set | [9:37](../PluginConfiguration.cs#L9) |
| property | Emby.YouTubePlugin.PluginConfiguration.SavedItems | [11:9](../PluginConfiguration.cs#L11) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.SavedItems.get | [11:36](../PluginConfiguration.cs#L11) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.SavedItems.set | [11:41](../PluginConfiguration.cs#L11) |
| property | Emby.YouTubePlugin.PluginConfiguration.WatchLaterPlaylist | [13:9](../PluginConfiguration.cs#L13) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.WatchLaterPlaylist.get | [13:44](../PluginConfiguration.cs#L13) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.WatchLaterPlaylist.set | [13:49](../PluginConfiguration.cs#L13) |
| property | Emby.YouTubePlugin.PluginConfiguration.ShowTrending | [15:9](../PluginConfiguration.cs#L15) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.ShowTrending.get | [15:36](../PluginConfiguration.cs#L15) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.ShowTrending.set | [15:41](../PluginConfiguration.cs#L15) |
| property | Emby.YouTubePlugin.PluginConfiguration.ShowCategories | [17:9](../PluginConfiguration.cs#L17) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.ShowCategories.get | [17:38](../PluginConfiguration.cs#L17) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.ShowCategories.set | [17:43](../PluginConfiguration.cs#L17) |
| property | Emby.YouTubePlugin.PluginConfiguration.ShowRootFoldersAtTopLevel | [19:9](../PluginConfiguration.cs#L19) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.ShowRootFoldersAtTopLevel.get | [19:49](../PluginConfiguration.cs#L19) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.ShowRootFoldersAtTopLevel.set | [19:54](../PluginConfiguration.cs#L19) |
| property | Emby.YouTubePlugin.PluginConfiguration.ShowShorts | [21:9](../PluginConfiguration.cs#L21) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.ShowShorts.get | [21:34](../PluginConfiguration.cs#L21) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.ShowShorts.set | [21:39](../PluginConfiguration.cs#L21) |
| property | Emby.YouTubePlugin.PluginConfiguration.HideShorts | [24:9](../PluginConfiguration.cs#L24) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.HideShorts.get | [24:34](../PluginConfiguration.cs#L24) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.HideShorts.set | [24:39](../PluginConfiguration.cs#L24) |
| property | Emby.YouTubePlugin.PluginConfiguration.ShortsEnabled | [26:9](../PluginConfiguration.cs#L26) |
| property | Emby.YouTubePlugin.PluginConfiguration.TrendingRegion | [29:9](../PluginConfiguration.cs#L29) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.TrendingRegion.get | [29:40](../PluginConfiguration.cs#L29) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.TrendingRegion.set | [29:45](../PluginConfiguration.cs#L29) |
| property | Emby.YouTubePlugin.PluginConfiguration.TrendingCategory | [31:9](../PluginConfiguration.cs#L31) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.TrendingCategory.get | [31:42](../PluginConfiguration.cs#L31) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.TrendingCategory.set | [31:47](../PluginConfiguration.cs#L31) |
| property | Emby.YouTubePlugin.PluginConfiguration.ShowLikeCount | [33:9](../PluginConfiguration.cs#L33) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.ShowLikeCount.get | [33:37](../PluginConfiguration.cs#L33) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.ShowLikeCount.set | [33:42](../PluginConfiguration.cs#L33) |
| property | Emby.YouTubePlugin.PluginConfiguration.ShowCommentCount | [35:9](../PluginConfiguration.cs#L35) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.ShowCommentCount.get | [35:40](../PluginConfiguration.cs#L35) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.ShowCommentCount.set | [35:45](../PluginConfiguration.cs#L35) |
| property | Emby.YouTubePlugin.PluginConfiguration.ChannelSortBy | [37:9](../PluginConfiguration.cs#L37) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.ChannelSortBy.get | [37:39](../PluginConfiguration.cs#L37) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.ChannelSortBy.set | [37:44](../PluginConfiguration.cs#L37) |
| property | Emby.YouTubePlugin.PluginConfiguration.MaxChannelVideos | [39:9](../PluginConfiguration.cs#L39) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.MaxChannelVideos.get | [39:39](../PluginConfiguration.cs#L39) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.MaxChannelVideos.set | [39:44](../PluginConfiguration.cs#L39) |
| property | Emby.YouTubePlugin.PluginConfiguration.MaxSearchVideos | [41:9](../PluginConfiguration.cs#L41) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.MaxSearchVideos.get | [41:38](../PluginConfiguration.cs#L41) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.MaxSearchVideos.set | [41:43](../PluginConfiguration.cs#L41) |
| property | Emby.YouTubePlugin.PluginConfiguration.WatchLaterPollMinutes | [43:9](../PluginConfiguration.cs#L43) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.WatchLaterPollMinutes.get | [43:44](../PluginConfiguration.cs#L43) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.WatchLaterPollMinutes.set | [43:49](../PluginConfiguration.cs#L43) |
| property | Emby.YouTubePlugin.PluginConfiguration.DirectPlaybackEnabled | [45:9](../PluginConfiguration.cs#L45) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.DirectPlaybackEnabled.get | [45:45](../PluginConfiguration.cs#L45) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.DirectPlaybackEnabled.set | [45:50](../PluginConfiguration.cs#L45) |
| property | Emby.YouTubePlugin.PluginConfiguration.YtDlpPath | [49:9](../PluginConfiguration.cs#L49) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.YtDlpPath.get | [49:35](../PluginConfiguration.cs#L49) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.YtDlpPath.set | [49:40](../PluginConfiguration.cs#L49) |
| property | Emby.YouTubePlugin.PluginConfiguration.YtDlpAutoUpdateEnabled | [52:9](../PluginConfiguration.cs#L52) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.YtDlpAutoUpdateEnabled.get | [52:46](../PluginConfiguration.cs#L52) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.YtDlpAutoUpdateEnabled.set | [52:51](../PluginConfiguration.cs#L52) |
| property | Emby.YouTubePlugin.PluginConfiguration.YtDlpJsRuntimePath | [54:9](../PluginConfiguration.cs#L54) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.YtDlpJsRuntimePath.get | [54:44](../PluginConfiguration.cs#L54) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.YtDlpJsRuntimePath.set | [54:49](../PluginConfiguration.cs#L54) |
| field | Emby.YouTubePlugin.PluginConfiguration._directPlaybackMaxHeight | [56:21](../PluginConfiguration.cs#L56) |
| property | Emby.YouTubePlugin.PluginConfiguration.DirectPlaybackMaxHeight | [60:9](../PluginConfiguration.cs#L60) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.DirectPlaybackMaxHeight.get | [62:13](../PluginConfiguration.cs#L62) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.DirectPlaybackMaxHeight.set | [63:13](../PluginConfiguration.cs#L63) |
| field | Emby.YouTubePlugin.PluginConfiguration._directPlaybackH264MaxHeight | [66:21](../PluginConfiguration.cs#L66) |
| property | Emby.YouTubePlugin.PluginConfiguration.DirectPlaybackH264MaxHeight | [68:9](../PluginConfiguration.cs#L68) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.DirectPlaybackH264MaxHeight.get | [70:13](../PluginConfiguration.cs#L70) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.DirectPlaybackH264MaxHeight.set | [71:13](../PluginConfiguration.cs#L71) |
| property | Emby.YouTubePlugin.PluginConfiguration.Donate | [74:9](../PluginConfiguration.cs#L74) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.Donate.get | [74:32](../PluginConfiguration.cs#L74) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.Donate.set | [74:37](../PluginConfiguration.cs#L74) |
| property | Emby.YouTubePlugin.PluginConfiguration.QuotaStatus | [76:9](../PluginConfiguration.cs#L76) |
| property | Emby.YouTubePlugin.PluginConfiguration.QuotaUsedToday | [82:9](../PluginConfiguration.cs#L82) |
| property | Emby.YouTubePlugin.PluginConfiguration.QuotaDailyLimit | [85:9](../PluginConfiguration.cs#L85) |
| property | Emby.YouTubePlugin.PluginConfiguration.QuotaResetSeconds | [88:9](../PluginConfiguration.cs#L88) |
| accessor | Emby.YouTubePlugin.PluginConfiguration.QuotaResetSeconds.get | [91:13](../PluginConfiguration.cs#L91) |
| local-variable | Emby.YouTubePlugin.PluginConfiguration.QuotaResetSeconds.get.sec | [93:21](../PluginConfiguration.cs#L93) |
| property | Emby.YouTubePlugin.PluginConfiguration.QuotaLifetime | [98:9](../PluginConfiguration.cs#L98) |
| property | Emby.YouTubePlugin.PluginConfiguration.QuotaSearchCallsToday | [101:9](../PluginConfiguration.cs#L101) |
| property | Emby.YouTubePlugin.PluginConfiguration.QuotaSearchDailyLimit | [104:9](../PluginConfiguration.cs#L104) |
| property | Emby.YouTubePlugin.PluginConfiguration.QuotaOtherUnitsToday | [107:9](../PluginConfiguration.cs#L107) |
| property | Emby.YouTubePlugin.PluginConfiguration.QuotaOtherDailyLimit | [110:9](../PluginConfiguration.cs#L110) |
| property | Emby.YouTubePlugin.PluginConfiguration.QuotaTotalSearchCalls | [113:9](../PluginConfiguration.cs#L113) |
| property | Emby.YouTubePlugin.PluginConfiguration.QuotaTotalOtherUnits | [116:9](../PluginConfiguration.cs#L116) |

</details>

<a id="file-d27f2eafc5169b60"></a>
## [PluginEntryPoint.cs](../PluginEntryPoint.cs)

Plugin Compile source (SDK default); 132098 bytes; 3038 lines.

<details>
<summary>All 693 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [23:1](../PluginEntryPoint.cs#L23) |
| type | Emby.YouTubePlugin.PluginEntryPoint | [25:5](../PluginEntryPoint.cs#L25) |
| field | Emby.YouTubePlugin.PluginEntryPoint._sortNameRepairer | [27:50](../PluginEntryPoint.cs#L27) |
| field | Emby.YouTubePlugin.PluginEntryPoint._imageRepairsInFlight | [28:59](../PluginEntryPoint.cs#L28) |
| field | Emby.YouTubePlugin.PluginEntryPoint._resumeSeeksInFlight | [29:65](../PluginEntryPoint.cs#L29) |
| field | Emby.YouTubePlugin.PluginEntryPoint._pendingResumeSeeks | [30:74](../PluginEntryPoint.cs#L30) |
| field | Emby.YouTubePlugin.PluginEntryPoint._resumeSeekTerminals | [31:75](../PluginEntryPoint.cs#L31) |
| field | Emby.YouTubePlugin.PluginEntryPoint._resumeCheckpoints | [32:73](../PluginEntryPoint.cs#L32) |
| field | Emby.YouTubePlugin.PluginEntryPoint._lastResumeProgressBySession | [33:73](../PluginEntryPoint.cs#L33) |
| field | Emby.YouTubePlugin.PluginEntryPoint._resumeTrackingFloorsBySession | [34:61](../PluginEntryPoint.cs#L34) |
| field | Emby.YouTubePlugin.PluginEntryPoint._lastResumeCheckpointFlushByKey | [35:65](../PluginEntryPoint.cs#L35) |
| field | Emby.YouTubePlugin.PluginEntryPoint._lastPlaylistFingerprints | [36:53](../PluginEntryPoint.cs#L36) |
| field | Emby.YouTubePlugin.PluginEntryPoint._playlistFingerprintLock | [37:33](../PluginEntryPoint.cs#L37) |
| field | Emby.YouTubePlugin.PluginEntryPoint.PlaylistFingerprintFileLock | [38:40](../PluginEntryPoint.cs#L38) |
| field | Emby.YouTubePlugin.PluginEntryPoint._resumeCheckpointFileLock | [39:33](../PluginEntryPoint.cs#L39) |
| field | Emby.YouTubePlugin.PluginEntryPoint._homeSectionManager | [40:52](../PluginEntryPoint.cs#L40) |
| field | Emby.YouTubePlugin.PluginEntryPoint._captionOffGuard | [41:49](../PluginEntryPoint.cs#L41) |
| field | Emby.YouTubePlugin.PluginEntryPoint._userManager | [42:39](../PluginEntryPoint.cs#L42) |
| field | Emby.YouTubePlugin.PluginEntryPoint._lifetimeCts | [43:50](../PluginEntryPoint.cs#L43) |
| field | Emby.YouTubePlugin.PluginEntryPoint._lifetimeToken | [44:44](../PluginEntryPoint.cs#L44) |
| field | Emby.YouTubePlugin.PluginEntryPoint._resumeDispatchLifetimeGate | [45:33](../PluginEntryPoint.cs#L45) |
| field | Emby.YouTubePlugin.PluginEntryPoint._pollIdle | [46:47](../PluginEntryPoint.cs#L46) |
| field | Emby.YouTubePlugin.PluginEntryPoint._libraryManager | [47:34](../PluginEntryPoint.cs#L47) |
| field | Emby.YouTubePlugin.PluginEntryPoint._sessionManager | [48:34](../PluginEntryPoint.cs#L48) |
| field | Emby.YouTubePlugin.PluginEntryPoint._userDataManager | [49:35](../PluginEntryPoint.cs#L49) |
| field | Emby.YouTubePlugin.PluginEntryPoint._pollTimer | [50:24](../PluginEntryPoint.cs#L50) |
| field | Emby.YouTubePlugin.PluginEntryPoint._resumeCheckpointSaveTimer | [51:24](../PluginEntryPoint.cs#L51) |
| field | Emby.YouTubePlugin.PluginEntryPoint._pollRunning | [52:21](../PluginEntryPoint.cs#L52) |
| field | Emby.YouTubePlugin.PluginEntryPoint._currentPollMinutes | [53:21](../PluginEntryPoint.cs#L53) |
| field | Emby.YouTubePlugin.PluginEntryPoint._resumeCheckpointsDirty | [54:21](../PluginEntryPoint.cs#L54) |
| field | Emby.YouTubePlugin.PluginEntryPoint._disposed | [55:21](../PluginEntryPoint.cs#L55) |
| field | Emby.YouTubePlugin.PluginEntryPoint._bootstrapHashChecked | [56:21](../PluginEntryPoint.cs#L56) |
| field | Emby.YouTubePlugin.PluginEntryPoint._nextResumeSeekGeneration | [57:22](../PluginEntryPoint.cs#L57) |
| field | Emby.YouTubePlugin.PluginEntryPoint._playlistFingerprintsDirty | [58:22](../PluginEntryPoint.cs#L58) |
| field | Emby.YouTubePlugin.PluginEntryPoint._current | [59:42](../PluginEntryPoint.cs#L59) |
| method | Emby.YouTubePlugin.PluginEntryPoint.SuppressLegacyPlaybackForDirectMode | [63:9](../PluginEntryPoint.cs#L63) |
| method | Emby.YouTubePlugin.PluginEntryPoint.SkipLegacyPlayback | [65:9](../PluginEntryPoint.cs#L65) |
| field | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekMinimumTicks | [83:28](../PluginEntryPoint.cs#L83) |
| field | Emby.YouTubePlugin.PluginEntryPoint.FireTvFallbackReadyTicks | [87:28](../PluginEntryPoint.cs#L87) |
| field | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekEndGuardTicks | [88:28](../PluginEntryPoint.cs#L88) |
| field | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekDelay | [89:42](../PluginEntryPoint.cs#L89) |
| field | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekRetryDelay | [90:42](../PluginEntryPoint.cs#L90) |
| field | Emby.YouTubePlugin.PluginEntryPoint.FireTvFallbackWindow | [91:42](../PluginEntryPoint.cs#L91) |
| field | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekRetiredTtl | [92:42](../PluginEntryPoint.cs#L92) |
| field | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekRawProgressFreshness | [93:42](../PluginEntryPoint.cs#L93) |
| field | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekPostSendGrace | [97:42](../PluginEntryPoint.cs#L97) |
| field | Emby.YouTubePlugin.PluginEntryPoint.FireTvManualSeekTolerance | [98:42](../PluginEntryPoint.cs#L98) |
| field | Emby.YouTubePlugin.PluginEntryPoint.ResumeCheckpointFlushDelay | [99:42](../PluginEntryPoint.cs#L99) |
| field | Emby.YouTubePlugin.PluginEntryPoint.ResumeCheckpointFlushInterval | [100:42](../PluginEntryPoint.cs#L100) |
| field | Emby.YouTubePlugin.PluginEntryPoint.ResumeCheckpointTtl | [101:42](../PluginEntryPoint.cs#L101) |
| field | Emby.YouTubePlugin.PluginEntryPoint.RecentSessionRestartWindow | [102:42](../PluginEntryPoint.cs#L102) |
| field | Emby.YouTubePlugin.PluginEntryPoint.RecentSessionCleanupInterval | [103:42](../PluginEntryPoint.cs#L103) |
| field | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekMaxAttempts | [104:27](../PluginEntryPoint.cs#L104) |
| field | Emby.YouTubePlugin.PluginEntryPoint.FireTvResumeSeekMaxAttempts | [108:27](../PluginEntryPoint.cs#L108) |
| field | Emby.YouTubePlugin.PluginEntryPoint._lastRecentSessionCleanupUtc | [109:26](../PluginEntryPoint.cs#L109) |
| type | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek | [111:9](../PluginEntryPoint.cs#L111) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.Key | [112:13](../PluginEntryPoint.cs#L112) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.SessionId | [113:13](../PluginEntryPoint.cs#L113) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.UserId | [114:13](../PluginEntryPoint.cs#L114) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.VideoId | [115:13](../PluginEntryPoint.cs#L115) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.ItemId | [116:13](../PluginEntryPoint.cs#L116) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.PositionTicks | [117:13](../PluginEntryPoint.cs#L117) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.RuntimeTicks | [118:13](../PluginEntryPoint.cs#L118) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.SingleSeekAttempt | [119:13](../PluginEntryPoint.cs#L119) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.IsFireTvFallback | [120:13](../PluginEntryPoint.cs#L120) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.RequiresObservedProgress | [121:13](../PluginEntryPoint.cs#L121) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.Generation | [122:13](../PluginEntryPoint.cs#L122) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.PlaySessionId | [123:13](../PluginEntryPoint.cs#L123) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.DispatchGate | [124:13](../PluginEntryPoint.cs#L124) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.EarliestSeekUtc | [125:13](../PluginEntryPoint.cs#L125) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.ExpiresUtc | [126:13](../PluginEntryPoint.cs#L126) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.HasObservedProgress | [127:13](../PluginEntryPoint.cs#L127) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.ObservedProgressTicks | [128:13](../PluginEntryPoint.cs#L128) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.FirstObservedProgressUtc | [129:13](../PluginEntryPoint.cs#L129) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.LastRawProgressTicks | [130:13](../PluginEntryPoint.cs#L130) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.LastRawProgressUtc | [131:13](../PluginEntryPoint.cs#L131) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.ProgressSequence | [132:13](../PluginEntryPoint.cs#L132) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.AttemptsSent | [133:13](../PluginEntryPoint.cs#L133) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.LastSeekUtc | [134:13](../PluginEntryPoint.cs#L134) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PendingResumeSeek.DispatchInFlight | [135:13](../PluginEntryPoint.cs#L135) |
| type | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekDispatchGate | [137:9](../PluginEntryPoint.cs#L137) |
| type | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekTerminal | [141:9](../PluginEntryPoint.cs#L141) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekTerminal.UserId | [141:50](../PluginEntryPoint.cs#L141) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekTerminal.UpdatedUtc | [141:66](../PluginEntryPoint.cs#L141) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekTerminal.Active | [141:87](../PluginEntryPoint.cs#L141) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekTerminal.Reason | [141:100](../PluginEntryPoint.cs#L141) |
| property | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekTerminal.ProtectedVideoId | [145:13](../PluginEntryPoint.cs#L145) |
| accessor | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekTerminal.ProtectedVideoId.get | [145:47](../PluginEntryPoint.cs#L145) |
| accessor | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekTerminal.ProtectedVideoId.init | [145:52](../PluginEntryPoint.cs#L145) |
| property | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekTerminal.ProtectedPositionTicks | [146:13](../PluginEntryPoint.cs#L146) |
| accessor | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekTerminal.ProtectedPositionTicks.get | [146:50](../PluginEntryPoint.cs#L146) |
| accessor | Emby.YouTubePlugin.PluginEntryPoint.ResumeSeekTerminal.ProtectedPositionTicks.init | [146:55](../PluginEntryPoint.cs#L146) |
| type | Emby.YouTubePlugin.PluginEntryPoint.ResumePlaybackStatus | [149:9](../PluginEntryPoint.cs#L149) |
| enum-member | Emby.YouTubePlugin.PluginEntryPoint.ResumePlaybackStatus.NotReady | [149:45](../PluginEntryPoint.cs#L149) |
| enum-member | Emby.YouTubePlugin.PluginEntryPoint.ResumePlaybackStatus.Current | [149:55](../PluginEntryPoint.cs#L149) |
| enum-member | Emby.YouTubePlugin.PluginEntryPoint.ResumePlaybackStatus.Superseded | [149:64](../PluginEntryPoint.cs#L149) |
| type | Emby.YouTubePlugin.PluginEntryPoint.ResumeCheckpoint | [151:9](../PluginEntryPoint.cs#L151) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ResumeCheckpoint.UserId | [152:13](../PluginEntryPoint.cs#L152) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ResumeCheckpoint.VideoId | [153:13](../PluginEntryPoint.cs#L153) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ResumeCheckpoint.PositionTicks | [154:13](../PluginEntryPoint.cs#L154) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ResumeCheckpoint.RuntimeTicks | [155:13](../PluginEntryPoint.cs#L155) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ResumeCheckpoint.UpdatedUtc | [156:13](../PluginEntryPoint.cs#L156) |
| field | Emby.YouTubePlugin.PluginEntryPoint.LastConfigHash | [161:32](../PluginEntryPoint.cs#L161) |
| property | Emby.YouTubePlugin.PluginEntryPoint.ConfigHashPath | [163:9](../PluginEntryPoint.cs#L163) |
| property | Emby.YouTubePlugin.PluginEntryPoint.ResumeCheckpointPath | [166:9](../PluginEntryPoint.cs#L166) |
| property | Emby.YouTubePlugin.PluginEntryPoint.PlaylistFingerprintPath | [169:9](../PluginEntryPoint.cs#L169) |
| constructor | Emby.YouTubePlugin.PluginEntryPoint.PluginEntryPoint | [172:9](../PluginEntryPoint.cs#L172) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PluginEntryPoint.applicationHost | [173:13](../PluginEntryPoint.cs#L173) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PluginEntryPoint.sessionManager | [174:13](../PluginEntryPoint.cs#L174) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PluginEntryPoint.libraryManager | [175:13](../PluginEntryPoint.cs#L175) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PluginEntryPoint.userDataManager | [176:13](../PluginEntryPoint.cs#L176) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PluginEntryPoint.userManager | [177:13](../PluginEntryPoint.cs#L177) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PluginEntryPoint.userViewManager | [178:13](../PluginEntryPoint.cs#L178) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PluginEntryPoint.channelManager | [179:13](../PluginEntryPoint.cs#L179) |
| method | Emby.YouTubePlugin.PluginEntryPoint.Run | [192:9](../PluginEntryPoint.cs#L192) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.Run.upgradeRefreshQueued | [200:17](../PluginEntryPoint.cs#L200) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.Run.ex | [223:19](../PluginEntryPoint.cs#L223) |
| method | Emby.YouTubePlugin.PluginEntryPoint.OnHomeSectionUserChanged | [243:9](../PluginEntryPoint.cs#L243) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.OnHomeSectionUserChanged.sender | [244:13](../PluginEntryPoint.cs#L244) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.OnHomeSectionUserChanged.eventArgs | [245:13](../PluginEntryPoint.cs#L245) |
| method | Emby.YouTubePlugin.PluginEntryPoint.RequestHomeSectionSync | [253:9](../PluginEntryPoint.cs#L253) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.RequestHomeSectionSync.reason | [254:13](../PluginEntryPoint.cs#L254) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.RequestHomeSectionSync.delay | [255:13](../PluginEntryPoint.cs#L255) |
| method | Emby.YouTubePlugin.PluginEntryPoint.QueueHomeSectionSync | [260:9](../PluginEntryPoint.cs#L260) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.QueueHomeSectionSync.reason | [260:43](../PluginEntryPoint.cs#L260) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.QueueHomeSectionSync.delay | [260:58](../PluginEntryPoint.cs#L260) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.QueueHomeSectionSync.cancellationToken | [262:17](../PluginEntryPoint.cs#L262) |
| anonymous-function | Emby.YouTubePlugin.PluginEntryPoint.QueueHomeSectionSync.anonymous@12766 | [263:26](../PluginEntryPoint.cs#L263) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.QueueHomeSectionSync.anonymous@12766.ex | [276:23](../PluginEntryPoint.cs#L276) |
| method | Emby.YouTubePlugin.PluginEntryPoint.AttachResumeSeekHook | [284:9](../PluginEntryPoint.cs#L284) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.AttachResumeSeekHook.ex | [299:19](../PluginEntryPoint.cs#L299) |
| method | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStart | [305:9](../PluginEntryPoint.cs#L305) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStart.sender | [305:38](../PluginEntryPoint.cs#L305) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStart.e | [305:54](../PluginEntryPoint.cs#L305) |
| method | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStartCore | [314:9](../PluginEntryPoint.cs#L314) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStartCore.sender | [314:42](../PluginEntryPoint.cs#L314) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStartCore.e | [314:58](../PluginEntryPoint.cs#L314) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStartCore.item | [322:21](../PluginEntryPoint.cs#L322) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStartCore.session | [323:21](../PluginEntryPoint.cs#L323) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStartCore.key | [331:21](../PluginEntryPoint.cs#L331) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStartCore.videoId | [335:21](../PluginEntryPoint.cs#L335) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStartCore.runtimeTicks | [361:21](../PluginEntryPoint.cs#L361) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStartCore.positionTicks | [362:22](../PluginEntryPoint.cs#L362) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStartCore.isFireTvOrAndroidTv | [363:21](../PluginEntryPoint.cs#L363) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStartCore.intent | [369:33](../PluginEntryPoint.cs#L369) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStartCore.ambiguousIntent | [370:33](../PluginEntryPoint.cs#L370) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStartCore.checkpoint | [390:82](../PluginEntryPoint.cs#L390) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStartCore.generation | [428:21](../PluginEntryPoint.cs#L428) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStartCore.now | [429:21](../PluginEntryPoint.cs#L429) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStartCore.pending | [430:21](../PluginEntryPoint.cs#L430) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStartCore.ex | [473:19](../PluginEntryPoint.cs#L473) |
| method | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStopped | [479:9](../PluginEntryPoint.cs#L479) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStopped.sender | [479:40](../PluginEntryPoint.cs#L479) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStopped.e | [479:56](../PluginEntryPoint.cs#L479) |
| method | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStoppedCore | [488:9](../PluginEntryPoint.cs#L488) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStoppedCore.sender | [488:44](../PluginEntryPoint.cs#L488) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStoppedCore.e | [488:60](../PluginEntryPoint.cs#L488) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStoppedCore.item | [491:17](../PluginEntryPoint.cs#L491) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStoppedCore.session | [492:17](../PluginEntryPoint.cs#L492) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStoppedCore.sessionId | [493:17](../PluginEntryPoint.cs#L493) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStoppedCore.videoId | [494:17](../PluginEntryPoint.cs#L494) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStoppedCore.stopKey | [495:17](../PluginEntryPoint.cs#L495) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStoppedCore.isStaleStop | [507:17](../PluginEntryPoint.cs#L507) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStoppedCore.stopPositionTicks | [530:29](../PluginEntryPoint.cs#L530) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStoppedCore.ex | [562:19](../PluginEntryPoint.cs#L562) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStoppedCore.pending | [572:78](../PluginEntryPoint.cs#L572) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStoppedCore.preserveReplacementState | [579:25](../PluginEntryPoint.cs#L579) |
| anonymous-function | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStoppedCore.anonymous@27208 | [585:61](../PluginEntryPoint.cs#L585) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackStoppedCore.anonymous@27208.p | [585:61](../PluginEntryPoint.cs#L585) |
| method | Emby.YouTubePlugin.PluginEntryPoint.CancelPendingResumeSeeksForSession | [596:9](../PluginEntryPoint.cs#L596) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.CancelPendingResumeSeeksForSession.sessionId | [596:57](../PluginEntryPoint.cs#L596) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.CancelPendingResumeSeeksForSession.exceptKey | [596:75](../PluginEntryPoint.cs#L596) |
| iteration-variable | Emby.YouTubePlugin.PluginEntryPoint.CancelPendingResumeSeeksForSession.kvp | [598:13](../PluginEntryPoint.cs#L598) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.CancelPendingResumeSeeksForSession.prefix | [610:17](../PluginEntryPoint.cs#L610) |
| iteration-variable | Emby.YouTubePlugin.PluginEntryPoint.CancelPendingResumeSeeksForSession.key | [611:13](../PluginEntryPoint.cs#L611) |
| method | Emby.YouTubePlugin.PluginEntryPoint.TryBeginResumePlayback | [618:9](../PluginEntryPoint.cs#L618) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryBeginResumePlayback.key | [618:45](../PluginEntryPoint.cs#L618) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryBeginResumePlayback.session | [618:57](../PluginEntryPoint.cs#L618) |
| method | Emby.YouTubePlugin.PluginEntryPoint.HasResumeSeekTerminal | [647:9](../PluginEntryPoint.cs#L647) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.HasResumeSeekTerminal.key | [647:44](../PluginEntryPoint.cs#L647) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.HasResumeSeekTerminal.userId | [647:56](../PluginEntryPoint.cs#L647) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.HasResumeSeekTerminal.terminal | [648:59](../PluginEntryPoint.cs#L648) |
| method | Emby.YouTubePlugin.PluginEntryPoint.IsResumePlaybackReservationCurrent | [651:9](../PluginEntryPoint.cs#L651) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.IsResumePlaybackReservationCurrent.key | [651:57](../PluginEntryPoint.cs#L651) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.IsResumePlaybackReservationCurrent.userId | [651:69](../PluginEntryPoint.cs#L651) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.IsResumePlaybackReservationCurrent.terminal | [652:59](../PluginEntryPoint.cs#L652) |
| method | Emby.YouTubePlugin.PluginEntryPoint.SetResumeProtection | [656:9](../PluginEntryPoint.cs#L656) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.SetResumeProtection.key | [656:42](../PluginEntryPoint.cs#L656) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.SetResumeProtection.userId | [656:54](../PluginEntryPoint.cs#L656) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.SetResumeProtection.videoId | [656:70](../PluginEntryPoint.cs#L656) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.SetResumeProtection.positionTicks | [656:87](../PluginEntryPoint.cs#L656) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.SetResumeProtection.current | [658:63](../PluginEntryPoint.cs#L658) |
| method | Emby.YouTubePlugin.PluginEntryPoint.ClearResumeProtection | [670:9](../PluginEntryPoint.cs#L670) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ClearResumeProtection.key | [670:44](../PluginEntryPoint.cs#L670) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ClearResumeProtection.userId | [670:56](../PluginEntryPoint.cs#L670) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ClearResumeProtection.sessionId | [670:72](../PluginEntryPoint.cs#L670) |
| method | Emby.YouTubePlugin.PluginEntryPoint.HasOtherActiveResumePlayback | [676:9](../PluginEntryPoint.cs#L676) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.HasOtherActiveResumePlayback.sessionId | [676:51](../PluginEntryPoint.cs#L676) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.HasOtherActiveResumePlayback.key | [676:69](../PluginEntryPoint.cs#L676) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.HasOtherActiveResumePlayback.prefix | [678:17](../PluginEntryPoint.cs#L678) |
| anonymous-function | Emby.YouTubePlugin.PluginEntryPoint.HasOtherActiveResumePlayback.anonymous@31261 | [679:45](../PluginEntryPoint.cs#L679) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.HasOtherActiveResumePlayback.anonymous@31261.terminal | [679:45](../PluginEntryPoint.cs#L679) |
| method | Emby.YouTubePlugin.PluginEntryPoint.TerminalizePendingResumeSeek | [685:9](../PluginEntryPoint.cs#L685) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TerminalizePendingResumeSeek.key | [686:13](../PluginEntryPoint.cs#L686) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TerminalizePendingResumeSeek.pending | [687:13](../PluginEntryPoint.cs#L687) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TerminalizePendingResumeSeek.reason | [688:13](../PluginEntryPoint.cs#L688) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.TerminalizePendingResumeSeek.current | [694:71](../PluginEntryPoint.cs#L694) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.TerminalizePendingResumeSeek.terminal | [701:25](../PluginEntryPoint.cs#L701) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.TerminalizePendingResumeSeek.terminal.existing | [701:82](../PluginEntryPoint.cs#L701) |
| method | Emby.YouTubePlugin.PluginEntryPoint.RetireResumeSeekTerminal | [715:9](../PluginEntryPoint.cs#L715) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.RetireResumeSeekTerminal.key | [715:47](../PluginEntryPoint.cs#L715) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.RetireResumeSeekTerminal.userId | [715:59](../PluginEntryPoint.cs#L715) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.RetireResumeSeekTerminal.reason | [715:75](../PluginEntryPoint.cs#L715) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.RetireResumeSeekTerminal.current | [717:63](../PluginEntryPoint.cs#L717) |
| method | Emby.YouTubePlugin.PluginEntryPoint.RetireOtherResumeSeekTerminals | [727:9](../PluginEntryPoint.cs#L727) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.RetireOtherResumeSeekTerminals.sessionId | [727:53](../PluginEntryPoint.cs#L727) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.RetireOtherResumeSeekTerminals.exceptKey | [727:71](../PluginEntryPoint.cs#L727) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.RetireOtherResumeSeekTerminals.reason | [727:89](../PluginEntryPoint.cs#L727) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.RetireOtherResumeSeekTerminals.prefix | [729:17](../PluginEntryPoint.cs#L729) |
| iteration-variable | Emby.YouTubePlugin.PluginEntryPoint.RetireOtherResumeSeekTerminals.terminal | [730:13](../PluginEntryPoint.cs#L730) |
| method | Emby.YouTubePlugin.PluginEntryPoint.CleanupResumeSeekTerminals | [740:9](../PluginEntryPoint.cs#L740) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.CleanupResumeSeekTerminals.now | [742:17](../PluginEntryPoint.cs#L742) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.CleanupResumeSeekTerminals.liveSessionIds | [743:30](../PluginEntryPoint.cs#L743) |
| anonymous-function | Emby.YouTubePlugin.PluginEntryPoint.CleanupResumeSeekTerminals.anonymous@34013 | [749:32](../PluginEntryPoint.cs#L749) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.CleanupResumeSeekTerminals.anonymous@34013.session | [749:32](../PluginEntryPoint.cs#L749) |
| anonymous-function | Emby.YouTubePlugin.PluginEntryPoint.CleanupResumeSeekTerminals.anonymous@34091 | [750:33](../PluginEntryPoint.cs#L750) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.CleanupResumeSeekTerminals.anonymous@34091.session | [750:33](../PluginEntryPoint.cs#L750) |
| iteration-variable | Emby.YouTubePlugin.PluginEntryPoint.CleanupResumeSeekTerminals.pendingEntry | [764:13](../PluginEntryPoint.cs#L764) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.CleanupResumeSeekTerminals.pendingEntry.pending | [766:21](../PluginEntryPoint.cs#L766) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.CleanupResumeSeekTerminals.pendingEntry.sessionDisappeared | [767:21](../PluginEntryPoint.cs#L767) |
| iteration-variable | Emby.YouTubePlugin.PluginEntryPoint.CleanupResumeSeekTerminals.terminal | [778:13](../PluginEntryPoint.cs#L778) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.CleanupResumeSeekTerminals.terminal.sessionIdEnd | [780:21](../PluginEntryPoint.cs#L780) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.CleanupResumeSeekTerminals.terminal.terminalSessionId | [781:21](../PluginEntryPoint.cs#L781) |
| method | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgress | [806:9](../PluginEntryPoint.cs#L806) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgress.sender | [806:41](../PluginEntryPoint.cs#L806) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgress.e | [806:57](../PluginEntryPoint.cs#L806) |
| method | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgressCore | [815:9](../PluginEntryPoint.cs#L815) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgressCore.sender | [815:45](../PluginEntryPoint.cs#L815) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgressCore.e | [815:61](../PluginEntryPoint.cs#L815) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgressCore.item | [820:21](../PluginEntryPoint.cs#L820) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgressCore.session | [821:21](../PluginEntryPoint.cs#L821) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgressCore.videoId | [825:21](../PluginEntryPoint.cs#L825) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgressCore.key | [826:21](../PluginEntryPoint.cs#L826) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgressCore.hasPendingSeek | [838:21](../PluginEntryPoint.cs#L838) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgressCore.hasPendingSeek.pending | [838:83](../PluginEntryPoint.cs#L838) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgressCore.currentTicks | [839:21](../PluginEntryPoint.cs#L839) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgressCore.now | [840:21](../PluginEntryPoint.cs#L840) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgressCore.reportedSeekTicks | [845:21](../PluginEntryPoint.cs#L845) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgressCore.rawProgressTicks | [882:25](../PluginEntryPoint.cs#L882) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgressCore.confirmed | [883:25](../PluginEntryPoint.cs#L883) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgressCore.observed | [904:29](../PluginEntryPoint.cs#L904) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgressCore.observed.manualDiscontinuity | [908:37](../PluginEntryPoint.cs#L908) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgressCore.runtimeTicksForCheckpoint | [939:33](../PluginEntryPoint.cs#L939) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.OnPlaybackProgressCore.ex | [971:19](../PluginEntryPoint.cs#L971) |
| method | Emby.YouTubePlugin.PluginEntryPoint.CanTrackPlaybackPosition | [977:9](../PluginEntryPoint.cs#L977) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.CanTrackPlaybackPosition.sessionId | [977:47](../PluginEntryPoint.cs#L977) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.CanTrackPlaybackPosition.positionTicks | [977:66](../PluginEntryPoint.cs#L977) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.CanTrackPlaybackPosition.floorTicks | [983:83](../PluginEntryPoint.cs#L983) |
| method | Emby.YouTubePlugin.PluginEntryPoint.GetResumeSeekKey | [997:9](../PluginEntryPoint.cs#L997) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetResumeSeekKey.e | [997:48](../PluginEntryPoint.cs#L997) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetResumeSeekKey.item | [997:77](../PluginEntryPoint.cs#L997) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.GetResumeSeekKey.sessionId | [999:17](../PluginEntryPoint.cs#L999) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.GetResumeSeekKey.userId | [1000:17](../PluginEntryPoint.cs#L1000) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.GetResumeSeekKey.playSessionId | [1001:17](../PluginEntryPoint.cs#L1001) |
| method | Emby.YouTubePlugin.PluginEntryPoint.GetResumeSeekKey | [1005:9](../PluginEntryPoint.cs#L1005) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetResumeSeekKey.sessionId | [1006:13](../PluginEntryPoint.cs#L1006) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetResumeSeekKey.userId | [1007:13](../PluginEntryPoint.cs#L1007) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetResumeSeekKey.playSessionId | [1008:13](../PluginEntryPoint.cs#L1008) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetResumeSeekKey.itemId | [1009:13](../PluginEntryPoint.cs#L1009) |
| method | Emby.YouTubePlugin.PluginEntryPoint.GetPlaybackPositionTicks | [1012:9](../PluginEntryPoint.cs#L1012) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetPlaybackPositionTicks.eventPositionTicks | [1012:47](../PluginEntryPoint.cs#L1012) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetPlaybackPositionTicks.session | [1012:73](../PluginEntryPoint.cs#L1012) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.GetPlaybackPositionTicks.positionTicks | [1014:17](../PluginEntryPoint.cs#L1014) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.GetPlaybackPositionTicks.sessionId | [1018:17](../PluginEntryPoint.cs#L1018) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.GetPlaybackPositionTicks.sessionPositionTicks | [1020:66](../PluginEntryPoint.cs#L1020) |
| method | Emby.YouTubePlugin.PluginEntryPoint.HasUsableFireTvFallbackProgress | [1029:9](../PluginEntryPoint.cs#L1029) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.HasUsableFireTvFallbackProgress.pending | [1030:13](../PluginEntryPoint.cs#L1030) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.HasUsableFireTvFallbackProgress.rawProgressTicks | [1031:13](../PluginEntryPoint.cs#L1031) |
| method | Emby.YouTubePlugin.PluginEntryPoint.MinimumRawProgressForFallback | [1043:9](../PluginEntryPoint.cs#L1043) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.MinimumRawProgressForFallback.pending | [1043:59](../PluginEntryPoint.cs#L1043) |
| method | Emby.YouTubePlugin.PluginEntryPoint.ObserveFireTvFallbackProgress | [1050:9](../PluginEntryPoint.cs#L1050) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ObserveFireTvFallbackProgress.pending | [1051:13](../PluginEntryPoint.cs#L1051) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ObserveFireTvFallbackProgress.rawProgressTicks | [1052:13](../PluginEntryPoint.cs#L1052) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ObserveFireTvFallbackProgress.now | [1053:13](../PluginEntryPoint.cs#L1053) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ObserveFireTvFallbackProgress.manualDiscontinuity | [1054:13](../PluginEntryPoint.cs#L1054) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.ObserveFireTvFallbackProgress.raw | [1067:17](../PluginEntryPoint.cs#L1067) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.ObserveFireTvFallbackProgress.previousUtc | [1096:87](../PluginEntryPoint.cs#L1096) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.ObserveFireTvFallbackProgress.elapsedTicks | [1114:21](../PluginEntryPoint.cs#L1114) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.ObserveFireTvFallbackProgress.progressDelta | [1115:21](../PluginEntryPoint.cs#L1115) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.ObserveFireTvFallbackProgress.isNearIntendedPosition | [1116:21](../PluginEntryPoint.cs#L1116) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.ObserveFireTvFallbackProgress.firstObservedUtc | [1147:17](../PluginEntryPoint.cs#L1147) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.ObserveFireTvFallbackProgress.progressSequence | [1148:17](../PluginEntryPoint.cs#L1148) |
| method | Emby.YouTubePlugin.PluginEntryPoint.CanDispatchFireTvFallback | [1160:9](../PluginEntryPoint.cs#L1160) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.CanDispatchFireTvFallback.pending | [1161:13](../PluginEntryPoint.cs#L1161) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.CanDispatchFireTvFallback.now | [1162:13](../PluginEntryPoint.cs#L1162) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.CanDispatchFireTvFallback.lastRawProgressUtc | [1170:58](../PluginEntryPoint.cs#L1170) |
| method | Emby.YouTubePlugin.PluginEntryPoint.IsFireTvRawPositionConfirmed | [1180:9](../PluginEntryPoint.cs#L1180) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.IsFireTvRawPositionConfirmed.pending | [1181:13](../PluginEntryPoint.cs#L1181) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.IsFireTvRawPositionConfirmed.rawProgressTicks | [1182:13](../PluginEntryPoint.cs#L1182) |
| method | Emby.YouTubePlugin.PluginEntryPoint.IsReportedManualSeek | [1187:9](../PluginEntryPoint.cs#L1187) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.IsReportedManualSeek.e | [1188:13](../PluginEntryPoint.cs#L1188) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.IsReportedManualSeek.seeksToBeginning | [1189:13](../PluginEntryPoint.cs#L1189) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.IsReportedManualSeek.seekPosition | [1191:17](../PluginEntryPoint.cs#L1191) |
| method | Emby.YouTubePlugin.PluginEntryPoint.ApplyReportedManualSeek | [1198:9](../PluginEntryPoint.cs#L1198) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ApplyReportedManualSeek.key | [1199:13](../PluginEntryPoint.cs#L1199) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ApplyReportedManualSeek.session | [1199:25](../PluginEntryPoint.cs#L1199) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ApplyReportedManualSeek.videoId | [1199:46](../PluginEntryPoint.cs#L1199) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ApplyReportedManualSeek.seekPositionTicks | [1199:62](../PluginEntryPoint.cs#L1199) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.ApplyReportedManualSeek.pending | [1205:62](../PluginEntryPoint.cs#L1205) |
| method | Emby.YouTubePlugin.PluginEntryPoint.GetLongProperty | [1216:9](../PluginEntryPoint.cs#L1216) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetLongProperty.source | [1216:46](../PluginEntryPoint.cs#L1216) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetLongProperty.name | [1216:62](../PluginEntryPoint.cs#L1216) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.GetLongProperty.value | [1220:21](../PluginEntryPoint.cs#L1220) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.GetLongProperty.longValue | [1225:26](../PluginEntryPoint.cs#L1225) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.GetLongProperty.intValue | [1226:25](../PluginEntryPoint.cs#L1226) |
| method | Emby.YouTubePlugin.PluginEntryPoint.CancelPendingResumeSeekForManualPosition | [1236:9](../PluginEntryPoint.cs#L1236) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.CancelPendingResumeSeekForManualPosition.key | [1237:13](../PluginEntryPoint.cs#L1237) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.CancelPendingResumeSeekForManualPosition.pending | [1238:13](../PluginEntryPoint.cs#L1238) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.CancelPendingResumeSeekForManualPosition.reason | [1239:13](../PluginEntryPoint.cs#L1239) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.CancelPendingResumeSeekForManualPosition.clearResumeCheckpoint | [1240:13](../PluginEntryPoint.cs#L1240) |
| method | Emby.YouTubePlugin.PluginEntryPoint.HasPendingObservedResumeSeek | [1249:9](../PluginEntryPoint.cs#L1249) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.HasPendingObservedResumeSeek.sessionId | [1249:51](../PluginEntryPoint.cs#L1249) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.HasPendingObservedResumeSeek.itemId | [1249:70](../PluginEntryPoint.cs#L1249) |
| anonymous-function | Emby.YouTubePlugin.PluginEntryPoint.HasPendingObservedResumeSeek.anonymous@55976 | [1254:51](../PluginEntryPoint.cs#L1254) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.HasPendingObservedResumeSeek.anonymous@55976.pending | [1254:51](../PluginEntryPoint.cs#L1254) |
| method | Emby.YouTubePlugin.PluginEntryPoint.IsPlaybackPaused | [1260:9](../PluginEntryPoint.cs#L1260) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.IsPlaybackPaused.e | [1260:46](../PluginEntryPoint.cs#L1260) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.IsPlaybackPaused.session | [1260:75](../PluginEntryPoint.cs#L1260) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.IsPlaybackPaused.playState | [1265:17](../PluginEntryPoint.cs#L1265) |
| method | Emby.YouTubePlugin.PluginEntryPoint.GetBooleanProperty | [1271:9](../PluginEntryPoint.cs#L1271) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetBooleanProperty.source | [1271:48](../PluginEntryPoint.cs#L1271) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetBooleanProperty.name | [1271:64](../PluginEntryPoint.cs#L1271) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.GetBooleanProperty.value | [1277:48](../PluginEntryPoint.cs#L1277) |
| method | Emby.YouTubePlugin.PluginEntryPoint.IsDisposingOrDisposed | [1286:9](../PluginEntryPoint.cs#L1286) |
| method | Emby.YouTubePlugin.PluginEntryPoint.SchedulePendingResumeSeek | [1289:9](../PluginEntryPoint.cs#L1289) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.SchedulePendingResumeSeek.key | [1289:48](../PluginEntryPoint.cs#L1289) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.SchedulePendingResumeSeek.generation | [1289:60](../PluginEntryPoint.cs#L1289) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.SchedulePendingResumeSeek.delay | [1289:77](../PluginEntryPoint.cs#L1289) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.SchedulePendingResumeSeek.reason | [1289:93](../PluginEntryPoint.cs#L1289) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.SchedulePendingResumeSeek.cancellationToken | [1292:17](../PluginEntryPoint.cs#L1292) |
| anonymous-function | Emby.YouTubePlugin.PluginEntryPoint.SchedulePendingResumeSeek.anonymous@57416 | [1293:26](../PluginEntryPoint.cs#L1293) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.SchedulePendingResumeSeek.anonymous@57416.ex | [1309:23](../PluginEntryPoint.cs#L1309) |
| method | Emby.YouTubePlugin.PluginEntryPoint.SendPlaystateCommandAsync | [1316:9](../PluginEntryPoint.cs#L1316) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.SendPlaystateCommandAsync.sessionId | [1316:54](../PluginEntryPoint.cs#L1316) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.SendPlaystateCommandAsync.userId | [1316:72](../PluginEntryPoint.cs#L1316) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.SendPlaystateCommandAsync.commandType | [1316:88](../PluginEntryPoint.cs#L1316) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.SendPlaystateCommandAsync.request | [1321:17](../PluginEntryPoint.cs#L1321) |
| method | Emby.YouTubePlugin.PluginEntryPoint.TrySendPendingResumeSeekAsync | [1335:9](../PluginEntryPoint.cs#L1335) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TrySendPendingResumeSeekAsync.key | [1335:58](../PluginEntryPoint.cs#L1335) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TrySendPendingResumeSeekAsync.generation | [1335:70](../PluginEntryPoint.cs#L1335) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TrySendPendingResumeSeekAsync.reason | [1335:87](../PluginEntryPoint.cs#L1335) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.TrySendPendingResumeSeekAsync.pending | [1340:63](../PluginEntryPoint.cs#L1340) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.TrySendPendingResumeSeekAsync.dispatchTask | [1344:17](../PluginEntryPoint.cs#L1344) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.TrySendPendingResumeSeekAsync.dispatchTask.reserved | [1344:95](../PluginEntryPoint.cs#L1344) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.TrySendPendingResumeSeekAsync.current | [1353:70](../PluginEntryPoint.cs#L1353) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.TrySendPendingResumeSeekAsync.ex | [1367:19](../PluginEntryPoint.cs#L1367) |
| method | Emby.YouTubePlugin.PluginEntryPoint.TryReserveAndBeginResumeSeek | [1377:9](../PluginEntryPoint.cs#L1377) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryReserveAndBeginResumeSeek.key | [1378:13](../PluginEntryPoint.cs#L1378) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryReserveAndBeginResumeSeek.generation | [1379:13](../PluginEntryPoint.cs#L1379) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryReserveAndBeginResumeSeek.expected | [1380:13](../PluginEntryPoint.cs#L1380) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryReserveAndBeginResumeSeek.reserved | [1381:13](../PluginEntryPoint.cs#L1381) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.TryReserveAndBeginResumeSeek.pending | [1390:74](../PluginEntryPoint.cs#L1390) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.TryReserveAndBeginResumeSeek.now | [1395:25](../PluginEntryPoint.cs#L1395) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.TryReserveAndBeginResumeSeek.playbackStatus | [1396:25](../PluginEntryPoint.cs#L1396) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.TryReserveAndBeginResumeSeek.lastRawProgressUtc | [1420:73](../PluginEntryPoint.cs#L1420) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.TryReserveAndBeginResumeSeek.lastSeekUtc | [1431:55](../PluginEntryPoint.cs#L1431) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.TryReserveAndBeginResumeSeek.commandSessionId | [1444:37](../PluginEntryPoint.cs#L1444) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.TryReserveAndBeginResumeSeek.inFlightKey | [1449:25](../PluginEntryPoint.cs#L1449) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.TryReserveAndBeginResumeSeek.currentTicks | [1454:82](../PluginEntryPoint.cs#L1454) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.TryReserveAndBeginResumeSeek.maxAttempts | [1463:25](../PluginEntryPoint.cs#L1463) |
| method | Emby.YouTubePlugin.PluginEntryPoint.GetResumeSeekInFlightKey | [1517:9](../PluginEntryPoint.cs#L1517) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetResumeSeekInFlightKey.pending | [1517:56](../PluginEntryPoint.cs#L1517) |
| method | Emby.YouTubePlugin.PluginEntryPoint.RemovePendingResumeSeek | [1520:9](../PluginEntryPoint.cs#L1520) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.RemovePendingResumeSeek.key | [1520:46](../PluginEntryPoint.cs#L1520) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.RemovePendingResumeSeek.pending | [1520:58](../PluginEntryPoint.cs#L1520) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.RemovePendingResumeSeek.current | [1524:66](../PluginEntryPoint.cs#L1524) |
| method | Emby.YouTubePlugin.PluginEntryPoint.MarkResumeSeekDispatchComplete | [1535:9](../PluginEntryPoint.cs#L1535) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.MarkResumeSeekDispatchComplete.key | [1535:53](../PluginEntryPoint.cs#L1535) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.MarkResumeSeekDispatchComplete.reserved | [1535:65](../PluginEntryPoint.cs#L1535) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.MarkResumeSeekDispatchComplete.current | [1539:66](../PluginEntryPoint.cs#L1539) |
| method | Emby.YouTubePlugin.PluginEntryPoint.TryUpdatePendingResumeSeek | [1552:9](../PluginEntryPoint.cs#L1552) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryUpdatePendingResumeSeek.key | [1553:13](../PluginEntryPoint.cs#L1553) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryUpdatePendingResumeSeek.expected | [1554:13](../PluginEntryPoint.cs#L1554) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryUpdatePendingResumeSeek.updated | [1555:13](../PluginEntryPoint.cs#L1555) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.TryUpdatePendingResumeSeek.current | [1559:67](../PluginEntryPoint.cs#L1559) |
| method | Emby.YouTubePlugin.PluginEntryPoint.TryClaimResumeSeekAttempt | [1571:9](../PluginEntryPoint.cs#L1571) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryClaimResumeSeekAttempt.inFlightKey | [1571:48](../PluginEntryPoint.cs#L1571) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryClaimResumeSeekAttempt.now | [1571:68](../PluginEntryPoint.cs#L1571) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.TryClaimResumeSeekAttempt.lastAttempt | [1575:75](../PluginEntryPoint.cs#L1575) |
| method | Emby.YouTubePlugin.PluginEntryPoint.SendSeekCommandAsync | [1591:9](../PluginEntryPoint.cs#L1591) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.SendSeekCommandAsync.pending | [1591:49](../PluginEntryPoint.cs#L1591) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.SendSeekCommandAsync.commandSessionId | [1591:76](../PluginEntryPoint.cs#L1591) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.SendSeekCommandAsync.request | [1596:17](../PluginEntryPoint.cs#L1596) |
| field | Emby.YouTubePlugin.PluginEntryPoint._sessionPositionPropertyCache | [1614:75](../PluginEntryPoint.cs#L1614) |
| method | Emby.YouTubePlugin.PluginEntryPoint.TryGetSessionPositionTicks | [1616:9](../PluginEntryPoint.cs#L1616) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryGetSessionPositionTicks.sessionId | [1616:49](../PluginEntryPoint.cs#L1616) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryGetSessionPositionTicks.positionTicks | [1616:67](../PluginEntryPoint.cs#L1616) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.TryGetSessionPositionTicks.session | [1622:21](../PluginEntryPoint.cs#L1622) |
| anonymous-function | Emby.YouTubePlugin.PluginEntryPoint.TryGetSessionPositionTicks.session.anonymous@71555 | [1623:37](../PluginEntryPoint.cs#L1623) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryGetSessionPositionTicks.session.anonymous@71555.s | [1623:37](../PluginEntryPoint.cs#L1623) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.TryGetSessionPositionTicks.prop | [1627:21](../PluginEntryPoint.cs#L1627) |
| anonymous-function | Emby.YouTubePlugin.PluginEntryPoint.TryGetSessionPositionTicks.prop.anonymous@71817 | [1629:21](../PluginEntryPoint.cs#L1629) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryGetSessionPositionTicks.prop.anonymous@71817.t | [1629:21](../PluginEntryPoint.cs#L1629) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.TryGetSessionPositionTicks.longValue | [1630:53](../PluginEntryPoint.cs#L1630) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.TryGetSessionPositionTicks.ex | [1636:19](../PluginEntryPoint.cs#L1636) |
| method | Emby.YouTubePlugin.PluginEntryPoint.SaveNativeResumePosition | [1644:9](../PluginEntryPoint.cs#L1644) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.SaveNativeResumePosition.item | [1645:13](../PluginEntryPoint.cs#L1645) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.SaveNativeResumePosition.session | [1646:13](../PluginEntryPoint.cs#L1646) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.SaveNativeResumePosition.positionTicks | [1647:13](../PluginEntryPoint.cs#L1647) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.SaveNativeResumePosition.reason | [1648:13](../PluginEntryPoint.cs#L1648) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.SaveNativeResumePosition.data | [1660:21](../PluginEntryPoint.cs#L1660) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.SaveNativeResumePosition.playedToCompletion | [1664:21](../PluginEntryPoint.cs#L1664) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.SaveNativeResumePosition.ex | [1675:19](../PluginEntryPoint.cs#L1675) |
| method | Emby.YouTubePlugin.PluginEntryPoint.GetProtectedResumePosition | [1681:9](../PluginEntryPoint.cs#L1681) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetProtectedResumePosition.key | [1681:49](../PluginEntryPoint.cs#L1681) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetProtectedResumePosition.userId | [1681:61](../PluginEntryPoint.cs#L1681) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetProtectedResumePosition.videoId | [1681:77](../PluginEntryPoint.cs#L1681) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.GetProtectedResumePosition.current | [1683:63](../PluginEntryPoint.cs#L1683) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.GetProtectedResumePosition.checkpoint | [1692:68](../PluginEntryPoint.cs#L1692) |
| method | Emby.YouTubePlugin.PluginEntryPoint.RestoreProtectedNativeResumePosition | [1697:9](../PluginEntryPoint.cs#L1697) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.RestoreProtectedNativeResumePosition.item | [1698:13](../PluginEntryPoint.cs#L1698) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.RestoreProtectedNativeResumePosition.session | [1698:28](../PluginEntryPoint.cs#L1698) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.RestoreProtectedNativeResumePosition.videoId | [1698:50](../PluginEntryPoint.cs#L1698) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.RestoreProtectedNativeResumePosition.key | [1698:66](../PluginEntryPoint.cs#L1698) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.RestoreProtectedNativeResumePosition.positionTicks | [1703:17](../PluginEntryPoint.cs#L1703) |
| method | Emby.YouTubePlugin.PluginEntryPoint.GetSessionId | [1715:9](../PluginEntryPoint.cs#L1715) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetSessionId.session | [1715:45](../PluginEntryPoint.cs#L1715) |
| method | Emby.YouTubePlugin.PluginEntryPoint.GetStringProperty | [1718:9](../PluginEntryPoint.cs#L1718) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetStringProperty.source | [1718:49](../PluginEntryPoint.cs#L1718) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetStringProperty.name | [1718:65](../PluginEntryPoint.cs#L1718) |
| method | Emby.YouTubePlugin.PluginEntryPoint.ShouldSuppressServerSideResumeSeek | [1723:9](../PluginEntryPoint.cs#L1723) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ShouldSuppressServerSideResumeSeek.session | [1723:64](../PluginEntryPoint.cs#L1723) |
| method | Emby.YouTubePlugin.PluginEntryPoint.IsLikelyNativeAndroidTabletSession | [1726:9](../PluginEntryPoint.cs#L1726) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.IsLikelyNativeAndroidTabletSession.session | [1726:64](../PluginEntryPoint.cs#L1726) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.IsLikelyNativeAndroidTabletSession.client | [1731:17](../PluginEntryPoint.cs#L1731) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.IsLikelyNativeAndroidTabletSession.deviceName | [1735:17](../PluginEntryPoint.cs#L1735) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.IsLikelyNativeAndroidTabletSession.userAgent | [1736:17](../PluginEntryPoint.cs#L1736) |
| method | Emby.YouTubePlugin.PluginEntryPoint.IsLikelyFireTvOrAndroidTvSession | [1751:9](../PluginEntryPoint.cs#L1751) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.IsLikelyFireTvOrAndroidTvSession.session | [1751:62](../PluginEntryPoint.cs#L1751) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.IsLikelyFireTvOrAndroidTvSession.client | [1753:17](../PluginEntryPoint.cs#L1753) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.IsLikelyFireTvOrAndroidTvSession.deviceName | [1754:17](../PluginEntryPoint.cs#L1754) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.IsLikelyFireTvOrAndroidTvSession.userAgent | [1755:17](../PluginEntryPoint.cs#L1755) |
| method | Emby.YouTubePlugin.PluginEntryPoint.IsLikelyLgOrWebOsSession | [1769:9](../PluginEntryPoint.cs#L1769) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.IsLikelyLgOrWebOsSession.session | [1769:54](../PluginEntryPoint.cs#L1769) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.IsLikelyLgOrWebOsSession.client | [1771:17](../PluginEntryPoint.cs#L1771) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.IsLikelyLgOrWebOsSession.deviceName | [1772:17](../PluginEntryPoint.cs#L1772) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.IsLikelyLgOrWebOsSession.userAgent | [1773:17](../PluginEntryPoint.cs#L1773) |
| method | Emby.YouTubePlugin.PluginEntryPoint.IsYouTubeLiveItem | [1783:9](../PluginEntryPoint.cs#L1783) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.IsYouTubeLiveItem.item | [1783:47](../PluginEntryPoint.cs#L1783) |
| method | Emby.YouTubePlugin.PluginEntryPoint.ContainsIgnoreCase | [1787:9](../PluginEntryPoint.cs#L1787) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ContainsIgnoreCase.value | [1787:48](../PluginEntryPoint.cs#L1787) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ContainsIgnoreCase.needle | [1787:63](../PluginEntryPoint.cs#L1787) |
| method | Emby.YouTubePlugin.PluginEntryPoint.Shorten | [1791:9](../PluginEntryPoint.cs#L1791) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.Shorten.value | [1791:39](../PluginEntryPoint.cs#L1791) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.Shorten.maxLength | [1791:53](../PluginEntryPoint.cs#L1791) |
| method | Emby.YouTubePlugin.PluginEntryPoint.GetResumePlaybackStatus | [1799:9](../PluginEntryPoint.cs#L1799) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetResumePlaybackStatus.pending | [1799:62](../PluginEntryPoint.cs#L1799) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.GetResumePlaybackStatus.session | [1803:21](../PluginEntryPoint.cs#L1803) |
| anonymous-function | Emby.YouTubePlugin.PluginEntryPoint.GetResumePlaybackStatus.session.anonymous@78965 | [1804:37](../PluginEntryPoint.cs#L1804) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetResumePlaybackStatus.session.anonymous@78965.s | [1804:37](../PluginEntryPoint.cs#L1804) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.GetResumePlaybackStatus.currentPlaySessionId | [1824:21](../PluginEntryPoint.cs#L1824) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.GetResumePlaybackStatus.ex | [1838:19](../PluginEntryPoint.cs#L1838) |
| method | Emby.YouTubePlugin.PluginEntryPoint.GetCurrentPlaySessionId | [1845:9](../PluginEntryPoint.cs#L1845) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.GetCurrentPlaySessionId.session | [1845:55](../PluginEntryPoint.cs#L1845) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.GetCurrentPlaySessionId.playSessionId | [1847:17](../PluginEntryPoint.cs#L1847) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.GetCurrentPlaySessionId.playState | [1851:17](../PluginEntryPoint.cs#L1851) |
| method | Emby.YouTubePlugin.PluginEntryPoint.IsSessionPaused | [1857:9](../PluginEntryPoint.cs#L1857) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.IsSessionPaused.session | [1857:45](../PluginEntryPoint.cs#L1857) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.IsSessionPaused.playState | [1859:17](../PluginEntryPoint.cs#L1859) |
| method | Emby.YouTubePlugin.PluginEntryPoint.TrackResumeCheckpoint | [1865:9](../PluginEntryPoint.cs#L1865) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TrackResumeCheckpoint.sessionId | [1866:13](../PluginEntryPoint.cs#L1866) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TrackResumeCheckpoint.userId | [1867:13](../PluginEntryPoint.cs#L1867) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TrackResumeCheckpoint.videoId | [1868:13](../PluginEntryPoint.cs#L1868) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TrackResumeCheckpoint.positionTicks | [1869:13](../PluginEntryPoint.cs#L1869) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TrackResumeCheckpoint.runtimeTicks | [1870:13](../PluginEntryPoint.cs#L1870) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TrackResumeCheckpoint.forceSave | [1871:13](../PluginEntryPoint.cs#L1871) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.TrackResumeCheckpoint.normalizedUserId | [1878:17](../PluginEntryPoint.cs#L1878) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.TrackResumeCheckpoint.normalizedVideoId | [1879:17](../PluginEntryPoint.cs#L1879) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.TrackResumeCheckpoint.checkpoint | [1894:17](../PluginEntryPoint.cs#L1894) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.TrackResumeCheckpoint.key | [1907:17](../PluginEntryPoint.cs#L1907) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.TrackResumeCheckpoint.existing | [1909:64](../PluginEntryPoint.cs#L1909) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.TrackResumeCheckpoint.lastFlush | [1919:78](../PluginEntryPoint.cs#L1919) |
| method | Emby.YouTubePlugin.PluginEntryPoint.TryGetResumeCheckpoint | [1927:9](../PluginEntryPoint.cs#L1927) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryGetResumeCheckpoint.userId | [1927:45](../PluginEntryPoint.cs#L1927) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryGetResumeCheckpoint.videoId | [1927:61](../PluginEntryPoint.cs#L1927) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryGetResumeCheckpoint.checkpoint | [1927:77](../PluginEntryPoint.cs#L1927) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.TryGetResumeCheckpoint.normalizedUserId | [1930:17](../PluginEntryPoint.cs#L1930) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.TryGetResumeCheckpoint.normalizedVideoId | [1931:17](../PluginEntryPoint.cs#L1931) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.TryGetResumeCheckpoint.key | [1935:17](../PluginEntryPoint.cs#L1935) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.TryGetResumeCheckpoint.candidate | [1936:62](../PluginEntryPoint.cs#L1936) |
| method | Emby.YouTubePlugin.PluginEntryPoint.TryGetRecentSessionCheckpoint | [1951:9](../PluginEntryPoint.cs#L1951) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryGetRecentSessionCheckpoint.sessionId | [1951:52](../PluginEntryPoint.cs#L1951) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryGetRecentSessionCheckpoint.videoId | [1951:71](../PluginEntryPoint.cs#L1951) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryGetRecentSessionCheckpoint.checkpoint | [1951:87](../PluginEntryPoint.cs#L1951) |
| pattern-variable | Emby.YouTubePlugin.PluginEntryPoint.TryGetRecentSessionCheckpoint.candidate | [1957:78](../PluginEntryPoint.cs#L1957) |
| method | Emby.YouTubePlugin.PluginEntryPoint.CleanupRecentSessionCheckpoints | [1972:9](../PluginEntryPoint.cs#L1972) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.CleanupRecentSessionCheckpoints.now | [1977:17](../PluginEntryPoint.cs#L1977) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.CleanupRecentSessionCheckpoints.cutoff | [1982:17](../PluginEntryPoint.cs#L1982) |
| iteration-variable | Emby.YouTubePlugin.PluginEntryPoint.CleanupRecentSessionCheckpoints.kvp | [1983:13](../PluginEntryPoint.cs#L1983) |
| method | Emby.YouTubePlugin.PluginEntryPoint.RemoveResumeCheckpoint | [1991:9](../PluginEntryPoint.cs#L1991) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.RemoveResumeCheckpoint.userId | [1991:45](../PluginEntryPoint.cs#L1991) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.RemoveResumeCheckpoint.videoId | [1991:61](../PluginEntryPoint.cs#L1991) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.RemoveResumeCheckpoint.normalizedUserId | [1994:17](../PluginEntryPoint.cs#L1994) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.RemoveResumeCheckpoint.normalizedVideoId | [1995:17](../PluginEntryPoint.cs#L1995) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.RemoveResumeCheckpoint.key | [1999:17](../PluginEntryPoint.cs#L1999) |
| method | Emby.YouTubePlugin.PluginEntryPoint.LoadResumeCheckpoints | [2007:9](../PluginEntryPoint.cs#L2007) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.LoadResumeCheckpoints.path | [2012:21](../PluginEntryPoint.cs#L2012) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.LoadResumeCheckpoints.json | [2016:21](../PluginEntryPoint.cs#L2016) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.LoadResumeCheckpoints.loaded | [2017:21](../PluginEntryPoint.cs#L2017) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.LoadResumeCheckpoints.cutoff | [2021:21](../PluginEntryPoint.cs#L2021) |
| iteration-variable | Emby.YouTubePlugin.PluginEntryPoint.LoadResumeCheckpoints.kvp | [2022:17](../PluginEntryPoint.cs#L2022) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.LoadResumeCheckpoints.kvp.checkpoint | [2024:25](../PluginEntryPoint.cs#L2024) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.LoadResumeCheckpoints.kvp.key | [2036:25](../PluginEntryPoint.cs#L2036) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.LoadResumeCheckpoints.ex | [2042:19](../PluginEntryPoint.cs#L2042) |
| method | Emby.YouTubePlugin.PluginEntryPoint.MarkResumeCheckpointsDirty | [2048:9](../PluginEntryPoint.cs#L2048) |
| anonymous-function | Emby.YouTubePlugin.PluginEntryPoint.MarkResumeCheckpointsDirty.anonymous@89526 | [2055:58](../PluginEntryPoint.cs#L2055) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.MarkResumeCheckpointsDirty.anonymous@89526._ | [2055:58](../PluginEntryPoint.cs#L2055) |
| method | Emby.YouTubePlugin.PluginEntryPoint.SaveResumeCheckpoints | [2060:9](../PluginEntryPoint.cs#L2060) |
| method | Emby.YouTubePlugin.PluginEntryPoint.SaveResumeCheckpointsLocked | [2070:9](../PluginEntryPoint.cs#L2070) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.SaveResumeCheckpointsLocked.cutoff | [2078:21](../PluginEntryPoint.cs#L2078) |
| iteration-variable | Emby.YouTubePlugin.PluginEntryPoint.SaveResumeCheckpointsLocked.kvp | [2079:17](../PluginEntryPoint.cs#L2079) |
| iteration-variable | Emby.YouTubePlugin.PluginEntryPoint.SaveResumeCheckpointsLocked.flushKey | [2090:17](../PluginEntryPoint.cs#L2090) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.SaveResumeCheckpointsLocked.snapshot | [2096:21](../PluginEntryPoint.cs#L2096) |
| anonymous-function | Emby.YouTubePlugin.PluginEntryPoint.SaveResumeCheckpointsLocked.snapshot.anonymous@91390 | [2096:64](../PluginEntryPoint.cs#L2096) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.SaveResumeCheckpointsLocked.snapshot.anonymous@91390.kvp | [2096:64](../PluginEntryPoint.cs#L2096) |
| anonymous-function | Emby.YouTubePlugin.PluginEntryPoint.SaveResumeCheckpointsLocked.snapshot.anonymous@91406 | [2096:80](../PluginEntryPoint.cs#L2096) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.SaveResumeCheckpointsLocked.snapshot.anonymous@91406.kvp | [2096:80](../PluginEntryPoint.cs#L2096) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.SaveResumeCheckpointsLocked.path | [2097:21](../PluginEntryPoint.cs#L2097) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.SaveResumeCheckpointsLocked.dir | [2098:21](../PluginEntryPoint.cs#L2098) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.SaveResumeCheckpointsLocked.json | [2102:21](../PluginEntryPoint.cs#L2102) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.SaveResumeCheckpointsLocked.tempPath | [2105:21](../PluginEntryPoint.cs#L2105) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.SaveResumeCheckpointsLocked.ex | [2112:19](../PluginEntryPoint.cs#L2112) |
| method | Emby.YouTubePlugin.PluginEntryPoint.MakeResumeCheckpointKey | [2132:9](../PluginEntryPoint.cs#L2132) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.MakeResumeCheckpointKey.userId | [2132:55](../PluginEntryPoint.cs#L2132) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.MakeResumeCheckpointKey.videoId | [2132:70](../PluginEntryPoint.cs#L2132) |
| method | Emby.YouTubePlugin.PluginEntryPoint.NormalizeResumeComponent | [2135:9](../PluginEntryPoint.cs#L2135) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.NormalizeResumeComponent.value | [2135:56](../PluginEntryPoint.cs#L2135) |
| property | Emby.YouTubePlugin.PluginEntryPoint.PluginVersionStampPath | [2138:9](../PluginEntryPoint.cs#L2138) |
| field | Emby.YouTubePlugin.PluginEntryPoint.PluginBuildRevision | [2141:30](../PluginEntryPoint.cs#L2141) |
| property | Emby.YouTubePlugin.PluginEntryPoint.ChannelSurfaceStampPath | [2147:9](../PluginEntryPoint.cs#L2147) |
| field | Emby.YouTubePlugin.PluginEntryPoint.ChannelSurfaceStamp | [2150:30](../PluginEntryPoint.cs#L2150) |
| method | Emby.YouTubePlugin.PluginEntryPoint.WipeCachesIfPluginUpgraded | [2152:9](../PluginEntryPoint.cs#L2152) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.WipeCachesIfPluginUpgraded.version | [2156:21](../PluginEntryPoint.cs#L2156) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.WipeCachesIfPluginUpgraded.current | [2157:21](../PluginEntryPoint.cs#L2157) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.WipeCachesIfPluginUpgraded.previous | [2158:25](../PluginEntryPoint.cs#L2158) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.WipeCachesIfPluginUpgraded.stampPath | [2159:21](../PluginEntryPoint.cs#L2159) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.WipeCachesIfPluginUpgraded.cacheDir | [2172:21](../PluginEntryPoint.cs#L2172) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.WipeCachesIfPluginUpgraded.ex | [2176:27](../PluginEntryPoint.cs#L2176) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.WipeCachesIfPluginUpgraded.dataDir | [2180:21](../PluginEntryPoint.cs#L2180) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.WipeCachesIfPluginUpgraded.legacyShortsProbe | [2183:25](../PluginEntryPoint.cs#L2183) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.WipeCachesIfPluginUpgraded.ex | [2188:23](../PluginEntryPoint.cs#L2188) |
| anonymous-function | Emby.YouTubePlugin.PluginEntryPoint.WipeCachesIfPluginUpgraded.anonymous@95977 | [2193:30](../PluginEntryPoint.cs#L2193) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.WipeCachesIfPluginUpgraded.anonymous@95977.refreshDepth | [2207:29](../PluginEntryPoint.cs#L2207) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.WipeCachesIfPluginUpgraded.anonymous@95977.ex | [2218:27](../PluginEntryPoint.cs#L2218) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.WipeCachesIfPluginUpgraded.ex | [2225:19](../PluginEntryPoint.cs#L2225) |
| method | Emby.YouTubePlugin.PluginEntryPoint.EnsureChannelSurfaceMigration | [2232:9](../PluginEntryPoint.cs#L2232) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.EnsureChannelSurfaceMigration.upgradeRefreshAlreadyQueued | [2232:52](../PluginEntryPoint.cs#L2232) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.EnsureChannelSurfaceMigration.stampPath | [2236:21](../PluginEntryPoint.cs#L2236) |
| anonymous-function | Emby.YouTubePlugin.PluginEntryPoint.EnsureChannelSurfaceMigration.anonymous@98669 | [2252:30](../PluginEntryPoint.cs#L2252) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.EnsureChannelSurfaceMigration.anonymous@98669.ex | [2267:27](../PluginEntryPoint.cs#L2267) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.EnsureChannelSurfaceMigration.ex | [2273:19](../PluginEntryPoint.cs#L2273) |
| method | Emby.YouTubePlugin.PluginEntryPoint.TryWriteChannelSurfaceStamp | [2279:9](../PluginEntryPoint.cs#L2279) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TryWriteChannelSurfaceStamp.stampPath | [2279:57](../PluginEntryPoint.cs#L2279) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.TryWriteChannelSurfaceStamp.ex | [2282:19](../PluginEntryPoint.cs#L2282) |
| method | Emby.YouTubePlugin.PluginEntryPoint.AttachImageRepairHook | [2285:9](../PluginEntryPoint.cs#L2285) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.AttachImageRepairHook.ex | [2298:19](../PluginEntryPoint.cs#L2298) |
| method | Emby.YouTubePlugin.PluginEntryPoint.OnItemUpdated | [2304:9](../PluginEntryPoint.cs#L2304) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.OnItemUpdated.sender | [2304:36](../PluginEntryPoint.cs#L2304) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.OnItemUpdated.e | [2304:52](../PluginEntryPoint.cs#L2304) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnItemUpdated.item | [2306:17](../PluginEntryPoint.cs#L2306) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.OnItemUpdated.itemId | [2315:17](../PluginEntryPoint.cs#L2315) |
| anonymous-function | Emby.YouTubePlugin.PluginEntryPoint.OnItemUpdated.anonymous@101254 | [2319:26](../PluginEntryPoint.cs#L2319) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.OnItemUpdated.anonymous@101254.ex | [2339:23](../PluginEntryPoint.cs#L2339) |
| method | Emby.YouTubePlugin.PluginEntryPoint.QueueExistingSortNameRepair | [2350:9](../PluginEntryPoint.cs#L2350) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.QueueExistingSortNameRepair.reason | [2350:50](../PluginEntryPoint.cs#L2350) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.QueueExistingSortNameRepair.libraryManager | [2352:17](../PluginEntryPoint.cs#L2352) |
| anonymous-function | Emby.YouTubePlugin.PluginEntryPoint.QueueExistingSortNameRepair.anonymous@102827 | [2359:26](../PluginEntryPoint.cs#L2359) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.QueueExistingSortNameRepair.anonymous@102827.items | [2365:25](../PluginEntryPoint.cs#L2365) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.QueueExistingSortNameRepair.anonymous@102827.queued | [2366:25](../PluginEntryPoint.cs#L2366) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.QueueExistingSortNameRepair.anonymous@102827.ex | [2369:23](../PluginEntryPoint.cs#L2369) |
| method | Emby.YouTubePlugin.PluginEntryPoint.MarkConfigSaved | [2378:9](../PluginEntryPoint.cs#L2378) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.MarkConfigSaved.config | [2378:48](../PluginEntryPoint.cs#L2378) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.MarkConfigSaved.hash | [2380:17](../PluginEntryPoint.cs#L2380) |
| method | Emby.YouTubePlugin.PluginEntryPoint.PollTick | [2387:9](../PluginEntryPoint.cs#L2387) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PollTick.state | [2387:31](../PluginEntryPoint.cs#L2387) |
| method | Emby.YouTubePlugin.PluginEntryPoint.PollTickAsync | [2406:9](../PluginEntryPoint.cs#L2406) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PollTickAsync.ct | [2406:42](../PluginEntryPoint.cs#L2406) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.PollTickAsync.config | [2411:21](../PluginEntryPoint.cs#L2411) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.PollTickAsync.apiKey | [2414:21](../PluginEntryPoint.cs#L2414) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.PollTickAsync.watchLaterRaw | [2415:21](../PluginEntryPoint.cs#L2415) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.PollTickAsync.ex | [2432:19](../PluginEntryPoint.cs#L2432) |
| method | Emby.YouTubePlugin.PluginEntryPoint.AdjustPollIntervalToConfig | [2443:9](../PluginEntryPoint.cs#L2443) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.AdjustPollIntervalToConfig.config | [2443:49](../PluginEntryPoint.cs#L2443) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.AdjustPollIntervalToConfig.configured | [2448:17](../PluginEntryPoint.cs#L2448) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.AdjustPollIntervalToConfig.ex | [2457:19](../PluginEntryPoint.cs#L2457) |
| method | Emby.YouTubePlugin.PluginEntryPoint.RefreshOnConfigChange | [2463:9](../PluginEntryPoint.cs#L2463) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.RefreshOnConfigChange.apiKey | [2464:13](../PluginEntryPoint.cs#L2464) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.RefreshOnConfigChange.config | [2465:13](../PluginEntryPoint.cs#L2465) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.RefreshOnConfigChange.ct | [2466:13](../PluginEntryPoint.cs#L2466) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.RefreshOnConfigChange.currentHash | [2469:17](../PluginEntryPoint.cs#L2469) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.RefreshOnConfigChange.configChanged | [2470:17](../PluginEntryPoint.cs#L2470) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.RefreshOnConfigChange.ex | [2483:19](../PluginEntryPoint.cs#L2483) |
| method | Emby.YouTubePlugin.PluginEntryPoint.PollWatchLaterPlaylists | [2489:9](../PluginEntryPoint.cs#L2489) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PollWatchLaterPlaylists.apiKey | [2490:13](../PluginEntryPoint.cs#L2490) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PollWatchLaterPlaylists.watchLaterRaw | [2491:13](../PluginEntryPoint.cs#L2491) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PollWatchLaterPlaylists.ct | [2492:13](../PluginEntryPoint.cs#L2492) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.PollWatchLaterPlaylists.playlists | [2495:17](../PluginEntryPoint.cs#L2495) |
| anonymous-function | Emby.YouTubePlugin.PluginEntryPoint.PollWatchLaterPlaylists.playlists.anonymous@108073 | [2497:25](../PluginEntryPoint.cs#L2497) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PollWatchLaterPlaylists.playlists.anonymous@108073.s | [2497:25](../PluginEntryPoint.cs#L2497) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.PollWatchLaterPlaylists.configured | [2504:17](../PluginEntryPoint.cs#L2504) |
| iteration-variable | Emby.YouTubePlugin.PluginEntryPoint.PollWatchLaterPlaylists.stalePlaylist | [2513:17](../PluginEntryPoint.cs#L2513) |
| anonymous-function | Emby.YouTubePlugin.PluginEntryPoint.PollWatchLaterPlaylists.stalePlaylist.anonymous@108814 | [2514:37](../PluginEntryPoint.cs#L2514) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.PollWatchLaterPlaylists.stalePlaylist.anonymous@108814.id | [2514:37](../PluginEntryPoint.cs#L2514) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.PollWatchLaterPlaylists.anyChanged | [2526:17](../PluginEntryPoint.cs#L2526) |
| iteration-variable | Emby.YouTubePlugin.PluginEntryPoint.PollWatchLaterPlaylists.playlist | [2527:13](../PluginEntryPoint.cs#L2527) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.PollWatchLaterPlaylists.playlist.snapshot | [2535:25](../PluginEntryPoint.cs#L2535) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.PollWatchLaterPlaylists.playlist.current | [2539:25](../PluginEntryPoint.cs#L2539) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.PollWatchLaterPlaylists.playlist.hasPrevious | [2541:26](../PluginEntryPoint.cs#L2541) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.PollWatchLaterPlaylists.playlist.previous | [2542:29](../PluginEntryPoint.cs#L2542) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.PollWatchLaterPlaylists.playlist.changed | [2543:26](../PluginEntryPoint.cs#L2543) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.PollWatchLaterPlaylists.playlist.ex | [2571:23](../PluginEntryPoint.cs#L2571) |
| method | Emby.YouTubePlugin.PluginEntryPoint.ComputePlaylistFingerprint | [2591:9](../PluginEntryPoint.cs#L2591) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ComputePlaylistFingerprint.videoIds | [2591:58](../PluginEntryPoint.cs#L2591) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ComputePlaylistFingerprint.totalResults | [2591:88](../PluginEntryPoint.cs#L2591) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.ComputePlaylistFingerprint.sha | [2593:23](../PluginEntryPoint.cs#L2593) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.ComputePlaylistFingerprint.value | [2594:17](../PluginEntryPoint.cs#L2594) |
| method | Emby.YouTubePlugin.PluginEntryPoint.LoadPlaylistFingerprints | [2600:9](../PluginEntryPoint.cs#L2600) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.LoadPlaylistFingerprints.json | [2609:25](../PluginEntryPoint.cs#L2609) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.LoadPlaylistFingerprints.loaded | [2610:25](../PluginEntryPoint.cs#L2610) |
| iteration-variable | Emby.YouTubePlugin.PluginEntryPoint.LoadPlaylistFingerprints.pair | [2616:25](../PluginEntryPoint.cs#L2616) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.LoadPlaylistFingerprints.ex | [2628:19](../PluginEntryPoint.cs#L2628) |
| method | Emby.YouTubePlugin.PluginEntryPoint.SavePlaylistFingerprints | [2634:9](../PluginEntryPoint.cs#L2634) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.SavePlaylistFingerprints.path | [2643:25](../PluginEntryPoint.cs#L2643) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.SavePlaylistFingerprints.tempPath | [2644:25](../PluginEntryPoint.cs#L2644) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.SavePlaylistFingerprints.directory | [2647:29](../PluginEntryPoint.cs#L2647) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.SavePlaylistFingerprints.ex | [2658:27](../PluginEntryPoint.cs#L2658) |
| method | Emby.YouTubePlugin.PluginEntryPoint.TrySaveConfigHash | [2668:9](../PluginEntryPoint.cs#L2668) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.TrySaveConfigHash.currentHash | [2668:47](../PluginEntryPoint.cs#L2668) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.TrySaveConfigHash.dir | [2672:21](../PluginEntryPoint.cs#L2672) |
| catch-variable | Emby.YouTubePlugin.PluginEntryPoint.TrySaveConfigHash.ex | [2678:19](../PluginEntryPoint.cs#L2678) |
| method | Emby.YouTubePlugin.PluginEntryPoint.ComputeConfigHash | [2684:9](../PluginEntryPoint.cs#L2684) |
| parameter | Emby.YouTubePlugin.PluginEntryPoint.ComputeConfigHash.c | [2684:50](../PluginEntryPoint.cs#L2684) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.ComputeConfigHash.sha | [2686:23](../PluginEntryPoint.cs#L2686) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.ComputeConfigHash.blob | [2687:17](../PluginEntryPoint.cs#L2687) |
| method | Emby.YouTubePlugin.PluginEntryPoint.Dispose | [2708:9](../PluginEntryPoint.cs#L2708) |
| local-variable | Emby.YouTubePlugin.PluginEntryPoint.Dispose.pollStopped | [2738:17](../PluginEntryPoint.cs#L2738) |
| type | Emby.YouTubePlugin.ChannelRefreshInvoker | [2753:5](../PluginEntryPoint.cs#L2753) |
| field | Emby.YouTubePlugin.ChannelRefreshInvoker.RootRefreshDepth | [2755:26](../PluginEntryPoint.cs#L2755) |
| field | Emby.YouTubePlugin.ChannelRefreshInvoker.ContentRefreshDepth | [2756:26](../PluginEntryPoint.cs#L2756) |
| field | Emby.YouTubePlugin.ChannelRefreshInvoker._channelMgr | [2757:41](../PluginEntryPoint.cs#L2757) |
| field | Emby.YouTubePlugin.ChannelRefreshInvoker._registeredChannel | [2758:34](../PluginEntryPoint.cs#L2758) |
| field | Emby.YouTubePlugin.ChannelRefreshInvoker._refreshAgainRequested | [2759:28](../PluginEntryPoint.cs#L2759) |
| field | Emby.YouTubePlugin.ChannelRefreshInvoker._nonSkippableRefreshRequested | [2760:28](../PluginEntryPoint.cs#L2760) |
| field | Emby.YouTubePlugin.ChannelRefreshInvoker._nextRefreshDepth | [2761:28](../PluginEntryPoint.cs#L2761) |
| field | Emby.YouTubePlugin.ChannelRefreshInvoker.RefreshGate | [2766:47](../PluginEntryPoint.cs#L2766) |
| field | Emby.YouTubePlugin.ChannelRefreshInvoker._refreshActive | [2772:28](../PluginEntryPoint.cs#L2772) |
| field | Emby.YouTubePlugin.ChannelRefreshInvoker._lastChannelScanTicks | [2779:29](../PluginEntryPoint.cs#L2779) |
| field | Emby.YouTubePlugin.ChannelRefreshInvoker.ScanQuietWindowMs | [2787:28](../PluginEntryPoint.cs#L2787) |
| field | Emby.YouTubePlugin.ChannelRefreshInvoker.MaxScanWait | [2794:42](../PluginEntryPoint.cs#L2794) |
| field | Emby.YouTubePlugin.ChannelRefreshInvoker.ScanWaitPollDelay | [2795:42](../PluginEntryPoint.cs#L2795) |
| method | Emby.YouTubePlugin.ChannelRefreshInvoker.NoteChannelScanActivity | [2797:9](../PluginEntryPoint.cs#L2797) |
| property | Emby.YouTubePlugin.ChannelRefreshInvoker.IsRefreshInProgress | [2800:9](../PluginEntryPoint.cs#L2800) |
| accessor | Emby.YouTubePlugin.ChannelRefreshInvoker.IsRefreshInProgress.get | [2802:13](../PluginEntryPoint.cs#L2802) |
| local-variable | Emby.YouTubePlugin.ChannelRefreshInvoker.IsRefreshInProgress.get.last | [2807:21](../PluginEntryPoint.cs#L2807) |
| method | Emby.YouTubePlugin.ChannelRefreshInvoker.Initialize | [2812:9](../PluginEntryPoint.cs#L2812) |
| parameter | Emby.YouTubePlugin.ChannelRefreshInvoker.Initialize.channelManager | [2812:39](../PluginEntryPoint.cs#L2812) |
| method | Emby.YouTubePlugin.ChannelRefreshInvoker.TriggerRefreshAsync | [2818:9](../PluginEntryPoint.cs#L2818) |
| parameter | Emby.YouTubePlugin.ChannelRefreshInvoker.TriggerRefreshAsync.requestedDepth | [2818:54](../PluginEntryPoint.cs#L2818) |
| parameter | Emby.YouTubePlugin.ChannelRefreshInvoker.TriggerRefreshAsync.skipIfScanActive | [2818:93](../PluginEntryPoint.cs#L2818) |
| local-variable | Emby.YouTubePlugin.ChannelRefreshInvoker.TriggerRefreshAsync.refreshDepth | [2837:25](../PluginEntryPoint.cs#L2837) |
| local-variable | Emby.YouTubePlugin.ChannelRefreshInvoker.TriggerRefreshAsync.maySkipIfScanActive | [2838:25](../PluginEntryPoint.cs#L2838) |
| local-variable | Emby.YouTubePlugin.ChannelRefreshInvoker.TriggerRefreshAsync.maySkipIfScanActive | [2855:25](../PluginEntryPoint.cs#L2855) |
| anonymous-function | Emby.YouTubePlugin.ChannelRefreshInvoker.TriggerRefreshAsync.anonymous@123777 | [2856:34](../PluginEntryPoint.cs#L2856) |
| method | Emby.YouTubePlugin.ChannelRefreshInvoker.TriggerRefreshCoreAsync | [2861:9](../PluginEntryPoint.cs#L2861) |
| parameter | Emby.YouTubePlugin.ChannelRefreshInvoker.TriggerRefreshCoreAsync.refreshDepth | [2861:59](../PluginEntryPoint.cs#L2861) |
| parameter | Emby.YouTubePlugin.ChannelRefreshInvoker.TriggerRefreshCoreAsync.skipIfScanActive | [2861:77](../PluginEntryPoint.cs#L2861) |
| local-variable | Emby.YouTubePlugin.ChannelRefreshInvoker.TriggerRefreshCoreAsync.waited | [2896:25](../PluginEntryPoint.cs#L2896) |
| local-variable | Emby.YouTubePlugin.ChannelRefreshInvoker.TriggerRefreshCoreAsync.gateHandedOff | [2921:21](../PluginEntryPoint.cs#L2921) |
| local-variable | Emby.YouTubePlugin.ChannelRefreshInvoker.TriggerRefreshCoreAsync.task | [2924:25](../PluginEntryPoint.cs#L2924) |
| anonymous-function | Emby.YouTubePlugin.ChannelRefreshInvoker.TriggerRefreshCoreAsync.anonymous@128408 | [2940:25](../PluginEntryPoint.cs#L2940) |
| parameter | Emby.YouTubePlugin.ChannelRefreshInvoker.TriggerRefreshCoreAsync.anonymous@128408.completedTask | [2940:25](../PluginEntryPoint.cs#L2940) |
| local-variable | Emby.YouTubePlugin.ChannelRefreshInvoker.TriggerRefreshCoreAsync.completed | [2954:25](../PluginEntryPoint.cs#L2954) |
| catch-variable | Emby.YouTubePlugin.ChannelRefreshInvoker.TriggerRefreshCoreAsync.ex | [2976:19](../PluginEntryPoint.cs#L2976) |
| method | Emby.YouTubePlugin.ChannelRefreshInvoker.RaiseNextRefreshDepth | [2982:9](../PluginEntryPoint.cs#L2982) |
| parameter | Emby.YouTubePlugin.ChannelRefreshInvoker.RaiseNextRefreshDepth.requestedDepth | [2982:51](../PluginEntryPoint.cs#L2982) |
| local-variable | Emby.YouTubePlugin.ChannelRefreshInvoker.RaiseNextRefreshDepth.normalized | [2984:17](../PluginEntryPoint.cs#L2984) |
| local-variable | Emby.YouTubePlugin.ChannelRefreshInvoker.RaiseNextRefreshDepth.current | [2988:21](../PluginEntryPoint.cs#L2988) |
| method | Emby.YouTubePlugin.ChannelRefreshInvoker.ConsumeNextRefreshDepth | [2997:9](../PluginEntryPoint.cs#L2997) |
| method | Emby.YouTubePlugin.ChannelRefreshInvoker.NormalizeRefreshDepth | [3003:9](../PluginEntryPoint.cs#L3003) |
| parameter | Emby.YouTubePlugin.ChannelRefreshInvoker.NormalizeRefreshDepth.requestedDepth | [3003:50](../PluginEntryPoint.cs#L3003) |
| method | Emby.YouTubePlugin.ChannelRefreshInvoker.EnsureChannelManager | [3011:9](../PluginEntryPoint.cs#L3011) |
| method | Emby.YouTubePlugin.ChannelRefreshInvoker.FindRegisteredYouTubeChannel | [3022:9](../PluginEntryPoint.cs#L3022) |
| catch-variable | Emby.YouTubePlugin.ChannelRefreshInvoker.FindRegisteredYouTubeChannel.ex | [3031:19](../PluginEntryPoint.cs#L3031) |

</details>

<a id="file-adad5fbc257fb6bf"></a>
## [PluginFileLog.cs](../PluginFileLog.cs)

Plugin Compile source (SDK default); 2817 bytes; 91 lines.

<details>
<summary>All 19 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [6:1](../PluginFileLog.cs#L6) |
| type | Emby.YouTubePlugin.PluginFileLog | [12:5](../PluginFileLog.cs#L12) |
| field | Emby.YouTubePlugin.PluginFileLog.MaxBytes | [14:28](../PluginFileLog.cs#L14) |
| field | Emby.YouTubePlugin.PluginFileLog.MaxBackups | [15:27](../PluginFileLog.cs#L15) |
| field | Emby.YouTubePlugin.PluginFileLog.Sync | [16:40](../PluginFileLog.cs#L16) |
| field | Emby.YouTubePlugin.PluginFileLog._resolvedPath | [17:32](../PluginFileLog.cs#L17) |
| property | Emby.YouTubePlugin.PluginFileLog.Path | [19:9](../PluginFileLog.cs#L19) |
| method | Emby.YouTubePlugin.PluginFileLog.Write | [21:9](../PluginFileLog.cs#L21) |
| parameter | Emby.YouTubePlugin.PluginFileLog.Write.message | [21:36](../PluginFileLog.cs#L21) |
| local-variable | Emby.YouTubePlugin.PluginFileLog.Write.path | [25:21](../PluginFileLog.cs#L25) |
| local-variable | Emby.YouTubePlugin.PluginFileLog.Write.line | [29:21](../PluginFileLog.cs#L29) |
| method | Emby.YouTubePlugin.PluginFileLog.ResolvePath | [43:9](../PluginFileLog.cs#L43) |
| local-variable | Emby.YouTubePlugin.PluginFileLog.ResolvePath.dataPath | [48:17](../PluginFileLog.cs#L48) |
| method | Emby.YouTubePlugin.PluginFileLog.RotateIfNeeded | [64:9](../PluginFileLog.cs#L64) |
| parameter | Emby.YouTubePlugin.PluginFileLog.RotateIfNeeded.path | [64:44](../PluginFileLog.cs#L64) |
| local-variable | Emby.YouTubePlugin.PluginFileLog.RotateIfNeeded.info | [68:21](../PluginFileLog.cs#L68) |
| local-variable | Emby.YouTubePlugin.PluginFileLog.RotateIfNeeded.i | [72:26](../PluginFileLog.cs#L72) |
| local-variable | Emby.YouTubePlugin.PluginFileLog.RotateIfNeeded.src | [74:25](../PluginFileLog.cs#L74) |
| local-variable | Emby.YouTubePlugin.PluginFileLog.RotateIfNeeded.dst | [75:25](../PluginFileLog.cs#L75) |

</details>

<a id="file-942e26b5e5820665"></a>
## [QuotaTracker.cs](../QuotaTracker.cs)

Plugin Compile source (SDK default); 15771 bytes; 423 lines.

<details>
<summary>All 113 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [9:1](../QuotaTracker.cs#L9) |
| type | Emby.YouTubePlugin.QuotaTracker | [16:5](../QuotaTracker.cs#L16) |
| field | Emby.YouTubePlugin.QuotaTracker.SearchDailyLimit | [18:28](../QuotaTracker.cs#L18) |
| field | Emby.YouTubePlugin.QuotaTracker.OtherDailyLimit | [19:28](../QuotaTracker.cs#L19) |
| field | Emby.YouTubePlugin.QuotaTracker.StateSchema | [20:27](../QuotaTracker.cs#L20) |
| field | Emby.YouTubePlugin.QuotaTracker.StateFile | [21:30](../QuotaTracker.cs#L21) |
| field | Emby.YouTubePlugin.QuotaTracker.SaveIntervalMs | [22:27](../QuotaTracker.cs#L22) |
| type | Emby.YouTubePlugin.QuotaTracker.QuotaStats | [24:9](../QuotaTracker.cs#L24) |
| parameter | Emby.YouTubePlugin.QuotaTracker.QuotaStats.SearchCallsToday | [25:13](../QuotaTracker.cs#L25) |
| parameter | Emby.YouTubePlugin.QuotaTracker.QuotaStats.SearchCallLimit | [26:13](../QuotaTracker.cs#L26) |
| parameter | Emby.YouTubePlugin.QuotaTracker.QuotaStats.OtherUnitsToday | [27:13](../QuotaTracker.cs#L27) |
| parameter | Emby.YouTubePlugin.QuotaTracker.QuotaStats.OtherUnitLimit | [28:13](../QuotaTracker.cs#L28) |
| parameter | Emby.YouTubePlugin.QuotaTracker.QuotaStats.TotalSearchCalls | [29:13](../QuotaTracker.cs#L29) |
| parameter | Emby.YouTubePlugin.QuotaTracker.QuotaStats.TotalOtherUnits | [30:13](../QuotaTracker.cs#L30) |
| parameter | Emby.YouTubePlugin.QuotaTracker.QuotaStats.QuotaDate | [31:13](../QuotaTracker.cs#L31) |
| parameter | Emby.YouTubePlugin.QuotaTracker.QuotaStats.UntilReset | [32:13](../QuotaTracker.cs#L32) |
| type | Emby.YouTubePlugin.QuotaTracker.QuotaCharge | [34:9](../QuotaTracker.cs#L34) |
| parameter | Emby.YouTubePlugin.QuotaTracker.QuotaCharge.IsSearch | [34:52](../QuotaTracker.cs#L34) |
| parameter | Emby.YouTubePlugin.QuotaTracker.QuotaCharge.OtherUnits | [34:67](../QuotaTracker.cs#L34) |
| type | Emby.YouTubePlugin.QuotaTracker.QuotaSnapshot | [36:9](../QuotaTracker.cs#L36) |
| parameter | Emby.YouTubePlugin.QuotaTracker.QuotaSnapshot.Date | [37:13](../QuotaTracker.cs#L37) |
| parameter | Emby.YouTubePlugin.QuotaTracker.QuotaSnapshot.SearchCalls | [38:13](../QuotaTracker.cs#L38) |
| parameter | Emby.YouTubePlugin.QuotaTracker.QuotaSnapshot.OtherUnits | [39:13](../QuotaTracker.cs#L39) |
| parameter | Emby.YouTubePlugin.QuotaTracker.QuotaSnapshot.TotalSearchCalls | [40:13](../QuotaTracker.cs#L40) |
| parameter | Emby.YouTubePlugin.QuotaTracker.QuotaSnapshot.TotalOtherUnits | [41:13](../QuotaTracker.cs#L41) |
| field | Emby.YouTubePlugin.QuotaTracker.StateLock | [43:40](../QuotaTracker.cs#L43) |
| field | Emby.YouTubePlugin.QuotaTracker._searchCallsToday | [44:29](../QuotaTracker.cs#L44) |
| field | Emby.YouTubePlugin.QuotaTracker._otherUnitsToday | [45:29](../QuotaTracker.cs#L45) |
| field | Emby.YouTubePlugin.QuotaTracker._totalSearchCalls | [46:29](../QuotaTracker.cs#L46) |
| field | Emby.YouTubePlugin.QuotaTracker._totalOtherUnits | [47:29](../QuotaTracker.cs#L47) |
| field | Emby.YouTubePlugin.QuotaTracker._quotaDate | [48:33](../QuotaTracker.cs#L48) |
| field | Emby.YouTubePlugin.QuotaTracker._stateVersion | [49:29](../QuotaTracker.cs#L49) |
| field | Emby.YouTubePlugin.QuotaTracker._persistedVersion | [50:29](../QuotaTracker.cs#L50) |
| field | Emby.YouTubePlugin.QuotaTracker._loaded | [51:28](../QuotaTracker.cs#L51) |
| field | Emby.YouTubePlugin.QuotaTracker._saveWorkerRunning | [52:28](../QuotaTracker.cs#L52) |
| field | Emby.YouTubePlugin.QuotaTracker._lastSaveTicks | [53:29](../QuotaTracker.cs#L53) |
| tuple-element | Emby.YouTubePlugin.QuotaTracker.Zone | [56:34](../QuotaTracker.cs#L56) |
| tuple-element | Emby.YouTubePlugin.QuotaTracker.Fallback | [56:53](../QuotaTracker.cs#L56) |
| field | Emby.YouTubePlugin.QuotaTracker.PacificTimeInfo | [56:68](../QuotaTracker.cs#L56) |
| field | Emby.YouTubePlugin.QuotaTracker.PacificTime | [57:46](../QuotaTracker.cs#L57) |
| field | Emby.YouTubePlugin.QuotaTracker.PacificFallback | [58:38](../QuotaTracker.cs#L58) |
| method | Emby.YouTubePlugin.QuotaTracker.RecordCall | [60:9](../QuotaTracker.cs#L60) |
| parameter | Emby.YouTubePlugin.QuotaTracker.RecordCall.url | [60:39](../QuotaTracker.cs#L60) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.RecordCall.charge | [62:17](../QuotaTracker.cs#L62) |
| method | Emby.YouTubePlugin.QuotaTracker.Classify | [87:9](../QuotaTracker.cs#L87) |
| parameter | Emby.YouTubePlugin.QuotaTracker.Classify.url | [87:45](../QuotaTracker.cs#L87) |
| method | Emby.YouTubePlugin.QuotaTracker.GetStats | [98:9](../QuotaTracker.cs#L98) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.GetStats.stats | [101:24](../QuotaTracker.cs#L101) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.GetStats.reset | [102:18](../QuotaTracker.cs#L102) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.GetStats.nextReset | [106:21](../QuotaTracker.cs#L106) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.GetStats.until | [107:21](../QuotaTracker.cs#L107) |
| method | Emby.YouTubePlugin.QuotaTracker.FormatStatus | [125:9](../QuotaTracker.cs#L125) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.FormatStatus.stats | [127:17](../QuotaTracker.cs#L127) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.FormatStatus.searchPct | [128:17](../QuotaTracker.cs#L128) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.FormatStatus.otherPct | [129:17](../QuotaTracker.cs#L129) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.FormatStatus.resetIn | [130:17](../QuotaTracker.cs#L130) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.FormatStatus.tzNote | [133:17](../QuotaTracker.cs#L133) |
| method | Emby.YouTubePlugin.QuotaTracker.Percentage | [143:9](../QuotaTracker.cs#L143) |
| parameter | Emby.YouTubePlugin.QuotaTracker.Percentage.used | [143:42](../QuotaTracker.cs#L143) |
| parameter | Emby.YouTubePlugin.QuotaTracker.Percentage.limit | [143:53](../QuotaTracker.cs#L143) |
| method | Emby.YouTubePlugin.QuotaTracker.BuildBar | [146:9](../QuotaTracker.cs#L146) |
| parameter | Emby.YouTubePlugin.QuotaTracker.BuildBar.pct | [146:40](../QuotaTracker.cs#L146) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.BuildBar.width | [148:23](../QuotaTracker.cs#L148) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.BuildBar.filled | [149:17](../QuotaTracker.cs#L149) |
| method | Emby.YouTubePlugin.QuotaTracker.ResetDayIfNeededLocked | [153:9](../QuotaTracker.cs#L153) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.ResetDayIfNeededLocked.today | [155:17](../QuotaTracker.cs#L155) |
| method | Emby.YouTubePlugin.QuotaTracker.CurrentQuotaDay | [166:9](../QuotaTracker.cs#L166) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.CurrentQuotaDay.ptNow | [168:17](../QuotaTracker.cs#L168) |
| method | Emby.YouTubePlugin.QuotaTracker.ResolvePacific | [172:9](../QuotaTracker.cs#L172) |
| tuple-element | Emby.YouTubePlugin.QuotaTracker.ResolvePacific.Zone | [172:25](../QuotaTracker.cs#L172) |
| tuple-element | Emby.YouTubePlugin.QuotaTracker.ResolvePacific.Fallback | [172:44](../QuotaTracker.cs#L172) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.ResolvePacific.zone | [174:17](../QuotaTracker.cs#L174) |
| method | Emby.YouTubePlugin.QuotaTracker.TryFindTimeZone | [188:9](../QuotaTracker.cs#L188) |
| parameter | Emby.YouTubePlugin.QuotaTracker.TryFindTimeZone.id | [188:54](../QuotaTracker.cs#L188) |
| catch-variable | Emby.YouTubePlugin.QuotaTracker.TryFindTimeZone.ex | [192:19](../QuotaTracker.cs#L192) |
| catch-variable | Emby.YouTubePlugin.QuotaTracker.TryFindTimeZone.ex | [197:19](../QuotaTracker.cs#L197) |
| method | Emby.YouTubePlugin.QuotaTracker.FilePath | [204:9](../QuotaTracker.cs#L204) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.FilePath.dir | [206:17](../QuotaTracker.cs#L206) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.FilePath.parent | [208:17](../QuotaTracker.cs#L208) |
| method | Emby.YouTubePlugin.QuotaTracker.EnsureLoaded | [214:9](../QuotaTracker.cs#L214) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.EnsureLoaded.needsSave | [219:17](../QuotaTracker.cs#L219) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.EnsureLoaded.path | [228:25](../QuotaTracker.cs#L228) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.EnsureLoaded.doc | [234:39](../QuotaTracker.cs#L234) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.EnsureLoaded.root | [235:33](../QuotaTracker.cs#L235) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.EnsureLoaded.schema | [236:33](../QuotaTracker.cs#L236) |
| pattern-variable | Emby.YouTubePlugin.QuotaTracker.EnsureLoaded.dateElement | [239:73](../QuotaTracker.cs#L239) |
| pattern-variable | Emby.YouTubePlugin.QuotaTracker.EnsureLoaded.parsedDate | [240:91](../QuotaTracker.cs#L240) |
| catch-variable | Emby.YouTubePlugin.QuotaTracker.EnsureLoaded.ex | [264:23](../QuotaTracker.cs#L264) |
| method | Emby.YouTubePlugin.QuotaTracker.MoveLegacyStateIfNeeded | [291:9](../QuotaTracker.cs#L291) |
| parameter | Emby.YouTubePlugin.QuotaTracker.MoveLegacyStateIfNeeded.path | [291:53](../QuotaTracker.cs#L291) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.MoveLegacyStateIfNeeded.legacy | [294:17](../QuotaTracker.cs#L294) |
| catch-variable | Emby.YouTubePlugin.QuotaTracker.MoveLegacyStateIfNeeded.ex | [297:19](../QuotaTracker.cs#L297) |
| method | Emby.YouTubePlugin.QuotaTracker.ReadInt64 | [303:9](../QuotaTracker.cs#L303) |
| parameter | Emby.YouTubePlugin.QuotaTracker.ReadInt64.root | [303:39](../QuotaTracker.cs#L303) |
| parameter | Emby.YouTubePlugin.QuotaTracker.ReadInt64.name | [303:57](../QuotaTracker.cs#L303) |
| pattern-variable | Emby.YouTubePlugin.QuotaTracker.ReadInt64.element | [304:47](../QuotaTracker.cs#L304) |
| pattern-variable | Emby.YouTubePlugin.QuotaTracker.ReadInt64.value | [304:87](../QuotaTracker.cs#L304) |
| method | Emby.YouTubePlugin.QuotaTracker.ScheduleSave | [308:9](../QuotaTracker.cs#L308) |
| method | Emby.YouTubePlugin.QuotaTracker.SaveWorkerAsync | [315:9](../QuotaTracker.cs#L315) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.SaveWorkerAsync.lastSave | [321:25](../QuotaTracker.cs#L321) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.SaveWorkerAsync.elapsed | [322:25](../QuotaTracker.cs#L322) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.SaveWorkerAsync.snapshot | [326:35](../QuotaTracker.cs#L326) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.SaveWorkerAsync.version | [327:26](../QuotaTracker.cs#L327) |
| catch-variable | Emby.YouTubePlugin.QuotaTracker.SaveWorkerAsync.ex | [356:19](../QuotaTracker.cs#L356) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.SaveWorkerAsync.dirty | [363:22](../QuotaTracker.cs#L363) |
| method | Emby.YouTubePlugin.QuotaTracker.WriteSnapshot | [371:9](../QuotaTracker.cs#L371) |
| parameter | Emby.YouTubePlugin.QuotaTracker.WriteSnapshot.snapshot | [371:43](../QuotaTracker.cs#L371) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.WriteSnapshot.tempPath | [373:21](../QuotaTracker.cs#L373) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.WriteSnapshot.path | [376:21](../QuotaTracker.cs#L376) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.WriteSnapshot.directory | [378:21](../QuotaTracker.cs#L378) |
| local-variable | Emby.YouTubePlugin.QuotaTracker.WriteSnapshot.json | [382:21](../QuotaTracker.cs#L382) |
| catch-variable | Emby.YouTubePlugin.QuotaTracker.WriteSnapshot.ex | [401:19](../QuotaTracker.cs#L401) |
| catch-variable | Emby.YouTubePlugin.QuotaTracker.WriteSnapshot.ex | [415:27](../QuotaTracker.cs#L415) |

</details>

<a id="file-b335630551682c19"></a>
## [README.md](../README.md)

Documentation/resource; 4849 bytes; 121 lines.

<a id="file-03de83be30141261"></a>
## [YouTubeAdaptiveSourceBudget.cs](../YouTubeAdaptiveSourceBudget.cs)

Plugin Compile source (SDK default); 15996 bytes; 235 lines.

<details>
<summary>All 195 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [12:1](../YouTubeAdaptiveSourceBudget.cs#L12) |
| type | Emby.YouTubePlugin.YouTubeAdaptiveSelection | [14:5](../YouTubeAdaptiveSourceBudget.cs#L14) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSelection.Video | [14:53](../YouTubeAdaptiveSourceBudget.cs#L14) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSelection.Audio | [14:72](../YouTubeAdaptiveSourceBudget.cs#L14) |
| type | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget | [19:5](../YouTubeAdaptiveSourceBudget.cs#L19) |
| field | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.MaximumProbeBytes | [21:28](../YouTubeAdaptiveSourceBudget.cs#L21) |
| field | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.MinimumProbeBytes | [22:28](../YouTubeAdaptiveSourceBudget.cs#L22) |
| field | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ProbeDeadlineMilliseconds | [23:28](../YouTubeAdaptiveSourceBudget.cs#L23) |
| field | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.MaximumCacheEntries | [24:28](../YouTubeAdaptiveSourceBudget.cs#L24) |
| field | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.MaximumPendingProbes | [25:28](../YouTubeAdaptiveSourceBudget.cs#L25) |
| field | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.MaximumParallelProbes | [26:28](../YouTubeAdaptiveSourceBudget.cs#L26) |
| field | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.FallbackMaximumHeight | [27:28](../YouTubeAdaptiveSourceBudget.cs#L27) |
| field | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ThroughputHeadroom | [28:31](../YouTubeAdaptiveSourceBudget.cs#L28) |
| field | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.MaximumFileBytes | [29:28](../YouTubeAdaptiveSourceBudget.cs#L29) |
| field | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Gate | [30:40](../YouTubeAdaptiveSourceBudget.cs#L30) |
| field | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Cache | [31:71](../YouTubeAdaptiveSourceBudget.cs#L31) |
| field | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ProbeSlots | [32:47](../YouTubeAdaptiveSourceBudget.cs#L32) |
| field | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget._pendingProbes | [33:28](../YouTubeAdaptiveSourceBudget.cs#L33) |
| field | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget._clockMilliseconds | [34:35](../YouTubeAdaptiveSourceBudget.cs#L34) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget._clockMilliseconds.anonymous@1547 | [34:56](../YouTubeAdaptiveSourceBudget.cs#L34) |
| field | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget._fetchRange | [35:121](../YouTubeAdaptiveSourceBudget.cs#L35) |
| type | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Input | [38:9](../YouTubeAdaptiveSourceBudget.cs#L38) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Input.Format | [38:37](../YouTubeAdaptiveSourceBudget.cs#L38) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Input.Uri | [38:57](../YouTubeAdaptiveSourceBudget.cs#L38) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Input.Length | [38:66](../YouTubeAdaptiveSourceBudget.cs#L38) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Input.Height | [38:79](../YouTubeAdaptiveSourceBudget.cs#L38) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Input.VideoCodec | [38:91](../YouTubeAdaptiveSourceBudget.cs#L38) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Input.AudioCodec | [39:13](../YouTubeAdaptiveSourceBudget.cs#L39) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Input.Bitrate | [39:32](../YouTubeAdaptiveSourceBudget.cs#L39) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Input.Headers | [39:49](../YouTubeAdaptiveSourceBudget.cs#L39) |
| type | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Choice | [40:9](../YouTubeAdaptiveSourceBudget.cs#L40) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Choice.Video | [40:38](../YouTubeAdaptiveSourceBudget.cs#L40) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Choice.Audio | [40:51](../YouTubeAdaptiveSourceBudget.cs#L40) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Choice.Bitrate | [40:65](../YouTubeAdaptiveSourceBudget.cs#L40) |
| type | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ProbeInput | [41:9](../YouTubeAdaptiveSourceBudget.cs#L41) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ProbeInput.Uri | [41:42](../YouTubeAdaptiveSourceBudget.cs#L41) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ProbeInput.Length | [41:51](../YouTubeAdaptiveSourceBudget.cs#L41) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ProbeInput.Headers | [41:64](../YouTubeAdaptiveSourceBudget.cs#L41) |
| type | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Observation | [42:9](../YouTubeAdaptiveSourceBudget.cs#L42) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Observation.Bitrate | [42:43](../YouTubeAdaptiveSourceBudget.cs#L42) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Observation.Denied | [42:60](../YouTubeAdaptiveSourceBudget.cs#L42) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Observation.ExpiresAt | [42:73](../YouTubeAdaptiveSourceBudget.cs#L42) |
| method | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync | [44:9](../YouTubeAdaptiveSourceBudget.cs#L44) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.resolved | [44:74](../YouTubeAdaptiveSourceBudget.cs#L44) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.headers | [45:13](../YouTubeAdaptiveSourceBudget.cs#L45) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.maximumHeight | [45:58](../YouTubeAdaptiveSourceBudget.cs#L45) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.cancellationToken | [45:77](../YouTubeAdaptiveSourceBudget.cs#L45) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.anonymous@2694 | [50:41](../YouTubeAdaptiveSourceBudget.cs#L50) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.anonymous@2694.c | [50:41](../YouTubeAdaptiveSourceBudget.cs#L50) |
| pattern-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.formats | [52:73](../YouTubeAdaptiveSourceBudget.cs#L52) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.commonHeaders | [56:17](../YouTubeAdaptiveSourceBudget.cs#L56) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.duration | [57:17](../YouTubeAdaptiveSourceBudget.cs#L57) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.unique | [59:17](../YouTubeAdaptiveSourceBudget.cs#L59) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.unique.anonymous@3384 | [59:59](../YouTubeAdaptiveSourceBudget.cs#L59) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.unique.anonymous@3384.f | [59:59](../YouTubeAdaptiveSourceBudget.cs#L59) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.unique.anonymous@3458 | [60:24](../YouTubeAdaptiveSourceBudget.cs#L60) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.unique.anonymous@3458.group | [60:24](../YouTubeAdaptiveSourceBudget.cs#L60) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.unique.anonymous@3494 | [60:60](../YouTubeAdaptiveSourceBudget.cs#L60) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.unique.anonymous@3494.group | [60:60](../YouTubeAdaptiveSourceBudget.cs#L60) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.inputs | [61:17](../YouTubeAdaptiveSourceBudget.cs#L61) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.inputs.anonymous@3558 | [61:40](../YouTubeAdaptiveSourceBudget.cs#L61) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.inputs.anonymous@3558.format | [61:40](../YouTubeAdaptiveSourceBudget.cs#L61) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.inputs.anonymous@3656 | [62:24](../YouTubeAdaptiveSourceBudget.cs#L62) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.inputs.anonymous@3656.input | [62:24](../YouTubeAdaptiveSourceBudget.cs#L62) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.audio | [63:17](../YouTubeAdaptiveSourceBudget.cs#L63) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.audio.anonymous@3742 | [63:38](../YouTubeAdaptiveSourceBudget.cs#L63) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.audio.anonymous@3742.input | [63:38](../YouTubeAdaptiveSourceBudget.cs#L63) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.audio.anonymous@3845 | [64:36](../YouTubeAdaptiveSourceBudget.cs#L64) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.audio.anonymous@3845.input | [64:36](../YouTubeAdaptiveSourceBudget.cs#L64) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.audio.anonymous@3938 | [65:35](../YouTubeAdaptiveSourceBudget.cs#L65) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.audio.anonymous@3938.input | [65:35](../YouTubeAdaptiveSourceBudget.cs#L65) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.audio.anonymous@3991 | [66:25](../YouTubeAdaptiveSourceBudget.cs#L66) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.audio.anonymous@3991.input | [66:25](../YouTubeAdaptiveSourceBudget.cs#L66) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.choices | [67:17](../YouTubeAdaptiveSourceBudget.cs#L67) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.choices.anonymous@4114 | [67:40](../YouTubeAdaptiveSourceBudget.cs#L67) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.choices.anonymous@4114.input | [67:40](../YouTubeAdaptiveSourceBudget.cs#L67) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.choices.anonymous@4329 | [69:25](../YouTubeAdaptiveSourceBudget.cs#L69) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.choices.anonymous@4329.input | [69:25](../YouTubeAdaptiveSourceBudget.cs#L69) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.codec | [74:17](../YouTubeAdaptiveSourceBudget.cs#L74) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.codec.anonymous@4763 | [74:37](../YouTubeAdaptiveSourceBudget.cs#L74) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.codec.anonymous@4763.choice | [74:37](../YouTubeAdaptiveSourceBudget.cs#L74) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.eligible | [75:17](../YouTubeAdaptiveSourceBudget.cs#L75) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.eligible.anonymous@4857 | [75:42](../YouTubeAdaptiveSourceBudget.cs#L75) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.eligible.anonymous@4857.choice | [75:42](../YouTubeAdaptiveSourceBudget.cs#L75) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.eligible.anonymous@4953 | [76:36](../YouTubeAdaptiveSourceBudget.cs#L76) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.eligible.anonymous@4953.choice | [76:36](../YouTubeAdaptiveSourceBudget.cs#L76) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.eligible.anonymous@5018 | [77:35](../YouTubeAdaptiveSourceBudget.cs#L77) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.eligible.anonymous@5018.choice | [77:35](../YouTubeAdaptiveSourceBudget.cs#L77) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.eligible.anonymous@5156 | [78:35](../YouTubeAdaptiveSourceBudget.cs#L78) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.eligible.anonymous@5156.choice | [78:35](../YouTubeAdaptiveSourceBudget.cs#L78) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.eligible.anonymous@5231 | [79:25](../YouTubeAdaptiveSourceBudget.cs#L79) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.eligible.anonymous@5231.choice | [79:25](../YouTubeAdaptiveSourceBudget.cs#L79) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.eligible.anonymous@5287 | [80:25](../YouTubeAdaptiveSourceBudget.cs#L80) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.eligible.anonymous@5287.choice | [80:25](../YouTubeAdaptiveSourceBudget.cs#L80) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.eligible.anonymous@5356 | [81:25](../YouTubeAdaptiveSourceBudget.cs#L81) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.eligible.anonymous@5356.choice | [81:25](../YouTubeAdaptiveSourceBudget.cs#L81) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.lowest | [82:17](../YouTubeAdaptiveSourceBudget.cs#L82) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.lowest.anonymous@5483 | [82:43](../YouTubeAdaptiveSourceBudget.cs#L82) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.lowest.anonymous@5483.choice | [82:43](../YouTubeAdaptiveSourceBudget.cs#L82) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.lowest.anonymous@5538 | [83:25](../YouTubeAdaptiveSourceBudget.cs#L83) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.lowest.anonymous@5538.choice | [83:25](../YouTubeAdaptiveSourceBudget.cs#L83) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.fallback | [84:17](../YouTubeAdaptiveSourceBudget.cs#L84) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.fallback.anonymous@5643 | [84:52](../YouTubeAdaptiveSourceBudget.cs#L84) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.fallback.anonymous@5643.choice | [84:52](../YouTubeAdaptiveSourceBudget.cs#L84) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.preferred | [87:17](../YouTubeAdaptiveSourceBudget.cs#L87) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.observation | [88:17](../YouTubeAdaptiveSourceBudget.cs#L88) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.selected | [91:17](../YouTubeAdaptiveSourceBudget.cs#L91) |
| pattern-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.selected.budget | [91:55](../YouTubeAdaptiveSourceBudget.cs#L91) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.selected.anonymous@6275 | [92:43](../YouTubeAdaptiveSourceBudget.cs#L92) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.selected.anonymous@6275.choice | [92:43](../YouTubeAdaptiveSourceBudget.cs#L92) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.selected.anonymous@6460 | [94:39](../YouTubeAdaptiveSourceBudget.cs#L94) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.selected.anonymous@6460.choice | [94:39](../YouTubeAdaptiveSourceBudget.cs#L94) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.selected.anonymous@6501 | [94:80](../YouTubeAdaptiveSourceBudget.cs#L94) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.selected.anonymous@6501.choice | [94:80](../YouTubeAdaptiveSourceBudget.cs#L94) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.selected.anonymous@6559 | [95:33](../YouTubeAdaptiveSourceBudget.cs#L95) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.SelectAsync.selected.anonymous@6559.choice | [95:33](../YouTubeAdaptiveSourceBudget.cs#L95) |
| method | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput | [106:9](../YouTubeAdaptiveSourceBudget.cs#L106) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.format | [106:41](../YouTubeAdaptiveSourceBudget.cs#L106) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.headers | [106:61](../YouTubeAdaptiveSourceBudget.cs#L106) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.duration | [107:13](../YouTubeAdaptiveSourceBudget.cs#L107) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.expiresUtc | [107:31](../YouTubeAdaptiveSourceBudget.cs#L107) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.id | [110:20](../YouTubeAdaptiveSourceBudget.cs#L110) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.extension | [110:52](../YouTubeAdaptiveSourceBudget.cs#L110) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.anonymous@7663 | [111:50](../YouTubeAdaptiveSourceBudget.cs#L111) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.anonymous@7663.c | [111:50](../YouTubeAdaptiveSourceBudget.cs#L111) |
| pattern-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.size | [113:63](../YouTubeAdaptiveSourceBudget.cs#L113) |
| pattern-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.length | [114:47](../YouTubeAdaptiveSourceBudget.cs#L114) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.video | [115:20](../YouTubeAdaptiveSourceBudget.cs#L115) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.audio | [115:52](../YouTubeAdaptiveSourceBudget.cs#L115) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.videoCodec | [116:20](../YouTubeAdaptiveSourceBudget.cs#L116) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.audioCodec | [117:20](../YouTubeAdaptiveSourceBudget.cs#L117) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.height | [119:18](../YouTubeAdaptiveSourceBudget.cs#L119) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.width | [119:54](../YouTubeAdaptiveSourceBudget.cs#L119) |
| iteration-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.field | [121:13](../YouTubeAdaptiveSourceBudget.cs#L121) |
| pattern-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.field.value | [122:58](../YouTubeAdaptiveSourceBudget.cs#L122) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.uri | [125:17](../YouTubeAdaptiveSourceBudget.cs#L125) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.declared | [126:19](../YouTubeAdaptiveSourceBudget.cs#L126) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.expiry | [126:36](../YouTubeAdaptiveSourceBudget.cs#L126) |
| iteration-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.part | [127:13](../YouTubeAdaptiveSourceBudget.cs#L127) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.part.pieces | [129:21](../YouTubeAdaptiveSourceBudget.cs#L129) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.part.name | [130:24](../YouTubeAdaptiveSourceBudget.cs#L130) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.part.anonymous@9641 | [133:38](../YouTubeAdaptiveSourceBudget.cs#L133) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.part.anonymous@9641.c | [133:38](../YouTubeAdaptiveSourceBudget.cs#L133) |
| pattern-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.part.parsed | [134:108](../YouTubeAdaptiveSourceBudget.cs#L134) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.safeHeaders | [150:17](../YouTubeAdaptiveSourceBudget.cs#L150) |
| pattern-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.local | [151:63](../YouTubeAdaptiveSourceBudget.cs#L151) |
| iteration-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.header | [152:17](../YouTubeAdaptiveSourceBudget.cs#L152) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.ReadInput.bitrate | [156:21](../YouTubeAdaptiveSourceBudget.cs#L156) |
| method | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation | [162:9](../YouTubeAdaptiveSourceBudget.cs#L162) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation.input | [162:57](../YouTubeAdaptiveSourceBudget.cs#L162) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation.route | [164:20](../YouTubeAdaptiveSourceBudget.cs#L164) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation.identity | [166:20](../YouTubeAdaptiveSourceBudget.cs#L166) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation.identity.anonymous@11656 | [166:86](../YouTubeAdaptiveSourceBudget.cs#L166) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation.identity.anonymous@11656.pair | [166:86](../YouTubeAdaptiveSourceBudget.cs#L166) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation.identity.anonymous@11732 | [167:25](../YouTubeAdaptiveSourceBudget.cs#L167) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation.identity.anonymous@11732.pair | [167:25](../YouTubeAdaptiveSourceBudget.cs#L167) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation.key | [168:17](../YouTubeAdaptiveSourceBudget.cs#L168) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation.now | [171:22](../YouTubeAdaptiveSourceBudget.cs#L171) |
| iteration-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation.entry | [172:17](../YouTubeAdaptiveSourceBudget.cs#L172) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation.entry.anonymous@12021 | [172:51](../YouTubeAdaptiveSourceBudget.cs#L172) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation.entry.anonymous@12021.entry | [172:51](../YouTubeAdaptiveSourceBudget.cs#L172) |
| pattern-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation.existing | [174:52](../YouTubeAdaptiveSourceBudget.cs#L174) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation.oldest | [177:25](../YouTubeAdaptiveSourceBudget.cs#L177) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation.oldest.anonymous@12360 | [177:46](../YouTubeAdaptiveSourceBudget.cs#L177) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation.oldest.anonymous@12360.entry | [177:46](../YouTubeAdaptiveSourceBudget.cs#L177) |
| anonymous-function | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation.oldest.anonymous@12439 | [178:34](../YouTubeAdaptiveSourceBudget.cs#L178) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation.oldest.anonymous@12439.entry | [178:34](../YouTubeAdaptiveSourceBudget.cs#L178) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.GetObservation.pending | [182:35](../YouTubeAdaptiveSourceBudget.cs#L182) |
| method | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.MeasureAsync | [196:9](../YouTubeAdaptiveSourceBudget.cs#L196) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.MeasureAsync.input | [196:61](../YouTubeAdaptiveSourceBudget.cs#L196) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.MeasureAsync.acquired | [198:18](../YouTubeAdaptiveSourceBudget.cs#L198) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.MeasureAsync.deadline | [199:23](../YouTubeAdaptiveSourceBudget.cs#L199) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.MeasureAsync.count | [205:21](../YouTubeAdaptiveSourceBudget.cs#L205) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.MeasureAsync.watch | [207:21](../YouTubeAdaptiveSourceBudget.cs#L207) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.MeasureAsync.bytes | [208:21](../YouTubeAdaptiveSourceBudget.cs#L208) |
| local-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.MeasureAsync.rate | [212:24](../YouTubeAdaptiveSourceBudget.cs#L212) |
| catch-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.MeasureAsync.error | [216:19](../YouTubeAdaptiveSourceBudget.cs#L216) |
| method | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Fallback | [224:9](../YouTubeAdaptiveSourceBudget.cs#L224) |
| method | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.CodecPreference | [225:9](../YouTubeAdaptiveSourceBudget.cs#L225) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.CodecPreference.codec | [225:44](../YouTubeAdaptiveSourceBudget.cs#L225) |
| method | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Text | [226:9](../YouTubeAdaptiveSourceBudget.cs#L226) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Text.value | [226:36](../YouTubeAdaptiveSourceBudget.cs#L226) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Text.field | [226:55](../YouTubeAdaptiveSourceBudget.cs#L226) |
| pattern-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Text.item | [227:52](../YouTubeAdaptiveSourceBudget.cs#L227) |
| method | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Number | [228:9](../YouTubeAdaptiveSourceBudget.cs#L228) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Number.value | [228:39](../YouTubeAdaptiveSourceBudget.cs#L228) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Number.field | [228:58](../YouTubeAdaptiveSourceBudget.cs#L228) |
| pattern-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Number.item | [229:52](../YouTubeAdaptiveSourceBudget.cs#L229) |
| pattern-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Number.number | [230:42](../YouTubeAdaptiveSourceBudget.cs#L230) |
| method | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Integer | [231:9](../YouTubeAdaptiveSourceBudget.cs#L231) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Integer.value | [231:37](../YouTubeAdaptiveSourceBudget.cs#L231) |
| parameter | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Integer.field | [231:56](../YouTubeAdaptiveSourceBudget.cs#L231) |
| pattern-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Integer.item | [232:52](../YouTubeAdaptiveSourceBudget.cs#L232) |
| pattern-variable | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Integer.number | [232:128](../YouTubeAdaptiveSourceBudget.cs#L232) |
| method | Emby.YouTubePlugin.YouTubeAdaptiveSourceBudget.Failure | [233:9](../YouTubeAdaptiveSourceBudget.cs#L233) |

</details>

<a id="file-7011a74936995935"></a>
## [YouTubeApi.cs](../YouTubeApi.cs)

Plugin Compile source (SDK default); 54112 bytes; 1248 lines.

<details>
<summary>All 377 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [15:1](../YouTubeApi.cs#L15) |
| type | Emby.YouTubePlugin.YouTubeApi | [17:5](../YouTubeApi.cs#L17) |
| field | Emby.YouTubePlugin.YouTubeApi.ApiBase | [19:30](../YouTubeApi.cs#L19) |
| type | Emby.YouTubePlugin.YouTubeApi.CachedResponse | [23:9](../YouTubeApi.cs#L23) |
| parameter | Emby.YouTubePlugin.YouTubeApi.CachedResponse.Json | [24:13](../YouTubeApi.cs#L24) |
| parameter | Emby.YouTubePlugin.YouTubeApi.CachedResponse.CachedAtMs | [25:13](../YouTubeApi.cs#L25) |
| parameter | Emby.YouTubePlugin.YouTubeApi.CachedResponse.ExpiresAtMs | [26:13](../YouTubeApi.cs#L26) |
| parameter | Emby.YouTubePlugin.YouTubeApi.CachedResponse.CacheTag | [27:13](../YouTubeApi.cs#L27) |
| type | Emby.YouTubePlugin.YouTubeApi.DiskCacheHit | [28:9](../YouTubeApi.cs#L28) |
| parameter | Emby.YouTubePlugin.YouTubeApi.DiskCacheHit.Json | [28:37](../YouTubeApi.cs#L28) |
| parameter | Emby.YouTubePlugin.YouTubeApi.DiskCacheHit.AgeMs | [28:50](../YouTubeApi.cs#L28) |
| field | Emby.YouTubePlugin.YouTubeApi.ResponseCache | [29:78](../YouTubeApi.cs#L29) |
| field | Emby.YouTubePlugin.YouTubeApi.InFlightRequests | [30:85](../YouTubeApi.cs#L30) |
| field | Emby.YouTubePlugin.YouTubeApi.CacheMutationLock | [31:40](../YouTubeApi.cs#L31) |
| field | Emby.YouTubePlugin.YouTubeApi._supportedContentRegions | [32:41](../YouTubeApi.cs#L32) |
| field | Emby.YouTubePlugin.YouTubeApi._cacheGeneration | [33:29](../YouTubeApi.cs#L33) |
| field | Emby.YouTubePlugin.YouTubeApi.MaxCacheEntries | [34:27](../YouTubeApi.cs#L34) |
| field | Emby.YouTubePlugin.YouTubeApi.CacheTtlMs | [38:28](../YouTubeApi.cs#L38) |
| field | Emby.YouTubePlugin.YouTubeApi.FreshListTtlMs | [39:28](../YouTubeApi.cs#L39) |
| field | Emby.YouTubePlugin.YouTubeApi.ChannelDetailsCacheTtlMs | [40:28](../YouTubeApi.cs#L40) |
| field | Emby.YouTubePlugin.YouTubeApi.SearchTtlMs | [41:28](../YouTubeApi.cs#L41) |
| field | Emby.YouTubePlugin.YouTubeApi.CategoriesTtlMs | [42:28](../YouTubeApi.cs#L42) |
| field | Emby.YouTubePlugin.YouTubeApi.VideoDetailTtlMs | [45:28](../YouTubeApi.cs#L45) |
| field | Emby.YouTubePlugin.YouTubeApi.DiskCacheTtlMs | [49:28](../YouTubeApi.cs#L49) |
| field | Emby.YouTubePlugin.YouTubeApi.CacheKeySchemaPrefix | [50:30](../YouTubeApi.cs#L50) |
| field | Emby.YouTubePlugin.YouTubeApi.PlaylistCacheTagPrefix | [51:30](../YouTubeApi.cs#L51) |
| field | Emby.YouTubePlugin.YouTubeApi._lastDiskCleanupUtcTicks | [52:29](../YouTubeApi.cs#L52) |
| field | Emby.YouTubePlugin.YouTubeApi._diskCleanupRunning | [53:28](../YouTubeApi.cs#L53) |
| field | Emby.YouTubePlugin.YouTubeApi.Http | [55:44](../YouTubeApi.cs#L55) |
| field | Emby.YouTubePlugin.YouTubeApi.RetryDelaysMs | [71:39](../YouTubeApi.cs#L71) |
| field | Emby.YouTubePlugin.YouTubeApi.RequestAttemptTimeout | [72:42](../YouTubeApi.cs#L72) |
| field | Emby.YouTubePlugin.YouTubeApi.ApiGate | [77:47](../YouTubeApi.cs#L77) |
| field | Emby.YouTubePlugin.YouTubeApi.RequestStartGate | [78:47](../YouTubeApi.cs#L78) |
| field | Emby.YouTubePlugin.YouTubeApi._lastCallTicks | [79:29](../YouTubeApi.cs#L79) |
| field | Emby.YouTubePlugin.YouTubeApi.MinCallIntervalMs | [80:27](../YouTubeApi.cs#L80) |
| field | Emby.YouTubePlugin.YouTubeApi._requestTimestamps | [82:45](../YouTubeApi.cs#L82) |
| field | Emby.YouTubePlugin.YouTubeApi._budgetLock | [83:40](../YouTubeApi.cs#L83) |
| field | Emby.YouTubePlugin.YouTubeApi.MaxRequestsPerWindow | [86:27](../YouTubeApi.cs#L86) |
| field | Emby.YouTubePlugin.YouTubeApi.BudgetWindowMs | [87:27](../YouTubeApi.cs#L87) |
| method | Emby.YouTubePlugin.YouTubeApi.AcquireGateAsync | [92:9](../YouTubeApi.cs#L92) |
| parameter | Emby.YouTubePlugin.YouTubeApi.AcquireGateAsync.ct | [92:65](../YouTubeApi.cs#L92) |
| type | Emby.YouTubePlugin.YouTubeApi.GateLease | [107:9](../YouTubeApi.cs#L107) |
| field | Emby.YouTubePlugin.YouTubeApi.GateLease._disposed | [109:25](../YouTubeApi.cs#L109) |
| method | Emby.YouTubePlugin.YouTubeApi.GateLease.Dispose | [110:13](../YouTubeApi.cs#L110) |
| type | Emby.YouTubePlugin.YouTubeApi.SharedInFlightRequest | [121:9](../YouTubeApi.cs#L121) |
| field | Emby.YouTubePlugin.YouTubeApi.SharedInFlightRequest._sync | [123:37](../YouTubeApi.cs#L123) |
| field | Emby.YouTubePlugin.YouTubeApi.SharedInFlightRequest._cancellation | [124:54](../YouTubeApi.cs#L124) |
| field | Emby.YouTubePlugin.YouTubeApi.SharedInFlightRequest._work | [125:50](../YouTubeApi.cs#L125) |
| field | Emby.YouTubePlugin.YouTubeApi.SharedInFlightRequest._waiters | [126:25](../YouTubeApi.cs#L126) |
| field | Emby.YouTubePlugin.YouTubeApi.SharedInFlightRequest._abandoned | [127:26](../YouTubeApi.cs#L127) |
| field | Emby.YouTubePlugin.YouTubeApi.SharedInFlightRequest._completed | [128:26](../YouTubeApi.cs#L128) |
| constructor | Emby.YouTubePlugin.YouTubeApi.SharedInFlightRequest.SharedInFlightRequest | [130:13](../YouTubeApi.cs#L130) |
| parameter | Emby.YouTubePlugin.YouTubeApi.SharedInFlightRequest.SharedInFlightRequest.factory | [131:17](../YouTubeApi.cs#L131) |
| parameter | Emby.YouTubePlugin.YouTubeApi.SharedInFlightRequest.SharedInFlightRequest.onCompleted | [132:17](../YouTubeApi.cs#L132) |
| anonymous-function | Emby.YouTubePlugin.YouTubeApi.SharedInFlightRequest.SharedInFlightRequest.anonymous@6143 | [135:21](../YouTubeApi.cs#L135) |
| property | Emby.YouTubePlugin.YouTubeApi.SharedInFlightRequest.Work | [158:13](../YouTubeApi.cs#L158) |
| method | Emby.YouTubePlugin.YouTubeApi.SharedInFlightRequest.TryAddWaiter | [160:13](../YouTubeApi.cs#L160) |
| method | Emby.YouTubePlugin.YouTubeApi.SharedInFlightRequest.ReleaseWaiterAndAbandonIfUnused | [172:13](../YouTubeApi.cs#L172) |
| method | Emby.YouTubePlugin.YouTubeApi.SharedInFlightRequest.Cancel | [188:13](../YouTubeApi.cs#L188) |
| method | Emby.YouTubePlugin.YouTubeApi.ReserveRequestStartAsync | [195:9](../YouTubeApi.cs#L195) |
| parameter | Emby.YouTubePlugin.YouTubeApi.ReserveRequestStartAsync.ct | [195:60](../YouTubeApi.cs#L195) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.ReserveRequestStartAsync.waitMs | [203:26](../YouTubeApi.cs#L203) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.ReserveRequestStartAsync.now | [206:29](../YouTubeApi.cs#L206) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.ReserveRequestStartAsync.spacingWait | [213:29](../YouTubeApi.cs#L213) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.ReserveRequestStartAsync.budgetWait | [216:29](../YouTubeApi.cs#L216) |
| method | Emby.YouTubePlugin.YouTubeApi.IsTransientError | [239:9](../YouTubeApi.cs#L239) |
| parameter | Emby.YouTubePlugin.YouTubeApi.IsTransientError.code | [239:46](../YouTubeApi.cs#L239) |
| method | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync | [247:9](../YouTubeApi.cs#L247) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.url | [247:62](../YouTubeApi.cs#L247) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.ct | [247:74](../YouTubeApi.cs#L247) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.lastStatus | [249:29](../YouTubeApi.cs#L249) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.lastException | [250:24](../YouTubeApi.cs#L250) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.attempt | [252:22](../YouTubeApi.cs#L252) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.overrideDelayMs | [254:22](../YouTubeApi.cs#L254) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.shouldRetry | [255:21](../YouTubeApi.cs#L255) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.attemptCts | [267:35](../YouTubeApi.cs#L267) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.attemptToken | [269:29](../YouTubeApi.cs#L269) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.resp | [271:35](../YouTubeApi.cs#L271) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.stream | [280:45](../YouTubeApi.cs#L280) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.ra | [292:33](../YouTubeApi.cs#L292) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.delay | [293:33](../YouTubeApi.cs#L293) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.delta | [294:55](../YouTubeApi.cs#L294) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.when | [296:65](../YouTubeApi.cs#L296) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.ex | [309:23](../YouTubeApi.cs#L309) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.ex | [316:23](../YouTubeApi.cs#L316) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.ex | [324:23](../YouTubeApi.cs#L324) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.ex | [330:23](../YouTubeApi.cs#L330) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.ex | [336:23](../YouTubeApi.cs#L336) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.nextDelay | [346:21](../YouTubeApi.cs#L346) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetJsonAsync.message | [351:17](../YouTubeApi.cs#L351) |
| method | Emby.YouTubePlugin.YouTubeApi.TryGetJsonAsync | [357:9](../YouTubeApi.cs#L357) |
| parameter | Emby.YouTubePlugin.YouTubeApi.TryGetJsonAsync.url | [357:66](../YouTubeApi.cs#L357) |
| parameter | Emby.YouTubePlugin.YouTubeApi.TryGetJsonAsync.ct | [357:78](../YouTubeApi.cs#L357) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.TryGetJsonAsync.ex | [367:19](../YouTubeApi.cs#L367) |
| method | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync | [374:9](../YouTubeApi.cs#L374) |
| parameter | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.url | [375:13](../YouTubeApi.cs#L375) |
| parameter | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.ct | [376:13](../YouTubeApi.cs#L376) |
| parameter | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.customTtlMs | [377:13](../YouTubeApi.cs#L377) |
| parameter | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.cacheTag | [378:13](../YouTubeApi.cs#L378) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.memTtl | [381:17](../YouTubeApi.cs#L381) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.diskTtl | [382:17](../YouTubeApi.cs#L382) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.now | [383:17](../YouTubeApi.cs#L383) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.cached | [386:56](../YouTubeApi.cs#L386) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.ex | [391:27](../YouTubeApi.cs#L391) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.diskReadGeneration | [404:17](../YouTubeApi.cs#L404) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.diskHit | [405:17](../YouTubeApi.cs#L405) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.remainingMs | [410:21](../YouTubeApi.cs#L410) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.promoted | [411:21](../YouTubeApi.cs#L411) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.cacheGeneration | [432:17](../YouTubeApi.cs#L432) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.inFlightKey | [433:17](../YouTubeApi.cs#L433) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.pending | [434:35](../YouTubeApi.cs#L434) |
| anonymous-function | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.anonymous@18674 | [439:21](../YouTubeApi.cs#L439) |
| parameter | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.anonymous@18674._ | [439:21](../YouTubeApi.cs#L439) |
| anonymous-function | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.anonymous@18674.anonymous@18730 | [440:25](../YouTubeApi.cs#L440) |
| parameter | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.anonymous@18674.anonymous@18730.cancellationToken | [440:25](../YouTubeApi.cs#L440) |
| anonymous-function | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.anonymous@18674.anonymous@18999 | [446:25](../YouTubeApi.cs#L446) |
| parameter | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.anonymous@18674.anonymous@18999.completed | [446:25](../YouTubeApi.cs#L446) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.freshJson | [453:21](../YouTubeApi.cs#L453) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.staleReadGeneration | [472:17](../YouTubeApi.cs#L472) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.staleHit | [473:17](../YouTubeApi.cs#L473) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.promoted | [479:21](../YouTubeApi.cs#L479) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryGetCachedJsonAsync.staleCachedAt | [484:29](../YouTubeApi.cs#L484) |
| method | Emby.YouTubePlugin.YouTubeApi.GetInFlightKey | [501:9](../YouTubeApi.cs#L501) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetInFlightKey.url | [502:13](../YouTubeApi.cs#L502) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetInFlightKey.ttlMs | [503:13](../YouTubeApi.cs#L503) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetInFlightKey.cacheTag | [504:13](../YouTubeApi.cs#L504) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetInFlightKey.cacheGeneration | [505:13](../YouTubeApi.cs#L505) |
| method | Emby.YouTubePlugin.YouTubeApi.RemoveInFlightRequest | [508:9](../YouTubeApi.cs#L508) |
| parameter | Emby.YouTubePlugin.YouTubeApi.RemoveInFlightRequest.inFlightKey | [509:13](../YouTubeApi.cs#L509) |
| parameter | Emby.YouTubePlugin.YouTubeApi.RemoveInFlightRequest.request | [510:13](../YouTubeApi.cs#L510) |
| method | Emby.YouTubePlugin.YouTubeApi.FetchAndCacheJsonAsync | [514:9](../YouTubeApi.cs#L514) |
| parameter | Emby.YouTubePlugin.YouTubeApi.FetchAndCacheJsonAsync.url | [515:13](../YouTubeApi.cs#L515) |
| parameter | Emby.YouTubePlugin.YouTubeApi.FetchAndCacheJsonAsync.memTtl | [516:13](../YouTubeApi.cs#L516) |
| parameter | Emby.YouTubePlugin.YouTubeApi.FetchAndCacheJsonAsync.cacheTag | [517:13](../YouTubeApi.cs#L517) |
| parameter | Emby.YouTubePlugin.YouTubeApi.FetchAndCacheJsonAsync.cacheGeneration | [518:13](../YouTubeApi.cs#L518) |
| parameter | Emby.YouTubePlugin.YouTubeApi.FetchAndCacheJsonAsync.cancellationToken | [519:13](../YouTubeApi.cs#L519) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.FetchAndCacheJsonAsync.doc | [521:23](../YouTubeApi.cs#L521) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.FetchAndCacheJsonAsync.json | [525:17](../YouTubeApi.cs#L525) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.FetchAndCacheJsonAsync.cachedAt | [530:25](../YouTubeApi.cs#L530) |
| method | Emby.YouTubePlugin.YouTubeApi.GetDiskCacheKey | [545:9](../YouTubeApi.cs#L545) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetDiskCacheKey.url | [545:47](../YouTubeApi.cs#L545) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetDiskCacheKey.bytes | [547:17](../YouTubeApi.cs#L547) |
| method | Emby.YouTubePlugin.YouTubeApi.GetDiskCacheTagKey | [551:9](../YouTubeApi.cs#L551) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetDiskCacheTagKey.cacheTag | [551:50](../YouTubeApi.cs#L551) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetDiskCacheTagKey.bytes | [556:17](../YouTubeApi.cs#L556) |
| method | Emby.YouTubePlugin.YouTubeApi.GetDiskCacheFile | [560:9](../YouTubeApi.cs#L560) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetDiskCacheFile.cacheDir | [560:48](../YouTubeApi.cs#L560) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetDiskCacheFile.url | [560:65](../YouTubeApi.cs#L560) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetDiskCacheFile.cacheTag | [560:77](../YouTubeApi.cs#L560) |
| method | Emby.YouTubePlugin.YouTubeApi.GetPlaylistCacheTag | [565:9](../YouTubeApi.cs#L565) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetPlaylistCacheTag.playlistId | [565:51](../YouTubeApi.cs#L565) |
| method | Emby.YouTubePlugin.YouTubeApi.TryReadDiskCache | [568:9](../YouTubeApi.cs#L568) |
| parameter | Emby.YouTubePlugin.YouTubeApi.TryReadDiskCache.url | [568:55](../YouTubeApi.cs#L568) |
| parameter | Emby.YouTubePlugin.YouTubeApi.TryReadDiskCache.ttlMs | [568:67](../YouTubeApi.cs#L568) |
| parameter | Emby.YouTubePlugin.YouTubeApi.TryReadDiskCache.cacheTag | [568:79](../YouTubeApi.cs#L568) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryReadDiskCache.file | [570:21](../YouTubeApi.cs#L570) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryReadDiskCache.cacheDir | [573:21](../YouTubeApi.cs#L573) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryReadDiskCache.lastWrite | [580:21](../YouTubeApi.cs#L580) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryReadDiskCache.ageMs | [581:21](../YouTubeApi.cs#L581) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryReadDiskCache.json | [584:21](../YouTubeApi.cs#L584) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.TryReadDiskCache.parsed | [587:28](../YouTubeApi.cs#L587) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.TryReadDiskCache.ex | [594:19](../YouTubeApi.cs#L594) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.TryReadDiskCache.ex | [600:19](../YouTubeApi.cs#L600) |
| method | Emby.YouTubePlugin.YouTubeApi.WriteDiskCache | [607:9](../YouTubeApi.cs#L607) |
| parameter | Emby.YouTubePlugin.YouTubeApi.WriteDiskCache.url | [607:44](../YouTubeApi.cs#L607) |
| parameter | Emby.YouTubePlugin.YouTubeApi.WriteDiskCache.json | [607:56](../YouTubeApi.cs#L607) |
| parameter | Emby.YouTubePlugin.YouTubeApi.WriteDiskCache.cacheTag | [607:69](../YouTubeApi.cs#L607) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.WriteDiskCache.tempFile | [609:21](../YouTubeApi.cs#L609) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.WriteDiskCache.cacheDir | [612:21](../YouTubeApi.cs#L612) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.WriteDiskCache.file | [616:21](../YouTubeApi.cs#L616) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.WriteDiskCache.ex | [622:19](../YouTubeApi.cs#L622) |
| method | Emby.YouTubePlugin.YouTubeApi.MaybeRunDiskCleanup | [632:9](../YouTubeApi.cs#L632) |
| parameter | Emby.YouTubePlugin.YouTubeApi.MaybeRunDiskCleanup.cacheDir | [632:49](../YouTubeApi.cs#L632) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.MaybeRunDiskCleanup.nowTicks | [634:17](../YouTubeApi.cs#L634) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.MaybeRunDiskCleanup.lastTicks | [635:17](../YouTubeApi.cs#L635) |
| anonymous-function | Emby.YouTubePlugin.YouTubeApi.MaybeRunDiskCleanup.anonymous@26838 | [642:22](../YouTubeApi.cs#L642) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.MaybeRunDiskCleanup.anonymous@26838.cutoff | [646:25](../YouTubeApi.cs#L646) |
| iteration-variable | Emby.YouTubePlugin.YouTubeApi.MaybeRunDiskCleanup.anonymous@26838.file | [647:21](../YouTubeApi.cs#L647) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.MaybeRunDiskCleanup.anonymous@26838.file.ex | [654:31](../YouTubeApi.cs#L654) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.MaybeRunDiskCleanup.anonymous@26838.tempCutoff | [659:25](../YouTubeApi.cs#L659) |
| iteration-variable | Emby.YouTubePlugin.YouTubeApi.MaybeRunDiskCleanup.anonymous@26838.file | [660:21](../YouTubeApi.cs#L660) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.MaybeRunDiskCleanup.anonymous@26838.file.ex | [667:31](../YouTubeApi.cs#L667) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.MaybeRunDiskCleanup.anonymous@26838.ex | [674:23](../YouTubeApi.cs#L674) |
| method | Emby.YouTubePlugin.YouTubeApi.TryDeleteCacheFile | [685:9](../YouTubeApi.cs#L685) |
| parameter | Emby.YouTubePlugin.YouTubeApi.TryDeleteCacheFile.file | [685:48](../YouTubeApi.cs#L685) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.TryDeleteCacheFile.ex | [693:19](../YouTubeApi.cs#L693) |
| method | Emby.YouTubePlugin.YouTubeApi.EvictCacheIfNeeded | [699:9](../YouTubeApi.cs#L699) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.EvictCacheIfNeeded.now | [702:17](../YouTubeApi.cs#L702) |
| iteration-variable | Emby.YouTubePlugin.YouTubeApi.EvictCacheIfNeeded.kvp | [705:13](../YouTubeApi.cs#L705) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.EvictCacheIfNeeded.overflow | [715:21](../YouTubeApi.cs#L715) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.EvictCacheIfNeeded.oldest | [716:21](../YouTubeApi.cs#L716) |
| anonymous-function | Emby.YouTubePlugin.YouTubeApi.EvictCacheIfNeeded.oldest.anonymous@29743 | [717:30](../YouTubeApi.cs#L717) |
| parameter | Emby.YouTubePlugin.YouTubeApi.EvictCacheIfNeeded.oldest.anonymous@29743.kvp | [717:30](../YouTubeApi.cs#L717) |
| anonymous-function | Emby.YouTubePlugin.YouTubeApi.EvictCacheIfNeeded.oldest.anonymous@29836 | [719:29](../YouTubeApi.cs#L719) |
| parameter | Emby.YouTubePlugin.YouTubeApi.EvictCacheIfNeeded.oldest.anonymous@29836.kvp | [719:29](../YouTubeApi.cs#L719) |
| iteration-variable | Emby.YouTubePlugin.YouTubeApi.EvictCacheIfNeeded.key | [721:17](../YouTubeApi.cs#L721) |
| method | Emby.YouTubePlugin.YouTubeApi.InvalidateCacheContaining | [731:9](../YouTubeApi.cs#L731) |
| parameter | Emby.YouTubePlugin.YouTubeApi.InvalidateCacheContaining.substring | [731:54](../YouTubeApi.cs#L731) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.InvalidateCacheContaining.playlistTag | [740:29](../YouTubeApi.cs#L740) |
| iteration-variable | Emby.YouTubePlugin.YouTubeApi.InvalidateCacheContaining.kvp | [741:25](../YouTubeApi.cs#L741) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.InvalidateCacheContaining.kvp.cacheDir | [747:37](../YouTubeApi.cs#L747) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.InvalidateCacheContaining.diskCacheDir | [753:29](../YouTubeApi.cs#L753) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.InvalidateCacheContaining.prefix | [756:33](../YouTubeApi.cs#L756) |
| iteration-variable | Emby.YouTubePlugin.YouTubeApi.InvalidateCacheContaining.file | [757:29](../YouTubeApi.cs#L757) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.InvalidateCacheContaining.ex | [773:19](../YouTubeApi.cs#L773) |
| method | Emby.YouTubePlugin.YouTubeApi.InvalidateAllCache | [784:9](../YouTubeApi.cs#L784) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.InvalidateAllCache.ex | [794:19](../YouTubeApi.cs#L794) |
| method | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync | [799:9](../YouTubeApi.cs#L799) |
| tuple-element | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.id | [799:35](../YouTubeApi.cs#L799) |
| tuple-element | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.name | [799:47](../YouTubeApi.cs#L799) |
| tuple-element | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.thumb | [799:61](../YouTubeApi.cs#L799) |
| tuple-element | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.uploadsPlaylistId | [799:76](../YouTubeApi.cs#L799) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.apiKey | [800:36](../YouTubeApi.cs#L800) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.query | [800:51](../YouTubeApi.cs#L800) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.isHandle | [800:65](../YouTubeApi.cs#L800) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.ct | [800:80](../YouTubeApi.cs#L800) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.handle | [808:25](../YouTubeApi.cs#L808) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.url | [809:25](../YouTubeApi.cs#L809) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.doc | [810:31](../YouTubeApi.cs#L810) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.root | [813:25](../YouTubeApi.cs#L813) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.items2 | [814:62](../YouTubeApi.cs#L814) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.ch | [818:29](../YouTubeApi.cs#L818) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.channelId | [819:29](../YouTubeApi.cs#L819) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.name | [820:29](../YouTubeApi.cs#L820) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.thumb | [821:29](../YouTubeApi.cs#L821) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.uploadsId | [822:33](../YouTubeApi.cs#L822) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.cd | [823:73](../YouTubeApi.cs#L823) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.rp | [824:78](../YouTubeApi.cs#L824) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelDetailsAsync.ex | [841:19](../YouTubeApi.cs#L841) |
| method | Emby.YouTubePlugin.YouTubeApi.GetChannelByIdAsync | [848:9](../YouTubeApi.cs#L848) |
| tuple-element | Emby.YouTubePlugin.YouTubeApi.GetChannelByIdAsync.id | [848:36](../YouTubeApi.cs#L848) |
| tuple-element | Emby.YouTubePlugin.YouTubeApi.GetChannelByIdAsync.name | [848:48](../YouTubeApi.cs#L848) |
| tuple-element | Emby.YouTubePlugin.YouTubeApi.GetChannelByIdAsync.thumb | [848:62](../YouTubeApi.cs#L848) |
| tuple-element | Emby.YouTubePlugin.YouTubeApi.GetChannelByIdAsync.uploadsPlaylistId | [848:77](../YouTubeApi.cs#L848) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetChannelByIdAsync.apiKey | [849:33](../YouTubeApi.cs#L849) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetChannelByIdAsync.channelId | [849:48](../YouTubeApi.cs#L849) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetChannelByIdAsync.ct | [849:66](../YouTubeApi.cs#L849) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelByIdAsync.url | [851:17](../YouTubeApi.cs#L851) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelByIdAsync.doc | [852:23](../YouTubeApi.cs#L852) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelByIdAsync.root | [855:17](../YouTubeApi.cs#L855) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelByIdAsync.items | [856:54](../YouTubeApi.cs#L856) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelByIdAsync.ch | [860:21](../YouTubeApi.cs#L860) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelByIdAsync.name | [861:21](../YouTubeApi.cs#L861) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelByIdAsync.thumb | [862:21](../YouTubeApi.cs#L862) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelByIdAsync.uploadsId | [863:25](../YouTubeApi.cs#L863) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelByIdAsync.cd | [864:65](../YouTubeApi.cs#L864) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelByIdAsync.rp | [865:70](../YouTubeApi.cs#L865) |
| method | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync | [876:9](../YouTubeApi.cs#L876) |
| tuple-element | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync.name | [876:35](../YouTubeApi.cs#L876) |
| tuple-element | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync.thumb | [876:49](../YouTubeApi.cs#L876) |
| tuple-element | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync.videoCount | [876:64](../YouTubeApi.cs#L876) |
| tuple-element | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync.lookupSucceeded | [876:80](../YouTubeApi.cs#L876) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync.apiKey | [877:37](../YouTubeApi.cs#L877) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync.playlistId | [877:52](../YouTubeApi.cs#L877) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync.ct | [877:71](../YouTubeApi.cs#L877) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync.url | [881:21](../YouTubeApi.cs#L881) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync.doc | [882:27](../YouTubeApi.cs#L882) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync.root | [890:21](../YouTubeApi.cs#L890) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync.items | [891:58](../YouTubeApi.cs#L891) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync.pl | [895:25](../YouTubeApi.cs#L895) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync.name | [896:25](../YouTubeApi.cs#L896) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync.thumb | [897:25](../YouTubeApi.cs#L897) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync.count | [898:25](../YouTubeApi.cs#L898) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync.cd | [899:69](../YouTubeApi.cs#L899) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync.ic | [900:67](../YouTubeApi.cs#L900) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync.n | [901:51](../YouTubeApi.cs#L901) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistDetailsAsync.ex | [912:19](../YouTubeApi.cs#L912) |
| method | Emby.YouTubePlugin.YouTubeApi.SearchVideosAsync | [921:9](../YouTubeApi.cs#L921) |
| parameter | Emby.YouTubePlugin.YouTubeApi.SearchVideosAsync.apiKey | [922:13](../YouTubeApi.cs#L922) |
| parameter | Emby.YouTubePlugin.YouTubeApi.SearchVideosAsync.query | [922:28](../YouTubeApi.cs#L922) |
| parameter | Emby.YouTubePlugin.YouTubeApi.SearchVideosAsync.pageToken | [922:42](../YouTubeApi.cs#L922) |
| parameter | Emby.YouTubePlugin.YouTubeApi.SearchVideosAsync.ct | [922:61](../YouTubeApi.cs#L922) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.SearchVideosAsync.q | [924:17](../YouTubeApi.cs#L924) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.SearchVideosAsync.url | [925:17](../YouTubeApi.cs#L925) |
| method | Emby.YouTubePlugin.YouTubeApi.GetChannelVideosAsync | [934:9](../YouTubeApi.cs#L934) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetChannelVideosAsync.apiKey | [935:13](../YouTubeApi.cs#L935) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetChannelVideosAsync.channelId | [935:28](../YouTubeApi.cs#L935) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetChannelVideosAsync.pageToken | [935:46](../YouTubeApi.cs#L935) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetChannelVideosAsync.ct | [935:65](../YouTubeApi.cs#L935) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetChannelVideosAsync.sortBy | [936:13](../YouTubeApi.cs#L936) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelVideosAsync.normalized | [940:17](../YouTubeApi.cs#L940) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelVideosAsync.uploadsPlaylistId | [949:21](../YouTubeApi.cs#L949) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetChannelVideosAsync.url | [954:17](../YouTubeApi.cs#L954) |
| method | Emby.YouTubePlugin.YouTubeApi.GetPlaylistVideosAsync | [964:9](../YouTubeApi.cs#L964) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetPlaylistVideosAsync.apiKey | [965:13](../YouTubeApi.cs#L965) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetPlaylistVideosAsync.playlistId | [965:28](../YouTubeApi.cs#L965) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetPlaylistVideosAsync.pageToken | [965:47](../YouTubeApi.cs#L965) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetPlaylistVideosAsync.ct | [965:66](../YouTubeApi.cs#L965) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistVideosAsync.url | [967:17](../YouTubeApi.cs#L967) |
| method | Emby.YouTubePlugin.YouTubeApi.GetPlaylistVideoIdsFreshAsync | [986:9](../YouTubeApi.cs#L986) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetPlaylistVideoIdsFreshAsync.apiKey | [987:13](../YouTubeApi.cs#L987) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetPlaylistVideoIdsFreshAsync.playlistId | [987:28](../YouTubeApi.cs#L987) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetPlaylistVideoIdsFreshAsync.maxItems | [987:47](../YouTubeApi.cs#L987) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetPlaylistVideoIdsFreshAsync.ct | [987:61](../YouTubeApi.cs#L987) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistVideoIdsFreshAsync.snapshot | [989:17](../YouTubeApi.cs#L989) |
| method | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync | [994:9](../YouTubeApi.cs#L994) |
| tuple-element | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.VideoIds | [994:35](../YouTubeApi.cs#L994) |
| tuple-element | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.TotalResults | [994:58](../YouTubeApi.cs#L994) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.apiKey | [995:13](../YouTubeApi.cs#L995) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.playlistId | [995:28](../YouTubeApi.cs#L995) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.maxItems | [995:47](../YouTubeApi.cs#L995) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.ct | [995:61](../YouTubeApi.cs#L995) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.ids | [1000:17](../YouTubeApi.cs#L1000) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.seenPageTokens | [1001:17](../YouTubeApi.cs#L1001) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.pageToken | [1002:21](../YouTubeApi.cs#L1002) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.totalResults | [1003:17](../YouTubeApi.cs#L1003) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.page | [1005:22](../YouTubeApi.cs#L1005) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.url | [1010:21](../YouTubeApi.cs#L1010) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.doc | [1020:31](../YouTubeApi.cs#L1020) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.items | [1021:74](../YouTubeApi.cs#L1021) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.pageInfo | [1029:81](../YouTubeApi.cs#L1029) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.totalResultsElement | [1031:81](../YouTubeApi.cs#L1031) |
| iteration-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.item | [1038:21](../YouTubeApi.cs#L1038) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.item.vid | [1040:29](../YouTubeApi.cs#L1040) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.ptEl | [1045:89](../YouTubeApi.cs#L1045) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.GetPlaylistSnapshotFreshAsync.ex | [1053:23](../YouTubeApi.cs#L1053) |
| method | Emby.YouTubePlugin.YouTubeApi.GetTrendingAsync | [1070:9](../YouTubeApi.cs#L1070) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetTrendingAsync.apiKey | [1071:13](../YouTubeApi.cs#L1071) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetTrendingAsync.regionCode | [1071:28](../YouTubeApi.cs#L1071) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetTrendingAsync.categoryId | [1071:48](../YouTubeApi.cs#L1071) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetTrendingAsync.ct | [1071:68](../YouTubeApi.cs#L1071) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetTrendingAsync.url | [1075:17](../YouTubeApi.cs#L1075) |
| method | Emby.YouTubePlugin.YouTubeApi.GetVideoCategoriesAsync | [1087:9](../YouTubeApi.cs#L1087) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetVideoCategoriesAsync.apiKey | [1088:13](../YouTubeApi.cs#L1088) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetVideoCategoriesAsync.regionCode | [1088:28](../YouTubeApi.cs#L1088) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetVideoCategoriesAsync.ct | [1088:47](../YouTubeApi.cs#L1088) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetVideoCategoriesAsync.url | [1092:17](../YouTubeApi.cs#L1092) |
| method | Emby.YouTubePlugin.YouTubeApi.GetI18nRegionsAsync | [1099:9](../YouTubeApi.cs#L1099) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetI18nRegionsAsync.apiKey | [1100:13](../YouTubeApi.cs#L1100) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetI18nRegionsAsync.ct | [1101:13](../YouTubeApi.cs#L1101) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetI18nRegionsAsync.url | [1103:17](../YouTubeApi.cs#L1103) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetI18nRegionsAsync.doc | [1104:17](../YouTubeApi.cs#L1104) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetI18nRegionsAsync.codes | [1108:17](../YouTubeApi.cs#L1108) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetI18nRegionsAsync.items | [1109:65](../YouTubeApi.cs#L1109) |
| iteration-variable | Emby.YouTubePlugin.YouTubeApi.GetI18nRegionsAsync.item | [1112:17](../YouTubeApi.cs#L1112) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetI18nRegionsAsync.item.code | [1114:25](../YouTubeApi.cs#L1114) |
| anonymous-function | Emby.YouTubePlugin.YouTubeApi.GetI18nRegionsAsync.item.anonymous@48741 | [1117:37](../YouTubeApi.cs#L1117) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetI18nRegionsAsync.item.anonymous@48741.character | [1117:37](../YouTubeApi.cs#L1117) |
| method | Emby.YouTubePlugin.YouTubeApi.ResolveContentRegionAsync | [1129:9](../YouTubeApi.cs#L1129) |
| parameter | Emby.YouTubePlugin.YouTubeApi.ResolveContentRegionAsync.apiKey | [1130:13](../YouTubeApi.cs#L1130) |
| parameter | Emby.YouTubePlugin.YouTubeApi.ResolveContentRegionAsync.regionCode | [1131:13](../YouTubeApi.cs#L1131) |
| parameter | Emby.YouTubePlugin.YouTubeApi.ResolveContentRegionAsync.fallback | [1132:13](../YouTubeApi.cs#L1132) |
| parameter | Emby.YouTubePlugin.YouTubeApi.ResolveContentRegionAsync.ct | [1133:13](../YouTubeApi.cs#L1133) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.ResolveContentRegionAsync.normalized | [1135:17](../YouTubeApi.cs#L1135) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.ResolveContentRegionAsync.supported | [1139:17](../YouTubeApi.cs#L1139) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.ResolveContentRegionAsync.regionDocument | [1142:27](../YouTubeApi.cs#L1142) |
| method | Emby.YouTubePlugin.YouTubeApi.GetVideoDetailsBatchAsync | [1158:9](../YouTubeApi.cs#L1158) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetVideoDetailsBatchAsync.apiKey | [1159:13](../YouTubeApi.cs#L1159) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetVideoDetailsBatchAsync.videoIds | [1159:28](../YouTubeApi.cs#L1159) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetVideoDetailsBatchAsync.ct | [1159:58](../YouTubeApi.cs#L1159) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetVideoDetailsBatchAsync.ids | [1161:17](../YouTubeApi.cs#L1161) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetVideoDetailsBatchAsync.url | [1164:17](../YouTubeApi.cs#L1164) |
| method | Emby.YouTubePlugin.YouTubeApi.GetString | [1172:9](../YouTubeApi.cs#L1172) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetString.el | [1172:41](../YouTubeApi.cs#L1172) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetString.name | [1172:57](../YouTubeApi.cs#L1172) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetString.p | [1175:50](../YouTubeApi.cs#L1175) |
| method | Emby.YouTubePlugin.YouTubeApi.GetNestedString | [1179:9](../YouTubeApi.cs#L1179) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetNestedString.el | [1179:47](../YouTubeApi.cs#L1179) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetNestedString.parent | [1179:63](../YouTubeApi.cs#L1179) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetNestedString.child | [1179:78](../YouTubeApi.cs#L1179) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetNestedString.p | [1181:51](../YouTubeApi.cs#L1181) |
| method | Emby.YouTubePlugin.YouTubeApi.GetBestThumbnail | [1186:9](../YouTubeApi.cs#L1186) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetBestThumbnail.el | [1186:48](../YouTubeApi.cs#L1186) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetBestThumbnail.snippet | [1188:55](../YouTubeApi.cs#L1188) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetBestThumbnail.thumbs | [1189:63](../YouTubeApi.cs#L1189) |
| iteration-variable | Emby.YouTubePlugin.YouTubeApi.GetBestThumbnail.quality | [1193:13](../YouTubeApi.cs#L1193) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.GetBestThumbnail.quality.t | [1195:60](../YouTubeApi.cs#L1195) |
| local-variable | Emby.YouTubePlugin.YouTubeApi.GetBestThumbnail.quality.url | [1197:25](../YouTubeApi.cs#L1197) |
| method | Emby.YouTubePlugin.YouTubeApi.GetStableVideoThumbnailUrl | [1204:9](../YouTubeApi.cs#L1204) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetStableVideoThumbnailUrl.videoId | [1204:57](../YouTubeApi.cs#L1204) |
| parameter | Emby.YouTubePlugin.YouTubeApi.GetStableVideoThumbnailUrl.preferredUrl | [1204:73](../YouTubeApi.cs#L1204) |
| method | Emby.YouTubePlugin.YouTubeApi.ParseDuration | [1217:9](../YouTubeApi.cs#L1217) |
| parameter | Emby.YouTubePlugin.YouTubeApi.ParseDuration.duration | [1217:47](../YouTubeApi.cs#L1217) |
| catch-variable | Emby.YouTubePlugin.YouTubeApi.ParseDuration.ex | [1228:19](../YouTubeApi.cs#L1228) |
| method | Emby.YouTubePlugin.YouTubeApi.ParsePublishedAt | [1238:9](../YouTubeApi.cs#L1238) |
| parameter | Emby.YouTubePlugin.YouTubeApi.ParsePublishedAt.publishedAt | [1238:50](../YouTubeApi.cs#L1238) |
| pattern-variable | Emby.YouTubePlugin.YouTubeApi.ParsePublishedAt.dt | [1243:29](../YouTubeApi.cs#L1243) |

</details>

<a id="file-914fdb518473aaa9"></a>
## [YouTubeCaptionMetadata.cs](../YouTubeCaptionMetadata.cs)

C# source excluded from plugin Compile; 12636 bytes; 322 lines.

<details>
<summary>All 102 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [8:1](../YouTubeCaptionMetadata.cs#L8) |
| type | Emby.YouTubePlugin.YouTubeCaptionTrackMetadata | [10:5](../YouTubeCaptionMetadata.cs#L10) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionTrackMetadata.LanguageCode | [11:9](../YouTubeCaptionMetadata.cs#L11) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionTrackMetadata.DisplayName | [12:9](../YouTubeCaptionMetadata.cs#L12) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionTrackMetadata.Kind | [13:9](../YouTubeCaptionMetadata.cs#L13) |
| type | Emby.YouTubePlugin.YouTubeCaptionMetadata | [17:5](../YouTubeCaptionMetadata.cs#L17) |
| field | Emby.YouTubePlugin.YouTubeCaptionMetadata.SuccessTtl | [19:42](../YouTubeCaptionMetadata.cs#L19) |
| field | Emby.YouTubePlugin.YouTubeCaptionMetadata.FailureTtl | [20:42](../YouTubeCaptionMetadata.cs#L20) |
| field | Emby.YouTubePlugin.YouTubeCaptionMetadata.RequestTimeout | [21:42](../YouTubeCaptionMetadata.cs#L21) |
| field | Emby.YouTubePlugin.YouTubeCaptionMetadata.MaxCacheEntries | [22:27](../YouTubeCaptionMetadata.cs#L22) |
| field | Emby.YouTubePlugin.YouTubeCaptionMetadata.FetchGate | [23:47](../YouTubeCaptionMetadata.cs#L23) |
| field | Emby.YouTubePlugin.YouTubeCaptionMetadata.Client | [24:44](../YouTubeCaptionMetadata.cs#L24) |
| field | Emby.YouTubePlugin.YouTubeCaptionMetadata.Cache | [31:74](../YouTubeCaptionMetadata.cs#L31) |
| field | Emby.YouTubePlugin.YouTubeCaptionMetadata.InFlight | [32:118](../YouTubeCaptionMetadata.cs#L32) |
| constructor | Emby.YouTubePlugin.YouTubeCaptionMetadata.YouTubeCaptionMetadata | [35:9](../YouTubeCaptionMetadata.cs#L35) |
| method | Emby.YouTubePlugin.YouTubeCaptionMetadata.Prefetch | [42:9](../YouTubeCaptionMetadata.cs#L42) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.Prefetch.videoId | [42:37](../YouTubeCaptionMetadata.cs#L42) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.Prefetch.pending | [47:17](../YouTubeCaptionMetadata.cs#L47) |
| anonymous-function | Emby.YouTubePlugin.YouTubeCaptionMetadata.Prefetch.pending.anonymous@2039 | [49:17](../YouTubeCaptionMetadata.cs#L49) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.Prefetch.pending.anonymous@2039.id | [49:17](../YouTubeCaptionMetadata.cs#L49) |
| anonymous-function | Emby.YouTubePlugin.YouTubeCaptionMetadata.Prefetch.pending.anonymous@2039.anonymous@2125 | [50:21](../YouTubeCaptionMetadata.cs#L50) |
| method | Emby.YouTubePlugin.YouTubeCaptionMetadata.GetCachedTracks | [58:9](../YouTubeCaptionMetadata.cs#L58) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.GetCachedTracks.videoId | [58:82](../YouTubeCaptionMetadata.cs#L58) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.GetCachedTracks.fresh | [63:46](../YouTubeCaptionMetadata.cs#L63) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.GetCachedTracks.stale | [68:48](../YouTubeCaptionMetadata.cs#L68) |
| method | Emby.YouTubePlugin.YouTubeCaptionMetadata.GetTracksWithBudgetAsync | [73:9](../YouTubeCaptionMetadata.cs#L73) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.GetTracksWithBudgetAsync.videoId | [74:13](../YouTubeCaptionMetadata.cs#L74) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.GetTracksWithBudgetAsync.maxWait | [75:13](../YouTubeCaptionMetadata.cs#L75) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.GetTracksWithBudgetAsync.fresh | [79:46](../YouTubeCaptionMetadata.cs#L79) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.GetTracksWithBudgetAsync.pending | [83:55](../YouTubeCaptionMetadata.cs#L83) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.GetTracksWithBudgetAsync.stale | [96:48](../YouTubeCaptionMetadata.cs#L96) |
| method | Emby.YouTubePlugin.YouTubeCaptionMetadata.TryGetFresh | [100:9](../YouTubeCaptionMetadata.cs#L100) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.TryGetFresh.videoId | [101:13](../YouTubeCaptionMetadata.cs#L101) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.TryGetFresh.tracks | [102:13](../YouTubeCaptionMetadata.cs#L102) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.TryGetFresh.cached | [104:52](../YouTubeCaptionMetadata.cs#L104) |
| method | Emby.YouTubePlugin.YouTubeCaptionMetadata.FetchAndCacheAsync | [115:9](../YouTubeCaptionMetadata.cs#L115) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.FetchAndCacheAsync.videoId | [115:98](../YouTubeCaptionMetadata.cs#L115) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.FetchAndCacheAsync.now | [117:17](../YouTubeCaptionMetadata.cs#L117) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.FetchAndCacheAsync.url | [121:21](../YouTubeCaptionMetadata.cs#L121) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.FetchAndCacheAsync.timeout | [122:27](../YouTubeCaptionMetadata.cs#L122) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.FetchAndCacheAsync.html | [124:24](../YouTubeCaptionMetadata.cs#L124) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.FetchAndCacheAsync.request | [127:31](../YouTubeCaptionMetadata.cs#L127) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.FetchAndCacheAsync.response | [128:31](../YouTubeCaptionMetadata.cs#L128) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.FetchAndCacheAsync.json | [140:21](../YouTubeCaptionMetadata.cs#L140) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.FetchAndCacheAsync.tracks | [144:21](../YouTubeCaptionMetadata.cs#L144) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.FetchAndCacheAsync.tracks | [156:21](../YouTubeCaptionMetadata.cs#L156) |
| catch-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.FetchAndCacheAsync.ex | [161:19](../YouTubeCaptionMetadata.cs#L161) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.FetchAndCacheAsync.tracks | [164:21](../YouTubeCaptionMetadata.cs#L164) |
| method | Emby.YouTubePlugin.YouTubeCaptionMetadata.PreserveStaleTracks | [175:9](../YouTubeCaptionMetadata.cs#L175) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.PreserveStaleTracks.videoId | [176:13](../YouTubeCaptionMetadata.cs#L176) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.PreserveStaleTracks.now | [177:13](../YouTubeCaptionMetadata.cs#L177) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.PreserveStaleTracks.tracks | [179:17](../YouTubeCaptionMetadata.cs#L179) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.PreserveStaleTracks.tracks.previous | [179:61](../YouTubeCaptionMetadata.cs#L179) |
| method | Emby.YouTubePlugin.YouTubeCaptionMetadata.TrimCache | [186:9](../YouTubeCaptionMetadata.cs#L186) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.TrimCache.now | [186:39](../YouTubeCaptionMetadata.cs#L186) |
| iteration-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.TrimCache.expired | [191:13](../YouTubeCaptionMetadata.cs#L191) |
| anonymous-function | Emby.YouTubePlugin.YouTubeCaptionMetadata.TrimCache.expired.anonymous@7828 | [192:33](../YouTubeCaptionMetadata.cs#L192) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.TrimCache.expired.anonymous@7828.entry | [192:33](../YouTubeCaptionMetadata.cs#L192) |
| anonymous-function | Emby.YouTubePlugin.YouTubeCaptionMetadata.TrimCache.expired.anonymous@7901 | [193:34](../YouTubeCaptionMetadata.cs#L193) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.TrimCache.expired.anonymous@7901.entry | [193:34](../YouTubeCaptionMetadata.cs#L193) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.TrimCache.overflow | [199:17](../YouTubeCaptionMetadata.cs#L199) |
| iteration-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.TrimCache.oldest | [203:13](../YouTubeCaptionMetadata.cs#L203) |
| anonymous-function | Emby.YouTubePlugin.YouTubeCaptionMetadata.TrimCache.oldest.anonymous@8225 | [204:35](../YouTubeCaptionMetadata.cs#L204) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.TrimCache.oldest.anonymous@8225.entry | [204:35](../YouTubeCaptionMetadata.cs#L204) |
| anonymous-function | Emby.YouTubePlugin.YouTubeCaptionMetadata.TrimCache.oldest.anonymous@8331 | [206:34](../YouTubeCaptionMetadata.cs#L206) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.TrimCache.oldest.anonymous@8331.entry | [206:34](../YouTubeCaptionMetadata.cs#L206) |
| method | Emby.YouTubePlugin.YouTubeCaptionMetadata.ParseTracks | [213:9](../YouTubeCaptionMetadata.cs#L213) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.ParseTracks.json | [213:79](../YouTubeCaptionMetadata.cs#L213) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ParseTracks.document | [215:23](../YouTubeCaptionMetadata.cs#L215) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ParseTracks.captions | [216:74](../YouTubeCaptionMetadata.cs#L216) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ParseTracks.renderer | [217:88](../YouTubeCaptionMetadata.cs#L217) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ParseTracks.captionTracks | [218:70](../YouTubeCaptionMetadata.cs#L218) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ParseTracks.result | [224:17](../YouTubeCaptionMetadata.cs#L224) |
| iteration-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ParseTracks.track | [225:13](../YouTubeCaptionMetadata.cs#L225) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ParseTracks.track.languageElement | [227:67](../YouTubeCaptionMetadata.cs#L227) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ParseTracks.track.languageCode | [230:21](../YouTubeCaptionMetadata.cs#L230) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ParseTracks.track.displayName | [234:21](../YouTubeCaptionMetadata.cs#L234) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ParseTracks.track.kind | [235:21](../YouTubeCaptionMetadata.cs#L235) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ParseTracks.track.kind.kindElement | [235:65](../YouTubeCaptionMetadata.cs#L235) |
| method | Emby.YouTubePlugin.YouTubeCaptionMetadata.ReadText | [252:9](../YouTubeCaptionMetadata.cs#L252) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.ReadText.parent | [252:41](../YouTubeCaptionMetadata.cs#L252) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.ReadText.propertyName | [252:61](../YouTubeCaptionMetadata.cs#L252) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ReadText.value | [254:62](../YouTubeCaptionMetadata.cs#L254) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ReadText.simpleText | [256:60](../YouTubeCaptionMetadata.cs#L256) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ReadText.runs | [258:55](../YouTubeCaptionMetadata.cs#L258) |
| anonymous-function | Emby.YouTubePlugin.YouTubeCaptionMetadata.ReadText.anonymous@10837 | [263:29](../YouTubeCaptionMetadata.cs#L263) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.ReadText.anonymous@10837.run | [263:29](../YouTubeCaptionMetadata.cs#L263) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ReadText.anonymous@10837.text | [263:71](../YouTubeCaptionMetadata.cs#L263) |
| method | Emby.YouTubePlugin.YouTubeCaptionMetadata.ExtractPlayerResponseJson | [266:9](../YouTubeCaptionMetadata.cs#L266) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.ExtractPlayerResponseJson.html | [266:58](../YouTubeCaptionMetadata.cs#L266) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ExtractPlayerResponseJson.marker | [268:26](../YouTubeCaptionMetadata.cs#L268) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ExtractPlayerResponseJson.markerIndex | [269:17](../YouTubeCaptionMetadata.cs#L269) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ExtractPlayerResponseJson.start | [273:17](../YouTubeCaptionMetadata.cs#L273) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ExtractPlayerResponseJson.depth | [277:17](../YouTubeCaptionMetadata.cs#L277) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ExtractPlayerResponseJson.inString | [278:17](../YouTubeCaptionMetadata.cs#L278) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ExtractPlayerResponseJson.escaped | [279:17](../YouTubeCaptionMetadata.cs#L279) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ExtractPlayerResponseJson.i | [280:22](../YouTubeCaptionMetadata.cs#L280) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionMetadata.ExtractPlayerResponseJson.character | [282:21](../YouTubeCaptionMetadata.cs#L282) |
| type | Emby.YouTubePlugin.YouTubeCaptionMetadata.CacheEntry | [317:9](../YouTubeCaptionMetadata.cs#L317) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.CacheEntry.Tracks | [318:13](../YouTubeCaptionMetadata.cs#L318) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.CacheEntry.CachedUtc | [319:13](../YouTubeCaptionMetadata.cs#L319) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionMetadata.CacheEntry.ExpiresUtc | [320:13](../YouTubeCaptionMetadata.cs#L320) |

</details>

<a id="file-179571d4992d8567"></a>
## [YouTubeCaptionNormalizer.cs](../YouTubeCaptionNormalizer.cs)

Plugin Compile source (SDK default); 13318 bytes; 265 lines.

<details>
<summary>All 89 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [5:1](../YouTubeCaptionNormalizer.cs#L5) |
| type | Emby.YouTubePlugin.YouTubeCaptionNormalizer | [9:5](../YouTubeCaptionNormalizer.cs#L9) |
| field | Emby.YouTubePlugin.YouTubeCaptionNormalizer.CacheRevision | [11:31](../YouTubeCaptionNormalizer.cs#L11) |
| field | Emby.YouTubePlugin.YouTubeCaptionNormalizer.MaximumPayloadBytes | [12:28](../YouTubeCaptionNormalizer.cs#L12) |
| field | Emby.YouTubePlugin.YouTubeCaptionNormalizer.MaximumLines | [13:27](../YouTubeCaptionNormalizer.cs#L13) |
| field | Emby.YouTubePlugin.YouTubeCaptionNormalizer.MaximumCueCharacters | [14:27](../YouTubeCaptionNormalizer.cs#L14) |
| field | Emby.YouTubePlugin.YouTubeCaptionNormalizer.MaximumTimestampMilliseconds | [15:28](../YouTubeCaptionNormalizer.cs#L15) |
| field | Emby.YouTubePlugin.YouTubeCaptionNormalizer.StrictUtf8 | [16:46](../YouTubeCaptionNormalizer.cs#L16) |
| method | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryNormalize | [18:9](../YouTubeCaptionNormalizer.cs#L18) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryNormalize.payload | [18:43](../YouTubeCaptionNormalizer.cs#L18) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryNormalize.normalized | [18:60](../YouTubeCaptionNormalizer.cs#L18) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryNormalize.text | [23:20](../YouTubeCaptionNormalizer.cs#L23) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryNormalize.lines | [30:17](../YouTubeCaptionNormalizer.cs#L30) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryNormalize.index | [35:17](../YouTubeCaptionNormalizer.cs#L35) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryNormalize.output | [48:17](../YouTubeCaptionNormalizer.cs#L48) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryNormalize.cues | [49:17](../YouTubeCaptionNormalizer.cs#L49) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryNormalize.previousStart | [50:18](../YouTubeCaptionNormalizer.cs#L50) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryNormalize.first | [55:21](../YouTubeCaptionNormalizer.cs#L55) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryNormalize.end | [57:21](../YouTubeCaptionNormalizer.cs#L57) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryNormalize.timing | [63:21](../YouTubeCaptionNormalizer.cs#L63) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryNormalize.startToken | [65:58](../YouTubeCaptionNormalizer.cs#L65) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryNormalize.endToken | [65:78](../YouTubeCaptionNormalizer.cs#L65) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryNormalize.startMs | [65:96](../YouTubeCaptionNormalizer.cs#L65) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryNormalize.sourceText | [69:21](../YouTubeCaptionNormalizer.cs#L69) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryNormalize.i | [70:26](../YouTubeCaptionNormalizer.cs#L70) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryNormalize.cueText | [78:66](../YouTubeCaptionNormalizer.cs#L78) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryNormalize.canonical | [94:17](../YouTubeCaptionNormalizer.cs#L94) |
| method | Emby.YouTubePlugin.YouTubeCaptionNormalizer.IsCanonical | [101:9](../YouTubeCaptionNormalizer.cs#L101) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.IsCanonical.payload | [101:42](../YouTubeCaptionNormalizer.cs#L101) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.IsCanonical.normalized | [102:43](../YouTubeCaptionNormalizer.cs#L102) |
| method | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTiming | [104:9](../YouTubeCaptionNormalizer.cs#L104) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTiming.line | [104:39](../YouTubeCaptionNormalizer.cs#L104) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTiming.start | [104:52](../YouTubeCaptionNormalizer.cs#L104) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTiming.end | [104:70](../YouTubeCaptionNormalizer.cs#L104) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTiming.startMs | [104:86](../YouTubeCaptionNormalizer.cs#L104) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTiming.separator | [110:17](../YouTubeCaptionNormalizer.cs#L110) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTiming.after | [114:17](../YouTubeCaptionNormalizer.cs#L114) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTiming.whitespace | [115:17](../YouTubeCaptionNormalizer.cs#L115) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTiming.endMs | [118:82](../YouTubeCaptionNormalizer.cs#L118) |
| method | Emby.YouTubePlugin.YouTubeCaptionNormalizer.ValidSettings | [122:9](../YouTubeCaptionNormalizer.cs#L122) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.ValidSettings.settings | [122:43](../YouTubeCaptionNormalizer.cs#L122) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.ValidSettings.seen | [124:17](../YouTubeCaptionNormalizer.cs#L124) |
| iteration-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.ValidSettings.token | [125:13](../YouTubeCaptionNormalizer.cs#L125) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.ValidSettings.token.colon | [127:21](../YouTubeCaptionNormalizer.cs#L127) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.ValidSettings.token.name | [129:21](../YouTubeCaptionNormalizer.cs#L129) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.ValidSettings.token.value | [130:21](../YouTubeCaptionNormalizer.cs#L130) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.ValidSettings.token.components | [132:21](../YouTubeCaptionNormalizer.cs#L132) |
| local-function | Emby.YouTubePlugin.YouTubeCaptionNormalizer.ValidSettings.token.Percentage | [133:17](../YouTubeCaptionNormalizer.cs#L133) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.ValidSettings.token.Percentage.number | [133:33](../YouTubeCaptionNormalizer.cs#L133) |
| anonymous-function | Emby.YouTubePlugin.YouTubeCaptionNormalizer.ValidSettings.token.Percentage.anonymous@6916 | [134:64](../YouTubeCaptionNormalizer.cs#L134) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.ValidSettings.token.Percentage.anonymous@6916.c | [134:64](../YouTubeCaptionNormalizer.cs#L134) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.ValidSettings.token.Percentage.percent | [135:125](../YouTubeCaptionNormalizer.cs#L135) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.ValidSettings.token.line | [156:135](../YouTubeCaptionNormalizer.cs#L156) |
| anonymous-function | Emby.YouTubePlugin.YouTubeCaptionNormalizer.ValidSettings.token.anonymous@8616 | [162:62](../YouTubeCaptionNormalizer.cs#L162) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.ValidSettings.token.anonymous@8616.c | [162:62](../YouTubeCaptionNormalizer.cs#L162) |
| method | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTimestamp | [172:9](../YouTubeCaptionNormalizer.cs#L172) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTimestamp.value | [172:42](../YouTubeCaptionNormalizer.cs#L172) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTimestamp.milliseconds | [172:56](../YouTubeCaptionNormalizer.cs#L172) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTimestamp.parts | [175:17](../YouTubeCaptionNormalizer.cs#L175) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTimestamp.seconds | [177:17](../YouTubeCaptionNormalizer.cs#L177) |
| local-function | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTimestamp.Number | [181:13](../YouTubeCaptionNormalizer.cs#L181) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTimestamp.Number.token | [181:32](../YouTubeCaptionNormalizer.cs#L181) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTimestamp.Number.number | [181:46](../YouTubeCaptionNormalizer.cs#L181) |
| anonymous-function | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTimestamp.Number.anonymous@9537 | [184:34](../YouTubeCaptionNormalizer.cs#L184) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTimestamp.Number.anonymous@9537.character | [184:34](../YouTubeCaptionNormalizer.cs#L184) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTimestamp.minutes | [187:44](../YouTubeCaptionNormalizer.cs#L187) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTimestamp.secs | [188:48](../YouTubeCaptionNormalizer.cs#L188) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTimestamp.ms | [188:98](../YouTubeCaptionNormalizer.cs#L188) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryTimestamp.hours | [190:17](../YouTubeCaptionNormalizer.cs#L190) |
| method | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryPlainText | [196:9](../YouTubeCaptionNormalizer.cs#L196) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryPlainText.source | [196:42](../YouTubeCaptionNormalizer.cs#L196) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryPlainText.text | [196:57](../YouTubeCaptionNormalizer.cs#L196) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryPlainText.stripped | [199:17](../YouTubeCaptionNormalizer.cs#L199) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryPlainText.index | [200:22](../YouTubeCaptionNormalizer.cs#L200) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryPlainText.character | [202:21](../YouTubeCaptionNormalizer.cs#L202) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryPlainText.close | [208:21](../YouTubeCaptionNormalizer.cs#L208) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryPlainText.tag | [211:21](../YouTubeCaptionNormalizer.cs#L211) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryPlainText.content | [220:25](../YouTubeCaptionNormalizer.cs#L220) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryPlainText.boundary | [221:25](../YouTubeCaptionNormalizer.cs#L221) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryPlainText.name | [222:25](../YouTubeCaptionNormalizer.cs#L222) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryPlainText.decoded | [232:17](../YouTubeCaptionNormalizer.cs#L232) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryPlainText.escaped | [236:17](../YouTubeCaptionNormalizer.cs#L236) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryPlainText.meaningful | [237:17](../YouTubeCaptionNormalizer.cs#L237) |
| iteration-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryPlainText.line | [238:13](../YouTubeCaptionNormalizer.cs#L238) |
| iteration-variable | Emby.YouTubePlugin.YouTubeCaptionNormalizer.TryPlainText.line.character | [242:17](../YouTubeCaptionNormalizer.cs#L242) |
| method | Emby.YouTubePlugin.YouTubeCaptionNormalizer.SafeCharacters | [260:9](../YouTubeCaptionNormalizer.cs#L260) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.SafeCharacters.text | [260:44](../YouTubeCaptionNormalizer.cs#L260) |
| anonymous-function | Emby.YouTubePlugin.YouTubeCaptionNormalizer.SafeCharacters.anonymous@13093 | [260:69](../YouTubeCaptionNormalizer.cs#L260) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionNormalizer.SafeCharacters.anonymous@13093.character | [260:69](../YouTubeCaptionNormalizer.cs#L260) |

</details>

<a id="file-1dd14218ee040c7f"></a>
## [YouTubeCaptionOffGuard.cs](../YouTubeCaptionOffGuard.cs)

Plugin Compile source (SDK default); 16243 bytes; 392 lines.

<details>
<summary>All 121 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [14:1](../YouTubeCaptionOffGuard.cs#L14) |
| type | Emby.YouTubePlugin.YouTubeCaptionOffGuard | [20:5](../YouTubeCaptionOffGuard.cs#L20) |
| field | Emby.YouTubePlugin.YouTubeCaptionOffGuard.RetryDelay | [22:42](../YouTubeCaptionOffGuard.cs#L22) |
| field | Emby.YouTubePlugin.YouTubeCaptionOffGuard.MaxAttempts | [23:27](../YouTubeCaptionOffGuard.cs#L23) |
| field | Emby.YouTubePlugin.YouTubeCaptionOffGuard._sessionManager | [25:42](../YouTubeCaptionOffGuard.cs#L25) |
| field | Emby.YouTubePlugin.YouTubeCaptionOffGuard._playbacks | [26:77](../YouTubeCaptionOffGuard.cs#L26) |
| field | Emby.YouTubePlugin.YouTubeCaptionOffGuard._playbackStartGate | [27:33](../YouTubeCaptionOffGuard.cs#L27) |
| field | Emby.YouTubePlugin.YouTubeCaptionOffGuard._lifetimeCts | [28:50](../YouTubeCaptionOffGuard.cs#L28) |
| field | Emby.YouTubePlugin.YouTubeCaptionOffGuard._started | [29:21](../YouTubeCaptionOffGuard.cs#L29) |
| field | Emby.YouTubePlugin.YouTubeCaptionOffGuard._disposed | [30:21](../YouTubeCaptionOffGuard.cs#L30) |
| type | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CaptionPlaybackIdentity | [32:9](../YouTubeCaptionOffGuard.cs#L32) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CaptionPlaybackIdentity.Key | [33:13](../YouTubeCaptionOffGuard.cs#L33) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CaptionPlaybackIdentity.SessionId | [34:13](../YouTubeCaptionOffGuard.cs#L34) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CaptionPlaybackIdentity.PlaySessionId | [35:13](../YouTubeCaptionOffGuard.cs#L35) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CaptionPlaybackIdentity.UserId | [36:13](../YouTubeCaptionOffGuard.cs#L36) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CaptionPlaybackIdentity.ItemId | [37:13](../YouTubeCaptionOffGuard.cs#L37) |
| type | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CaptionPlaybackState | [39:9](../YouTubeCaptionOffGuard.cs#L39) |
| property | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CaptionPlaybackState.Identity | [41:13](../YouTubeCaptionOffGuard.cs#L41) |
| accessor | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CaptionPlaybackState.Identity.get | [41:55](../YouTubeCaptionOffGuard.cs#L41) |
| property | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CaptionPlaybackState.Gate | [42:13](../YouTubeCaptionOffGuard.cs#L42) |
| accessor | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CaptionPlaybackState.Gate.get | [42:34](../YouTubeCaptionOffGuard.cs#L42) |
| field | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CaptionPlaybackState.AttemptsSent | [43:24](../YouTubeCaptionOffGuard.cs#L43) |
| field | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CaptionPlaybackState.RetryNotBeforeUtc | [44:29](../YouTubeCaptionOffGuard.cs#L44) |
| field | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CaptionPlaybackState.Cancelled | [45:25](../YouTubeCaptionOffGuard.cs#L45) |
| field | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CaptionPlaybackState.Paused | [46:25](../YouTubeCaptionOffGuard.cs#L46) |
| constructor | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CaptionPlaybackState.CaptionPlaybackState | [48:13](../YouTubeCaptionOffGuard.cs#L48) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CaptionPlaybackState.CaptionPlaybackState.identity | [48:41](../YouTubeCaptionOffGuard.cs#L48) |
| constructor | Emby.YouTubePlugin.YouTubeCaptionOffGuard.YouTubeCaptionOffGuard | [51:9](../YouTubeCaptionOffGuard.cs#L51) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.YouTubeCaptionOffGuard.sessionManager | [51:39](../YouTubeCaptionOffGuard.cs#L51) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.Start | [56:9](../YouTubeCaptionOffGuard.cs#L56) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.SuspendForDirectPlayback | [66:9](../YouTubeCaptionOffGuard.cs#L66) |
| iteration-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.SuspendForDirectPlayback.state | [68:13](../YouTubeCaptionOffGuard.cs#L68) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.OnPlaybackStart | [72:9](../YouTubeCaptionOffGuard.cs#L72) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.OnPlaybackStart.sender | [72:38](../YouTubeCaptionOffGuard.cs#L72) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.OnPlaybackStart.e | [72:54](../YouTubeCaptionOffGuard.cs#L72) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.OnPlaybackStart.session | [81:21](../YouTubeCaptionOffGuard.cs#L81) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.OnPlaybackStart.current | [88:68](../YouTubeCaptionOffGuard.cs#L88) |
| catch-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.OnPlaybackStart.ex | [103:19](../YouTubeCaptionOffGuard.cs#L103) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.OnPlaybackProgress | [109:9](../YouTubeCaptionOffGuard.cs#L109) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.OnPlaybackProgress.sender | [109:41](../YouTubeCaptionOffGuard.cs#L109) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.OnPlaybackProgress.e | [109:57](../YouTubeCaptionOffGuard.cs#L109) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.OnPlaybackProgress.session | [121:21](../YouTubeCaptionOffGuard.cs#L121) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.OnPlaybackProgress.item | [122:21](../YouTubeCaptionOffGuard.cs#L122) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.OnPlaybackProgress.sessionId | [123:21](../YouTubeCaptionOffGuard.cs#L123) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.OnPlaybackProgress.state | [127:64](../YouTubeCaptionOffGuard.cs#L127) |
| catch-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.OnPlaybackProgress.ex | [135:19](../YouTubeCaptionOffGuard.cs#L135) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.OnPlaybackStopped | [141:9](../YouTubeCaptionOffGuard.cs#L141) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.OnPlaybackStopped.sender | [141:40](../YouTubeCaptionOffGuard.cs#L141) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.OnPlaybackStopped.e | [141:56](../YouTubeCaptionOffGuard.cs#L141) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.OnPlaybackStopped.session | [143:17](../YouTubeCaptionOffGuard.cs#L143) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.OnPlaybackStopped.state | [145:63](../YouTubeCaptionOffGuard.cs#L145) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.TrySendCaptionOffAsync | [150:9](../YouTubeCaptionOffGuard.cs#L150) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.TrySendCaptionOffAsync.state | [150:51](../YouTubeCaptionOffGuard.cs#L150) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.TrySendCaptionOffAsync.dispatch | [154:22](../YouTubeCaptionOffGuard.cs#L154) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.TrySendCaptionOffAsync.identity | [155:21](../YouTubeCaptionOffGuard.cs#L155) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.TrySendCaptionOffAsync.active | [159:80](../YouTubeCaptionOffGuard.cs#L159) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.TrySendCaptionOffAsync.currentSession | [161:65](../YouTubeCaptionOffGuard.cs#L161) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.TrySendCaptionOffAsync.commandSessionId | [169:72](../YouTubeCaptionOffGuard.cs#L169) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.TrySendCaptionOffAsync.commandSession | [171:25](../YouTubeCaptionOffGuard.cs#L171) |
| anonymous-function | Emby.YouTubePlugin.YouTubeCaptionOffGuard.TrySendCaptionOffAsync.commandSession.anonymous@6802 | [171:82](../YouTubeCaptionOffGuard.cs#L171) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.TrySendCaptionOffAsync.commandSession.anonymous@6802.s | [171:82](../YouTubeCaptionOffGuard.cs#L171) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.TrySendCaptionOffAsync.command | [183:25](../YouTubeCaptionOffGuard.cs#L183) |
| catch-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.TrySendCaptionOffAsync.ex | [208:19](../YouTubeCaptionOffGuard.cs#L208) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.ScheduleRetry | [216:9](../YouTubeCaptionOffGuard.cs#L216) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.ScheduleRetry.state | [216:36](../YouTubeCaptionOffGuard.cs#L216) |
| anonymous-function | Emby.YouTubePlugin.YouTubeCaptionOffGuard.ScheduleRetry.anonymous@9019 | [218:26](../YouTubeCaptionOffGuard.cs#L218) |
| catch-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.ScheduleRetry.anonymous@9019.ex | [230:23](../YouTubeCaptionOffGuard.cs#L230) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsCurrentIdentity | [237:9](../YouTubeCaptionOffGuard.cs#L237) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsCurrentIdentity.identity | [237:40](../YouTubeCaptionOffGuard.cs#L237) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsCurrentIdentity.session | [237:74](../YouTubeCaptionOffGuard.cs#L237) |
| anonymous-function | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsCurrentIdentity.anonymous@9915 | [239:63](../YouTubeCaptionOffGuard.cs#L239) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsCurrentIdentity.anonymous@9915.s | [239:63](../YouTubeCaptionOffGuard.cs#L239) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsCurrentIdentity.nowPlaying | [249:17](../YouTubeCaptionOffGuard.cs#L249) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsCurrentIdentity.currentPlaySessionId | [256:17](../YouTubeCaptionOffGuard.cs#L256) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CreateIdentity | [261:9](../YouTubeCaptionOffGuard.cs#L261) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CreateIdentity.e | [262:13](../YouTubeCaptionOffGuard.cs#L262) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CreateIdentity.session | [263:13](../YouTubeCaptionOffGuard.cs#L263) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CreateIdentity.item | [264:13](../YouTubeCaptionOffGuard.cs#L264) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CreateIdentity.sessionId | [266:17](../YouTubeCaptionOffGuard.cs#L266) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CreateIdentity.playSessionId | [267:17](../YouTubeCaptionOffGuard.cs#L267) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CreateIdentity.userId | [268:17](../YouTubeCaptionOffGuard.cs#L268) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CancelForSession | [277:9](../YouTubeCaptionOffGuard.cs#L277) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CancelForSession.sessionId | [277:39](../YouTubeCaptionOffGuard.cs#L277) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CancelForSession.state | [279:59](../YouTubeCaptionOffGuard.cs#L279) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CancelState | [283:9](../YouTubeCaptionOffGuard.cs#L283) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.CancelState.state | [283:34](../YouTubeCaptionOffGuard.cs#L283) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.MatchesEvent | [293:9](../YouTubeCaptionOffGuard.cs#L293) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.MatchesEvent.identity | [293:42](../YouTubeCaptionOffGuard.cs#L293) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.MatchesEvent.e | [294:13](../YouTubeCaptionOffGuard.cs#L294) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.MatchesEvent.session | [294:42](../YouTubeCaptionOffGuard.cs#L294) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.MatchesEvent.item | [294:63](../YouTubeCaptionOffGuard.cs#L294) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsYouTubeItem | [299:9](../YouTubeCaptionOffGuard.cs#L299) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsYouTubeItem.item | [299:43](../YouTubeCaptionOffGuard.cs#L299) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsNativeAndroidOrFireTv | [302:9](../YouTubeCaptionOffGuard.cs#L302) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsNativeAndroidOrFireTv.session | [302:53](../YouTubeCaptionOffGuard.cs#L302) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsNativeAndroidOrFireTv.client | [304:17](../YouTubeCaptionOffGuard.cs#L304) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsNativeAndroidOrFireTv.deviceName | [305:17](../YouTubeCaptionOffGuard.cs#L305) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsNativeAndroidOrFireTv.userAgent | [306:17](../YouTubeCaptionOffGuard.cs#L306) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.SupportsSetSubtitleStreamIndex | [317:9](../YouTubeCaptionOffGuard.cs#L317) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.SupportsSetSubtitleStreamIndex.session | [317:60](../YouTubeCaptionOffGuard.cs#L317) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.SupportsSetSubtitleStreamIndex.supportedCommands | [319:17](../YouTubeCaptionOffGuard.cs#L319) |
| iteration-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.SupportsSetSubtitleStreamIndex.command | [325:13](../YouTubeCaptionOffGuard.cs#L325) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsPaused | [334:9](../YouTubeCaptionOffGuard.cs#L334) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsPaused.e | [334:38](../YouTubeCaptionOffGuard.cs#L334) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsPaused.session | [334:67](../YouTubeCaptionOffGuard.cs#L334) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsSessionPaused | [337:9](../YouTubeCaptionOffGuard.cs#L337) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsSessionPaused.session | [337:45](../YouTubeCaptionOffGuard.cs#L337) |
| local-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsSessionPaused.playState | [339:17](../YouTubeCaptionOffGuard.cs#L339) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.GetStringProperty | [345:9](../YouTubeCaptionOffGuard.cs#L345) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.GetStringProperty.source | [345:49](../YouTubeCaptionOffGuard.cs#L345) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.GetStringProperty.name | [345:65](../YouTubeCaptionOffGuard.cs#L345) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.GetBooleanProperty | [350:9](../YouTubeCaptionOffGuard.cs#L350) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.GetBooleanProperty.source | [350:48](../YouTubeCaptionOffGuard.cs#L350) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.GetBooleanProperty.name | [350:64](../YouTubeCaptionOffGuard.cs#L350) |
| pattern-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.GetBooleanProperty.value | [356:48](../YouTubeCaptionOffGuard.cs#L356) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.ContainsIgnoreCase | [365:9](../YouTubeCaptionOffGuard.cs#L365) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.ContainsIgnoreCase.value | [365:48](../YouTubeCaptionOffGuard.cs#L365) |
| parameter | Emby.YouTubePlugin.YouTubeCaptionOffGuard.ContainsIgnoreCase.needle | [365:63](../YouTubeCaptionOffGuard.cs#L365) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.IsDisposingOrDisposed | [369:9](../YouTubeCaptionOffGuard.cs#L369) |
| method | Emby.YouTubePlugin.YouTubeCaptionOffGuard.Dispose | [372:9](../YouTubeCaptionOffGuard.cs#L372) |
| iteration-variable | Emby.YouTubePlugin.YouTubeCaptionOffGuard.Dispose.state | [387:17](../YouTubeCaptionOffGuard.cs#L387) |

</details>

<a id="file-f2ed4ae35969d6d0"></a>
## [YouTubeChannel.Aggregates.cs](../YouTubeChannel.Aggregates.cs)

Plugin Compile source (SDK default); 4987 bytes; 131 lines.

<details>
<summary>All 27 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [17:1](../YouTubeChannel.Aggregates.cs#L17) |
| type | Emby.YouTubePlugin.YouTubeChannel | [19:5](../YouTubeChannel.Aggregates.cs#L19) |
| method | Emby.YouTubePlugin.YouTubeChannel.LoadTrending | [22:9](../YouTubeChannel.Aggregates.cs#L22) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.apiKey | [23:13](../YouTubeChannel.Aggregates.cs#L23) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.ct | [23:28](../YouTubeChannel.Aggregates.cs#L23) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.region | [23:50](../YouTubeChannel.Aggregates.cs#L23) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.category | [23:70](../YouTubeChannel.Aggregates.cs#L23) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.allVideos | [25:17](../YouTubeChannel.Aggregates.cs#L25) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.seenIds | [26:17](../YouTubeChannel.Aggregates.cs#L26) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.cfg | [28:17](../YouTubeChannel.Aggregates.cs#L28) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.target | [29:23](../YouTubeChannel.Aggregates.cs#L29) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.showShorts | [30:18](../YouTubeChannel.Aggregates.cs#L30) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.isCategoryChild | [31:18](../YouTubeChannel.Aggregates.cs#L31) |
| local-function | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.TryAdd | [33:13](../YouTubeChannel.Aggregates.cs#L33) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.TryAdd.v | [33:25](../YouTubeChannel.Aggregates.cs#L33) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.TryAdd.rawId | [35:21](../YouTubeChannel.Aggregates.cs#L35) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.effectiveRegion | [43:21](../YouTubeChannel.Aggregates.cs#L43) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.buckets | [52:31](../YouTubeChannel.Aggregates.cs#L52) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.cat | [53:21](../YouTubeChannel.Aggregates.cs#L53) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.cat.doc | [57:35](../YouTubeChannel.Aggregates.cs#L57) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.cat.v | [60:25](../YouTubeChannel.Aggregates.cs#L60) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.doc | [70:31](../YouTubeChannel.Aggregates.cs#L70) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.v | [74:25](../YouTubeChannel.Aggregates.cs#L74) |
| catch-variable | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.ex | [86:19](../YouTubeChannel.Aggregates.cs#L86) |
| catch-variable | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.ex | [110:19](../YouTubeChannel.Aggregates.cs#L110) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.anonymous@4639 | [118:37](../YouTubeChannel.Aggregates.cs#L118) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadTrending.anonymous@4639.v | [118:37](../YouTubeChannel.Aggregates.cs#L118) |

</details>

<a id="file-d1cc636f42c38fb0"></a>
## [YouTubeChannel.DirectPlayback.cs](../YouTubeChannel.DirectPlayback.cs)

Plugin Compile source (SDK default); 56934 bytes; 869 lines.

<details>
<summary>All 363 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [18:1](../YouTubeChannel.DirectPlayback.cs#L18) |
| type | Emby.YouTubePlugin.YouTubeChannel | [20:5](../YouTubeChannel.DirectPlayback.cs#L20) |
| method | Emby.YouTubePlugin.YouTubeChannel.GetChannelItemMediaInfo | [22:9](../YouTubeChannel.DirectPlayback.cs#L22) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetChannelItemMediaInfo.id | [22:75](../YouTubeChannel.DirectPlayback.cs#L22) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetChannelItemMediaInfo.cancellationToken | [22:86](../YouTubeChannel.DirectPlayback.cs#L22) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelItemMediaInfo.live | [24:17](../YouTubeChannel.DirectPlayback.cs#L24) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelItemMediaInfo.videoId | [25:17](../YouTubeChannel.DirectPlayback.cs#L25) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.GetChannelItemMediaInfo.anonymous@983 | [27:53](../YouTubeChannel.DirectPlayback.cs#L27) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetChannelItemMediaInfo.anonymous@983.c | [27:53](../YouTubeChannel.DirectPlayback.cs#L27) |
| type | Emby.YouTubePlugin.YouTubeDirectPlayback | [37:5](../YouTubeChannel.DirectPlayback.cs#L37) |
| field | Emby.YouTubePlugin.YouTubeDirectPlayback.ManifestGate | [39:47](../YouTubeChannel.DirectPlayback.cs#L39) |
| field | Emby.YouTubePlugin.YouTubeDirectPlayback.MaximumManifestFiles | [40:27](../YouTubeChannel.DirectPlayback.cs#L40) |
| field | Emby.YouTubePlugin.YouTubeDirectPlayback.ManifestLifetime | [41:42](../YouTubeChannel.DirectPlayback.cs#L41) |
| field | Emby.YouTubePlugin.YouTubeDirectPlayback._fetchMaster | [44:109](../YouTubeChannel.DirectPlayback.cs#L44) |
| property | Emby.YouTubePlugin.YouTubeDirectPlayback.Enabled | [46:9](../YouTubeChannel.DirectPlayback.cs#L46) |
| method | Emby.YouTubePlugin.YouTubeDirectPlayback.IsVideoIdCharacter | [47:9](../YouTubeChannel.DirectPlayback.cs#L47) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.IsVideoIdCharacter.c | [47:49](../YouTubeChannel.DirectPlayback.cs#L47) |
| method | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync | [49:9](../YouTubeChannel.DirectPlayback.cs#L49) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.videoId | [49:87](../YouTubeChannel.DirectPlayback.cs#L49) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.cancellationToken | [49:103](../YouTubeChannel.DirectPlayback.cs#L49) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.stage | [52:17](../YouTubeChannel.DirectPlayback.cs#L52) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.resolved | [55:21](../YouTubeChannel.DirectPlayback.cs#L55) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.metadata | [58:21](../YouTubeChannel.DirectPlayback.cs#L58) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.formats | [61:65](../YouTubeChannel.DirectPlayback.cs#L61) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.maximumHeight | [64:21](../YouTubeChannel.DirectPlayback.cs#L64) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.candidates | [65:21](../YouTubeChannel.DirectPlayback.cs#L65) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.candidates.anonymous@3609 | [65:65](../YouTubeChannel.DirectPlayback.cs#L65) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.candidates.anonymous@3609.f | [65:65](../YouTubeChannel.DirectPlayback.cs#L65) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.hls | [67:21](../YouTubeChannel.DirectPlayback.cs#L67) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.hls.anonymous@3801 | [67:44](../YouTubeChannel.DirectPlayback.cs#L67) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.hls.anonymous@3801.f | [67:44](../YouTubeChannel.DirectPlayback.cs#L67) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.hls.anonymous@4086 | [70:40](../YouTubeChannel.DirectPlayback.cs#L70) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.hls.anonymous@4086.f | [70:40](../YouTubeChannel.DirectPlayback.cs#L70) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.hls.anonymous@4130 | [70:84](../YouTubeChannel.DirectPlayback.cs#L70) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.hls.anonymous@4130.f | [70:84](../YouTubeChannel.DirectPlayback.cs#L70) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.headers | [71:21](../YouTubeChannel.DirectPlayback.cs#L71) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.source | [72:33](../YouTubeChannel.DirectPlayback.cs#L72) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.masterUri | [76:22](../YouTubeChannel.DirectPlayback.cs#L76) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.managedMaster | [77:22](../YouTubeChannel.DirectPlayback.cs#L77) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.managedMaster.masterValue | [77:90](../YouTubeChannel.DirectPlayback.cs#L77) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.header | [89:21](../YouTubeChannel.DirectPlayback.cs#L89) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.bytes | [97:25](../YouTubeChannel.DirectPlayback.cs#L97) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.master | [101:25](../YouTubeChannel.DirectPlayback.cs#L101) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.advertised | [103:25](../YouTubeChannel.DirectPlayback.cs#L103) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.selected | [111:25](../YouTubeChannel.DirectPlayback.cs#L111) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.manifestPath | [113:25](../YouTubeChannel.DirectPlayback.cs#L113) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.selected | [133:25](../YouTubeChannel.DirectPlayback.cs#L133) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.video | [134:25](../YouTubeChannel.DirectPlayback.cs#L134) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.audio | [135:25](../YouTubeChannel.DirectPlayback.cs#L135) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.videoHeaders | [141:25](../YouTubeChannel.DirectPlayback.cs#L141) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.header | [142:21](../YouTubeChannel.DirectPlayback.cs#L142) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.videoPath | [144:25](../YouTubeChannel.DirectPlayback.cs#L144) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.audioPath | [145:29](../YouTubeChannel.DirectPlayback.cs#L145) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.audioHeaders | [148:29](../YouTubeChannel.DirectPlayback.cs#L148) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.header | [149:25](../YouTubeChannel.DirectPlayback.cs#L149) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.audioFormat | [155:25](../YouTubeChannel.DirectPlayback.cs#L155) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.audioStream | [156:25](../YouTubeChannel.DirectPlayback.cs#L156) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.isLive | [187:21](../YouTubeChannel.DirectPlayback.cs#L187) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.isLive.live | [188:67](../YouTubeChannel.DirectPlayback.cs#L188) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.duration | [191:21](../YouTubeChannel.DirectPlayback.cs#L191) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.anonymous@13803 | [205:67](../YouTubeChannel.DirectPlayback.cs#L205) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.anonymous@13803.s | [205:67](../YouTubeChannel.DirectPlayback.cs#L205) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.anonymous@14131 | [210:76](../YouTubeChannel.DirectPlayback.cs#L210) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.anonymous@14131.s | [210:76](../YouTubeChannel.DirectPlayback.cs#L210) |
| catch-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.GetMediaSourcesAsync.error | [220:19](../YouTubeChannel.DirectPlayback.cs#L220) |
| method | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireDirectPlaybackInfo | [229:9](../YouTubeChannel.DirectPlayback.cs#L229) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireDirectPlaybackInfo.response | [229:58](../YouTubeChannel.DirectPlayback.cs#L229) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireDirectPlaybackInfo.videoId | [229:75](../YouTubeChannel.DirectPlayback.cs#L229) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireDirectPlaybackInfo.property | [234:17](../YouTubeChannel.DirectPlayback.cs#L234) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireDirectPlaybackInfo.original | [235:17](../YouTubeChannel.DirectPlayback.cs#L235) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireDirectPlaybackInfo.sources | [236:17](../YouTubeChannel.DirectPlayback.cs#L236) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireDirectPlaybackInfo.sources.anonymous@15988 | [237:24](../YouTubeChannel.DirectPlayback.cs#L237) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireDirectPlaybackInfo.sources.anonymous@15988.s | [237:24](../YouTubeChannel.DirectPlayback.cs#L237) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireDirectPlaybackInfo.sources.anonymous@15988.anonymous@16101 | [238:44](../YouTubeChannel.DirectPlayback.cs#L238) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireDirectPlaybackInfo.sources.anonymous@15988.anonymous@16101.t | [238:44](../YouTubeChannel.DirectPlayback.cs#L238) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireDirectPlaybackInfo.sources.anonymous@15988.anonymous@16189 | [239:43](../YouTubeChannel.DirectPlayback.cs#L239) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireDirectPlaybackInfo.sources.anonymous@15988.anonymous@16189.t | [239:43](../YouTubeChannel.DirectPlayback.cs#L239) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireDirectPlaybackInfo.list | [246:40](../YouTubeChannel.DirectPlayback.cs#L246) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireDirectPlaybackInfo.i | [248:26](../YouTubeChannel.DirectPlayback.cs#L248) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireDirectPlaybackInfo.source | [249:56](../YouTubeChannel.DirectPlayback.cs#L249) |
| method | Emby.YouTubePlugin.YouTubeDirectPlayback.SourceId | [255:9](../YouTubeChannel.DirectPlayback.cs#L255) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.SourceId.videoId | [255:41](../YouTubeChannel.DirectPlayback.cs#L255) |
| method | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireOwnedMediaUri | [256:9](../YouTubeChannel.DirectPlayback.cs#L256) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireOwnedMediaUri.value | [256:53](../YouTubeChannel.DirectPlayback.cs#L256) |
| method | Emby.YouTubePlugin.YouTubeDirectPlayback.Text | [264:9](../YouTubeChannel.DirectPlayback.cs#L264) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.Text.value | [264:36](../YouTubeChannel.DirectPlayback.cs#L264) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.Text.name | [264:55](../YouTubeChannel.DirectPlayback.cs#L264) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.Text.p | [264:106](../YouTubeChannel.DirectPlayback.cs#L264) |
| method | Emby.YouTubePlugin.YouTubeDirectPlayback.Number | [265:9](../YouTubeChannel.DirectPlayback.cs#L265) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.Number.value | [265:39](../YouTubeChannel.DirectPlayback.cs#L265) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.Number.name | [265:58](../YouTubeChannel.DirectPlayback.cs#L265) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.Number.p | [265:109](../YouTubeChannel.DirectPlayback.cs#L265) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.Number.number | [265:177](../YouTubeChannel.DirectPlayback.cs#L265) |
| method | Emby.YouTubePlugin.YouTubeDirectPlayback.Integer | [266:9](../YouTubeChannel.DirectPlayback.cs#L266) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.Integer.value | [266:36](../YouTubeChannel.DirectPlayback.cs#L266) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.Integer.name | [266:55](../YouTubeChannel.DirectPlayback.cs#L266) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.Integer.p | [266:106](../YouTubeChannel.DirectPlayback.cs#L266) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.Integer.n | [266:173](../YouTubeChannel.DirectPlayback.cs#L266) |
| method | Emby.YouTubePlugin.YouTubeDirectPlayback.PositiveInteger | [267:9](../YouTubeChannel.DirectPlayback.cs#L267) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.PositiveInteger.value | [267:45](../YouTubeChannel.DirectPlayback.cs#L267) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.PositiveInteger.name | [267:64](../YouTubeChannel.DirectPlayback.cs#L267) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.PositiveInteger.number | [269:17](../YouTubeChannel.DirectPlayback.cs#L269) |
| method | Emby.YouTubePlugin.YouTubeDirectPlayback.BitsPerSecond | [272:9](../YouTubeChannel.DirectPlayback.cs#L272) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.BitsPerSecond.kbps | [272:43](../YouTubeChannel.DirectPlayback.cs#L272) |
| method | Emby.YouTubePlugin.YouTubeDirectPlayback.Headers | [273:9](../YouTubeChannel.DirectPlayback.cs#L273) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.Headers.value | [273:59](../YouTubeChannel.DirectPlayback.cs#L273) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.Headers.h | [273:156](../YouTubeChannel.DirectPlayback.cs#L273) |
| method | Emby.YouTubePlugin.YouTubeDirectPlayback.StoreManifestAsync | [275:9](../YouTubeChannel.DirectPlayback.cs#L275) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.StoreManifestAsync.content | [275:62](../YouTubeChannel.DirectPlayback.cs#L275) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.StoreManifestAsync.cancellationToken | [275:78](../YouTubeChannel.DirectPlayback.cs#L275) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.StoreManifestAsync.bytes | [277:17](../YouTubeChannel.DirectPlayback.cs#L277) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.StoreManifestAsync.root | [279:17](../YouTubeChannel.DirectPlayback.cs#L279) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.StoreManifestAsync.directory | [281:17](../YouTubeChannel.DirectPlayback.cs#L281) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.StoreManifestAsync.name | [282:17](../YouTubeChannel.DirectPlayback.cs#L282) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.StoreManifestAsync.path | [289:21](../YouTubeChannel.DirectPlayback.cs#L289) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.StoreManifestAsync.files | [296:21](../YouTubeChannel.DirectPlayback.cs#L296) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectPlayback.StoreManifestAsync.files.anonymous@20346 | [296:95](../YouTubeChannel.DirectPlayback.cs#L296) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.StoreManifestAsync.files.anonymous@20346.p | [296:95](../YouTubeChannel.DirectPlayback.cs#L296) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.StoreManifestAsync.file | [297:17](../YouTubeChannel.DirectPlayback.cs#L297) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectPlayback.StoreManifestAsync.file.anonymous@20428 | [297:50](../YouTubeChannel.DirectPlayback.cs#L297) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.StoreManifestAsync.file.anonymous@20428.f | [297:50](../YouTubeChannel.DirectPlayback.cs#L297) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.StoreManifestAsync.temporary | [300:21](../YouTubeChannel.DirectPlayback.cs#L300) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.StoreManifestAsync.stream | [303:32](../YouTubeChannel.DirectPlayback.cs#L303) |
| method | Emby.YouTubePlugin.YouTubeDirectPlayback.IsOwnedManifest | [318:9](../YouTubeChannel.DirectPlayback.cs#L318) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.IsOwnedManifest.path | [318:45](../YouTubeChannel.DirectPlayback.cs#L318) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.IsOwnedManifest.name | [320:17](../YouTubeChannel.DirectPlayback.cs#L320) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectPlayback.IsOwnedManifest.anonymous@21793 | [322:38](../YouTubeChannel.DirectPlayback.cs#L322) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.IsOwnedManifest.anonymous@21793.c | [322:38](../YouTubeChannel.DirectPlayback.cs#L322) |
| method | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireOwnedManifestPath | [325:9](../YouTubeChannel.DirectPlayback.cs#L325) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireOwnedManifestPath.path | [325:57](../YouTubeChannel.DirectPlayback.cs#L325) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireOwnedManifestPath.root | [327:17](../YouTubeChannel.DirectPlayback.cs#L327) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireOwnedManifestPath.directory | [330:17](../YouTubeChannel.DirectPlayback.cs#L330) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireOwnedManifestPath.fullPath | [331:17](../YouTubeChannel.DirectPlayback.cs#L331) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireOwnedManifestPath.comparison | [332:17](../YouTubeChannel.DirectPlayback.cs#L332) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireOwnedManifestPath.parent | [333:17](../YouTubeChannel.DirectPlayback.cs#L333) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectPlayback.RequireOwnedManifestPath.file | [334:17](../YouTubeChannel.DirectPlayback.cs#L334) |
| method | Emby.YouTubePlugin.YouTubeDirectPlayback.Chmod | [341:9](../YouTubeChannel.DirectPlayback.cs#L341) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.Chmod.path | [342:41](../YouTubeChannel.DirectPlayback.cs#L342) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.Chmod.mode | [342:54](../YouTubeChannel.DirectPlayback.cs#L342) |
| method | Emby.YouTubePlugin.YouTubeDirectPlayback.RestrictAccess | [343:9](../YouTubeChannel.DirectPlayback.cs#L343) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.RestrictAccess.path | [343:44](../YouTubeChannel.DirectPlayback.cs#L343) |
| parameter | Emby.YouTubePlugin.YouTubeDirectPlayback.RestrictAccess.mode | [343:57](../YouTubeChannel.DirectPlayback.cs#L343) |
| type | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility | [355:5](../YouTubeChannel.DirectPlayback.cs#L355) |
| field | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.HarmonyId | [357:30](../YouTubeChannel.DirectPlayback.cs#L357) |
| field | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.SourcePrefix | [358:30](../YouTubeChannel.DirectPlayback.cs#L358) |
| field | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Sync | [359:40](../YouTubeChannel.DirectPlayback.cs#L359) |
| field | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility._harmony | [360:32](../YouTubeChannel.DirectPlayback.cs#L360) |
| field | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility._harmonyType | [361:30](../YouTubeChannel.DirectPlayback.cs#L361) |
| field | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility._binding | [362:33](../YouTubeChannel.DirectPlayback.cs#L362) |
| type | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding | [364:9](../YouTubeChannel.DirectPlayback.cs#L364) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.Source | [364:39](../YouTubeChannel.DirectPlayback.cs#L364) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.OutputCodec | [364:60](../YouTubeChannel.DirectPlayback.cs#L364) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.Input0 | [365:13](../YouTubeChannel.DirectPlayback.cs#L365) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.Inputs | [365:34](../YouTubeChannel.DirectPlayback.cs#L365) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.Url | [365:55](../YouTubeChannel.DirectPlayback.cs#L365) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.Options | [366:13](../YouTubeChannel.DirectPlayback.cs#L366) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.Protocol | [366:35](../YouTubeChannel.DirectPlayback.cs#L366) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.HttpType | [366:58](../YouTubeChannel.DirectPlayback.cs#L366) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.HttpStrings | [367:13](../YouTubeChannel.DirectPlayback.cs#L367) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.InputOptions | [367:41](../YouTubeChannel.DirectPlayback.cs#L367) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.ResolutionWidth | [368:13](../YouTubeChannel.DirectPlayback.cs#L368) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.ResolutionHeight | [368:43](../YouTubeChannel.DirectPlayback.cs#L368) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.SupportedCodecs | [368:74](../YouTubeChannel.DirectPlayback.cs#L368) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.OriginalAudioStream | [369:13](../YouTubeChannel.DirectPlayback.cs#L369) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.BaseRequest | [369:47](../YouTubeChannel.DirectPlayback.cs#L369) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.StaticRequest | [369:73](../YouTubeChannel.DirectPlayback.cs#L369) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.TargetFrameRate | [370:13](../YouTubeChannel.DirectPlayback.cs#L370) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.Output0 | [370:41](../YouTubeChannel.DirectPlayback.cs#L370) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.OutputVideo | [370:63](../YouTubeChannel.DirectPlayback.cs#L370) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.VideoOptions | [371:13](../YouTubeChannel.DirectPlayback.cs#L371) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Binding.OutputFrameRate | [371:40](../YouTubeChannel.DirectPlayback.cs#L371) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.RequireInstalled | [373:9](../YouTubeChannel.DirectPlayback.cs#L373) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install | [380:9](../YouTubeChannel.DirectPlayback.cs#L380) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.stage | [385:21](../YouTubeChannel.DirectPlayback.cs#L385) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.encoder | [388:34](../YouTubeChannel.DirectPlayback.cs#L388) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.builder | [389:25](../YouTubeChannel.DirectPlayback.cs#L389) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.state | [390:25](../YouTubeChannel.DirectPlayback.cs#L390) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.command | [391:25](../YouTubeChannel.DirectPlayback.cs#L391) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.input | [392:25](../YouTubeChannel.DirectPlayback.cs#L392) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.output | [393:25](../YouTubeChannel.DirectPlayback.cs#L393) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.http | [394:25](../YouTubeChannel.DirectPlayback.cs#L394) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.resolution | [395:25](../YouTubeChannel.DirectPlayback.cs#L395) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.map | [398:25](../YouTubeChannel.DirectPlayback.cs#L398) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.setup | [399:25](../YouTubeChannel.DirectPlayback.cs#L399) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.resolutionLimits | [400:25](../YouTubeChannel.DirectPlayback.cs#L400) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.outputStream | [403:25](../YouTubeChannel.DirectPlayback.cs#L403) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.audioChannels | [405:25](../YouTubeChannel.DirectPlayback.cs#L405) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.audioSelector | [411:25](../YouTubeChannel.DirectPlayback.cs#L411) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.audioSelector.anonymous@27289 | [413:33](../YouTubeChannel.DirectPlayback.cs#L413) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.audioSelector.anonymous@27289.m | [413:33](../YouTubeChannel.DirectPlayback.cs#L413) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.audioSelector.anonymous@27289.p | [414:57](../YouTubeChannel.DirectPlayback.cs#L414) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.codecSelector | [419:25](../YouTubeChannel.DirectPlayback.cs#L419) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.frameRateSelector | [424:25](../YouTubeChannel.DirectPlayback.cs#L424) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.videoArguments | [428:25](../YouTubeChannel.DirectPlayback.cs#L428) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.videoArguments.anonymous@28770 | [429:33](../YouTubeChannel.DirectPlayback.cs#L429) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.videoArguments.anonymous@28770.m | [429:33](../YouTubeChannel.DirectPlayback.cs#L429) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.videoArguments.anonymous@28770.p | [431:57](../YouTubeChannel.DirectPlayback.cs#L431) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.segmentPolicyType | [439:25](../YouTubeChannel.DirectPlayback.cs#L439) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.segmentPolicy | [440:33](../YouTubeChannel.DirectPlayback.cs#L440) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.options | [450:25](../YouTubeChannel.DirectPlayback.cs#L450) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.baseRequest | [451:25](../YouTubeChannel.DirectPlayback.cs#L451) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.output0 | [452:25](../YouTubeChannel.DirectPlayback.cs#L452) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.outputVideo | [453:25](../YouTubeChannel.DirectPlayback.cs#L453) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.videoOptions | [456:25](../YouTubeChannel.DirectPlayback.cs#L456) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.binding | [461:25](../YouTubeChannel.DirectPlayback.cs#L461) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.binding.anonymous@31860 | [467:77](../YouTubeChannel.DirectPlayback.cs#L467) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.binding.anonymous@31860.n | [467:77](../YouTubeChannel.DirectPlayback.cs#L467) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.binding.anonymous@31974 | [468:58](../YouTubeChannel.DirectPlayback.cs#L468) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.binding.anonymous@31974.n | [468:58](../YouTubeChannel.DirectPlayback.cs#L468) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.assembly | [482:25](../YouTubeChannel.DirectPlayback.cs#L482) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.methodType | [484:25](../YouTubeChannel.DirectPlayback.cs#L484) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.patch | [486:25](../YouTubeChannel.DirectPlayback.cs#L486) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.patch.anonymous@33713 | [486:66](../YouTubeChannel.DirectPlayback.cs#L486) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.patch.anonymous@33713.m | [486:66](../YouTubeChannel.DirectPlayback.cs#L486) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.patch.anonymous@33713.anonymous@33902 | [488:50](../YouTubeChannel.DirectPlayback.cs#L488) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.patch.anonymous@33713.anonymous@33902.p | [488:50](../YouTubeChannel.DirectPlayback.cs#L488) |
| local-function | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.Add | [489:21](../YouTubeChannel.DirectPlayback.cs#L489) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.Add.original | [489:30](../YouTubeChannel.DirectPlayback.cs#L489) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.Add.name | [489:51](../YouTubeChannel.DirectPlayback.cs#L489) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.Add.isPrefix | [489:64](../YouTubeChannel.DirectPlayback.cs#L489) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.Add.callback | [491:29](../YouTubeChannel.DirectPlayback.cs#L491) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.Add.patchMethod | [492:29](../YouTubeChannel.DirectPlayback.cs#L492) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.Add.parameters | [493:29](../YouTubeChannel.DirectPlayback.cs#L493) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.Add.arguments | [494:29](../YouTubeChannel.DirectPlayback.cs#L494) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.Add.i | [496:34](../YouTubeChannel.DirectPlayback.cs#L496) |
| catch-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Install.error | [517:23](../YouTubeChannel.DirectPlayback.cs#L517) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Uninstall | [527:9](../YouTubeChannel.DirectPlayback.cs#L527) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.MapStreamsPostfix | [543:9](../YouTubeChannel.DirectPlayback.cs#L543) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.MapStreamsPostfix.__0 | [543:47](../YouTubeChannel.DirectPlayback.cs#L543) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.MapStreamsPostfix.__1 | [543:59](../YouTubeChannel.DirectPlayback.cs#L543) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.SetupInputPostfix | [544:9](../YouTubeChannel.DirectPlayback.cs#L544) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.SetupInputPostfix.__0 | [544:47](../YouTubeChannel.DirectPlayback.cs#L544) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.SetupInputPostfix.__1 | [544:59](../YouTubeChannel.DirectPlayback.cs#L544) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.CodecResolutionPostfix | [545:9](../YouTubeChannel.DirectPlayback.cs#L545) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.CodecResolutionPostfix.__0 | [545:52](../YouTubeChannel.DirectPlayback.cs#L545) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.CodecResolutionPostfix.__3 | [545:64](../YouTubeChannel.DirectPlayback.cs#L545) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.FrameRatePostfix | [546:9](../YouTubeChannel.DirectPlayback.cs#L546) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.FrameRatePostfix.__instance | [546:46](../YouTubeChannel.DirectPlayback.cs#L546) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.FrameRatePostfix.__0 | [546:65](../YouTubeChannel.DirectPlayback.cs#L546) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.FrameRatePostfix.__1 | [546:77](../YouTubeChannel.DirectPlayback.cs#L546) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.FrameRatePostfix.__result | [546:87](../YouTubeChannel.DirectPlayback.cs#L546) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.FrameRatePostfix.binding | [550:17](../YouTubeChannel.DirectPlayback.cs#L550) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.FrameRatePostfix.source | [552:17](../YouTubeChannel.DirectPlayback.cs#L552) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.FrameRatePostfix.request | [555:17](../YouTubeChannel.DirectPlayback.cs#L555) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.FrameRatePostfix.effective | [570:25](../YouTubeChannel.DirectPlayback.cs#L570) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.VideoArgumentsPostfix | [582:9](../YouTubeChannel.DirectPlayback.cs#L582) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.VideoArgumentsPostfix.__0 | [582:51](../YouTubeChannel.DirectPlayback.cs#L582) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.VideoArgumentsPostfix.__3 | [582:63](../YouTubeChannel.DirectPlayback.cs#L582) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.VideoArgumentsPostfix.binding | [584:17](../YouTubeChannel.DirectPlayback.cs#L584) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.VideoArgumentsPostfix.source | [586:17](../YouTubeChannel.DirectPlayback.cs#L586) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.VideoArgumentsPostfix.codec | [588:17](../YouTubeChannel.DirectPlayback.cs#L588) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.VideoArgumentsPostfix.request | [589:17](../YouTubeChannel.DirectPlayback.cs#L589) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.VideoArgumentsPostfix.output | [599:21](../YouTubeChannel.DirectPlayback.cs#L599) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.VideoArgumentsPostfix.video | [600:21](../YouTubeChannel.DirectPlayback.cs#L600) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.VideoArgumentsPostfix.options | [601:21](../YouTubeChannel.DirectPlayback.cs#L601) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.VideoArgumentsPostfix.oldText | [603:21](../YouTubeChannel.DirectPlayback.cs#L603) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.VideoArgumentsPostfix.oldRate | [604:21](../YouTubeChannel.DirectPlayback.cs#L604) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.VideoArgumentsPostfix.requested | [608:21](../YouTubeChannel.DirectPlayback.cs#L608) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.VideoArgumentsPostfix.validRequested | [609:21](../YouTubeChannel.DirectPlayback.cs#L609) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.VideoArgumentsPostfix.rate | [610:21](../YouTubeChannel.DirectPlayback.cs#L610) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.VideoArgumentsPostfix.cappedRate | [613:21](../YouTubeChannel.DirectPlayback.cs#L613) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ParseOutputFrameRate | [629:9](../YouTubeChannel.DirectPlayback.cs#L629) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ParseOutputFrameRate.text | [629:53](../YouTubeChannel.DirectPlayback.cs#L629) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ParseOutputFrameRate.parts | [632:17](../YouTubeChannel.DirectPlayback.cs#L632) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ParseOutputFrameRate.numerator | [634:105](../YouTubeChannel.DirectPlayback.cs#L634) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ParseOutputFrameRate.denominator | [636:20](../YouTubeChannel.DirectPlayback.cs#L636) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ParseOutputFrameRate.rate | [639:17](../YouTubeChannel.DirectPlayback.cs#L639) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.PreferH264Prefix | [642:9](../YouTubeChannel.DirectPlayback.cs#L642) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.PreferH264Prefix.__0 | [642:46](../YouTubeChannel.DirectPlayback.cs#L642) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.PreferH264Prefix.binding | [644:17](../YouTubeChannel.DirectPlayback.cs#L644) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.PreferH264Prefix.source | [646:17](../YouTubeChannel.DirectPlayback.cs#L646) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.PreferH264Prefix.codecs | [648:17](../YouTubeChannel.DirectPlayback.cs#L648) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.PreferH264Prefix.index | [650:17](../YouTubeChannel.DirectPlayback.cs#L650) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.PreferH264Prefix.index.anonymous@43564 | [650:49](../YouTubeChannel.DirectPlayback.cs#L650) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.PreferH264Prefix.index.anonymous@43564.c | [650:49](../YouTubeChannel.DirectPlayback.cs#L650) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.PreferH264Prefix.preferred | [662:21](../YouTubeChannel.DirectPlayback.cs#L662) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.GetStereoOrUnknownAudio | [674:9](../YouTubeChannel.DirectPlayback.cs#L674) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.GetStereoOrUnknownAudio.state | [674:61](../YouTubeChannel.DirectPlayback.cs#L674) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.GetStereoOrUnknownAudio.outputStream | [674:75](../YouTubeChannel.DirectPlayback.cs#L674) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.GetStereoOrUnknownAudio.binding | [676:17](../YouTubeChannel.DirectPlayback.cs#L676) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.GetStereoOrUnknownAudio.source | [678:17](../YouTubeChannel.DirectPlayback.cs#L678) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.GetStereoOrUnknownAudio.audio | [680:17](../YouTubeChannel.DirectPlayback.cs#L680) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.AudioChannelsPostfix | [697:9](../YouTubeChannel.DirectPlayback.cs#L697) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.AudioChannelsPostfix.__0 | [697:50](../YouTubeChannel.DirectPlayback.cs#L697) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.AudioChannelsPostfix.__1 | [697:62](../YouTubeChannel.DirectPlayback.cs#L697) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.AudioChannelsPostfix.__2 | [697:74](../YouTubeChannel.DirectPlayback.cs#L697) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.AudioChannelsPostfix.__result | [697:86](../YouTubeChannel.DirectPlayback.cs#L697) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.AudioChannelsPostfix.audio | [700:17](../YouTubeChannel.DirectPlayback.cs#L700) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.AudioChannelsPostfix.maximum | [705:17](../YouTubeChannel.DirectPlayback.cs#L705) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.PreferAacPrefix | [709:9](../YouTubeChannel.DirectPlayback.cs#L709) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.PreferAacPrefix.__0 | [709:45](../YouTubeChannel.DirectPlayback.cs#L709) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.PreferAacPrefix.__1 | [709:57](../YouTubeChannel.DirectPlayback.cs#L709) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.PreferAacPrefix.__2 | [709:69](../YouTubeChannel.DirectPlayback.cs#L709) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.PreferAacPrefix.index | [712:17](../YouTubeChannel.DirectPlayback.cs#L712) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.PreferAacPrefix.index.anonymous@46942 | [712:46](../YouTubeChannel.DirectPlayback.cs#L712) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.PreferAacPrefix.index.anonymous@46942.c | [712:46](../YouTubeChannel.DirectPlayback.cs#L712) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.PreferAacPrefix.preferred | [716:17](../YouTubeChannel.DirectPlayback.cs#L716) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.SegmentLengthPostfix | [722:9](../YouTubeChannel.DirectPlayback.cs#L722) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.SegmentLengthPostfix.__0 | [722:50](../YouTubeChannel.DirectPlayback.cs#L722) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.SegmentLengthPostfix.__1 | [722:62](../YouTubeChannel.DirectPlayback.cs#L722) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.SegmentLengthPostfix.__result | [722:71](../YouTubeChannel.DirectPlayback.cs#L722) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.SegmentLengthPostfix.binding | [724:17](../YouTubeChannel.DirectPlayback.cs#L724) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.SegmentLengthPostfix.source | [726:17](../YouTubeChannel.DirectPlayback.cs#L726) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.IsCanonicalDirectSource | [745:9](../YouTubeChannel.DirectPlayback.cs#L745) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.IsCanonicalDirectSource.source | [745:53](../YouTubeChannel.DirectPlayback.cs#L745) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ApplyOutputLimit | [750:9](../YouTubeChannel.DirectPlayback.cs#L750) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ApplyOutputLimit.state | [750:46](../YouTubeChannel.DirectPlayback.cs#L750) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ApplyOutputLimit.resolution | [750:60](../YouTubeChannel.DirectPlayback.cs#L750) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ApplyOutputLimit.binding | [752:17](../YouTubeChannel.DirectPlayback.cs#L752) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ApplyOutputLimit.source | [754:17](../YouTubeChannel.DirectPlayback.cs#L754) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ApplyOutputLimit.maximumHeight | [764:21](../YouTubeChannel.DirectPlayback.cs#L764) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ApplyOutputLimit.maximumWidth | [768:21](../YouTubeChannel.DirectPlayback.cs#L768) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ApplyOutputLimit.previousWidth | [769:21](../YouTubeChannel.DirectPlayback.cs#L769) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ApplyOutputLimit.previousHeight | [770:21](../YouTubeChannel.DirectPlayback.cs#L770) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply | [785:9](../YouTubeChannel.DirectPlayback.cs#L785) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.state | [785:35](../YouTubeChannel.DirectPlayback.cs#L785) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.command | [785:49](../YouTubeChannel.DirectPlayback.cs#L785) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.requireMappedInput | [785:65](../YouTubeChannel.DirectPlayback.cs#L785) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.binding | [787:17](../YouTubeChannel.DirectPlayback.cs#L787) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.source | [792:17](../YouTubeChannel.DirectPlayback.cs#L792) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.manifest | [799:25](../YouTubeChannel.DirectPlayback.cs#L799) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.hlsInput | [800:25](../YouTubeChannel.DirectPlayback.cs#L800) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.hlsOptions | [803:25](../YouTubeChannel.DirectPlayback.cs#L803) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.anonymous@52497 | [804:49](../YouTubeChannel.DirectPlayback.cs#L804) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.anonymous@52497.p | [804:49](../YouTubeChannel.DirectPlayback.cs#L804) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.tracks | [813:17](../YouTubeChannel.DirectPlayback.cs#L813) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.tracks.anonymous@52884 | [813:53](../YouTubeChannel.DirectPlayback.cs#L813) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.tracks.anonymous@52884.s | [813:53](../YouTubeChannel.DirectPlayback.cs#L813) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.videoUrl | [819:21](../YouTubeChannel.DirectPlayback.cs#L819) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.audioUrl | [820:21](../YouTubeChannel.DirectPlayback.cs#L820) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.primary | [821:21](../YouTubeChannel.DirectPlayback.cs#L821) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.inputs | [824:21](../YouTubeChannel.DirectPlayback.cs#L824) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.inputs.anonymous@53843 | [825:43](../YouTubeChannel.DirectPlayback.cs#L825) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.inputs.anonymous@53843.i | [825:43](../YouTubeChannel.DirectPlayback.cs#L825) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.outputCodec | [830:21](../YouTubeChannel.DirectPlayback.cs#L830) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.protocol | [834:21](../YouTubeChannel.DirectPlayback.cs#L834) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.copiedProtocol | [836:21](../YouTubeChannel.DirectPlayback.cs#L836) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.property | [837:17](../YouTubeChannel.DirectPlayback.cs#L837) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.primaryOptions | [838:21](../YouTubeChannel.DirectPlayback.cs#L838) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.audioOptions | [839:21](../YouTubeChannel.DirectPlayback.cs#L839) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Apply.property | [840:17](../YouTubeChannel.DirectPlayback.cs#L840) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.FindType | [849:9](../YouTubeChannel.DirectPlayback.cs#L849) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.FindType.name | [849:38](../YouTubeChannel.DirectPlayback.cs#L849) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.FindType.anonymous@55717 | [850:21](../YouTubeChannel.DirectPlayback.cs#L850) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.FindType.anonymous@55717.a | [850:21](../YouTubeChannel.DirectPlayback.cs#L850) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.FindType.anonymous@55761 | [850:65](../YouTubeChannel.DirectPlayback.cs#L850) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.FindType.anonymous@55761.t | [850:65](../YouTubeChannel.DirectPlayback.cs#L850) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Property | [851:9](../YouTubeChannel.DirectPlayback.cs#L851) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Property.type | [851:46](../YouTubeChannel.DirectPlayback.cs#L851) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Property.name | [851:57](../YouTubeChannel.DirectPlayback.cs#L851) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Property.expected | [851:70](../YouTubeChannel.DirectPlayback.cs#L851) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Property.writable | [851:93](../YouTubeChannel.DirectPlayback.cs#L851) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.Property.property | [853:17](../YouTubeChannel.DirectPlayback.cs#L853) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ExactStaticMethod | [858:9](../YouTubeChannel.DirectPlayback.cs#L858) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ExactStaticMethod.type | [858:53](../YouTubeChannel.DirectPlayback.cs#L858) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ExactStaticMethod.name | [858:64](../YouTubeChannel.DirectPlayback.cs#L858) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ExactStaticMethod.parameters | [858:77](../YouTubeChannel.DirectPlayback.cs#L858) |
| method | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ExactStaticReturning | [861:9](../YouTubeChannel.DirectPlayback.cs#L861) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ExactStaticReturning.returns | [861:56](../YouTubeChannel.DirectPlayback.cs#L861) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ExactStaticReturning.type | [861:70](../YouTubeChannel.DirectPlayback.cs#L861) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ExactStaticReturning.name | [861:81](../YouTubeChannel.DirectPlayback.cs#L861) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ExactStaticReturning.parameters | [861:94](../YouTubeChannel.DirectPlayback.cs#L861) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSeekCompatibility.ExactStaticReturning.method | [863:17](../YouTubeChannel.DirectPlayback.cs#L863) |

</details>

<a id="file-d51ff2da196617b0"></a>
## [YouTubeChannel.Enrichment.cs](../YouTubeChannel.Enrichment.cs)

Plugin Compile source (SDK default); 31509 bytes; 580 lines.

<details>
<summary>All 130 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [17:1](../YouTubeChannel.Enrichment.cs#L17) |
| type | Emby.YouTubePlugin.YouTubeChannel | [19:5](../YouTubeChannel.Enrichment.cs#L19) |
| method | Emby.YouTubePlugin.YouTubeChannel.HasShortsTagOrHashtag | [23:9](../YouTubeChannel.Enrichment.cs#L23) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.HasShortsTagOrHashtag.detail | [23:51](../YouTubeChannel.Enrichment.cs#L23) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.HasShortsTagOrHashtag.snip | [25:59](../YouTubeChannel.Enrichment.cs#L25) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.HasShortsTagOrHashtag.tags | [26:53](../YouTubeChannel.Enrichment.cs#L26) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.HasShortsTagOrHashtag.tag | [27:17](../YouTubeChannel.Enrichment.cs#L27) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.HasShortsTagOrHashtag.title | [30:17](../YouTubeChannel.Enrichment.cs#L30) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.HasShortsTagOrHashtag.desc | [31:17](../YouTubeChannel.Enrichment.cs#L31) |
| method | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync | [39:9](../YouTubeChannel.Enrichment.cs#L39) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.batch | [40:13](../YouTubeChannel.Enrichment.cs#L40) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.ct | [41:13](../YouTubeChannel.Enrichment.cs#L41) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.removeUnknown | [42:13](../YouTubeChannel.Enrichment.cs#L42) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.candidates | [46:17](../YouTubeChannel.Enrichment.cs#L46) |
| tuple-element | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.candidates.item | [46:40](../YouTubeChannel.Enrichment.cs#L46) |
| tuple-element | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.candidates.id | [46:62](../YouTubeChannel.Enrichment.cs#L46) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.item | [47:13](../YouTubeChannel.Enrichment.cs#L47) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.item.rawId | [53:21](../YouTubeChannel.Enrichment.cs#L53) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.item.secs | [62:25](../YouTubeChannel.Enrichment.cs#L62) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.item.meta | [65:65](../YouTubeChannel.Enrichment.cs#L65) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.classifications | [76:17](../YouTubeChannel.Enrichment.cs#L76) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.sem | [77:23](../YouTubeChannel.Enrichment.cs#L77) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.anonymous@3693 | [78:50](../YouTubeChannel.Enrichment.cs#L78) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.anonymous@3693.c | [78:56](../YouTubeChannel.Enrichment.cs#L78) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.candidate | [90:13](../YouTubeChannel.Enrichment.cs#L90) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.candidate.classification | [92:72](../YouTubeChannel.Enrichment.cs#L92) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.anonymous@5041 | [107:33](../YouTubeChannel.Enrichment.cs#L107) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.anonymous@5041.item | [107:33](../YouTubeChannel.Enrichment.cs#L107) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.anonymous@5041.rawId | [110:25](../YouTubeChannel.Enrichment.cs#L110) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsProbeUpgradeAsync.anonymous@5041.classification | [111:71](../YouTubeChannel.Enrichment.cs#L111) |
| method | Emby.YouTubePlugin.YouTubeChannel.EvictExpiredMetaCache | [117:9](../YouTubeChannel.Enrichment.cs#L117) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EvictExpiredMetaCache.now | [120:17](../YouTubeChannel.Enrichment.cs#L120) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.EvictExpiredMetaCache.kvp | [121:13](../YouTubeChannel.Enrichment.cs#L121) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EvictExpiredMetaCache.oldest | [127:17](../YouTubeChannel.Enrichment.cs#L127) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.EvictExpiredMetaCache.oldest.anonymous@5895 | [128:26](../YouTubeChannel.Enrichment.cs#L128) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.EvictExpiredMetaCache.oldest.anonymous@5895.kvp | [128:26](../YouTubeChannel.Enrichment.cs#L128) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.EvictExpiredMetaCache.oldest.anonymous@6007 | [130:25](../YouTubeChannel.Enrichment.cs#L130) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.EvictExpiredMetaCache.oldest.anonymous@6007.kvp | [130:25](../YouTubeChannel.Enrichment.cs#L130) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.EvictExpiredMetaCache.key | [132:13](../YouTubeChannel.Enrichment.cs#L132) |
| method | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch | [136:9](../YouTubeChannel.Enrichment.cs#L136) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.apiKey | [137:13](../YouTubeChannel.Enrichment.cs#L137) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batch | [137:28](../YouTubeChannel.Enrichment.cs#L137) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.videoIds | [137:57](../YouTubeChannel.Enrichment.cs#L137) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.ct | [138:13](../YouTubeChannel.Enrichment.cs#L138) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.knownShortsIds | [139:13](../YouTubeChannel.Enrichment.cs#L139) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.excludeUnknownShortCandidates | [140:13](../YouTubeChannel.Enrichment.cs#L140) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.foundIds | [145:17](../YouTubeChannel.Enrichment.cs#L145) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.unplayableIds | [152:17](../YouTubeChannel.Enrichment.cs#L152) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.queriedIds | [155:17](../YouTubeChannel.Enrichment.cs#L155) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.unresolvedShortCandidates | [160:17](../YouTubeChannel.Enrichment.cs#L160) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.serverRegion | [165:21](../YouTubeChannel.Enrichment.cs#L165) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.i | [172:26](../YouTubeChannel.Enrichment.cs#L172) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.chunkList | [174:25](../YouTubeChannel.Enrichment.cs#L174) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.doc | [175:31](../YouTubeChannel.Enrichment.cs#L175) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.qid | [178:21](../YouTubeChannel.Enrichment.cs#L178) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.items | [180:73](../YouTubeChannel.Enrichment.cs#L180) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.detailsMap | [183:29](../YouTubeChannel.Enrichment.cs#L183) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.item | [184:25](../YouTubeChannel.Enrichment.cs#L184) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.item.id | [186:33](../YouTubeChannel.Enrichment.cs#L186) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.probeCandidates | [204:29](../YouTubeChannel.Enrichment.cs#L204) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.probeResults | [205:29](../YouTubeChannel.Enrichment.cs#L205) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.vid | [206:39](../YouTubeChannel.Enrichment.cs#L206) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.det | [206:44](../YouTubeChannel.Enrichment.cs#L206) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.cachedProbe | [218:78](../YouTubeChannel.Enrichment.cs#L218) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.durStr | [226:33](../YouTubeChannel.Enrichment.cs#L226) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.tsDet | [227:33](../YouTubeChannel.Enrichment.cs#L227) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.sem | [233:39](../YouTubeChannel.Enrichment.cs#L233) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.anonymous@11967 | [234:71](../YouTubeChannel.Enrichment.cs#L234) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.anonymous@11967.vid | [234:77](../YouTubeChannel.Enrichment.cs#L234) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem | [245:25](../YouTubeChannel.Enrichment.cs#L245) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.rawId | [247:33](../YouTubeChannel.Enrichment.cs#L247) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.detail | [253:72](../YouTubeChannel.Enrichment.cs#L253) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.isLiveStream | [257:34](../YouTubeChannel.Enrichment.cs#L257) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.statusEl | [258:73](../YouTubeChannel.Enrichment.cs#L258) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.privacy | [261:37](../YouTubeChannel.Enrichment.cs#L261) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.upload | [262:37](../YouTubeChannel.Enrichment.cs#L262) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.embeddable | [263:38](../YouTubeChannel.Enrichment.cs#L263) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.embEl | [264:83](../YouTubeChannel.Enrichment.cs#L264) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.cdEl | [282:81](../YouTubeChannel.Enrichment.cs#L282) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.crEl | [284:81](../YouTubeChannel.Enrichment.cs#L284) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.ytrEl | [286:76](../YouTubeChannel.Enrichment.cs#L286) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.rrEl | [298:85](../YouTubeChannel.Enrichment.cs#L298) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.blocked | [301:38](../YouTubeChannel.Enrichment.cs#L301) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.blockedEl | [302:76](../YouTubeChannel.Enrichment.cs#L302) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.b | [305:37](../YouTubeChannel.Enrichment.cs#L305) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.allowedEl | [312:79](../YouTubeChannel.Enrichment.cs#L312) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.inAllowed | [315:42](../YouTubeChannel.Enrichment.cs#L315) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.a | [316:37](../YouTubeChannel.Enrichment.cs#L316) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.duration | [332:33](../YouTubeChannel.Enrichment.cs#L332) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.ts | [333:33](../YouTubeChannel.Enrichment.cs#L333) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.durationAllowsShort | [356:34](../YouTubeChannel.Enrichment.cs#L356) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.shortsClassification | [360:33](../YouTubeChannel.Enrichment.cs#L360) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.isShort | [386:34](../YouTubeChannel.Enrichment.cs#L386) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.desc | [397:33](../YouTubeChannel.Enrichment.cs#L397) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.viewCount | [398:35](../YouTubeChannel.Enrichment.cs#L398) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.likeCount | [399:35](../YouTubeChannel.Enrichment.cs#L399) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.commentCount | [400:35](../YouTubeChannel.Enrichment.cs#L400) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.stats | [401:77](../YouTubeChannel.Enrichment.cs#L401) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.v | [403:101](../YouTubeChannel.Enrichment.cs#L403) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.l | [404:101](../YouTubeChannel.Enrichment.cs#L404) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.c | [405:104](../YouTubeChannel.Enrichment.cs#L405) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.apiOverview | [408:37](../YouTubeChannel.Enrichment.cs#L408) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.overview | [409:37](../YouTubeChannel.Enrichment.cs#L409) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.pubStr | [416:33](../YouTubeChannel.Enrichment.cs#L416) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.premiere | [417:33](../YouTubeChannel.Enrichment.cs#L417) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.isLiveOrUpcoming | [428:33](../YouTubeChannel.Enrichment.cs#L428) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.batchItem.origLang | [447:33](../YouTubeChannel.Enrichment.cs#L447) |
| catch-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.ex | [468:19](../YouTubeChannel.Enrichment.cs#L468) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.anonymous@26579 | [480:37](../YouTubeChannel.Enrichment.cs#L480) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.anonymous@26579.item | [480:37](../YouTubeChannel.Enrichment.cs#L480) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.anonymous@26579.raw | [482:29](../YouTubeChannel.Enrichment.cs#L482) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.anonymous@27266 | [494:37](../YouTubeChannel.Enrichment.cs#L494) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.anonymous@27266.item | [494:37](../YouTubeChannel.Enrichment.cs#L494) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EnrichBatch.anonymous@27266.raw | [496:29](../YouTubeChannel.Enrichment.cs#L496) |
| method | Emby.YouTubePlugin.YouTubeChannel.IsLiveOrUpcomingVideo | [507:9](../YouTubeChannel.Enrichment.cs#L507) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.IsLiveOrUpcomingVideo.detail | [507:51](../YouTubeChannel.Enrichment.cs#L507) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.IsLiveOrUpcomingVideo.broadcastState | [509:17](../YouTubeChannel.Enrichment.cs#L509) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.IsLiveOrUpcomingVideo.liveDetails | [516:72](../YouTubeChannel.Enrichment.cs#L516) |
| method | Emby.YouTubePlugin.YouTubeChannel.RemoveLivePrefix | [528:9](../YouTubeChannel.Enrichment.cs#L528) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.RemoveLivePrefix.value | [528:48](../YouTubeChannel.Enrichment.cs#L528) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.RemoveLivePrefix.prefix | [530:26](../YouTubeChannel.Enrichment.cs#L530) |
| method | Emby.YouTubePlugin.YouTubeChannel.ApplyCachedMeta | [536:9](../YouTubeChannel.Enrichment.cs#L536) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ApplyCachedMeta.batch | [536:45](../YouTubeChannel.Enrichment.cs#L536) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyCachedMeta.now | [538:17](../YouTubeChannel.Enrichment.cs#L538) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyCachedMeta.item | [539:13](../YouTubeChannel.Enrichment.cs#L539) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyCachedMeta.item.cached | [541:61](../YouTubeChannel.Enrichment.cs#L541) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyCachedMeta.item.recent | [549:87](../YouTubeChannel.Enrichment.cs#L549) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyCachedMeta.item.overview | [558:21](../YouTubeChannel.Enrichment.cs#L558) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyCachedMeta.item.runtime | [573:25](../YouTubeChannel.Enrichment.cs#L573) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyCachedMeta.item.runtime.direct | [573:94](../YouTubeChannel.Enrichment.cs#L573) |

</details>

<a id="file-3cf86b8d83708a69"></a>
## [YouTubeChannel.Folders.cs](../YouTubeChannel.Folders.cs)

Plugin Compile source (SDK default); 14906 bytes; 353 lines.

<details>
<summary>All 108 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [17:1](../YouTubeChannel.Folders.cs#L17) |
| type | Emby.YouTubePlugin.YouTubeChannel | [19:5](../YouTubeChannel.Folders.cs#L19) |
| method | Emby.YouTubePlugin.YouTubeChannel.BuildRootItemsAsync | [21:9](../YouTubeChannel.Folders.cs#L21) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.BuildRootItemsAsync.apiKey | [22:13](../YouTubeChannel.Folders.cs#L22) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.BuildRootItemsAsync.config | [22:28](../YouTubeChannel.Folders.cs#L22) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.BuildRootItemsAsync.ct | [22:56](../YouTubeChannel.Folders.cs#L22) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.BuildRootItemsAsync.items | [24:17](../YouTubeChannel.Folders.cs#L24) |
| method | Emby.YouTubePlugin.YouTubeChannel.AddWatchLaterFoldersAsync | [40:9](../YouTubeChannel.Folders.cs#L40) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.AddWatchLaterFoldersAsync.items | [41:13](../YouTubeChannel.Folders.cs#L41) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.AddWatchLaterFoldersAsync.apiKey | [41:42](../YouTubeChannel.Folders.cs#L41) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.AddWatchLaterFoldersAsync.config | [41:57](../YouTubeChannel.Folders.cs#L41) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.AddWatchLaterFoldersAsync.ct | [41:85](../YouTubeChannel.Folders.cs#L41) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.AddWatchLaterFoldersAsync.playlistIds | [43:17](../YouTubeChannel.Folders.cs#L43) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.AddWatchLaterFoldersAsync.index | [47:22](../YouTubeChannel.Folders.cs#L47) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.AddWatchLaterFoldersAsync.playlistId | [50:21](../YouTubeChannel.Folders.cs#L50) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.AddWatchLaterFoldersAsync.details | [62:21](../YouTubeChannel.Folders.cs#L62) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.AddWatchLaterFoldersAsync.displayName | [70:21](../YouTubeChannel.Folders.cs#L70) |
| method | Emby.YouTubePlugin.YouTubeChannel.AddSavedContentFoldersAsync | [81:9](../YouTubeChannel.Folders.cs#L81) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.AddSavedContentFoldersAsync.items | [82:13](../YouTubeChannel.Folders.cs#L82) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.AddSavedContentFoldersAsync.apiKey | [82:42](../YouTubeChannel.Folders.cs#L82) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.AddSavedContentFoldersAsync.config | [82:57](../YouTubeChannel.Folders.cs#L82) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.AddSavedContentFoldersAsync.ct | [82:85](../YouTubeChannel.Folders.cs#L82) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.AddSavedContentFoldersAsync.term | [84:13](../YouTubeChannel.Folders.cs#L84) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.AddSavedContentFoldersAsync.term.details | [91:25](../YouTubeChannel.Folders.cs#L91) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.AddSavedContentFoldersAsync.term.details | [101:25](../YouTubeChannel.Folders.cs#L101) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.AddSavedContentFoldersAsync.term.details | [114:25](../YouTubeChannel.Folders.cs#L114) |
| method | Emby.YouTubePlugin.YouTubeChannel.SplitConfiguredItems | [141:9](../YouTubeChannel.Folders.cs#L141) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.SplitConfiguredItems.value | [141:65](../YouTubeChannel.Folders.cs#L141) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.SplitConfiguredItems.anonymous@5977 | [144:25](../YouTubeChannel.Folders.cs#L144) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.SplitConfiguredItems.anonymous@5977.s | [144:25](../YouTubeChannel.Folders.cs#L144) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.SplitConfiguredItems.anonymous@6015 | [145:24](../YouTubeChannel.Folders.cs#L145) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.SplitConfiguredItems.anonymous@6015.s | [145:24](../YouTubeChannel.Folders.cs#L145) |
| method | Emby.YouTubePlugin.YouTubeChannel.HasConfiguredSavedChannels | [147:9](../YouTubeChannel.Folders.cs#L147) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.HasConfiguredSavedChannels.value | [147:57](../YouTubeChannel.Folders.cs#L147) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.HasConfiguredSavedChannels.anonymous@6171 | [149:22](../YouTubeChannel.Folders.cs#L149) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.HasConfiguredSavedChannels.anonymous@6171.term | [149:22](../YouTubeChannel.Folders.cs#L149) |
| method | Emby.YouTubePlugin.YouTubeChannel.IsSavedChannelRootExternalId | [151:9](../YouTubeChannel.Folders.cs#L151) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.IsSavedChannelRootExternalId.externalId | [151:59](../YouTubeChannel.Folders.cs#L151) |
| method | Emby.YouTubePlugin.YouTubeChannel.Folder | [155:9](../YouTubeChannel.Folders.cs#L155) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.Folder.name | [155:47](../YouTubeChannel.Folders.cs#L155) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.Folder.id | [155:60](../YouTubeChannel.Folders.cs#L155) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.Folder.imageUrl | [155:71](../YouTubeChannel.Folders.cs#L155) |
| method | Emby.YouTubePlugin.YouTubeChannel.BuildChannelSubfoldersAsync | [166:9](../YouTubeChannel.Folders.cs#L166) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.BuildChannelSubfoldersAsync.apiKey | [167:13](../YouTubeChannel.Folders.cs#L167) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.BuildChannelSubfoldersAsync.config | [167:28](../YouTubeChannel.Folders.cs#L167) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.BuildChannelSubfoldersAsync.term | [167:56](../YouTubeChannel.Folders.cs#L167) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.BuildChannelSubfoldersAsync.ct | [167:69](../YouTubeChannel.Folders.cs#L167) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.BuildChannelSubfoldersAsync.items | [169:17](../YouTubeChannel.Folders.cs#L169) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.BuildChannelSubfoldersAsync.channelThumb | [170:17](../YouTubeChannel.Folders.cs#L170) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.BuildChannelSubfoldersAsync.resolvedChannelId | [171:17](../YouTubeChannel.Folders.cs#L171) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.BuildChannelSubfoldersAsync.details | [175:21](../YouTubeChannel.Folders.cs#L175) |
| catch-variable | Emby.YouTubePlugin.YouTubeChannel.BuildChannelSubfoldersAsync.ex | [186:19](../YouTubeChannel.Folders.cs#L186) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.BuildChannelSubfoldersAsync.includeShorts | [191:17](../YouTubeChannel.Folders.cs#L191) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.BuildChannelSubfoldersAsync.includeLive | [193:17](../YouTubeChannel.Folders.cs#L193) |
| method | Emby.YouTubePlugin.YouTubeChannel.LoadCategoryRootAsync | [220:9](../YouTubeChannel.Folders.cs#L220) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadCategoryRootAsync.apiKey | [221:13](../YouTubeChannel.Folders.cs#L221) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadCategoryRootAsync.config | [221:28](../YouTubeChannel.Folders.cs#L221) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadCategoryRootAsync.ct | [221:56](../YouTubeChannel.Folders.cs#L221) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadCategoryRootAsync.configuredRegion | [223:17](../YouTubeChannel.Folders.cs#L223) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadCategoryRootAsync.region | [226:17](../YouTubeChannel.Folders.cs#L226) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadCategoryRootAsync.candidates | [232:17](../YouTubeChannel.Folders.cs#L232) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadCategoryRootAsync.nonEmpty | [234:17](../YouTubeChannel.Folders.cs#L234) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadCategoryRootAsync.items | [237:17](../YouTubeChannel.Folders.cs#L237) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.LoadCategoryRootAsync.items.anonymous@10032 | [238:24](../YouTubeChannel.Folders.cs#L238) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadCategoryRootAsync.items.anonymous@10032.c | [238:24](../YouTubeChannel.Folders.cs#L238) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.LoadCategoryRootAsync.items.anonymous@10089 | [239:25](../YouTubeChannel.Folders.cs#L239) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadCategoryRootAsync.items.anonymous@10089.c | [239:25](../YouTubeChannel.Folders.cs#L239) |
| method | Emby.YouTubePlugin.YouTubeChannel.GetAssignableCategoriesAsync | [247:9](../YouTubeChannel.Folders.cs#L247) |
| tuple-element | Emby.YouTubePlugin.YouTubeChannel.GetAssignableCategoriesAsync.id | [247:41](../YouTubeChannel.Folders.cs#L247) |
| tuple-element | Emby.YouTubePlugin.YouTubeChannel.GetAssignableCategoriesAsync.name | [247:52](../YouTubeChannel.Folders.cs#L247) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetAssignableCategoriesAsync.apiKey | [248:13](../YouTubeChannel.Folders.cs#L248) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetAssignableCategoriesAsync.region | [248:28](../YouTubeChannel.Folders.cs#L248) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetAssignableCategoriesAsync.ct | [248:43](../YouTubeChannel.Folders.cs#L248) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetAssignableCategoriesAsync.catDoc | [250:23](../YouTubeChannel.Folders.cs#L250) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetAssignableCategoriesAsync.candidates | [252:17](../YouTubeChannel.Folders.cs#L252) |
| tuple-element | Emby.YouTubePlugin.YouTubeChannel.GetAssignableCategoriesAsync.candidates.id | [252:40](../YouTubeChannel.Folders.cs#L252) |
| tuple-element | Emby.YouTubePlugin.YouTubeChannel.GetAssignableCategoriesAsync.candidates.name | [252:51](../YouTubeChannel.Folders.cs#L252) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.GetAssignableCategoriesAsync.catItems | [254:72](../YouTubeChannel.Folders.cs#L254) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.GetAssignableCategoriesAsync.category | [258:13](../YouTubeChannel.Folders.cs#L258) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetAssignableCategoriesAsync.category.id | [260:21](../YouTubeChannel.Folders.cs#L260) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetAssignableCategoriesAsync.category.name | [261:21](../YouTubeChannel.Folders.cs#L261) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.GetAssignableCategoriesAsync.category.snippet | [263:64](../YouTubeChannel.Folders.cs#L263) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.GetAssignableCategoriesAsync.category.assignable | [264:69](../YouTubeChannel.Folders.cs#L264) |
| method | Emby.YouTubePlugin.YouTubeChannel.NormalizeCategoryName | [272:9](../YouTubeChannel.Folders.cs#L272) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.NormalizeCategoryName.name | [272:53](../YouTubeChannel.Folders.cs#L272) |
| method | Emby.YouTubePlugin.YouTubeChannel.ProbeNonEmptyCategoriesAsync | [279:9](../YouTubeChannel.Folders.cs#L279) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ProbeNonEmptyCategoriesAsync.apiKey | [280:13](../YouTubeChannel.Folders.cs#L280) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ProbeNonEmptyCategoriesAsync.region | [281:13](../YouTubeChannel.Folders.cs#L281) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ProbeNonEmptyCategoriesAsync.candidates | [282:13](../YouTubeChannel.Folders.cs#L282) |
| tuple-element | Emby.YouTubePlugin.YouTubeChannel.ProbeNonEmptyCategoriesAsync.candidates.id | [282:19](../YouTubeChannel.Folders.cs#L282) |
| tuple-element | Emby.YouTubePlugin.YouTubeChannel.ProbeNonEmptyCategoriesAsync.candidates.name | [282:30](../YouTubeChannel.Folders.cs#L282) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ProbeNonEmptyCategoriesAsync.showShorts | [283:13](../YouTubeChannel.Folders.cs#L283) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ProbeNonEmptyCategoriesAsync.ct | [284:13](../YouTubeChannel.Folders.cs#L284) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ProbeNonEmptyCategoriesAsync.nonEmpty | [286:17](../YouTubeChannel.Folders.cs#L286) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ProbeNonEmptyCategoriesAsync.probeSem | [287:23](../YouTubeChannel.Folders.cs#L287) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ProbeNonEmptyCategoriesAsync.probes | [288:17](../YouTubeChannel.Folders.cs#L288) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.ProbeNonEmptyCategoriesAsync.probes.anonymous@12304 | [288:44](../YouTubeChannel.Folders.cs#L288) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ProbeNonEmptyCategoriesAsync.probes.anonymous@12304.category | [288:50](../YouTubeChannel.Folders.cs#L288) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ProbeNonEmptyCategoriesAsync.probes.anonymous@12304.doc | [293:31](../YouTubeChannel.Folders.cs#L293) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ProbeNonEmptyCategoriesAsync.probes.anonymous@12304.videos | [297:25](../YouTubeChannel.Folders.cs#L297) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.ProbeNonEmptyCategoriesAsync.probes.anonymous@12304.anonymous@13612 | [315:42](../YouTubeChannel.Folders.cs#L315) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ProbeNonEmptyCategoriesAsync.probes.anonymous@12304.anonymous@13612.v | [315:42](../YouTubeChannel.Folders.cs#L315) |
| catch-variable | Emby.YouTubePlugin.YouTubeChannel.ProbeNonEmptyCategoriesAsync.probes.anonymous@12304.ex | [325:23](../YouTubeChannel.Folders.cs#L325) |
| catch-variable | Emby.YouTubePlugin.YouTubeChannel.ProbeNonEmptyCategoriesAsync.ex | [337:19](../YouTubeChannel.Folders.cs#L337) |
| method | Emby.YouTubePlugin.YouTubeChannel.AnyTrendingVideoSurvives | [342:9](../YouTubeChannel.Folders.cs#L342) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.AnyTrendingVideoSurvives.doc | [342:54](../YouTubeChannel.Folders.cs#L342) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.AnyTrendingVideoSurvives.showShorts | [342:72](../YouTubeChannel.Folders.cs#L342) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.AnyTrendingVideoSurvives.video | [344:13](../YouTubeChannel.Folders.cs#L344) |

</details>

<a id="file-fd75d22d5169819c"></a>
## [YouTubeChannel.Live.cs](../YouTubeChannel.Live.cs)

Plugin Compile source (SDK default); 6186 bytes; 120 lines.

<details>
<summary>All 30 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [8:1](../YouTubeChannel.Live.cs#L8) |
| type | Emby.YouTubePlugin.YouTubeChannel | [10:5](../YouTubeChannel.Live.cs#L10) |
| field | Emby.YouTubePlugin.YouTubeChannel.LiveBadgeForwardRegex | [19:39](../YouTubeChannel.Live.cs#L19) |
| field | Emby.YouTubePlugin.YouTubeChannel.LiveBadgeReverseRegex | [23:39](../YouTubeChannel.Live.cs#L23) |
| field | Emby.YouTubePlugin.YouTubeChannel.LiveStreamsPageCache | [27:84](../YouTubeChannel.Live.cs#L27) |
| field | Emby.YouTubePlugin.YouTubeChannel.LiveStreamsPageLocks | [29:77](../YouTubeChannel.Live.cs#L29) |
| field | Emby.YouTubePlugin.YouTubeChannel.LiveStreamsPageCacheTtl | [31:42](../YouTubeChannel.Live.cs#L31) |
| field | Emby.YouTubePlugin.YouTubeChannel.LiveStreamsPageEmptyCacheTtl | [32:42](../YouTubeChannel.Live.cs#L32) |
| field | Emby.YouTubePlugin.YouTubeChannel.LiveStreamsPageStaleFallbackTtl | [33:42](../YouTubeChannel.Live.cs#L33) |
| method | Emby.YouTubePlugin.YouTubeChannel.GetChannelLiveAndUpcomingIdsAsync | [37:9](../YouTubeChannel.Live.cs#L37) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetChannelLiveAndUpcomingIdsAsync.channelId | [38:13](../YouTubeChannel.Live.cs#L38) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetChannelLiveAndUpcomingIdsAsync.cancellationToken | [38:31](../YouTubeChannel.Live.cs#L38) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelLiveAndUpcomingIdsAsync.cached | [43:69](../YouTubeChannel.Live.cs#L43) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelLiveAndUpcomingIdsAsync.probeLock | [50:17](../YouTubeChannel.Live.cs#L50) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.GetChannelLiveAndUpcomingIdsAsync.probeLock.anonymous@2861 | [50:70](../YouTubeChannel.Live.cs#L50) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetChannelLiveAndUpcomingIdsAsync.probeLock.anonymous@2861._ | [50:70](../YouTubeChannel.Live.cs#L50) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelLiveAndUpcomingIdsAsync.ids | [61:21](../YouTubeChannel.Live.cs#L61) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelLiveAndUpcomingIdsAsync.url | [64:25](../YouTubeChannel.Live.cs#L64) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelLiveAndUpcomingIdsAsync.html | [65:25](../YouTubeChannel.Live.cs#L65) |
| catch-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelLiveAndUpcomingIdsAsync.ex | [80:23](../YouTubeChannel.Live.cs#L80) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelLiveAndUpcomingIdsAsync.stale | [83:77](../YouTubeChannel.Live.cs#L83) |
| method | Emby.YouTubePlugin.YouTubeChannel.ExtractIds | [99:9](../YouTubeChannel.Live.cs#L99) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ExtractIds.html | [99:40](../YouTubeChannel.Live.cs#L99) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ExtractIds.regex | [99:53](../YouTubeChannel.Live.cs#L99) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ExtractIds.idGroup | [99:66](../YouTubeChannel.Live.cs#L99) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ExtractIds.ids | [99:79](../YouTubeChannel.Live.cs#L99) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractIds.match | [102:13](../YouTubeChannel.Live.cs#L102) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractIds.match.value | [104:21](../YouTubeChannel.Live.cs#L104) |
| method | Emby.YouTubePlugin.YouTubeChannel.IsUsableStreamsPage | [109:9](../YouTubeChannel.Live.cs#L109) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.IsUsableStreamsPage.html | [109:49](../YouTubeChannel.Live.cs#L109) |

</details>

<a id="file-b5a9b427d9c6cb61"></a>
## [YouTubeChannel.Mapping.cs](../YouTubeChannel.Mapping.cs)

Plugin Compile source (SDK default); 14971 bytes; 310 lines.

<details>
<summary>All 95 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [18:1](../YouTubeChannel.Mapping.cs#L18) |
| type | Emby.YouTubePlugin.YouTubeChannel | [20:5](../YouTubeChannel.Mapping.cs#L20) |
| method | Emby.YouTubePlugin.YouTubeChannel.GetYouTubeWatchUrl | [22:9](../YouTubeChannel.Mapping.cs#L22) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetYouTubeWatchUrl.videoId | [22:51](../YouTubeChannel.Mapping.cs#L22) |
| method | Emby.YouTubePlugin.YouTubeChannel.BuildEngagementStatsLine | [26:9](../YouTubeChannel.Mapping.cs#L26) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.BuildEngagementStatsLine.viewCount | [27:13](../YouTubeChannel.Mapping.cs#L27) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.BuildEngagementStatsLine.likeCount | [28:13](../YouTubeChannel.Mapping.cs#L28) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.BuildEngagementStatsLine.commentCount | [29:13](../YouTubeChannel.Mapping.cs#L29) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.BuildEngagementStatsLine.includeLikes | [30:13](../YouTubeChannel.Mapping.cs#L30) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.BuildEngagementStatsLine.includeComments | [31:13](../YouTubeChannel.Mapping.cs#L31) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.BuildEngagementStatsLine.parts | [33:17](../YouTubeChannel.Mapping.cs#L33) |
| method | Emby.YouTubePlugin.YouTubeChannel.BuildVideoOverview | [44:9](../YouTubeChannel.Mapping.cs#L44) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.BuildVideoOverview.videoId | [44:51](../YouTubeChannel.Mapping.cs#L44) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.BuildVideoOverview.description | [44:68](../YouTubeChannel.Mapping.cs#L44) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.BuildVideoOverview.viewCount | [45:13](../YouTubeChannel.Mapping.cs#L45) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.BuildVideoOverview.likeCount | [45:30](../YouTubeChannel.Mapping.cs#L45) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.BuildVideoOverview.commentCount | [45:47](../YouTubeChannel.Mapping.cs#L45) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.BuildVideoOverview.direct | [50:82](../YouTubeChannel.Mapping.cs#L50) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.BuildVideoOverview.stats | [57:20](../YouTubeChannel.Mapping.cs#L57) |
| method | Emby.YouTubePlugin.YouTubeChannel.MakeMediaSources | [65:9](../YouTubeChannel.Mapping.cs#L65) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.MakeMediaSources.videoId | [65:63](../YouTubeChannel.Mapping.cs#L65) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.MakeMediaSources.isLive | [65:79](../YouTubeChannel.Mapping.cs#L65) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.MakeMediaSources.url | [70:20](../YouTubeChannel.Mapping.cs#L70) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.MakeMediaSources.mediaSource | [71:17](../YouTubeChannel.Mapping.cs#L71) |
| method | Emby.YouTubePlugin.YouTubeChannel.DisableMediaSourceProbing | [92:9](../YouTubeChannel.Mapping.cs#L92) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.DisableMediaSourceProbing.mediaSource | [92:55](../YouTubeChannel.Mapping.cs#L92) |
| method | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos | [99:9](../YouTubeChannel.Mapping.cs#L99) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.doc | [100:13](../YouTubeChannel.Mapping.cs#L100) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.effectiveRegion | [101:13](../YouTubeChannel.Mapping.cs#L101) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.list | [103:17](../YouTubeChannel.Mapping.cs#L103) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.items | [104:66](../YouTubeChannel.Mapping.cs#L104) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.serverRegion | [111:17](../YouTubeChannel.Mapping.cs#L111) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el | [113:13](../YouTubeChannel.Mapping.cs#L113) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.videoId | [115:21](../YouTubeChannel.Mapping.cs#L115) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.cdRR | [120:68](../YouTubeChannel.Mapping.cs#L120) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.rrEl | [122:73](../YouTubeChannel.Mapping.cs#L122) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.blocked | [125:26](../YouTubeChannel.Mapping.cs#L125) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.bl | [126:64](../YouTubeChannel.Mapping.cs#L126) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.b | [129:25](../YouTubeChannel.Mapping.cs#L129) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.al | [134:67](../YouTubeChannel.Mapping.cs#L134) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.inAllowed | [137:30](../YouTubeChannel.Mapping.cs#L137) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.a | [138:25](../YouTubeChannel.Mapping.cs#L138) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.title | [146:21](../YouTubeChannel.Mapping.cs#L146) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.author | [147:21](../YouTubeChannel.Mapping.cs#L147) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.desc | [148:21](../YouTubeChannel.Mapping.cs#L148) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.pubStr | [149:21](../YouTubeChannel.Mapping.cs#L149) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.premiere | [150:21](../YouTubeChannel.Mapping.cs#L150) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.thumb | [151:21](../YouTubeChannel.Mapping.cs#L151) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.origLang | [156:21](../YouTubeChannel.Mapping.cs#L156) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.duration | [160:21](../YouTubeChannel.Mapping.cs#L160) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.ts | [161:21](../YouTubeChannel.Mapping.cs#L161) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.viewCount | [164:23](../YouTubeChannel.Mapping.cs#L164) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.likeCount | [165:23](../YouTubeChannel.Mapping.cs#L165) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.commentCount | [166:23](../YouTubeChannel.Mapping.cs#L166) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.stats | [167:61](../YouTubeChannel.Mapping.cs#L167) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.v | [169:89](../YouTubeChannel.Mapping.cs#L169) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.l | [170:89](../YouTubeChannel.Mapping.cs#L170) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.c | [171:92](../YouTubeChannel.Mapping.cs#L171) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.apiOverview | [174:25](../YouTubeChannel.Mapping.cs#L174) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.isLive | [177:22](../YouTubeChannel.Mapping.cs#L177) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.lsd | [178:71](../YouTubeChannel.Mapping.cs#L178) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.concurrentViewers | [180:25](../YouTubeChannel.Mapping.cs#L180) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.overview | [184:25](../YouTubeChannel.Mapping.cs#L184) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.runtimeTicks | [185:23](../YouTubeChannel.Mapping.cs#L185) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.direct | [186:78](../YouTubeChannel.Mapping.cs#L186) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.isReel | [190:22](../YouTubeChannel.Mapping.cs#L190) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.itemId | [192:24](../YouTubeChannel.Mapping.cs#L192) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.displayTitle | [195:24](../YouTubeChannel.Mapping.cs#L195) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractTrendingVideos.el.info | [199:21](../YouTubeChannel.Mapping.cs#L199) |
| method | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos | [228:9](../YouTubeChannel.Mapping.cs#L228) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.doc | [228:60](../YouTubeChannel.Mapping.cs#L228) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.isPlaylist | [228:78](../YouTubeChannel.Mapping.cs#L228) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.list | [230:17](../YouTubeChannel.Mapping.cs#L230) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.items | [231:66](../YouTubeChannel.Mapping.cs#L231) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.el | [235:13](../YouTubeChannel.Mapping.cs#L235) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.el.videoId | [237:25](../YouTubeChannel.Mapping.cs#L237) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.el.snip | [245:65](../YouTubeChannel.Mapping.cs#L245) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.el.rid | [246:70](../YouTubeChannel.Mapping.cs#L246) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.el.idProp | [255:86](../YouTubeChannel.Mapping.cs#L255) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.el.title | [265:21](../YouTubeChannel.Mapping.cs#L265) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.el.author | [266:21](../YouTubeChannel.Mapping.cs#L266) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.el.desc | [267:21](../YouTubeChannel.Mapping.cs#L267) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.el.pubStr | [268:21](../YouTubeChannel.Mapping.cs#L268) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.el.premiere | [269:21](../YouTubeChannel.Mapping.cs#L269) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.el.thumb | [270:21](../YouTubeChannel.Mapping.cs#L270) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.el.liveBroadcastContent | [275:21](../YouTubeChannel.Mapping.cs#L275) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.el.isLive | [276:22](../YouTubeChannel.Mapping.cs#L276) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.el.overview | [278:25](../YouTubeChannel.Mapping.cs#L278) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.el.itemId | [280:24](../YouTubeChannel.Mapping.cs#L280) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.el.displayTitle | [281:24](../YouTubeChannel.Mapping.cs#L281) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ExtractVideos.el.info | [283:21](../YouTubeChannel.Mapping.cs#L283) |
| method | Emby.YouTubePlugin.YouTubeChannel.ClampVideos | [306:9](../YouTubeChannel.Mapping.cs#L306) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ClampVideos.val | [306:40](../YouTubeChannel.Mapping.cs#L306) |
| method | Emby.YouTubePlugin.YouTubeChannel.ClampSearchVideos | [308:9](../YouTubeChannel.Mapping.cs#L308) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ClampSearchVideos.val | [308:46](../YouTubeChannel.Mapping.cs#L308) |

</details>

<a id="file-11faad7ba4aab64e"></a>
## [YouTubeChannel.MediaFolders.cs](../YouTubeChannel.MediaFolders.cs)

Plugin Compile source (SDK default); 19016 bytes; 467 lines.

<details>
<summary>All 130 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [17:1](../YouTubeChannel.MediaFolders.cs#L17) |
| type | Emby.YouTubePlugin.YouTubeChannel | [19:5](../YouTubeChannel.MediaFolders.cs#L19) |
| field | Emby.YouTubePlugin.YouTubeChannel.MaxMediaPageRequests | [21:27](../YouTubeChannel.MediaFolders.cs#L21) |
| method | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync | [23:9](../YouTubeChannel.MediaFolders.cs#L23) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.apiKey | [24:13](../YouTubeChannel.MediaFolders.cs#L24) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.config | [25:13](../YouTubeChannel.MediaFolders.cs#L25) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.type | [26:13](../YouTubeChannel.MediaFolders.cs#L26) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.term | [27:13](../YouTubeChannel.MediaFolders.cs#L27) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.ct | [28:13](../YouTubeChannel.MediaFolders.cs#L28) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.items | [33:17](../YouTubeChannel.MediaFolders.cs#L33) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.limit | [34:17](../YouTubeChannel.MediaFolders.cs#L34) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.seenIds | [37:17](../YouTubeChannel.MediaFolders.cs#L37) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.seenPageTokens | [38:17](../YouTubeChannel.MediaFolders.cs#L38) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.pageToken | [39:21](../YouTubeChannel.MediaFolders.cs#L39) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.hasMore | [40:17](../YouTubeChannel.MediaFolders.cs#L40) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.reachedEnd | [41:17](../YouTubeChannel.MediaFolders.cs#L41) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.observedShorts | [42:17](../YouTubeChannel.MediaFolders.cs#L42) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.observedLive | [43:17](../YouTubeChannel.MediaFolders.cs#L43) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.pageRequests | [44:17](../YouTubeChannel.MediaFolders.cs#L44) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.usesSearchEndpoint | [45:17](../YouTubeChannel.MediaFolders.cs#L45) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.maxPageRequests | [48:17](../YouTubeChannel.MediaFolders.cs#L48) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.knownShortsIds | [52:30](../YouTubeChannel.MediaFolders.cs#L52) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.shortsProbe | [55:21](../YouTubeChannel.MediaFolders.cs#L55) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.requestedPageToken | [63:21](../YouTubeChannel.MediaFolders.cs#L63) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.doc | [71:27](../YouTubeChannel.MediaFolders.cs#L71) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.batch | [75:21](../YouTubeChannel.MediaFolders.cs#L75) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.videoIds | [88:21](../YouTubeChannel.MediaFolders.cs#L88) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.anonymous@4267 | [106:45](../YouTubeChannel.MediaFolders.cs#L106) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.anonymous@4267.item | [106:45](../YouTubeChannel.MediaFolders.cs#L106) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.anonymous@4376 | [107:43](../YouTubeChannel.MediaFolders.cs#L107) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.anonymous@4376.item | [107:43](../YouTubeChannel.MediaFolders.cs#L107) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadMediaFolderAsync.remaining | [110:21](../YouTubeChannel.MediaFolders.cs#L110) |
| method | Emby.YouTubePlugin.YouTubeChannel.GetMediaPageAsync | [134:9](../YouTubeChannel.MediaFolders.cs#L134) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetMediaPageAsync.apiKey | [135:13](../YouTubeChannel.MediaFolders.cs#L135) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetMediaPageAsync.config | [136:13](../YouTubeChannel.MediaFolders.cs#L136) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetMediaPageAsync.type | [137:13](../YouTubeChannel.MediaFolders.cs#L137) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetMediaPageAsync.term | [138:13](../YouTubeChannel.MediaFolders.cs#L138) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetMediaPageAsync.pageToken | [139:13](../YouTubeChannel.MediaFolders.cs#L139) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetMediaPageAsync.ct | [140:13](../YouTubeChannel.MediaFolders.cs#L140) |
| method | Emby.YouTubePlugin.YouTubeChannel.IsPlaylistDocument | [153:9](../YouTubeChannel.MediaFolders.cs#L153) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.IsPlaylistDocument.type | [153:48](../YouTubeChannel.MediaFolders.cs#L153) |
| method | Emby.YouTubePlugin.YouTubeChannel.GetNextPageToken | [156:9](../YouTubeChannel.MediaFolders.cs#L156) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetNextPageToken.doc | [156:49](../YouTubeChannel.MediaFolders.cs#L156) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.GetNextPageToken.token | [158:76](../YouTubeChannel.MediaFolders.cs#L158) |
| method | Emby.YouTubePlugin.YouTubeChannel.KeepUniquePageItems | [163:9](../YouTubeChannel.MediaFolders.cs#L163) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.KeepUniquePageItems.pageItems | [164:13](../YouTubeChannel.MediaFolders.cs#L164) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.KeepUniquePageItems.seenIds | [165:13](../YouTubeChannel.MediaFolders.cs#L165) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.KeepUniquePageItems.videoIds | [167:17](../YouTubeChannel.MediaFolders.cs#L167) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.KeepUniquePageItems.selected | [168:17](../YouTubeChannel.MediaFolders.cs#L168) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.KeepUniquePageItems.item | [170:13](../YouTubeChannel.MediaFolders.cs#L170) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.KeepUniquePageItems.item.rawId | [172:21](../YouTubeChannel.MediaFolders.cs#L172) |
| method | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync | [183:9](../YouTubeChannel.MediaFolders.cs#L183) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.apiKey | [184:13](../YouTubeChannel.MediaFolders.cs#L184) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.config | [185:13](../YouTubeChannel.MediaFolders.cs#L185) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.channelId | [186:13](../YouTubeChannel.MediaFolders.cs#L186) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.ct | [187:13](../YouTubeChannel.MediaFolders.cs#L187) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.liveProbe | [189:17](../YouTubeChannel.MediaFolders.cs#L189) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.liveIds | [190:17](../YouTubeChannel.MediaFolders.cs#L190) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.limit | [202:17](../YouTubeChannel.MediaFolders.cs#L202) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.result | [203:17](../YouTubeChannel.MediaFolders.cs#L203) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.seenIds | [204:17](../YouTubeChannel.MediaFolders.cs#L204) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.orderedIds | [205:17](../YouTubeChannel.MediaFolders.cs#L205) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.orderedIds.anonymous@8423 | [205:46](../YouTubeChannel.MediaFolders.cs#L205) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.orderedIds.anonymous@8423.id | [205:46](../YouTubeChannel.MediaFolders.cs#L205) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.allDetailLookupsSucceeded | [206:17](../YouTubeChannel.MediaFolders.cs#L206) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.effectiveRegion | [207:17](../YouTubeChannel.MediaFolders.cs#L207) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.index | [213:22](../YouTubeChannel.MediaFolders.cs#L213) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.chunk | [216:21](../YouTubeChannel.MediaFolders.cs#L216) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.doc | [217:27](../YouTubeChannel.MediaFolders.cs#L217) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.playableIds | [225:21](../YouTubeChannel.MediaFolders.cs#L225) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.item | [226:17](../YouTubeChannel.MediaFolders.cs#L226) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.item.rawId | [228:25](../YouTubeChannel.MediaFolders.cs#L228) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.item.previousId | [232:25](../YouTubeChannel.MediaFolders.cs#L232) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.LoadLiveFolderAsync.item.meta | [238:67](../YouTubeChannel.MediaFolders.cs#L238) |
| method | Emby.YouTubePlugin.YouTubeChannel.GetPlayableLiveVideoIds | [268:9](../YouTubeChannel.MediaFolders.cs#L268) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetPlayableLiveVideoIds.doc | [269:13](../YouTubeChannel.MediaFolders.cs#L269) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetPlayableLiveVideoIds.candidateIds | [270:13](../YouTubeChannel.MediaFolders.cs#L270) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetPlayableLiveVideoIds.playable | [272:17](../YouTubeChannel.MediaFolders.cs#L272) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.GetPlayableLiveVideoIds.items | [273:66](../YouTubeChannel.MediaFolders.cs#L273) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.GetPlayableLiveVideoIds.detail | [277:13](../YouTubeChannel.MediaFolders.cs#L277) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetPlayableLiveVideoIds.detail.id | [279:21](../YouTubeChannel.MediaFolders.cs#L279) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.GetPlayableLiveVideoIds.detail.status | [285:61](../YouTubeChannel.MediaFolders.cs#L285) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetPlayableLiveVideoIds.detail.privacy | [288:25](../YouTubeChannel.MediaFolders.cs#L288) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.GetPlayableLiveVideoIds.detail.embeddable | [291:69](../YouTubeChannel.MediaFolders.cs#L291) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetPlayableLiveVideoIds.detail.upload | [295:25](../YouTubeChannel.MediaFolders.cs#L295) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.GetPlayableLiveVideoIds.detail.contentDetails | [302:69](../YouTubeChannel.MediaFolders.cs#L302) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.GetPlayableLiveVideoIds.detail.contentRating | [304:79](../YouTubeChannel.MediaFolders.cs#L304) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.GetPlayableLiveVideoIds.detail.ytRating | [306:73](../YouTubeChannel.MediaFolders.cs#L306) |
| method | Emby.YouTubePlugin.YouTubeChannel.CacheInitialThumbnails | [319:9](../YouTubeChannel.MediaFolders.cs#L319) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.CacheInitialThumbnails.batch | [319:52](../YouTubeChannel.MediaFolders.cs#L319) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.CacheInitialThumbnails.item | [321:13](../YouTubeChannel.MediaFolders.cs#L321) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.CacheInitialThumbnails.item.existing | [326:60](../YouTubeChannel.MediaFolders.cs#L326) |
| method | Emby.YouTubePlugin.YouTubeChannel.ApplyFolderFilters | [338:9](../YouTubeChannel.MediaFolders.cs#L338) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ApplyFolderFilters.batch | [339:13](../YouTubeChannel.MediaFolders.cs#L339) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ApplyFolderFilters.type | [339:42](../YouTubeChannel.MediaFolders.cs#L339) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ApplyFolderFilters.config | [339:55](../YouTubeChannel.MediaFolders.cs#L339) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.ApplyFolderFilters.anonymous@14142 | [343:33](../YouTubeChannel.MediaFolders.cs#L343) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ApplyFolderFilters.anonymous@14142.item | [343:33](../YouTubeChannel.MediaFolders.cs#L343) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.ApplyFolderFilters.anonymous@14412 | [348:33](../YouTubeChannel.MediaFolders.cs#L348) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ApplyFolderFilters.anonymous@14412.item | [348:33](../YouTubeChannel.MediaFolders.cs#L348) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.ApplyFolderFilters.anonymous@14565 | [351:33](../YouTubeChannel.MediaFolders.cs#L351) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ApplyFolderFilters.anonymous@14565.item | [351:33](../YouTubeChannel.MediaFolders.cs#L351) |
| method | Emby.YouTubePlugin.YouTubeChannel.UpdateChannelContentFlags | [354:9](../YouTubeChannel.MediaFolders.cs#L354) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.UpdateChannelContentFlags.type | [355:13](../YouTubeChannel.MediaFolders.cs#L355) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.UpdateChannelContentFlags.term | [356:13](../YouTubeChannel.MediaFolders.cs#L356) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.UpdateChannelContentFlags.observedShorts | [357:13](../YouTubeChannel.MediaFolders.cs#L357) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.UpdateChannelContentFlags.observedLive | [358:13](../YouTubeChannel.MediaFolders.cs#L358) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.UpdateChannelContentFlags.reachedEnd | [359:13](../YouTubeChannel.MediaFolders.cs#L359) |
| method | Emby.YouTubePlugin.YouTubeChannel.IsChannelId | [373:9](../YouTubeChannel.MediaFolders.cs#L373) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.IsChannelId.channelId | [373:41](../YouTubeChannel.MediaFolders.cs#L373) |
| method | Emby.YouTubePlugin.YouTubeChannel.ChannelHasShortsAsync | [376:9](../YouTubeChannel.MediaFolders.cs#L376) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ChannelHasShortsAsync.apiKey | [377:13](../YouTubeChannel.MediaFolders.cs#L377) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ChannelHasShortsAsync.channelId | [377:28](../YouTubeChannel.MediaFolders.cs#L377) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ChannelHasShortsAsync.ct | [377:46](../YouTubeChannel.MediaFolders.cs#L377) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ChannelHasShortsAsync.cached | [382:17](../YouTubeChannel.MediaFolders.cs#L382) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ChannelHasShortsAsync.shortsProbe | [390:21](../YouTubeChannel.MediaFolders.cs#L390) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ChannelHasShortsAsync.hasShorts | [394:21](../YouTubeChannel.MediaFolders.cs#L394) |
| catch-variable | Emby.YouTubePlugin.YouTubeChannel.ChannelHasShortsAsync.ex | [402:19](../YouTubeChannel.MediaFolders.cs#L402) |
| method | Emby.YouTubePlugin.YouTubeChannel.ChannelHasLiveAsync | [409:9](../YouTubeChannel.MediaFolders.cs#L409) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ChannelHasLiveAsync.apiKey | [410:13](../YouTubeChannel.MediaFolders.cs#L410) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ChannelHasLiveAsync.channelId | [410:28](../YouTubeChannel.MediaFolders.cs#L410) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ChannelHasLiveAsync.ct | [410:46](../YouTubeChannel.MediaFolders.cs#L410) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ChannelHasLiveAsync.cached | [415:17](../YouTubeChannel.MediaFolders.cs#L415) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ChannelHasLiveAsync.liveProbe | [422:21](../YouTubeChannel.MediaFolders.cs#L422) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ChannelHasLiveAsync.liveIds | [424:21](../YouTubeChannel.MediaFolders.cs#L424) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ChannelHasLiveAsync.hasLive | [428:21](../YouTubeChannel.MediaFolders.cs#L428) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ChannelHasLiveAsync.allDetailLookupsSucceeded | [429:21](../YouTubeChannel.MediaFolders.cs#L429) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.ChannelHasLiveAsync.chunk | [430:17](../YouTubeChannel.MediaFolders.cs#L430) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ChannelHasLiveAsync.chunk.details | [432:31](../YouTubeChannel.MediaFolders.cs#L432) |
| catch-variable | Emby.YouTubePlugin.YouTubeChannel.ChannelHasLiveAsync.ex | [460:19](../YouTubeChannel.MediaFolders.cs#L460) |

</details>

<a id="file-b6bd071cf0a75be9"></a>
## [YouTubeChannel.Shorts.cs](../YouTubeChannel.Shorts.cs)

Plugin Compile source (SDK default); 25656 bytes; 553 lines.

<details>
<summary>All 148 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [13:1](../YouTubeChannel.Shorts.cs#L13) |
| type | Emby.YouTubePlugin.YouTubeChannel | [15:5](../YouTubeChannel.Shorts.cs#L15) |
| type | Emby.YouTubePlugin.YouTubeChannel.ShortsClassification | [17:9](../YouTubeChannel.Shorts.cs#L17) |
| enum-member | Emby.YouTubePlugin.YouTubeChannel.ShortsClassification.Unknown | [19:13](../YouTubeChannel.Shorts.cs#L19) |
| enum-member | Emby.YouTubePlugin.YouTubeChannel.ShortsClassification.Regular | [20:13](../YouTubeChannel.Shorts.cs#L20) |
| enum-member | Emby.YouTubePlugin.YouTubeChannel.ShortsClassification.Short | [21:13](../YouTubeChannel.Shorts.cs#L21) |
| field | Emby.YouTubePlugin.YouTubeChannel.ShortsHttp | [24:44](../YouTubeChannel.Shorts.cs#L24) |
| field | Emby.YouTubePlugin.YouTubeChannel.ShortsProbeTimeoutSeconds | [25:27](../YouTubeChannel.Shorts.cs#L25) |
| field | Emby.YouTubePlugin.YouTubeChannel.ShortsBrowserUserAgent | [26:30](../YouTubeChannel.Shorts.cs#L26) |
| field | Emby.YouTubePlugin.YouTubeChannel.ShortsConsentCookie | [33:30](../YouTubeChannel.Shorts.cs#L33) |
| field | Emby.YouTubePlugin.YouTubeChannel.ShortsVideoIdRegex | [41:39](../YouTubeChannel.Shorts.cs#L41) |
| method | Emby.YouTubePlugin.YouTubeChannel.CreateShortsHttp | [46:9](../YouTubeChannel.Shorts.cs#L46) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.CreateShortsHttp.handler | [48:17](../YouTubeChannel.Shorts.cs#L48) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.CreateShortsHttp.client | [52:17](../YouTubeChannel.Shorts.cs#L52) |
| field | Emby.YouTubePlugin.YouTubeChannel.ShortsProbeHttp | [66:44](../YouTubeChannel.Shorts.cs#L66) |
| method | Emby.YouTubePlugin.YouTubeChannel.CreateShortsProbeHttp | [68:9](../YouTubeChannel.Shorts.cs#L68) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.CreateShortsProbeHttp.handler | [70:17](../YouTubeChannel.Shorts.cs#L70) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.CreateShortsProbeHttp.client | [73:17](../YouTubeChannel.Shorts.cs#L73) |
| tuple-element | Emby.YouTubePlugin.YouTubeChannel.IsShort | [81:63](../YouTubeChannel.Shorts.cs#L81) |
| tuple-element | Emby.YouTubePlugin.YouTubeChannel.CachedAt | [81:77](../YouTubeChannel.Shorts.cs#L81) |
| field | Emby.YouTubePlugin.YouTubeChannel.ShortsUrlProbeCache | [81:97](../YouTubeChannel.Shorts.cs#L81) |
| field | Emby.YouTubePlugin.YouTubeChannel.ShortsUrlProbeTtl | [83:42](../YouTubeChannel.Shorts.cs#L83) |
| field | Emby.YouTubePlugin.YouTubeChannel.ShortsUrlProbeCacheMaxEntries | [84:27](../YouTubeChannel.Shorts.cs#L84) |
| field | Emby.YouTubePlugin.YouTubeChannel.ShortsProbeDocumentPrefixBytes | [85:27](../YouTubeChannel.Shorts.cs#L85) |
| field | Emby.YouTubePlugin.YouTubeChannel.ShortsWatchBootstrapMarker | [86:40](../YouTubeChannel.Shorts.cs#L86) |
| field | Emby.YouTubePlugin.YouTubeChannel.ShortsUrlProbeConcurrency | [88:47](../YouTubeChannel.Shorts.cs#L88) |
| field | Emby.YouTubePlugin.YouTubeChannel.ShortsProbeCircuitLock | [89:40](../YouTubeChannel.Shorts.cs#L89) |
| field | Emby.YouTubePlugin.YouTubeChannel.ShortsProbeFailureWindow | [90:42](../YouTubeChannel.Shorts.cs#L90) |
| field | Emby.YouTubePlugin.YouTubeChannel.ShortsProbeCooldown | [91:42](../YouTubeChannel.Shorts.cs#L91) |
| field | Emby.YouTubePlugin.YouTubeChannel.ShortsProbeFailureThreshold | [92:27](../YouTubeChannel.Shorts.cs#L92) |
| field | Emby.YouTubePlugin.YouTubeChannel._shortsProbeFailureWindowStartedUtc | [93:33](../YouTubeChannel.Shorts.cs#L93) |
| field | Emby.YouTubePlugin.YouTubeChannel._shortsProbeCooldownUntilUtc | [94:33](../YouTubeChannel.Shorts.cs#L94) |
| field | Emby.YouTubePlugin.YouTubeChannel._shortsProbeAmbiguousCount | [95:28](../YouTubeChannel.Shorts.cs#L95) |
| field | Emby.YouTubePlugin.YouTubeChannel._probeCacheLoaded | [96:28](../YouTubeChannel.Shorts.cs#L96) |
| field | Emby.YouTubePlugin.YouTubeChannel._probeCacheLastWriteTicks | [97:29](../YouTubeChannel.Shorts.cs#L97) |
| property | Emby.YouTubePlugin.YouTubeChannel.ShortsProbeCachePath | [99:9](../YouTubeChannel.Shorts.cs#L99) |
| method | Emby.YouTubePlugin.YouTubeChannel.LoadShortsProbeCache | [106:9](../YouTubeChannel.Shorts.cs#L106) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadShortsProbeCache.path | [111:21](../YouTubeChannel.Shorts.cs#L111) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadShortsProbeCache.json | [113:21](../YouTubeChannel.Shorts.cs#L113) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadShortsProbeCache.doc | [114:27](../YouTubeChannel.Shorts.cs#L114) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadShortsProbeCache.cutoff | [117:21](../YouTubeChannel.Shorts.cs#L117) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.LoadShortsProbeCache.prop | [118:17](../YouTubeChannel.Shorts.cs#L118) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.LoadShortsProbeCache.prop.isShortEl | [121:71](../YouTubeChannel.Shorts.cs#L121) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.LoadShortsProbeCache.prop.cachedAtEl | [122:72](../YouTubeChannel.Shorts.cs#L122) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.LoadShortsProbeCache.prop.ts | [123:57](../YouTubeChannel.Shorts.cs#L123) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LoadShortsProbeCache.prop.cachedAt | [124:25](../YouTubeChannel.Shorts.cs#L124) |
| catch-variable | Emby.YouTubePlugin.YouTubeChannel.LoadShortsProbeCache.ex | [129:19](../YouTubeChannel.Shorts.cs#L129) |
| method | Emby.YouTubePlugin.YouTubeChannel.PersistShortsProbeCache | [137:9](../YouTubeChannel.Shorts.cs#L137) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.PersistShortsProbeCache.now | [139:17](../YouTubeChannel.Shorts.cs#L139) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.PersistShortsProbeCache.last | [140:17](../YouTubeChannel.Shorts.cs#L140) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.PersistShortsProbeCache.path | [146:21](../YouTubeChannel.Shorts.cs#L146) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.PersistShortsProbeCache.dir | [147:21](../YouTubeChannel.Shorts.cs#L147) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.PersistShortsProbeCache.ms | [151:27](../YouTubeChannel.Shorts.cs#L151) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.PersistShortsProbeCache.writer | [152:28](../YouTubeChannel.Shorts.cs#L152) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.PersistShortsProbeCache.kvp | [155:21](../YouTubeChannel.Shorts.cs#L155) |
| catch-variable | Emby.YouTubePlugin.YouTubeChannel.PersistShortsProbeCache.ex | [166:19](../YouTubeChannel.Shorts.cs#L166) |
| method | Emby.YouTubePlugin.YouTubeChannel.IsShortByUrlProbeAsync | [177:9](../YouTubeChannel.Shorts.cs#L177) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.IsShortByUrlProbeAsync.videoId | [178:13](../YouTubeChannel.Shorts.cs#L178) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.IsShortByUrlProbeAsync.ct | [179:13](../YouTubeChannel.Shorts.cs#L179) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.IsShortByUrlProbeAsync.entry | [181:66](../YouTubeChannel.Shorts.cs#L181) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.IsShortByUrlProbeAsync.url | [208:21](../YouTubeChannel.Shorts.cs#L208) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.IsShortByUrlProbeAsync.headProbe | [209:21](../YouTubeChannel.Shorts.cs#L209) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.IsShortByUrlProbeAsync.result | [211:21](../YouTubeChannel.Shorts.cs#L211) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.IsShortByUrlProbeAsync.getProbe | [218:25](../YouTubeChannel.Shorts.cs#L218) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.IsShortByUrlProbeAsync.isShort | [230:21](../YouTubeChannel.Shorts.cs#L230) |
| catch-variable | Emby.YouTubePlugin.YouTubeChannel.IsShortByUrlProbeAsync.ex | [240:19](../YouTubeChannel.Shorts.cs#L240) |
| method | Emby.YouTubePlugin.YouTubeChannel.IsShortsProbeCircuitOpen | [252:9](../YouTubeChannel.Shorts.cs#L252) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.IsShortsProbeCircuitOpen.now | [256:21](../YouTubeChannel.Shorts.cs#L256) |
| method | Emby.YouTubePlugin.YouTubeChannel.RegisterAmbiguousShortsProbe | [271:9](../YouTubeChannel.Shorts.cs#L271) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.RegisterAmbiguousShortsProbe.opened | [273:17](../YouTubeChannel.Shorts.cs#L273) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.RegisterAmbiguousShortsProbe.now | [276:21](../YouTubeChannel.Shorts.cs#L276) |
| method | Emby.YouTubePlugin.YouTubeChannel.ClassifyShortsProbeResponse | [297:9](../YouTubeChannel.Shorts.cs#L297) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ClassifyShortsProbeResponse.status | [298:13](../YouTubeChannel.Shorts.cs#L298) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ClassifyShortsProbeResponse.requestUri | [299:13](../YouTubeChannel.Shorts.cs#L299) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ClassifyShortsProbeResponse.location | [300:13](../YouTubeChannel.Shorts.cs#L300) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ClassifyShortsProbeResponse.resolved | [304:21](../YouTubeChannel.Shorts.cs#L304) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ClassifyShortsProbeResponse.path | [307:21](../YouTubeChannel.Shorts.cs#L307) |
| method | Emby.YouTubePlugin.YouTubeChannel.ProbeShortsRedirectAsync | [317:9](../YouTubeChannel.Shorts.cs#L317) |
| tuple-element | Emby.YouTubePlugin.YouTubeChannel.ProbeShortsRedirectAsync.Classification | [317:36](../YouTubeChannel.Shorts.cs#L317) |
| tuple-element | Emby.YouTubePlugin.YouTubeChannel.ProbeShortsRedirectAsync.ShouldRetryWithGet | [317:73](../YouTubeChannel.Shorts.cs#L317) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ProbeShortsRedirectAsync.url | [318:13](../YouTubeChannel.Shorts.cs#L318) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ProbeShortsRedirectAsync.method | [319:13](../YouTubeChannel.Shorts.cs#L319) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ProbeShortsRedirectAsync.ct | [320:13](../YouTubeChannel.Shorts.cs#L320) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ProbeShortsRedirectAsync.requestCts | [322:23](../YouTubeChannel.Shorts.cs#L322) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ProbeShortsRedirectAsync.requestToken | [324:17](../YouTubeChannel.Shorts.cs#L324) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ProbeShortsRedirectAsync.req | [326:23](../YouTubeChannel.Shorts.cs#L326) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ProbeShortsRedirectAsync.resp | [329:23](../YouTubeChannel.Shorts.cs#L329) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ProbeShortsRedirectAsync.result | [333:17](../YouTubeChannel.Shorts.cs#L333) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ProbeShortsRedirectAsync.shouldRetryWithGet | [342:21](../YouTubeChannel.Shorts.cs#L342) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ProbeShortsRedirectAsync.documentResult | [349:17](../YouTubeChannel.Shorts.cs#L349) |
| method | Emby.YouTubePlugin.YouTubeChannel.ClassifyShortsDocumentAsync | [355:9](../YouTubeChannel.Shorts.cs#L355) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ClassifyShortsDocumentAsync.response | [356:13](../YouTubeChannel.Shorts.cs#L356) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ClassifyShortsDocumentAsync.requestUri | [357:13](../YouTubeChannel.Shorts.cs#L357) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ClassifyShortsDocumentAsync.ct | [358:13](../YouTubeChannel.Shorts.cs#L358) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ClassifyShortsDocumentAsync.mediaType | [360:17](../YouTubeChannel.Shorts.cs#L360) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ClassifyShortsDocumentAsync.stream | [364:23](../YouTubeChannel.Shorts.cs#L364) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ClassifyShortsDocumentAsync.buffer | [365:17](../YouTubeChannel.Shorts.cs#L365) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ClassifyShortsDocumentAsync.total | [366:17](../YouTubeChannel.Shorts.cs#L366) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ClassifyShortsDocumentAsync.read | [369:21](../YouTubeChannel.Shorts.cs#L369) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ClassifyShortsDocumentAsync.expectedUrl | [380:17](../YouTubeChannel.Shorts.cs#L380) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ClassifyShortsDocumentAsync.expectedUrlMarker | [381:17](../YouTubeChannel.Shorts.cs#L381) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ClassifyShortsDocumentAsync.hasExpectedUrl | [383:17](../YouTubeChannel.Shorts.cs#L383) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ClassifyShortsDocumentAsync.hasWatchBootstrap | [384:17](../YouTubeChannel.Shorts.cs#L384) |
| method | Emby.YouTubePlugin.YouTubeChannel.ContainsBytes | [392:9](../YouTubeChannel.Shorts.cs#L392) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ContainsBytes.buffer | [392:43](../YouTubeChannel.Shorts.cs#L392) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ContainsBytes.count | [392:58](../YouTubeChannel.Shorts.cs#L392) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ContainsBytes.marker | [392:69](../YouTubeChannel.Shorts.cs#L392) |
| method | Emby.YouTubePlugin.YouTubeChannel.EvictShortsUrlProbeCache | [395:9](../YouTubeChannel.Shorts.cs#L395) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EvictShortsUrlProbeCache.now | [400:17](../YouTubeChannel.Shorts.cs#L400) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.EvictShortsUrlProbeCache.kvp | [401:13](../YouTubeChannel.Shorts.cs#L401) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.EvictShortsUrlProbeCache.oldest | [409:21](../YouTubeChannel.Shorts.cs#L409) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.EvictShortsUrlProbeCache.oldest.anonymous@18915 | [410:30](../YouTubeChannel.Shorts.cs#L410) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.EvictShortsUrlProbeCache.oldest.anonymous@18915.kvp | [410:30](../YouTubeChannel.Shorts.cs#L410) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.EvictShortsUrlProbeCache.oldest.anonymous@19055 | [412:29](../YouTubeChannel.Shorts.cs#L412) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.EvictShortsUrlProbeCache.oldest.anonymous@19055.kvp | [412:29](../YouTubeChannel.Shorts.cs#L412) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.EvictShortsUrlProbeCache.key | [414:17](../YouTubeChannel.Shorts.cs#L414) |
| method | Emby.YouTubePlugin.YouTubeChannel.GetChannelShortVideoIdsAsync | [419:9](../YouTubeChannel.Shorts.cs#L419) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetChannelShortVideoIdsAsync.channelId | [420:13](../YouTubeChannel.Shorts.cs#L420) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetChannelShortVideoIdsAsync.cancellationToken | [421:13](../YouTubeChannel.Shorts.cs#L421) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelShortVideoIdsAsync.cached | [426:64](../YouTubeChannel.Shorts.cs#L426) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelShortVideoIdsAsync.ids | [434:17](../YouTubeChannel.Shorts.cs#L434) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelShortVideoIdsAsync.url | [438:21](../YouTubeChannel.Shorts.cs#L438) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelShortVideoIdsAsync.html | [439:21](../YouTubeChannel.Shorts.cs#L439) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelShortVideoIdsAsync.match | [444:17](../YouTubeChannel.Shorts.cs#L444) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelShortVideoIdsAsync.match.groupIndex | [446:30](../YouTubeChannel.Shorts.cs#L446) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelShortVideoIdsAsync.match.value | [448:29](../YouTubeChannel.Shorts.cs#L448) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelShortVideoIdsAsync.hasVideoIdMarker | [457:25](../YouTubeChannel.Shorts.cs#L457) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelShortVideoIdsAsync.hasShortsPathMarker | [459:25](../YouTubeChannel.Shorts.cs#L459) |
| catch-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelShortVideoIdsAsync.ex | [474:19](../YouTubeChannel.Shorts.cs#L474) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelShortVideoIdsAsync.stale | [477:68](../YouTubeChannel.Shorts.cs#L477) |
| method | Emby.YouTubePlugin.YouTubeChannel.IsUsableShortsPage | [487:9](../YouTubeChannel.Shorts.cs#L487) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.IsUsableShortsPage.html | [487:48](../YouTubeChannel.Shorts.cs#L487) |
| method | Emby.YouTubePlugin.YouTubeChannel.GetShortsPageHtmlAsync | [499:9](../YouTubeChannel.Shorts.cs#L499) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetShortsPageHtmlAsync.url | [499:66](../YouTubeChannel.Shorts.cs#L499) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetShortsPageHtmlAsync.cancellationToken | [499:78](../YouTubeChannel.Shorts.cs#L499) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetShortsPageHtmlAsync.deadline | [501:23](../YouTubeChannel.Shorts.cs#L501) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetShortsPageHtmlAsync.request | [503:23](../YouTubeChannel.Shorts.cs#L503) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetShortsPageHtmlAsync.response | [506:23](../YouTubeChannel.Shorts.cs#L506) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetShortsPageHtmlAsync.maximumBytes | [513:23](../YouTubeChannel.Shorts.cs#L513) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetShortsPageHtmlAsync.stream | [516:23](../YouTubeChannel.Shorts.cs#L516) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetShortsPageHtmlAsync.body | [517:23](../YouTubeChannel.Shorts.cs#L517) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetShortsPageHtmlAsync.buffer | [518:17](../YouTubeChannel.Shorts.cs#L518) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetShortsPageHtmlAsync.read | [521:21](../YouTubeChannel.Shorts.cs#L521) |
| method | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsPageMatches | [531:9](../YouTubeChannel.Shorts.cs#L531) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsPageMatches.batch | [531:52](../YouTubeChannel.Shorts.cs#L531) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsPageMatches.shortsVideoIds | [531:81](../YouTubeChannel.Shorts.cs#L531) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsPageMatches.item | [536:13](../YouTubeChannel.Shorts.cs#L536) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.ApplyShortsPageMatches.item.rawId | [542:21](../YouTubeChannel.Shorts.cs#L542) |

</details>

<a id="file-bd6961656d4c3822"></a>
## [YouTubeChannel.State.cs](../YouTubeChannel.State.cs)

Plugin Compile source (SDK default); 4189 bytes; 86 lines.

<details>
<summary>All 52 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [4:1](../YouTubeChannel.State.cs#L4) |
| type | Emby.YouTubePlugin.YouTubeChannel | [6:5](../YouTubeChannel.State.cs#L6) |
| field | Emby.YouTubePlugin.YouTubeChannel.ChannelIdPrefix | [8:30](../YouTubeChannel.State.cs#L8) |
| field | Emby.YouTubePlugin.YouTubeChannel.HandlePrefix | [9:30](../YouTubeChannel.State.cs#L9) |
| field | Emby.YouTubePlugin.YouTubeChannel.FolderSeparator | [10:30](../YouTubeChannel.State.cs#L10) |
| field | Emby.YouTubePlugin.YouTubeChannel.MaxMetaCacheEntries | [11:27](../YouTubeChannel.State.cs#L11) |
| type | Emby.YouTubePlugin.YouTubeChannel.VideoMeta | [13:9](../YouTubeChannel.State.cs#L13) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.VideoMeta.Overview | [14:13](../YouTubeChannel.State.cs#L14) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.VideoMeta.Premiere | [14:31](../YouTubeChannel.State.cs#L14) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.VideoMeta.Year | [14:51](../YouTubeChannel.State.cs#L14) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.VideoMeta.RuntimeTicks | [15:13](../YouTubeChannel.State.cs#L15) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.VideoMeta.ThumbUrl | [15:33](../YouTubeChannel.State.cs#L15) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.VideoMeta.CachedAt | [15:51](../YouTubeChannel.State.cs#L15) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.VideoMeta.OriginalLang | [16:13](../YouTubeChannel.State.cs#L16) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.VideoMeta.Description | [17:13](../YouTubeChannel.State.cs#L17) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.VideoMeta.ViewCount | [17:41](../YouTubeChannel.State.cs#L17) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.VideoMeta.LikeCount | [18:13](../YouTubeChannel.State.cs#L18) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.VideoMeta.CommentCount | [18:37](../YouTubeChannel.State.cs#L18) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.VideoMeta.HasEngagementMetadata | [19:13](../YouTubeChannel.State.cs#L19) |
| field | Emby.YouTubePlugin.YouTubeChannel.MetaCache | [21:73](../YouTubeChannel.State.cs#L21) |
| field | Emby.YouTubePlugin.YouTubeChannel.ShortsPageCache | [22:84](../YouTubeChannel.State.cs#L22) |
| field | Emby.YouTubePlugin.YouTubeChannel.MetaCacheTtl | [23:42](../YouTubeChannel.State.cs#L23) |
| field | Emby.YouTubePlugin.YouTubeChannel.ShortsPageCacheTtl | [24:42](../YouTubeChannel.State.cs#L24) |
| field | Emby.YouTubePlugin.YouTubeChannel.ShortsPageEmptyCacheTtl | [25:42](../YouTubeChannel.State.cs#L25) |
| field | Emby.YouTubePlugin.YouTubeChannel.NegativeCacheTtl | [26:42](../YouTubeChannel.State.cs#L26) |
| type | Emby.YouTubePlugin.YouTubeChannel.ShortsPageCacheEntry | [28:9](../YouTubeChannel.State.cs#L28) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ShortsPageCacheEntry.VideoIds | [28:52](../YouTubeChannel.State.cs#L28) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ShortsPageCacheEntry.CachedAt | [28:105](../YouTubeChannel.State.cs#L28) |
| type | Emby.YouTubePlugin.YouTubeChannel.ChannelPageProbeResult | [29:9](../YouTubeChannel.State.cs#L29) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ChannelPageProbeResult.VideoIds | [30:13](../YouTubeChannel.State.cs#L30) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ChannelPageProbeResult.LookupSucceeded | [31:13](../YouTubeChannel.State.cs#L31) |
| method | Emby.YouTubePlugin.YouTubeChannel.ResetCrossFolderSeen | [38:9](../YouTubeChannel.State.cs#L38) |
| method | Emby.YouTubePlugin.YouTubeChannel.StripPrefix | [40:9](../YouTubeChannel.State.cs#L40) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.StripPrefix.id | [40:43](../YouTubeChannel.State.cs#L40) |
| method | Emby.YouTubePlugin.YouTubeChannel.IsSupportedPublicPlaylistId | [47:9](../YouTubeChannel.State.cs#L47) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.IsSupportedPublicPlaylistId.value | [47:58](../YouTubeChannel.State.cs#L47) |
| method | Emby.YouTubePlugin.YouTubeChannel.IsSupportedChannelId | [54:9](../YouTubeChannel.State.cs#L54) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.IsSupportedChannelId.value | [54:50](../YouTubeChannel.State.cs#L54) |
| method | Emby.YouTubePlugin.YouTubeChannel.IsSupportedHandle | [59:9](../YouTubeChannel.State.cs#L59) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.IsSupportedHandle.value | [59:47](../YouTubeChannel.State.cs#L59) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.IsSupportedHandle.anonymous@3094 | [62:34](../YouTubeChannel.State.cs#L62) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.IsSupportedHandle.anonymous@3094.character | [62:34](../YouTubeChannel.State.cs#L62) |
| method | Emby.YouTubePlugin.YouTubeChannel.HasResourceIdCharacters | [64:9](../YouTubeChannel.State.cs#L64) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.HasResourceIdCharacters.value | [64:53](../YouTubeChannel.State.cs#L64) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.HasResourceIdCharacters.anonymous@3231 | [65:23](../YouTubeChannel.State.cs#L65) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.HasResourceIdCharacters.anonymous@3231.character | [65:23](../YouTubeChannel.State.cs#L65) |
| method | Emby.YouTubePlugin.YouTubeChannel.IsPrivateWatchLaterPlaylistId | [72:9](../YouTubeChannel.State.cs#L72) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.IsPrivateWatchLaterPlaylistId.value | [72:59](../YouTubeChannel.State.cs#L72) |
| method | Emby.YouTubePlugin.YouTubeChannel.HasReservedResourcePrefix | [75:9](../YouTubeChannel.State.cs#L75) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.HasReservedResourcePrefix.value | [75:55](../YouTubeChannel.State.cs#L75) |
| field | Emby.YouTubePlugin.YouTubeChannel.LivePrefix | [83:30](../YouTubeChannel.State.cs#L83) |
| field | Emby.YouTubePlugin.YouTubeChannel.ReelPrefix | [84:30](../YouTubeChannel.State.cs#L84) |

</details>

<a id="file-aa77154cbf6e5636"></a>
## [YouTubeChannel.cs](../YouTubeChannel.cs)

Plugin Compile source (SDK default); 8522 bytes; 208 lines.

<details>
<summary>All 47 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [18:1](../YouTubeChannel.cs#L18) |
| type | Emby.YouTubePlugin.YouTubeChannel | [20:5](../YouTubeChannel.cs#L20) |
| method | Emby.YouTubePlugin.YouTubeChannel.Log | [22:9](../YouTubeChannel.cs#L22) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.Log.msg | [22:33](../YouTubeChannel.cs#L22) |
| method | Emby.YouTubePlugin.YouTubeChannel.LogPublic | [23:9](../YouTubeChannel.cs#L23) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LogPublic.msg | [23:38](../YouTubeChannel.cs#L23) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.LogPublic.logger | [25:17](../YouTubeChannel.cs#L25) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChannel.LogPublic.anonymous@887 | [27:64](../YouTubeChannel.cs#L27) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LogPublic.anonymous@887.s | [27:65](../YouTubeChannel.cs#L27) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.LogPublic.anonymous@887._ | [27:68](../YouTubeChannel.cs#L27) |
| property | Emby.YouTubePlugin.YouTubeChannel.Name | [38:9](../YouTubeChannel.cs#L38) |
| property | Emby.YouTubePlugin.YouTubeChannel.Description | [39:9](../YouTubeChannel.cs#L39) |
| property | Emby.YouTubePlugin.YouTubeChannel.Id | [40:9](../YouTubeChannel.cs#L40) |
| field | Emby.YouTubePlugin.YouTubeChannel.ChannelDataRevision | [41:30](../YouTubeChannel.cs#L41) |
| property | Emby.YouTubePlugin.YouTubeChannel.DataVersion | [49:9](../YouTubeChannel.cs#L49) |
| accessor | Emby.YouTubePlugin.YouTubeChannel.DataVersion.get | [51:13](../YouTubeChannel.cs#L51) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.DataVersion.get.config | [55:25](../YouTubeChannel.cs#L55) |
| property | Emby.YouTubePlugin.YouTubeChannel.ParentalRating | [66:9](../YouTubeChannel.cs#L66) |
| property | Emby.YouTubePlugin.YouTubeChannel.IsEnabledByDefault | [67:9](../YouTubeChannel.cs#L67) |
| property | Emby.YouTubePlugin.YouTubeChannel.Features | [69:9](../YouTubeChannel.cs#L69) |
| method | Emby.YouTubePlugin.YouTubeChannel.GetChannelFeatures | [74:9](../YouTubeChannel.cs#L74) |
| method | Emby.YouTubePlugin.YouTubeChannel.GetChannelItems | [76:9](../YouTubeChannel.cs#L76) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetChannelItems.query | [77:13](../YouTubeChannel.cs#L77) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetChannelItems.cancellationToken | [77:45](../YouTubeChannel.cs#L77) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelItems.items | [79:17](../YouTubeChannel.cs#L79) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelItems.plugin | [80:17](../YouTubeChannel.cs#L80) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelItems.config | [84:17](../YouTubeChannel.cs#L84) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelItems.apiKey | [85:17](../YouTubeChannel.cs#L85) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelItems.sepIdx | [109:25](../YouTubeChannel.cs#L109) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelItems.type | [111:28](../YouTubeChannel.cs#L111) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelItems.term | [112:28](../YouTubeChannel.cs#L112) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelItems.region | [139:29](../YouTubeChannel.cs#L139) |
| catch-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelItems.ex | [153:19](../YouTubeChannel.cs#L153) |
| method | Emby.YouTubePlugin.YouTubeChannel.GetSupportedChannelImages | [160:9](../YouTubeChannel.cs#L160) |
| method | Emby.YouTubePlugin.YouTubeChannel.GetChannelImage | [163:9](../YouTubeChannel.cs#L163) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetChannelImage.type | [164:13](../YouTubeChannel.cs#L164) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.GetChannelImage.cancellationToken | [164:29](../YouTubeChannel.cs#L164) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelImage.response | [166:17](../YouTubeChannel.cs#L166) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelImage.t | [167:17](../YouTubeChannel.cs#L167) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelImage.stream | [168:17](../YouTubeChannel.cs#L168) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelImage.assemblyDir | [175:17](../YouTubeChannel.cs#L175) |
| local-variable | Emby.YouTubePlugin.YouTubeChannel.GetChannelImage.filePath | [176:17](../YouTubeChannel.cs#L176) |
| method | Emby.YouTubePlugin.YouTubeChannel.Msg | [185:9](../YouTubeChannel.cs#L185) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.Msg.items | [185:46](../YouTubeChannel.cs#L185) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.Msg.msg | [185:75](../YouTubeChannel.cs#L185) |
| method | Emby.YouTubePlugin.YouTubeChannel.ToResult | [200:9](../YouTubeChannel.cs#L200) |
| parameter | Emby.YouTubePlugin.YouTubeChannel.ToResult.items | [200:51](../YouTubeChannel.cs#L200) |

</details>

<a id="file-90d85cea05c050fd"></a>
## [YouTubeChapterSync.cs](../YouTubeChapterSync.cs)

Plugin Compile source (SDK default); 9995 bytes; 194 lines.

<details>
<summary>All 75 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [12:1](../YouTubeChapterSync.cs#L12) |
| type | Emby.YouTubePlugin.YouTubeChapterSync | [16:5](../YouTubeChapterSync.cs#L16) |
| field | Emby.YouTubePlugin.YouTubeChapterSync.Gates | [18:42](../YouTubeChapterSync.cs#L18) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChapterSync.Gates.anonymous@620 | [18:81](../YouTubeChapterSync.cs#L18) |
| parameter | Emby.YouTubePlugin.YouTubeChapterSync.Gates.anonymous@620._ | [18:81](../YouTubeChapterSync.cs#L18) |
| field | Emby.YouTubePlugin.YouTubeChapterSync.StateAdmissionGate | [19:40](../YouTubeChapterSync.cs#L19) |
| field | Emby.YouTubePlugin.YouTubeChapterSync.MaximumItems | [20:27](../YouTubeChapterSync.cs#L20) |
| field | Emby.YouTubePlugin.YouTubeChapterSync.MaximumStateBytes | [21:27](../YouTubeChapterSync.cs#L21) |
| method | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize | [23:9](../YouTubeChapterSync.cs#L23) |
| parameter | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.itemId | [23:42](../YouTubeChapterSync.cs#L23) |
| parameter | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.videoId | [23:58](../YouTubeChapterSync.cs#L23) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.id | [26:59](../YouTubeChapterSync.cs#L26) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.incoming | [28:17](../YouTubeChapterSync.cs#L28) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.library | [34:25](../YouTubeChapterSync.cs#L34) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.repository | [35:25](../YouTubeChapterSync.cs#L35) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.item | [36:25](../YouTubeChapterSync.cs#L36) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.existing | [40:25](../YouTubeChapterSync.cs#L40) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.ordinary | [42:25](../YouTubeChapterSync.cs#L42) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.ordinary.anonymous@1965 | [42:51](../YouTubeChapterSync.cs#L42) |
| parameter | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.ordinary.anonymous@1965.c | [42:51](../YouTubeChapterSync.cs#L42) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.oldHash | [43:25](../YouTubeChapterSync.cs#L43) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.newHash | [44:25](../YouTubeChapterSync.cs#L44) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.root | [46:25](../YouTubeChapterSync.cs#L46) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.directory | [48:25](../YouTubeChapterSync.cs#L48) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.path | [51:25](../YouTubeChapterSync.cs#L51) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.state | [52:28](../YouTubeChapterSync.cs#L52) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.info | [55:29](../YouTubeChapterSync.cs#L55) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.ownedFingerprint | [64:59](../YouTubeChapterSync.cs#L64) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.merged | [69:25](../YouTubeChapterSync.cs#L69) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.merged.anonymous@3696 | [69:49](../YouTubeChapterSync.cs#L69) |
| parameter | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.merged.anonymous@3696.c | [69:49](../YouTubeChapterSync.cs#L69) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.merged.anonymous@3787 | [70:51](../YouTubeChapterSync.cs#L70) |
| parameter | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.merged.anonymous@3787.c | [70:51](../YouTubeChapterSync.cs#L70) |
| catch-variable | Emby.YouTubePlugin.YouTubeChapterSync.Synchronize.error | [94:19](../YouTubeChapterSync.cs#L94) |
| method | Emby.YouTubePlugin.YouTubeChapterSync.ApplyPreferredChapters | [103:9](../YouTubeChapterSync.cs#L103) |
| parameter | Emby.YouTubePlugin.YouTubeChapterSync.ApplyPreferredChapters.response | [103:53](../YouTubeChapterSync.cs#L103) |
| parameter | Emby.YouTubePlugin.YouTubeChapterSync.ApplyPreferredChapters.itemId | [103:70](../YouTubeChapterSync.cs#L103) |
| parameter | Emby.YouTubePlugin.YouTubeChapterSync.ApplyPreferredChapters.videoId | [103:86](../YouTubeChapterSync.cs#L103) |
| pattern-variable | Emby.YouTubePlugin.YouTubeChapterSync.ApplyPreferredChapters.id | [106:59](../YouTubeChapterSync.cs#L106) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.ApplyPreferredChapters.sources | [110:21](../YouTubeChapterSync.cs#L110) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.ApplyPreferredChapters.owned | [113:21](../YouTubeChapterSync.cs#L113) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChapterSync.ApplyPreferredChapters.owned.anonymous@6260 | [114:28](../YouTubeChapterSync.cs#L114) |
| parameter | Emby.YouTubePlugin.YouTubeChapterSync.ApplyPreferredChapters.owned.anonymous@6260.source | [114:28](../YouTubeChapterSync.cs#L114) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.ApplyPreferredChapters.library | [119:25](../YouTubeChapterSync.cs#L119) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.ApplyPreferredChapters.repository | [120:25](../YouTubeChapterSync.cs#L120) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.ApplyPreferredChapters.item | [121:25](../YouTubeChapterSync.cs#L121) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.ApplyPreferredChapters.existing | [125:25](../YouTubeChapterSync.cs#L125) |
| iteration-variable | Emby.YouTubePlugin.YouTubeChapterSync.ApplyPreferredChapters.source | [130:21](../YouTubeChapterSync.cs#L130) |
| catch-variable | Emby.YouTubePlugin.YouTubeChapterSync.ApplyPreferredChapters.error | [134:19](../YouTubeChapterSync.cs#L134) |
| method | Emby.YouTubePlugin.YouTubeChapterSync.CloneChapter | [141:9](../YouTubeChapterSync.cs#L141) |
| parameter | Emby.YouTubePlugin.YouTubeChapterSync.CloneChapter.chapter | [141:49](../YouTubeChapterSync.cs#L141) |
| method | Emby.YouTubePlugin.YouTubeChapterSync.Fingerprint | [152:9](../YouTubeChapterSync.cs#L152) |
| parameter | Emby.YouTubePlugin.YouTubeChapterSync.Fingerprint.chapters | [152:44](../YouTubeChapterSync.cs#L152) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.Fingerprint.value | [156:17](../YouTubeChapterSync.cs#L156) |
| anonymous-function | Emby.YouTubePlugin.YouTubeChapterSync.Fingerprint.value.anonymous@8445 | [156:41](../YouTubeChapterSync.cs#L156) |
| parameter | Emby.YouTubePlugin.YouTubeChapterSync.Fingerprint.value.anonymous@8445.c | [156:41](../YouTubeChapterSync.cs#L156) |
| method | Emby.YouTubePlugin.YouTubeChapterSync.Store | [164:9](../YouTubeChapterSync.cs#L164) |
| parameter | Emby.YouTubePlugin.YouTubeChapterSync.Store.path | [164:35](../YouTubeChapterSync.cs#L164) |
| parameter | Emby.YouTubePlugin.YouTubeChapterSync.Store.state | [164:48](../YouTubeChapterSync.cs#L164) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.Store.bytes | [166:17](../YouTubeChapterSync.cs#L166) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.Store.temporary | [168:17](../YouTubeChapterSync.cs#L168) |
| local-variable | Emby.YouTubePlugin.YouTubeChapterSync.Store.file | [171:28](../YouTubeChapterSync.cs#L171) |
| type | Emby.YouTubePlugin.YouTubeChapterSync.State | [186:9](../YouTubeChapterSync.cs#L186) |
| property | Emby.YouTubePlugin.YouTubeChapterSync.State.ItemId | [188:13](../YouTubeChapterSync.cs#L188) |
| accessor | Emby.YouTubePlugin.YouTubeChapterSync.State.ItemId.get | [188:34](../YouTubeChapterSync.cs#L188) |
| accessor | Emby.YouTubePlugin.YouTubeChapterSync.State.ItemId.set | [188:39](../YouTubeChapterSync.cs#L188) |
| property | Emby.YouTubePlugin.YouTubeChapterSync.State.VideoId | [189:13](../YouTubeChapterSync.cs#L189) |
| accessor | Emby.YouTubePlugin.YouTubeChapterSync.State.VideoId.get | [189:37](../YouTubeChapterSync.cs#L189) |
| accessor | Emby.YouTubePlugin.YouTubeChapterSync.State.VideoId.set | [189:42](../YouTubeChapterSync.cs#L189) |
| property | Emby.YouTubePlugin.YouTubeChapterSync.State.OwnedFingerprint | [190:13](../YouTubeChapterSync.cs#L190) |
| accessor | Emby.YouTubePlugin.YouTubeChapterSync.State.OwnedFingerprint.get | [190:47](../YouTubeChapterSync.cs#L190) |
| accessor | Emby.YouTubePlugin.YouTubeChapterSync.State.OwnedFingerprint.set | [190:52](../YouTubeChapterSync.cs#L190) |
| property | Emby.YouTubePlugin.YouTubeChapterSync.State.OriginalChapters | [191:13](../YouTubeChapterSync.cs#L191) |
| accessor | Emby.YouTubePlugin.YouTubeChapterSync.State.OriginalChapters.get | [191:53](../YouTubeChapterSync.cs#L191) |
| accessor | Emby.YouTubePlugin.YouTubeChapterSync.State.OriginalChapters.set | [191:58](../YouTubeChapterSync.cs#L191) |

</details>

<a id="file-afb9dc3e97044d5d"></a>
## [YouTubeConfigSaveService.cs](../YouTubeConfigSaveService.cs)

Plugin Compile source (SDK default); 10549 bytes; 222 lines.

<details>
<summary>All 117 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [12:1](../YouTubeConfigSaveService.cs#L12) |
| type | Emby.YouTubePlugin.SaveYouTubeConfiguration | [21:5](../YouTubeConfigSaveService.cs#L21) |
| property | Emby.YouTubePlugin.SaveYouTubeConfiguration.RequestStream | [25:9](../YouTubeConfigSaveService.cs#L25) |
| accessor | Emby.YouTubePlugin.SaveYouTubeConfiguration.RequestStream.get | [25:39](../YouTubeConfigSaveService.cs#L25) |
| accessor | Emby.YouTubePlugin.SaveYouTubeConfiguration.RequestStream.set | [25:44](../YouTubeConfigSaveService.cs#L25) |
| type | Emby.YouTubePlugin.SaveYouTubeConfigurationResult | [28:5](../YouTubeConfigSaveService.cs#L28) |
| property | Emby.YouTubePlugin.SaveYouTubeConfigurationResult.Saved | [30:9](../YouTubeConfigSaveService.cs#L30) |
| accessor | Emby.YouTubePlugin.SaveYouTubeConfigurationResult.Saved.get | [30:29](../YouTubeConfigSaveService.cs#L30) |
| accessor | Emby.YouTubePlugin.SaveYouTubeConfigurationResult.Saved.set | [30:34](../YouTubeConfigSaveService.cs#L30) |
| type | Emby.YouTubePlugin.GetYouTubeRegions | [33:5](../YouTubeConfigSaveService.cs#L33) |
| type | Emby.YouTubePlugin.YouTubeRegionInfo | [39:5](../YouTubeConfigSaveService.cs#L39) |
| property | Emby.YouTubePlugin.YouTubeRegionInfo.Code | [41:9](../YouTubeConfigSaveService.cs#L41) |
| accessor | Emby.YouTubePlugin.YouTubeRegionInfo.Code.get | [41:30](../YouTubeConfigSaveService.cs#L41) |
| accessor | Emby.YouTubePlugin.YouTubeRegionInfo.Code.set | [41:35](../YouTubeConfigSaveService.cs#L41) |
| property | Emby.YouTubePlugin.YouTubeRegionInfo.Name | [42:9](../YouTubeConfigSaveService.cs#L42) |
| accessor | Emby.YouTubePlugin.YouTubeRegionInfo.Name.get | [42:30](../YouTubeConfigSaveService.cs#L42) |
| accessor | Emby.YouTubePlugin.YouTubeRegionInfo.Name.set | [42:35](../YouTubeConfigSaveService.cs#L42) |
| type | Emby.YouTubePlugin.GetYouTubeRegionsResult | [45:5](../YouTubeConfigSaveService.cs#L45) |
| property | Emby.YouTubePlugin.GetYouTubeRegionsResult.LookupSucceeded | [47:9](../YouTubeConfigSaveService.cs#L47) |
| accessor | Emby.YouTubePlugin.GetYouTubeRegionsResult.LookupSucceeded.get | [47:39](../YouTubeConfigSaveService.cs#L47) |
| accessor | Emby.YouTubePlugin.GetYouTubeRegionsResult.LookupSucceeded.set | [47:44](../YouTubeConfigSaveService.cs#L47) |
| property | Emby.YouTubePlugin.GetYouTubeRegionsResult.Regions | [48:9](../YouTubeConfigSaveService.cs#L48) |
| accessor | Emby.YouTubePlugin.GetYouTubeRegionsResult.Regions.get | [48:50](../YouTubeConfigSaveService.cs#L48) |
| accessor | Emby.YouTubePlugin.GetYouTubeRegionsResult.Regions.set | [48:55](../YouTubeConfigSaveService.cs#L48) |
| type | Emby.YouTubePlugin.YouTubeConfigSaveService | [51:5](../YouTubeConfigSaveService.cs#L51) |
| field | Emby.YouTubePlugin.YouTubeConfigSaveService.JsonOptions | [53:55](../YouTubeConfigSaveService.cs#L53) |
| field | Emby.YouTubePlugin.YouTubeConfigSaveService.SaveLock | [61:40](../YouTubeConfigSaveService.cs#L61) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.Request | [63:9](../YouTubeConfigSaveService.cs#L63) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.Request.get | [63:35](../YouTubeConfigSaveService.cs#L63) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.Request.set | [63:40](../YouTubeConfigSaveService.cs#L63) |
| method | Emby.YouTubePlugin.YouTubeConfigSaveService.Get | [65:9](../YouTubeConfigSaveService.cs#L65) |
| parameter | Emby.YouTubePlugin.YouTubeConfigSaveService.Get.request | [65:39](../YouTubeConfigSaveService.cs#L65) |
| local-variable | Emby.YouTubePlugin.YouTubeConfigSaveService.Get.apiKey | [67:17](../YouTubeConfigSaveService.cs#L67) |
| local-variable | Emby.YouTubePlugin.YouTubeConfigSaveService.Get.timeout | [71:23](../YouTubeConfigSaveService.cs#L71) |
| local-variable | Emby.YouTubePlugin.YouTubeConfigSaveService.Get.doc | [72:27](../YouTubeConfigSaveService.cs#L72) |
| pattern-variable | Emby.YouTubePlugin.YouTubeConfigSaveService.Get.items | [85:73](../YouTubeConfigSaveService.cs#L85) |
| local-variable | Emby.YouTubePlugin.YouTubeConfigSaveService.Get.regions | [91:21](../YouTubeConfigSaveService.cs#L91) |
| iteration-variable | Emby.YouTubePlugin.YouTubeConfigSaveService.Get.item | [92:17](../YouTubeConfigSaveService.cs#L92) |
| local-variable | Emby.YouTubePlugin.YouTubeConfigSaveService.Get.item.code | [94:25](../YouTubeConfigSaveService.cs#L94) |
| local-variable | Emby.YouTubePlugin.YouTubeConfigSaveService.Get.item.name | [96:25](../YouTubeConfigSaveService.cs#L96) |
| anonymous-function | Emby.YouTubePlugin.YouTubeConfigSaveService.Get.item.anonymous@3995 | [98:38](../YouTubeConfigSaveService.cs#L98) |
| parameter | Emby.YouTubePlugin.YouTubeConfigSaveService.Get.item.anonymous@3995.character | [98:38](../YouTubeConfigSaveService.cs#L98) |
| anonymous-function | Emby.YouTubePlugin.YouTubeConfigSaveService.Get.anonymous@4552 | [113:33](../YouTubeConfigSaveService.cs#L113) |
| parameter | Emby.YouTubePlugin.YouTubeConfigSaveService.Get.anonymous@4552.pair | [113:33](../YouTubeConfigSaveService.cs#L113) |
| anonymous-function | Emby.YouTubePlugin.YouTubeConfigSaveService.Get.anonymous@4655 | [114:34](../YouTubeConfigSaveService.cs#L114) |
| parameter | Emby.YouTubePlugin.YouTubeConfigSaveService.Get.anonymous@4655.region | [114:34](../YouTubeConfigSaveService.cs#L114) |
| method | Emby.YouTubePlugin.YouTubeConfigSaveService.Post | [120:9](../YouTubeConfigSaveService.cs#L120) |
| parameter | Emby.YouTubePlugin.YouTubeConfigSaveService.Post.request | [120:40](../YouTubeConfigSaveService.cs#L120) |
| local-variable | Emby.YouTubePlugin.YouTubeConfigSaveService.Post.plugin | [122:17](../YouTubeConfigSaveService.cs#L122) |
| local-variable | Emby.YouTubePlugin.YouTubeConfigSaveService.Post.dto | [130:17](../YouTubeConfigSaveService.cs#L130) |
| local-variable | Emby.YouTubePlugin.YouTubeConfigSaveService.Post.config | [136:21](../YouTubeConfigSaveService.cs#L136) |
| method | Emby.YouTubePlugin.YouTubeConfigSaveService.ReadDtoAsync | [171:9](../YouTubeConfigSaveService.cs#L171) |
| parameter | Emby.YouTubePlugin.YouTubeConfigSaveService.ReadDtoAsync.request | [171:60](../YouTubeConfigSaveService.cs#L171) |
| local-variable | Emby.YouTubePlugin.YouTubeConfigSaveService.ReadDtoAsync.reader | [178:23](../YouTubeConfigSaveService.cs#L178) |
| local-variable | Emby.YouTubePlugin.YouTubeConfigSaveService.ReadDtoAsync.json | [184:17](../YouTubeConfigSaveService.cs#L184) |
| catch-variable | Emby.YouTubePlugin.YouTubeConfigSaveService.ReadDtoAsync.ex | [192:19](../YouTubeConfigSaveService.cs#L192) |
| type | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto | [198:9](../YouTubeConfigSaveService.cs#L198) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ApiKey | [200:13](../YouTubeConfigSaveService.cs#L200) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ApiKey.get | [200:37](../YouTubeConfigSaveService.cs#L200) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ApiKey.set | [200:42](../YouTubeConfigSaveService.cs#L200) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.SavedItems | [201:13](../YouTubeConfigSaveService.cs#L201) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.SavedItems.get | [201:41](../YouTubeConfigSaveService.cs#L201) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.SavedItems.set | [201:46](../YouTubeConfigSaveService.cs#L201) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.WatchLaterPlaylist | [202:13](../YouTubeConfigSaveService.cs#L202) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.WatchLaterPlaylist.get | [202:49](../YouTubeConfigSaveService.cs#L202) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.WatchLaterPlaylist.set | [202:54](../YouTubeConfigSaveService.cs#L202) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ShowTrending | [203:13](../YouTubeConfigSaveService.cs#L203) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ShowTrending.get | [203:41](../YouTubeConfigSaveService.cs#L203) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ShowTrending.set | [203:46](../YouTubeConfigSaveService.cs#L203) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ShowCategories | [204:13](../YouTubeConfigSaveService.cs#L204) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ShowCategories.get | [204:43](../YouTubeConfigSaveService.cs#L204) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ShowCategories.set | [204:48](../YouTubeConfigSaveService.cs#L204) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ShowRootFoldersAtTopLevel | [205:13](../YouTubeConfigSaveService.cs#L205) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ShowRootFoldersAtTopLevel.get | [205:54](../YouTubeConfigSaveService.cs#L205) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ShowRootFoldersAtTopLevel.set | [205:59](../YouTubeConfigSaveService.cs#L205) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ShowShorts | [206:13](../YouTubeConfigSaveService.cs#L206) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ShowShorts.get | [206:39](../YouTubeConfigSaveService.cs#L206) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ShowShorts.set | [206:44](../YouTubeConfigSaveService.cs#L206) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.HideShorts | [207:13](../YouTubeConfigSaveService.cs#L207) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.HideShorts.get | [207:39](../YouTubeConfigSaveService.cs#L207) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.HideShorts.set | [207:44](../YouTubeConfigSaveService.cs#L207) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.TrendingRegion | [208:13](../YouTubeConfigSaveService.cs#L208) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.TrendingRegion.get | [208:45](../YouTubeConfigSaveService.cs#L208) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.TrendingRegion.set | [208:50](../YouTubeConfigSaveService.cs#L208) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.TrendingCategory | [209:13](../YouTubeConfigSaveService.cs#L209) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.TrendingCategory.get | [209:47](../YouTubeConfigSaveService.cs#L209) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.TrendingCategory.set | [209:52](../YouTubeConfigSaveService.cs#L209) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ShowLikeCount | [210:13](../YouTubeConfigSaveService.cs#L210) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ShowLikeCount.get | [210:42](../YouTubeConfigSaveService.cs#L210) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ShowLikeCount.set | [210:47](../YouTubeConfigSaveService.cs#L210) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ShowCommentCount | [211:13](../YouTubeConfigSaveService.cs#L211) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ShowCommentCount.get | [211:45](../YouTubeConfigSaveService.cs#L211) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ShowCommentCount.set | [211:50](../YouTubeConfigSaveService.cs#L211) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ChannelSortBy | [212:13](../YouTubeConfigSaveService.cs#L212) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ChannelSortBy.get | [212:44](../YouTubeConfigSaveService.cs#L212) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.ChannelSortBy.set | [212:49](../YouTubeConfigSaveService.cs#L212) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.MaxChannelVideos | [213:13](../YouTubeConfigSaveService.cs#L213) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.MaxChannelVideos.get | [213:44](../YouTubeConfigSaveService.cs#L213) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.MaxChannelVideos.set | [213:49](../YouTubeConfigSaveService.cs#L213) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.MaxSearchVideos | [214:13](../YouTubeConfigSaveService.cs#L214) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.MaxSearchVideos.get | [214:43](../YouTubeConfigSaveService.cs#L214) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.MaxSearchVideos.set | [214:48](../YouTubeConfigSaveService.cs#L214) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.WatchLaterPollMinutes | [215:13](../YouTubeConfigSaveService.cs#L215) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.WatchLaterPollMinutes.get | [215:49](../YouTubeConfigSaveService.cs#L215) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.WatchLaterPollMinutes.set | [215:54](../YouTubeConfigSaveService.cs#L215) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.DirectPlaybackEnabled | [216:13](../YouTubeConfigSaveService.cs#L216) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.DirectPlaybackEnabled.get | [216:50](../YouTubeConfigSaveService.cs#L216) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.DirectPlaybackEnabled.set | [216:55](../YouTubeConfigSaveService.cs#L216) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.DirectPlaybackMaxHeight | [217:13](../YouTubeConfigSaveService.cs#L217) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.DirectPlaybackMaxHeight.get | [217:51](../YouTubeConfigSaveService.cs#L217) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.DirectPlaybackMaxHeight.set | [217:56](../YouTubeConfigSaveService.cs#L217) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.DirectPlaybackH264MaxHeight | [218:13](../YouTubeConfigSaveService.cs#L218) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.DirectPlaybackH264MaxHeight.get | [218:55](../YouTubeConfigSaveService.cs#L218) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.DirectPlaybackH264MaxHeight.set | [218:60](../YouTubeConfigSaveService.cs#L218) |
| property | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.Donate | [219:13](../YouTubeConfigSaveService.cs#L219) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.Donate.get | [219:37](../YouTubeConfigSaveService.cs#L219) |
| accessor | Emby.YouTubePlugin.YouTubeConfigSaveService.ConfigDto.Donate.set | [219:42](../YouTubeConfigSaveService.cs#L219) |

</details>

<a id="file-1922be3cbc99a179"></a>
## [YouTubeDirectHttp.cs](../YouTubeDirectHttp.cs)

Plugin Compile source (SDK default); 30432 bytes; 545 lines.

<details>
<summary>All 181 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [14:1](../YouTubeDirectHttp.cs#L14) |
| type | Emby.YouTubePlugin.YouTubeMediaProbe | [16:5](../YouTubeDirectHttp.cs#L16) |
| parameter | Emby.YouTubePlugin.YouTubeMediaProbe.ReceivedBytes | [16:46](../YouTubeDirectHttp.cs#L16) |
| parameter | Emby.YouTubePlugin.YouTubeMediaProbe.Elapsed | [16:65](../YouTubeDirectHttp.cs#L16) |
| type | Emby.YouTubePlugin.YouTubeDirectHttp | [20:5](../YouTubeDirectHttp.cs#L20) |
| field | Emby.YouTubePlugin.YouTubeDirectHttp.MaximumPayloadBytes | [22:27](../YouTubeDirectHttp.cs#L22) |
| field | Emby.YouTubePlugin.YouTubeDirectHttp.MaximumMediaPlaylistBytes | [23:28](../YouTubeDirectHttp.cs#L23) |
| field | Emby.YouTubePlugin.YouTubeDirectHttp.RequestSlots | [24:47](../YouTubeDirectHttp.cs#L24) |
| field | Emby.YouTubePlugin.YouTubeDirectHttp._pendingRequests | [25:28](../YouTubeDirectHttp.cs#L25) |
| field | Emby.YouTubePlugin.YouTubeDirectHttp.Client | [26:44](../YouTubeDirectHttp.cs#L26) |
| field | Emby.YouTubePlugin.YouTubeDirectHttp.RangeClient | [27:44](../YouTubeDirectHttp.cs#L27) |
| field | Emby.YouTubePlugin.YouTubeDirectHttp.AllowedHeaders | [28:49](../YouTubeDirectHttp.cs#L28) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateMediaUri | [33:9](../YouTubeDirectHttp.cs#L33) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateMediaUri.value | [33:46](../YouTubeDirectHttp.cs#L33) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateMediaUri.uri | [35:17](../YouTubeDirectHttp.cs#L35) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateCaptionUri | [41:9](../YouTubeDirectHttp.cs#L41) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateCaptionUri.value | [41:48](../YouTubeDirectHttp.cs#L41) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateCaptionUri.uri | [43:17](../YouTubeDirectHttp.cs#L43) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.ReadHeaders | [50:9](../YouTubeDirectHttp.cs#L50) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.ReadHeaders.headers | [50:64](../YouTubeDirectHttp.cs#L50) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.ReadHeaders.result | [52:17](../YouTubeDirectHttp.cs#L52) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectHttp.ReadHeaders.property | [55:13](../YouTubeDirectHttp.cs#L55) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.GetBytesAsync | [65:9](../YouTubeDirectHttp.cs#L65) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.GetBytesAsync.uri | [66:13](../YouTubeDirectHttp.cs#L66) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.GetBytesAsync.headers | [66:22](../YouTubeDirectHttp.cs#L66) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.GetBytesAsync.maximumBytes | [66:67](../YouTubeDirectHttp.cs#L66) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.GetBytesAsync.cancellationToken | [67:13](../YouTubeDirectHttp.cs#L67) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.GetMediaPlaylistBytesAsync | [73:9](../YouTubeDirectHttp.cs#L73) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.GetMediaPlaylistBytesAsync.uri | [74:13](../YouTubeDirectHttp.cs#L74) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.GetMediaPlaylistBytesAsync.headers | [74:22](../YouTubeDirectHttp.cs#L74) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.GetMediaPlaylistBytesAsync.maximumBytes | [74:67](../YouTubeDirectHttp.cs#L74) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.GetMediaPlaylistBytesAsync.cancellationToken | [75:13](../YouTubeDirectHttp.cs#L75) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.CopyHeaders | [78:9](../YouTubeDirectHttp.cs#L78) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.CopyHeaders.headers | [78:64](../YouTubeDirectHttp.cs#L78) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.CopyHeaders.result | [81:17](../YouTubeDirectHttp.cs#L81) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectHttp.CopyHeaders.header | [82:13](../YouTubeDirectHttp.cs#L82) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.GetRangeBytesAsync | [87:9](../YouTubeDirectHttp.cs#L87) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.GetRangeBytesAsync.uri | [87:57](../YouTubeDirectHttp.cs#L87) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.GetRangeBytesAsync.headers | [87:66](../YouTubeDirectHttp.cs#L87) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.GetRangeBytesAsync.offset | [88:13](../YouTubeDirectHttp.cs#L88) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.GetRangeBytesAsync.count | [88:26](../YouTubeDirectHttp.cs#L88) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.GetRangeBytesAsync.totalLength | [88:37](../YouTubeDirectHttp.cs#L88) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.GetRangeBytesAsync.cancellationToken | [88:55](../YouTubeDirectHttp.cs#L88) |
| field | Emby.YouTubePlugin.YouTubeDirectHttp.MaximumProbeBytes | [91:28](../YouTubeDirectHttp.cs#L91) |
| field | Emby.YouTubePlugin.YouTubeDirectHttp.MinimumProbeBytes | [92:28](../YouTubeDirectHttp.cs#L92) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.ProbeMediaAsync | [94:9](../YouTubeDirectHttp.cs#L94) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.ProbeMediaAsync.uri | [94:65](../YouTubeDirectHttp.cs#L94) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.ProbeMediaAsync.headers | [95:13](../YouTubeDirectHttp.cs#L95) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.ProbeMediaAsync.cancellationToken | [95:58](../YouTubeDirectHttp.cs#L95) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync | [103:9](../YouTubeDirectHttp.cs#L103) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.uri | [103:70](../YouTubeDirectHttp.cs#L103) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.headers | [104:13](../YouTubeDirectHttp.cs#L104) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.cancellationToken | [104:58](../YouTubeDirectHttp.cs#L104) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.client | [104:95](../YouTubeDirectHttp.cs#L104) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.current | [107:17](../YouTubeDirectHttp.cs#L107) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.acquired | [114:18](../YouTubeDirectHttp.cs#L114) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.received | [115:17](../YouTubeDirectHttp.cs#L115) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.prefix | [116:17](../YouTubeDirectHttp.cs#L116) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.elapsed | [117:17](../YouTubeDirectHttp.cs#L117) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.deadline | [118:23](../YouTubeDirectHttp.cs#L118) |
| local-function | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.HasMediaSample | [120:13](../YouTubeDirectHttp.cs#L120) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.redirect | [126:26](../YouTubeDirectHttp.cs#L126) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.request | [128:31](../YouTubeDirectHttp.cs#L128) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.header | [129:21](../YouTubeDirectHttp.cs#L129) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.response | [134:31](../YouTubeDirectHttp.cs#L134) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.anonymous@7622 | [145:70](../YouTubeDirectHttp.cs#L145) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.anonymous@7622.value | [145:70](../YouTubeDirectHttp.cs#L145) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.type | [147:25](../YouTubeDirectHttp.cs#L147) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.range | [153:29](../YouTubeDirectHttp.cs#L153) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.body | [161:31](../YouTubeDirectHttp.cs#L161) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.buffer | [162:25](../YouTubeDirectHttp.cs#L162) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.read | [165:29](../YouTubeDirectHttp.cs#L165) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchProbeAsync.length | [169:79](../YouTubeDirectHttp.cs#L169) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync | [200:9](../YouTubeDirectHttp.cs#L200) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.uri | [200:59](../YouTubeDirectHttp.cs#L200) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.headers | [200:68](../YouTubeDirectHttp.cs#L200) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.offset | [201:13](../YouTubeDirectHttp.cs#L201) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.count | [201:26](../YouTubeDirectHttp.cs#L201) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.totalLength | [201:37](../YouTubeDirectHttp.cs#L201) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.cancellationToken | [201:55](../YouTubeDirectHttp.cs#L201) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.client | [201:92](../YouTubeDirectHttp.cs#L201) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.current | [206:17](../YouTubeDirectHttp.cs#L206) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.acquired | [213:18](../YouTubeDirectHttp.cs#L213) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.deadline | [214:23](../YouTubeDirectHttp.cs#L214) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.end | [220:22](../YouTubeDirectHttp.cs#L220) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.redirect | [221:26](../YouTubeDirectHttp.cs#L221) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.request | [223:31](../YouTubeDirectHttp.cs#L223) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.header | [224:21](../YouTubeDirectHttp.cs#L224) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.response | [229:31](../YouTubeDirectHttp.cs#L229) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.range | [240:25](../YouTubeDirectHttp.cs#L240) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.anonymous@14085 | [244:73](../YouTubeDirectHttp.cs#L244) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.anonymous@14085.value | [244:73](../YouTubeDirectHttp.cs#L244) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.body | [246:31](../YouTubeDirectHttp.cs#L246) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.bytes | [247:25](../YouTubeDirectHttp.cs#L247) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.received | [248:25](../YouTubeDirectHttp.cs#L248) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchRangeAsync.read | [251:29](../YouTubeDirectHttp.cs#L251) |
| type | Emby.YouTubePlugin.YouTubeDirectHttp.RangeForbiddenException | [277:9](../YouTubeDirectHttp.cs#L277) |
| constructor | Emby.YouTubePlugin.YouTubeDirectHttp.RangeForbiddenException.RangeForbiddenException | [279:13](../YouTubeDirectHttp.cs#L279) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.RangeForbiddenException.RangeForbiddenException.rateLimited | [279:46](../YouTubeDirectHttp.cs#L279) |
| property | Emby.YouTubePlugin.YouTubeDirectHttp.RangeForbiddenException.RateLimited | [280:13](../YouTubeDirectHttp.cs#L280) |
| accessor | Emby.YouTubePlugin.YouTubeDirectHttp.RangeForbiddenException.RateLimited.get | [280:41](../YouTubeDirectHttp.cs#L280) |
| type | Emby.YouTubePlugin.YouTubeDirectHttp.RangeBusyException | [283:9](../YouTubeDirectHttp.cs#L283) |
| constructor | Emby.YouTubePlugin.YouTubeDirectHttp.RangeBusyException.RangeBusyException | [285:13](../YouTubeDirectHttp.cs#L285) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.FetchAsync | [288:9](../YouTubeDirectHttp.cs#L288) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchAsync.uri | [289:13](../YouTubeDirectHttp.cs#L289) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchAsync.headers | [289:22](../YouTubeDirectHttp.cs#L289) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchAsync.maximumBytes | [289:67](../YouTubeDirectHttp.cs#L289) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchAsync.cancellationToken | [290:13](../YouTubeDirectHttp.cs#L290) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchAsync.client | [290:50](../YouTubeDirectHttp.cs#L290) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.FetchMediaPlaylistAsync | [293:9](../YouTubeDirectHttp.cs#L293) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchMediaPlaylistAsync.uri | [294:13](../YouTubeDirectHttp.cs#L294) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchMediaPlaylistAsync.headers | [294:22](../YouTubeDirectHttp.cs#L294) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchMediaPlaylistAsync.maximumBytes | [294:67](../YouTubeDirectHttp.cs#L294) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchMediaPlaylistAsync.cancellationToken | [295:13](../YouTubeDirectHttp.cs#L295) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchMediaPlaylistAsync.client | [295:50](../YouTubeDirectHttp.cs#L295) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync | [298:9](../YouTubeDirectHttp.cs#L298) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.uri | [299:13](../YouTubeDirectHttp.cs#L299) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.headers | [299:22](../YouTubeDirectHttp.cs#L299) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.maximumBytes | [299:67](../YouTubeDirectHttp.cs#L299) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.cancellationToken | [300:13](../YouTubeDirectHttp.cs#L300) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.client | [300:50](../YouTubeDirectHttp.cs#L300) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.mediaPlaylist | [300:69](../YouTubeDirectHttp.cs#L300) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.isCaption | [305:18](../YouTubeDirectHttp.cs#L305) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.validate | [306:31](../YouTubeDirectHttp.cs#L306) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.current | [307:17](../YouTubeDirectHttp.cs#L307) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.acquired | [315:18](../YouTubeDirectHttp.cs#L315) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.deadline | [316:23](../YouTubeDirectHttp.cs#L316) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.redirect | [322:26](../YouTubeDirectHttp.cs#L322) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.request | [324:31](../YouTubeDirectHttp.cs#L324) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.header | [325:21](../YouTubeDirectHttp.cs#L325) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.response | [328:31](../YouTubeDirectHttp.cs#L328) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.body | [341:31](../YouTubeDirectHttp.cs#L341) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.output | [342:31](../YouTubeDirectHttp.cs#L342) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.buffer | [343:25](../YouTubeDirectHttp.cs#L343) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.read | [346:29](../YouTubeDirectHttp.cs#L346) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.FetchPayloadAsync.bytes | [351:33](../YouTubeDirectHttp.cs#L351) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.HasPlaylistHeader | [381:9](../YouTubeDirectHttp.cs#L381) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.HasPlaylistHeader.bytes | [381:47](../YouTubeDirectHttp.cs#L381) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateHttpsUri | [390:9](../YouTubeDirectHttp.cs#L390) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateHttpsUri.value | [390:45](../YouTubeDirectHttp.cs#L390) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateHttpsUri.anonymous@22100 | [393:30](../YouTubeDirectHttp.cs#L393) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateHttpsUri.anonymous@22100.c | [393:30](../YouTubeDirectHttp.cs#L393) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateHttpsUri.uri | [395:68](../YouTubeDirectHttp.cs#L395) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateHttpsUri.anonymous@22568 | [399:33](../YouTubeDirectHttp.cs#L399) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateHttpsUri.anonymous@22568.c | [399:33](../YouTubeDirectHttp.cs#L399) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateHeaders | [404:9](../YouTubeDirectHttp.cs#L404) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateHeaders.headers | [404:45](../YouTubeDirectHttp.cs#L404) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateHeaders.total | [407:17](../YouTubeDirectHttp.cs#L407) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateHeaders.header | [408:13](../YouTubeDirectHttp.cs#L408) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateHeaders.header.anonymous@23171 | [412:41](../YouTubeDirectHttp.cs#L412) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateHeaders.header.anonymous@23171.c | [412:41](../YouTubeDirectHttp.cs#L412) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateHeaders.header.reference | [419:25](../YouTubeDirectHttp.cs#L419) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.IsMediaHost | [432:9](../YouTubeDirectHttp.cs#L432) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.IsMediaHost.host | [432:41](../YouTubeDirectHttp.cs#L432) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.IsYouTubeHost | [436:9](../YouTubeDirectHttp.cs#L436) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.IsYouTubeHost.host | [436:43](../YouTubeDirectHttp.cs#L436) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.IsRedirect | [440:9](../YouTubeDirectHttp.cs#L440) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.IsRedirect.code | [440:40](../YouTubeDirectHttp.cs#L440) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.CreateClient | [444:9](../YouTubeDirectHttp.cs#L444) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.CreateClient.allowCompression | [444:48](../YouTubeDirectHttp.cs#L444) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.CreateClient.handler | [446:17](../YouTubeDirectHttp.cs#L446) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.CreateResolverClient | [465:9](../YouTubeDirectHttp.cs#L465) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.ConnectPublicAddressAsync | [467:9](../YouTubeDirectHttp.cs#L467) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.ConnectPublicAddressAsync.context | [468:13](../YouTubeDirectHttp.cs#L468) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.ConnectPublicAddressAsync.cancellationToken | [468:51](../YouTubeDirectHttp.cs#L468) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.ConnectPublicAddressAsync.endpoint | [470:17](../YouTubeDirectHttp.cs#L470) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.ConnectPublicAddressAsync.addresses | [475:21](../YouTubeDirectHttp.cs#L475) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectHttp.ConnectPublicAddressAsync.address | [478:17](../YouTubeDirectHttp.cs#L478) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectHttp.ConnectPublicAddressAsync.address.anonymous@26968 | [478:59](../YouTubeDirectHttp.cs#L478) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.ConnectPublicAddressAsync.address.anonymous@26968.a | [478:59](../YouTubeDirectHttp.cs#L478) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.ConnectPublicAddressAsync.address.socket | [481:25](../YouTubeDirectHttp.cs#L481) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.ConnectPublicAddressAsync.address.attempt | [485:35](../YouTubeDirectHttp.cs#L485) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.IsPublicAddress | [510:9](../YouTubeDirectHttp.cs#L510) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.IsPublicAddress.address | [510:46](../YouTubeDirectHttp.cs#L510) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectHttp.IsPublicAddress.bytes | [513:17](../YouTubeDirectHttp.cs#L513) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateAddresses | [537:9](../YouTubeDirectHttp.cs#L537) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateAddresses.addresses | [537:47](../YouTubeDirectHttp.cs#L537) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateAddresses.anonymous@30219 | [539:81](../YouTubeDirectHttp.cs#L539) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.ValidateAddresses.anonymous@30219.a | [539:81](../YouTubeDirectHttp.cs#L539) |
| method | Emby.YouTubePlugin.YouTubeDirectHttp.Failure | [543:9](../YouTubeDirectHttp.cs#L543) |
| parameter | Emby.YouTubePlugin.YouTubeDirectHttp.Failure.message | [543:58](../YouTubeDirectHttp.cs#L543) |

</details>

<a id="file-5a4b4574992dd577"></a>
## [YouTubeDirectManifest.cs](../YouTubeDirectManifest.cs)

Plugin Compile source (SDK default); 13694 bytes; 215 lines.

<details>
<summary>All 148 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [7:1](../YouTubeDirectManifest.cs#L7) |
| type | Emby.YouTubePlugin.YouTubeNoHlsVariantException | [9:5](../YouTubeDirectManifest.cs#L9) |
| constructor | Emby.YouTubePlugin.YouTubeNoHlsVariantException.YouTubeNoHlsVariantException | [11:9](../YouTubeDirectManifest.cs#L11) |
| type | Emby.YouTubePlugin.YouTubeHlsSelection | [14:5](../YouTubeDirectManifest.cs#L14) |
| parameter | Emby.YouTubePlugin.YouTubeHlsSelection.Content | [15:9](../YouTubeDirectManifest.cs#L15) |
| parameter | Emby.YouTubePlugin.YouTubeHlsSelection.VideoUri | [15:25](../YouTubeDirectManifest.cs#L15) |
| parameter | Emby.YouTubePlugin.YouTubeHlsSelection.AudioUri | [15:39](../YouTubeDirectManifest.cs#L15) |
| parameter | Emby.YouTubePlugin.YouTubeHlsSelection.Width | [15:54](../YouTubeDirectManifest.cs#L15) |
| parameter | Emby.YouTubePlugin.YouTubeHlsSelection.Height | [15:65](../YouTubeDirectManifest.cs#L15) |
| parameter | Emby.YouTubePlugin.YouTubeHlsSelection.Bitrate | [16:9](../YouTubeDirectManifest.cs#L16) |
| parameter | Emby.YouTubePlugin.YouTubeHlsSelection.FrameRate | [16:22](../YouTubeDirectManifest.cs#L16) |
| parameter | Emby.YouTubePlugin.YouTubeHlsSelection.VideoCodec | [16:40](../YouTubeDirectManifest.cs#L16) |
| parameter | Emby.YouTubePlugin.YouTubeHlsSelection.AudioCodec | [16:59](../YouTubeDirectManifest.cs#L16) |
| parameter | Emby.YouTubePlugin.YouTubeHlsSelection.AudioChannels | [17:9](../YouTubeDirectManifest.cs#L17) |
| parameter | Emby.YouTubePlugin.YouTubeHlsSelection.AudioLanguage | [17:29](../YouTubeDirectManifest.cs#L17) |
| type | Emby.YouTubePlugin.YouTubeDirectManifest | [22:5](../YouTubeDirectManifest.cs#L22) |
| field | Emby.YouTubePlugin.YouTubeDirectManifest.MaximumMasterBytes | [24:28](../YouTubeDirectManifest.cs#L24) |
| method | Emby.YouTubePlugin.YouTubeDirectManifest.Select | [26:9](../YouTubeDirectManifest.cs#L26) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.text | [27:13](../YouTubeDirectManifest.cs#L27) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.masterUri | [27:26](../YouTubeDirectManifest.cs#L27) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.maximumHeight | [27:41](../YouTubeDirectManifest.cs#L27) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.validateUri | [27:60](../YouTubeDirectManifest.cs#L27) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.requiredVideoCodec | [28:13](../YouTubeDirectManifest.cs#L28) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.maximumBitrate | [28:48](../YouTubeDirectManifest.cs#L28) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.requiredAudioCodec | [28:76](../YouTubeDirectManifest.cs#L28) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.Select.lines | [33:17](../YouTubeDirectManifest.cs#L33) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.Select.lines.anonymous@1606 | [33:93](../YouTubeDirectManifest.cs#L33) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.lines.anonymous@1606.x | [33:93](../YouTubeDirectManifest.cs#L33) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.Select.audio | [34:17](../YouTubeDirectManifest.cs#L34) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.Select.variants | [35:17](../YouTubeDirectManifest.cs#L35) |
| tuple-element | Emby.YouTubePlugin.YouTubeDirectManifest.Select.variants.Attributes | [35:38](../YouTubeDirectManifest.cs#L35) |
| tuple-element | Emby.YouTubePlugin.YouTubeDirectManifest.Select.variants.Uri | [35:77](../YouTubeDirectManifest.cs#L35) |
| tuple-element | Emby.YouTubePlugin.YouTubeDirectManifest.Select.variants.Width | [35:86](../YouTubeDirectManifest.cs#L35) |
| tuple-element | Emby.YouTubePlugin.YouTubeDirectManifest.Select.variants.Height | [35:97](../YouTubeDirectManifest.cs#L35) |
| tuple-element | Emby.YouTubePlugin.YouTubeDirectManifest.Select.variants.Bitrate | [35:109](../YouTubeDirectManifest.cs#L35) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.Select.i | [36:22](../YouTubeDirectManifest.cs#L36) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.Select.line | [38:21](../YouTubeDirectManifest.cs#L38) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.Select.attrs | [47:25](../YouTubeDirectManifest.cs#L47) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.Select.attrs | [56:25](../YouTubeDirectManifest.cs#L56) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.Select.uri | [60:25](../YouTubeDirectManifest.cs#L60) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.Select.dimensions | [61:25](../YouTubeDirectManifest.cs#L61) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectManifest.Select.width | [63:114](../YouTubeDirectManifest.cs#L63) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectManifest.Select.height | [64:114](../YouTubeDirectManifest.cs#L64) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectManifest.Select.bitrate | [66:126](../YouTubeDirectManifest.cs#L66) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible | [76:17](../YouTubeDirectManifest.cs#L76) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@4468 | [76:43](../YouTubeDirectManifest.cs#L76) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@4468.v | [76:43](../YouTubeDirectManifest.cs#L76) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@4523 | [77:24](../YouTubeDirectManifest.cs#L77) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@4523.v | [77:24](../YouTubeDirectManifest.cs#L77) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@4523.anonymous@4624 | [78:26](../YouTubeDirectManifest.cs#L78) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@4523.anonymous@4624.codec | [78:26](../YouTubeDirectManifest.cs#L78) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@4705 | [79:24](../YouTubeDirectManifest.cs#L79) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@4705.v | [79:24](../YouTubeDirectManifest.cs#L79) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@4705.anonymous@4806 | [80:26](../YouTubeDirectManifest.cs#L80) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@4705.anonymous@4806.codec | [80:26](../YouTubeDirectManifest.cs#L80) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@4899 | [81:36](../YouTubeDirectManifest.cs#L81) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@4899.v | [81:36](../YouTubeDirectManifest.cs#L81) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@4948 | [82:35](../YouTubeDirectManifest.cs#L82) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@4948.v | [82:35](../YouTubeDirectManifest.cs#L82) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@5071 | [83:35](../YouTubeDirectManifest.cs#L83) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@5071.v | [83:35](../YouTubeDirectManifest.cs#L83) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@5193 | [84:35](../YouTubeDirectManifest.cs#L84) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@5193.v | [84:35](../YouTubeDirectManifest.cs#L84) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@5262 | [85:35](../YouTubeDirectManifest.cs#L85) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.eligible.anonymous@5262.v | [85:35](../YouTubeDirectManifest.cs#L85) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.Select.preferred | [90:17](../YouTubeDirectManifest.cs#L90) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.Select.affordable | [92:17](../YouTubeDirectManifest.cs#L92) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.Select.affordable.anonymous@5716 | [92:45](../YouTubeDirectManifest.cs#L92) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.affordable.anonymous@5716.v | [92:45](../YouTubeDirectManifest.cs#L92) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.Select.selected | [93:17](../YouTubeDirectManifest.cs#L93) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.Select.selected.anonymous@5865 | [94:36](../YouTubeDirectManifest.cs#L94) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.selected.anonymous@5865.v | [94:36](../YouTubeDirectManifest.cs#L94) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.Select.selected.anonymous@5888 | [94:59](../YouTubeDirectManifest.cs#L94) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Select.selected.anonymous@5888.v | [94:59](../YouTubeDirectManifest.cs#L94) |
| method | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection | [98:9](../YouTubeDirectManifest.cs#L98) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.selected | [99:13](../YouTubeDirectManifest.cs#L99) |
| tuple-element | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.selected.Attributes | [99:14](../YouTubeDirectManifest.cs#L99) |
| tuple-element | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.selected.Uri | [99:53](../YouTubeDirectManifest.cs#L99) |
| tuple-element | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.selected.Width | [99:62](../YouTubeDirectManifest.cs#L99) |
| tuple-element | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.selected.Height | [99:73](../YouTubeDirectManifest.cs#L99) |
| tuple-element | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.selected.Bitrate | [99:85](../YouTubeDirectManifest.cs#L99) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.audio | [100:13](../YouTubeDirectManifest.cs#L100) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.masterUri | [100:53](../YouTubeDirectManifest.cs#L100) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.validateUri | [100:68](../YouTubeDirectManifest.cs#L100) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.codecs | [102:17](../YouTubeDirectManifest.cs#L102) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.codecs.anonymous@6353 | [102:81](../YouTubeDirectManifest.cs#L102) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.codecs.anonymous@6353.x | [102:81](../YouTubeDirectManifest.cs#L102) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.videoCodecs | [103:17](../YouTubeDirectManifest.cs#L103) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.videoCodecs.anonymous@6441 | [103:63](../YouTubeDirectManifest.cs#L103) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.videoCodecs.anonymous@6441.x | [103:63](../YouTubeDirectManifest.cs#L103) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.audioCodecs | [104:17](../YouTubeDirectManifest.cs#L104) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.audioCodecs.anonymous@6541 | [104:63](../YouTubeDirectManifest.cs#L104) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.audioCodecs.anonymous@6541.x | [104:63](../YouTubeDirectManifest.cs#L104) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.anonymous@6676 | [106:31](../YouTubeDirectManifest.cs#L106) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.anonymous@6676.x | [106:31](../YouTubeDirectManifest.cs#L106) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.rendition | [109:41](../YouTubeDirectManifest.cs#L109) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.audioUri | [110:18](../YouTubeDirectManifest.cs#L110) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.group | [111:17](../YouTubeDirectManifest.cs#L111) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.anonymous@7092 | [114:41](../YouTubeDirectManifest.cs#L114) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.anonymous@7092.a | [114:41](../YouTubeDirectManifest.cs#L114) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.anonymous@7167 | [115:40](../YouTubeDirectManifest.cs#L115) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.anonymous@7167.a | [115:40](../YouTubeDirectManifest.cs#L115) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.anonymous@7240 | [116:39](../YouTubeDirectManifest.cs#L116) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.anonymous@7240.a | [116:39](../YouTubeDirectManifest.cs#L116) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.anonymous@7306 | [117:29](../YouTubeDirectManifest.cs#L117) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.anonymous@7306.a | [117:29](../YouTubeDirectManifest.cs#L117) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.output | [124:17](../YouTubeDirectManifest.cs#L124) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.key | [130:17](../YouTubeDirectManifest.cs#L130) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.frameRate | [138:55](../YouTubeDirectManifest.cs#L138) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.range | [140:17](../YouTubeDirectManifest.cs#L140) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.average | [146:136](../YouTubeDirectManifest.cs#L146) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.channels | [150:18](../YouTubeDirectManifest.cs#L150) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectManifest.BuildSelection.count | [151:152](../YouTubeDirectManifest.cs#L151) |
| method | Emby.YouTubePlugin.YouTubeDirectManifest.ParseAttributes | [159:9](../YouTubeDirectManifest.cs#L159) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.ParseAttributes.input | [159:68](../YouTubeDirectManifest.cs#L159) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.ParseAttributes.values | [161:17](../YouTubeDirectManifest.cs#L161) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.ParseAttributes.i | [162:17](../YouTubeDirectManifest.cs#L162) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.ParseAttributes.start | [165:21](../YouTubeDirectManifest.cs#L165) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.ParseAttributes.key | [168:21](../YouTubeDirectManifest.cs#L168) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectManifest.ParseAttributes.value | [169:24](../YouTubeDirectManifest.cs#L169) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.ParseAttributes.anonymous@11674 | [183:61](../YouTubeDirectManifest.cs#L183) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.ParseAttributes.anonymous@11674.c | [183:61](../YouTubeDirectManifest.cs#L183) |
| method | Emby.YouTubePlugin.YouTubeDirectManifest.Resolve | [190:9](../YouTubeDirectManifest.cs#L190) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Resolve.value | [190:36](../YouTubeDirectManifest.cs#L190) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Resolve.master | [190:50](../YouTubeDirectManifest.cs#L190) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Resolve.validate | [190:62](../YouTubeDirectManifest.cs#L190) |
| method | Emby.YouTubePlugin.YouTubeDirectManifest.Value | [192:9](../YouTubeDirectManifest.cs#L192) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Value.attrs | [192:37](../YouTubeDirectManifest.cs#L192) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Value.key | [192:80](../YouTubeDirectManifest.cs#L192) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectManifest.Value.value | [192:126](../YouTubeDirectManifest.cs#L192) |
| method | Emby.YouTubePlugin.YouTubeDirectManifest.FrameRate | [193:9](../YouTubeDirectManifest.cs#L193) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.FrameRate.attrs | [193:41](../YouTubeDirectManifest.cs#L193) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectManifest.FrameRate.fps | [194:114](../YouTubeDirectManifest.cs#L194) |
| method | Emby.YouTubePlugin.YouTubeDirectManifest.Quote | [195:9](../YouTubeDirectManifest.cs#L195) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.Quote.value | [195:37](../YouTubeDirectManifest.cs#L195) |
| method | Emby.YouTubePlugin.YouTubeDirectManifest.SafeToken | [196:9](../YouTubeDirectManifest.cs#L196) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.SafeToken.value | [196:41](../YouTubeDirectManifest.cs#L196) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectManifest.SafeToken.anonymous@12786 | [196:68](../YouTubeDirectManifest.cs#L196) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.SafeToken.anonymous@12786.c | [196:68](../YouTubeDirectManifest.cs#L196) |
| method | Emby.YouTubePlugin.YouTubeDirectManifest.VideoCodec | [198:9](../YouTubeDirectManifest.cs#L198) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.VideoCodec.value | [198:44](../YouTubeDirectManifest.cs#L198) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectManifest.VideoCodec.s | [200:17](../YouTubeDirectManifest.cs#L200) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectManifest.VideoCodec.s | [201:17](../YouTubeDirectManifest.cs#L201) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectManifest.VideoCodec.s | [202:17](../YouTubeDirectManifest.cs#L202) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectManifest.VideoCodec.s | [203:17](../YouTubeDirectManifest.cs#L203) |
| method | Emby.YouTubePlugin.YouTubeDirectManifest.AudioCodec | [206:9](../YouTubeDirectManifest.cs#L206) |
| parameter | Emby.YouTubePlugin.YouTubeDirectManifest.AudioCodec.value | [206:44](../YouTubeDirectManifest.cs#L206) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectManifest.AudioCodec.s | [208:17](../YouTubeDirectManifest.cs#L208) |

</details>

<a id="file-5ec1b64fd6d6c34f"></a>
## [YouTubeDirectMetadata.cs](../YouTubeDirectMetadata.cs)

Plugin Compile source (SDK default); 11206 bytes; 224 lines.

<details>
<summary>All 136 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [8:1](../YouTubeDirectMetadata.cs#L8) |
| type | Emby.YouTubePlugin.YouTubeDirectMetadata | [12:5](../YouTubeDirectMetadata.cs#L12) |
| field | Emby.YouTubePlugin.YouTubeDirectMetadata.MaximumEntries | [14:28](../YouTubeDirectMetadata.cs#L14) |
| field | Emby.YouTubePlugin.YouTubeDirectMetadata.MaximumBytes | [15:28](../YouTubeDirectMetadata.cs#L15) |
| field | Emby.YouTubePlugin.YouTubeDirectMetadata.MaximumDescriptionCharacters | [16:28](../YouTubeDirectMetadata.cs#L16) |
| field | Emby.YouTubePlugin.YouTubeDirectMetadata.MaximumChapters | [17:28](../YouTubeDirectMetadata.cs#L17) |
| field | Emby.YouTubePlugin.YouTubeDirectMetadata.MaximumChapterTitleCharacters | [18:28](../YouTubeDirectMetadata.cs#L18) |
| field | Emby.YouTubePlugin.YouTubeDirectMetadata.Freshness | [19:43](../YouTubeDirectMetadata.cs#L19) |
| field | Emby.YouTubePlugin.YouTubeDirectMetadata.MaximumDurationSeconds | [20:31](../YouTubeDirectMetadata.cs#L20) |
| field | Emby.YouTubePlugin.YouTubeDirectMetadata.Cache | [21:47](../YouTubeDirectMetadata.cs#L21) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectMetadata.Cache.anonymous@888 | [21:59](../YouTubeDirectMetadata.cs#L21) |
| method | Emby.YouTubePlugin.YouTubeDirectMetadata.Record | [25:9](../YouTubeDirectMetadata.cs#L25) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.Record.videoId | [25:37](../YouTubeDirectMetadata.cs#L25) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.Record.metadata | [25:53](../YouTubeDirectMetadata.cs#L25) |
| method | Emby.YouTubePlugin.YouTubeDirectMetadata.TryGet | [27:9](../YouTubeDirectMetadata.cs#L27) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.TryGet.videoId | [27:37](../YouTubeDirectMetadata.cs#L27) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.TryGet.metadata | [27:53](../YouTubeDirectMetadata.cs#L27) |
| method | Emby.YouTubePlugin.YouTubeDirectMetadata.GetChapters | [29:9](../YouTubeDirectMetadata.cs#L29) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.GetChapters.videoId | [29:51](../YouTubeDirectMetadata.cs#L29) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.GetChapters.metadata | [30:37](../YouTubeDirectMetadata.cs#L30) |
| type | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot | [32:9](../YouTubeDirectMetadata.cs#L32) |
| field | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot._chapters | [34:40](../YouTubeDirectMetadata.cs#L34) |
| constructor | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.Snapshot | [36:13](../YouTubeDirectMetadata.cs#L36) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.Snapshot.capturedUtc | [36:31](../YouTubeDirectMetadata.cs#L36) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.Snapshot.description | [36:53](../YouTubeDirectMetadata.cs#L36) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.Snapshot.views | [36:74](../YouTubeDirectMetadata.cs#L36) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.Snapshot.likes | [36:87](../YouTubeDirectMetadata.cs#L36) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.Snapshot.comments | [37:17](../YouTubeDirectMetadata.cs#L37) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.Snapshot.durationTicks | [37:33](../YouTubeDirectMetadata.cs#L37) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.Snapshot.chapters | [37:54](../YouTubeDirectMetadata.cs#L37) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.Snapshot.anonymous@2224 | [48:36](../YouTubeDirectMetadata.cs#L48) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.Snapshot.anonymous@2224.chapter | [48:36](../YouTubeDirectMetadata.cs#L48) |
| property | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.CapturedUtc | [51:13](../YouTubeDirectMetadata.cs#L51) |
| accessor | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.CapturedUtc.get | [51:45](../YouTubeDirectMetadata.cs#L51) |
| property | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.Description | [52:13](../YouTubeDirectMetadata.cs#L52) |
| accessor | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.Description.get | [52:44](../YouTubeDirectMetadata.cs#L52) |
| property | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.ViewCount | [53:13](../YouTubeDirectMetadata.cs#L53) |
| accessor | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.ViewCount.get | [53:40](../YouTubeDirectMetadata.cs#L53) |
| property | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.LikeCount | [54:13](../YouTubeDirectMetadata.cs#L54) |
| accessor | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.LikeCount.get | [54:40](../YouTubeDirectMetadata.cs#L54) |
| property | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.CommentCount | [55:13](../YouTubeDirectMetadata.cs#L55) |
| accessor | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.CommentCount.get | [55:43](../YouTubeDirectMetadata.cs#L55) |
| property | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.DurationTicks | [56:13](../YouTubeDirectMetadata.cs#L56) |
| accessor | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.DurationTicks.get | [56:44](../YouTubeDirectMetadata.cs#L56) |
| property | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.SizeBytes | [57:13](../YouTubeDirectMetadata.cs#L57) |
| accessor | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.SizeBytes.get | [57:38](../YouTubeDirectMetadata.cs#L57) |
| method | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.GetChapters | [60:13](../YouTubeDirectMetadata.cs#L60) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.GetChapters.anonymous@2769 | [60:70](../YouTubeDirectMetadata.cs#L60) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.GetChapters.anonymous@2769.chapter | [60:71](../YouTubeDirectMetadata.cs#L60) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.Snapshot.GetChapters.anonymous@2769.index | [60:80](../YouTubeDirectMetadata.cs#L60) |
| type | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache | [69:9](../YouTubeDirectMetadata.cs#L69) |
| field | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache._gate | [71:37](../YouTubeDirectMetadata.cs#L71) |
| field | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache._entries | [72:59](../YouTubeDirectMetadata.cs#L72) |
| field | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache._utcNow | [73:45](../YouTubeDirectMetadata.cs#L73) |
| field | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache._bytes | [74:25](../YouTubeDirectMetadata.cs#L74) |
| constructor | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.MetadataCache | [76:13](../YouTubeDirectMetadata.cs#L76) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.MetadataCache.utcNow | [76:36](../YouTubeDirectMetadata.cs#L76) |
| method | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.Record | [78:13](../YouTubeDirectMetadata.cs#L78) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.Record.videoId | [78:34](../YouTubeDirectMetadata.cs#L78) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.Record.metadata | [78:50](../YouTubeDirectMetadata.cs#L78) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.Record.id | [81:63](../YouTubeDirectMetadata.cs#L81) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.Record.now | [85:21](../YouTubeDirectMetadata.cs#L85) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.Record.duration | [86:23](../YouTubeDirectMetadata.cs#L86) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.Record.snapshot | [87:21](../YouTubeDirectMetadata.cs#L87) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.Record.cacheNow | [95:25](../YouTubeDirectMetadata.cs#L95) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.Record.previous | [99:63](../YouTubeDirectMetadata.cs#L99) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.Record.anonymous@5126 | [104:49](../YouTubeDirectMetadata.cs#L104) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.Record.anonymous@5126.pair | [104:49](../YouTubeDirectMetadata.cs#L104) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.Record.anonymous@5194 | [105:37](../YouTubeDirectMetadata.cs#L105) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.Record.anonymous@5194.pair | [105:37](../YouTubeDirectMetadata.cs#L105) |
| method | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.TryGet | [109:13](../YouTubeDirectMetadata.cs#L109) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.TryGet.videoId | [109:34](../YouTubeDirectMetadata.cs#L109) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.TryGet.metadata | [109:50](../YouTubeDirectMetadata.cs#L109) |
| method | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.Prune | [120:13](../YouTubeDirectMetadata.cs#L120) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.Prune.now | [120:32](../YouTubeDirectMetadata.cs#L120) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.Prune.pair | [122:17](../YouTubeDirectMetadata.cs#L122) |
| method | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.Remove | [127:13](../YouTubeDirectMetadata.cs#L127) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.Remove.videoId | [127:33](../YouTubeDirectMetadata.cs#L127) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.MetadataCache.Remove.removed | [129:54](../YouTubeDirectMetadata.cs#L129) |
| type | Emby.YouTubePlugin.YouTubeDirectMetadata.Chapter | [133:9](../YouTubeDirectMetadata.cs#L133) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.Chapter.StartTicks | [133:40](../YouTubeDirectMetadata.cs#L133) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.Chapter.Name | [133:57](../YouTubeDirectMetadata.cs#L133) |
| method | Emby.YouTubePlugin.YouTubeDirectMetadata.IsVideoId | [135:9](../YouTubeDirectMetadata.cs#L135) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.IsVideoId.value | [135:39](../YouTubeDirectMetadata.cs#L135) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectMetadata.IsVideoId.anonymous@6275 | [135:93](../YouTubeDirectMetadata.cs#L135) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.IsVideoId.anonymous@6275.character | [135:93](../YouTubeDirectMetadata.cs#L135) |
| method | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadCount | [138:9](../YouTubeDirectMetadata.cs#L138) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadCount.metadata | [138:40](../YouTubeDirectMetadata.cs#L138) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadCount.name | [138:62](../YouTubeDirectMetadata.cs#L138) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadCount.value | [139:51](../YouTubeDirectMetadata.cs#L139) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadCount.count | [140:43](../YouTubeDirectMetadata.cs#L140) |
| method | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadDescription | [142:9](../YouTubeDirectMetadata.cs#L142) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadDescription.metadata | [142:48](../YouTubeDirectMetadata.cs#L142) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadDescription.value | [144:65](../YouTubeDirectMetadata.cs#L144) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadDescription.description | [146:17](../YouTubeDirectMetadata.cs#L146) |
| method | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadTime | [153:9](../YouTubeDirectMetadata.cs#L153) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadTime.metadata | [153:39](../YouTubeDirectMetadata.cs#L153) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadTime.name | [153:61](../YouTubeDirectMetadata.cs#L153) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadTime.allowZero | [153:74](../YouTubeDirectMetadata.cs#L153) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadTime.seconds | [155:17](../YouTubeDirectMetadata.cs#L155) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadTime.ticks | [157:17](../YouTubeDirectMetadata.cs#L157) |
| method | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadSeconds | [161:9](../YouTubeDirectMetadata.cs#L161) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadSeconds.metadata | [161:45](../YouTubeDirectMetadata.cs#L161) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadSeconds.name | [161:67](../YouTubeDirectMetadata.cs#L161) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadSeconds.allowZero | [161:80](../YouTubeDirectMetadata.cs#L161) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadSeconds.value | [163:56](../YouTubeDirectMetadata.cs#L163) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadSeconds.seconds | [164:53](../YouTubeDirectMetadata.cs#L164) |
| method | Emby.YouTubePlugin.YouTubeDirectMetadata.ToTicks | [171:9](../YouTubeDirectMetadata.cs#L171) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.ToTicks.seconds | [171:37](../YouTubeDirectMetadata.cs#L171) |
| method | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters | [174:9](../YouTubeDirectMetadata.cs#L174) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.metadata | [174:47](../YouTubeDirectMetadata.cs#L174) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.duration | [174:69](../YouTubeDirectMetadata.cs#L174) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.chapters | [176:84](../YouTubeDirectMetadata.cs#L176) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.durationSeconds | [180:17](../YouTubeDirectMetadata.cs#L180) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.result | [181:17](../YouTubeDirectMetadata.cs#L181) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.chapter | [182:13](../YouTubeDirectMetadata.cs#L182) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.chapter.start | [185:21](../YouTubeDirectMetadata.cs#L185) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.chapter.endValue | [187:64](../YouTubeDirectMetadata.cs#L187) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.chapter.end | [189:25](../YouTubeDirectMetadata.cs#L189) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.chapter.title | [193:62](../YouTubeDirectMetadata.cs#L193) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.chapter.text | [194:21](../YouTubeDirectMetadata.cs#L194) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.chapter.normalized | [195:21](../YouTubeDirectMetadata.cs#L195) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.chapter.character | [196:17](../YouTubeDirectMetadata.cs#L196) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.chapter.name | [201:21](../YouTubeDirectMetadata.cs#L201) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.anonymous@10415 | [205:35](../YouTubeDirectMetadata.cs#L205) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.anonymous@10415.chapter | [205:35](../YouTubeDirectMetadata.cs#L205) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.anonymous@10454 | [205:74](../YouTubeDirectMetadata.cs#L205) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.anonymous@10454.chapter | [205:74](../YouTubeDirectMetadata.cs#L205) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.anonymous@10509 | [206:25](../YouTubeDirectMetadata.cs#L206) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadChapters.anonymous@10509.group | [206:25](../YouTubeDirectMetadata.cs#L206) |
| method | Emby.YouTubePlugin.YouTubeDirectMetadata.LimitText | [209:9](../YouTubeDirectMetadata.cs#L209) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.LimitText.value | [209:41](../YouTubeDirectMetadata.cs#L209) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.LimitText.maximum | [209:55](../YouTubeDirectMetadata.cs#L209) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectMetadata.LimitText.length | [211:17](../YouTubeDirectMetadata.cs#L211) |
| method | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadString | [216:9](../YouTubeDirectMetadata.cs#L216) |
| parameter | Emby.YouTubePlugin.YouTubeDirectMetadata.ReadString.value | [216:43](../YouTubeDirectMetadata.cs#L216) |

</details>

<a id="file-db78dbee2ac97cd9"></a>
## [YouTubeDirectModels.cs](../YouTubeDirectModels.cs)

Plugin Compile source (SDK default); 290 bytes; 11 lines.

<details>
<summary>All 5 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [4:1](../YouTubeDirectModels.cs#L4) |
| type | Emby.YouTubePlugin.YouTubeResolvedVideo | [7:5](../YouTubeDirectModels.cs#L7) |
| parameter | Emby.YouTubePlugin.YouTubeResolvedVideo.VideoId | [8:9](../YouTubeDirectModels.cs#L8) |
| parameter | Emby.YouTubePlugin.YouTubeResolvedVideo.Metadata | [9:9](../YouTubeDirectModels.cs#L9) |
| parameter | Emby.YouTubePlugin.YouTubeResolvedVideo.ExpiresUtc | [10:9](../YouTubeDirectModels.cs#L10) |

</details>

<a id="file-2b6757d8b9192715"></a>
## [YouTubeDirectQualityPolicy.cs](../YouTubeDirectQualityPolicy.cs)

Plugin Compile source (SDK default); 1192 bytes; 28 lines.

<details>
<summary>All 12 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [3:1](../YouTubeDirectQualityPolicy.cs#L3) |
| type | Emby.YouTubePlugin.YouTubeDirectQualityPolicy | [7:5](../YouTubeDirectQualityPolicy.cs#L7) |
| field | Emby.YouTubePlugin.YouTubeDirectQualityPolicy.MinimumHeight | [9:28](../YouTubeDirectQualityPolicy.cs#L9) |
| field | Emby.YouTubePlugin.YouTubeDirectQualityPolicy.MaximumHeight | [10:28](../YouTubeDirectQualityPolicy.cs#L10) |
| field | Emby.YouTubePlugin.YouTubeDirectQualityPolicy.MaximumStreamingBitrate | [14:28](../YouTubeDirectQualityPolicy.cs#L14) |
| field | Emby.YouTubePlugin.YouTubeDirectQualityPolicy.MaximumFrameRate | [15:28](../YouTubeDirectQualityPolicy.cs#L15) |
| method | Emby.YouTubePlugin.YouTubeDirectQualityPolicy.ClampHeight | [17:9](../YouTubeDirectQualityPolicy.cs#L17) |
| parameter | Emby.YouTubePlugin.YouTubeDirectQualityPolicy.ClampHeight.height | [17:41](../YouTubeDirectQualityPolicy.cs#L17) |
| method | Emby.YouTubePlugin.YouTubeDirectQualityPolicy.ClampStreamingBitrate | [20:9](../YouTubeDirectQualityPolicy.cs#L20) |
| parameter | Emby.YouTubePlugin.YouTubeDirectQualityPolicy.ClampStreamingBitrate.requested | [20:52](../YouTubeDirectQualityPolicy.cs#L20) |
| parameter | Emby.YouTubePlugin.YouTubeDirectQualityPolicy.ClampStreamingBitrate.query | [20:69](../YouTubeDirectQualityPolicy.cs#L20) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectQualityPolicy.ClampStreamingBitrate.maximum | [22:18](../YouTubeDirectQualityPolicy.cs#L22) |

</details>

<a id="file-538c7a88a964bb3f"></a>
## [YouTubeDirectRangeProxy.cs](../YouTubeDirectRangeProxy.cs)

Plugin Compile source (SDK default); 24520 bytes; 453 lines.

<details>
<summary>All 198 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [15:1](../YouTubeDirectRangeProxy.cs#L15) |
| type | Emby.YouTubePlugin.YouTubeDirectRangeProxy | [19:5](../YouTubeDirectRangeProxy.cs#L19) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ChunkBytes | [21:27](../YouTubeDirectRangeProxy.cs#L21) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.MaximumEntries | [22:27](../YouTubeDirectRangeProxy.cs#L22) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.MaximumFileBytes | [23:28](../YouTubeDirectRangeProxy.cs#L23) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Gate | [24:40](../YouTubeDirectRangeProxy.cs#L24) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RequestSlots | [25:47](../YouTubeDirectRangeProxy.cs#L25) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.VideoId | [26:39](../YouTubeDirectRangeProxy.cs#L26) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.FormatId | [27:39](../YouTubeDirectRangeProxy.cs#L27) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.CapabilityPath | [28:39](../YouTubeDirectRangeProxy.cs#L28) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy._server | [29:32](../YouTubeDirectRangeProxy.cs#L29) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy._shutdown | [30:29](../YouTubeDirectRangeProxy.cs#L30) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy._fetchRange | [34:121](../YouTubeDirectRangeProxy.cs#L34) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy._resolve | [36:84](../YouTubeDirectRangeProxy.cs#L36) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy._invalidate | [37:39](../YouTubeDirectRangeProxy.cs#L37) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RegisterAsync | [39:9](../YouTubeDirectRangeProxy.cs#L39) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RegisterAsync.video | [39:52](../YouTubeDirectRangeProxy.cs#L39) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RegisterAsync.format | [39:80](../YouTubeDirectRangeProxy.cs#L39) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RegisterAsync.headers | [40:13](../YouTubeDirectRangeProxy.cs#L40) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RegisterAsync.cancellationToken | [40:58](../YouTubeDirectRangeProxy.cs#L40) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RegisterAsync.snapshot | [43:17](../YouTubeDirectRangeProxy.cs#L43) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.IsOwnedUri | [53:9](../YouTubeDirectRangeProxy.cs#L53) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.IsOwnedUri.value | [53:41](../YouTubeDirectRangeProxy.cs#L53) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Shutdown | [58:9](../YouTubeDirectRangeProxy.cs#L58) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Shutdown.previous | [60:21](../YouTubeDirectRangeProxy.cs#L60) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection | [65:9](../YouTubeDirectRangeProxy.cs#L65) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.video | [65:51](../YouTubeDirectRangeProxy.cs#L65) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.format | [65:79](../YouTubeDirectRangeProxy.cs#L65) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.headers | [66:13](../YouTubeDirectRangeProxy.cs#L66) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.id | [71:20](../YouTubeDirectRangeProxy.cs#L71) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.size | [72:85](../YouTubeDirectRangeProxy.cs#L72) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.length | [73:47](../YouTubeDirectRangeProxy.cs#L73) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.uri | [75:17](../YouTubeDirectRangeProxy.cs#L75) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.declaredLength | [76:19](../YouTubeDirectRangeProxy.cs#L76) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.expiry | [77:19](../YouTubeDirectRangeProxy.cs#L77) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.pair | [78:13](../YouTubeDirectRangeProxy.cs#L78) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.pair.pieces | [80:21](../YouTubeDirectRangeProxy.cs#L80) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.pair.name | [81:24](../YouTubeDirectRangeProxy.cs#L81) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.pair.value | [85:57](../YouTubeDirectRangeProxy.cs#L85) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.pair.value | [90:120](../YouTubeDirectRangeProxy.cs#L90) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.formats | [95:67](../YouTubeDirectRangeProxy.cs#L95) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.matches | [97:17](../YouTubeDirectRangeProxy.cs#L97) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.matches.anonymous@4758 | [97:58](../YouTubeDirectRangeProxy.cs#L97) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.matches.anonymous@4758.f | [97:58](../YouTubeDirectRangeProxy.cs#L97) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.identity | [99:17](../YouTubeDirectRangeProxy.cs#L99) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ValidateSelection.expires | [105:22](../YouTubeDirectRangeProxy.cs#L105) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Text | [111:9](../YouTubeDirectRangeProxy.cs#L111) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Text.value | [111:36](../YouTubeDirectRangeProxy.cs#L111) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Text.name | [111:55](../YouTubeDirectRangeProxy.cs#L111) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Text.property | [112:51](../YouTubeDirectRangeProxy.cs#L112) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Integer | [114:9](../YouTubeDirectRangeProxy.cs#L114) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Integer.value | [114:37](../YouTubeDirectRangeProxy.cs#L114) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Integer.name | [114:56](../YouTubeDirectRangeProxy.cs#L114) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Integer.property | [119:53](../YouTubeDirectRangeProxy.cs#L119) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Integer.number | [120:93](../YouTubeDirectRangeProxy.cs#L120) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Decimal | [124:9](../YouTubeDirectRangeProxy.cs#L124) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Decimal.value | [124:37](../YouTubeDirectRangeProxy.cs#L124) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Decimal.number | [124:51](../YouTubeDirectRangeProxy.cs#L124) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Decimal.anonymous@6919 | [127:63](../YouTubeDirectRangeProxy.cs#L127) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Decimal.anonymous@6919.c | [127:63](../YouTubeDirectRangeProxy.cs#L127) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ParseRange | [131:9](../YouTubeDirectRangeProxy.cs#L131) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ParseRange.value | [131:40](../YouTubeDirectRangeProxy.cs#L131) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ParseRange.length | [131:55](../YouTubeDirectRangeProxy.cs#L131) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ParseRange.start | [131:68](../YouTubeDirectRangeProxy.cs#L131) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ParseRange.end | [131:84](../YouTubeDirectRangeProxy.cs#L131) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ParseRange.pieces | [136:17](../YouTubeDirectRangeProxy.cs#L136) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ParseRange.suffix | [140:50](../YouTubeDirectRangeProxy.cs#L140) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ParseRange.requestedEnd | [146:46](../YouTubeDirectRangeProxy.cs#L146) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ReadChunkAsync | [151:9](../YouTubeDirectRangeProxy.cs#L151) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ReadChunkAsync.server | [151:58](../YouTubeDirectRangeProxy.cs#L151) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ReadChunkAsync.entry | [151:73](../YouTubeDirectRangeProxy.cs#L151) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ReadChunkAsync.request | [151:86](../YouTubeDirectRangeProxy.cs#L151) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ReadChunkAsync.offset | [152:13](../YouTubeDirectRangeProxy.cs#L152) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ReadChunkAsync.count | [152:26](../YouTubeDirectRangeProxy.cs#L152) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ReadChunkAsync.cancellationToken | [152:37](../YouTubeDirectRangeProxy.cs#L152) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ReadChunkAsync.snapshot | [154:22](../YouTubeDirectRangeProxy.cs#L154) |
| catch-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.ReadChunkAsync.error | [167:19](../YouTubeDirectRangeProxy.cs#L167) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RenewAsync | [176:9](../YouTubeDirectRangeProxy.cs#L176) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RenewAsync.server | [176:56](../YouTubeDirectRangeProxy.cs#L176) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RenewAsync.entry | [176:71](../YouTubeDirectRangeProxy.cs#L176) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RenewAsync.stale | [176:84](../YouTubeDirectRangeProxy.cs#L176) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RenewAsync.cancellationToken | [176:100](../YouTubeDirectRangeProxy.cs#L176) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RenewAsync.renewal | [178:28](../YouTubeDirectRangeProxy.cs#L178) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RenewAsync.anonymous@9952 | [185:46](../YouTubeDirectRangeProxy.cs#L185) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RenewAsync.anonymous@10046 | [186:52](../YouTubeDirectRangeProxy.cs#L186) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RenewAsync.anonymous@10046.task | [186:52](../YouTubeDirectRangeProxy.cs#L186) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RefreshAsync | [194:9](../YouTubeDirectRangeProxy.cs#L194) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RefreshAsync.server | [194:58](../YouTubeDirectRangeProxy.cs#L194) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RefreshAsync.entry | [194:73](../YouTubeDirectRangeProxy.cs#L194) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RefreshAsync.stale | [194:86](../YouTubeDirectRangeProxy.cs#L194) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RefreshAsync.deadline | [196:23](../YouTubeDirectRangeProxy.cs#L196) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RefreshAsync.resolved | [199:17](../YouTubeDirectRangeProxy.cs#L199) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RefreshAsync.formats | [200:70](../YouTubeDirectRangeProxy.cs#L200) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RefreshAsync.selected | [201:17](../YouTubeDirectRangeProxy.cs#L201) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RefreshAsync.selected.anonymous@11055 | [201:59](../YouTubeDirectRangeProxy.cs#L201) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RefreshAsync.selected.anonymous@11055.f | [201:59](../YouTubeDirectRangeProxy.cs#L201) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RefreshAsync.headers | [203:17](../YouTubeDirectRangeProxy.cs#L203) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RefreshAsync.headers.global | [203:114](../YouTubeDirectRangeProxy.cs#L203) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RefreshAsync.header | [204:13](../YouTubeDirectRangeProxy.cs#L204) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RefreshAsync.header.local | [204:117](../YouTubeDirectRangeProxy.cs#L204) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RefreshAsync.refreshed | [206:17](../YouTubeDirectRangeProxy.cs#L206) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Failure | [219:9](../YouTubeDirectRangeProxy.cs#L219) |
| type | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Identity | [221:9](../YouTubeDirectRangeProxy.cs#L221) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Identity.VideoId | [221:40](../YouTubeDirectRangeProxy.cs#L221) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Identity.FormatId | [221:56](../YouTubeDirectRangeProxy.cs#L221) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Identity.Length | [221:73](../YouTubeDirectRangeProxy.cs#L221) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Identity.VideoCodec | [221:86](../YouTubeDirectRangeProxy.cs#L221) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Identity.AudioCodec | [222:13](../YouTubeDirectRangeProxy.cs#L222) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Identity.Extension | [222:32](../YouTubeDirectRangeProxy.cs#L222) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Identity.Language | [222:50](../YouTubeDirectRangeProxy.cs#L222) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Identity.Width | [222:67](../YouTubeDirectRangeProxy.cs#L222) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Identity.Height | [222:79](../YouTubeDirectRangeProxy.cs#L222) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Identity.Channels | [222:92](../YouTubeDirectRangeProxy.cs#L222) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Identity.SampleRate | [222:107](../YouTubeDirectRangeProxy.cs#L222) |
| type | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Snapshot | [223:9](../YouTubeDirectRangeProxy.cs#L223) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Snapshot.Identity | [223:40](../YouTubeDirectRangeProxy.cs#L223) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Snapshot.Uri | [223:59](../YouTubeDirectRangeProxy.cs#L223) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Snapshot.Headers | [223:68](../YouTubeDirectRangeProxy.cs#L223) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Snapshot.ExpiresUtc | [223:113](../YouTubeDirectRangeProxy.cs#L223) |
| type | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RequestState | [224:9](../YouTubeDirectRangeProxy.cs#L224) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.RequestState.Renewed | [224:59](../YouTubeDirectRangeProxy.cs#L224) |
| type | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Entry | [225:9](../YouTubeDirectRangeProxy.cs#L225) |
| constructor | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Entry.Entry | [227:13](../YouTubeDirectRangeProxy.cs#L227) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Entry.Entry.snapshot | [227:28](../YouTubeDirectRangeProxy.cs#L227) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Entry.Entry.token | [227:47](../YouTubeDirectRangeProxy.cs#L227) |
| property | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Entry.Sync | [229:13](../YouTubeDirectRangeProxy.cs#L229) |
| accessor | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Entry.Sync.get | [229:36](../YouTubeDirectRangeProxy.cs#L229) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Entry.Current | [230:31](../YouTubeDirectRangeProxy.cs#L230) |
| property | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Entry.Identity | [231:13](../YouTubeDirectRangeProxy.cs#L231) |
| accessor | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Entry.Identity.get | [231:42](../YouTubeDirectRangeProxy.cs#L231) |
| property | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Entry.Token | [232:13](../YouTubeDirectRangeProxy.cs#L232) |
| accessor | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Entry.Token.get | [232:37](../YouTubeDirectRangeProxy.cs#L232) |
| property | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Entry.CreatedUtc | [233:13](../YouTubeDirectRangeProxy.cs#L233) |
| accessor | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Entry.CreatedUtc.get | [233:44](../YouTubeDirectRangeProxy.cs#L233) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Entry.LastAccessUtc | [234:31](../YouTubeDirectRangeProxy.cs#L234) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Entry.ActiveRequests | [235:26](../YouTubeDirectRangeProxy.cs#L235) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Entry.Renewal | [236:38](../YouTubeDirectRangeProxy.cs#L236) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Entry.RenewalRetryAfterUtc | [237:31](../YouTubeDirectRangeProxy.cs#L237) |
| type | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server | [240:9](../YouTubeDirectRangeProxy.cs#L240) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server._gate | [242:37](../YouTubeDirectRangeProxy.cs#L242) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server._listener | [243:43](../YouTubeDirectRangeProxy.cs#L243) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server._stop | [244:54](../YouTubeDirectRangeProxy.cs#L244) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server._entries | [245:56](../YouTubeDirectRangeProxy.cs#L245) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server._contexts | [246:59](../YouTubeDirectRangeProxy.cs#L246) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server._origin | [247:37](../YouTubeDirectRangeProxy.cs#L247) |
| field | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server._stopped | [248:26](../YouTubeDirectRangeProxy.cs#L248) |
| constructor | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Server | [249:13](../YouTubeDirectRangeProxy.cs#L249) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Server.listener | [249:28](../YouTubeDirectRangeProxy.cs#L249) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Server.port | [249:51](../YouTubeDirectRangeProxy.cs#L249) |
| property | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.StopToken | [251:13](../YouTubeDirectRangeProxy.cs#L251) |
| accessor | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.StopToken.get | [251:52](../YouTubeDirectRangeProxy.cs#L251) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Start | [253:13](../YouTubeDirectRangeProxy.cs#L253) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Start.attempt | [255:26](../YouTubeDirectRangeProxy.cs#L255) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Start.reservation | [257:25](../YouTubeDirectRangeProxy.cs#L257) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Start.port | [258:25](../YouTubeDirectRangeProxy.cs#L258) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Start.listener | [261:25](../YouTubeDirectRangeProxy.cs#L261) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Start.server | [266:29](../YouTubeDirectRangeProxy.cs#L266) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Register | [276:13](../YouTubeDirectRangeProxy.cs#L276) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Register.snapshot | [276:38](../YouTubeDirectRangeProxy.cs#L276) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Register.existing | [282:25](../YouTubeDirectRangeProxy.cs#L282) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Register.existing.anonymous@15401 | [282:67](../YouTubeDirectRangeProxy.cs#L282) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Register.existing.anonymous@15401.e | [282:67](../YouTubeDirectRangeProxy.cs#L282) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Register.token | [292:28](../YouTubeDirectRangeProxy.cs#L292) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Owns | [300:13](../YouTubeDirectRangeProxy.cs#L300) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Owns.value | [300:32](../YouTubeDirectRangeProxy.cs#L300) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Owns.path | [305:28](../YouTubeDirectRangeProxy.cs#L305) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Owns.entry | [306:107](../YouTubeDirectRangeProxy.cs#L306) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Prune | [313:13](../YouTubeDirectRangeProxy.cs#L313) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Prune.entry | [315:17](../YouTubeDirectRangeProxy.cs#L315) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Prune.entry.anonymous@17108 | [315:61](../YouTubeDirectRangeProxy.cs#L315) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Prune.entry.anonymous@17108.e | [315:61](../YouTubeDirectRangeProxy.cs#L315) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.AcceptAsync | [320:13](../YouTubeDirectRangeProxy.cs#L320) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.AcceptAsync.context | [326:29](../YouTubeDirectRangeProxy.cs#L326) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.HandleAsync | [341:13](../YouTubeDirectRangeProxy.cs#L341) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.HandleAsync.context | [341:44](../YouTubeDirectRangeProxy.cs#L341) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.HandleAsync.entry | [343:24](../YouTubeDirectRangeProxy.cs#L343) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.HandleAsync.committed | [344:22](../YouTubeDirectRangeProxy.cs#L344) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.HandleAsync.deadline | [345:27](../YouTubeDirectRangeProxy.cs#L345) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.HandleAsync.request | [349:25](../YouTubeDirectRangeProxy.cs#L349) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.HandleAsync.path | [355:28](../YouTubeDirectRangeProxy.cs#L355) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.HandleAsync.head | [364:26](../YouTubeDirectRangeProxy.cs#L364) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.HandleAsync.range | [365:29](../YouTubeDirectRangeProxy.cs#L365) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.HandleAsync.offset | [366:76](../YouTubeDirectRangeProxy.cs#L366) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.HandleAsync.end | [366:93](../YouTubeDirectRangeProxy.cs#L366) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.HandleAsync.remaining | [371:26](../YouTubeDirectRangeProxy.cs#L371) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.HandleAsync.state | [372:25](../YouTubeDirectRangeProxy.cs#L372) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.HandleAsync.first | [373:29](../YouTubeDirectRangeProxy.cs#L373) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.HandleAsync.response | [376:25](../YouTubeDirectRangeProxy.cs#L376) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.HandleAsync.count | [392:33](../YouTubeDirectRangeProxy.cs#L392) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.HandleAsync.bytes | [393:33](../YouTubeDirectRangeProxy.cs#L393) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Reject | [421:13](../YouTubeDirectRangeProxy.cs#L421) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Reject.context | [421:40](../YouTubeDirectRangeProxy.cs#L421) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Reject.status | [421:69](../YouTubeDirectRangeProxy.cs#L421) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Abort | [434:13](../YouTubeDirectRangeProxy.cs#L434) |
| parameter | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Abort.context | [434:39](../YouTubeDirectRangeProxy.cs#L434) |
| method | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Stop | [437:13](../YouTubeDirectRangeProxy.cs#L437) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Stop.contexts | [439:39](../YouTubeDirectRangeProxy.cs#L439) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectRangeProxy.Server.Stop.context | [449:17](../YouTubeDirectRangeProxy.cs#L449) |

</details>

<a id="file-8dd75b168cf4c866"></a>
## [YouTubeDirectSubtitles.cs](../YouTubeDirectSubtitles.cs)

Plugin Compile source (SDK default); 24799 bytes; 466 lines.

<details>
<summary>All 198 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [11:1](../YouTubeDirectSubtitles.cs#L11) |
| type | Emby.YouTubePlugin.YouTubeDirectSubtitles | [15:5](../YouTubeDirectSubtitles.cs#L15) |
| field | Emby.YouTubePlugin.YouTubeDirectSubtitles.MaximumTracks | [17:27](../YouTubeDirectSubtitles.cs#L17) |
| field | Emby.YouTubePlugin.YouTubeDirectSubtitles.MaximumCaptionBytes | [18:27](../YouTubeDirectSubtitles.cs#L18) |
| field | Emby.YouTubePlugin.YouTubeDirectSubtitles.MaximumCacheFiles | [19:27](../YouTubeDirectSubtitles.cs#L19) |
| field | Emby.YouTubePlugin.YouTubeDirectSubtitles.MaximumCacheBytes | [20:28](../YouTubeDirectSubtitles.cs#L20) |
| field | Emby.YouTubePlugin.YouTubeDirectSubtitles.Retention | [21:42](../YouTubeDirectSubtitles.cs#L21) |
| field | Emby.YouTubePlugin.YouTubeDirectSubtitles.TotalBudget | [22:42](../YouTubeDirectSubtitles.cs#L22) |
| field | Emby.YouTubePlugin.YouTubeDirectSubtitles.TrackBudget | [23:42](../YouTubeDirectSubtitles.cs#L23) |
| field | Emby.YouTubePlugin.YouTubeDirectSubtitles.InvocationGate | [24:47](../YouTubeDirectSubtitles.cs#L24) |
| field | Emby.YouTubePlugin.YouTubeDirectSubtitles.FetchGate | [25:47](../YouTubeDirectSubtitles.cs#L25) |
| field | Emby.YouTubePlugin.YouTubeDirectSubtitles.CacheGate | [26:47](../YouTubeDirectSubtitles.cs#L26) |
| field | Emby.YouTubePlugin.YouTubeDirectSubtitles.Cache | [27:64](../YouTubeDirectSubtitles.cs#L27) |
| field | Emby.YouTubePlugin.YouTubeDirectSubtitles.StrictUtf8 | [28:46](../YouTubeDirectSubtitles.cs#L28) |
| type | Emby.YouTubePlugin.YouTubeDirectSubtitles.CaptionCandidate | [30:9](../YouTubeDirectSubtitles.cs#L30) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.CaptionCandidate.Key | [31:13](../YouTubeDirectSubtitles.cs#L31) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.CaptionCandidate.Language | [31:25](../YouTubeDirectSubtitles.cs#L31) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.CaptionCandidate.Title | [31:42](../YouTubeDirectSubtitles.cs#L31) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.CaptionCandidate.Automatic | [31:56](../YouTubeDirectSubtitles.cs#L31) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.CaptionCandidate.Url | [32:13](../YouTubeDirectSubtitles.cs#L32) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.CaptionCandidate.Data | [32:23](../YouTubeDirectSubtitles.cs#L32) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.CaptionCandidate.Headers | [32:37](../YouTubeDirectSubtitles.cs#L32) |
| type | Emby.YouTubePlugin.YouTubeDirectSubtitles.CacheEntry | [33:9](../YouTubeDirectSubtitles.cs#L33) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.CacheEntry.Path | [33:42](../YouTubeDirectSubtitles.cs#L33) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.CacheEntry.CachedUtc | [33:55](../YouTubeDirectSubtitles.cs#L33) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.CacheEntry.LastUseUtc | [33:75](../YouTubeDirectSubtitles.cs#L33) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamsAsync | [35:9](../YouTubeDirectSubtitles.cs#L35) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamsAsync.resolved | [36:13](../YouTubeDirectSubtitles.cs#L36) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamsAsync.firstIndex | [36:44](../YouTubeDirectSubtitles.cs#L36) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamsAsync.cancellationToken | [36:60](../YouTubeDirectSubtitles.cs#L36) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamsAsync.candidates | [51:21](../YouTubeDirectSubtitles.cs#L51) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamsAsync.candidates.translated | [51:78](../YouTubeDirectSubtitles.cs#L51) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamsAsync.candidates.truncated | [51:98](../YouTubeDirectSubtitles.cs#L51) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamsAsync.indexedCandidates | [56:21](../YouTubeDirectSubtitles.cs#L56) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamsAsync.indexedCandidates.collisions | [56:87](../YouTubeDirectSubtitles.cs#L56) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamsAsync.budget | [60:27](../YouTubeDirectSubtitles.cs#L60) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamsAsync.pending | [65:21](../YouTubeDirectSubtitles.cs#L65) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamsAsync.pending.anonymous@3689 | [65:56](../YouTubeDirectSubtitles.cs#L65) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamsAsync.pending.anonymous@3689.entry | [65:56](../YouTubeDirectSubtitles.cs#L65) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamsAsync.streams | [67:21](../YouTubeDirectSubtitles.cs#L67) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamsAsync.result | [69:21](../YouTubeDirectSubtitles.cs#L69) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamsAsync.result.anonymous@3997 | [69:44](../YouTubeDirectSubtitles.cs#L69) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamsAsync.result.anonymous@3997.stream | [69:44](../YouTubeDirectSubtitles.cs#L69) |
| catch-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamsAsync.ex | [78:19](../YouTubeDirectSubtitles.cs#L78) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.TrackIndex | [91:9](../YouTubeDirectSubtitles.cs#L91) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.TrackIndex.key | [91:40](../YouTubeDirectSubtitles.cs#L91) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.TrackIndex.firstIndex | [91:52](../YouTubeDirectSubtitles.cs#L91) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.TrackIndex.minimum | [95:17](../YouTubeDirectSubtitles.cs#L95) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.TrackIndex.digest | [96:17](../YouTubeDirectSubtitles.cs#L96) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.TrackIndex.value | [97:17](../YouTubeDirectSubtitles.cs#L97) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.TrackIndex.range | [98:17](../YouTubeDirectSubtitles.cs#L98) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices | [102:9](../YouTubeDirectSubtitles.cs#L102) |
| tuple-element | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices.Candidate | [102:31](../YouTubeDirectSubtitles.cs#L102) |
| tuple-element | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices.Index | [102:59](../YouTubeDirectSubtitles.cs#L102) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices.candidates | [103:13](../YouTubeDirectSubtitles.cs#L103) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices.firstIndex | [103:57](../YouTubeDirectSubtitles.cs#L103) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices.collisionCount | [103:73](../YouTubeDirectSubtitles.cs#L103) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices.indexed | [105:17](../YouTubeDirectSubtitles.cs#L105) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices.indexed.anonymous@5834 | [105:45](../YouTubeDirectSubtitles.cs#L105) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices.indexed.anonymous@5834.candidate | [105:45](../YouTubeDirectSubtitles.cs#L105) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices.collisions | [107:17](../YouTubeDirectSubtitles.cs#L107) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices.collisions.anonymous@5989 | [107:46](../YouTubeDirectSubtitles.cs#L107) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices.collisions.anonymous@5989.entry | [107:46](../YouTubeDirectSubtitles.cs#L107) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices.collisions.anonymous@6017 | [107:74](../YouTubeDirectSubtitles.cs#L107) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices.collisions.anonymous@6017.group | [107:74](../YouTubeDirectSubtitles.cs#L107) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices.collisions.anonymous@6069 | [108:25](../YouTubeDirectSubtitles.cs#L108) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices.collisions.anonymous@6069.group | [108:25](../YouTubeDirectSubtitles.cs#L108) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices.anonymous@6145 | [109:44](../YouTubeDirectSubtitles.cs#L109) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices.anonymous@6145.entry | [109:44](../YouTubeDirectSubtitles.cs#L109) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices.anonymous@6222 | [110:34](../YouTubeDirectSubtitles.cs#L110) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.AssignIndices.anonymous@6222.entry | [110:34](../YouTubeDirectSubtitles.cs#L110) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates | [113:9](../YouTubeDirectSubtitles.cs#L113) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.metadata | [114:13](../YouTubeDirectSubtitles.cs#L114) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.translated | [114:35](../YouTubeDirectSubtitles.cs#L114) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.truncated | [114:55](../YouTubeDirectSubtitles.cs#L114) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.selected | [118:17](../YouTubeDirectSubtitles.cs#L118) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.commonHeaders | [121:17](../YouTubeDirectSubtitles.cs#L121) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.property | [122:27](../YouTubeDirectSubtitles.cs#L122) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.automatic | [122:37](../YouTubeDirectSubtitles.cs#L122) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.dictionary | [124:21](../YouTubeDirectSubtitles.cs#L124) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.languages | [129:21](../YouTubeDirectSubtitles.cs#L129) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.languages.anonymous@7316 | [129:70](../YouTubeDirectSubtitles.cs#L129) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.languages.anonymous@7316.p | [129:70](../YouTubeDirectSubtitles.cs#L129) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.language | [130:17](../YouTubeDirectSubtitles.cs#L130) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.language.isOriginalAlias | [134:25](../YouTubeDirectSubtitles.cs#L134) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.language.keyLanguage | [135:25](../YouTubeDirectSubtitles.cs#L135) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.language.key | [136:25](../YouTubeDirectSubtitles.cs#L136) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.language.candidate | [137:39](../YouTubeDirectSubtitles.cs#L137) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.language.isTranslation | [138:25](../YouTubeDirectSubtitles.cs#L138) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.language.format | [139:21](../YouTubeDirectSubtitles.cs#L139) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.language.format.uri | [145:34](../YouTubeDirectSubtitles.cs#L145) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.language.format.url | [146:33](../YouTubeDirectSubtitles.cs#L146) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.language.format.data | [147:33](../YouTubeDirectSubtitles.cs#L147) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.language.format.query | [148:33](../YouTubeDirectSubtitles.cs#L148) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.language.format.target | [156:68](../YouTubeDirectSubtitles.cs#L156) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.language.format.kind | [162:72](../YouTubeDirectSubtitles.cs#L162) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.language.format.sourceLanguage | [165:33](../YouTubeDirectSubtitles.cs#L165) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.language.format.sourceLanguage.lang | [165:84](../YouTubeDirectSubtitles.cs#L165) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.language.format.headers | [167:33](../YouTubeDirectSubtitles.cs#L167) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.language.format.header | [168:29](../YouTubeDirectSubtitles.cs#L168) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.language.format.display | [170:33](../YouTubeDirectSubtitles.cs#L170) |
| catch-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.language.format.ex | [177:31](../YouTubeDirectSubtitles.cs#L177) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.ordered | [188:17](../YouTubeDirectSubtitles.cs#L188) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.ordered.anonymous@11247 | [188:51](../YouTubeDirectSubtitles.cs#L188) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.ordered.anonymous@11247.c | [188:51](../YouTubeDirectSubtitles.cs#L188) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.ordered.anonymous@11312 | [189:25](../YouTubeDirectSubtitles.cs#L189) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.ordered.anonymous@11312.c | [189:25](../YouTubeDirectSubtitles.cs#L189) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.ordered.anonymous@11337 | [189:50](../YouTubeDirectSubtitles.cs#L189) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.SelectCandidates.ordered.anonymous@11337.c | [189:50](../YouTubeDirectSubtitles.cs#L189) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamAsync | [194:9](../YouTubeDirectSubtitles.cs#L194) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamAsync.videoId | [195:13](../YouTubeDirectSubtitles.cs#L195) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamAsync.candidate | [195:29](../YouTubeDirectSubtitles.cs#L195) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamAsync.index | [195:57](../YouTubeDirectSubtitles.cs#L195) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamAsync.cancellationToken | [195:68](../YouTubeDirectSubtitles.cs#L195) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamAsync.cacheKey | [201:21](../YouTubeDirectSubtitles.cs#L201) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamAsync.path | [202:21](../YouTubeDirectSubtitles.cs#L202) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamAsync.track | [213:39](../YouTubeDirectSubtitles.cs#L213) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamAsync.payload | [215:36](../YouTubeDirectSubtitles.cs#L215) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamAsync.normalized | [227:89](../YouTubeDirectSubtitles.cs#L227) |
| catch-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.GetStreamAsync.ex | [253:19](../YouTubeDirectSubtitles.cs#L253) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.FindCachedAsync | [261:9](../YouTubeDirectSubtitles.cs#L261) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.FindCachedAsync.key | [261:60](../YouTubeDirectSubtitles.cs#L261) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.FindCachedAsync.cancellationToken | [261:72](../YouTubeDirectSubtitles.cs#L261) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.FindCachedAsync.now | [266:21](../YouTubeDirectSubtitles.cs#L266) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.FindCachedAsync.entry | [267:53](../YouTubeDirectSubtitles.cs#L267) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.FindCachedAsync.info | [269:21](../YouTubeDirectSubtitles.cs#L269) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.StoreAsync | [286:9](../YouTubeDirectSubtitles.cs#L286) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.StoreAsync.videoId | [287:13](../YouTubeDirectSubtitles.cs#L287) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.StoreAsync.key | [287:29](../YouTubeDirectSubtitles.cs#L287) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.StoreAsync.payload | [287:41](../YouTubeDirectSubtitles.cs#L287) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.StoreAsync.cancellationToken | [287:57](../YouTubeDirectSubtitles.cs#L287) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.StoreAsync.temporaryPath | [290:21](../YouTubeDirectSubtitles.cs#L290) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.StoreAsync.directory | [295:21](../YouTubeDirectSubtitles.cs#L295) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.StoreAsync.now | [299:21](../YouTubeDirectSubtitles.cs#L299) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.StoreAsync.keyHash | [300:21](../YouTubeDirectSubtitles.cs#L300) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.StoreAsync.contentHash | [301:21](../YouTubeDirectSubtitles.cs#L301) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.StoreAsync.path | [302:21](../YouTubeDirectSubtitles.cs#L302) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.StoreAsync.existing | [305:25](../YouTubeDirectSubtitles.cs#L305) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.StoreAsync.output | [318:38](../YouTubeDirectSubtitles.cs#L318) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.StoreAsync.oldest | [330:25](../YouTubeDirectSubtitles.cs#L330) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSubtitles.StoreAsync.oldest.anonymous@18527 | [330:48](../YouTubeDirectSubtitles.cs#L330) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.StoreAsync.oldest.anonymous@18527.pair | [330:48](../YouTubeDirectSubtitles.cs#L330) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.HasCapacity | [348:9](../YouTubeDirectSubtitles.cs#L348) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.HasCapacity.directory | [348:41](../YouTubeDirectSubtitles.cs#L348) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.HasCapacity.incomingBytes | [348:59](../YouTubeDirectSubtitles.cs#L348) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.HasCapacity.now | [348:78](../YouTubeDirectSubtitles.cs#L348) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.HasCapacity.files | [352:17](../YouTubeDirectSubtitles.cs#L352) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.HasCapacity.activeVideos | [355:17](../YouTubeDirectSubtitles.cs#L355) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.HasCapacity.bytes | [356:18](../YouTubeDirectSubtitles.cs#L356) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.HasCapacity.count | [357:17](../YouTubeDirectSubtitles.cs#L357) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.HasCapacity.file | [358:13](../YouTubeDirectSubtitles.cs#L358) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.HasCapacity.file.videoId | [362:21](../YouTubeDirectSubtitles.cs#L362) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.TryGetActiveVideos | [380:9](../YouTubeDirectSubtitles.cs#L380) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.TryGetActiveVideos.manager | [384:21](../YouTubeDirectSubtitles.cs#L384) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.TryGetActiveVideos.result | [387:21](../YouTubeDirectSubtitles.cs#L387) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.TryGetActiveVideos.session | [388:17](../YouTubeDirectSubtitles.cs#L388) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.TryGetActiveVideos.session.item | [390:25](../YouTubeDirectSubtitles.cs#L390) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.TryGetActiveVideos.session.id | [397:25](../YouTubeDirectSubtitles.cs#L397) |
| catch-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.TryGetActiveVideos.ex | [403:19](../YouTubeDirectSubtitles.cs#L403) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.IsValidVtt | [409:9](../YouTubeDirectSubtitles.cs#L409) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.IsValidVtt.payload | [409:41](../YouTubeDirectSubtitles.cs#L409) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.CanonicalLanguage | [411:9](../YouTubeDirectSubtitles.cs#L411) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.CanonicalLanguage.language | [411:49](../YouTubeDirectSubtitles.cs#L411) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.CanonicalLanguage.known | [417:21](../YouTubeDirectSubtitles.cs#L417) |
| catch-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.CanonicalLanguage.ex | [421:19](../YouTubeDirectSubtitles.cs#L421) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.CanonicalLanguage.culture | [424:21](../YouTubeDirectSubtitles.cs#L424) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.CanonicalLanguage.iso | [425:21](../YouTubeDirectSubtitles.cs#L425) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.ReadQuery | [431:9](../YouTubeDirectSubtitles.cs#L431) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.ReadQuery.uri | [431:61](../YouTubeDirectSubtitles.cs#L431) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.ReadQuery.result | [433:17](../YouTubeDirectSubtitles.cs#L433) |
| iteration-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.ReadQuery.part | [434:13](../YouTubeDirectSubtitles.cs#L434) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.ReadQuery.part.separator | [436:21](../YouTubeDirectSubtitles.cs#L436) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.Property | [443:9](../YouTubeDirectSubtitles.cs#L443) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.Property.value | [443:45](../YouTubeDirectSubtitles.cs#L443) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.Property.name | [443:64](../YouTubeDirectSubtitles.cs#L443) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.Property.property | [444:91](../YouTubeDirectSubtitles.cs#L444) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.Text | [445:9](../YouTubeDirectSubtitles.cs#L445) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.Text.value | [445:37](../YouTubeDirectSubtitles.cs#L445) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.Text.name | [445:56](../YouTubeDirectSubtitles.cs#L445) |
| pattern-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.Text.property | [446:42](../YouTubeDirectSubtitles.cs#L446) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.CleanTitle | [447:9](../YouTubeDirectSubtitles.cs#L447) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.CleanTitle.value | [447:42](../YouTubeDirectSubtitles.cs#L447) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSubtitles.CleanTitle.anonymous@23601 | [448:29](../YouTubeDirectSubtitles.cs#L448) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.CleanTitle.anonymous@23601.character | [448:29](../YouTubeDirectSubtitles.cs#L448) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.Hash | [449:9](../YouTubeDirectSubtitles.cs#L449) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.Hash.value | [449:36](../YouTubeDirectSubtitles.cs#L449) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.IsLink | [450:9](../YouTubeDirectSubtitles.cs#L450) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.IsLink.info | [450:36](../YouTubeDirectSubtitles.cs#L450) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.IsVideoId | [451:9](../YouTubeDirectSubtitles.cs#L451) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.IsVideoId.value | [451:39](../YouTubeDirectSubtitles.cs#L451) |
| anonymous-function | Emby.YouTubePlugin.YouTubeDirectSubtitles.IsVideoId.anonymous@23992 | [452:26](../YouTubeDirectSubtitles.cs#L452) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.IsVideoId.anonymous@23992.character | [452:26](../YouTubeDirectSubtitles.cs#L452) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.OwnedFileVideoId | [454:9](../YouTubeDirectSubtitles.cs#L454) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.OwnedFileVideoId.name | [454:49](../YouTubeDirectSubtitles.cs#L454) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.OwnedFileVideoId.expectedLength | [456:23](../YouTubeDirectSubtitles.cs#L456) |
| local-variable | Emby.YouTubePlugin.YouTubeDirectSubtitles.OwnedFileVideoId.videoId | [460:17](../YouTubeDirectSubtitles.cs#L460) |
| method | Emby.YouTubePlugin.YouTubeDirectSubtitles.IsHex | [464:9](../YouTubeDirectSubtitles.cs#L464) |
| parameter | Emby.YouTubePlugin.YouTubeDirectSubtitles.IsHex.value | [464:35](../YouTubeDirectSubtitles.cs#L464) |

</details>

<a id="file-39b78aa97731c824"></a>
## [YouTubeEmbed.html](../YouTubeEmbed.html)

EmbeddedResource; 9942 bytes; 266 lines.

<details>
<summary>All 71 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| style-block | style@7 | [7:5](../YouTubeEmbed.html#L7) |
| dom-id | div.player | [18:5](../YouTubeEmbed.html#L18) |
| script-block | script@19 | [19:5](../YouTubeEmbed.html#L19) |
| variable | script@19.parentOrigin | [22:13](../YouTubeEmbed.html#L22) |
| variable | script@19.videoId | [23:13](../YouTubeEmbed.html#L23) |
| variable | script@19.player | [24:13](../YouTubeEmbed.html#L24) |
| variable | script@19.reportTimer | [25:13](../YouTubeEmbed.html#L25) |
| variable | script@19.captionGuardTimer | [26:13](../YouTubeEmbed.html#L26) |
| variable | script@19.captionRetryTimers | [27:13](../YouTubeEmbed.html#L27) |
| variable | script@19.captionForceBusy | [28:13](../YouTubeEmbed.html#L28) |
| variable | script@19.initialReadySent | [29:13](../YouTubeEmbed.html#L29) |
| variable | script@19.autoplayRetryUsed | [30:13](../YouTubeEmbed.html#L30) |
| variable | script@19.autoplayRecoveryReported | [31:13](../YouTubeEmbed.html#L31) |
| variable | script@19.disposed | [32:13](../YouTubeEmbed.html#L32) |
| variable | script@19.playerCreationStarted | [33:13](../YouTubeEmbed.html#L33) |
| function | script@19.disposePlayer | [35:9](../YouTubeEmbed.html#L35) |
| variable | script@19.disposePlayer.current | [38:17](../YouTubeEmbed.html#L38) |
| catch-variable | script@19.disposePlayer._ | [43:83](../YouTubeEmbed.html#L43) |
| catch-variable | script@19.disposePlayer._ | [44:79](../YouTubeEmbed.html#L44) |
| variable | script@19.disposePlayer.frames | [47:17](../YouTubeEmbed.html#L47) |
| variable | script@19.disposePlayer.i | [48:22](../YouTubeEmbed.html#L48) |
| catch-variable | script@19.disposePlayer._ | [49:63](../YouTubeEmbed.html#L49) |
| function | script@19.notify | [54:9](../YouTubeEmbed.html#L54) |
| parameter | script@19.notify.type | [54:25](../YouTubeEmbed.html#L54) |
| parameter | script@19.notify.data | [54:31](../YouTubeEmbed.html#L54) |
| function | script@19.callPlayer | [59:9](../YouTubeEmbed.html#L59) |
| parameter | script@19.callPlayer.name | [59:29](../YouTubeEmbed.html#L59) |
| parameter | script@19.callPlayer.fallback | [59:35](../YouTubeEmbed.html#L59) |
| catch-variable | script@19.callPlayer._ | [62:22](../YouTubeEmbed.html#L62) |
| function | script@19.makePlayerVars | [67:9](../YouTubeEmbed.html#L67) |
| function | script@19.forceCaptionsOff | [83:9](../YouTubeEmbed.html#L83) |
| parameter | script@19.forceCaptionsOff.target | [83:35](../YouTubeEmbed.html#L83) |
| variable | script@19.forceCaptionsOff.current | [85:17](../YouTubeEmbed.html#L85) |
| catch-variable | script@19.forceCaptionsOff._ | [93:26](../YouTubeEmbed.html#L93) |
| catch-variable | script@19.forceCaptionsOff._ | [98:26](../YouTubeEmbed.html#L98) |
| function | script@19.stopCaptionGuard | [104:9](../YouTubeEmbed.html#L104) |
| function | script@19.stopCaptionGuard.captionRetryTimers.forEach callback@3098 | [105:40](../YouTubeEmbed.html#L105) |
| parameter | script@19.stopCaptionGuard.captionRetryTimers.forEach callback@3098.timer | [105:50](../YouTubeEmbed.html#L105) |
| function | script@19.startCaptionGuard | [115:9](../YouTubeEmbed.html#L115) |
| parameter | script@19.startCaptionGuard.target | [115:36](../YouTubeEmbed.html#L115) |
| variable | script@19.startCaptionGuard.guardedPlayer | [118:17](../YouTubeEmbed.html#L118) |
| function | script@19.startCaptionGuard.[100, 500, 1500, 4000].forEach callback@3661 | [121:44](../YouTubeEmbed.html#L121) |
| parameter | script@19.startCaptionGuard.[100, 500, 1500, 4000].forEach callback@3661.delay | [121:54](../YouTubeEmbed.html#L121) |
| function | script@19.startCaptionGuard.[100, 500, 1500, 4000].forEach callback@3661.window.setTimeout callback@3738 | [122:59](../YouTubeEmbed.html#L122) |
| function | script@19.startCaptionGuard.window.setInterval callback@3930 | [126:52](../YouTubeEmbed.html#L126) |
| function | script@19.startStatusTimer | [131:9](../YouTubeEmbed.html#L131) |
| function | script@19.startStatusTimer.window.setInterval callback@4176 | [133:46](../YouTubeEmbed.html#L133) |
| function | script@19.onPlayerReady | [145:9](../YouTubeEmbed.html#L145) |
| parameter | script@19.onPlayerReady.event | [145:32](../YouTubeEmbed.html#L145) |
| catch-variable | script@19.onPlayerReady._ | [147:56](../YouTubeEmbed.html#L147) |
| function | script@19.onAutoplayBlocked | [159:9](../YouTubeEmbed.html#L159) |
| catch-variable | script@19.onAutoplayBlocked._ | [172:22](../YouTubeEmbed.html#L172) |
| function | script@19.createPlayer | [177:9](../YouTubeEmbed.html#L177) |
| function | script@19.createPlayer.onApiChange | [189:34](../YouTubeEmbed.html#L189) |
| parameter | script@19.createPlayer.onApiChange.event | [189:44](../YouTubeEmbed.html#L189) |
| function | script@19.createPlayer.onStateChange | [192:36](../YouTubeEmbed.html#L192) |
| parameter | script@19.createPlayer.onStateChange.event | [192:46](../YouTubeEmbed.html#L192) |
| function | script@19.createPlayer.onPlaybackQualityChange | [207:46](../YouTubeEmbed.html#L207) |
| parameter | script@19.createPlayer.onPlaybackQualityChange.event | [207:56](../YouTubeEmbed.html#L207) |
| function | script@19.createPlayer.onPlaybackRateChange | [210:43](../YouTubeEmbed.html#L210) |
| parameter | script@19.createPlayer.onPlaybackRateChange.event | [210:53](../YouTubeEmbed.html#L210) |
| function | script@19.createPlayer.onError | [213:30](../YouTubeEmbed.html#L213) |
| parameter | script@19.createPlayer.onError.event | [213:40](../YouTubeEmbed.html#L213) |
| function | script@19.window.onYouTubeIframeAPIReady | [221:42](../YouTubeEmbed.html#L221) |
| function | script@19.window.addEventListener callback@7954 | [230:44](../YouTubeEmbed.html#L230) |
| parameter | script@19.window.addEventListener callback@7954.event | [230:54](../YouTubeEmbed.html#L230) |
| variable | script@19.window.addEventListener callback@7954.message | [233:17](../YouTubeEmbed.html#L233) |
| variable | script@19.window.addEventListener callback@7954.args | [235:17](../YouTubeEmbed.html#L235) |
| variable | script@19.window.addEventListener callback@7954.isCaptionPolicyCommand | [237:21](../YouTubeEmbed.html#L237) |
| catch-variable | script@19.window.addEventListener callback@7954._ | [255:22](../YouTubeEmbed.html#L255) |
| variable | script@19.apiScript | [261:13](../YouTubeEmbed.html#L261) |

</details>

<a id="file-973493e8a800cabd"></a>
## [YouTubeEpisodeMetadataProvider.cs](../YouTubeEpisodeMetadataProvider.cs)

Plugin Compile source (SDK default); 2315 bytes; 61 lines.

<details>
<summary>All 16 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [11:1](../YouTubeEpisodeMetadataProvider.cs#L11) |
| type | Emby.YouTubePlugin.YouTubeEpisodeMetadataProvider | [13:5](../YouTubeEpisodeMetadataProvider.cs#L13) |
| property | Emby.YouTubePlugin.YouTubeEpisodeMetadataProvider.Name | [15:9](../YouTubeEpisodeMetadataProvider.cs#L15) |
| method | Emby.YouTubePlugin.YouTubeEpisodeMetadataProvider.FetchAsync | [17:9](../YouTubeEpisodeMetadataProvider.cs#L17) |
| parameter | Emby.YouTubePlugin.YouTubeEpisodeMetadataProvider.FetchAsync.itemResult | [18:13](../YouTubeEpisodeMetadataProvider.cs#L18) |
| parameter | Emby.YouTubePlugin.YouTubeEpisodeMetadataProvider.FetchAsync.options | [19:13](../YouTubeEpisodeMetadataProvider.cs#L19) |
| parameter | Emby.YouTubePlugin.YouTubeEpisodeMetadataProvider.FetchAsync.libraryOptions | [20:13](../YouTubeEpisodeMetadataProvider.cs#L20) |
| parameter | Emby.YouTubePlugin.YouTubeEpisodeMetadataProvider.FetchAsync.cancellationToken | [21:13](../YouTubeEpisodeMetadataProvider.cs#L21) |
| local-variable | Emby.YouTubePlugin.YouTubeEpisodeMetadataProvider.FetchAsync.baseItem | [25:17](../YouTubeEpisodeMetadataProvider.cs#L25) |
| local-variable | Emby.YouTubePlugin.YouTubeEpisodeMetadataProvider.FetchAsync.resultItem | [26:17](../YouTubeEpisodeMetadataProvider.cs#L26) |
| local-variable | Emby.YouTubePlugin.YouTubeEpisodeMetadataProvider.FetchAsync.videoId | [27:17](../YouTubeEpisodeMetadataProvider.cs#L27) |
| local-variable | Emby.YouTubePlugin.YouTubeEpisodeMetadataProvider.FetchAsync.url | [36:17](../YouTubeEpisodeMetadataProvider.cs#L36) |
| method | Emby.YouTubePlugin.YouTubeEpisodeMetadataProvider.ShouldRestoreImage | [51:9](../YouTubeEpisodeMetadataProvider.cs#L51) |
| parameter | Emby.YouTubePlugin.YouTubeEpisodeMetadataProvider.ShouldRestoreImage.baseItem | [51:48](../YouTubeEpisodeMetadataProvider.cs#L51) |
| parameter | Emby.YouTubePlugin.YouTubeEpisodeMetadataProvider.ShouldRestoreImage.resultItem | [51:68](../YouTubeEpisodeMetadataProvider.cs#L51) |
| parameter | Emby.YouTubePlugin.YouTubeEpisodeMetadataProvider.ShouldRestoreImage.options | [51:90](../YouTubeEpisodeMetadataProvider.cs#L51) |

</details>

<a id="file-0fc93e92e0c0f80b"></a>
## [YouTubeExternalId.cs](../YouTubeExternalId.cs)

Plugin Compile source (SDK default); 470 bytes; 19 lines.

<details>
<summary>All 7 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [4:1](../YouTubeExternalId.cs#L4) |
| type | Emby.YouTubePlugin.YouTubeExternalId | [6:5](../YouTubeExternalId.cs#L6) |
| property | Emby.YouTubePlugin.YouTubeExternalId.Name | [8:9](../YouTubeExternalId.cs#L8) |
| property | Emby.YouTubePlugin.YouTubeExternalId.Key | [10:9](../YouTubeExternalId.cs#L10) |
| property | Emby.YouTubePlugin.YouTubeExternalId.UrlFormatString | [12:9](../YouTubeExternalId.cs#L12) |
| method | Emby.YouTubePlugin.YouTubeExternalId.Supports | [14:9](../YouTubeExternalId.cs#L14) |
| parameter | Emby.YouTubePlugin.YouTubeExternalId.Supports.item | [14:30](../YouTubeExternalId.cs#L14) |

</details>

<a id="file-2bdd885ca109f825"></a>
## [YouTubeHomeSectionManager.cs](../YouTubeHomeSectionManager.cs)

Plugin Compile source (SDK default); 62349 bytes; 1451 lines.

<details>
<summary>All 415 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [19:1](../YouTubeHomeSectionManager.cs#L19) |
| type | Emby.YouTubePlugin.YouTubeHomeSectionManager | [26:5](../YouTubeHomeSectionManager.cs#L26) |
| field | Emby.YouTubePlugin.YouTubeHomeSectionManager.ManagedIdPrefix | [31:30](../YouTubeHomeSectionManager.cs#L31) |
| field | Emby.YouTubePlugin.YouTubeHomeSectionManager.AggregateBackupDirectoryName | [33:30](../YouTubeHomeSectionManager.cs#L33) |
| field | Emby.YouTubePlugin.YouTubeHomeSectionManager.LatestMediaBlockFilterDirectoryName | [34:30](../YouTubeHomeSectionManager.cs#L34) |
| field | Emby.YouTubePlugin.YouTubeHomeSectionManager.LegacyAggregateName | [36:30](../YouTubeHomeSectionManager.cs#L36) |
| field | Emby.YouTubePlugin.YouTubeHomeSectionManager.LatestMediaBlockSectionType | [37:30](../YouTubeHomeSectionManager.cs#L37) |
| field | Emby.YouTubePlugin.YouTubeHomeSectionManager.BackupJsonOptions | [38:55](../YouTubeHomeSectionManager.cs#L38) |
| field | Emby.YouTubePlugin.YouTubeHomeSectionManager._userManager | [44:39](../YouTubeHomeSectionManager.cs#L44) |
| field | Emby.YouTubePlugin.YouTubeHomeSectionManager._userViewManager | [45:43](../YouTubeHomeSectionManager.cs#L45) |
| field | Emby.YouTubePlugin.YouTubeHomeSectionManager._addHomeSection | [46:38](../YouTubeHomeSectionManager.cs#L46) |
| field | Emby.YouTubePlugin.YouTubeHomeSectionManager._updateHomeSection | [47:38](../YouTubeHomeSectionManager.cs#L47) |
| field | Emby.YouTubePlugin.YouTubeHomeSectionManager._deleteHomeSections | [48:38](../YouTubeHomeSectionManager.cs#L48) |
| field | Emby.YouTubePlugin.YouTubeHomeSectionManager._moveHomeSections | [49:38](../YouTubeHomeSectionManager.cs#L49) |
| field | Emby.YouTubePlugin.YouTubeHomeSectionManager._syncGate | [50:40](../YouTubeHomeSectionManager.cs#L50) |
| field | Emby.YouTubePlugin.YouTubeHomeSectionManager._unsupportedLogged | [51:21](../YouTubeHomeSectionManager.cs#L51) |
| type | Emby.YouTubePlugin.YouTubeHomeSectionManager.AggregateBackup | [53:9](../YouTubeHomeSectionManager.cs#L53) |
| property | Emby.YouTubePlugin.YouTubeHomeSectionManager.AggregateBackup.SectionId | [55:13](../YouTubeHomeSectionManager.cs#L55) |
| accessor | Emby.YouTubePlugin.YouTubeHomeSectionManager.AggregateBackup.SectionId.get | [55:39](../YouTubeHomeSectionManager.cs#L55) |
| accessor | Emby.YouTubePlugin.YouTubeHomeSectionManager.AggregateBackup.SectionId.set | [55:44](../YouTubeHomeSectionManager.cs#L55) |
| property | Emby.YouTubePlugin.YouTubeHomeSectionManager.AggregateBackup.SectionJson | [56:13](../YouTubeHomeSectionManager.cs#L56) |
| accessor | Emby.YouTubePlugin.YouTubeHomeSectionManager.AggregateBackup.SectionJson.get | [56:41](../YouTubeHomeSectionManager.cs#L56) |
| accessor | Emby.YouTubePlugin.YouTubeHomeSectionManager.AggregateBackup.SectionJson.set | [56:46](../YouTubeHomeSectionManager.cs#L56) |
| property | Emby.YouTubePlugin.YouTubeHomeSectionManager.AggregateBackup.OriginalIndex | [57:13](../YouTubeHomeSectionManager.cs#L57) |
| accessor | Emby.YouTubePlugin.YouTubeHomeSectionManager.AggregateBackup.OriginalIndex.get | [57:40](../YouTubeHomeSectionManager.cs#L57) |
| accessor | Emby.YouTubePlugin.YouTubeHomeSectionManager.AggregateBackup.OriginalIndex.set | [57:45](../YouTubeHomeSectionManager.cs#L57) |
| property | Emby.YouTubePlugin.YouTubeHomeSectionManager.AggregateBackup.SavedAtUtc | [58:13](../YouTubeHomeSectionManager.cs#L58) |
| accessor | Emby.YouTubePlugin.YouTubeHomeSectionManager.AggregateBackup.SavedAtUtc.get | [58:42](../YouTubeHomeSectionManager.cs#L58) |
| accessor | Emby.YouTubePlugin.YouTubeHomeSectionManager.AggregateBackup.SavedAtUtc.set | [58:47](../YouTubeHomeSectionManager.cs#L58) |
| type | Emby.YouTubePlugin.YouTubeHomeSectionManager.LatestMediaBlockFilterState | [61:9](../YouTubeHomeSectionManager.cs#L61) |
| property | Emby.YouTubePlugin.YouTubeHomeSectionManager.LatestMediaBlockFilterState.Version | [63:13](../YouTubeHomeSectionManager.cs#L63) |
| accessor | Emby.YouTubePlugin.YouTubeHomeSectionManager.LatestMediaBlockFilterState.Version.get | [63:34](../YouTubeHomeSectionManager.cs#L63) |
| accessor | Emby.YouTubePlugin.YouTubeHomeSectionManager.LatestMediaBlockFilterState.Version.set | [63:39](../YouTubeHomeSectionManager.cs#L63) |
| property | Emby.YouTubePlugin.YouTubeHomeSectionManager.LatestMediaBlockFilterState.Entries | [64:13](../YouTubeHomeSectionManager.cs#L64) |
| accessor | Emby.YouTubePlugin.YouTubeHomeSectionManager.LatestMediaBlockFilterState.Entries.get | [64:64](../YouTubeHomeSectionManager.cs#L64) |
| accessor | Emby.YouTubePlugin.YouTubeHomeSectionManager.LatestMediaBlockFilterState.Entries.set | [64:69](../YouTubeHomeSectionManager.cs#L64) |
| type | Emby.YouTubePlugin.YouTubeHomeSectionManager.LatestMediaBlockFilterEntry | [67:9](../YouTubeHomeSectionManager.cs#L67) |
| property | Emby.YouTubePlugin.YouTubeHomeSectionManager.LatestMediaBlockFilterEntry.SectionId | [69:13](../YouTubeHomeSectionManager.cs#L69) |
| accessor | Emby.YouTubePlugin.YouTubeHomeSectionManager.LatestMediaBlockFilterEntry.SectionId.get | [69:39](../YouTubeHomeSectionManager.cs#L69) |
| accessor | Emby.YouTubePlugin.YouTubeHomeSectionManager.LatestMediaBlockFilterEntry.SectionId.set | [69:44](../YouTubeHomeSectionManager.cs#L69) |
| property | Emby.YouTubePlugin.YouTubeHomeSectionManager.LatestMediaBlockFilterEntry.AddedExcludedFolderIds | [70:13](../YouTubeHomeSectionManager.cs#L70) |
| accessor | Emby.YouTubePlugin.YouTubeHomeSectionManager.LatestMediaBlockFilterEntry.AddedExcludedFolderIds.get | [70:58](../YouTubeHomeSectionManager.cs#L70) |
| accessor | Emby.YouTubePlugin.YouTubeHomeSectionManager.LatestMediaBlockFilterEntry.AddedExcludedFolderIds.set | [70:63](../YouTubeHomeSectionManager.cs#L70) |
| property | Emby.YouTubePlugin.YouTubeHomeSectionManager.LatestMediaBlockFilterEntry.SavedAtUtc | [71:13](../YouTubeHomeSectionManager.cs#L71) |
| accessor | Emby.YouTubePlugin.YouTubeHomeSectionManager.LatestMediaBlockFilterEntry.SavedAtUtc.get | [71:42](../YouTubeHomeSectionManager.cs#L71) |
| accessor | Emby.YouTubePlugin.YouTubeHomeSectionManager.LatestMediaBlockFilterEntry.SavedAtUtc.set | [71:47](../YouTubeHomeSectionManager.cs#L71) |
| type | Emby.YouTubePlugin.YouTubeHomeSectionManager.AggregateFolderContext | [74:9](../YouTubeHomeSectionManager.cs#L74) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.AggregateFolderContext.AllViewIds | [75:13](../YouTubeHomeSectionManager.cs#L75) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.AggregateFolderContext.YouTubeRootIds | [76:13](../YouTubeHomeSectionManager.cs#L76) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.AggregateFolderContext.YouTubeViewId | [77:13](../YouTubeHomeSectionManager.cs#L77) |
| constructor | Emby.YouTubePlugin.YouTubeHomeSectionManager.YouTubeHomeSectionManager | [79:9](../YouTubeHomeSectionManager.cs#L79) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.YouTubeHomeSectionManager.userManager | [80:13](../YouTubeHomeSectionManager.cs#L80) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.YouTubeHomeSectionManager.userViewManager | [81:13](../YouTubeHomeSectionManager.cs#L81) |
| property | Emby.YouTubePlugin.YouTubeHomeSectionManager.SupportsHomeSectionWrites | [91:9](../YouTubeHomeSectionManager.cs#L91) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncAsync | [96:9](../YouTubeHomeSectionManager.cs#L96) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncAsync.cancellationToken | [96:37](../YouTubeHomeSectionManager.cs#L96) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncAsync.config | [113:21](../YouTubeHomeSectionManager.cs#L113) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncAsync.enabled | [117:21](../YouTubeHomeSectionManager.cs#L117) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncAsync.hasConfiguredChannels | [118:21](../YouTubeHomeSectionManager.cs#L118) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncAsync.users | [119:21](../YouTubeHomeSectionManager.cs#L119) |
| iteration-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncAsync.user | [121:17](../YouTubeHomeSectionManager.cs#L121) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncAsync.user.userEnabled | [126:29](../YouTubeHomeSectionManager.cs#L126) |
| catch-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncAsync.user.ex | [139:27](../YouTubeHomeSectionManager.cs#L139) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync | [152:9](../YouTubeHomeSectionManager.cs#L152) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.user | [153:13](../YouTubeHomeSectionManager.cs#L153) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.globallyEnabled | [154:13](../YouTubeHomeSectionManager.cs#L154) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.enabled | [155:13](../YouTubeHomeSectionManager.cs#L155) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.hasConfiguredChannels | [156:13](../YouTubeHomeSectionManager.cs#L156) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.cancellationToken | [157:13](../YouTubeHomeSectionManager.cs#L157) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.homeSections | [159:17](../YouTubeHomeSectionManager.cs#L159) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.current | [160:17](../YouTubeHomeSectionManager.cs#L160) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.roots | [173:36](../YouTubeHomeSectionManager.cs#L173) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.aggregateContext | [174:37](../YouTubeHomeSectionManager.cs#L174) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.desired | [193:17](../YouTubeHomeSectionManager.cs#L193) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.desired.anonymous@8126 | [195:26](../YouTubeHomeSectionManager.cs#L195) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.desired.anonymous@8126.section | [195:26](../YouTubeHomeSectionManager.cs#L195) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.desired.anonymous@8207 | [196:25](../YouTubeHomeSectionManager.cs#L196) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.desired.anonymous@8207.group | [196:25](../YouTubeHomeSectionManager.cs#L196) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.desired.anonymous@8261 | [197:31](../YouTubeHomeSectionManager.cs#L197) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.desired.anonymous@8261.section | [197:31](../YouTubeHomeSectionManager.cs#L197) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.managedGroups | [198:17](../YouTubeHomeSectionManager.cs#L198) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.managedGroups.anonymous@8418 | [200:26](../YouTubeHomeSectionManager.cs#L200) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.managedGroups.anonymous@8418.section | [200:26](../YouTubeHomeSectionManager.cs#L200) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.duplicateDesiredGroups | [202:17](../YouTubeHomeSectionManager.cs#L202) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.duplicateDesiredGroups.anonymous@8581 | [203:24](../YouTubeHomeSectionManager.cs#L203) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.duplicateDesiredGroups.anonymous@8581.group | [203:24](../YouTubeHomeSectionManager.cs#L203) |
| iteration-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.group | [210:13](../YouTubeHomeSectionManager.cs#L210) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.duplicateDesiredIds | [220:17](../YouTubeHomeSectionManager.cs#L220) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.duplicateDesiredIds.anonymous@9423 | [221:25](../YouTubeHomeSectionManager.cs#L221) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.duplicateDesiredIds.anonymous@9423.group | [221:25](../YouTubeHomeSectionManager.cs#L221) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.managed | [223:17](../YouTubeHomeSectionManager.cs#L223) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.managed.anonymous@9568 | [224:24](../YouTubeHomeSectionManager.cs#L224) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.managed.anonymous@9568.group | [224:24](../YouTubeHomeSectionManager.cs#L224) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.managed.anonymous@9670 | [226:21](../YouTubeHomeSectionManager.cs#L226) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.managed.anonymous@9670.group | [226:21](../YouTubeHomeSectionManager.cs#L226) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.managed.anonymous@9710 | [227:21](../YouTubeHomeSectionManager.cs#L227) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.managed.anonymous@9710.group | [227:21](../YouTubeHomeSectionManager.cs#L227) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.added | [230:17](../YouTubeHomeSectionManager.cs#L230) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.updated | [231:17](../YouTubeHomeSectionManager.cs#L231) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.removed | [232:17](../YouTubeHomeSectionManager.cs#L232) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.removed.anonymous@9899 | [232:54](../YouTubeHomeSectionManager.cs#L232) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.removed.anonymous@9899.group | [232:54](../YouTubeHomeSectionManager.cs#L232) |
| iteration-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.pair | [233:13](../YouTubeHomeSectionManager.cs#L233) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.pair.anonymous@9973 | [233:50](../YouTubeHomeSectionManager.cs#L233) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.pair.anonymous@9973.pair | [233:50](../YouTubeHomeSectionManager.cs#L233) |
| pattern-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.pair.existing | [236:60](../YouTubeHomeSectionManager.cs#L236) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.pair.update | [248:25](../YouTubeHomeSectionManager.cs#L248) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.staleIds | [262:17](../YouTubeHomeSectionManager.cs#L262) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.staleIds.anonymous@11309 | [263:24](../YouTubeHomeSectionManager.cs#L263) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.staleIds.anonymous@11309.id | [263:24](../YouTubeHomeSectionManager.cs#L263) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.staleIds.anonymous@11366 | [264:26](../YouTubeHomeSectionManager.cs#L264) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.staleIds.anonymous@11366.id | [264:26](../YouTubeHomeSectionManager.cs#L264) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.anonymous@11809 | [276:28](../YouTubeHomeSectionManager.cs#L276) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.anonymous@11809.group | [276:28](../YouTubeHomeSectionManager.cs#L276) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.anonymous@11907 | [277:26](../YouTubeHomeSectionManager.cs#L277) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.anonymous@11907.group | [277:26](../YouTubeHomeSectionManager.cs#L277) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.aggregateSuppressed | [280:17](../YouTubeHomeSectionManager.cs#L280) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.aggregateRestored | [281:17](../YouTubeHomeSectionManager.cs#L281) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.latestMediaBlockFiltered | [282:17](../YouTubeHomeSectionManager.cs#L282) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SyncUserAsync.latestMediaBlockRestored | [283:17](../YouTubeHomeSectionManager.cs#L283) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots | [334:9](../YouTubeHomeSectionManager.cs#L334) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.user | [335:13](../YouTubeHomeSectionManager.cs#L335) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.hasConfiguredChannels | [336:13](../YouTubeHomeSectionManager.cs#L336) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.aggregateContext | [337:13](../YouTubeHomeSectionManager.cs#L337) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.topLevelViews | [344:17](../YouTubeHomeSectionManager.cs#L344) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.youtubeCandidates | [345:17](../YouTubeHomeSectionManager.cs#L345) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.youtubeCandidates.anonymous@15035 | [347:24](../YouTubeHomeSectionManager.cs#L347) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.youtubeCandidates.anonymous@15035.view | [347:24](../YouTubeHomeSectionManager.cs#L347) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.dynamicViews | [358:17](../YouTubeHomeSectionManager.cs#L358) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.youtubeView | [359:17](../YouTubeHomeSectionManager.cs#L359) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.youtubeView.anonymous@15621 | [359:64](../YouTubeHomeSectionManager.cs#L359) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.youtubeView.anonymous@15621.candidate | [359:64](../YouTubeHomeSectionManager.cs#L359) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.youtubeView.anonymous@15621.anonymous@15671 | [360:38](../YouTubeHomeSectionManager.cs#L360) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.youtubeView.anonymous@15621.anonymous@15671.view | [360:38](../YouTubeHomeSectionManager.cs#L360) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.aggregateViews | [369:17](../YouTubeHomeSectionManager.cs#L369) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.allViewIds | [373:17](../YouTubeHomeSectionManager.cs#L373) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.allViewIds.anonymous@16418 | [374:25](../YouTubeHomeSectionManager.cs#L374) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.allViewIds.anonymous@16418.view | [374:25](../YouTubeHomeSectionManager.cs#L374) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.allViewIds.anonymous@16469 | [375:24](../YouTubeHomeSectionManager.cs#L375) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.allViewIds.anonymous@16469.id | [375:24](../YouTubeHomeSectionManager.cs#L375) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.youtubeRootIds | [377:17](../YouTubeHomeSectionManager.cs#L377) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.youtubeRootIds.anonymous@16640 | [378:24](../YouTubeHomeSectionManager.cs#L378) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.youtubeRootIds.anonymous@16640.view | [378:24](../YouTubeHomeSectionManager.cs#L378) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.youtubeRootIds.anonymous@16713 | [379:25](../YouTubeHomeSectionManager.cs#L379) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.youtubeRootIds.anonymous@16713.view | [379:25](../YouTubeHomeSectionManager.cs#L379) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.youtubeRootIds.anonymous@16764 | [380:24](../YouTubeHomeSectionManager.cs#L380) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.youtubeRootIds.anonymous@16764.id | [380:24](../YouTubeHomeSectionManager.cs#L380) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.roots | [387:17](../YouTubeHomeSectionManager.cs#L387) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.roots.anonymous@17088 | [388:24](../YouTubeHomeSectionManager.cs#L388) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.roots.anonymous@17088.view | [388:24](../YouTubeHomeSectionManager.cs#L388) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.roots.anonymous@17266 | [391:26](../YouTubeHomeSectionManager.cs#L391) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.roots.anonymous@17266.view | [391:26](../YouTubeHomeSectionManager.cs#L391) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.roots.anonymous@17339 | [392:25](../YouTubeHomeSectionManager.cs#L392) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.roots.anonymous@17339.group | [392:25](../YouTubeHomeSectionManager.cs#L392) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.roots.anonymous@17388 | [393:26](../YouTubeHomeSectionManager.cs#L393) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.roots.anonymous@17388.view | [393:26](../YouTubeHomeSectionManager.cs#L393) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.roots.anonymous@17465 | [394:25](../YouTubeHomeSectionManager.cs#L394) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetSavedChannelRoots.roots.anonymous@17465.view | [394:25](../YouTubeHomeSectionManager.cs#L394) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetUserViews | [403:9](../YouTubeHomeSectionManager.cs#L403) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetUserViews.userId | [404:13](../YouTubeHomeSectionManager.cs#L404) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetUserViews.allowDynamicChildren | [405:13](../YouTubeHomeSectionManager.cs#L405) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetUserViews.includeLiveTvView | [406:13](../YouTubeHomeSectionManager.cs#L406) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.SuppressLegacyAggregateAsync | [416:9](../YouTubeHomeSectionManager.cs#L416) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SuppressLegacyAggregateAsync.user | [417:13](../YouTubeHomeSectionManager.cs#L417) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SuppressLegacyAggregateAsync.context | [418:13](../YouTubeHomeSectionManager.cs#L418) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SuppressLegacyAggregateAsync.desired | [419:13](../YouTubeHomeSectionManager.cs#L419) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SuppressLegacyAggregateAsync.cancellationToken | [420:13](../YouTubeHomeSectionManager.cs#L420) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SuppressLegacyAggregateAsync.current | [425:17](../YouTubeHomeSectionManager.cs#L425) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SuppressLegacyAggregateAsync.matches | [434:17](../YouTubeHomeSectionManager.cs#L434) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.SuppressLegacyAggregateAsync.matches.anonymous@19119 | [435:25](../YouTubeHomeSectionManager.cs#L435) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SuppressLegacyAggregateAsync.matches.anonymous@19119.section | [435:26](../YouTubeHomeSectionManager.cs#L435) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SuppressLegacyAggregateAsync.matches.anonymous@19119.index | [435:35](../YouTubeHomeSectionManager.cs#L435) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.SuppressLegacyAggregateAsync.matches.anonymous@19196 | [436:24](../YouTubeHomeSectionManager.cs#L436) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SuppressLegacyAggregateAsync.matches.anonymous@19196.match | [436:24](../YouTubeHomeSectionManager.cs#L436) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SuppressLegacyAggregateAsync.candidate | [451:17](../YouTubeHomeSectionManager.cs#L451) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SuppressLegacyAggregateAsync.duplicateIdCount | [452:17](../YouTubeHomeSectionManager.cs#L452) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.SuppressLegacyAggregateAsync.duplicateIdCount.anonymous@19823 | [452:50](../YouTubeHomeSectionManager.cs#L452) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SuppressLegacyAggregateAsync.duplicateIdCount.anonymous@19823.section | [452:50](../YouTubeHomeSectionManager.cs#L452) |
| pattern-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SuppressLegacyAggregateAsync.existingBackup | [464:55](../YouTubeHomeSectionManager.cs#L464) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.SuppressLegacyAggregateAsync.anonymous@21872 | [500:65](../YouTubeHomeSectionManager.cs#L500) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SuppressLegacyAggregateAsync.anonymous@21872.section | [500:65](../YouTubeHomeSectionManager.cs#L500) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.AreDesiredRowsReady | [517:9](../YouTubeHomeSectionManager.cs#L517) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.AreDesiredRowsReady.current | [518:13](../YouTubeHomeSectionManager.cs#L518) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.AreDesiredRowsReady.desired | [519:13](../YouTubeHomeSectionManager.cs#L519) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.AreDesiredRowsReady.sections | [521:17](../YouTubeHomeSectionManager.cs#L521) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.AreDesiredRowsReady.anonymous@22796 | [522:32](../YouTubeHomeSectionManager.cs#L522) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.AreDesiredRowsReady.anonymous@22796.pair | [522:32](../YouTubeHomeSectionManager.cs#L522) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.AreDesiredRowsReady.anonymous@22796.live | [524:21](../YouTubeHomeSectionManager.cs#L524) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.AreDesiredRowsReady.anonymous@22796.live.anonymous@22881 | [525:28](../YouTubeHomeSectionManager.cs#L525) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.AreDesiredRowsReady.anonymous@22796.live.anonymous@22881.section | [525:28](../YouTubeHomeSectionManager.cs#L525) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync | [534:9](../YouTubeHomeSectionManager.cs#L534) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.user | [535:13](../YouTubeHomeSectionManager.cs#L535) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.cancellationToken | [536:13](../YouTubeHomeSectionManager.cs#L536) |
| pattern-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.backup | [538:55](../YouTubeHomeSectionManager.cs#L538) |
| pattern-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.section | [538:71](../YouTubeHomeSectionManager.cs#L538) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.current | [543:17](../YouTubeHomeSectionManager.cs#L543) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.sameId | [544:17](../YouTubeHomeSectionManager.cs#L544) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.sameId.anonymous@23604 | [544:40](../YouTubeHomeSectionManager.cs#L544) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.sameId.anonymous@23604.section | [544:40](../YouTubeHomeSectionManager.cs#L544) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.restoredSections | [584:17](../YouTubeHomeSectionManager.cs#L584) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.restored | [585:17](../YouTubeHomeSectionManager.cs#L585) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.restored.anonymous@25385 | [586:25](../YouTubeHomeSectionManager.cs#L586) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.restored.anonymous@25385.candidate | [586:26](../YouTubeHomeSectionManager.cs#L586) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.restored.anonymous@25385.index | [586:37](../YouTubeHomeSectionManager.cs#L586) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.restored.anonymous@25466 | [587:24](../YouTubeHomeSectionManager.cs#L587) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.restored.anonymous@25466.candidate | [587:24](../YouTubeHomeSectionManager.cs#L587) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.restoredIndex | [598:17](../YouTubeHomeSectionManager.cs#L598) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.targetIndex | [599:17](../YouTubeHomeSectionManager.cs#L599) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.moved | [608:21](../YouTubeHomeSectionManager.cs#L608) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.moved.anonymous@26517 | [609:29](../YouTubeHomeSectionManager.cs#L609) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.moved.anonymous@26517.candidate | [609:30](../YouTubeHomeSectionManager.cs#L609) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.moved.anonymous@26517.index | [609:41](../YouTubeHomeSectionManager.cs#L609) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.moved.anonymous@26602 | [610:28](../YouTubeHomeSectionManager.cs#L610) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLegacyAggregateAsync.moved.anonymous@26602.candidate | [610:28](../YouTubeHomeSectionManager.cs#L610) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync | [632:9](../YouTubeHomeSectionManager.cs#L632) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.user | [633:13](../YouTubeHomeSectionManager.cs#L633) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.context | [634:13](../YouTubeHomeSectionManager.cs#L634) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.desired | [635:13](../YouTubeHomeSectionManager.cs#L635) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.cancellationToken | [636:13](../YouTubeHomeSectionManager.cs#L636) |
| pattern-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.state | [639:70](../YouTubeHomeSectionManager.cs#L639) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.current | [647:17](../YouTubeHomeSectionManager.cs#L647) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.withoutId | [656:17](../YouTubeHomeSectionManager.cs#L656) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.withoutId.anonymous@28688 | [656:43](../YouTubeHomeSectionManager.cs#L656) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.withoutId.anonymous@28688.section | [656:43](../YouTubeHomeSectionManager.cs#L656) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.filtered | [665:17](../YouTubeHomeSectionManager.cs#L665) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.groups | [666:17](../YouTubeHomeSectionManager.cs#L666) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.groups.anonymous@29128 | [667:24](../YouTubeHomeSectionManager.cs#L667) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.groups.anonymous@29128.section | [667:24](../YouTubeHomeSectionManager.cs#L667) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.groups.anonymous@29255 | [669:26](../YouTubeHomeSectionManager.cs#L669) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.groups.anonymous@29255.section | [669:26](../YouTubeHomeSectionManager.cs#L669) |
| iteration-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.group | [671:13](../YouTubeHomeSectionManager.cs#L671) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.group.section | [682:21](../YouTubeHomeSectionManager.cs#L682) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.group.entry | [683:21](../YouTubeHomeSectionManager.cs#L683) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.group.entry.anonymous@29892 | [683:58](../YouTubeHomeSectionManager.cs#L683) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.group.entry.anonymous@29892.candidate | [683:58](../YouTubeHomeSectionManager.cs#L683) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.group.excluded | [687:21](../YouTubeHomeSectionManager.cs#L687) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.group.providerAlreadyExcluded | [689:21](../YouTubeHomeSectionManager.cs#L689) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.group.providerOwnedByPlugin | [692:21](../YouTubeHomeSectionManager.cs#L692) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.group.stateChanged | [702:21](../YouTubeHomeSectionManager.cs#L702) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.group.updatedExcluded | [725:21](../YouTubeHomeSectionManager.cs#L725) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.group.readback | [734:21](../YouTubeHomeSectionManager.cs#L734) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.group.readback.anonymous@32243 | [735:28](../YouTubeHomeSectionManager.cs#L735) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.FilterLatestMediaBlocksAsync.group.readback.anonymous@32243.candidate | [735:28](../YouTubeHomeSectionManager.cs#L735) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLatestMediaBlockAsync | [760:9](../YouTubeHomeSectionManager.cs#L760) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLatestMediaBlockAsync.user | [761:13](../YouTubeHomeSectionManager.cs#L761) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLatestMediaBlockAsync.cancellationToken | [762:13](../YouTubeHomeSectionManager.cs#L762) |
| pattern-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLatestMediaBlockAsync.state | [764:67](../YouTubeHomeSectionManager.cs#L764) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLatestMediaBlockAsync.restored | [770:17](../YouTubeHomeSectionManager.cs#L770) |
| iteration-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLatestMediaBlockAsync.entry | [771:13](../YouTubeHomeSectionManager.cs#L771) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLatestMediaBlockAsync.entry.sameId | [774:21](../YouTubeHomeSectionManager.cs#L774) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLatestMediaBlockAsync.entry.sameId.anonymous@33843 | [775:28](../YouTubeHomeSectionManager.cs#L775) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLatestMediaBlockAsync.entry.sameId.anonymous@33843.section | [775:28](../YouTubeHomeSectionManager.cs#L775) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLatestMediaBlockAsync.entry.section | [797:21](../YouTubeHomeSectionManager.cs#L797) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLatestMediaBlockAsync.entry.currentExcluded | [798:21](../YouTubeHomeSectionManager.cs#L798) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLatestMediaBlockAsync.entry.ownedIds | [800:21](../YouTubeHomeSectionManager.cs#L800) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLatestMediaBlockAsync.entry.restoredExcluded | [802:21](../YouTubeHomeSectionManager.cs#L802) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLatestMediaBlockAsync.entry.restoredExcluded.anonymous@35218 | [803:28](../YouTubeHomeSectionManager.cs#L803) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLatestMediaBlockAsync.entry.restoredExcluded.anonymous@35218.id | [803:28](../YouTubeHomeSectionManager.cs#L803) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLatestMediaBlockAsync.entry.readback | [824:21](../YouTubeHomeSectionManager.cs#L824) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLatestMediaBlockAsync.entry.readback.anonymous@36182 | [825:28](../YouTubeHomeSectionManager.cs#L825) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLatestMediaBlockAsync.entry.readback.anonymous@36182.candidate | [825:28](../YouTubeHomeSectionManager.cs#L825) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RestoreLatestMediaBlockAsync.entry.readbackExcluded | [830:21](../YouTubeHomeSectionManager.cs#L830) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.IsLatestMediaBlock | [858:9](../YouTubeHomeSectionManager.cs#L858) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.IsLatestMediaBlock.section | [858:48](../YouTubeHomeSectionManager.cs#L858) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.ContainsFolderId | [864:9](../YouTubeHomeSectionManager.cs#L864) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.ContainsFolderId.folderIds | [865:13](../YouTubeHomeSectionManager.cs#L865) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.ContainsFolderId.folderId | [866:13](../YouTubeHomeSectionManager.cs#L866) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.AppendFolderId | [869:9](../YouTubeHomeSectionManager.cs#L869) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.AppendFolderId.folderIds | [870:13](../YouTubeHomeSectionManager.cs#L870) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.AppendFolderId.folderId | [871:13](../YouTubeHomeSectionManager.cs#L871) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.AppendFolderId.values | [873:17](../YouTubeHomeSectionManager.cs#L873) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetCurrentSections | [879:9](../YouTubeHomeSectionManager.cs#L879) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetCurrentSections.user | [880:13](../YouTubeHomeSectionManager.cs#L880) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetCurrentSections.cancellationToken | [881:13](../YouTubeHomeSectionManager.cs#L881) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.IsStrictLegacyAggregate | [885:9](../YouTubeHomeSectionManager.cs#L885) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.IsStrictLegacyAggregate.section | [886:13](../YouTubeHomeSectionManager.cs#L886) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.IsStrictLegacyAggregate.context | [887:13](../YouTubeHomeSectionManager.cs#L887) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.IsStrictLegacyAggregate.excluded | [896:17](../YouTubeHomeSectionManager.cs#L896) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.IsStrictLegacyAggregate.excludedSet | [900:17](../YouTubeHomeSectionManager.cs#L900) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.IsStrictLegacyAggregate.nonYouTubeViewIds | [904:17](../YouTubeHomeSectionManager.cs#L904) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.IsStrictLegacyAggregate.nonYouTubeViewIds.anonymous@39376 | [905:24](../YouTubeHomeSectionManager.cs#L905) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.IsStrictLegacyAggregate.nonYouTubeViewIds.anonymous@39376.id | [905:24](../YouTubeHomeSectionManager.cs#L905) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.LogLegacyAggregateNoMatch | [915:9](../YouTubeHomeSectionManager.cs#L915) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.LogLegacyAggregateNoMatch.user | [916:13](../YouTubeHomeSectionManager.cs#L916) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.LogLegacyAggregateNoMatch.sections | [917:13](../YouTubeHomeSectionManager.cs#L917) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.LogLegacyAggregateNoMatch.context | [918:13](../YouTubeHomeSectionManager.cs#L918) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.LogLegacyAggregateNoMatch.candidates | [920:17](../YouTubeHomeSectionManager.cs#L920) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.LogLegacyAggregateNoMatch.candidates.anonymous@40151 | [921:24](../YouTubeHomeSectionManager.cs#L921) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.LogLegacyAggregateNoMatch.candidates.anonymous@40151.section | [921:24](../YouTubeHomeSectionManager.cs#L921) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.LogLegacyAggregateNoMatch.expectedExcluded | [931:17](../YouTubeHomeSectionManager.cs#L931) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.LogLegacyAggregateNoMatch.expectedExcluded.anonymous@40626 | [932:24](../YouTubeHomeSectionManager.cs#L932) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.LogLegacyAggregateNoMatch.expectedExcluded.anonymous@40626.id | [932:24](../YouTubeHomeSectionManager.cs#L932) |
| iteration-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.LogLegacyAggregateNoMatch.candidate | [934:13](../YouTubeHomeSectionManager.cs#L934) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.LogLegacyAggregateNoMatch.candidate.excluded | [936:21](../YouTubeHomeSectionManager.cs#L936) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.LogLegacyAggregateNoMatch.candidate.customName | [939:21](../YouTubeHomeSectionManager.cs#L939) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.LogLegacyAggregateNoMatch.candidate.customNameState | [940:21](../YouTubeHomeSectionManager.cs#L940) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.HasLegacyAggregateShape | [957:9](../YouTubeHomeSectionManager.cs#L957) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.HasLegacyAggregateShape.section | [957:53](../YouTubeHomeSectionManager.cs#L957) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.HasLegacyAggregateShape.itemTypes | [970:17](../YouTubeHomeSectionManager.cs#L970) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.HasLegacyAggregateShape.customName | [971:17](../YouTubeHomeSectionManager.cs#L971) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetRuntimeProperty | [984:9](../YouTubeHomeSectionManager.cs#L984) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetRuntimeProperty.section | [984:51](../YouTubeHomeSectionManager.cs#L984) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetRuntimeProperty.propertyName | [984:75](../YouTubeHomeSectionManager.cs#L984) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetRuntimeString | [989:9](../YouTubeHomeSectionManager.cs#L989) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetRuntimeString.section | [989:49](../YouTubeHomeSectionManager.cs#L989) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetRuntimeString.propertyName | [989:73](../YouTubeHomeSectionManager.cs#L989) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetRuntimeStringArray | [992:9](../YouTubeHomeSectionManager.cs#L992) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetRuntimeStringArray.section | [992:56](../YouTubeHomeSectionManager.cs#L992) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetRuntimeStringArray.propertyName | [992:80](../YouTubeHomeSectionManager.cs#L992) |
| pattern-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetRuntimeStringArray.values | [993:78](../YouTubeHomeSectionManager.cs#L993) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.MatchesAggregateSnapshot | [997:9](../YouTubeHomeSectionManager.cs#L997) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.MatchesAggregateSnapshot.section | [998:13](../YouTubeHomeSectionManager.cs#L998) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.MatchesAggregateSnapshot.backup | [999:13](../YouTubeHomeSectionManager.cs#L999) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryLoadLatestMediaBlockFilterState | [1014:9](../YouTubeHomeSectionManager.cs#L1014) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryLoadLatestMediaBlockFilterState.user | [1015:13](../YouTubeHomeSectionManager.cs#L1015) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryLoadLatestMediaBlockFilterState.state | [1016:13](../YouTubeHomeSectionManager.cs#L1016) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryLoadLatestMediaBlockFilterState.path | [1019:17](../YouTubeHomeSectionManager.cs#L1019) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryLoadLatestMediaBlockFilterState.sectionIds | [1034:21](../YouTubeHomeSectionManager.cs#L1034) |
| iteration-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryLoadLatestMediaBlockFilterState.entry | [1035:17](../YouTubeHomeSectionManager.cs#L1035) |
| catch-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryLoadLatestMediaBlockFilterState.ex | [1054:19](../YouTubeHomeSectionManager.cs#L1054) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.TrySaveLatestMediaBlockFilterState | [1064:9](../YouTubeHomeSectionManager.cs#L1064) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.TrySaveLatestMediaBlockFilterState.user | [1065:13](../YouTubeHomeSectionManager.cs#L1065) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.TrySaveLatestMediaBlockFilterState.state | [1066:13](../YouTubeHomeSectionManager.cs#L1066) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.TrySaveLatestMediaBlockFilterState.path | [1068:17](../YouTubeHomeSectionManager.cs#L1068) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.TrySaveLatestMediaBlockFilterState.tempPath | [1072:21](../YouTubeHomeSectionManager.cs#L1072) |
| catch-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.TrySaveLatestMediaBlockFilterState.ex | [1089:19](../YouTubeHomeSectionManager.cs#L1089) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryLoadAggregateBackup | [1103:9](../YouTubeHomeSectionManager.cs#L1103) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryLoadAggregateBackup.user | [1104:13](../YouTubeHomeSectionManager.cs#L1104) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryLoadAggregateBackup.backup | [1105:13](../YouTubeHomeSectionManager.cs#L1105) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryLoadAggregateBackup.section | [1106:13](../YouTubeHomeSectionManager.cs#L1106) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryLoadAggregateBackup.path | [1110:17](../YouTubeHomeSectionManager.cs#L1110) |
| catch-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryLoadAggregateBackup.ex | [1141:19](../YouTubeHomeSectionManager.cs#L1141) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryStoreAggregateBackup | [1152:9](../YouTubeHomeSectionManager.cs#L1152) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryStoreAggregateBackup.user | [1152:46](../YouTubeHomeSectionManager.cs#L1152) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryStoreAggregateBackup.section | [1152:57](../YouTubeHomeSectionManager.cs#L1152) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryStoreAggregateBackup.originalIndex | [1152:81](../YouTubeHomeSectionManager.cs#L1152) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryStoreAggregateBackup.path | [1154:17](../YouTubeHomeSectionManager.cs#L1154) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryStoreAggregateBackup.tempPath | [1158:21](../YouTubeHomeSectionManager.cs#L1158) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryStoreAggregateBackup.backup | [1165:21](../YouTubeHomeSectionManager.cs#L1165) |
| catch-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryStoreAggregateBackup.ex | [1180:19](../YouTubeHomeSectionManager.cs#L1180) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryRemoveAggregateBackup | [1193:9](../YouTubeHomeSectionManager.cs#L1193) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryRemoveAggregateBackup.user | [1193:54](../YouTubeHomeSectionManager.cs#L1193) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryRemoveAggregateBackup.path | [1195:17](../YouTubeHomeSectionManager.cs#L1195) |
| catch-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.TryRemoveAggregateBackup.ex | [1199:19](../YouTubeHomeSectionManager.cs#L1199) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetAggregateBackupPath | [1207:9](../YouTubeHomeSectionManager.cs#L1207) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetAggregateBackupPath.user | [1207:55](../YouTubeHomeSectionManager.cs#L1207) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetLatestMediaBlockFilterPath | [1215:9](../YouTubeHomeSectionManager.cs#L1215) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.GetLatestMediaBlockFilterPath.user | [1215:62](../YouTubeHomeSectionManager.cs#L1215) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.BuildSection | [1223:9](../YouTubeHomeSectionManager.cs#L1223) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.BuildSection.root | [1223:52](../YouTubeHomeSectionManager.cs#L1223) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.BuildSection.name | [1225:17](../YouTubeHomeSectionManager.cs#L1225) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.BuildSection.section | [1228:17](../YouTubeHomeSectionManager.cs#L1228) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.BuildSection.parentId | [1240:17](../YouTubeHomeSectionManager.cs#L1240) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.NeedsUpdate | [1252:9](../YouTubeHomeSectionManager.cs#L1252) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.NeedsUpdate.current | [1252:41](../YouTubeHomeSectionManager.cs#L1252) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.NeedsUpdate.desired | [1252:65](../YouTubeHomeSectionManager.cs#L1252) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.ApplyManagedValues | [1263:9](../YouTubeHomeSectionManager.cs#L1263) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.ApplyManagedValues.current | [1264:13](../YouTubeHomeSectionManager.cs#L1264) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.ApplyManagedValues.desired | [1265:13](../YouTubeHomeSectionManager.cs#L1265) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.IsManaged | [1282:9](../YouTubeHomeSectionManager.cs#L1282) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.IsManaged.section | [1282:39](../YouTubeHomeSectionManager.cs#L1282) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.BuildManagedId | [1286:9](../YouTubeHomeSectionManager.cs#L1286) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.BuildManagedId.externalId | [1286:46](../YouTubeHomeSectionManager.cs#L1286) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.BuildManagedId.sha | [1288:23](../YouTubeHomeSectionManager.cs#L1288) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.BuildManagedId.digest | [1289:17](../YouTubeHomeSectionManager.cs#L1289) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.FindSectionMutation | [1295:9](../YouTubeHomeSectionManager.cs#L1295) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.FindSectionMutation.name | [1295:56](../YouTubeHomeSectionManager.cs#L1295) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.FindSectionMutation.payloadType | [1295:69](../YouTubeHomeSectionManager.cs#L1295) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHomeSectionManager.FindSectionMutation.anonymous@56680 | [1299:33](../YouTubeHomeSectionManager.cs#L1299) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.FindSectionMutation.anonymous@56680.method | [1299:33](../YouTubeHomeSectionManager.cs#L1299) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.FindSectionMutation.anonymous@56680.parameters | [1304:25](../YouTubeHomeSectionManager.cs#L1304) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.FindMoveHomeSections | [1312:9](../YouTubeHomeSectionManager.cs#L1312) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.InvokeMoveAsync | [1326:9](../YouTubeHomeSectionManager.cs#L1326) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.InvokeMoveAsync.userId | [1327:13](../YouTubeHomeSectionManager.cs#L1327) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.InvokeMoveAsync.sectionId | [1328:13](../YouTubeHomeSectionManager.cs#L1328) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.InvokeMoveAsync.newIndex | [1329:13](../YouTubeHomeSectionManager.cs#L1329) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.InvokeMoveAsync.cancellationToken | [1330:13](../YouTubeHomeSectionManager.cs#L1330) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.InvokeMoveAsync.result | [1332:21](../YouTubeHomeSectionManager.cs#L1332) |
| catch-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.InvokeMoveAsync.ex | [1339:19](../YouTubeHomeSectionManager.cs#L1339) |
| pattern-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.InvokeMoveAsync.task | [1346:32](../YouTubeHomeSectionManager.cs#L1346) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.InvokeMutationAsync | [1350:9](../YouTubeHomeSectionManager.cs#L1350) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.InvokeMutationAsync.method | [1351:13](../YouTubeHomeSectionManager.cs#L1351) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.InvokeMutationAsync.userId | [1352:13](../YouTubeHomeSectionManager.cs#L1352) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.InvokeMutationAsync.payload | [1353:13](../YouTubeHomeSectionManager.cs#L1353) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.InvokeMutationAsync.cancellationToken | [1354:13](../YouTubeHomeSectionManager.cs#L1354) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.InvokeMutationAsync.result | [1356:21](../YouTubeHomeSectionManager.cs#L1356) |
| catch-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.InvokeMutationAsync.ex | [1363:19](../YouTubeHomeSectionManager.cs#L1363) |
| pattern-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.InvokeMutationAsync.task | [1370:32](../YouTubeHomeSectionManager.cs#L1370) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.SetRequiredRuntimeProperty | [1374:9](../YouTubeHomeSectionManager.cs#L1374) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SetRequiredRuntimeProperty.section | [1375:13](../YouTubeHomeSectionManager.cs#L1375) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SetRequiredRuntimeProperty.propertyName | [1376:13](../YouTubeHomeSectionManager.cs#L1376) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SetRequiredRuntimeProperty.value | [1377:13](../YouTubeHomeSectionManager.cs#L1377) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.SetRuntimeProperty | [1387:9](../YouTubeHomeSectionManager.cs#L1387) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SetRuntimeProperty.section | [1388:13](../YouTubeHomeSectionManager.cs#L1388) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SetRuntimeProperty.propertyName | [1389:13](../YouTubeHomeSectionManager.cs#L1389) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.SetRuntimeProperty.value | [1390:13](../YouTubeHomeSectionManager.cs#L1390) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.SetRuntimeProperty.property | [1392:17](../YouTubeHomeSectionManager.cs#L1392) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.CopyRuntimeProperty | [1402:9](../YouTubeHomeSectionManager.cs#L1402) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.CopyRuntimeProperty.source | [1403:13](../YouTubeHomeSectionManager.cs#L1403) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.CopyRuntimeProperty.target | [1404:13](../YouTubeHomeSectionManager.cs#L1404) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.CopyRuntimeProperty.propertyName | [1405:13](../YouTubeHomeSectionManager.cs#L1405) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.CopyRuntimeProperty.property | [1407:17](../YouTubeHomeSectionManager.cs#L1407) |
| method | Emby.YouTubePlugin.YouTubeHomeSectionManager.RuntimePropertyEquals | [1416:9](../YouTubeHomeSectionManager.cs#L1416) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.RuntimePropertyEquals.current | [1417:13](../YouTubeHomeSectionManager.cs#L1417) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.RuntimePropertyEquals.desired | [1418:13](../YouTubeHomeSectionManager.cs#L1418) |
| parameter | Emby.YouTubePlugin.YouTubeHomeSectionManager.RuntimePropertyEquals.propertyName | [1419:13](../YouTubeHomeSectionManager.cs#L1419) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RuntimePropertyEquals.property | [1421:17](../YouTubeHomeSectionManager.cs#L1421) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RuntimePropertyEquals.currentValue | [1427:17](../YouTubeHomeSectionManager.cs#L1427) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RuntimePropertyEquals.desiredValue | [1428:17](../YouTubeHomeSectionManager.cs#L1428) |
| pattern-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RuntimePropertyEquals.emptyDesired | [1429:63](../YouTubeHomeSectionManager.cs#L1429) |
| pattern-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RuntimePropertyEquals.emptyCurrent | [1431:63](../YouTubeHomeSectionManager.cs#L1431) |
| pattern-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RuntimePropertyEquals.currentArray | [1433:39](../YouTubeHomeSectionManager.cs#L1433) |
| pattern-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RuntimePropertyEquals.desiredArray | [1433:77](../YouTubeHomeSectionManager.cs#L1433) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RuntimePropertyEquals.currentStrings | [1435:21](../YouTubeHomeSectionManager.cs#L1435) |
| local-variable | Emby.YouTubePlugin.YouTubeHomeSectionManager.RuntimePropertyEquals.desiredStrings | [1436:21](../YouTubeHomeSectionManager.cs#L1436) |

</details>

<a id="file-086dc9043b3c2586"></a>
## [YouTubeHttpClientFactory.cs](../YouTubeHttpClientFactory.cs)

Plugin Compile source (SDK default); 4205 bytes; 110 lines.

<details>
<summary>All 26 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [11:1](../YouTubeHttpClientFactory.cs#L11) |
| type | Emby.YouTubePlugin.YouTubeHttpClientFactory | [13:5](../YouTubeHttpClientFactory.cs#L13) |
| field | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectTimeout | [15:42](../YouTubeHttpClientFactory.cs#L15) |
| field | Emby.YouTubePlugin.YouTubeHttpClientFactory.AddressAttemptTimeout | [16:42](../YouTubeHttpClientFactory.cs#L16) |
| method | Emby.YouTubePlugin.YouTubeHttpClientFactory.CreateHandler | [18:9](../YouTubeHttpClientFactory.cs#L18) |
| parameter | Emby.YouTubePlugin.YouTubeHttpClientFactory.CreateHandler.allowAutoRedirect | [19:13](../YouTubeHttpClientFactory.cs#L19) |
| parameter | Emby.YouTubePlugin.YouTubeHttpClientFactory.CreateHandler.automaticDecompression | [20:13](../YouTubeHttpClientFactory.cs#L20) |
| method | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectIpv4FirstAsync | [34:9](../YouTubeHttpClientFactory.cs#L34) |
| parameter | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectIpv4FirstAsync.context | [35:13](../YouTubeHttpClientFactory.cs#L35) |
| parameter | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectIpv4FirstAsync.cancellationToken | [36:13](../YouTubeHttpClientFactory.cs#L36) |
| local-variable | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectIpv4FirstAsync.endpoint | [38:17](../YouTubeHttpClientFactory.cs#L38) |
| local-variable | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectIpv4FirstAsync.addresses | [39:25](../YouTubeHttpClientFactory.cs#L39) |
| catch-variable | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectIpv4FirstAsync.ex | [46:19](../YouTubeHttpClientFactory.cs#L46) |
| local-variable | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectIpv4FirstAsync.ipv4 | [57:17](../YouTubeHttpClientFactory.cs#L57) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectIpv4FirstAsync.ipv4.anonymous@2172 | [58:24](../YouTubeHttpClientFactory.cs#L58) |
| parameter | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectIpv4FirstAsync.ipv4.anonymous@2172.address | [58:24](../YouTubeHttpClientFactory.cs#L58) |
| local-variable | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectIpv4FirstAsync.ipv6 | [61:17](../YouTubeHttpClientFactory.cs#L61) |
| anonymous-function | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectIpv4FirstAsync.ipv6.anonymous@2347 | [62:24](../YouTubeHttpClientFactory.cs#L62) |
| parameter | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectIpv4FirstAsync.ipv6.anonymous@2347.address | [62:24](../YouTubeHttpClientFactory.cs#L62) |
| local-variable | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectIpv4FirstAsync.ordered | [65:17](../YouTubeHttpClientFactory.cs#L65) |
| local-variable | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectIpv4FirstAsync.lastError | [71:24](../YouTubeHttpClientFactory.cs#L71) |
| iteration-variable | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectIpv4FirstAsync.address | [72:13](../YouTubeHttpClientFactory.cs#L72) |
| local-variable | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectIpv4FirstAsync.address.socket | [75:21](../YouTubeHttpClientFactory.cs#L75) |
| local-variable | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectIpv4FirstAsync.address.attempt | [82:31](../YouTubeHttpClientFactory.cs#L82) |
| catch-variable | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectIpv4FirstAsync.address.ex | [90:23](../YouTubeHttpClientFactory.cs#L90) |
| catch-variable | Emby.YouTubePlugin.YouTubeHttpClientFactory.ConnectIpv4FirstAsync.address.ex | [98:23](../YouTubeHttpClientFactory.cs#L98) |

</details>

<a id="file-7d33b8b54f4ebb57"></a>
## [YouTubeImageProvider.cs](../YouTubeImageProvider.cs)

Plugin Compile source (SDK default); 14149 bytes; 338 lines.

<details>
<summary>All 84 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [19:1](../YouTubeImageProvider.cs#L19) |
| type | Emby.YouTubePlugin.YouTubeImageProvider | [28:5](../YouTubeImageProvider.cs#L28) |
| property | Emby.YouTubePlugin.YouTubeImageProvider.Name | [30:9](../YouTubeImageProvider.cs#L30) |
| field | Emby.YouTubePlugin.YouTubeImageProvider._failedFetches | [38:70](../YouTubeImageProvider.cs#L38) |
| field | Emby.YouTubePlugin.YouTubeImageProvider.FailedFetchCooldown | [39:42](../YouTubeImageProvider.cs#L39) |
| field | Emby.YouTubePlugin.YouTubeImageProvider.VideoIdRegex | [41:39](../YouTubeImageProvider.cs#L41) |
| field | Emby.YouTubePlugin.YouTubeImageProvider.Http | [45:44](../YouTubeImageProvider.cs#L45) |
| method | Emby.YouTubePlugin.YouTubeImageProvider.CreateImageHttp | [47:9](../YouTubeImageProvider.cs#L47) |
| local-variable | Emby.YouTubePlugin.YouTubeImageProvider.CreateImageHttp.client | [49:17](../YouTubeImageProvider.cs#L49) |
| field | Emby.YouTubePlugin.YouTubeImageProvider.MaxFailedFetches | [64:27](../YouTubeImageProvider.cs#L64) |
| method | Emby.YouTubePlugin.YouTubeImageProvider.TryGetVideoId | [66:9](../YouTubeImageProvider.cs#L66) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.TryGetVideoId.item | [66:47](../YouTubeImageProvider.cs#L66) |
| local-variable | Emby.YouTubePlugin.YouTubeImageProvider.TryGetVideoId.path | [77:21](../YouTubeImageProvider.cs#L77) |
| local-variable | Emby.YouTubePlugin.YouTubeImageProvider.TryGetVideoId.m | [80:25](../YouTubeImageProvider.cs#L80) |
| local-variable | Emby.YouTubePlugin.YouTubeImageProvider.TryGetVideoId.videoId | [83:29](../YouTubeImageProvider.cs#L83) |
| local-variable | Emby.YouTubePlugin.YouTubeImageProvider.TryGetVideoId.providerId | [84:29](../YouTubeImageProvider.cs#L84) |
| catch-variable | Emby.YouTubePlugin.YouTubeImageProvider.TryGetVideoId.ex | [93:19](../YouTubeImageProvider.cs#L93) |
| method | Emby.YouTubePlugin.YouTubeImageProvider.IsPluginExternalId | [100:9](../YouTubeImageProvider.cs#L100) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.IsPluginExternalId.externalId | [100:48](../YouTubeImageProvider.cs#L100) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.IsPluginExternalId.videoId | [100:68](../YouTubeImageProvider.cs#L100) |
| method | Emby.YouTubePlugin.YouTubeImageProvider.Supports | [107:9](../YouTubeImageProvider.cs#L107) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.Supports.item | [107:30](../YouTubeImageProvider.cs#L107) |
| method | Emby.YouTubePlugin.YouTubeImageProvider.GetSupportedImages | [109:9](../YouTubeImageProvider.cs#L109) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.GetSupportedImages.item | [109:47](../YouTubeImageProvider.cs#L109) |
| method | Emby.YouTubePlugin.YouTubeImageProvider.IRemoteImageProvider.GetSupportedImages | [112:9](../YouTubeImageProvider.cs#L112) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.IRemoteImageProvider.GetSupportedImages.item | [112:72](../YouTubeImageProvider.cs#L112) |
| method | Emby.YouTubePlugin.YouTubeImageProvider.GetImage | [115:9](../YouTubeImageProvider.cs#L115) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.GetImage.result | [116:13](../YouTubeImageProvider.cs#L116) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.GetImage.type | [116:40](../YouTubeImageProvider.cs#L116) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.GetImage.cancellationToken | [116:56](../YouTubeImageProvider.cs#L116) |
| local-variable | Emby.YouTubePlugin.YouTubeImageProvider.GetImage.response | [118:17](../YouTubeImageProvider.cs#L118) |
| local-variable | Emby.YouTubePlugin.YouTubeImageProvider.GetImage.videoId | [119:17](../YouTubeImageProvider.cs#L119) |
| iteration-variable | Emby.YouTubePlugin.YouTubeImageProvider.GetImage.url | [122:13](../YouTubeImageProvider.cs#L122) |
| local-variable | Emby.YouTubePlugin.YouTubeImageProvider.GetImage.url.resp | [127:31](../YouTubeImageProvider.cs#L127) |
| local-variable | Emby.YouTubePlugin.YouTubeImageProvider.GetImage.url.bytes | [130:25](../YouTubeImageProvider.cs#L130) |
| catch-variable | Emby.YouTubePlugin.YouTubeImageProvider.GetImage.url.ex | [141:23](../YouTubeImageProvider.cs#L141) |
| catch-variable | Emby.YouTubePlugin.YouTubeImageProvider.GetImage.url.ex | [145:23](../YouTubeImageProvider.cs#L145) |
| method | Emby.YouTubePlugin.YouTubeImageProvider.EvictFailedFetches | [162:9](../YouTubeImageProvider.cs#L162) |
| local-variable | Emby.YouTubePlugin.YouTubeImageProvider.EvictFailedFetches.cutoff | [165:17](../YouTubeImageProvider.cs#L165) |
| iteration-variable | Emby.YouTubePlugin.YouTubeImageProvider.EvictFailedFetches.kvp | [166:13](../YouTubeImageProvider.cs#L166) |
| local-variable | Emby.YouTubePlugin.YouTubeImageProvider.EvictFailedFetches.overflow | [173:21](../YouTubeImageProvider.cs#L173) |
| local-variable | Emby.YouTubePlugin.YouTubeImageProvider.EvictFailedFetches.oldest | [174:21](../YouTubeImageProvider.cs#L174) |
| anonymous-function | Emby.YouTubePlugin.YouTubeImageProvider.EvictFailedFetches.oldest.anonymous@7781 | [175:30](../YouTubeImageProvider.cs#L175) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.EvictFailedFetches.oldest.anonymous@7781.kvp | [175:30](../YouTubeImageProvider.cs#L175) |
| anonymous-function | Emby.YouTubePlugin.YouTubeImageProvider.EvictFailedFetches.oldest.anonymous@7863 | [177:29](../YouTubeImageProvider.cs#L177) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.EvictFailedFetches.oldest.anonymous@7863.kvp | [177:29](../YouTubeImageProvider.cs#L177) |
| iteration-variable | Emby.YouTubePlugin.YouTubeImageProvider.EvictFailedFetches.key | [179:17](../YouTubeImageProvider.cs#L179) |
| method | Emby.YouTubePlugin.YouTubeImageProvider.GetImages | [184:9](../YouTubeImageProvider.cs#L184) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.GetImages.item | [185:13](../YouTubeImageProvider.cs#L185) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.GetImages.libraryOptions | [185:28](../YouTubeImageProvider.cs#L185) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.GetImages.cancellationToken | [185:59](../YouTubeImageProvider.cs#L185) |
| local-variable | Emby.YouTubePlugin.YouTubeImageProvider.GetImages.videoId | [187:17](../YouTubeImageProvider.cs#L187) |
| local-variable | Emby.YouTubePlugin.YouTubeImageProvider.GetImages.url | [191:17](../YouTubeImageProvider.cs#L191) |
| local-variable | Emby.YouTubePlugin.YouTubeImageProvider.GetImages.images | [192:42](../YouTubeImageProvider.cs#L192) |
| method | Emby.YouTubePlugin.YouTubeImageProvider.GetImageResponse | [217:9](../YouTubeImageProvider.cs#L217) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.GetImageResponse.url | [217:62](../YouTubeImageProvider.cs#L217) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.GetImageResponse.cancellationToken | [217:74](../YouTubeImageProvider.cs#L217) |
| local-variable | Emby.YouTubePlugin.YouTubeImageProvider.GetImageResponse.response | [219:17](../YouTubeImageProvider.cs#L219) |
| anonymous-function | Emby.YouTubePlugin.YouTubeImageProvider.GetImageResponse.anonymous@10077 | [233:25](../YouTubeImageProvider.cs#L233) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.GetImageResponse.anonymous@10077.h | [233:25](../YouTubeImageProvider.cs#L233) |
| anonymous-function | Emby.YouTubePlugin.YouTubeImageProvider.GetImageResponse.anonymous@10113 | [234:25](../YouTubeImageProvider.cs#L234) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.GetImageResponse.anonymous@10113.h | [234:25](../YouTubeImageProvider.cs#L234) |
| method | Emby.YouTubePlugin.YouTubeImageProvider.HasChanged | [247:9](../YouTubeImageProvider.cs#L247) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.HasChanged.item | [247:32](../YouTubeImageProvider.cs#L247) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.HasChanged.libraryOptions | [247:47](../YouTubeImageProvider.cs#L247) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.HasChanged.directoryService | [247:78](../YouTubeImageProvider.cs#L247) |
| pattern-variable | Emby.YouTubePlugin.YouTubeImageProvider.HasChanged.lastFail | [259:73](../YouTubeImageProvider.cs#L259) |
| catch-variable | Emby.YouTubePlugin.YouTubeImageProvider.HasChanged.ex | [265:19](../YouTubeImageProvider.cs#L265) |
| method | Emby.YouTubePlugin.YouTubeImageProvider.HasChanged | [272:9](../YouTubeImageProvider.cs#L272) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.HasChanged.itemResult | [273:13](../YouTubeImageProvider.cs#L273) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.HasChanged.libraryOptions | [274:13](../YouTubeImageProvider.cs#L274) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.HasChanged.options | [275:13](../YouTubeImageProvider.cs#L275) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.HasChanged.directoryService | [276:13](../YouTubeImageProvider.cs#L276) |
| local-variable | Emby.YouTubePlugin.YouTubeImageProvider.HasChanged.item | [280:21](../YouTubeImageProvider.cs#L280) |
| catch-variable | Emby.YouTubePlugin.YouTubeImageProvider.HasChanged.ex | [292:19](../YouTubeImageProvider.cs#L292) |
| method | Emby.YouTubePlugin.YouTubeImageProvider.GetStableThumbnailUrl | [300:9](../YouTubeImageProvider.cs#L300) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.GetStableThumbnailUrl.videoId | [300:54](../YouTubeImageProvider.cs#L300) |
| method | Emby.YouTubePlugin.YouTubeImageProvider.EnsurePrimaryImage | [303:9](../YouTubeImageProvider.cs#L303) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.EnsurePrimaryImage.item | [303:49](../YouTubeImageProvider.cs#L303) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.EnsurePrimaryImage.reason | [303:65](../YouTubeImageProvider.cs#L303) |
| local-variable | Emby.YouTubePlugin.YouTubeImageProvider.EnsurePrimaryImage.videoId | [305:17](../YouTubeImageProvider.cs#L305) |
| local-variable | Emby.YouTubePlugin.YouTubeImageProvider.EnsurePrimaryImage.url | [312:17](../YouTubeImageProvider.cs#L312) |
| method | Emby.YouTubePlugin.YouTubeImageProvider.GetThumbnailCandidates | [326:9](../YouTubeImageProvider.cs#L326) |
| parameter | Emby.YouTubePlugin.YouTubeImageProvider.GetThumbnailCandidates.videoId | [326:67](../YouTubeImageProvider.cs#L326) |

</details>

<a id="file-bc134ac40739676b"></a>
## [YouTubeManagedHttp.cs](../YouTubeManagedHttp.cs)

Plugin Compile source (SDK default); 11990 bytes; 198 lines.

<details>
<summary>All 88 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [12:1](../YouTubeManagedHttp.cs#L12) |
| type | Emby.YouTubePlugin.YouTubeManagedFailureKind | [14:5](../YouTubeManagedHttp.cs#L14) |
| enum-member | Emby.YouTubePlugin.YouTubeManagedFailureKind.INVALID_REQUEST | [16:9](../YouTubeManagedHttp.cs#L16) |
| enum-member | Emby.YouTubePlugin.YouTubeManagedFailureKind.INVALID_RESPONSE | [16:26](../YouTubeManagedHttp.cs#L16) |
| enum-member | Emby.YouTubePlugin.YouTubeManagedFailureKind.HTTP_DENIED | [16:44](../YouTubeManagedHttp.cs#L16) |
| enum-member | Emby.YouTubePlugin.YouTubeManagedFailureKind.VERIFICATION_REQUIRED | [16:57](../YouTubeManagedHttp.cs#L16) |
| enum-member | Emby.YouTubePlugin.YouTubeManagedFailureKind.VIDEO_RESTRICTED | [16:80](../YouTubeManagedHttp.cs#L16) |
| enum-member | Emby.YouTubePlugin.YouTubeManagedFailureKind.HTTP_FAILED | [17:9](../YouTubeManagedHttp.cs#L17) |
| enum-member | Emby.YouTubePlugin.YouTubeManagedFailureKind.SIZE_LIMIT | [17:22](../YouTubeManagedHttp.cs#L17) |
| enum-member | Emby.YouTubePlugin.YouTubeManagedFailureKind.REQUEST_BUDGET | [17:34](../YouTubeManagedHttp.cs#L17) |
| enum-member | Emby.YouTubePlugin.YouTubeManagedFailureKind.CANCELED | [17:50](../YouTubeManagedHttp.cs#L17) |
| enum-member | Emby.YouTubePlugin.YouTubeManagedFailureKind.TRANSPORT_FAILED | [17:60](../YouTubeManagedHttp.cs#L17) |
| type | Emby.YouTubePlugin.YouTubeManagedResolutionException | [20:5](../YouTubeManagedHttp.cs#L20) |
| constructor | Emby.YouTubePlugin.YouTubeManagedResolutionException.YouTubeManagedResolutionException | [22:9](../YouTubeManagedHttp.cs#L22) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolutionException.YouTubeManagedResolutionException.kind | [22:52](../YouTubeManagedHttp.cs#L22) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolutionException.YouTubeManagedResolutionException.stage | [22:84](../YouTubeManagedHttp.cs#L22) |
| property | Emby.YouTubePlugin.YouTubeManagedResolutionException.Kind | [24:9](../YouTubeManagedHttp.cs#L24) |
| accessor | Emby.YouTubePlugin.YouTubeManagedResolutionException.Kind.get | [24:51](../YouTubeManagedHttp.cs#L24) |
| property | Emby.YouTubePlugin.YouTubeManagedResolutionException.Stage | [25:9](../YouTubeManagedHttp.cs#L25) |
| accessor | Emby.YouTubePlugin.YouTubeManagedResolutionException.Stage.get | [25:33](../YouTubeManagedHttp.cs#L25) |
| type | Emby.YouTubePlugin.YouTubeManagedHttp | [30:5](../YouTubeManagedHttp.cs#L30) |
| field | Emby.YouTubePlugin.YouTubeManagedHttp.MaximumRequests | [32:28](../YouTubeManagedHttp.cs#L32) |
| field | Emby.YouTubePlugin.YouTubeManagedHttp.MaximumResponseBytes | [33:28](../YouTubeManagedHttp.cs#L33) |
| field | Emby.YouTubePlugin.YouTubeManagedHttp.MaximumTotalBytes | [34:28](../YouTubeManagedHttp.cs#L34) |
| field | Emby.YouTubePlugin.YouTubeManagedHttp._transport | [35:37](../YouTubeManagedHttp.cs#L35) |
| field | Emby.YouTubePlugin.YouTubeManagedHttp._videoId | [36:33](../YouTubeManagedHttp.cs#L36) |
| field | Emby.YouTubePlugin.YouTubeManagedHttp._maximumRequests | [37:30](../YouTubeManagedHttp.cs#L37) |
| field | Emby.YouTubePlugin.YouTubeManagedHttp._usedEndpoints | [38:42](../YouTubeManagedHttp.cs#L38) |
| field | Emby.YouTubePlugin.YouTubeManagedHttp._admitted | [39:22](../YouTubeManagedHttp.cs#L39) |
| field | Emby.YouTubePlugin.YouTubeManagedHttp._terminal | [40:22](../YouTubeManagedHttp.cs#L40) |
| field | Emby.YouTubePlugin.YouTubeManagedHttp._requests | [41:21](../YouTubeManagedHttp.cs#L41) |
| field | Emby.YouTubePlugin.YouTubeManagedHttp._responseBytes | [42:22](../YouTubeManagedHttp.cs#L42) |
| field | Emby.YouTubePlugin.YouTubeManagedHttp._stage | [43:24](../YouTubeManagedHttp.cs#L43) |
| constructor | Emby.YouTubePlugin.YouTubeManagedHttp.YouTubeManagedHttp | [45:9](../YouTubeManagedHttp.cs#L45) |
| parameter | Emby.YouTubePlugin.YouTubeManagedHttp.YouTubeManagedHttp.videoId | [45:37](../YouTubeManagedHttp.cs#L45) |
| parameter | Emby.YouTubePlugin.YouTubeManagedHttp.YouTubeManagedHttp.transport | [45:53](../YouTubeManagedHttp.cs#L45) |
| parameter | Emby.YouTubePlugin.YouTubeManagedHttp.YouTubeManagedHttp.maximumRequests | [45:75](../YouTubeManagedHttp.cs#L45) |
| property | Emby.YouTubePlugin.YouTubeManagedHttp.Terminal | [48:9](../YouTubeManagedHttp.cs#L48) |
| property | Emby.YouTubePlugin.YouTubeManagedHttp.RequestCount | [49:9](../YouTubeManagedHttp.cs#L49) |
| property | Emby.YouTubePlugin.YouTubeManagedHttp.ResponseBytes | [50:9](../YouTubeManagedHttp.cs#L50) |
| method | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync | [52:9](../YouTubeManagedHttp.cs#L52) |
| parameter | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.request | [52:70](../YouTubeManagedHttp.cs#L52) |
| parameter | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.cancellationToken | [52:98](../YouTubeManagedHttp.cs#L52) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.endpoint | [57:24](../YouTubeManagedHttp.cs#L57) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.deadline | [61:27](../YouTubeManagedHttp.cs#L61) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.forwarded | [63:27](../YouTubeManagedHttp.cs#L63) |
| iteration-variable | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.header | [64:17](../YouTubeManagedHttp.cs#L64) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.content | [74:28](../YouTubeManagedHttp.cs#L74) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.sent | [80:44](../YouTubeManagedHttp.cs#L80) |
| anonymous-function | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.anonymous@4313 | [84:60](../YouTubeManagedHttp.cs#L84) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.response | [96:27](../YouTubeManagedHttp.cs#L96) |
| anonymous-function | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.anonymous@5471 | [102:66](../YouTubeManagedHttp.cs#L102) |
| parameter | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.anonymous@5471.value | [102:66](../YouTubeManagedHttp.cs#L102) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.mediaType | [104:25](../YouTubeManagedHttp.cs#L104) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.maximum | [107:21](../YouTubeManagedHttp.cs#L107) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.announced | [108:68](../YouTubeManagedHttp.cs#L108) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.body | [110:27](../YouTubeManagedHttp.cs#L110) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.input | [112:27](../YouTubeManagedHttp.cs#L112) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.buffer | [113:21](../YouTubeManagedHttp.cs#L113) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.read | [116:25](../YouTubeManagedHttp.cs#L116) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.length | [122:88](../YouTubeManagedHttp.cs#L122) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.bytes | [124:24](../YouTubeManagedHttp.cs#L124) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.bounded | [126:21](../YouTubeManagedHttp.cs#L126) |
| catch-variable | Emby.YouTubePlugin.YouTubeManagedHttp.SendAsync.error | [138:19](../YouTubeManagedHttp.cs#L138) |
| method | Emby.YouTubePlugin.YouTubeManagedHttp.ValidateRequest | [142:9](../YouTubeManagedHttp.cs#L142) |
| parameter | Emby.YouTubePlugin.YouTubeManagedHttp.ValidateRequest.request | [142:40](../YouTubeManagedHttp.cs#L142) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.ValidateRequest.uri | [144:17](../YouTubeManagedHttp.cs#L144) |
| method | Emby.YouTubePlugin.YouTubeManagedHttp.ValidatePlayerRequest | [157:9](../YouTubeManagedHttp.cs#L157) |
| parameter | Emby.YouTubePlugin.YouTubeManagedHttp.ValidatePlayerRequest.content | [157:44](../YouTubeManagedHttp.cs#L157) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.ValidatePlayerRequest.document | [159:23](../YouTubeManagedHttp.cs#L159) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.ValidatePlayerRequest.value | [160:17](../YouTubeManagedHttp.cs#L160) |
| method | Emby.YouTubePlugin.YouTubeManagedHttp.ValidatePlayerResponse | [168:9](../YouTubeManagedHttp.cs#L168) |
| parameter | Emby.YouTubePlugin.YouTubeManagedHttp.ValidatePlayerResponse.content | [168:45](../YouTubeManagedHttp.cs#L168) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.ValidatePlayerResponse.document | [170:23](../YouTubeManagedHttp.cs#L170) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.ValidatePlayerResponse.root | [171:17](../YouTubeManagedHttp.cs#L171) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedHttp.ValidatePlayerResponse.status | [172:67](../YouTubeManagedHttp.cs#L172) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedHttp.ValidatePlayerResponse.field | [172:119](../YouTubeManagedHttp.cs#L172) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedHttp.ValidatePlayerResponse.bot | [179:22](../YouTubeManagedHttp.cs#L179) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedHttp.ValidatePlayerResponse.bot.reason | [179:68](../YouTubeManagedHttp.cs#L179) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedHttp.ValidatePlayerResponse.video | [183:62](../YouTubeManagedHttp.cs#L183) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedHttp.ValidatePlayerResponse.id | [183:113](../YouTubeManagedHttp.cs#L183) |
| method | Emby.YouTubePlugin.YouTubeManagedHttp.Deny | [188:9](../YouTubeManagedHttp.cs#L188) |
| parameter | Emby.YouTubePlugin.YouTubeManagedHttp.Deny.verification | [188:27](../YouTubeManagedHttp.cs#L188) |
| parameter | Emby.YouTubePlugin.YouTubeManagedHttp.Deny.globalCooldown | [188:46](../YouTubeManagedHttp.cs#L188) |
| method | Emby.YouTubePlugin.YouTubeManagedHttp.Failure | [195:9](../YouTubeManagedHttp.cs#L195) |
| parameter | Emby.YouTubePlugin.YouTubeManagedHttp.Failure.kind | [195:59](../YouTubeManagedHttp.cs#L195) |
| method | Emby.YouTubePlugin.YouTubeManagedHttp.Dispose | [196:9](../YouTubeManagedHttp.cs#L196) |
| parameter | Emby.YouTubePlugin.YouTubeManagedHttp.Dispose.disposing | [196:41](../YouTubeManagedHttp.cs#L196) |

</details>

<a id="file-d1109fcde8c29d7a"></a>
## [YouTubeManagedResolver.cs](../YouTubeManagedResolver.cs)

Plugin Compile source (SDK default); 21294 bytes; 321 lines.

<details>
<summary>All 182 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [14:1](../YouTubeManagedResolver.cs#L14) |
| type | Emby.YouTubePlugin.YouTubeManagedResolver | [18:5](../YouTubeManagedResolver.cs#L18) |
| field | Emby.YouTubePlugin.YouTubeManagedResolver.Revision | [20:31](../YouTubeManagedResolver.cs#L20) |
| field | Emby.YouTubePlugin.YouTubeManagedResolver.MaximumOutputBytes | [21:28](../YouTubeManagedResolver.cs#L21) |
| field | Emby.YouTubePlugin.YouTubeManagedResolver.MaximumFormats | [22:27](../YouTubeManagedResolver.cs#L22) |
| field | Emby.YouTubePlugin.YouTubeManagedResolver.MaximumDuration | [23:30](../YouTubeManagedResolver.cs#L23) |
| field | Emby.YouTubePlugin.YouTubeManagedResolver.LanguagePattern | [24:39](../YouTubeManagedResolver.cs#L24) |
| field | Emby.YouTubePlugin.YouTubeManagedResolver.IdentityPattern | [25:39](../YouTubeManagedResolver.cs#L25) |
| field | Emby.YouTubePlugin.YouTubeManagedResolver.InitialDataPattern | [26:39](../YouTubeManagedResolver.cs#L26) |
| field | Emby.YouTubePlugin.YouTubeManagedResolver._transportFactory | [27:41](../YouTubeManagedResolver.cs#L27) |
| method | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync | [29:9](../YouTubeManagedResolver.cs#L29) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync.videoId | [29:57](../YouTubeManagedResolver.cs#L29) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync.cancellationToken | [29:73](../YouTubeManagedResolver.cs#L29) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync.deadline | [31:23](../YouTubeManagedResolver.cs#L31) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync.guarded | [33:23](../YouTubeManagedResolver.cs#L33) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync.http | [34:23](../YouTubeManagedResolver.cs#L34) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync.controller | [35:17](../YouTubeManagedResolver.cs#L35) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync.response | [36:17](../YouTubeManagedResolver.cs#L36) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync.result | [37:17](../YouTubeManagedResolver.cs#L37) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync.duration | [38:17](../YouTubeManagedResolver.cs#L38) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync.duration.durationValue | [38:67](../YouTubeManagedResolver.cs#L38) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync.duration.seconds | [38:109](../YouTubeManagedResolver.cs#L38) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync.chapters | [39:17](../YouTubeManagedResolver.cs#L39) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync.optional | [44:27](../YouTubeManagedResolver.cs#L44) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync.request | [48:31](../YouTubeManagedResolver.cs#L48) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync.watch | [51:31](../YouTubeManagedResolver.cs#L51) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync.html | [52:28](../YouTubeManagedResolver.cs#L52) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync.initial | [53:25](../YouTubeManagedResolver.cs#L53) |
| catch-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync.error | [56:23](../YouTubeManagedResolver.cs#L56) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ResolveAsync.json | [65:20](../YouTubeManagedResolver.cs#L65) |
| method | Emby.YouTubePlugin.YouTubeManagedResolver.ConvertSnapshot | [70:9](../YouTubeManagedResolver.cs#L70) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.ConvertSnapshot.videoId | [70:69](../YouTubeManagedResolver.cs#L70) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.ConvertSnapshot.root | [70:85](../YouTubeManagedResolver.cs#L70) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ConvertSnapshot.player | [74:17](../YouTubeManagedResolver.cs#L74) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ConvertSnapshot.details | [76:17](../YouTubeManagedResolver.cs#L76) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ConvertSnapshot.streaming | [77:17](../YouTubeManagedResolver.cs#L77) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ConvertSnapshot.micro | [78:17](../YouTubeManagedResolver.cs#L78) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ConvertSnapshot.live | [79:18](../YouTubeManagedResolver.cs#L79) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ConvertSnapshot.result | [81:17](../YouTubeManagedResolver.cs#L81) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ConvertSnapshot.duration | [88:71](../YouTubeManagedResolver.cs#L88) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ConvertSnapshot.description | [89:61](../YouTubeManagedResolver.cs#L89) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ConvertSnapshot.title | [90:50](../YouTubeManagedResolver.cs#L90) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ConvertSnapshot.views | [91:84](../YouTubeManagedResolver.cs#L91) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ConvertSnapshot.hls | [92:61](../YouTubeManagedResolver.cs#L92) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ConvertSnapshot.chapters | [95:17](../YouTubeManagedResolver.cs#L95) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ConvertSnapshot.chapters.value | [95:86](../YouTubeManagedResolver.cs#L95) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ConvertSnapshot.chapters.seconds | [95:112](../YouTubeManagedResolver.cs#L95) |
| method | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats | [100:9](../YouTubeManagedResolver.cs#L100) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.streaming | [100:70](../YouTubeManagedResolver.cs#L100) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.formats | [102:17](../YouTubeManagedResolver.cs#L102) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.identities | [103:17](../YouTubeManagedResolver.cs#L103) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.considered | [104:17](../YouTubeManagedResolver.cs#L104) |
| iteration-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field | [105:13](../YouTubeManagedResolver.cs#L105) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.array | [107:21](../YouTubeManagedResolver.cs#L107) |
| iteration-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry | [110:17](../YouTubeManagedResolver.cs#L110) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.data | [116:29](../YouTubeManagedResolver.cs#L116) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.itag | [118:50](../YouTubeManagedResolver.cs#L118) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.length | [118:102](../YouTubeManagedResolver.cs#L118) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.uri | [119:29](../YouTubeManagedResolver.cs#L119) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.container | [121:33](../YouTubeManagedResolver.cs#L121) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.videoCodec | [122:33](../YouTubeManagedResolver.cs#L122) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.audioCodec | [122:63](../YouTubeManagedResolver.cs#L122) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.id | [127:32](../YouTubeManagedResolver.cs#L127) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.trackId | [128:81](../YouTubeManagedResolver.cs#L128) |
| anonymous-function | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.anonymous@9302 | [141:47](../YouTubeManagedResolver.cs#L141) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.anonymous@9302.item | [141:47](../YouTubeManagedResolver.cs#L141) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.format | [144:29](../YouTubeManagedResolver.cs#L144) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.bitrate | [151:50](../YouTubeManagedResolver.cs#L151) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.width | [152:52](../YouTubeManagedResolver.cs#L152) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.height | [153:53](../YouTubeManagedResolver.cs#L153) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.fps | [154:56](../YouTubeManagedResolver.cs#L154) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.language | [155:62](../YouTubeManagedResolver.cs#L155) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.isDefault | [156:65](../YouTubeManagedResolver.cs#L156) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.channels | [157:68](../YouTubeManagedResolver.cs#L157) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.rate | [158:101](../YouTubeManagedResolver.cs#L158) |
| catch-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadFormats.field.entry.error | [162:27](../YouTubeManagedResolver.cs#L162) |
| method | Emby.YouTubePlugin.YouTubeManagedResolver.ValidateLength | [172:9](../YouTubeManagedResolver.cs#L172) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.ValidateLength.uri | [172:45](../YouTubeManagedResolver.cs#L172) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.ValidateLength.length | [172:54](../YouTubeManagedResolver.cs#L172) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ValidateLength.value | [174:19](../YouTubeManagedResolver.cs#L174) |
| iteration-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ValidateLength.pair | [175:13](../YouTubeManagedResolver.cs#L175) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ValidateLength.pair.pieces | [177:21](../YouTubeManagedResolver.cs#L177) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ValidateLength.pair.name | [178:24](../YouTubeManagedResolver.cs#L178) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ValidateLength.pair.parsed | [182:76](../YouTubeManagedResolver.cs#L182) |
| method | Emby.YouTubePlugin.YouTubeManagedResolver.RawQuery | [190:9](../YouTubeManagedResolver.cs#L190) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.RawQuery.uri | [190:41](../YouTubeManagedResolver.cs#L190) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.RawQuery.offset | [192:17](../YouTubeManagedResolver.cs#L192) |
| method | Emby.YouTubePlugin.YouTubeManagedResolver.AddCaptions | [196:9](../YouTubeManagedResolver.cs#L196) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.AddCaptions.root | [196:41](../YouTubeManagedResolver.cs#L196) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.AddCaptions.target | [196:59](../YouTubeManagedResolver.cs#L196) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.AddCaptions.tracks | [198:17](../YouTubeManagedResolver.cs#L198) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.AddCaptions.accepted | [201:17](../YouTubeManagedResolver.cs#L201) |
| iteration-variable | Emby.YouTubePlugin.YouTubeManagedResolver.AddCaptions.track | [202:13](../YouTubeManagedResolver.cs#L202) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.AddCaptions.track.data | [205:21](../YouTubeManagedResolver.cs#L205) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.AddCaptions.track.language | [206:25](../YouTubeManagedResolver.cs#L206) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.AddCaptions.track.uri | [208:21](../YouTubeManagedResolver.cs#L208) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.AddCaptions.track.query | [210:21](../YouTubeManagedResolver.cs#L210) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.AddCaptions.track.duplicate | [211:22](../YouTubeManagedResolver.cs#L211) |
| iteration-variable | Emby.YouTubePlugin.YouTubeManagedResolver.AddCaptions.track.pair | [212:17](../YouTubeManagedResolver.cs#L212) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.AddCaptions.track.pair.parts | [214:25](../YouTubeManagedResolver.cs#L214) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.AddCaptions.track.actualLanguage | [217:98](../YouTubeManagedResolver.cs#L217) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.AddCaptions.track.kind | [219:80](../YouTubeManagedResolver.cs#L219) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.AddCaptions.track.url | [220:24](../YouTubeManagedResolver.cs#L220) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.AddCaptions.track.group | [222:21](../YouTubeManagedResolver.cs#L222) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.AddCaptions.track.key | [223:24](../YouTubeManagedResolver.cs#L223) |
| method | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters | [229:9](../YouTubeManagedResolver.cs#L229) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.root | [229:72](../YouTubeManagedResolver.cs#L229) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.duration | [229:90](../YouTubeManagedResolver.cs#L229) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.found | [231:17](../YouTubeManagedResolver.cs#L231) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.nodes | [234:17](../YouTubeManagedResolver.cs#L234) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.exceeded | [235:18](../YouTubeManagedResolver.cs#L235) |
| local-function | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.Visit | [236:13](../YouTubeManagedResolver.cs#L236) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.Visit.value | [236:24](../YouTubeManagedResolver.cs#L236) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.Visit.depth | [236:43](../YouTubeManagedResolver.cs#L236) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.Visit.chapterPanel | [236:54](../YouTubeManagedResolver.cs#L236) |
| iteration-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.Visit.item | [240:19](../YouTubeManagedResolver.cs#L240) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.Visit.targetId | [242:25](../YouTubeManagedResolver.cs#L242) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.Visit.knownPanel | [243:22](../YouTubeManagedResolver.cs#L243) |
| iteration-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.Visit.property | [244:17](../YouTubeManagedResolver.cs#L244) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.Visit.property.marker | [246:25](../YouTubeManagedResolver.cs#L246) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.Visit.property.millis | [248:73](../YouTubeManagedResolver.cs#L248) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.Visit.property.watch | [252:29](../YouTubeManagedResolver.cs#L252) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.Visit.property.seconds | [253:69](../YouTubeManagedResolver.cs#L253) |
| local-function | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.Add | [259:13](../YouTubeManagedResolver.cs#L259) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.Add.milliseconds | [259:22](../YouTubeManagedResolver.cs#L259) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.Add.name | [259:41](../YouTubeManagedResolver.cs#L259) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.Add.cleaned | [262:24](../YouTubeManagedResolver.cs#L262) |
| anonymous-function | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.Add.cleaned.anonymous@17238 | [262:63](../YouTubeManagedResolver.cs#L262) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.Add.cleaned.anonymous@17238.c | [262:63](../YouTubeManagedResolver.cs#L262) |
| anonymous-function | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.anonymous@17553 | [267:33](../YouTubeManagedResolver.cs#L267) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.ReadChapters.anonymous@17553.pair | [267:33](../YouTubeManagedResolver.cs#L267) |
| method | Emby.YouTubePlugin.YouTubeManagedResolver.ParseInitialData | [271:9](../YouTubeManagedResolver.cs#L271) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.ParseInitialData.html | [271:55](../YouTubeManagedResolver.cs#L271) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ParseInitialData.match | [274:17](../YouTubeManagedResolver.cs#L274) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ParseInitialData.start | [276:17](../YouTubeManagedResolver.cs#L276) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ParseInitialData.depth | [278:17](../YouTubeManagedResolver.cs#L278) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ParseInitialData.quoted | [278:33](../YouTubeManagedResolver.cs#L278) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ParseInitialData.escaped | [278:49](../YouTubeManagedResolver.cs#L278) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ParseInitialData.i | [279:22](../YouTubeManagedResolver.cs#L279) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ParseInitialData.ch | [281:22](../YouTubeManagedResolver.cs#L281) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.ParseInitialData.document | [293:31](../YouTubeManagedResolver.cs#L293) |
| method | Emby.YouTubePlugin.YouTubeManagedResolver.Title | [300:9](../YouTubeManagedResolver.cs#L300) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.Title.value | [300:38](../YouTubeManagedResolver.cs#L300) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.Title.simple | [302:53](../YouTubeManagedResolver.cs#L302) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.Title.runs | [303:17](../YouTubeManagedResolver.cs#L303) |
| anonymous-function | Emby.YouTubePlugin.YouTubeManagedResolver.Title.anonymous@19312 | [305:63](../YouTubeManagedResolver.cs#L305) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.Title.anonymous@19312.run | [305:63](../YouTubeManagedResolver.cs#L305) |
| method | Emby.YouTubePlugin.YouTubeManagedResolver.Property | [307:9](../YouTubeManagedResolver.cs#L307) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.Property.value | [307:45](../YouTubeManagedResolver.cs#L307) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.Property.name | [307:64](../YouTubeManagedResolver.cs#L307) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.Property.found | [307:158](../YouTubeManagedResolver.cs#L307) |
| method | Emby.YouTubePlugin.YouTubeManagedResolver.Text | [308:9](../YouTubeManagedResolver.cs#L308) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.Text.value | [308:37](../YouTubeManagedResolver.cs#L308) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.Text.name | [308:56](../YouTubeManagedResolver.cs#L308) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.Text.found | [308:101](../YouTubeManagedResolver.cs#L308) |
| method | Emby.YouTubePlugin.YouTubeManagedResolver.Boolean | [309:9](../YouTubeManagedResolver.cs#L309) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.Boolean.value | [309:38](../YouTubeManagedResolver.cs#L309) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.Boolean.name | [309:57](../YouTubeManagedResolver.cs#L309) |
| method | Emby.YouTubePlugin.YouTubeManagedResolver.Integer | [310:9](../YouTubeManagedResolver.cs#L310) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.Integer.value | [310:37](../YouTubeManagedResolver.cs#L310) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.Integer.name | [310:56](../YouTubeManagedResolver.cs#L310) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.Integer.found | [310:101](../YouTubeManagedResolver.cs#L310) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.Integer.number | [310:179](../YouTubeManagedResolver.cs#L310) |
| method | Emby.YouTubePlugin.YouTubeManagedResolver.Long | [311:9](../YouTubeManagedResolver.cs#L311) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.Long.value | [311:35](../YouTubeManagedResolver.cs#L311) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.Long.name | [311:54](../YouTubeManagedResolver.cs#L311) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.Long.found | [311:99](../YouTubeManagedResolver.cs#L311) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.Long.number | [311:178](../YouTubeManagedResolver.cs#L311) |
| method | Emby.YouTubePlugin.YouTubeManagedResolver.CanonicalLong | [312:9](../YouTubeManagedResolver.cs#L312) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.CanonicalLong.value | [312:44](../YouTubeManagedResolver.cs#L312) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.CanonicalLong.allowZero | [312:59](../YouTubeManagedResolver.cs#L312) |
| anonymous-function | Emby.YouTubePlugin.YouTubeManagedResolver.CanonicalLong.anonymous@20454 | [313:26](../YouTubeManagedResolver.cs#L313) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.CanonicalLong.anonymous@20454.ch | [313:26](../YouTubeManagedResolver.cs#L313) |
| pattern-variable | Emby.YouTubePlugin.YouTubeManagedResolver.CanonicalLong.number | [314:95](../YouTubeManagedResolver.cs#L314) |
| method | Emby.YouTubePlugin.YouTubeManagedResolver.TrySeconds | [315:9](../YouTubeManagedResolver.cs#L315) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.TrySeconds.value | [315:40](../YouTubeManagedResolver.cs#L315) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.TrySeconds.seconds | [315:55](../YouTubeManagedResolver.cs#L315) |
| method | Emby.YouTubePlugin.YouTubeManagedResolver.Limit | [317:9](../YouTubeManagedResolver.cs#L317) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.Limit.value | [317:37](../YouTubeManagedResolver.cs#L317) |
| parameter | Emby.YouTubePlugin.YouTubeManagedResolver.Limit.maximum | [317:51](../YouTubeManagedResolver.cs#L317) |
| local-variable | Emby.YouTubePlugin.YouTubeManagedResolver.Limit.length | [318:15](../YouTubeManagedResolver.cs#L318) |
| method | Emby.YouTubePlugin.YouTubeManagedResolver.Failure | [319:9](../YouTubeManagedResolver.cs#L319) |

</details>

<a id="file-7a9c7096d206cd40"></a>
## [YouTubeRequestBackoff.cs](../YouTubeRequestBackoff.cs)

Plugin Compile source (SDK default); 3951 bytes; 85 lines.

<details>
<summary>All 27 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [6:1](../YouTubeRequestBackoff.cs#L6) |
| type | Emby.YouTubePlugin.YouTubeRequestBackoff | [10:5](../YouTubeRequestBackoff.cs#L10) |
| field | Emby.YouTubePlugin.YouTubeRequestBackoff.Gate | [12:40](../YouTubeRequestBackoff.cs#L12) |
| field | Emby.YouTubePlugin.YouTubeRequestBackoff.MinimumLaunchSpacingMilliseconds | [13:28](../YouTubeRequestBackoff.cs#L13) |
| field | Emby.YouTubePlugin.YouTubeRequestBackoff.MaximumStartsPerHour | [14:28](../YouTubeRequestBackoff.cs#L14) |
| field | Emby.YouTubePlugin.YouTubeRequestBackoff.HourMilliseconds | [15:28](../YouTubeRequestBackoff.cs#L15) |
| field | Emby.YouTubePlugin.YouTubeRequestBackoff.LaunchGate | [16:47](../YouTubeRequestBackoff.cs#L16) |
| field | Emby.YouTubePlugin.YouTubeRequestBackoff.Starts | [17:45](../YouTubeRequestBackoff.cs#L17) |
| field | Emby.YouTubePlugin.YouTubeRequestBackoff._clockMilliseconds | [18:35](../YouTubeRequestBackoff.cs#L18) |
| anonymous-function | Emby.YouTubePlugin.YouTubeRequestBackoff._clockMilliseconds.anonymous@760 | [18:56](../YouTubeRequestBackoff.cs#L18) |
| field | Emby.YouTubePlugin.YouTubeRequestBackoff._delay | [19:64](../YouTubeRequestBackoff.cs#L19) |
| field | Emby.YouTubePlugin.YouTubeRequestBackoff._untilMilliseconds | [20:29](../YouTubeRequestBackoff.cs#L20) |
| field | Emby.YouTubePlugin.YouTubeRequestBackoff._lastDeniedMilliseconds | [21:29](../YouTubeRequestBackoff.cs#L21) |
| field | Emby.YouTubePlugin.YouTubeRequestBackoff._lastStartMilliseconds | [22:30](../YouTubeRequestBackoff.cs#L22) |
| field | Emby.YouTubePlugin.YouTubeRequestBackoff._strikes | [23:28](../YouTubeRequestBackoff.cs#L23) |
| method | Emby.YouTubePlugin.YouTubeRequestBackoff.IsBlocked | [25:9](../YouTubeRequestBackoff.cs#L25) |
| method | Emby.YouTubePlugin.YouTubeRequestBackoff.Record | [30:9](../YouTubeRequestBackoff.cs#L30) |
| parameter | Emby.YouTubePlugin.YouTubeRequestBackoff.Record.verificationRequired | [30:37](../YouTubeRequestBackoff.cs#L30) |
| local-variable | Emby.YouTubePlugin.YouTubeRequestBackoff.Record.now | [34:21](../YouTubeRequestBackoff.cs#L34) |
| local-variable | Emby.YouTubePlugin.YouTubeRequestBackoff.Record.delay | [38:21](../YouTubeRequestBackoff.cs#L38) |
| method | Emby.YouTubePlugin.YouTubeRequestBackoff.StartAsync | [47:9](../YouTubeRequestBackoff.cs#L47) |
| parameter | Emby.YouTubePlugin.YouTubeRequestBackoff.StartAsync.start | [47:53](../YouTubeRequestBackoff.cs#L47) |
| parameter | Emby.YouTubePlugin.YouTubeRequestBackoff.StartAsync.cancellationToken | [47:71](../YouTubeRequestBackoff.cs#L47) |
| local-variable | Emby.YouTubePlugin.YouTubeRequestBackoff.StartAsync.wait | [54:26](../YouTubeRequestBackoff.cs#L54) |
| local-variable | Emby.YouTubePlugin.YouTubeRequestBackoff.StartAsync.now | [58:29](../YouTubeRequestBackoff.cs#L58) |
| local-variable | Emby.YouTubePlugin.YouTubeRequestBackoff.StartAsync.started | [68:34](../YouTubeRequestBackoff.cs#L68) |
| local-variable | Emby.YouTubePlugin.YouTubeRequestBackoff.StartAsync.launched | [71:37](../YouTubeRequestBackoff.cs#L71) |

</details>

<a id="file-8ca449cb8064e640"></a>
## [YouTubeSessionCommandRouter.cs](../YouTubeSessionCommandRouter.cs)

Plugin Compile source (SDK default); 5165 bytes; 121 lines.

<details>
<summary>All 36 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [5:1](../YouTubeSessionCommandRouter.cs#L5) |
| type | Emby.YouTubePlugin.YouTubeSessionCommandRouter | [7:5](../YouTubeSessionCommandRouter.cs#L7) |
| type | Emby.YouTubePlugin.YouTubeSessionCommandRouter.PlaybackItemMatch | [9:9](../YouTubeSessionCommandRouter.cs#L9) |
| enum-member | Emby.YouTubePlugin.YouTubeSessionCommandRouter.PlaybackItemMatch.Unavailable | [9:43](../YouTubeSessionCommandRouter.cs#L9) |
| enum-member | Emby.YouTubePlugin.YouTubeSessionCommandRouter.PlaybackItemMatch.Match | [9:56](../YouTubeSessionCommandRouter.cs#L9) |
| enum-member | Emby.YouTubePlugin.YouTubeSessionCommandRouter.PlaybackItemMatch.Mismatch | [9:63](../YouTubeSessionCommandRouter.cs#L9) |
| method | Emby.YouTubePlugin.YouTubeSessionCommandRouter.MatchPlaybackItem | [11:9](../YouTubeSessionCommandRouter.cs#L11) |
| parameter | Emby.YouTubePlugin.YouTubeSessionCommandRouter.MatchPlaybackItem.session | [11:61](../YouTubeSessionCommandRouter.cs#L11) |
| parameter | Emby.YouTubePlugin.YouTubeSessionCommandRouter.MatchPlaybackItem.itemId | [11:82](../YouTubeSessionCommandRouter.cs#L11) |
| local-variable | Emby.YouTubePlugin.YouTubeSessionCommandRouter.MatchPlaybackItem.fullItem | [16:17](../YouTubeSessionCommandRouter.cs#L16) |
| local-variable | Emby.YouTubePlugin.YouTubeSessionCommandRouter.MatchPlaybackItem.dtoId | [20:17](../YouTubeSessionCommandRouter.cs#L20) |
| pattern-variable | Emby.YouTubePlugin.YouTubeSessionCommandRouter.MatchPlaybackItem.parsedId | [24:49](../YouTubeSessionCommandRouter.cs#L24) |
| method | Emby.YouTubePlugin.YouTubeSessionCommandRouter.TryResolve | [33:9](../YouTubeSessionCommandRouter.cs#L33) |
| parameter | Emby.YouTubePlugin.YouTubeSessionCommandRouter.TryResolve.manager | [34:13](../YouTubeSessionCommandRouter.cs#L34) |
| parameter | Emby.YouTubePlugin.YouTubeSessionCommandRouter.TryResolve.playbackSessionId | [35:13](../YouTubeSessionCommandRouter.cs#L35) |
| parameter | Emby.YouTubePlugin.YouTubeSessionCommandRouter.TryResolve.userId | [36:13](../YouTubeSessionCommandRouter.cs#L36) |
| parameter | Emby.YouTubePlugin.YouTubeSessionCommandRouter.TryResolve.itemId | [37:13](../YouTubeSessionCommandRouter.cs#L37) |
| parameter | Emby.YouTubePlugin.YouTubeSessionCommandRouter.TryResolve.messageName | [38:13](../YouTubeSessionCommandRouter.cs#L38) |
| parameter | Emby.YouTubePlugin.YouTubeSessionCommandRouter.TryResolve.commandSessionId | [39:13](../YouTubeSessionCommandRouter.cs#L39) |
| local-variable | Emby.YouTubePlugin.YouTubeSessionCommandRouter.TryResolve.sessions | [46:17](../YouTubeSessionCommandRouter.cs#L46) |
| local-variable | Emby.YouTubePlugin.YouTubeSessionCommandRouter.TryResolve.playback | [47:17](../YouTubeSessionCommandRouter.cs#L47) |
| anonymous-function | Emby.YouTubePlugin.YouTubeSessionCommandRouter.TryResolve.playback.anonymous@1841 | [47:52](../YouTubeSessionCommandRouter.cs#L47) |
| parameter | Emby.YouTubePlugin.YouTubeSessionCommandRouter.TryResolve.playback.anonymous@1841.s | [47:52](../YouTubeSessionCommandRouter.cs#L47) |
| local-variable | Emby.YouTubePlugin.YouTubeSessionCommandRouter.TryResolve.bridge | [64:26](../YouTubeSessionCommandRouter.cs#L64) |
| iteration-variable | Emby.YouTubePlugin.YouTubeSessionCommandRouter.TryResolve.candidate | [65:13](../YouTubeSessionCommandRouter.cs#L65) |
| method | Emby.YouTubePlugin.YouTubeSessionCommandRouter.IsNativeBridgePair | [91:9](../YouTubeSessionCommandRouter.cs#L91) |
| parameter | Emby.YouTubePlugin.YouTubeSessionCommandRouter.IsNativeBridgePair.first | [91:48](../YouTubeSessionCommandRouter.cs#L91) |
| parameter | Emby.YouTubePlugin.YouTubeSessionCommandRouter.IsNativeBridgePair.second | [91:63](../YouTubeSessionCommandRouter.cs#L91) |
| method | Emby.YouTubePlugin.YouTubeSessionCommandRouter.IsAndroidWebLayer | [95:9](../YouTubeSessionCommandRouter.cs#L95) |
| parameter | Emby.YouTubePlugin.YouTubeSessionCommandRouter.IsAndroidWebLayer.client | [95:47](../YouTubeSessionCommandRouter.cs#L95) |
| method | Emby.YouTubePlugin.YouTubeSessionCommandRouter.IsAndroidBridge | [99:9](../YouTubeSessionCommandRouter.cs#L99) |
| parameter | Emby.YouTubePlugin.YouTubeSessionCommandRouter.IsAndroidBridge.client | [99:45](../YouTubeSessionCommandRouter.cs#L99) |
| method | Emby.YouTubePlugin.YouTubeSessionCommandRouter.HasActiveTransport | [102:9](../YouTubeSessionCommandRouter.cs#L102) |
| parameter | Emby.YouTubePlugin.YouTubeSessionCommandRouter.HasActiveTransport.session | [102:48](../YouTubeSessionCommandRouter.cs#L102) |
| parameter | Emby.YouTubePlugin.YouTubeSessionCommandRouter.HasActiveTransport.messageName | [102:69](../YouTubeSessionCommandRouter.cs#L102) |
| iteration-variable | Emby.YouTubePlugin.YouTubeSessionCommandRouter.HasActiveTransport.controller | [106:13](../YouTubeSessionCommandRouter.cs#L106) |

</details>

<a id="file-ff64b0c5af701f8e"></a>
## [YouTubeSortNameRepairer.cs](../YouTubeSortNameRepairer.cs)

Plugin Compile source (SDK default); 8281 bytes; 230 lines.

<details>
<summary>All 57 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [10:1](../YouTubeSortNameRepairer.cs#L10) |
| type | Emby.YouTubePlugin.YouTubeSortNameRepairer | [12:5](../YouTubeSortNameRepairer.cs#L12) |
| field | Emby.YouTubePlugin.YouTubeSortNameRepairer.UpdateSpacing | [14:42](../YouTubeSortNameRepairer.cs#L14) |
| field | Emby.YouTubePlugin.YouTubeSortNameRepairer.RetryBaseDelay | [15:42](../YouTubeSortNameRepairer.cs#L15) |
| field | Emby.YouTubePlugin.YouTubeSortNameRepairer.RefreshPollDelay | [16:42](../YouTubeSortNameRepairer.cs#L16) |
| field | Emby.YouTubePlugin.YouTubeSortNameRepairer.MaxAttempts | [17:27](../YouTubeSortNameRepairer.cs#L17) |
| field | Emby.YouTubePlugin.YouTubeSortNameRepairer.SortDescendingFrom | [18:28](../YouTubeSortNameRepairer.cs#L18) |
| field | Emby.YouTubePlugin.YouTubeSortNameRepairer.DayPrefixLength | [19:27](../YouTubeSortNameRepairer.cs#L19) |
| field | Emby.YouTubePlugin.YouTubeSortNameRepairer.TimePrefixLength | [20:27](../YouTubeSortNameRepairer.cs#L20) |
| field | Emby.YouTubePlugin.YouTubeSortNameRepairer._pending | [22:63](../YouTubeSortNameRepairer.cs#L22) |
| field | Emby.YouTubePlugin.YouTubeSortNameRepairer._signal | [23:40](../YouTubeSortNameRepairer.cs#L23) |
| field | Emby.YouTubePlugin.YouTubeSortNameRepairer._cts | [24:50](../YouTubeSortNameRepairer.cs#L24) |
| field | Emby.YouTubePlugin.YouTubeSortNameRepairer._worker | [25:23](../YouTubeSortNameRepairer.cs#L25) |
| method | Emby.YouTubePlugin.YouTubeSortNameRepairer.Start | [27:9](../YouTubeSortNameRepairer.cs#L27) |
| method | Emby.YouTubePlugin.YouTubeSortNameRepairer.Enqueue | [32:9](../YouTubeSortNameRepairer.cs#L32) |
| parameter | Emby.YouTubePlugin.YouTubeSortNameRepairer.Enqueue.item | [32:29](../YouTubeSortNameRepairer.cs#L32) |
| method | Emby.YouTubePlugin.YouTubeSortNameRepairer.Enqueue | [45:9](../YouTubeSortNameRepairer.cs#L45) |
| parameter | Emby.YouTubePlugin.YouTubeSortNameRepairer.Enqueue.items | [45:28](../YouTubeSortNameRepairer.cs#L45) |
| local-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.Enqueue.count | [47:17](../YouTubeSortNameRepairer.cs#L47) |
| iteration-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.Enqueue.item | [48:13](../YouTubeSortNameRepairer.cs#L48) |
| method | Emby.YouTubePlugin.YouTubeSortNameRepairer.ProcessLoop | [57:9](../YouTubeSortNameRepairer.cs#L57) |
| local-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.ProcessLoop.ct | [59:17](../YouTubeSortNameRepairer.cs#L59) |
| pattern-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.ProcessLoop.item | [79:50](../YouTubeSortNameRepairer.cs#L79) |
| catch-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.ProcessLoop.ex | [90:23](../YouTubeSortNameRepairer.cs#L90) |
| method | Emby.YouTubePlugin.YouTubeSortNameRepairer.TryTakeNext | [97:9](../YouTubeSortNameRepairer.cs#L97) |
| parameter | Emby.YouTubePlugin.YouTubeSortNameRepairer.TryTakeNext.item | [97:34](../YouTubeSortNameRepairer.cs#L97) |
| iteration-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.TryTakeNext.key | [99:13](../YouTubeSortNameRepairer.cs#L99) |
| method | Emby.YouTubePlugin.YouTubeSortNameRepairer.RepairWithRetry | [109:9](../YouTubeSortNameRepairer.cs#L109) |
| parameter | Emby.YouTubePlugin.YouTubeSortNameRepairer.RepairWithRetry.item | [109:57](../YouTubeSortNameRepairer.cs#L109) |
| parameter | Emby.YouTubePlugin.YouTubeSortNameRepairer.RepairWithRetry.ct | [109:72](../YouTubeSortNameRepairer.cs#L109) |
| local-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.RepairWithRetry.attempt | [111:22](../YouTubeSortNameRepairer.cs#L111) |
| pattern-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.RepairWithRetry.desiredSortName | [115:57](../YouTubeSortNameRepairer.cs#L115) |
| catch-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.RepairWithRetry.ex | [122:23](../YouTubeSortNameRepairer.cs#L122) |
| local-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.RepairWithRetry.delay | [130:25](../YouTubeSortNameRepairer.cs#L130) |
| method | Emby.YouTubePlugin.YouTubeSortNameRepairer.TryApplySortName | [139:9](../YouTubeSortNameRepairer.cs#L139) |
| parameter | Emby.YouTubePlugin.YouTubeSortNameRepairer.TryApplySortName.item | [139:46](../YouTubeSortNameRepairer.cs#L139) |
| parameter | Emby.YouTubePlugin.YouTubeSortNameRepairer.TryApplySortName.desiredSortName | [139:61](../YouTubeSortNameRepairer.cs#L139) |
| local-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.TryApplySortName.videoId | [143:17](../YouTubeSortNameRepairer.cs#L143) |
| local-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.TryApplySortName.date | [147:17](../YouTubeSortNameRepairer.cs#L147) |
| local-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.TryApplySortName.currentSortName | [151:17](../YouTubeSortNameRepairer.cs#L151) |
| local-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.TryApplySortName.baseSortName | [152:17](../YouTubeSortNameRepairer.cs#L152) |
| local-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.TryApplySortName.utc | [160:17](../YouTubeSortNameRepairer.cs#L160) |
| local-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.TryApplySortName.dayNumber | [161:17](../YouTubeSortNameRepairer.cs#L161) |
| local-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.TryApplySortName.dayPrefix | [162:17](../YouTubeSortNameRepairer.cs#L162) |
| local-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.TryApplySortName.timePrefix | [164:17](../YouTubeSortNameRepairer.cs#L164) |
| method | Emby.YouTubePlugin.YouTubeSortNameRepairer.StripDatePrefix | [175:9](../YouTubeSortNameRepairer.cs#L175) |
| parameter | Emby.YouTubePlugin.YouTubeSortNameRepairer.StripDatePrefix.sortName | [175:47](../YouTubeSortNameRepairer.cs#L175) |
| local-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.StripDatePrefix.remainder | [180:17](../YouTubeSortNameRepairer.cs#L180) |
| method | Emby.YouTubePlugin.YouTubeSortNameRepairer.HasNumericPrefix | [191:9](../YouTubeSortNameRepairer.cs#L191) |
| parameter | Emby.YouTubePlugin.YouTubeSortNameRepairer.HasNumericPrefix.value | [191:46](../YouTubeSortNameRepairer.cs#L191) |
| parameter | Emby.YouTubePlugin.YouTubeSortNameRepairer.HasNumericPrefix.length | [191:60](../YouTubeSortNameRepairer.cs#L191) |
| local-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.HasNumericPrefix.i | [196:22](../YouTubeSortNameRepairer.cs#L196) |
| method | Emby.YouTubePlugin.YouTubeSortNameRepairer.LooksLikeTransientDatabaseLock | [205:9](../YouTubeSortNameRepairer.cs#L205) |
| parameter | Emby.YouTubePlugin.YouTubeSortNameRepairer.LooksLikeTransientDatabaseLock.ex | [205:60](../YouTubeSortNameRepairer.cs#L205) |
| local-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.LooksLikeTransientDatabaseLock.current | [207:22](../YouTubeSortNameRepairer.cs#L207) |
| local-variable | Emby.YouTubePlugin.YouTubeSortNameRepairer.LooksLikeTransientDatabaseLock.message | [209:21](../YouTubeSortNameRepairer.cs#L209) |
| method | Emby.YouTubePlugin.YouTubeSortNameRepairer.Dispose | [221:9](../YouTubeSortNameRepairer.cs#L221) |

</details>

<a id="file-28aab84e271eec1a"></a>
## [YouTubeSourceBudget.cs](../YouTubeSourceBudget.cs)

Plugin Compile source (SDK default); 13295 bytes; 224 lines.

<details>
<summary>All 92 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [10:1](../YouTubeSourceBudget.cs#L10) |
| type | Emby.YouTubePlugin.YouTubeSourceBudget | [15:5](../YouTubeSourceBudget.cs#L15) |
| field | Emby.YouTubePlugin.YouTubeSourceBudget.MaximumPlaylistBytes | [17:28](../YouTubeSourceBudget.cs#L17) |
| field | Emby.YouTubePlugin.YouTubeSourceBudget.MaximumPlaylistSegments | [18:28](../YouTubeSourceBudget.cs#L18) |
| field | Emby.YouTubePlugin.YouTubeSourceBudget.MaximumCacheEntries | [19:28](../YouTubeSourceBudget.cs#L19) |
| field | Emby.YouTubePlugin.YouTubeSourceBudget.MaximumPendingProbes | [20:28](../YouTubeSourceBudget.cs#L20) |
| field | Emby.YouTubePlugin.YouTubeSourceBudget.MaximumParallelProbes | [21:28](../YouTubeSourceBudget.cs#L21) |
| field | Emby.YouTubePlugin.YouTubeSourceBudget.ProbeDeadlineMilliseconds | [22:28](../YouTubeSourceBudget.cs#L22) |
| field | Emby.YouTubePlugin.YouTubeSourceBudget.FallbackMaximumHeight | [23:28](../YouTubeSourceBudget.cs#L23) |
| field | Emby.YouTubePlugin.YouTubeSourceBudget.ThroughputHeadroom | [24:31](../YouTubeSourceBudget.cs#L24) |
| field | Emby.YouTubePlugin.YouTubeSourceBudget.SuccessLifetime | [25:43](../YouTubeSourceBudget.cs#L25) |
| field | Emby.YouTubePlugin.YouTubeSourceBudget.FailureLifetime | [26:43](../YouTubeSourceBudget.cs#L26) |
| type | Emby.YouTubePlugin.YouTubeSourceBudget.Observation | [28:9](../YouTubeSourceBudget.cs#L28) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.Observation.Bitrate | [28:43](../YouTubeSourceBudget.cs#L28) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.Observation.Reason | [28:57](../YouTubeSourceBudget.cs#L28) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.Observation.ExpiresAtMilliseconds | [28:72](../YouTubeSourceBudget.cs#L28) |
| field | Emby.YouTubePlugin.YouTubeSourceBudget.CacheLock | [29:40](../YouTubeSourceBudget.cs#L29) |
| field | Emby.YouTubePlugin.YouTubeSourceBudget.Cache | [30:71](../YouTubeSourceBudget.cs#L30) |
| field | Emby.YouTubePlugin.YouTubeSourceBudget.ProbeSlots | [31:47](../YouTubeSourceBudget.cs#L31) |
| field | Emby.YouTubePlugin.YouTubeSourceBudget._pendingProbes | [32:28](../YouTubeSourceBudget.cs#L32) |
| field | Emby.YouTubePlugin.YouTubeSourceBudget._clockMilliseconds | [34:35](../YouTubeSourceBudget.cs#L34) |
| anonymous-function | Emby.YouTubePlugin.YouTubeSourceBudget._clockMilliseconds.anonymous@1735 | [34:56](../YouTubeSourceBudget.cs#L34) |
| field | Emby.YouTubePlugin.YouTubeSourceBudget._fetchPlaylist | [35:109](../YouTubeSourceBudget.cs#L35) |
| field | Emby.YouTubePlugin.YouTubeSourceBudget._probeMedia | [37:115](../YouTubeSourceBudget.cs#L37) |
| method | Emby.YouTubePlugin.YouTubeSourceBudget.SelectAsync | [40:9](../YouTubeSourceBudget.cs#L40) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.SelectAsync.masterText | [40:69](../YouTubeSourceBudget.cs#L40) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.SelectAsync.masterUri | [40:88](../YouTubeSourceBudget.cs#L40) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.SelectAsync.headers | [41:13](../YouTubeSourceBudget.cs#L41) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.SelectAsync.maximumHeight | [41:58](../YouTubeSourceBudget.cs#L41) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.SelectAsync.cancellationToken | [41:77](../YouTubeSourceBudget.cs#L41) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.SelectAsync.safeHeaders | [46:17](../YouTubeSourceBudget.cs#L46) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.SelectAsync.preferred | [53:17](../YouTubeSourceBudget.cs#L53) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.SelectAsync.lowest | [55:17](../YouTubeSourceBudget.cs#L55) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.SelectAsync.fallbackHeight | [57:17](../YouTubeSourceBudget.cs#L57) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.SelectAsync.fallback | [58:33](../YouTubeSourceBudget.cs#L58) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.SelectAsync.observation | [67:17](../YouTubeSourceBudget.cs#L67) |
| pattern-variable | Emby.YouTubePlugin.YouTubeSourceBudget.SelectAsync.bitrate | [70:47](../YouTubeSourceBudget.cs#L70) |
| method | Emby.YouTubePlugin.YouTubeSourceBudget.GetObservation | [77:9](../YouTubeSourceBudget.cs#L77) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.GetObservation.videoUri | [77:57](../YouTubeSourceBudget.cs#L77) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.GetObservation.headers | [77:71](../YouTubeSourceBudget.cs#L77) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.GetObservation.key | [79:17](../YouTubeSourceBudget.cs#L79) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.GetObservation.now | [82:21](../YouTubeSourceBudget.cs#L82) |
| iteration-variable | Emby.YouTubePlugin.YouTubeSourceBudget.GetObservation.expired | [83:17](../YouTubeSourceBudget.cs#L83) |
| anonymous-function | Emby.YouTubePlugin.YouTubeSourceBudget.GetObservation.expired.anonymous@4856 | [83:53](../YouTubeSourceBudget.cs#L83) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.GetObservation.expired.anonymous@4856.pair | [83:53](../YouTubeSourceBudget.cs#L83) |
| anonymous-function | Emby.YouTubePlugin.YouTubeSourceBudget.GetObservation.expired.anonymous@4977 | [84:79](../YouTubeSourceBudget.cs#L84) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.GetObservation.expired.anonymous@4977.pair | [84:79](../YouTubeSourceBudget.cs#L84) |
| pattern-variable | Emby.YouTubePlugin.YouTubeSourceBudget.GetObservation.cached | [85:52](../YouTubeSourceBudget.cs#L85) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.GetObservation.oldest | [88:25](../YouTubeSourceBudget.cs#L88) |
| anonymous-function | Emby.YouTubePlugin.YouTubeSourceBudget.GetObservation.oldest.anonymous@5223 | [88:46](../YouTubeSourceBudget.cs#L88) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.GetObservation.oldest.anonymous@5223.pair | [88:46](../YouTubeSourceBudget.cs#L88) |
| anonymous-function | Emby.YouTubePlugin.YouTubeSourceBudget.GetObservation.oldest.anonymous@5300 | [89:34](../YouTubeSourceBudget.cs#L89) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.GetObservation.oldest.anonymous@5300.pair | [89:34](../YouTubeSourceBudget.cs#L89) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.GetObservation.task | [93:35](../YouTubeSourceBudget.cs#L93) |
| method | Emby.YouTubePlugin.YouTubeSourceBudget.MeasureAsync | [105:9](../YouTubeSourceBudget.cs#L105) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.MeasureAsync.videoUri | [105:61](../YouTubeSourceBudget.cs#L105) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.MeasureAsync.headers | [105:75](../YouTubeSourceBudget.cs#L105) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.MeasureAsync.acquired | [107:18](../YouTubeSourceBudget.cs#L107) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.MeasureAsync.deadline | [108:23](../YouTubeSourceBudget.cs#L108) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.MeasureAsync.bytes | [117:21](../YouTubeSourceBudget.cs#L117) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.MeasureAsync.segment | [120:21](../YouTubeSourceBudget.cs#L120) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.MeasureAsync.sample | [122:21](../YouTubeSourceBudget.cs#L122) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.MeasureAsync.rate | [130:24](../YouTubeSourceBudget.cs#L130) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.MeasureAsync.budget | [131:21](../YouTubeSourceBudget.cs#L131) |
| method | Emby.YouTubePlugin.YouTubeSourceBudget.Fallback | [144:9](../YouTubeSourceBudget.cs#L144) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.Fallback.reason | [144:45](../YouTubeSourceBudget.cs#L144) |
| method | Emby.YouTubePlugin.YouTubeSourceBudget.CacheKey | [154:9](../YouTubeSourceBudget.cs#L154) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.CacheKey.uri | [154:40](../YouTubeSourceBudget.cs#L154) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.CacheKey.headers | [154:49](../YouTubeSourceBudget.cs#L154) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.CacheKey.route | [158:17](../YouTubeSourceBudget.cs#L158) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.CacheKey.identity | [159:17](../YouTubeSourceBudget.cs#L159) |
| anonymous-function | Emby.YouTubePlugin.YouTubeSourceBudget.CacheKey.identity.anonymous@9541 | [160:53](../YouTubeSourceBudget.cs#L160) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.CacheKey.identity.anonymous@9541.pair | [160:53](../YouTubeSourceBudget.cs#L160) |
| anonymous-function | Emby.YouTubePlugin.YouTubeSourceBudget.CacheKey.identity.anonymous@9621 | [161:29](../YouTubeSourceBudget.cs#L161) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.CacheKey.identity.anonymous@9621.pair | [161:29](../YouTubeSourceBudget.cs#L161) |
| method | Emby.YouTubePlugin.YouTubeSourceBudget.ParseProbeSegment | [165:9](../YouTubeSourceBudget.cs#L165) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.ParseProbeSegment.text | [165:47](../YouTubeSourceBudget.cs#L165) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.ParseProbeSegment.playlistUri | [165:60](../YouTubeSourceBudget.cs#L165) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.ParseProbeSegment.lines | [168:17](../YouTubeSourceBudget.cs#L168) |
| anonymous-function | Emby.YouTubePlugin.YouTubeSourceBudget.ParseProbeSegment.lines.anonymous@10095 | [169:37](../YouTubeSourceBudget.cs#L169) |
| parameter | Emby.YouTubePlugin.YouTubeSourceBudget.ParseProbeSegment.lines.anonymous@10095.line | [169:37](../YouTubeSourceBudget.cs#L169) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.ParseProbeSegment.first | [171:18](../YouTubeSourceBudget.cs#L171) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.ParseProbeSegment.durationPending | [172:18](../YouTubeSourceBudget.cs#L172) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.ParseProbeSegment.targetDuration | [172:43](../YouTubeSourceBudget.cs#L172) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.ParseProbeSegment.ended | [172:67](../YouTubeSourceBudget.cs#L172) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.ParseProbeSegment.segments | [173:17](../YouTubeSourceBudget.cs#L173) |
| iteration-variable | Emby.YouTubePlugin.YouTubeSourceBudget.ParseProbeSegment.line | [174:13](../YouTubeSourceBudget.cs#L174) |
| pattern-variable | Emby.YouTubePlugin.YouTubeSourceBudget.ParseProbeSegment.line.duration | [180:83](../YouTubeSourceBudget.cs#L180) |
| pattern-variable | Emby.YouTubePlugin.YouTubeSourceBudget.ParseProbeSegment.line.target | [189:63](../YouTubeSourceBudget.cs#L189) |
| local-variable | Emby.YouTubePlugin.YouTubeSourceBudget.ParseProbeSegment.line.segment | [209:21](../YouTubeSourceBudget.cs#L209) |
| type | Emby.YouTubePlugin.YouTubeSourceBudget.UnsupportedProbePlaylistException | [219:9](../YouTubeSourceBudget.cs#L219) |
| constructor | Emby.YouTubePlugin.YouTubeSourceBudget.UnsupportedProbePlaylistException.UnsupportedProbePlaylistException | [221:13](../YouTubeSourceBudget.cs#L221) |

</details>

<a id="file-a5337fff5b60437f"></a>
## [YouTubeStreamResolver.cs](../YouTubeStreamResolver.cs)

Plugin Compile source (SDK default); 16082 bytes; 301 lines.

<details>
<summary>All 146 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| namespace | Emby.YouTubePlugin | [10:1](../YouTubeStreamResolver.cs#L10) |
| type | Emby.YouTubePlugin.YouTubeStreamResolver | [14:5](../YouTubeStreamResolver.cs#L14) |
| field | Emby.YouTubePlugin.YouTubeStreamResolver.MaximumJsonBytes | [16:27](../YouTubeStreamResolver.cs#L16) |
| field | Emby.YouTubePlugin.YouTubeStreamResolver.MaximumCacheBytes | [17:27](../YouTubeStreamResolver.cs#L17) |
| field | Emby.YouTubePlugin.YouTubeStreamResolver.Gate | [18:40](../YouTubeStreamResolver.cs#L18) |
| field | Emby.YouTubePlugin.YouTubeStreamResolver.ResolutionSlots | [19:47](../YouTubeStreamResolver.cs#L19) |
| field | Emby.YouTubePlugin.YouTubeStreamResolver.Cache | [20:64](../YouTubeStreamResolver.cs#L20) |
| field | Emby.YouTubePlugin.YouTubeStreamResolver.Flights | [21:60](../YouTubeStreamResolver.cs#L21) |
| field | Emby.YouTubePlugin.YouTubeStreamResolver._acquire | [22:70](../YouTubeStreamResolver.cs#L22) |
| field | Emby.YouTubePlugin.YouTubeStreamResolver._utcNow | [23:39](../YouTubeStreamResolver.cs#L23) |
| anonymous-function | Emby.YouTubePlugin.YouTubeStreamResolver._utcNow.anonymous@1069 | [23:49](../YouTubeStreamResolver.cs#L23) |
| field | Emby.YouTubePlugin.YouTubeStreamResolver._cacheSequence | [24:29](../YouTubeStreamResolver.cs#L24) |
| field | Emby.YouTubePlugin.YouTubeStreamResolver._cacheBytes | [25:28](../YouTubeStreamResolver.cs#L25) |
| field | Emby.YouTubePlugin.YouTubeStreamResolver.VideoIdPattern | [26:39](../YouTubeStreamResolver.cs#L26) |
| field | Emby.YouTubePlugin.YouTubeStreamResolver.PathExpiryPattern | [27:39](../YouTubeStreamResolver.cs#L27) |
| method | Emby.YouTubePlugin.YouTubeStreamResolver.ResolveAsync | [29:9](../YouTubeStreamResolver.cs#L29) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.ResolveAsync.videoId | [29:71](../YouTubeStreamResolver.cs#L29) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.ResolveAsync.cancellationToken | [29:87](../YouTubeStreamResolver.cs#L29) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ResolveAsync.options | [33:17](../YouTubeStreamResolver.cs#L33) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ResolveAsync.key | [35:20](../YouTubeStreamResolver.cs#L35) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ResolveAsync.flight | [36:20](../YouTubeStreamResolver.cs#L36) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ResolveAsync.invalidated | [37:41](../YouTubeStreamResolver.cs#L37) |
| pattern-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ResolveAsync.cached | [41:52](../YouTubeStreamResolver.cs#L41) |
| anonymous-function | Emby.YouTubePlugin.YouTubeStreamResolver.ResolveAsync.anonymous@2928 | [54:44](../YouTubeStreamResolver.cs#L54) |
| anonymous-function | Emby.YouTubePlugin.YouTubeStreamResolver.ResolveAsync.anonymous@3013 | [55:50](../YouTubeStreamResolver.cs#L55) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.ResolveAsync.anonymous@3013.task | [55:50](../YouTubeStreamResolver.cs#L55) |
| method | Emby.YouTubePlugin.YouTubeStreamResolver.Invalidate | [91:9](../YouTubeStreamResolver.cs#L91) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.Invalidate.videoId | [91:41](../YouTubeStreamResolver.cs#L91) |
| iteration-variable | Emby.YouTubePlugin.YouTubeStreamResolver.Invalidate.key | [96:17](../YouTubeStreamResolver.cs#L96) |
| anonymous-function | Emby.YouTubePlugin.YouTubeStreamResolver.Invalidate.key.anonymous@4891 | [96:49](../YouTubeStreamResolver.cs#L96) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.Invalidate.key.anonymous@4891.pair | [96:49](../YouTubeStreamResolver.cs#L96) |
| anonymous-function | Emby.YouTubePlugin.YouTubeStreamResolver.Invalidate.key.anonymous@4937 | [96:95](../YouTubeStreamResolver.cs#L96) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.Invalidate.key.anonymous@4937.pair | [96:95](../YouTubeStreamResolver.cs#L96) |
| iteration-variable | Emby.YouTubePlugin.YouTubeStreamResolver.Invalidate.flight | [98:17](../YouTubeStreamResolver.cs#L98) |
| anonymous-function | Emby.YouTubePlugin.YouTubeStreamResolver.Invalidate.flight.anonymous@5064 | [98:61](../YouTubeStreamResolver.cs#L98) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.Invalidate.flight.anonymous@5064.value | [98:61](../YouTubeStreamResolver.cs#L98) |
| method | Emby.YouTubePlugin.YouTubeStreamResolver.RunFlightAsync | [103:9](../YouTubeStreamResolver.cs#L103) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.RunFlightAsync.key | [104:13](../YouTubeStreamResolver.cs#L104) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.RunFlightAsync.flight | [104:25](../YouTubeStreamResolver.cs#L104) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.RunFlightAsync.acquired | [106:18](../YouTubeStreamResolver.cs#L106) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.RunFlightAsync.json | [113:21](../YouTubeStreamResolver.cs#L113) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.RunFlightAsync.result | [115:21](../YouTubeStreamResolver.cs#L115) |
| catch-variable | Emby.YouTubePlugin.YouTubeStreamResolver.RunFlightAsync.error | [130:19](../YouTubeStreamResolver.cs#L130) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.RunFlightAsync.code | [132:24](../YouTubeStreamResolver.cs#L132) |
| pattern-variable | Emby.YouTubePlugin.YouTubeStreamResolver.RunFlightAsync.code.managed | [132:74](../YouTubeStreamResolver.cs#L132) |
| method | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult | [152:9](../YouTubeStreamResolver.cs#L152) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.videoId | [152:57](../YouTubeStreamResolver.cs#L152) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.json | [152:73](../YouTubeStreamResolver.cs#L152) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.now | [152:86](../YouTubeStreamResolver.cs#L152) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.document | [155:23](../YouTubeStreamResolver.cs#L155) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.metadata | [156:17](../YouTubeStreamResolver.cs#L156) |
| pattern-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.formats | [159:64](../YouTubeStreamResolver.cs#L159) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.liveStatus | [162:17](../YouTubeStreamResolver.cs#L162) |
| pattern-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.duration | [165:61](../YouTubeStreamResolver.cs#L165) |
| pattern-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.seconds | [167:101](../YouTubeStreamResolver.cs#L167) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.expiry | [170:22](../YouTubeStreamResolver.cs#L170) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.hasMedia | [171:18](../YouTubeStreamResolver.cs#L171) |
| pattern-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.hls | [172:69](../YouTubeStreamResolver.cs#L172) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.identities | [178:17](../YouTubeStreamResolver.cs#L178) |
| iteration-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.format | [179:13](../YouTubeStreamResolver.cs#L179) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.format.protocol | [181:25](../YouTubeStreamResolver.cs#L181) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.format.url | [183:25](../YouTubeStreamResolver.cs#L183) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.format.mediaUri | [185:21](../YouTubeStreamResolver.cs#L185) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.format.identity | [186:25](../YouTubeStreamResolver.cs#L186) |
| anonymous-function | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.format.anonymous@9813 | [187:86](../YouTubeStreamResolver.cs#L187) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.format.anonymous@9813.ch | [187:86](../YouTubeStreamResolver.cs#L187) |
| pattern-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.format.size | [188:96](../YouTubeStreamResolver.cs#L188) |
| pattern-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.format.length | [189:93](../YouTubeStreamResolver.cs#L189) |
| pattern-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.format.manifest | [193:67](../YouTubeStreamResolver.cs#L193) |
| pattern-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.format.headers | [195:67](../YouTubeStreamResolver.cs#L195) |
| iteration-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.name | [198:13](../YouTubeStreamResolver.cs#L198) |
| pattern-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.name.languages | [200:60](../YouTubeStreamResolver.cs#L200) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.name.languageCount | [202:21](../YouTubeStreamResolver.cs#L202) |
| iteration-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.name.language | [203:17](../YouTubeStreamResolver.cs#L203) |
| iteration-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.name.language.caption | [208:21](../YouTubeStreamResolver.cs#L208) |
| pattern-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.name.language.caption.url | [210:67](../YouTubeStreamResolver.cs#L210) |
| pattern-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.name.language.caption.headers | [212:76](../YouTubeStreamResolver.cs#L212) |
| pattern-variable | Emby.YouTubePlugin.YouTubeStreamResolver.ParseResult.topHeaders | [217:65](../YouTubeStreamResolver.cs#L217) |
| method | Emby.YouTubePlugin.YouTubeStreamResolver.UrlExpiry | [221:9](../YouTubeStreamResolver.cs#L221) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.UrlExpiry.uri | [221:43](../YouTubeStreamResolver.cs#L221) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.UrlExpiry.now | [221:52](../YouTubeStreamResolver.cs#L221) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.UrlExpiry.earliest | [223:19](../YouTubeStreamResolver.cs#L223) |
| iteration-variable | Emby.YouTubePlugin.YouTubeStreamResolver.UrlExpiry.pair | [224:13](../YouTubeStreamResolver.cs#L224) |
| local-variable | Emby.YouTubePlugin.YouTubeStreamResolver.UrlExpiry.pair.parts | [226:21](../YouTubeStreamResolver.cs#L226) |
| pattern-variable | Emby.YouTubePlugin.YouTubeStreamResolver.UrlExpiry.pair.value | [230:111](../YouTubeStreamResolver.cs#L230) |
| iteration-variable | Emby.YouTubePlugin.YouTubeStreamResolver.UrlExpiry.match | [235:13](../YouTubeStreamResolver.cs#L235) |
| pattern-variable | Emby.YouTubePlugin.YouTubeStreamResolver.UrlExpiry.match.value | [237:117](../YouTubeStreamResolver.cs#L237) |
| method | Emby.YouTubePlugin.YouTubeStreamResolver.StringValue | [246:9](../YouTubeStreamResolver.cs#L246) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.StringValue.element | [246:44](../YouTubeStreamResolver.cs#L246) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.StringValue.property | [246:65](../YouTubeStreamResolver.cs#L246) |
| pattern-variable | Emby.YouTubePlugin.YouTubeStreamResolver.StringValue.value | [247:99](../YouTubeStreamResolver.cs#L247) |
| method | Emby.YouTubePlugin.YouTubeStreamResolver.Earlier | [250:9](../YouTubeStreamResolver.cs#L250) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.Earlier.first | [250:41](../YouTubeStreamResolver.cs#L250) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.Earlier.second | [250:57](../YouTubeStreamResolver.cs#L250) |
| method | Emby.YouTubePlugin.YouTubeStreamResolver.AddCache | [252:9](../YouTubeStreamResolver.cs#L252) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.AddCache.key | [252:38](../YouTubeStreamResolver.cs#L252) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.AddCache.entry | [252:50](../YouTubeStreamResolver.cs#L252) |
| anonymous-function | Emby.YouTubePlugin.YouTubeStreamResolver.AddCache.anonymous@14378 | [259:43](../YouTubeStreamResolver.cs#L259) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.AddCache.anonymous@14378.pair | [259:43](../YouTubeStreamResolver.cs#L259) |
| method | Emby.YouTubePlugin.YouTubeStreamResolver.PruneCache | [262:9](../YouTubeStreamResolver.cs#L262) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.PruneCache.now | [262:40](../YouTubeStreamResolver.cs#L262) |
| iteration-variable | Emby.YouTubePlugin.YouTubeStreamResolver.PruneCache.key | [264:13](../YouTubeStreamResolver.cs#L264) |
| anonymous-function | Emby.YouTubePlugin.YouTubeStreamResolver.PruneCache.key.anonymous@14541 | [264:45](../YouTubeStreamResolver.cs#L264) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.PruneCache.key.anonymous@14541.pair | [264:45](../YouTubeStreamResolver.cs#L264) |
| anonymous-function | Emby.YouTubePlugin.YouTubeStreamResolver.PruneCache.key.anonymous@14586 | [264:90](../YouTubeStreamResolver.cs#L264) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.PruneCache.key.anonymous@14586.pair | [264:90](../YouTubeStreamResolver.cs#L264) |
| method | Emby.YouTubePlugin.YouTubeStreamResolver.RemoveCache | [268:9](../YouTubeStreamResolver.cs#L268) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.RemoveCache.key | [268:41](../YouTubeStreamResolver.cs#L268) |
| pattern-variable | Emby.YouTubePlugin.YouTubeStreamResolver.RemoveCache.entry | [270:48](../YouTubeStreamResolver.cs#L270) |
| method | Emby.YouTubePlugin.YouTubeStreamResolver.TryCancel | [273:9](../YouTubeStreamResolver.cs#L273) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.TryCancel.source | [273:39](../YouTubeStreamResolver.cs#L273) |
| method | Emby.YouTubePlugin.YouTubeStreamResolver.Failure | [278:9](../YouTubeStreamResolver.cs#L278) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.Failure.message | [278:58](../YouTubeStreamResolver.cs#L278) |
| type | Emby.YouTubePlugin.YouTubeStreamResolver.Flight | [280:9](../YouTubeStreamResolver.cs#L280) |
| constructor | Emby.YouTubePlugin.YouTubeStreamResolver.Flight.Flight | [282:13](../YouTubeStreamResolver.cs#L282) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.Flight.Flight.videoId | [282:29](../YouTubeStreamResolver.cs#L282) |
| property | Emby.YouTubePlugin.YouTubeStreamResolver.Flight.VideoId | [283:13](../YouTubeStreamResolver.cs#L283) |
| accessor | Emby.YouTubePlugin.YouTubeStreamResolver.Flight.VideoId.get | [283:39](../YouTubeStreamResolver.cs#L283) |
| property | Emby.YouTubePlugin.YouTubeStreamResolver.Flight.Deadline | [284:13](../YouTubeStreamResolver.cs#L284) |
| accessor | Emby.YouTubePlugin.YouTubeStreamResolver.Flight.Deadline.get | [284:57](../YouTubeStreamResolver.cs#L284) |
| property | Emby.YouTubePlugin.YouTubeStreamResolver.Flight.Task | [285:13](../YouTubeStreamResolver.cs#L285) |
| accessor | Emby.YouTubePlugin.YouTubeStreamResolver.Flight.Task.get | [285:56](../YouTubeStreamResolver.cs#L285) |
| accessor | Emby.YouTubePlugin.YouTubeStreamResolver.Flight.Task.set | [285:61](../YouTubeStreamResolver.cs#L285) |
| property | Emby.YouTubePlugin.YouTubeStreamResolver.Flight.Waiters | [286:13](../YouTubeStreamResolver.cs#L286) |
| accessor | Emby.YouTubePlugin.YouTubeStreamResolver.Flight.Waiters.get | [286:36](../YouTubeStreamResolver.cs#L286) |
| accessor | Emby.YouTubePlugin.YouTubeStreamResolver.Flight.Waiters.set | [286:41](../YouTubeStreamResolver.cs#L286) |
| property | Emby.YouTubePlugin.YouTubeStreamResolver.Flight.CacheAllowed | [287:13](../YouTubeStreamResolver.cs#L287) |
| accessor | Emby.YouTubePlugin.YouTubeStreamResolver.Flight.CacheAllowed.get | [287:42](../YouTubeStreamResolver.cs#L287) |
| accessor | Emby.YouTubePlugin.YouTubeStreamResolver.Flight.CacheAllowed.set | [287:47](../YouTubeStreamResolver.cs#L287) |
| type | Emby.YouTubePlugin.YouTubeStreamResolver.CacheEntry | [290:9](../YouTubeStreamResolver.cs#L290) |
| constructor | Emby.YouTubePlugin.YouTubeStreamResolver.CacheEntry.CacheEntry | [292:13](../YouTubeStreamResolver.cs#L292) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.CacheEntry.CacheEntry.videoId | [292:33](../YouTubeStreamResolver.cs#L292) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.CacheEntry.CacheEntry.result | [292:49](../YouTubeStreamResolver.cs#L292) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.CacheEntry.CacheEntry.expiresUtc | [292:79](../YouTubeStreamResolver.cs#L292) |
| parameter | Emby.YouTubePlugin.YouTubeStreamResolver.CacheEntry.CacheEntry.bytes | [292:100](../YouTubeStreamResolver.cs#L292) |
| property | Emby.YouTubePlugin.YouTubeStreamResolver.CacheEntry.VideoId | [294:13](../YouTubeStreamResolver.cs#L294) |
| accessor | Emby.YouTubePlugin.YouTubeStreamResolver.CacheEntry.VideoId.get | [294:39](../YouTubeStreamResolver.cs#L294) |
| property | Emby.YouTubePlugin.YouTubeStreamResolver.CacheEntry.Result | [295:13](../YouTubeStreamResolver.cs#L295) |
| accessor | Emby.YouTubePlugin.YouTubeStreamResolver.CacheEntry.Result.get | [295:53](../YouTubeStreamResolver.cs#L295) |
| property | Emby.YouTubePlugin.YouTubeStreamResolver.CacheEntry.ExpiresUtc | [296:13](../YouTubeStreamResolver.cs#L296) |
| accessor | Emby.YouTubePlugin.YouTubeStreamResolver.CacheEntry.ExpiresUtc.get | [296:44](../YouTubeStreamResolver.cs#L296) |
| property | Emby.YouTubePlugin.YouTubeStreamResolver.CacheEntry.Bytes | [297:13](../YouTubeStreamResolver.cs#L297) |
| accessor | Emby.YouTubePlugin.YouTubeStreamResolver.CacheEntry.Bytes.get | [297:34](../YouTubeStreamResolver.cs#L297) |
| property | Emby.YouTubePlugin.YouTubeStreamResolver.CacheEntry.LastAccess | [298:13](../YouTubeStreamResolver.cs#L298) |
| accessor | Emby.YouTubePlugin.YouTubeStreamResolver.CacheEntry.LastAccess.get | [298:40](../YouTubeStreamResolver.cs#L298) |
| accessor | Emby.YouTubePlugin.YouTubeStreamResolver.CacheEntry.LastAccess.set | [298:45](../YouTubeStreamResolver.cs#L298) |

</details>

<a id="file-8a63fe1dcd96b301"></a>
## [docs/CODE_GUIDE.md](../docs/CODE_GUIDE.md)

Documentation/resource; 34274 bytes; 156 lines.

<a id="file-24c05485c45ad84d"></a>
## [docs/CODE_MAP_MAINTENANCE.md](../docs/CODE_MAP_MAINTENANCE.md)

Documentation/resource; 6611 bytes; 54 lines.

<a id="file-31e2cff7937d621c"></a>
## [docs/code-map.flows.json](../docs/code-map.flows.json)

Documentation/resource; 9392 bytes; 77 lines.

<a id="file-51ccab4fa46b4e7f"></a>
## [docs/screenshots/.gitkeep](../docs/screenshots/.gitkeep)

Documentation/resource; 1 bytes.

<a id="file-73dd066abffa11bb"></a>
## [docs/screenshots/Root Folder.png](../docs/screenshots/Root%20Folder.png)

Documentation/resource; 108066 bytes.

<a id="file-5a6060bd29b474db"></a>
## [docs/screenshots/channel folder.png](../docs/screenshots/channel%20folder.png)

Documentation/resource; 27128 bytes.

<a id="file-51471e3d6d535ee4"></a>
## [docs/screenshots/setup 1.png](../docs/screenshots/setup%201.png)

Documentation/resource; 196516 bytes.

<a id="file-b31749773496f6d1"></a>
## [docs/screenshots/setup 2.png](../docs/screenshots/setup%202.png)

Documentation/resource; 150241 bytes.

<a id="file-31c7ba2a78fa4783"></a>
## [docs/screenshots/setup 3.png](../docs/screenshots/setup%203.png)

Documentation/resource; 85631 bytes.

<a id="file-991a05bbbd648bf8"></a>
## [docs/screenshots/setup 4.png](../docs/screenshots/setup%204.png)

Documentation/resource; 129561 bytes.

<a id="file-c296f1b827fce9f3"></a>
## [docs/screenshots/setup 5.png](../docs/screenshots/setup%205.png)

Documentation/resource; 179118 bytes.

<a id="file-ca941b12bd6e7d3d"></a>
## [docs/screenshots/trending videos.png](../docs/screenshots/trending%20videos.png)

Documentation/resource; 1344479 bytes.

<a id="file-8c560e4ae292c462"></a>
## [thumb.png](../thumb.png)

EmbeddedResource + Content; 10642 bytes.

<a id="file-f877f7ea17f46f5d"></a>
## [tools/CodeMap/CodeMap.csproj](../tools/CodeMap/CodeMap.csproj)

Build definition; 743 bytes; 18 lines.

<a id="file-bf979a122640e0a8"></a>
## [tools/CodeMap/Program.cs](../tools/CodeMap/Program.cs)

C# source excluded from plugin Compile; 10553 bytes; 179 lines.

<details>
<summary>All 98 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| local-variable | root | [18:9](../tools/CodeMap/Program.cs#L18) |
| local-variable | input | [19:15](../tools/CodeMap/Program.cs#L19) |
| local-variable | files | [22:9](../tools/CodeMap/Program.cs#L22) |
| local-variable | paths | [23:9](../tools/CodeMap/Program.cs#L23) |
| iteration-variable | entry | [24:5](../tools/CodeMap/Program.cs#L24) |
| local-variable | entry.relative | [26:13](../tools/CodeMap/Program.cs#L26) |
| anonymous-function | entry.anonymous@1247 | [29:40](../tools/CodeMap/Program.cs#L29) |
| parameter | entry.anonymous@1247.part | [29:40](../tools/CodeMap/Program.cs#L29) |
| local-variable | entry.path | [32:13](../tools/CodeMap/Program.cs#L32) |
| local-variable | entry.bytes | [33:13](../tools/CodeMap/Program.cs#L33) |
| local-variable | entry.sha256 | [34:13](../tools/CodeMap/Program.cs#L34) |
| local-variable | entry.source | [37:13](../tools/CodeMap/Program.cs#L37) |
| local-variable | entry.tree | [39:13](../tools/CodeMap/Program.cs#L39) |
| anonymous-function | entry.anonymous@2057 | [40:51](../tools/CodeMap/Program.cs#L40) |
| parameter | entry.anonymous@2057.trivia | [40:51](../tools/CodeMap/Program.cs#L40) |
| local-variable | entry.errors | [42:13](../tools/CodeMap/Program.cs#L42) |
| anonymous-function | entry.errors.anonymous@2292 | [42:50](../tools/CodeMap/Program.cs#L42) |
| parameter | entry.errors.anonymous@2292.d | [42:50](../tools/CodeMap/Program.cs#L42) |
| anonymous-function | entry.anonymous@2521 | [45:59](../tools/CodeMap/Program.cs#L45) |
| parameter | entry.anonymous@2521.d | [45:59](../tools/CodeMap/Program.cs#L45) |
| local-variable | entry.declarations | [47:13](../tools/CodeMap/Program.cs#L47) |
| local-variable | entry.payloads | [48:13](../tools/CodeMap/Program.cs#L48) |
| iteration-variable | entry.node | [49:9](../tools/CodeMap/Program.cs#L49) |
| local-variable | entry.node.description | [51:17](../tools/CodeMap/Program.cs#L51) |
| pattern-variable | entry.node.item | [52:36](../tools/CodeMap/Program.cs#L52) |
| local-variable | entry.node.location | [54:21](../tools/CodeMap/Program.cs#L54) |
| local-variable | entry.node.owners | [55:21](../tools/CodeMap/Program.cs#L55) |
| anonymous-function | entry.node.owners.anonymous@3056 | [56:28](../tools/CodeMap/Program.cs#L56) |
| parameter | entry.node.owners.anonymous@3056.value | [56:28](../tools/CodeMap/Program.cs#L56) |
| anonymous-function | entry.node.owners.anonymous@3112 | [57:29](../tools/CodeMap/Program.cs#L57) |
| parameter | entry.node.owners.anonymous@3112.value | [57:29](../tools/CodeMap/Program.cs#L57) |
| pattern-variable | entry.node.variable | [72:50](../tools/CodeMap/Program.cs#L72) |
| pattern-variable | entry.node.value | [74:68](../tools/CodeMap/Program.cs#L74) |
| local-variable | entry.node.payload | [76:21](../tools/CodeMap/Program.cs#L76) |
| local-variable | output | [88:9](../tools/CodeMap/Program.cs#L88) |
| local-variable | destination | [96:9](../tools/CodeMap/Program.cs#L96) |
| catch-variable | exception | [103:7](../tools/CodeMap/Program.cs#L103) |
| local-function | Describe | [109:1](../tools/CodeMap/Program.cs#L109) |
| tuple-element | Describe.Kind | [109:9](../tools/CodeMap/Program.cs#L109) |
| tuple-element | Describe.Name | [109:22](../tools/CodeMap/Program.cs#L109) |
| parameter | Describe.node | [109:45](../tools/CodeMap/Program.cs#L109) |
| pattern-variable | Describe.value | [111:36](../tools/CodeMap/Program.cs#L111) |
| pattern-variable | Describe.value | [112:31](../tools/CodeMap/Program.cs#L112) |
| pattern-variable | Describe.value | [113:31](../tools/CodeMap/Program.cs#L113) |
| pattern-variable | Describe.value | [114:29](../tools/CodeMap/Program.cs#L114) |
| pattern-variable | Describe.value | [115:34](../tools/CodeMap/Program.cs#L115) |
| pattern-variable | Describe.value | [116:33](../tools/CodeMap/Program.cs#L116) |
| pattern-variable | Describe.value | [117:31](../tools/CodeMap/Program.cs#L117) |
| pattern-variable | Describe.value | [118:41](../tools/CodeMap/Program.cs#L118) |
| pattern-variable | Describe.value | [119:31](../tools/CodeMap/Program.cs#L119) |
| pattern-variable | Describe.value | [120:30](../tools/CodeMap/Program.cs#L120) |
| pattern-variable | Describe.value | [121:28](../tools/CodeMap/Program.cs#L121) |
| pattern-variable | Describe.value | [122:33](../tools/CodeMap/Program.cs#L122) |
| pattern-variable | Describe.value | [123:34](../tools/CodeMap/Program.cs#L123) |
| pattern-variable | Describe.value | [124:30](../tools/CodeMap/Program.cs#L124) |
| pattern-variable | Describe.value | [126:21](../tools/CodeMap/Program.cs#L126) |
| pattern-variable | Describe.value | [127:25](../tools/CodeMap/Program.cs#L127) |
| pattern-variable | Describe.value | [128:24](../tools/CodeMap/Program.cs#L128) |
| pattern-variable | Describe.value | [129:31](../tools/CodeMap/Program.cs#L129) |
| pattern-variable | Describe.value | [131:28](../tools/CodeMap/Program.cs#L131) |
| pattern-variable | Describe.value | [132:28](../tools/CodeMap/Program.cs#L132) |
| pattern-variable | Describe.value | [133:37](../tools/CodeMap/Program.cs#L133) |
| pattern-variable | Describe.value | [134:28](../tools/CodeMap/Program.cs#L134) |
| pattern-variable | Describe.value | [135:26](../tools/CodeMap/Program.cs#L135) |
| pattern-variable | Describe.value | [136:32](../tools/CodeMap/Program.cs#L136) |
| pattern-variable | Describe.value | [137:22](../tools/CodeMap/Program.cs#L137) |
| pattern-variable | Describe.value | [138:21](../tools/CodeMap/Program.cs#L138) |
| pattern-variable | Describe.value | [139:22](../tools/CodeMap/Program.cs#L139) |
| pattern-variable | Describe.value | [140:26](../tools/CodeMap/Program.cs#L140) |
| pattern-variable | Describe.value | [141:29](../tools/CodeMap/Program.cs#L141) |
| local-function | Signature | [145:1](../tools/CodeMap/Program.cs#L145) |
| parameter | Signature.node | [145:25](../tools/CodeMap/Program.cs#L145) |
| local-variable | Signature.end | [149:9](../tools/CodeMap/Program.cs#L149) |
| pattern-variable | Signature.end.value | [151:37](../tools/CodeMap/Program.cs#L151) |
| pattern-variable | Signature.end.value | [152:35](../tools/CodeMap/Program.cs#L152) |
| pattern-variable | Signature.end.value | [153:40](../tools/CodeMap/Program.cs#L153) |
| pattern-variable | Signature.end.value | [154:39](../tools/CodeMap/Program.cs#L154) |
| pattern-variable | Signature.end.value | [155:38](../tools/CodeMap/Program.cs#L155) |
| pattern-variable | Signature.end.value | [156:34](../tools/CodeMap/Program.cs#L156) |
| pattern-variable | Signature.end.value | [157:43](../tools/CodeMap/Program.cs#L157) |
| local-variable | Signature.tokens | [160:9](../tools/CodeMap/Program.cs#L160) |
| anonymous-function | Signature.tokens.anonymous@9494 | [160:48](../tools/CodeMap/Program.cs#L160) |
| parameter | Signature.tokens.anonymous@9494.token | [160:48](../tools/CodeMap/Program.cs#L160) |
| anonymous-function | Signature.tokens.anonymous@9533 | [160:87](../tools/CodeMap/Program.cs#L160) |
| parameter | Signature.tokens.anonymous@9533.token | [160:87](../tools/CodeMap/Program.cs#L160) |
| local-variable | Signature.signature | [161:9](../tools/CodeMap/Program.cs#L161) |
| pattern-variable | Signature.declaration | [162:86](../tools/CodeMap/Program.cs#L162) |
| local-variable | Signature.modifiers | [164:13](../tools/CodeMap/Program.cs#L164) |
| pattern-variable | Signature.modifiers.field | [164:74](../tools/CodeMap/Program.cs#L164) |
| anonymous-function | Signature.modifiers.anonymous@9838 | [165:55](../tools/CodeMap/Program.cs#L165) |
| parameter | Signature.modifiers.anonymous@9838.token | [165:55](../tools/CodeMap/Program.cs#L165) |
| local-function | ConstantString | [171:1](../tools/CodeMap/Program.cs#L171) |
| parameter | ConstantString.expression | [171:31](../tools/CodeMap/Program.cs#L171) |
| pattern-variable | ConstantString.literal | [173:29](../tools/CodeMap/Program.cs#L173) |
| pattern-variable | ConstantString.binary | [174:28](../tools/CodeMap/Program.cs#L174) |
| pattern-variable | ConstantString.left | [175:50](../tools/CodeMap/Program.cs#L175) |
| pattern-variable | ConstantString.right | [176:51](../tools/CodeMap/Program.cs#L176) |
| pattern-variable | ConstantString.parenthesized | [177:35](../tools/CodeMap/Program.cs#L177) |

</details>

<a id="file-ed1a99afe3defdab"></a>
## [tools/code-map.config.json](../tools/code-map.config.json)

Development tooling; 420 bytes; 10 lines.

<a id="file-f02e27bb31214f09"></a>
## [tools/code-map.mjs](../tools/code-map.mjs)

Development tooling; 29814 bytes; 455 lines.

<details>
<summary>All 322 declarations / anchors</summary>

| Kind | Owner / name | Source |
|---|---|---|
| import | fs | [2:8](../tools/code-map.mjs#L2) |
| import | path | [3:8](../tools/code-map.mjs#L3) |
| import | crypto | [4:8](../tools/code-map.mjs#L4) |
| import | createRequire | [5:10](../tools/code-map.mjs#L5) |
| import | fileURLToPath | [6:10](../tools/code-map.mjs#L6) |
| variable | require | [8:7](../tools/code-map.mjs#L8) |
| variable | DEFAULT_ROOT | [9:7](../tools/code-map.mjs#L9) |
| variable | CONFIG | [10:7](../tools/code-map.mjs#L10) |
| variable | json | [11:7](../tools/code-map.mjs#L11) |
| function | json | [11:14](../tools/code-map.mjs#L11) |
| parameter | json.value | [11:14](../tools/code-map.mjs#L11) |
| variable | hash | [12:7](../tools/code-map.mjs#L12) |
| function | hash | [12:14](../tools/code-map.mjs#L12) |
| parameter | hash.value | [12:14](../tools/code-map.mjs#L12) |
| variable | sorted | [13:7](../tools/code-map.mjs#L13) |
| function | sorted | [13:16](../tools/code-map.mjs#L13) |
| parameter | sorted.values | [13:16](../tools/code-map.mjs#L13) |
| function | sorted.[...values].sort callback@560 | [13:43](../tools/code-map.mjs#L13) |
| parameter | sorted.[...values].sort callback@560.a | [13:44](../tools/code-map.mjs#L13) |
| parameter | sorted.[...values].sort callback@560.b | [13:47](../tools/code-map.mjs#L13) |
| variable | assert | [14:7](../tools/code-map.mjs#L14) |
| function | assert | [14:16](../tools/code-map.mjs#L14) |
| parameter | assert.condition | [14:17](../tools/code-map.mjs#L14) |
| parameter | assert.message | [14:28](../tools/code-map.mjs#L14) |
| variable | readJson | [15:7](../tools/code-map.mjs#L15) |
| function | readJson | [15:18](../tools/code-map.mjs#L15) |
| parameter | readJson.filename | [15:18](../tools/code-map.mjs#L15) |
| variable | sourceLink | [16:7](../tools/code-map.mjs#L16) |
| function | sourceLink | [16:20](../tools/code-map.mjs#L16) |
| parameter | sourceLink.filename | [16:21](../tools/code-map.mjs#L16) |
| parameter | sourceLink.line | [16:31](../tools/code-map.mjs#L16) |
| variable | escapeMd | [17:7](../tools/code-map.mjs#L17) |
| function | escapeMd | [17:18](../tools/code-map.mjs#L17) |
| parameter | escapeMd.value | [17:18](../tools/code-map.mjs#L17) |
| function | safeRelative | [19:1](../tools/code-map.mjs#L19) |
| parameter | safeRelative.value | [19:23](../tools/code-map.mjs#L19) |
| function | safeRelative.value.split('/').some callback@1219 | [21:60](../tools/code-map.mjs#L21) |
| parameter | safeRelative.value.split('/').some callback@1219.part | [21:60](../tools/code-map.mjs#L21) |
| function | loadConfig | [25:8](../tools/code-map.mjs#L25) |
| parameter | loadConfig.root | [25:28](../tools/code-map.mjs#L25) |
| variable | loadConfig.config | [26:11](../tools/code-map.mjs#L26) |
| variable | loadConfig.key | [28:16](../tools/code-map.mjs#L28) |
| variable | loadConfig.value | [29:16](../tools/code-map.mjs#L29) |
| variable | loadConfig.value | [30:16](../tools/code-map.mjs#L30) |
| function | snapshot | [35:8](../tools/code-map.mjs#L35) |
| parameter | snapshot.root | [35:26](../tools/code-map.mjs#L35) |
| parameter | snapshot.config | [35:32](../tools/code-map.mjs#L35) |
| variable | snapshot.excludedFiles | [36:11](../tools/code-map.mjs#L36) |
| variable | snapshot.excludedDirectories | [37:11](../tools/code-map.mjs#L37) |
| variable | snapshot.files | [38:11](../tools/code-map.mjs#L38) |
| function | snapshot.visit | [39:5](../tools/code-map.mjs#L39) |
| parameter | snapshot.visit.directory | [39:20](../tools/code-map.mjs#L39) |
| parameter | snapshot.visit.prefix | [39:31](../tools/code-map.mjs#L39) |
| variable | snapshot.visit.name | [40:20](../tools/code-map.mjs#L40) |
| variable | snapshot.visit.relative | [41:19](../tools/code-map.mjs#L41) |
| variable | snapshot.visit.absolute | [42:19](../tools/code-map.mjs#L42) |
| variable | snapshot.visit.stat | [43:19](../tools/code-map.mjs#L43) |
| variable | snapshot.visit.bytes | [49:23](../tools/code-map.mjs#L49) |
| function | snapshot.files.sort callback@3223 | [55:16](../tools/code-map.mjs#L55) |
| parameter | snapshot.files.sort callback@3223.a | [55:17](../tools/code-map.mjs#L55) |
| parameter | snapshot.files.sort callback@3223.b | [55:20](../tools/code-map.mjs#L55) |
| function | snapshot.files.some callback@3304 | [56:23](../tools/code-map.mjs#L56) |
| parameter | snapshot.files.some callback@3304.file | [56:23](../tools/code-map.mjs#L56) |
| function | prepare | [60:8](../tools/code-map.mjs#L60) |
| parameter | prepare.root | [60:25](../tools/code-map.mjs#L60) |
| parameter | prepare.destination | [60:31](../tools/code-map.mjs#L60) |
| variable | prepare.files | [61:11](../tools/code-map.mjs#L61) |
| function | prepare.snapshot(root).filter callback@3475 | [61:41](../tools/code-map.mjs#L61) |
| parameter | prepare.snapshot(root).filter callback@3475.file | [61:41](../tools/code-map.mjs#L61) |
| function | prepare.snapshot(root).filter(file =&gt; /\.cs$/i.test(file.path)).map callback@3513 | [61:79](../tools/code-map.mjs#L61) |
| parameter | prepare.snapshot(root).filter(file =&gt; /\.cs$/i.test(file.path)).map callback@3513.path | [61:82](../tools/code-map.mjs#L61) |
| parameter | prepare.snapshot(root).filter(file =&gt; /\.cs$/i.test(file.path)).map callback@3513.sha256 | [61:88](../tools/code-map.mjs#L61) |
| function | writeFile | [66:1](../tools/code-map.mjs#L66) |
| parameter | writeFile.filename | [66:20](../tools/code-map.mjs#L66) |
| parameter | writeFile.content | [66:30](../tools/code-map.mjs#L66) |
| function | globRegex | [71:1](../tools/code-map.mjs#L71) |
| parameter | globRegex.pattern | [71:20](../tools/code-map.mjs#L71) |
| variable | globRegex.result | [73:9](../tools/code-map.mjs#L73) |
| variable | globRegex.i | [74:14](../tools/code-map.mjs#L74) |
| function | attributes | [84:1](../tools/code-map.mjs#L84) |
| parameter | attributes.text | [84:21](../tools/code-map.mjs#L84) |
| variable | attributes.result | [85:11](../tools/code-map.mjs#L85) |
| variable | attributes.regex | [86:11](../tools/code-map.mjs#L86) |
| variable | attributes.match | [87:16](../tools/code-map.mjs#L87) |
| function | projectFacts | [91:1](../tools/code-map.mjs#L91) |
| parameter | projectFacts.root | [91:23](../tools/code-map.mjs#L91) |
| parameter | projectFacts.config | [91:29](../tools/code-map.mjs#L91) |
| variable | projectFacts.directory | [92:14](../tools/code-map.mjs#L92) |
| variable | projectFacts.name | [93:20](../tools/code-map.mjs#L93) |
| variable | projectFacts.source | [97:11](../tools/code-map.mjs#L97) |
| variable | projectFacts.removals | [100:11](../tools/code-map.mjs#L100) |
| variable | projectFacts.resources | [101:11](../tools/code-map.mjs#L101) |
| variable | projectFacts.match | [102:16](../tools/code-map.mjs#L102) |
| variable | projectFacts.attrs | [103:15](../tools/code-map.mjs#L103) |
| function | projectFacts.attrs.remove.split(';').filter(Boolean).map callback@6226 | [108:74](../tools/code-map.mjs#L108) |
| parameter | projectFacts.attrs.remove.split(';').filter(Boolean).map callback@6226.pattern | [108:74](../tools/code-map.mjs#L108) |
| variable | projectFacts.conditionedGroups | [115:11](../tools/code-map.mjs#L115) |
| function | projectFacts.[...source.matchAll(/&lt;ItemGroup\b([^&gt;]*)&gt;([\s\S]*?)&lt;\/ItemGroup&gt;/g)]         .some callback@6700 | [116:15](../tools/code-map.mjs#L116) |
| parameter | projectFacts.[...source.matchAll(/&lt;ItemGroup\b([^&gt;]*)&gt;([\s\S]*?)&lt;\/ItemGroup&gt;/g)]         .some callback@6700.match | [116:15](../tools/code-map.mjs#L116) |
| function | validateExcludedCompileInputs | [122:1](../tools/code-map.mjs#L122) |
| parameter | validateExcludedCompileInputs.root | [122:40](../tools/code-map.mjs#L122) |
| parameter | validateExcludedCompileInputs.config | [122:46](../tools/code-map.mjs#L122) |
| parameter | validateExcludedCompileInputs.project | [122:54](../tools/code-map.mjs#L122) |
| variable | validateExcludedCompileInputs.skipped | [123:11](../tools/code-map.mjs#L123) |
| function | validateExcludedCompileInputs.visit | [124:5](../tools/code-map.mjs#L124) |
| parameter | validateExcludedCompileInputs.visit.directory | [124:20](../tools/code-map.mjs#L124) |
| parameter | validateExcludedCompileInputs.visit.prefix | [124:31](../tools/code-map.mjs#L124) |
| parameter | validateExcludedCompileInputs.visit.excluded | [124:39](../tools/code-map.mjs#L124) |
| variable | validateExcludedCompileInputs.visit.name | [125:20](../tools/code-map.mjs#L125) |
| variable | validateExcludedCompileInputs.visit.relative | [126:19](../tools/code-map.mjs#L126) |
| variable | validateExcludedCompileInputs.visit.filename | [127:19](../tools/code-map.mjs#L127) |
| variable | validateExcludedCompileInputs.visit.stat | [127:58](../tools/code-map.mjs#L127) |
| function | validateExcludedCompileInputs.visit.project.compileRemovals.some callback@8101 | [135:53](../tools/code-map.mjs#L135) |
| parameter | validateExcludedCompileInputs.visit.project.compileRemovals.some callback@8101.pattern | [135:53](../tools/code-map.mjs#L135) |
| function | buildRole | [143:1](../tools/code-map.mjs#L143) |
| parameter | buildRole.filename | [143:20](../tools/code-map.mjs#L143) |
| parameter | buildRole.project | [143:30](../tools/code-map.mjs#L143) |
| function | buildRole.filename.split('/').slice(0, -1).some callback@8414 | [144:74](../tools/code-map.mjs#L144) |
| parameter | buildRole.filename.split('/').slice(0, -1).some callback@8414.part | [144:74](../tools/code-map.mjs#L144) |
| function | buildRole.project.compileRemovals.some callback@8583 | [146:70](../tools/code-map.mjs#L146) |
| parameter | buildRole.project.compileRemovals.some callback@8583.pattern | [146:70](../tools/code-map.mjs#L146) |
| variable | buildRole.roles | [148:11](../tools/code-map.mjs#L148) |
| function | buildRole.project.resources.filter callback@8764 | [148:44](../tools/code-map.mjs#L148) |
| parameter | buildRole.project.resources.filter callback@8764.item | [148:44](../tools/code-map.mjs#L148) |
| function | buildRole.project.resources.filter(item =&gt; !item.externalExpression && globRegex(item.include).test(filename)).map callback@8844 | [148:124](../tools/code-map.mjs#L148) |
| parameter | buildRole.project.resources.filter(item =&gt; !item.externalExpression && globRegex(item.include).test(filename)).map callback@8844.item | [148:124](../tools/code-map.mjs#L148) |
| function | bindingNames | [156:1](../tools/code-map.mjs#L156) |
| parameter | bindingNames.node | [156:23](../tools/code-map.mjs#L156) |
| function | bindingNames.node.properties.flatMap callback@9597 | [162:71](../tools/code-map.mjs#L162) |
| parameter | bindingNames.node.properties.flatMap callback@9597.property | [162:71](../tools/code-map.mjs#L162) |
| function | javascriptSymbols | [166:1](../tools/code-map.mjs#L166) |
| parameter | javascriptSymbols.source | [166:28](../tools/code-map.mjs#L166) |
| parameter | javascriptSymbols.acorn | [166:36](../tools/code-map.mjs#L166) |
| parameter | javascriptSymbols.lineOffset | [166:45](../tools/code-map.mjs#L166) |
| parameter | javascriptSymbols.columnOffset | [166:61](../tools/code-map.mjs#L166) |
| parameter | javascriptSymbols.owner | [166:79](../tools/code-map.mjs#L166) |
| parameter | javascriptSymbols.sourceType | [166:91](../tools/code-map.mjs#L166) |
| parameter | javascriptSymbols.payloadLine | [166:114](../tools/code-map.mjs#L166) |
| variable | javascriptSymbols.ast | [167:9](../tools/code-map.mjs#L167) |
| variable | javascriptSymbols.parseOptions | [168:11](../tools/code-map.mjs#L168) |
| catch-variable | javascriptSymbols.error | [170:12](../tools/code-map.mjs#L170) |
| variable | javascriptSymbols.symbols | [174:11](../tools/code-map.mjs#L174) |
| function | javascriptSymbols.add | [175:5](../tools/code-map.mjs#L175) |
| parameter | javascriptSymbols.add.node | [175:18](../tools/code-map.mjs#L175) |
| parameter | javascriptSymbols.add.kind | [175:24](../tools/code-map.mjs#L175) |
| parameter | javascriptSymbols.add.name | [175:30](../tools/code-map.mjs#L175) |
| parameter | javascriptSymbols.add.context | [175:36](../tools/code-map.mjs#L175) |
| function | javascriptSymbols.assignedName | [183:5](../tools/code-map.mjs#L183) |
| parameter | javascriptSymbols.assignedName.node | [183:27](../tools/code-map.mjs#L183) |
| parameter | javascriptSymbols.assignedName.parent | [183:33](../tools/code-map.mjs#L183) |
| function | javascriptSymbols.walk | [191:5](../tools/code-map.mjs#L191) |
| parameter | javascriptSymbols.walk.node | [191:19](../tools/code-map.mjs#L191) |
| parameter | javascriptSymbols.walk.parent | [191:25](../tools/code-map.mjs#L191) |
| parameter | javascriptSymbols.walk.context | [191:33](../tools/code-map.mjs#L191) |
| variable | javascriptSymbols.walk.children | [193:13](../tools/code-map.mjs#L193) |
| variable | javascriptSymbols.walk.name | [195:19](../tools/code-map.mjs#L195) |
| variable | javascriptSymbols.walk.parameter | [198:24](../tools/code-map.mjs#L198) |
| variable | javascriptSymbols.walk.name | [200:19](../tools/code-map.mjs#L200) |
| variable | javascriptSymbols.walk.name | [203:19](../tools/code-map.mjs#L203) |
| variable | javascriptSymbols.walk.identifier | [206:24](../tools/code-map.mjs#L206) |
| variable | javascriptSymbols.walk.identifier | [208:24](../tools/code-map.mjs#L208) |
| variable | javascriptSymbols.walk.key | [214:21](../tools/code-map.mjs#L214) |
| variable | javascriptSymbols.walk.value | [214:26](../tools/code-map.mjs#L214) |
| variable | javascriptSymbols.walk.child | [216:50](../tools/code-map.mjs#L216) |
| function | locationAt | [224:1](../tools/code-map.mjs#L224) |
| parameter | locationAt.source | [224:21](../tools/code-map.mjs#L224) |
| parameter | locationAt.offset | [224:29](../tools/code-map.mjs#L224) |
| variable | locationAt.prefix | [225:11](../tools/code-map.mjs#L225) |
| variable | locationAt.line | [226:11](../tools/code-map.mjs#L226) |
| function | htmlInventory | [230:1](../tools/code-map.mjs#L230) |
| parameter | htmlInventory.source | [230:24](../tools/code-map.mjs#L230) |
| parameter | htmlInventory.acorn | [230:32](../tools/code-map.mjs#L230) |
| variable | htmlInventory.symbols | [231:11](../tools/code-map.mjs#L231) |
| variable | htmlInventory.resources | [231:25](../tools/code-map.mjs#L231) |
| variable | htmlInventory.cursor | [232:9](../tools/code-map.mjs#L232) |
| variable | htmlInventory.start | [236:15](../tools/code-map.mjs#L236) |
| variable | htmlInventory.end | [239:19](../tools/code-map.mjs#L239) |
| variable | htmlInventory.end | [242:13](../tools/code-map.mjs#L242) |
| variable | htmlInventory.quote | [242:30](../tools/code-map.mjs#L242) |
| variable | htmlInventory.char | [244:19](../tools/code-map.mjs#L244) |
| variable | htmlInventory.match | [251:15](../tools/code-map.mjs#L251) |
| variable | htmlInventory.tag | [253:15](../tools/code-map.mjs#L253) |
| variable | htmlInventory.attrs | [253:45](../tools/code-map.mjs#L253) |
| variable | htmlInventory.at | [254:15](../tools/code-map.mjs#L254) |
| variable | htmlInventory.key | [257:20](../tools/code-map.mjs#L257) |
| variable | htmlInventory.closePattern | [259:15](../tools/code-map.mjs#L259) |
| variable | htmlInventory.close | [260:15](../tools/code-map.mjs#L260) |
| variable | htmlInventory.blockAt | [262:15](../tools/code-map.mjs#L262) |
| variable | htmlInventory.content | [263:15](../tools/code-map.mjs#L263) |
| function | flowDefinitions | [276:1](../tools/code-map.mjs#L276) |
| parameter | flowDefinitions.root | [276:26](../tools/code-map.mjs#L276) |
| parameter | flowDefinitions.config | [276:32](../tools/code-map.mjs#L276) |
| parameter | flowDefinitions.inputs | [276:40](../tools/code-map.mjs#L276) |
| variable | flowDefinitions.document | [277:11](../tools/code-map.mjs#L277) |
| variable | flowDefinitions.paths | [279:11](../tools/code-map.mjs#L279) |
| function | flowDefinitions.inputs.map callback@16813 | [279:38](../tools/code-map.mjs#L279) |
| parameter | flowDefinitions.inputs.map callback@16813.file | [279:38](../tools/code-map.mjs#L279) |
| variable | flowDefinitions.seen | [279:59](../tools/code-map.mjs#L279) |
| variable | flowDefinitions.flow | [280:16](../tools/code-map.mjs#L280) |
| variable | flowDefinitions.filename | [285:20](../tools/code-map.mjs#L285) |
| variable | flowDefinitions.assigned | [288:11](../tools/code-map.mjs#L288) |
| function | flowDefinitions.document.flows.flatMap callback@17548 | [288:53](../tools/code-map.mjs#L288) |
| parameter | flowDefinitions.document.flows.flatMap callback@17548.flow | [288:53](../tools/code-map.mjs#L288) |
| variable | flowDefinitions.input | [289:16](../tools/code-map.mjs#L289) |
| function | reviewBindings | [296:1](../tools/code-map.mjs#L296) |
| parameter | reviewBindings.root | [296:25](../tools/code-map.mjs#L296) |
| parameter | reviewBindings.config | [296:31](../tools/code-map.mjs#L296) |
| parameter | reviewBindings.inputs | [296:39](../tools/code-map.mjs#L296) |
| parameter | reviewBindings.flows | [296:47](../tools/code-map.mjs#L296) |
| variable | reviewBindings.hashes | [297:11](../tools/code-map.mjs#L297) |
| function | reviewBindings.inputs.map callback@17929 | [297:39](../tools/code-map.mjs#L297) |
| parameter | reviewBindings.inputs.map callback@17929.file | [297:39](../tools/code-map.mjs#L297) |
| variable | reviewBindings.guideHash | [298:11](../tools/code-map.mjs#L298) |
| variable | reviewBindings.inputSetHash | [302:11](../tools/code-map.mjs#L302) |
| function | reviewBindings.inputs.map callback@18271 | [302:47](../tools/code-map.mjs#L302) |
| parameter | reviewBindings.inputs.map callback@18271.file | [302:47](../tools/code-map.mjs#L302) |
| function | reviewBindings.flows.map callback@18333 | [303:41](../tools/code-map.mjs#L303) |
| parameter | reviewBindings.flows.map callback@18333.flow | [303:41](../tools/code-map.mjs#L303) |
| function | reviewBindings.flows.map callback@18333.sorted(flow.sources).map callback@18498 | [306:67](../tools/code-map.mjs#L306) |
| parameter | reviewBindings.flows.map callback@18333.sorted(flow.sources).map callback@18498.filename | [306:67](../tools/code-map.mjs#L306) |
| function | review | [310:8](../tools/code-map.mjs#L310) |
| parameter | review.root | [310:24](../tools/code-map.mjs#L310) |
| parameter | review.confirmed | [310:30](../tools/code-map.mjs#L310) |
| variable | review.config | [312:11](../tools/code-map.mjs#L312) |
| variable | review.inputs | [312:38](../tools/code-map.mjs#L312) |
| variable | review.flows | [312:71](../tools/code-map.mjs#L312) |
| function | reviewProblems | [316:1](../tools/code-map.mjs#L316) |
| parameter | reviewProblems.root | [316:25](../tools/code-map.mjs#L316) |
| parameter | reviewProblems.config | [316:31](../tools/code-map.mjs#L316) |
| parameter | reviewProblems.inputs | [316:39](../tools/code-map.mjs#L316) |
| variable | reviewProblems.flows | [317:11](../tools/code-map.mjs#L317) |
| variable | reviewProblems.expected | [318:11](../tools/code-map.mjs#L318) |
| variable | reviewProblems.actual | [320:11](../tools/code-map.mjs#L320) |
| variable | reviewProblems.problems | [322:11](../tools/code-map.mjs#L322) |
| variable | reviewProblems.id | [323:16](../tools/code-map.mjs#L323) |
| function | markdown | [329:1](../tools/code-map.mjs#L329) |
| parameter | markdown.map | [329:19](../tools/code-map.mjs#L329) |
| variable | markdown.lines | [330:11](../tools/code-map.mjs#L330) |
| function | markdown.map.files.reduce callback@20122 | [332:63](../tools/code-map.mjs#L332) |
| parameter | markdown.map.files.reduce callback@20122.sum | [332:64](../tools/code-map.mjs#L332) |
| parameter | markdown.map.files.reduce callback@20122.file | [332:69](../tools/code-map.mjs#L332) |
| variable | markdown.file | [336:16](../tools/code-map.mjs#L336) |
| variable | markdown.file | [337:16](../tools/code-map.mjs#L337) |
| variable | markdown.resource | [342:24](../tools/code-map.mjs#L342) |
| variable | markdown.symbol | [346:20](../tools/code-map.mjs#L346) |
| function | render | [352:8](../tools/code-map.mjs#L352) |
| parameter | render.root | [352:24](../tools/code-map.mjs#L352) |
| parameter | render.csharpFilename | [352:30](../tools/code-map.mjs#L352) |
| variable | render.config | [353:11](../tools/code-map.mjs#L353) |
| variable | render.inputs | [353:38](../tools/code-map.mjs#L353) |
| variable | render.project | [353:71](../tools/code-map.mjs#L353) |
| variable | render.csharp | [355:11](../tools/code-map.mjs#L355) |
| variable | render.expectedPaths | [357:11](../tools/code-map.mjs#L357) |
| function | render.inputs.filter callback@22491 | [357:41](../tools/code-map.mjs#L357) |
| parameter | render.inputs.filter callback@22491.file | [357:41](../tools/code-map.mjs#L357) |
| function | render.inputs.filter(file =&gt; /\.cs$/i.test(file.path)).map callback@22529 | [357:79](../tools/code-map.mjs#L357) |
| parameter | render.inputs.filter(file =&gt; /\.cs$/i.test(file.path)).map callback@22529.file | [357:79](../tools/code-map.mjs#L357) |
| function | render.csharp.files.map callback@22589 | [358:41](../tools/code-map.mjs#L358) |
| parameter | render.csharp.files.map callback@22589.file | [358:41](../tools/code-map.mjs#L358) |
| variable | render.parsed | [359:11](../tools/code-map.mjs#L359) |
| function | render.csharp.files.map callback@22722 | [359:45](../tools/code-map.mjs#L359) |
| parameter | render.csharp.files.map callback@22722.file | [359:45](../tools/code-map.mjs#L359) |
| variable | render.acorn | [362:11](../tools/code-map.mjs#L362) |
| variable | render.files | [363:11](../tools/code-map.mjs#L363) |
| function | render.inputs.map callback@22969 | [363:30](../tools/code-map.mjs#L363) |
| parameter | render.inputs.map callback@22969.input | [363:30](../tools/code-map.mjs#L363) |
| variable | render.inputs.map callback@22969.source | [364:15](../tools/code-map.mjs#L364) |
| variable | render.inputs.map callback@22969.symbols | [366:13](../tools/code-map.mjs#L366) |
| variable | render.inputs.map callback@22969.resources | [366:27](../tools/code-map.mjs#L366) |
| variable | render.inputs.map callback@22969.parsedFile | [368:19](../tools/code-map.mjs#L368) |
| function | render.inputs.map callback@22969.parsedFile.declarations.map callback@23535 | [371:51](../tools/code-map.mjs#L371) |
| parameter | render.inputs.map callback@22969.parsedFile.declarations.map callback@23535.symbol | [371:51](../tools/code-map.mjs#L371) |
| variable | render.inputs.map callback@22969.payload | [372:24](../tools/code-map.mjs#L372) |
| variable | render.inputs.map callback@22969.lineCount | [376:15](../tools/code-map.mjs#L376) |
| variable | render.inputs.map callback@22969.symbol | [377:20](../tools/code-map.mjs#L377) |
| function | render.inputs.map callback@22969.symbols.sort callback@24464 | [379:22](../tools/code-map.mjs#L379) |
| parameter | render.inputs.map callback@22969.symbols.sort callback@24464.a | [379:23](../tools/code-map.mjs#L379) |
| parameter | render.inputs.map callback@22969.symbols.sort callback@24464.b | [379:26](../tools/code-map.mjs#L379) |
| variable | render.map | [383:11](../tools/code-map.mjs#L383) |
| variable | render.outputs | [388:11](../tools/code-map.mjs#L388) |
| variable | render.manifest | [389:11](../tools/code-map.mjs#L389) |
| function | render.inputs.map callback@25550 | [389:61](../tools/code-map.mjs#L389) |
| parameter | render.inputs.map callback@25550.path | [389:64](../tools/code-map.mjs#L389) |
| parameter | render.inputs.map callback@25550.sha256 | [389:70](../tools/code-map.mjs#L389) |
| function | render.Object.entries(outputs).map callback@25638 | [390:46](../tools/code-map.mjs#L390) |
| parameter | render.Object.entries(outputs).map callback@25638.filename | [390:48](../tools/code-map.mjs#L390) |
| parameter | render.Object.entries(outputs).map callback@25638.content | [390:58](../tools/code-map.mjs#L390) |
| function | generate | [395:8](../tools/code-map.mjs#L395) |
| parameter | generate.root | [395:26](../tools/code-map.mjs#L395) |
| parameter | generate.csharpFilename | [395:32](../tools/code-map.mjs#L395) |
| parameter | generate.compareOnly | [395:48](../tools/code-map.mjs#L395) |
| variable | generate.outputs | [396:11](../tools/code-map.mjs#L396) |
| variable | generate.filename | [397:17](../tools/code-map.mjs#L397) |
| variable | generate.content | [397:27](../tools/code-map.mjs#L397) |
| function | check | [405:8](../tools/code-map.mjs#L405) |
| parameter | check.root | [405:23](../tools/code-map.mjs#L405) |
| variable | check.config | [406:11](../tools/code-map.mjs#L406) |
| variable | check.inputs | [406:38](../tools/code-map.mjs#L406) |
| variable | check.manifest | [408:11](../tools/code-map.mjs#L408) |
| variable | check.expected | [410:11](../tools/code-map.mjs#L410) |
| function | check.inputs.map callback@26736 | [410:41](../tools/code-map.mjs#L410) |
| parameter | check.inputs.map callback@26736.file | [410:41](../tools/code-map.mjs#L410) |
| variable | check.actual | [411:11](../tools/code-map.mjs#L411) |
| function | check.manifest.inputs.map callback@26819 | [411:48](../tools/code-map.mjs#L411) |
| parameter | check.manifest.inputs.map callback@26819.file | [411:48](../tools/code-map.mjs#L411) |
| variable | check.problems | [413:11](../tools/code-map.mjs#L413) |
| variable | check.filename | [414:16](../tools/code-map.mjs#L414) |
| function | check.manifest.outputs.map callback@27370 | [419:45](../tools/code-map.mjs#L419) |
| parameter | check.manifest.outputs.map callback@27370.file | [419:45](../tools/code-map.mjs#L419) |
| function | check.config.generatedOutputs.filter callback@27438 | [419:113](../tools/code-map.mjs#L419) |
| parameter | check.config.generatedOutputs.filter callback@27438.file | [419:113](../tools/code-map.mjs#L419) |
| variable | check.output | [420:16](../tools/code-map.mjs#L420) |
| variable | check.filename | [421:15](../tools/code-map.mjs#L421) |
| function | main | [429:1](../tools/code-map.mjs#L429) |
| variable | main.command | [430:12](../tools/code-map.mjs#L430) |
| variable | main.args | [430:24](../tools/code-map.mjs#L430) |
| variable | main.allowed | [431:11](../tools/code-map.mjs#L431) |
| variable | main.options | [432:11](../tools/code-map.mjs#L432) |
| variable | main.i | [433:14](../tools/code-map.mjs#L433) |
| variable | main.key | [435:15](../tools/code-map.mjs#L435) |
| variable | main.root | [439:11](../tools/code-map.mjs#L439) |
| catch-variable | error | [454:12](../tools/code-map.mjs#L454) |

</details>

<a id="file-fd2585028b28d5cf"></a>
## [tools/package.json](../tools/package.json)

Development tooling; 225 bytes; 7 lines.
