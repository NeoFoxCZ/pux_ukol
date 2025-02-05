namespace pux_ukol;

public class Const
{
    /// <summary>
    ///     Limit of files in folder (per folder)
    /// </summary>
    public const int MaxFileCount = 2;

    /// <summary>
    ///     Limit of size of file in bytes
    /// </summary>
    public const int MaxFileSize = 1024 * 1024 * 50; // 10 MB

    /// <summary>
    /// Cache key
    /// </summary>
    public const string CacheKey = "FileList";
}