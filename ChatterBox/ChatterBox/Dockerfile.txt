# Use the official .NET SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy the project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the remaining source code and build/publish the app in release mode
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image from the official ASP.NET Core image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose port 80 (or whichever port your app uses)
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "ChatterBox.dll"]
