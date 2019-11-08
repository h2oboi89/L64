################################################################################
# Makefile for building and cleaning the solution.
################################################################################
SHELL := C:\Windows\System32\cmd.exe
ZIP_TOOL := 7z.exe
PACKAGE_ROOT := $(shell echo $(USERPROFILE))\.nuget\packages

COMPILER := msbuild.exe

TDD_TOOL := $(PACKAGE_ROOT)\nunit.consolerunner\3.10.0\tools\nunit3-console.exe
TDD_DIR := .\OpenCover

COVERAGE_TOOL := $(PACKAGE_ROOT)\opencover\4.7.922\tools\OpenCover.Console.exe
COVERAGE_REPORT_TOOL := $(PACKAGE_ROOT)\reportgenerator\4.3.6\tools\net47\ReportGenerator.exe
COVERAGE_REPORT := $(TDD_DIR)\results.xml

GIT_LONG_HASH:=$(shell git rev-parse HEAD)
GIT_SHORT_HASH:=$(shell git rev-parse --short HEAD)

CONFIG := Debug
ZIP_TAG := $(GIT_SHORT_HASH)

SOLUTION := L64
SOLUTION_FILE := .\$(SOLUTION).sln

RELEASE_DIR := .\Release

# Zips a the specified files and move it to the specified location
# $1 Files to be zipped
# $2 Name of the zip file
# $3 Location to place the zip file
# Pre - If zipping on Windows 'ZIP_TOOL' needs to be defined
define zip_files
	if not ["$1"]==[] ($(subst /,\,$(ZIP_TOOL)) a -tzip -mx9 $2 $(subst /,\,$1) && @move $2 $3) ELSE ( echo Error: No files && exit 1 )
endef

# Deletes directory if it exists
# $1 Directory to delete
define delete_dir
	@if EXIST $1 rmdir $1 /s /q;
endef

# Creates a directory if it does not exist
# $! Directory to create
define make_dir
	@if NOT EXIST $1 mkdir $1;
endef

# Copies all output files for a specified project to $(RELEASE_DIR)
# $1 Name of project to copy files for
define copy_to_release
    $(call copy_to_folder,$1,$(RELEASE_DIR)\$1)
	$(call copy_to_folder,$1,$(RELEASE_DIR)\All)
endef

# Copies contents of bin\<CONFIG> folder to another folder
# $1 is folder to copy from
# $2 is folder to copy to
define copy_to_folder
	@echo $1
	@echo $2
	@(robocopy .\$1\bin\$(CONFIG) $2 /S /NFL /NDL /NJH /NJS /NC /NS /NP) ^& if %ERRORLEVEL% leq 1 exit 0
endef

# Files that are copied to the artifacts folder.
# Note: starting with ".\" copies just the file and not the path to the artifacts folder.
PACKAGE_CONTENTS_SOLUTION =\
	$(RELEASE_DIR)\*

PACKAGE_CONTENTS_COVERAGE :=\
	$(TDD_DIR)\*

TESTS =\
	.\UnitTests\bin\$(CONFIG)\UnitTests.dll

OPENCOVER_ASSEMBLY_FILTER =\
	-nunit.framework;-UnitTests

# Default rule.
.PHONY: all
all: build

# This rule builds the solution
.PHONY: build
build:
	@echo _
	@echo -----------------------------------
	@echo Building Solution ($(CONFIG)) ...
	@echo -----------------------------------
	$(COMPILER) $(SOLUTION_FILE) -v:m -nologo -t:Rebuild -p:Configuration=$(CONFIG) -restore -m -nr:False

# This rule runs nunit and coverage
.PHONY: tdd
tdd: build
	@echo _
	@echo -----------------------------------
	@echo Running TDD tests w/ coverage ...
	@echo -----------------------------------
	$(call delete_dir,$(TDD_DIR))
	@md $(TDD_DIR)
	$(COVERAGE_TOOL) -target:$(TDD_TOOL) -targetargs:"$(TESTS) --work=$(TDD_DIR)" -register:user -output:$(COVERAGE_REPORT)
	$(COVERAGE_REPORT_TOOL) -reports:$(COVERAGE_REPORT) -targetdir:$(TDD_DIR) -assemblyFilters:$(OPENCOVER_ASSEMBLY_FILTER) -verbosity:Warning -tag:$(GIT_LONG_HASH)

# This rule copies build output files to $(RELEASE_DIR)
# NOTE: this will be Release or Debug depending on build configuration
.PHONY: generate_release_contents
generate_release_contents: build
	@echo _
	@echo -----------------------------------
	@echo Copying to $(RELEASE_DIR) ...
	@echo -----------------------------------
	$(call copy_to_release,Cryptography)

# This rule packages a build.
.PHONY: package
package: generate_release_contents
	@echo _
	@echo -----------------------------------
	@echo Zipping up artifacts ...
	@echo -----------------------------------
	$(call make_dir,artifacts)
	$(call zip_files,$(PACKAGE_CONTENTS_SOLUTION),$(SOLUTION)_$(CONFIG)_$(ZIP_TAG).zip,artifacts)

# Builds a release build.
.PHONY: release
release: CONFIG := Release
release: tdd package

# Builds a debug build.
.PHONY: debug
debug: tdd package

# This rule cleans the project (removes binaries, etc).
.PHONY: clean
clean:
	@echo _
	@echo -----------------------------------
	@echo Cleaning solution and artifacts ...
	@echo -----------------------------------
	$(call delete_dir,artifacts)
	$(call delete_dir,$(RELEASE_DIR))
	$(COMPILER) $(SOLUTION_FILE) -v:m -nologo -t:Clean -p:Configuration=Debug -m -nr:False
	$(COMPILER) $(SOLUTION_FILE) -v:m -nologo -t:Clean -p:Configuration=Release -m -nr:False
