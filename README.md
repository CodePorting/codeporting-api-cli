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
   - Linux x64: `codeporting-api-cli-linux-x64.zip`
   - Apple Silicon: `codeporting-api-cli-osx-arm64.zip`
2. Extract `codeporting-api-cli` to `~/.codeporting/` (or any directory in your PATH, e.g., `~/.local/bin/`)
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
codeporting-api-cli upload <project-uid> <path> [--root <root-dir>]
```

### Parameters

- `<project-uid>` (Required): The unique identifier (GUID) of your project in CodePorting workspace. Retrieve it from the `.projectinfo` file at the local project root.
- `<path>` (Required): Path to the specific directory or file on your local machine. Both absolute and relative paths are supported.
- `--root <root-dir>` (Optional): Sets the project root directory. This is used to preserve the correct relative folder structure for uploaded files in the CodePorting workspace.
  *If not specified:* Defaults to the directory of `<path>` (if it's a file) or `<path>` itself (if it's a directory).

## Examples

### Upload entire project
```bash
codeporting-api-cli upload <project-uid> "/home/user/projects/my-app"
```
All files under `/home/user/projects/my-app/` are uploaded with their folder structure preserved.

### Upload all from current directory
```bash
codeporting-api-cli upload <project-uid> "."
```
All files from current directory are uploaded with their folder structure preserved.

### Upload directory preserving its structure
```bash
codeporting-api-cli upload <project-uid> "src/utils/" --root "."
```
The folder structure is preserved (e.g., `src/utils/file.cs` -> `src/utils/file.cs` in CodePorting workspace).

### Upload single file preserving its structure
```bash
codeporting-api-cli upload <project-uid> "src/utils/file.cs" --root "."
```
The folder structure is preserved (e.g., `src/utils/file.cs` -> `src/utils/file.cs` in CodePorting workspace).
