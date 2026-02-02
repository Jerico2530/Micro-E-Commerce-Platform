# ================================
# 1️⃣ BUILD STAGE
# ================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos SOLO los csproj para aprovechar cache
COPY ["OrderServices.csproj", "./"]


# Restauramos dependencias
RUN dotnet restore "OrderServices.csproj"

# Copiamos todo el código
COPY . .


# Publicamos en Release
RUN dotnet publish "OrderServices.csproj" -c Release -o /app/publish

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
ENTRYPOINT ["dotnet", "OrderServices.dll"]
