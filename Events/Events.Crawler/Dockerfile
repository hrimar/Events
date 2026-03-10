# =============================================================
# Stage 1: Build
# =============================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and all referenced project files first (better layer caching)
COPY Events.Web.sln ./
COPY Events.Crawler/Events.Crawler.csproj   ./Events.Crawler/
COPY Events.Models/Events.Models.csproj     ./Events.Models/
COPY Events.Data/Events.Data.csproj         ./Events.Data/
COPY Events.Services/Events.Services.csproj ./Events.Services/

# Restore NuGet packages (separate layer - only re-runs when .csproj files change)
RUN dotnet restore ./Events.Crawler/Events.Crawler.csproj

# Copy full source
COPY . .

# Publish the crawler project
RUN dotnet publish ./Events.Crawler/Events.Crawler.csproj \
    -c Release \
    -o /app/publish

# Install Playwright system deps and Chromium in the SDK image.
# The SDK image (mcr.microsoft.com/dotnet/sdk:8.0) includes PowerShell (pwsh)
# so we can use playwright.ps1 directly - no extra apt sources needed.
ENV PLAYWRIGHT_BROWSERS_PATH=/app/playwright-browsers
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        libnss3 libnspr4 libdbus-1-3 libatk1.0-0 libatk-bridge2.0-0 \
        libcups2 libxcomposite1 libxdamage1 libxfixes3 libxrandr2 \
        libgbm1 libxkbcommon0 libpango-1.0-0 libcairo2 libasound2 \
        libexpat1 libglib2.0-0 wget ca-certificates && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

RUN pwsh /app/publish/playwright.ps1 install chromium

# =============================================================
# Stage 2: Runtime
# =============================================================
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0

# Install Chromium runtime shared libraries needed by the browser at runtime
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        libnss3 libnspr4 libdbus-1-3 libatk1.0-0 libatk-bridge2.0-0 \
        libcups2 libxcomposite1 libxdamage1 libxfixes3 libxrandr2 \
        libgbm1 libxkbcommon0 libpango-1.0-0 libcairo2 libasound2 \
        libexpat1 libglib2.0-0 ca-certificates && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

WORKDIR /home/site/wwwroot

# Copy published .NET output
COPY --from=build /app/publish .

# Copy pre-installed Playwright browsers from build stage
COPY --from=build /app/playwright-browsers /home/site/wwwroot/.playwright

# Tell Playwright where to find the browsers at runtime
ENV PLAYWRIGHT_BROWSERS_PATH=/home/site/wwwroot/.playwright

# Azure Functions host settings
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true \
    FUNCTIONS_WORKER_RUNTIME=dotnet-isolated

EXPOSE 80