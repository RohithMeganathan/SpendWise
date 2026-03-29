# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY WebApplication1/ ./WebApplication1/
WORKDIR /src/WebApplication1
RUN dotnet restore WebApplication1.sln
RUN dotnet publish WebApplication1/IncomeExpenseManagementApp.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/publish .

# Default for local runs; platforms like Render override PORT at runtime
ENV PORT=8080
ENV ASPNETCORE_URLS=http://+:${PORT}

EXPOSE 8080

ENTRYPOINT ["dotnet", "IncomeExpenseManagementApp.dll"]
