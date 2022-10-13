# FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
# RUN mkdir ./app
# COPY PrjMealWebQuery/*.csproj /app/
# WORKDIR /app/
# RUN dotnet restore
# COPY PrjMealWebQuery/* ./
# RUN dotnet publish -o out

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS runtime
ENV ASPNETCORE_URLS http://+:80
EXPOSE 80
COPY out webapp/
WORKDIR webapp/
# COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "PrjMealWebQuery.dll"]