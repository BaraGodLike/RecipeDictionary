﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY bin/Release/net8.0/publish/ .

ENTRYPOINT ["sh", "-c", "dotnet RecipeDictionaryApi.dll --migrate"]