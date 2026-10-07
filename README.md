# ECommercePlatform

![GitHub repo size](https://img.shields.io/github/repo-size/vasudusa/ECommercePlatform)
![GitHub last commit](https://img.shields.io/github/last-commit/vasudusa/ECommercePlatform)
![GitHub top language](https://img.shields.io/github/languages/top/vasudusa/ECommercePlatform)
![Static Badge](https://img.shields.io/badge/Stack-.NET%209%20%2F%20C%23%20%2F%20ASP.NET%20Core-blueviolet)

A complete .NET e-commerce API for product catalog, cart-related order handling, dashboard metrics, and deployment-ready storefront operations.

## Overview
This repository now contains a working backend foundation for a storefront, with in-memory product data, product management endpoints, order creation, and summary metrics.

## Tech Stack
- .NET 9
- C# / ASP.NET Core
- OpenAPI support
- In-memory data layer for local prototyping

## Features
- Product listing and detail retrieval
- Product creation endpoint
- Order creation with stock validation
- Health and dashboard summaries
- CORS-enabled API for frontend integration

## Run locally
```bash
cd D:\ECommercePlatform
dotnet restore
dotnet run
```

Then open:
- http://localhost:5000/health
- http://localhost:5000/api/products
- http://localhost:5000/api/dashboard

## Example API calls
```bash
curl http://localhost:5000/api/products
curl -X POST http://localhost:5000/api/products -H "Content-Type: application/json" -d '{"name":"Travel Lamp","category":"Home","price":49.99,"stock":10,"description":"Portable lamp for desk use."}'
```

## Project status
- Functional product catalog API
- Order lifecycle foundation in place
- Dashboard summary ready for frontend integration
- Ready for next stage: database persistence, authentication, and checkout flow
