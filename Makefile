EXECUTABLE="DR Engine v2/bin/Debug/netcoreapp3.1/DR Engine"
EXECUTABLE_TEST="GameEngine/bin/Debug/netcoreapp3.1/GameEngine"


all: $(EXECUTABLE) *.cs

$EXECUTABLE: *.cs
	dotnet msbuild
$EXECUTABLE_TEST: *.cs
	dotnet msbuild

run: run_editor

run_editor: FORCE
	dotnet msbuild
	./$(EXECUTABLE)
run_game: FORCE
	dotnet msbuild
	./$(EXECUTABLE) --game
run_test: FORCE
	dotnet msbuild
	./$(EXECUTABLE_TEST)

FORCE: ;
