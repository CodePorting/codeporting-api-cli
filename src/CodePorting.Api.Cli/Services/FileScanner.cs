namespace CodePorting.Api.Cli.Services;

public class FileScanner
{
	private static readonly HashSet<string> _allowedExtensions = new(
		ConstLanguage.AllLanguages.SelectMany(l => l.FileDottedExtensions),
		StringComparer.OrdinalIgnoreCase);

	public IEnumerable<string> Scan(string path)
	{
		var allFiles = File.Exists(path) 
			? [path] 
			: Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);

		return allFiles.Where(f => !ShouldIgnore(f));
	}

	private bool ShouldIgnore(string file)
	{
		var ext = Path.GetExtension(file);

		return !_allowedExtensions.Contains(ext);
	}
}
