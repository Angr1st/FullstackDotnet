# Fullstack Dotnet

C# Asp.Net Core Backend + C# Blazor WebAssembly Frontend + MS SQL Database + F# Argument Parsing

## Description
This solution is comprised of multiple projects:
- ArgumentParsing.fsproj: Like the name implies this provides argument parsing functionality
- Frontend.csproj: This is the blazor web assembly frontend
- Lib.Shared.csproj: This contains common logic and is shared between Frontend and Server
- Server.csproj: This is the ASP.Net Core Backend. This hosts the Frontend and the gRPC Service used for most of the communication between front and backend
- Logic.Tests.cs: This is a unit test project testing the common functionality
- SimpleDatabase.sqlproj: This is the Sql Database Definition used by Visual Studio
- SimpleDatabase.Sdk.sqlproj: This is basically the same as SimpleDatabase.sqlproj but uses the new SDK-Style format and is buildable using `dotnet` command line tool

### Diagram
[SimplifiedArchitecture](./Diagram/FullstackDotnet.svg) created with draw.io. Unfortunatly can only be viewed locally in a browser.

### Endpoint Description
- StatusController a API Controller exposing a GET Endpoint. This is called by the Frontend to validate that the Server is online
- MessageService is a gRPC Service implementation. The Server uses GRPC.Web to enable the Frontend to call it, as browsers are currently incapable of actually sending gRPC requests. The MessageService exposes two Endpoints:
  - GetMessages which returns a Stream of messages
  - SendMessage which takes a `MessageRecord` and saves it to the Database

## How to build
Execute the `build.ps1` script at the root of the repository. This also runs the tests.

### Prerequisites:
- net 7
- `dotnet workload install wasm-tools`
- SqlServer 2016 or newer

## How to run
Execute the `launch.ps1` script at the root of the repository. This script takes a database connection string as parameter. If you don't supply your Database Connectionstring, it will try the default `Server=(localdb)\mssqllocaldb;Database=SimpleDatabase`. The script does these things:
1. Download SqlPackage.exe
2. Unzip SqlPackage.exe
3. Run `dotnet publish -c Release` to build all projects except the SqlDb ones.
4. Hide all Blazor .dll by switching them to .bin files
5. Build and deploy the .dacpac file onto the Database Server
6. Start the Server.exe

### Troubleshooting 'integrity' Errors
If the blazor frontend is not loading in your browser, try setting the `BlazorCacheBootResources` property in the `Frontend.csproj` to false and launching it again. You can also try running the integrity.ps1 script to find errors.