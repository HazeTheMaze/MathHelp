# Mattehjälpen

A math learning tool for generating multiplication tables and practice sheets as PDFs.

## Features

- **Multiplication Tables** - Generate reference sheets with all times tables (1-12)
- **Practice Sheets** - Create randomized practice problems (100 per sheet) with answer keys
- **PDF Export** - Print-ready A4 format with clear, readable layout

## Running the App

### Option 1: Installer

Download and run `installer.exe` to install the application.

### Option 2: From Source

```bash
cd MathHelpApp
dotnet run
```

The app opens automatically at `http://localhost:5000`.

## Development

### Prerequisites

- .NET 10.0 SDK

### Build

```bash
dotnet build
```

### Run Tests

```bash
dotnet test
```

### Code Quality

This project enforces code quality with:

- **Roslynator** and **Meziantou.Analyzer** for static analysis
- **EditorConfig** for consistent code style
- **Warnings as errors** - all warnings must be resolved

Run `dotnet format` to auto-fix formatting issues.

## Project Structure

```text
MathHelpApp/
├── Components/       # Blazor pages and layouts
├── Models/           # Data models
├── Services/         # Business logic (PDF generation, problem generation)
└── wwwroot/          # Static assets

MathHelpApp.Tests/    # Unit tests (NUnit + Shouldly)
```

## License

MIT License - see [LICENSE](LICENSE) for details.

Note: This project uses [QuestPDF](https://www.questpdf.com/) under the Community License.
