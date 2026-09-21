# Hotels Management System

ASP.NET Core Web API for hotel, room, manager, guest, and reservation management.

1. Install Visual Studio 2022 with the ASP.NET and web development workload and SQL Server LocalDB.
2. Open `HotelManagementSystem.csproj` in Visual Studio and allow NuGet restore to finish. The project automatically restores its packages from the project file.
3. In Visual Studio, open **Tools > Command Line > Developer PowerShell** in the project folder and run:

   ```powershell``
   dotnet user-secrets set "Smtp:SenderEmail" "your-sender-address@gmail.com"
   dotnet user-secrets set "Smtp:Password" "your-new-16-character-gmail-app-password"
   dotnet tool install --global dotnet-ef
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. Press **F5**. Swagger opens at `/swagger`.

Do not add the Gmail App Password to `appsettings.json`, source code, Git, or a ZIP file. The sender mailbox can send verification and login codes to any user's valid email address.
# Vacay — Docker demonstration

## Run on Docker with PostgreSQL

1. Install and open Docker Desktop.
2. Open a terminal in this folder.
3. Run `docker compose up --build`.
4. Open `http://localhost:8080`.
5. Open `http://localhost:8080/swagger` to demonstrate the API.

This starts two Linux containers: the Vacay ASP.NET application and PostgreSQL. The database is stored in the named Docker volume `vacay_postgres_data`, so it remains after the containers stop.

To stop it, press `Ctrl+C`. To run again later: `docker compose up`.
