using System.Collections.Generic;

namespace Emby.YouTubePlugin
{
    /// <summary>
    /// Maps folder names or YouTube category IDs to icon URLs for the UI.
    /// Uses OpenMoji 618x618 PNGs (CC BY-SA 4.0) for nice, large folder thumbnails.
    /// </summary>
    internal static class FolderIcons
    {
        private const string Cdn = "https://cdn.jsdelivr.net/gh/hfg-gmuend/openmoji@master/color/618x618/";

        public static readonly string WatchLater = Cdn + "2B50.png";   // ⭐
        public static readonly string Trending = Cdn + "1F525.png";  // 🔥
        public static readonly string Categories = Cdn + "1F4C2.png";  // 📂
        public static readonly string RecentlyAdded = Cdn + "1F195.png";  // 🆕
        public static readonly string Videos = Cdn + "1F4FA.png";  // 📺
        public static readonly string Shorts = Cdn + "26A1.png";   // ⚡
        public static readonly string Live = Cdn + "1F534.png";  // 🔴
        public static readonly string Search = Cdn + "1F50D.png";  // 🔍

        // Map YouTube videoCategoryId → emoji icon URL
        private static readonly Dictionary<string, string> CategoryMap = new()
        {
            { "1",  Cdn + "1F3AC.png" }, // 🎬 Film & Animation
            { "2",  Cdn + "1F697.png" }, // 🚗 Autos & Vehicles
            { "10", Cdn + "1F3B5.png" }, // 🎵 Music
            { "15", Cdn + "1F43E.png" }, // 🐾 Pets & Animals
            { "17", Cdn + "26BD.png"  }, // ⚽ Sports
            { "18", Cdn + "1F3A5.png" }, // 🎥 Short Movies
            { "19", Cdn + "2708.png"  }, // ✈️ Travel & Events
            { "20", Cdn + "1F3AE.png" }, // 🎮 Gaming
            { "21", Cdn + "1F4F9.png" }, // 📹 Videoblogging
            { "22", Cdn + "1F465.png" }, // 👥 People & Blogs
            { "23", Cdn + "1F602.png" }, // 😂 Comedy
            { "24", Cdn + "1F3AD.png" }, // 🎭 Entertainment
            { "25", Cdn + "1F4F0.png" }, // 📰 News & Politics
            { "26", Cdn + "1F527.png" }, // 🔧 Howto & Style
            { "27", Cdn + "1F393.png" }, // 🎓 Education
            { "28", Cdn + "1F52C.png" }, // 🔬 Science & Tech
            { "29", Cdn + "1F30D.png" }, // 🌍 Nonprofits & Activism
            { "30", Cdn + "1F3AC.png" },
            { "31", Cdn + "1F3AC.png" },
            { "32", Cdn + "1F3AC.png" },
            { "33", Cdn + "1F3AC.png" },
            { "34", Cdn + "1F602.png" },
            { "35", Cdn + "1F4F0.png" },
            { "36", Cdn + "1F3AD.png" },
            { "37", Cdn + "1F465.png" },
            { "38", Cdn + "1F30D.png" },
            { "39", Cdn + "1F3AC.png" },
            { "40", Cdn + "1F52C.png" },
            { "41", Cdn + "1F4FA.png" },
            { "42", Cdn + "1F3AC.png" },
            { "43", Cdn + "1F4FA.png" }, // 📺 Shows
            { "44", Cdn + "1F3AC.png" }
        };

        public static string ForCategory(string categoryId) =>
            CategoryMap.TryGetValue(categoryId, out var url) ? url : Categories;
    }
}