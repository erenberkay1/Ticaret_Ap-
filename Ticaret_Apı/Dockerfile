FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Her şeyi kopyala
COPY . .

# Artık her şey 'App' klasöründe ve 'App.csproj' adında
RUN dotnet restore "App/App.csproj"
RUN dotnet publish "App/App.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# DLL adı artık App.dll oldu
ENTRYPOINT ["dotnet", "App.dll"]