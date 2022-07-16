set installDotNetEF=dotnet tool install --global dotnet-ef
start /wait %installDotNetEF%
call "Runner.bat"