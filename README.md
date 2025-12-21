# Medical System – Accessing Data from Program Code

## Opis projekta
Web aplikacija za upravljanje pacijentima, liječnicima, bolestima, lijekovima i pregledima.
Projekt demonstrira rad s relacijskom bazom podataka u oblaku koristeći Entity Framework Core
i vlastitu implementaciju jednostavnog ORM alata.

## Tehnologije
- C# (.NET 8)
- ASP.NET Core MVC
- PostgreSQL (Docker / Supabase)
- Entity Framework Core
- Npgsql

## Funkcionalnosti
- CRUD operacije nad pacijentima, bolestima, lijekovima i pregledima
- Eager i Lazy loading povezanih podataka
- Code First pristup i migracije
- Vlastita ORM biblioteka:
  - Refleksija
  - Atributi za mapiranje
  - Automatsko generiranje SQL-a
  - Change tracking

## Arhitektura
- Med.Api
- MedCore
- MedORM

## Pokretanje
1. Pokrenuti PostgreSQL (Docker ili Supabase)
2. Izvršiti `dotnet ef database update`
3. Pokrenuti aplikaciju
