FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Ottawa Tech - BackEnd/Ottawa Tech - BackEnd.csproj", "Ottawa Tech - BackEnd/"]
RUN dotnet restore "Ottawa Tech - BackEnd/Ottawa Tech - BackEnd.csproj"
COPY ["Ottawa Tech - BackEnd/", "Ottawa Tech - BackEnd/"]
RUN dotnet publish "Ottawa Tech - BackEnd/Ottawa Tech - BackEnd.csproj" -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENV PORT=80 \
    ConnectionStrings__Sqlite="Data Source=/tmp/arena.db" \
    DOTNET_EnableDiagnostics=0
EXPOSE 80
ENTRYPOINT ["dotnet", "OttawaTech.BackEnd.dll"]
