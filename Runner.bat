set installDotNetEF=dotnet tool install --global dotnet-ef
start /wait %installDotNetEF%
cd .\Chatroom
::set buildCommand=msbuild Chatroom.sln /p:Configuration=Release /p:Platform="Any CPU"
::start /wait %buildCommand%
cd ..\Chatroom\Chatroom.UI
set migrationCommand=dotnet ef database update
start /wait %migrationCommand%
cd ..\..\Chatroom\Chatroom.Bot
start dotnet run
cd ..\..\Chatroom\Chatroom.UI
start dotnet watch
echo "Chatroom running! Click any button to close this installer."