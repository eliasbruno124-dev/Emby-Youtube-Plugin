using System;
using System.Buffers.Binary;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Emby.YouTubePlugin
{
    // Track identity is metadata, never an expiring URL, array position or itag.
    // Separate encodings of the same language/track are not extra audio tracks.
    internal static class YouTubeAudioTracks
    {
        internal const int MaximumTracks = 32;

        internal static int Index(string key)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(key));
            return 1000 + (int)(BinaryPrimitives.ReadUInt32BigEndian(hash) % 1_000_000_000);
        }

        internal static string Key(JsonElement format)
        {
            var id = Text(format, "audio_track_id");
            if (id.Length > 0) return "https:id:" + id;
            // Older resolver metadata has no track id. Do not invent a second
            // language from its encoding/bitrate; only distinguish known facts.
            return "https:metadata:" + Text(format, "language") + "\n" + Text(format, "audio_track_name");
        }

        internal static bool IsDefault(JsonElement format) => Flag(format, "audio_is_default")
            ?? (format.TryGetProperty("language_preference", out var value) && value.ValueKind == JsonValueKind.Number
                && value.TryGetDouble(out var preference) && double.IsFinite(preference) && preference > 0);

        // Only an explicit provider label is evidence; words embedded in a
        // language, identifier or arbitrary longer title are not classifiers.
        internal static bool HasExplicitOriginalLabel(string value) =>
            string.Equals(value, "Original", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "(Original)", StringComparison.OrdinalIgnoreCase)
            || value.EndsWith(" (Original)", StringComparison.OrdinalIgnoreCase);

        internal static bool IsOriginal(JsonElement format) => Flag(format, "audio_is_original")
            ?? Flag(format, "audio_has_original_label") ?? HasExplicitOriginalLabel(Text(format, "audio_track_name"));

        internal static string Title(JsonElement format)
        {
            var name = Clean(Text(format, "audio_track_name"));
            if (name.Length == 0) name = Clean(Text(format, "language"));
            if (name.Length == 0) name = "Audio";
            // Default does not prove original. Language/id suffixes likewise do
            // not establish that a translation was AI-generated.
            if (Flag(format, "audio_is_original") == true && !HasExplicitOriginalLabel(name)) name += " (Original)";
            if (Flag(format, "audio_is_auto_dubbed") == true) name += " (automatisch synchronisiert)";
            return name;
        }

        internal static string Clean(string value)
        {
            var text = new string(value.Where(c => !char.IsControl(c)).Take(256).ToArray());
            return text.Length > 0 && char.IsHighSurrogate(text[^1]) ? text[..^1] : text;
        }
        internal static string Text(JsonElement value, string field) => value.ValueKind == JsonValueKind.Object
            && value.TryGetProperty(field, out var item) && item.ValueKind == JsonValueKind.String ? item.GetString() ?? "" : "";
        internal static bool? Flag(JsonElement value, string field) => value.ValueKind == JsonValueKind.Object
            && value.TryGetProperty(field, out var item) && item.ValueKind is JsonValueKind.True or JsonValueKind.False ? item.GetBoolean() : null;
    }
}
