build:
	dotnet restore --use-lock-file
	dotnet build --no-restore

clean:
	dotnet clean

fix:
	dotnet format

test:
	dotnet test
	
clear-cache:
	dotnet nuget locals all --clear