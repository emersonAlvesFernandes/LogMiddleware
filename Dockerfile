FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 63423
EXPOSE 44397

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["CustomMiddleware/CustomMiddleware.csproj", "CustomMiddleware/"]
RUN dotnet restore "CustomMiddleware/CustomMiddleware.csproj"
COPY . .
WORKDIR "/src/CustomMiddleware"
RUN dotnet build "CustomMiddleware.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CustomMiddleware.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CustomMiddleware.dll"]