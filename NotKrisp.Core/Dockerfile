# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview-alpine AS build
WORKDIR /src

# Copy only the project file first
COPY *.csproj .
RUN dotnet restore -r linux-musl-x64

# Copy the rest and publish
COPY . .
RUN dotnet publish \
    --configuration Release \
    --runtime linux-musl-x64 \
    --self-contained true \
    --no-restore \
    /p:PublishTrimmed=true \
    /p:PublishSingleFile=true \
    -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-preview-alpine
WORKDIR /app

# Add non-root user for security
RUN adduser -u 1000 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Copy only the published files
COPY --from=build --chown=appuser:appuser /app/publish .

ENV ASPNETCORE_URLS=http://+:5149

EXPOSE 5149
ENTRYPOINT ["./NotKrisp.Core"] 