# 1. Aşama: Derleme
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Önce .sln veya .csproj dosyasını değil, her şeyi kopyalayalım
# Çünkü klasör yapın biraz karışık görünüyor.
COPY . .

# Proje dosyasını klasörün içine girip bulmasını sağlıyoruz
RUN dotnet restore "Ticaret_Apı/Ticaret_Apı.csproj"
RUN dotnet publish "Ticaret_Apı/Ticaret_Apı.csproj" -c Release -o /app/publish

# 2. Aşama: Çalıştırma
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

# Render Portu
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# DLL adındaki 'ı' harfine dikkat! Eğer hata verirse bunu 'i' yapıp dene.
ENTRYPOINT ["dotnet", "Ticaret_Apı.dll"]