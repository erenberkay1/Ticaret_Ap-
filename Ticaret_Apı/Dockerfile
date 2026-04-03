FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Her şeyi kopyala
COPY . .

# Klasör ve dosya adı artık 'Ticaret_Api' (noktalı i)
RUN dotnet restore "Ticaret_Api/Ticaret_Api.csproj"
RUN dotnet publish "Ticaret_Api/Ticaret_Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# DLL adı genellikle proje adıyla aynı olur
ENTRYPOINT ["dotnet", "Ticaret_Api.dll"]