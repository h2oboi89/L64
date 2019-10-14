@echo off
REM .NET Tools
set MSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild.exe"
set NUGET="C:\Program Files (x86)\NuGet\nuget.exe"

REM Nuget Package Tools
set TDD_TOOL=.\packages\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe
set COVERAGE_TOOL=.\packages\OpenCover.4.7.922\tools\OpenCover.Console.exe 
set COVERAGE_REPORT_TOOL=.\packages\ReportGenerator.4.3.0\tools\net47\ReportGenerator.exe

set TDD_DLL=.\UnitTests\bin\Release\UnitTests.dll
set COVERAGE_DIR=.\coverage
set COVERAGE_REPORT=%COVERAGE_DIR%\OpenCoverResults.xml


REM Build Solution
%NUGET% restore

%MSBUILD% L64.sln /v:q /nologo /t:Rebuild /p:Configuration=Release

REM Reset Coverage Directory
rd /s /q %COVERAGE_DIR%
md %COVERAGE_DIR%

%COVERAGE_TOOL% -target:%TDD_TOOL% -targetargs:"%TDD_DLL% --work=%COVERAGE_DIR%" -register:user -output:%COVERAGE_REPORT%

%COVERAGE_REPORT_TOOL% -reports:%COVERAGE_REPORT% -targetdir:%COVERAGE_DIR% -assemblyFilters:-nunit.framework;-UnitTests

