# Use SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["NotKrisp.API/NotKrisp.API.csproj", "NotKrisp.API/"]
RUN dotnet restore "NotKrisp.API/NotKrisp.API.csproj"

# Copy the rest of the code
COPY . .
WORKDIR "/src/NotKrisp.API"

# Build and publish
RUN dotnet build "NotKrisp.API.csproj" -c Release -o /app/build
RUN dotnet publish "NotKrisp.API.csproj" -c Release -o /app/publish

# Use runtime image for final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy published files from build stage
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "NotKrisp.API.dll"] 