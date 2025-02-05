using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using pux_ukol.DAL;

namespace pux_ukol.Controllers;

public class FileViewerController(IMemoryCache cache, ILogger<FileViewerController> logger, IWebHostEnvironment env,
    IFileCheckerService service)
    : Controller
{

    /// <summary>
    /// Check for changes in the folder and return the result
    /// </summary>
    /// <param name="dataFolderPath">Path to root folder</param>
    /// <returns>
    /// JSON object with messages about changes
    /// </returns>
    [HttpPost]
    public IActionResult CheckFileChanges(string dataFolderPath)
    {
        // If the path starts with "~", it is replaced with the path to the wwwroot folder
        if (dataFolderPath.StartsWith("\u007e"))
            dataFolderPath = Path.Combine(env.WebRootPath, dataFolderPath.TrimStart('\u007e', '/'));

        logger.LogDebug("CheckFileChanges method called.");
        logger.LogDebug($"DataFolderPath: {dataFolderPath}");
        
        // Received list of files from cache
        var oldFileData =
            cache.Get<Dictionary<string, (int? version, DateTime lastModified)>>("FileList") ??
            new Dictionary<string, (int? version, DateTime lastModified)>();
        
        #region Check Rules

        service.CheckNumberOfFilesPerFolder(dataFolderPath);
        
        var checkFilesSizeResponse = service.CheckFilesSize(dataFolderPath);
        if (checkFilesSizeResponse != null) return checkFilesSizeResponse;

        #endregion


        // Get the current list of files from the folder
        // note: the version of the file is increased by 1 if the file has been modified
        var newFileData =
            Directory.GetFiles(dataFolderPath, "*.*", SearchOption.AllDirectories)
                .Where(file => new FileInfo(file).Length <= Const.MaxFileSize)
                .ToDictionary(
                    f => Path.GetRelativePath(dataFolderPath, f),
                    f =>
                    {
                        var lastModified = System.IO.File.GetLastWriteTime(f);
                        return oldFileData.ContainsKey(Path.GetFileName(f))
                            ? (oldFileData[Path.GetFileName(f)].Item1, lastModified)
                            : (1, lastModified);
                    }
                );


        var addedFiles = newFileData.Keys.Except(oldFileData.Keys).ToList();
        var removedFiles = oldFileData.Keys.Except(newFileData.Keys).ToList();

        // Check modified files and increase the version by 1
        foreach (var file in newFileData.Keys.ToList()) // ToList() to avoid InvalidOperationException
            if (oldFileData.TryGetValue(file, out var value) && value.lastModified != newFileData[file].lastModified)
                newFileData[file] = (value.Item1 + 1, newFileData[file].lastModified);
            else if
                (oldFileData.TryGetValue(file,
                    out var valueFirstLoaded)) // If the file was not modified, the version is not increased
                newFileData[file] = (valueFirstLoaded.version ?? 1, newFileData[file].lastModified);

        // List of modified files with new versions
        var editedFiles = newFileData
            .Where(f => oldFileData.ContainsKey(f.Key) && oldFileData[f.Key].lastModified != f.Value.lastModified)
            .Select(f => $"{f.Key} (v{f.Value.Item1})")
            .ToList();

        // Log messages about all changes
        var logMessages = new List<string>();

        if (editedFiles.Count != 0) editedFiles.ForEach(f => logMessages.Add($"[M] {f}"));
        if (addedFiles.Count != 0) addedFiles.ForEach(f => logMessages.Add($"[A] {f}"));
        if (removedFiles.Count != 0) removedFiles.ForEach(f => logMessages.Add($"[R] {f}"));
        if (addedFiles.Count == 0 && removedFiles.Count == 0 && editedFiles.Count == 0) logMessages.Add("Žádné změny.");

        // Update cache to the current state of files
        cache.Set("FileList", newFileData);

        logger.LogInformation("Kontrola změn v adresáři dokončena.");
        return Json(new { messages = logMessages });
    }
}