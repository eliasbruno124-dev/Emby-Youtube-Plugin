# Managed resolver source subset

This directory contains a modified, pinned MIT-licensed subset of YoutubeExplode
6.6.3, commit `d8b1928bf32802c43532465f4d202d066ce77667`. `Upstream.json`
records the public source archive, upstream file hashes, resulting file hashes
and transformations. `License.txt` retains the upstream copyright and license.
These C# sources are compiled directly into the plugin DLL; this is not the
unmodified official NuGet binary. `Compatibility.cs` provides the small internal
framework-only helpers needed by the selected files. No dependency assembly,
native binary, downloaded player script or external program is loaded.

The subset retains one fixed VISIONOS player request and the upstream visitor
bootstrap. Android/TV player fallback, age-gate fallback, package initializers,
process/native utilities, stream download clients and per-format probes are not
included. Static numeric extensions are lowered to explicit C#12 helper calls.
The build target is net6.0; SDK8 suffices for compilation.

`VideoController.GetPlayerResponseAsync` returns one `PlayerResponse.Snapshot`.
The plugin adapter maps this single snapshot to actual HTTPS representations,
the separate actual HLS manifest URL, duration/description and original caption
tracks. It does not synthesize format lengths, codec profiles, translated
captions or chapters. HTTPS formats require exact `clen`/`filesize` agreement;
signature-cipher-only and unknown-codec representations are not eligible.

The chapter adapter reads actual `chapterRenderer` markers. When absent, it may
make one optional four-second watch-metadata request and read `ytInitialData`
chapter renderers or explicitly identified chapter panels. Description timecodes
alone are not chapters. Missing data does not instruct the existing metadata
layer to delete stored chapters. Caption URLs request the original track as VTT;
the existing subtitle service must still fetch and validate cues before exposing
the track to a native player.

The HTTP boundary admits only the exact visitor, player and selected-video watch
paths on `www.youtube.com`. It uses the plugin's HTTPS/public-address-pinned
socket transport without cookies, proxies, redirect following or decompression.
Requests and response bytes are bounded. HTTP403/429 or non-OK player status end
the flight. Only HTTP403/429 or an explicit bot challenge engages the existing
shared cooldown; an unavailable or account/age/country-restricted video does not
pause unrelated videos. No client, account, cookie or network-identity fallback
follows a denial. Actual DRC audio variants have a distinct `.drc` format suffix;
malformed or otherwise ambiguous representations are excluded.

An SDK build, schema fixture or small transport probe does not prove decoded
native playback, all client/platform support, HLS eligibility or seek behavior.
Those remain integration responsibilities of the playback layer.
