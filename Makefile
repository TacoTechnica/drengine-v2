EXECUTABLE="DR Engine v2/bin/Debug/netcoreapp3.1/DR Engine"
EXECUTABLE_TEST="GameEngine/bin/Debug/netcoreapp3.1/GameEngine"


all: $(EXECUTABLE) *.cs

$EXECUTABLE: *.cs
	dotnet msbuild
$EXECUTABLE_TEST: *.cs
	dotnet msbuild

run: run_editor

run_editor:
	dotnet msbuild
	./$(EXECUTABLE)
run_game:
	dotnet msbuild
	./$(EXECUTABLE) --game
run_test:
	dotnet msbuild
	./$(EXECUTABLE_TEST)

FORCE: ;
