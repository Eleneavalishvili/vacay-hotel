FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "HotelManagementSystem.csproj"
RUN dotnet publish "HotelManagementSystem.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
CMD sh -c 'dotnet HotelManagementSystem.dll --urls http://0.0.0.0:${PORT:-8080}'
