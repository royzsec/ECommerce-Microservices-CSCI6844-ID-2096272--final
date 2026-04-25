# E-Commerce Microservices System

## Overview

A full-stack e-commerce platform built with microservices architecture using ASP.NET Core, Blazor WebAssembly, Docker, and RabbitMQ. The system demonstrates database-per-service, API Gateway pattern, asynchronous messaging, and full CRUD operations.


## Services

| Service | Port | Database | CRUD Operations |
|---------|------|----------|-----------------|
| Blazor Frontend | 5006 | - | UI for all operations |
| API Gateway | 5005 | - | Routes all requests |
| Product Service | 5001 | products.db | Create, Read, Update, Delete |
| Customer Service | 5002 | customers.db | Create, Read, Update, Delete |
| Order Service | 5003 | orders.db | Create, Read, Cancel, Delete |
| Payment Service | 5004 | payments.db | Create, Read, Process, Delete |
| RabbitMQ | 5672/15672 | - | Message broker |

## Features

- Full CRUD operations for Products, Customers, Orders, Payments
- API Gateway as single entry point (frontend never calls services directly)
- Asynchronous stock updates via RabbitMQ when orders are created/cancelled
- Database-per-service architecture with SQLite
- Aggregated endpoint combining Order + Customer + Product + Payment
- Docker containerization with docker-compose
- Beautiful Blazor WebAssembly UI with Bootstrap

## Prerequisites

- Docker Desktop installed
- .NET 8 SDK (optional, for local development)

## Run the Project

```bash
git clone https://github.com/royzsec/ECommerce-Microservices-CSCI6844-ID-2096272--final.git
cd ECommerce-Microservices-CSCI6844-ID-2096272--final
docker-compose up --build
