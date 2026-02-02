# ================================
# 1️⃣ BUILD STAGE
# ================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos SOLO el csproj (cache de dependencias)
COPY ["ProductServices.csproj", "./"]

# Restauramos dependencias
RUN dotnet restore "ProductServices.csproj"

# Copiamos TODO el código fuente
COPY . .

# Publicamos en Release
RUN dotnet publish "ProductServices.csproj" -c Release -o /app/publish

# ================================
# 2️⃣ RUNTIME STAGE
# ================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copiamos SOLO el resultado compilado
COPY --from=build /app/publish .

# Puerto estándar ASP.NET
EXPOSE 80

# Arranque del microservicio
ENTRYPOINT ["dotnet", "ProductServices.dll"]

