# Test Projects

This directory contains standalone C# test projects that can be executed directly with the .NET SDK. The projects use .NET's file-based app support, so no `.csproj` file is required for the examples.

## Prerequisites

- Install the .NET 10 SDK or a later compatible SDK.
- Open a terminal in this directory.

You can verify the installed SDK with:

```bash
dotnet --version
```

## Running a demo project

Run a C# file by passing its path to `dotnet run`:

```bash
dotnet run 01_HelloWorld.cs
```

The first run restores the packages declared at the top of the C# file and then starts the example. For another demo file, replace `01_HelloWorld.cs` with the file name:

```bash
dotnet run <file-name>.cs
```

## Example: `01_HelloWorld.cs`

The example starts a hosted background service and registers two jobs. Once the application is running:

- Press `1` to execute `UniqueJobName_1`.
- Press `2` to execute `UniqueJobName_2`.
- Press any other key to display an error message.
- Press `Ctrl+C` to stop the application.

The job output is written to the console by the configured logging provider. Package references are restored automatically from the `#: package` declarations in the source file, so no manual package installation is needed.