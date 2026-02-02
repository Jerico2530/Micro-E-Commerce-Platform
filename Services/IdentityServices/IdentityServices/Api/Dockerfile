# ================================
# 1️⃣ BUILD STAGE
# ================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos SOLO los csproj para aprovechar cache
COPY ["IdentityServices.csproj", "./"]

# Restauramos dependencias (EF, Npgsql, Swagger, JWT, etc.)
RUN dotnet restore "IdentityServices.csproj"

# Copiamos todo el código
COPY . .

# Publicamos en Release
RUN dotnet publish "IdentityServices.csproj" -c Release -o /app/publish

# ================================
# 2️⃣ RUNTIME STAGE
# ================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copiamos SOLO el resultado compilado
COPY --from=build /app/publish .

# Exponemos el puerto
EXPOSE 80

# Arranque del microservicio
ENTRYPOINT ["dotnet", "IdentityServices.dll"]
