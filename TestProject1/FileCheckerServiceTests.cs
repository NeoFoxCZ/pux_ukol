using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using pux_ukol;
using pux_ukol.DAL;

public class FileCheckerServiceTests
{
    private readonly Mock<ILogger<IFileCheckerService>> _mockLogger;
    private readonly IFileCheckerService _service;
    private readonly string _testDirectory;

    public FileCheckerServiceTests()
    {
        _mockLogger = new Mock<ILogger<IFileCheckerService>>();
        _service = new FileCheckerService(_mockLogger.Object);

        // Create test directory
        _testDirectory = Path.Combine(Path.GetTempPath(), "FileCheckerServiceTest");
        Directory.CreateDirectory(_testDirectory);
    }

    [Fact]
    public void CheckNumberOfFilesPerFolder_ReturnsNull_WhenFileCountIsWithinLimit()
    {
        // remove all files
        Directory.GetFiles(_testDirectory).ToList().ForEach(File.Delete);
        
        // Arrange: create files within limit
        for (int i = 0; i < Const.MaxFileCount; i++)
        {
            var file = Path.Combine(_testDirectory, $"file{i}.txt");
            File.WriteAllBytes(file, new byte[1024]); // 1 KB
        }

        // Act
        var result = _service.CheckNumberOfFilesPerFolder(_testDirectory);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void CheckNumberOfFilesPerFolder_ReturnsErrorJson_WhenFileCountExceedsLimit()
    {
        // Arrange: create files exceeding limit
        for (int i = 0; i < Const.MaxFileCount + 1; i++)
        {
            var file = Path.Combine(_testDirectory, $"file{i}.txt");
            File.WriteAllBytes(file, new byte[1024]); // 1 KB
        }

        // Act
        var result = _service.CheckNumberOfFilesPerFolder(_testDirectory) as JsonResult;

        // Assert
        Assert.NotNull(result);
        var messages = result.Value.GetType().GetProperty("messages").GetValue(result.Value) as List<string>;
        Assert.Contains("Překročen maximální počet souborů", messages[0]);
    }

    [Fact]
    public void CheckFilesSize_ReturnsNull_WhenAllFilesAreWithinSizeLimit()
    {
        // Arrange: create files within size limit
        for (int i = 0; i < 5; i++)
        {
            var file = Path.Combine(_testDirectory, $"file{i}.txt");
            File.WriteAllBytes(file, new byte[Const.MaxFileSize - 1]); // Just below limit
        }

        // Act
        var result = _service.CheckFilesSize(_testDirectory);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void CheckFilesSize_ReturnsErrorJson_WhenFilesExceedSizeLimit()
    {
        // Arrange: create a file exceeding size limit
        var largeFile = Path.Combine(_testDirectory, "large.txt");
        File.WriteAllBytes(largeFile, new byte[Const.MaxFileSize + 1]); // Exceeds limit

        // Act
        var result = _service.CheckFilesSize(_testDirectory) as JsonResult;

        // Assert
        Assert.NotNull(result);
        var messages = result.Value.GetType().GetProperty("messages").GetValue(result.Value) as List<string>;
        Assert.Contains("Nalezeny soubory s velikostí větší než", messages[0]);
    }

    public void Dispose()
    {
        Directory.Delete(_testDirectory, true);
    }
}
