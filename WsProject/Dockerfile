FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

# Copiar csproj e restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Build da aplicacao
COPY . ./
RUN dotnet publish -c Release -o out
RUN dotnet add ApiRefibra.csproj package Swashbuckle.AspNetCore -v 5.0.0-rc2
# Build da imagem
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "ApiRefibra.dll"]

