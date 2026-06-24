namespace CodePorting.Api.Cli;

public class CodeLanguage(string name, string fileExtensions)
{
	public string Name { get; set; } = name;

	public IReadOnlySet<string> FileExtensions { get; } = ParseFileExtensions(fileExtensions);

	public IEnumerable<string> FileDottedExtensions
		=> FileExtensions.Select(e => $".{e}");

	private static HashSet<string> ParseFileExtensions(string fileExtensions)
		=> fileExtensions
			.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Select(e => e.TrimStart('.').ToLowerInvariant())
			.Where(e => e.Length > 0)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
}
