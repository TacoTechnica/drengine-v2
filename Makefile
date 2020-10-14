# This is here to make command line Linux development easier

EXECUTABLE="bin/Debug/netcoreapp3.1/DR\ Engine\ v2"


all: $(EXECUTABLE)

$EXECUTABLE: *.cs
	dotnet msbuild

run: run_editor

run_editor: *.cs
	dotnet msbuild
	"./$(EXECUTABLE)"
run_game: *.cs
	dotnet msbuild
	"./$(EXECUTABLE)" --game

FORCE: ;
