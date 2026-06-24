using System.CommandLine;
using CodePorting.Api.Cli.Commands;

var rootCommand = new RootCommand("CodePorting CLI utility");
var uploadCommand = UploadCommand.CreateCommand();

rootCommand.Add(uploadCommand);

var parseResult = rootCommand.Parse(args);

return await parseResult.InvokeAsync();
