# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
# For more information, please see https://aka.ms/containercompat
# Stage 1: Base image (Linux)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Auth-with-JWT/Auth-with-JWT.csproj", "Auth-with-JWT/"]
RUN dotnet restore "./Auth-with-JWT/Auth-with-JWT.csproj"
COPY . .
WORKDIR "/src/Auth-with-JWT"
RUN dotnet build "./Auth-with-JWT.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Auth-with-JWT.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final image
FROM base AS final
WORKDIR /app
# Hardcode giá trị biến môi trường Railway vào container
ENV RAILWAY_PROJECT_NAME=just-reprieve
ENV RAILWAY_ENVIRONMENT_NAME=production
ENV RAILWAY_SERVICE_NAME=backend-sohu
ENV RAILWAY_PROJECT_ID=44ae4fbf-78e0-4d77-9bf7-5ae0c88bb1d9
ENV RAILWAY_ENVIRONMENT_ID=538d133d-2e32-4a7e-8634-991a39a5c801
ENV RAILWAY_SERVICE_ID=4df5b535-67a5-4403-b021-cd09b7210149




COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Auth-with-JWT.dll"]
