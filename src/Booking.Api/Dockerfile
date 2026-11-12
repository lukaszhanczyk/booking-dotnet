FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /source
COPY ["src/Booking.Api/Booking.Api.csproj", "src/Booking.Api/"]
COPY ["src/Booking.Infrastructure/Booking.Infrastructure.csproj", "src/Booking.Infrastructure/"]
COPY ["src/Booking.Domain/Booking.Domain.csproj", "src/Booking.Domain/"]
RUN dotnet restore "src/Booking.Api/Booking.Api.csproj"
COPY . .
WORKDIR "/source/src/Booking.Api"
RUN dotnet build "./Booking.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Booking.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Booking.Api.dll"]
