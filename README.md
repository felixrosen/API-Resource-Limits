# API - Resource Limits with Orleans

## 🎯 Project Goals  
This project demonstrates how to use Microsoft Orleans as a distributed resource limiter for parallel API requests. Each client is limited in the number of concurrent requests they can make to specific resources, using Orleans grains for scalable, per-client enforcement.

## 📌 Features
- **Orleans-based resource limiting**: Uses Orleans grains to track and limit concurrent requests per client and resource.
- **Per-client granularity**: Each client (identified by `X-Client-Id` header) has independent limits.
- **Sample endpoint**: `/api/open-meteo` demonstrates the pattern with a simulated long-running request.

## ❔ How It Works
- Each API request includes an `X-Client-Id` header.
- The API calls an Orleans grain (`ClientIdResourceLimitGrain`) for the client, requesting a slot for the resource.
- If the client has not exceeded the limit (default: 4 concurrent requests), the request proceeds; otherwise, it is rejected.
- When the request completes, the slot is returned to the grain.

## ⚙️ Setup & Run
**Restore dependencies**:
   ```sh
   dotnet restore
   ```
**Build the project**:
   ```sh
   dotnet build
   ```
**Run the API**:
   ```sh
   dotnet run --project src/APIResourceLimits.API/APIResourceLimits.API.csproj
   ```
**Swagger UI**: Visit `https://localhost:5001` (or the port shown in the console) for interactive API docs.

## ⚙️ Example Usage
Request weather data (replace `<client-id>` as needed):

```sh
curl.exe -X GET "https://localhost:7222/api/open-meteo?longitude=18.162020473746654&latitude=59.29811528726271" -H "X-Client-Id: 123"
```

If the client exceeds the parallel request limit, the API returns a 429 response.

## ✏️ Customization
- **Change resource limits**: Edit `_maxResourceLimit` in `ClientIdResourceLimitGrain.cs`.
- **Add endpoints**: Implement `IEndpoint` and register new routes as shown in `OpenMeteoEndpoint.cs`.

## License
MIT or your preferred license.
