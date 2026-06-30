#build stage - contains the dotnet SDK and tools to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["E-Commerce.csproj", "./"]
#next line downloads packages for the project, runs after copying the .csproj as docker can cache 
#this step when source code changes but dependecies don't .
RUN dotnet restore "E-Commerce.csproj"

COPY . .
#this RUN compiles the app in release mode, and outputs the ready-to-run app in the /app/publish folder
# -C Release - optimized build,, -o /app/publish - output folder inside the build stage
# /p:UseAppHost=false - don't create a native executable; use dotnet E-commerce.dll instead of the host
RUN dotnet publish "E-Commerce.csproj" -c Release -o /app/publish /p:UseAppHost=false

#runtime stage - contains the runtime environment for the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

#this sets an env var inside the container, that ASP.NET Core reads at startup to determine the URL to listen on.
# http://+:8080 means Kestrel listens on port 8080 on all network interfaces inside the container
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "E-Commerce.dll"]
