# codeporting-api-cli

CLI client for **CodePorting AI API**. Companion tool for MCP integration — upload local project files to CodePorting storage.

## Installation

### Windows
1. Download `codeporting-api-cli-win-x64.zip` from [Releases](https://github.com/CodePorting/codeporting-api-cli/releases/latest)
2. Extract `codeporting-api-cli.exe` to a folder (e.g., `%USERPROFILE%\.codeporting`)
3. Add that folder to your system `PATH`
4. Restart your terminal

### macOS / Linux
1. Download the archive for your platform from [Releases](https://github.com/CodePorting/codeporting-api-cli/releases/latest):
   - Apple Silicon: `codeporting-api-cli-osx-arm64.zip`
   - Linux x64: `codeporting-api-cli-linux-x64.zip`
2. Extract `codeporting-api-cli` to `~/.codeporting/`
3. Make it executable: `chmod +x ~/.codeporting/codeporting-api-cli`
4. Add `export PATH="$HOME/.codeporting:$PATH"` to `~/.bashrc` or `~/.zshrc`
5. Restart your terminal

### Verify
```bash
codeporting-api-cli --version
```

## Configuration

Set the `CODEPORTING_API_TOKEN` environment variable:

- **Windows:** `[Environment]::SetEnvironmentVariable("CODEPORTING_API_TOKEN", "your-token", "User")`
- **macOS/Linux:** add `export CODEPORTING_API_TOKEN="your-token"` to `~/.bashrc`

## Usage

```bash
codeporting-api-cli upload <project-uid> <local-path> [--root <root-dir>]
```

### Parameters

- `<project-uid>` (Required): The unique identifier (GUID) of your project in CodePorting workspace.
- `<local-path>` (Required): Path to the specific file or directory on your local machine that you want to upload.
- `--root <root-dir>` (Optional): Sets the project root directory. This is used to preserve the correct relative folder structure for uploaded files in the CodePorting workspace.
  - *If not specified:* Defaults to the directory of `<local-path>` (if it's a file) or `<local-path>` itself (if it's a directory).
  - *Example:* If your project is in `/projects/my-app` and you want to upload `/projects/my-app/src/utils/Helper.cs` while preserving its nested structure, run:
    ```bash
    codeporting-api-cli upload <project-uid> /projects/my-app/src/utils/Helper.cs --root /projects/my-app
    ```
    This ensures the file is placed inside `src/utils/` in your CodePorting project, rather than at the root.


