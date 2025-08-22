FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Users.Api.csproj
RUN dotnet publish Users.Api.csproj -c Release -o /out


FROM base AS final
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Users.Api.dll"]