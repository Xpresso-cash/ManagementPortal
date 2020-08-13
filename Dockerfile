FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /xpresso
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["OfferManagement/OfferManagement.csproj", "OfferManagement/"]
COPY . .
RUN dotnet restore "OfferManagement/OfferManagement.csproj"

RUN dotnet build -c Release -o /app
RUN cp OfferManagement/OfferManagement.pfx /app/

FROM build AS publish
RUN dotnet publish "OfferManagement/OfferManagement.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ADD newrelic-netcore.tar.gz /
ARG NEWRELIC_LICENSE
ARG PROJECT
ENV NEW_RELIC_LICENSE=$NEWRELIC_LICENSE
ENV NEW_RELIC_APP_NAME=$PROJECT
RUN sed -i "s/REPLACE_WITH_LICENSE_KEY/$NEWRELIC_LICENSE/g" /newrelic-netcore20-agent/newrelic.config
RUN sed -i "s/My Application/$PROJECT/g"  /newrelic-netcore20-agent/newrelic.config
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/newrelic-netcore20-agent \
CORECLR_PROFILER_PATH=//newrelic-netcore20-agent/libNewRelicProfiler.so
ENTRYPOINT ["dotnet", "OfferManagement.dll"]