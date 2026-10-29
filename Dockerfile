FROM mcr.microsoft.com/dotnet/aspnet:11.0 AS base
WORKDIR /src
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
WORKDIR /src
COPY ["Goto/", "Goto/"]
COPY ["DigitalLinkToolkit/", "DigitalLinkToolkit/"]
COPY ["Directory.Packages.props", "."]
RUN dotnet restore "Goto/Goto.csproj"

WORKDIR "/src/Goto"
RUN dotnet build "Goto.csproj" -c Release -o /src/build

FROM build AS publish
RUN dotnet publish "Goto.csproj" -c Release -o /src/publish

FROM base AS final
WORKDIR /src
COPY --from=publish /src/publish .
ENTRYPOINT ["dotnet", "Goto.dll"]
