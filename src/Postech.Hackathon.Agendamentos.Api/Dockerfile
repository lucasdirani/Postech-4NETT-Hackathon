FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5060

ENV ASPNETCORE_URLS=http://+:5060
ENV ASPNETCORE_HTTP_PORTS=5060

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/Postech.Hackathon.Agendamentos.Api/Postech.Hackathon.Agendamentos.Api.csproj", "src/Postech.Hackathon.Agendamentos.Api/"]
RUN dotnet restore "src/Postech.Hackathon.Agendamentos.Api/Postech.Hackathon.Agendamentos.Api.csproj"
COPY . .
WORKDIR "/src/src/Postech.Hackathon.Agendamentos.Api"
RUN dotnet build "Postech.Hackathon.Agendamentos.Api.csproj" -c "$configuration" -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Postech.Hackathon.Agendamentos.Api.csproj" \
    -c "$configuration" \
    -o /app/publish \
    /p:UseAppHost=false

FROM build AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Postech.Hackathon.Agendamentos.Api.dll"]