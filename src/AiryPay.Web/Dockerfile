FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
EXPOSE 80

COPY ["src/AiryPay.Web/AiryPay.Web.csproj", "src/AiryPay.Web/"]
COPY ["src/AiryPay.Application/AiryPay.Application.csproj", "src/AiryPay.Application/"]
COPY ["src/AiryPay.Domain/AiryPay.Domain.csproj", "src/AiryPay.Domain/"]
COPY ["src/AiryPay.Infrastructure/AiryPay.Infrastructure.csproj", "src/AiryPay.Infrastructure/"]
COPY ["src/AiryPay.Shared/AiryPay.Shared.csproj", "src/AiryPay.Shared/"]
RUN dotnet restore "src/AiryPay.Web/AiryPay.Web.csproj"
COPY . .
WORKDIR "/src/src/AiryPay.Web"
RUN dotnet build "AiryPay.Web.csproj" -c Release -o /app/build
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
ENTRYPOINT ["dotnet", "AiryPay.Web.dll"]