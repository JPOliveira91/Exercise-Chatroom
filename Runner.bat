cd .\Chatroom
::set buildCommand=msbuild Chatroom.sln /p:Configuration=Release /p:Platform="Any CPU"
::start /wait %buildCommand%
cd ..\Chatroom\Chatroom.UI
set migrationCommand=dotnet ef database update
start /wait %migrationCommand%
start dotnet watch
cd ..\..\Chatroom\Chatroom.Bot
start dotnet run
echo "Chatroom running! Click any button to close this installer."