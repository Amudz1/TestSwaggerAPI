FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["TestSwaggerAPI.csproj", "./"]
RUN dotnet restore "TestSwaggerAPI.csproj"

COPY . .
RUN dotnet build "TestSwaggerAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestSwaggerAPI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "TestSwaggerAPI.dll"]