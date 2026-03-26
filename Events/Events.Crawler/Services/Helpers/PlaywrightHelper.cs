namespace Events.Crawler.Services.Helpers;

/// <summary>
/// Cross-platform helper for locating the Playwright Chromium executable.
/// Supports both Linux containers (Docker/Azure) and Windows development machines.
/// </summary>
internal static class PlaywrightHelper
{
    /// <summary>
    /// Returns the Chromium executable path, or null if not found.
    /// Resolution order:
    ///   1. PLAYWRIGHT_BROWSERS_PATH env var  (Docker image / Azure Container Apps Job)
    ///   2. LocalApplicationData/ms-playwright (Windows developer machine)
    /// </summary>
    public static string? GetChromiumExecutablePath()
    {
        // 1. Check env var — set in Dockerfile and Container Apps Job env vars
        var browsersPath = Environment.GetEnvironmentVariable("PLAYWRIGHT_BROWSERS_PATH");
        if (!string.IsNullOrEmpty(browsersPath))
        {
            var linuxPath = FindChromiumInDirectory(browsersPath, "chrome");   // Linux
            if (linuxPath != null) return linuxPath;

            var windowsPath = FindChromiumInDirectory(browsersPath, "chrome.exe"); // Windows (dev container)
            if (windowsPath != null) return windowsPath;
        }

        // 2. Fallback: Windows developer machine default location
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var windowsFallback = FindChromiumInDirectory(Path.Combine(localAppData, "ms-playwright"), "chrome.exe");

        return windowsFallback;
    }

    /// <summary>
    /// Returns true if the Chromium executable can be found on the current OS.
    /// </summary>
    public static bool IsChromiumAvailable() => GetChromiumExecutablePath() != null;

    private static string? FindChromiumInDirectory(string baseDir, string executableName)
    {
        if (!Directory.Exists(baseDir)) return null;

        var chromiumDirs = Directory.GetDirectories(baseDir, "chromium-*");
        if (chromiumDirs.Length == 0) return null;

        // Pick the latest version
        var latestDir = chromiumDirs.OrderByDescending(d => d).First();

        // Linux path: chromium-XXXX/chrome-linux/chrome
        var linuxExe = Path.Combine(latestDir, "chrome-linux", executableName);
        if (File.Exists(linuxExe)) return linuxExe;

        // Windows path: chromium-XXXX/chrome-win/chrome.exe
        var winExe = Path.Combine(latestDir, "chrome-win", executableName);
        if (File.Exists(winExe)) return winExe;

        return null;
    }
}
