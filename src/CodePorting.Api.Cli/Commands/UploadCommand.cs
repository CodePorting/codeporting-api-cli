using System.CommandLine;
using System.Threading;
using CodePorting.Api.Cli.Exceptions;
using CodePorting.Api.Cli.Services;

namespace CodePorting.Api.Cli.Commands;

public static class UploadCommand
{
	public static Command CreateCommand()
	{
		var uidArgument = new Argument<Guid>("uid")
		{
			Description = "Project UID (Guid)",
		};

		var pathArgument = new Argument<string>("path")
		{
			Description = "Path to file or directory to upload (relative or absolute)"
		};

		var rootOption = new Option<string?>("--root")
		{
			Description = "Project root directory for preserving file structure (optional, defaults to the provided path)"
		};

		var command = new Command("upload", "Upload project files to CodePorting")
		{
			uidArgument,
			pathArgument,
			rootOption,
		};

		command.SetAction(async (parseResult, cancellationToken) 
			=> await Execute(
				parseResult.GetValue(uidArgument),
				parseResult.GetValue(pathArgument)!,
				parseResult.GetValue(rootOption),
				cancellationToken));

		return command;
	}

	private static async Task Execute(Guid projectUid, string path, string? root, CancellationToken cancellationToken)
	{
		try
		{
			var context = DetermineUploadContext(path, root);

			Console.WriteLine($"Project UID: {projectUid}");
			Console.WriteLine($"Target path: {context.TargetPath}");
			Console.WriteLine($"Project root: {context.ProjectRoot}");

			await PerformUpload(projectUid, context, cancellationToken);
		}
		catch (CliException ex)
		{
			Console.Error.WriteLine(ex.Message);
			Environment.Exit(ex.ExitCode);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine($"Upload failed: an unexpected error occurred");
			Environment.Exit(1);
		}
	}

	private record UploadContext(string TargetPath, string ProjectRoot);

	private static UploadContext DetermineUploadContext(string inputPath, string? explicitRoot)
	{
		var targetPath = ResolvePath(inputPath);

		var isTargetFile = File.Exists(targetPath);
		var isTargetDir = Directory.Exists(targetPath);
		if (!isTargetFile && !isTargetDir)
		{
			throw new CliException($"Path does not exist: {inputPath}");
		}

		string projectRoot;

		if (!string.IsNullOrEmpty(explicitRoot))
		{
			projectRoot = ResolvePath(explicitRoot);

			if (!Directory.Exists(projectRoot))
			{
				throw new CliException($"Project root does not exist: {explicitRoot}");
			}

			var relativePath = Path.GetRelativePath(projectRoot, targetPath);
			if (relativePath.StartsWith(".."))
			{
				throw new CliException($"Target path '{inputPath}' is outside of project root '{explicitRoot}'");
			}
		}
		else
		{
			projectRoot = isTargetDir
				? targetPath
				: Path.GetDirectoryName(targetPath)!;
		}

		return new UploadContext(targetPath, projectRoot);
	}

	private static string ResolvePath(string path)
	{
		try
		{
			return Path.GetFullPath(path);
		}
		catch (Exception)
		{
			throw new CliException($"Invalid path format: {path}");
		}
	}

	private static async Task PerformUpload(Guid projectUid, UploadContext context, CancellationToken cancellationToken)
  {
		var files = ScanFiles(context.TargetPath);
		if (!files.Any())
		{
			return;
		}
		
		var archivePath = CreateAndValidateArchive(files, context.ProjectRoot);

		try
		{
			await UploadToServer(projectUid, archivePath, cancellationToken);
		}
		finally
		{
			File.Delete(archivePath);
		}
	}

	private static IEnumerable<string> ScanFiles(string targetPath)
	{
		var scanner = new FileScanner();
		var files = scanner.Scan(targetPath);

		if (files.Any())
		{
			Console.WriteLine($"Found {files.Count()} files to upload");
		}
		else
		{
			Console.WriteLine("No files found to upload");
		}

		return files;
	}

	private static string CreateAndValidateArchive(IEnumerable<string> files, string projectRoot)
	{
		var archiver = new Archiver();
		var archivePath = archiver.Create(files, projectRoot);
		var archiveInfo = new FileInfo(archivePath);

		const long MaxAllowedSize = 8_192_000;

		if (archiveInfo.Length > MaxAllowedSize)
		{
			File.Delete(archivePath);
			throw new CliException($"Archive size ({FormatFileSize(archiveInfo.Length)}) exceeds the server limit of {FormatFileSize(MaxAllowedSize)}");
		}

		Console.WriteLine($"Archive created: {FormatFileSize(archiveInfo.Length)}");
		return archivePath;
	}

	private static string FormatFileSize(long bytes)
		=> bytes < 1024 
			? $"{bytes} B"
			: bytes < 1024 * 1024
				? $"{bytes / 1024.0:F1} KB"
				: $"{bytes / 1024.0 / 1024.0:F2} MB";

	private static async Task UploadToServer(Guid projectUid, string archivePath, CancellationToken cancellationToken)
	{
		var uploader = new ApiUploader();
		var response = await uploader.UploadArchive(projectUid, archivePath, cancellationToken);

		Console.WriteLine("Upload successful!");
		Console.WriteLine(response);
	}
}
