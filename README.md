# ScoreRank Project

ScoreRank is a .NET 8 project for managing customer scores and leaderboard queries.

## API Endpoints

### Updates the score for a customer.

```
POST /api/customer/{customerId}/score/{score}
```

### Gets customers by their rank range.

```
GET /api/leaderboard?start=1&end=10
```

### Gets a customer and their neighboring customers by rank.

```
GET /api/leaderboard/{customerId}?high=1&low=2
```

### Quick start

```powershell
cd WebApi
dotnet run
```

The API will start at `https://localhost:5235`

### Access Swagger documentation

After starting the API, visit:
```
http://localhost:5235/swagger/index.html
```
