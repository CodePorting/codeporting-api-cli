# codeporting-api-cli

CLI client for **CodePorting AI API**. Companion tool for MCP integration — upload local project files to CodePorting storage.

## Installation

### Windows
1. Download `codeporting-api-cli-win-x64.exe` from [Releases](https://github.com/CodePorting/codeporting-api-cli/releases/latest)
2. Place it in a folder (e.g., `%USERPROFILE%\.codeporting`)
3. Add that folder to your system `PATH`
4. Restart your terminal

### macOS / Linux
1. Download the binary for your platform from [Releases](https://github.com/CodePorting/codeporting-api-cli/releases/latest):
   - Apple: `codeporting-api-cli-osx-arm64`
   - Linux x64: `codeporting-api-cli-linux-x64`
2. Rename the file to `codeporting-api-cli` and place it in `~/.codeporting/`
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


