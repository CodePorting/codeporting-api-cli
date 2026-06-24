namespace CodePorting.Api.Cli.Exceptions;

public class CliException : Exception
{
	public int ExitCode { get; }

	public CliException(string message, int exitCode = 1) : base(message)
	{
		ExitCode = exitCode;
	}

	public CliException(string message, Exception innerException, int exitCode = 1) : base(message, innerException)
	{
		ExitCode = exitCode;
	}
}
