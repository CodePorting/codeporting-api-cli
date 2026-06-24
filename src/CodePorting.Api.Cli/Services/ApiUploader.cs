using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using CodePorting.Api.Cli.Exceptions;

namespace CodePorting.Api.Cli.Services;

public class ApiUploader
{
	private static readonly HttpClient _httpClient = new();
	private readonly string _apiBaseUrl;
	private readonly string _apiToken;

	public const string ApiUrlEnvironmentVariable = "CODEPORTING_API_URL";
	public const string ApiTokenEnvironmentVariable = "CODEPORTING_API_TOKEN";
	public const string DefaultApiBaseUrl = "https://api.codeporting.ai";
	public const string ApiVersionPrefix = "v1";
	public const string TrustedApiProblemType = "https://api.codeporting.ai/errors/workspace";

	public ApiUploader()
	{
		_apiBaseUrl = Environment.GetEnvironmentVariable(ApiUrlEnvironmentVariable) ?? DefaultApiBaseUrl;
		
		_apiToken = Environment.GetEnvironmentVariable(ApiTokenEnvironmentVariable) ?? "";
		if (string.IsNullOrEmpty(_apiToken))
		{
			throw new CliException($"API access token is not set. Please set {ApiTokenEnvironmentVariable} environment variable.");
		}
	}

	public async Task<string> UploadArchive(Guid projectUid, string archivePath, CancellationToken cancellationToken)
	{
		var uploadUrl = BuildUploadUrl(projectUid);
		using var content = CreateMultipartContent(archivePath);

		using var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl);
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);
		request.Content = content;

		HttpResponseMessage response;
		try
		{
			response = await _httpClient.SendAsync(request, cancellationToken);
		}
		catch (HttpRequestException)
		{
			throw new CliException("Network error. Check your connection.");
		}

		using (response)
		{
			await EnsureSuccessAsync(response, cancellationToken);
			return await response.Content.ReadAsStringAsync(cancellationToken);
		}
	}

	private string BuildUploadUrl(Guid projectUid)
		=> $"{_apiBaseUrl.TrimEnd('/')}/{ApiVersionPrefix}/workspace/project/{projectUid}/files/archive";

	private static MultipartFormDataContent CreateMultipartContent(string archivePath)
	{
		var fileStream = File.OpenRead(archivePath);
		var fileContent = new StreamContent(fileStream);
		fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-zip-compressed");

		var content = new MultipartFormDataContent
		{
		  { fileContent, "archive", "project.zip" }
		};

		return content;
	}

	private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
	{
		if (response.IsSuccessStatusCode)
		{
			return;
		}

		var serverReason = response.StatusCode switch
		{
			HttpStatusCode.Unauthorized => "Authentication failed. Check your API token.",
			HttpStatusCode.RequestEntityTooLarge => "Archive is too large for the server.",
			_ => await ExtractServerMessage(response, cancellationToken)
		};

		throw new CliException($"Server rejected the upload: {serverReason}");
	}

	private static async Task<string> ExtractServerMessage(HttpResponseMessage response, CancellationToken cancellationToken)
	{
		var body = await response.Content.ReadAsStringAsync(cancellationToken);

		try
		{
			using var doc = JsonDocument.Parse(body);
			var root = doc.RootElement;

			if (root.TryGetProperty("type", out var typeElement))
			{
				var type = typeElement.GetString();
				if (type != null && type.StartsWith(TrustedApiProblemType, StringComparison.Ordinal))
				{
					if (root.TryGetProperty("detail", out var detailElement))
					{
						var detail = detailElement.GetString();
						if (detail != null)
						{
							return detail;
						}
					}
				}
			}
		}
		catch (Exception)
		{
		}

		return $"Unexpected server response.";
	}
}
