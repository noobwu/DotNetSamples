# DotNetSamples

## 常用命令

### 在终端中，运行以下命令创建创建解决方案

dotnet new sln -n DotNetSamples

### 在终端中，运行以下命令创建库项目

dotnet new classlib -o StackExchange\StackExchange.Utils\StackExchange.Utils.Configuration

### 在终端中，运行以下命令，向解决方案添加库项目

dotnet sln add StackExchange\StackExchange.Utils\StackExchange.Utils.Configuration\StackExchange.Utils.Configuration.csproj

### 在终端中，运行以下命令创建库项目(StackExchange.Utils.Http)

dotnet new classlib -o StackExchange\StackExchange.Utils\StackExchange.Utils.Http

### 在终端中，运行以下命令，向解决方案添加库项目(StackExchange.Utils.Http)

dotnet sln add StackExchange\StackExchange.Utils\StackExchange.Utils.Http\StackExchange.Utils.Http.csproj


### 运行以下命令以生成解决方案，并验证项目是否正确编译

dotnet build

#### 在终端中，运行以下命令，移除解决方案的项目

dotnet sln remove StackExchange/StackExchange.Utils/StackExchange.Utils.csproj

#### 在终端中，运行以下命令，添加第三方包

dotnet add StackExchange/StackExchange.Utils/StackExchange.Utils.csproj package Newtonsoft.Json
