using System.IO.Compression;

namespace CodePorting.Api.Cli.Services;

public class Archiver
{
	public string Create(IEnumerable<string> files, string rootPath)
	{
		var tempFile = Path.Combine(Path.GetTempPath(), $"project_{Guid.NewGuid()}.zip");

		using (var archive = ZipFile.Open(tempFile, ZipArchiveMode.Create))
		{
			foreach (var file in files)
			{
				var entryName = Path.GetRelativePath(rootPath, file);
				archive.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal);
			}
		}

		return tempFile;
	}
}
