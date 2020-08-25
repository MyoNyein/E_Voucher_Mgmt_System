#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-nanoserver-1903 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1903 AS build
WORKDIR /src
COPY ["E_Store_API/E_Store_API.csproj", "E_Store_API/"]
COPY ["E_Voucher.Repositories/E_Voucher.Repositories.csproj", "E_Voucher.Repositories/"]
COPY ["E_Voucher.Entities/E_Voucher.Entities.csproj", "E_Voucher.Entities/"]
RUN dotnet restore "E_Store_API/E_Store_API.csproj"
COPY . .
WORKDIR "/src/E_Store_API"
RUN dotnet build "E_Store_API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "E_Store_API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "E_Store_API.dll"]