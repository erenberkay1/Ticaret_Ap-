FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# İçinde bulunduğumuz klasördeki her şeyi kopyala
COPY . .

# Proje dosyasını belirterek derle (Dosya adındaki 'ı' harfine dikkat)
RUN dotnet restore "Ticaret_Apı.csproj"
RUN dotnet publish "Ticaret_Apı.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# DLL adının doğruluğundan emin ol (Genelde proje adıyla aynı olur)
ENTRYPOINT ["dotnet", "Ticaret_Apı.dll"]