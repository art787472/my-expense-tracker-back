# 建立階段：有 SDK，可以編譯和還原套件
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 複製 csproj 並還原（這樣快取比較有效）
COPY ["記帳程式後端/記帳程式後端.csproj", "記帳程式後端/"]
COPY ["記帳程式後端.Contract/記帳程式後端.Contract.csproj", "記帳程式後端.Contract/"]
COPY ["記帳程式後端.Test/記帳程式後端.Test.csproj", "記帳程式後端.Test/"]

RUN dotnet restore "記帳程式後端/記帳程式後端.csproj"

# 複製剩下的程式碼並編譯
COPY . .
WORKDIR "/src/記帳程式後端"
RUN dotnet build "記帳程式後端.csproj" -c Release -o /app/build

# 發布
FROM build AS publish
RUN dotnet publish "記帳程式後端.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 執行階段：只需要 runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "記帳程式後端.dll"]

