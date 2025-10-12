# 建立階段：有 SDK，可以編譯和還原套件
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
EXPOSE 8888

# 複製 csproj 並還原（這樣快取比較有效）
COPY ["ExpenseTracker/ExpenseTracker.csproj", "ExpenseTracker/"]
COPY ["ExpenseTracker.Contract/ExpenseTracker.Contract.csproj", "ExpenseTracker.Contract/"]
COPY ["ExpenseTracker.Test/ExpenseTracker.Test.csproj", "ExpenseTracker.Test/"]

RUN dotnet restore "ExpenseTracker/ExpenseTracker.csproj"

# 複製剩下的程式碼並編譯
COPY . .
WORKDIR "/src/ExpenseTracker"
RUN dotnet build "ExpenseTracker.csproj" -c Release -o /app/build

# 發布
FROM build AS publish
RUN dotnet publish "ExpenseTracker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 執行階段：只需要 runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExpenseTracker.dll"]

