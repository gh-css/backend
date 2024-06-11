# Backend for GHCSS <img height="30px" src="https://github.com/gh-css/backend/assets/46683337/4fde7337-d002-4140-9183-17f33e023b56" />

## Devlopment
**Requirements**
- .NET 8.X
- Docker & Docker Compose

### 1. Installing Dependencies
Naviagate to the cloned repository and run the following command to install the project dependencies:
```bash
dotnet restore
```

### 2. Enviorment Variables
Copy the `.env.example` to `.env` and enter the credentials.

### 3. Postgres
If you don't want to run postgres locally you can also use the following:
```bash
docker-compose --env-file .env run -d --service-ports postgres
```
(This will use the enviorment variables directly from `.env`)

### 4. Run the Backend
To run the backend you can either run the defined script in the `launchSettings.json` via the following command:
```bash
dotnet run --launch-profile "http"
```
Or just run the launch profile in your preferred IDE

### Running Database Migrations
If you changed or added any database relevant files like `Data/GitHubUser.cs` you can create a new migration by running:
```bash
dotnet ef migrations add ExampleMigrationName
```
Then you just re-run the dotnet application and it will automatically update the database.
