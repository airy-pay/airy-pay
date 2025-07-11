FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
EXPOSE 80

COPY ["src/AiryPayNew.Discord/AiryPayNew.Discord.csproj", "src/AiryPayNew.Discord/"]
COPY ["src/AiryPayNew.Application/AiryPayNew.Application.csproj", "src/AiryPayNew.Application/"]
COPY ["src/AiryPayNew.Domain/AiryPayNew.Domain.csproj", "src/AiryPayNew.Domain/"]
COPY ["src/AiryPayNew.Infrastructure/AiryPayNew.Infrastructure.csproj", "src/AiryPayNew.Infrastructure/"]
COPY ["src/AiryPayNew.Shared/AiryPayNew.Shared.csproj", "src/AiryPayNew.Shared/"]
RUN dotnet restore "src/AiryPayNew.Discord/AiryPayNew.Discord.csproj"
COPY . .
WORKDIR "/src/src/AiryPayNew.Discord"
RUN dotnet build "AiryPayNew.Discord.csproj" -c Release -o /app/build
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/build .
RUN apt-get update \
    && apt-get install -y wget apt-transport-https \
    && wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y dotnet-sdk-8.0 \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
ENTRYPOINT ["dotnet", "AiryPayNew.Discord.dll"]