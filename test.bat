@echo off
REM .NET Tools
set MSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild.exe"
set NUGET="C:\Program Files (x86)\NuGet\nuget.exe"

REM Nuget Package Tools
set TDD_TOOL=.\packages\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe
set COVERAGE_TOOL=.\packages\OpenCover.4.7.922\tools\OpenCover.Console.exe 
set COVERAGE_REPORT_TOOL=.\packages\ReportGenerator.4.3.0\tools\net47\ReportGenerator.exe

set TDD_DLL=.\UnitTests\bin\Release\UnitTests.dll
set OPEN_COVER_DIR=.\OpenCover
set COVERAGE_REPORT=%OPEN_COVER_DIR%\OpenCoverResults.xml

REM Build Solution
%NUGET% restore

%MSBUILD% L64.sln /v:q /nologo /t:Rebuild /p:Configuration=Release

REM Reset Coverage Directory
rd /s /q %OPEN_COVER_DIR%
md %OPEN_COVER_DIR%

REM Run unit tests and code coverage
%COVERAGE_TOOL% -target:%TDD_TOOL% -targetargs:"%TDD_DLL% --work=%OPEN_COVER_DIR%" -register:user -output:%COVERAGE_REPORT%

%COVERAGE_REPORT_TOOL% -reports:%COVERAGE_REPORT% -targetdir:%OPEN_COVER_DIR% -assemblyFilters:-nunit.framework;-UnitTests

