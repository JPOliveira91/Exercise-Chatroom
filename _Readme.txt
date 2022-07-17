Pre-requisites

- RabbitMQ Server
- MS SQL Server
- .NET 6
- Visual Studio

How to Run with Installer

	01) Execute "Runner.bat"
	02) Wait until browser opens in Login page of Chatroom.UI

How to Run with Visual Studio

	01) Open Visual Studio
	02) Open "File -> Open -> Project/Solution"
	03) Select "\Chatroom\Chatroom.sln"
	04) Build Solution
	05) Open "Tools -> NuGet Package Manager - > Package Manager Console"
	06) In Package Manager Console, run the following command to create the Database: "Update-Database"
	07) Open "View -> Terminal"
	08) Run command "cd Chatroom.Bot"
	09) Run command "dotnet run"
	10) Open new "View -> Terminal"
	11) Run command "cd Chatroom.UI"
	12) Run command "dotnet run"
	13) Open browser and go to "https://localhost:7221" or "http://localhost:5221"