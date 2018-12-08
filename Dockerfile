FROM microsoft/dotnet:2.1-aspnetcore-runtime
ENV ASPNETCORE_URLS http://0.0.0.0:5000
WORKDIR /app
COPY FunLand.Web/bin/Release/netcoreapp2.1/publish .
VOLUME wwwroot
EXPOSE 5000
ENTRYPOINT ["dotnet", "FunLand.Web.dll"]