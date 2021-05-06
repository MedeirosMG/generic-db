#Dynamic Database

MyDB system provide a simple way to manage your data. Databases, tables and entities are easy to manage with this system.

## How To

- [Get Started](#get-started)
- [Instalation](#instalation)
- [Instalation Windows](#instalation-windows)
- [Instalation Linux/Mac](#instalation-linux-mac)

## Get Started 🚀

- Complete documentation on %your_folder%/Documentation.html

- All Projects need to be running to use MyDB

- To run tests, navigate into %your_folder%/myDB/API and run, CMD command: dotnet test /p:CollectCoverage=true

- [Swagger URL](https://localhost:5001/swagger/index.html)

- [Application URL](http://localhost:4200/)


## Instalation
### Instalation Windows
MyDB instructions build&run for windows:

> `Backend`
>
>1. Install [Net Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet/thankyou/sdk-3.1.408-windows-x64-installer)
>2. Trust on .net certs, CMD command:
>```bash
>dotnet dev-certs https –trust
>```
>3. Into %your_folder%/myDB/API run the project, CMD command:
>```bash
>dotnet run --project Presentation/MyDB.csproj
>```

> `FrontEnd`
>
>1. Install [NPM](https://nodejs.org/en/)
>2. Install angular CLI, CMD command:
>```bash
>npm install -g @angular/cli
>```
>3. Into %your_folder%/myDB/Front, install all dependencies, CMD command:
>```bash
>npm install
>```
>4. Into %your_folder%/myDB/Front, run the project, CMD command:
>```bash
>npm run start
>```

> `Redis`
>
> 1. Download [Redis Portable](https://github.com/microsoftarchive/redis/releases/download/win-3.0.504/Redisx64-3.0.504.zip)
> 2. Run file redis-server.exe from download (Other installations need to use default configs instance: localhost:6379)
> 3. Accept Adm privileges

### Instalation Linux Mac
MyDB instructions build&run for Linux and Mac:

> `Backend`
> 
> 1. Install [.Net Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet/3.1)
> 2. Trust on .net certs, CMD command:
> ```bash
> sudo dotnet dev-certs https –trust
> ```
> 3. Into %your_folder%/myDB/API run the project, CMD command:
> ```bash
> dotnet run --project Presentation/MyDB.csproj
> ```
> 
>
> Obs. .Net core download page specific how to install SDK, ex:
> ```bash
> mkdir -p $HOME/dotnet && tar zxf dotnet-sdk-3.1.408-linux-x64.tar.gz -C $HOME/dotnet
> export DOTNET_ROOT=$HOME/dotnet
> export PATH=$PATH:$HOME/dotnet
> ```

> ``FrontEnd``
> 
> 1. Install [NPM](https://www.npmjs.com/get-npm)
> 2. Install angular CLI, CMD command:
> ```bash
> sudo npm install -g @angular/cli
> ```
> 3. Into %your_folder%/myDB/Front, install all dependencies, CMD command:
> ```bash
> sudo npm install
> ```
> 4. Into %your_folder%/myDB/Front, run the project, CMD command:
> ```bash
> npm run start
>````

> `Redis`
> 
> 1. Download Redis, command: (Other installations need to use default configs instance: localhost:6379)
> ```bash
> wget http://download.redis.io/redis-stable.tar.gz
> ```
> 2. Extract File, command:
> ```bash
> tar xvzf redis-stable.tar.gz
> ```
> 3. Into %your_folder%/redis-stable make Redis project, command:
> ```bash
> Make
> ```
> 4. Into %your_folder%/redis-stable, run redis server, command:
> ```bash
> ./src/redis-server
> ```