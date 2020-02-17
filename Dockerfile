FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["LineMessageApi/LineMessageApi.csproj", "LineMessageApi/"]
RUN dotnet restore "LineMessageApi/LineMessageApi.csproj"
COPY . .
WORKDIR "/src/LineMessageApi"
RUN dotnet build "LineMessageApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "LineMessageApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LineMessageApi.dll"]