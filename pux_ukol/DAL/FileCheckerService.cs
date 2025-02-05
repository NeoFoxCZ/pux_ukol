using Microsoft.AspNetCore.Mvc;

namespace pux_ukol.DAL;

public class FileCheckerService(ILogger<IFileCheckerService> logger) : IFileCheckerService
{
    public JsonResult?
        CheckNumberOfFilesPerFolder(string dataFolderPath)
    {
        // read subdirectories and add the main directory based on dataFolderPath (provided parameter)
        var directories = Directory
            .GetDirectories(dataFolderPath, "*", SearchOption.AllDirectories)
            .Append(dataFolderPath);

        // check the number of files in the folder
        foreach (var dir in directories)
        {
            var fileCount = Directory.GetFiles(dir, "*.*", SearchOption.TopDirectoryOnly).Length;
            if (fileCount <= Const.MaxFileCount) continue;

            logger.LogCritical(
                $"File Count exceeded in folder: {dir} ({fileCount} files, maximum is {Const.MaxFileCount})");

            var messages = new List<string>();
            messages.Add(
                $"Překročen maximální počet souborů ({Const.MaxFileCount}) ve složce: {dir} ({fileCount} souborů)");
            return new JsonResult(
                new
                {
                    messages
                });
        }

        return null;
    }


    public JsonResult? CheckFilesSize(string dataFolderPath)
    {
        var largeFiles = Directory.GetFiles(dataFolderPath, "*.*", SearchOption.AllDirectories)
            .Where(f => new FileInfo(f).Length > Const.MaxFileSize)
            .ToList();

        if (largeFiles.Count == 0) return null;
        logger.LogCritical($"There are files larger than {Const.MaxFileSize} KB: {string.Join(", ", largeFiles)}");

        var messages = new List<string>();
        messages.Add($"Nalezeny soubory s velikostí větší než {Const.MaxFileSize} KB: {string.Join(", ", largeFiles)}");

        return new JsonResult(
            new
            {
                messages
            });
    }
}