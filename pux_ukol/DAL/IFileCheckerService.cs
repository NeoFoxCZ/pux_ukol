using Microsoft.AspNetCore.Mvc;

namespace pux_ukol.DAL;

public interface IFileCheckerService
{
    /// <summary>
    ///     Check if there are files larger than the limit
    /// </summary>
    /// <param name="dataFolderPath">Path to folder</param>
    /// <returns>
    ///    JSON object with error message if there are files larger than the limit, otherwise null
    /// </returns>
    JsonResult? CheckFilesSize(string dataFolderPath);

    /// <summary>
    ///     Check if the number of files in the folder does not exceed the limit, based on the <see cref="MaxFileCount"/> constant
    /// </summary>
    /// <param name="dataFolderPath">Path to folder</param>
    JsonResult? CheckNumberOfFilesPerFolder(string dataFolderPath);
}