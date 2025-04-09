# Use the official .NET 9.0 SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy the project file and restore dependencies
# Assumes your project file is ChatterBox.csproj, adjust if needed
COPY ChatterBox.csproj ./
RUN dotnet restore ChatterBox.csproj

# Copy the remaining source code and build/publish the app in release mode
COPY . ./
# Adjust the project path if your .csproj file is not in the root
RUN dotnet publish ChatterBox.csproj -c Release -o out

# Build runtime image from the official ASP.NET Core 9.0 image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .
# Expose port 80 (or whichever port your app uses internally)
# Render automatically handles mapping external port 80/443 to this internal port.
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "ChatterBox.dll"]
