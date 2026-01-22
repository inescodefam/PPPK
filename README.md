# Medical System – Accessing Data from Program Code

## Project Description  
An application for managing patients, doctors, diseases, medications, and medical examinations.  
The project demonstrates working with a relational cloud database using Entity Framework Core and a custom implementation of a simple ORM tool.

## Technologies  
- C# (.NET 8)  
- ASP.NET Core MVC  
- PostgreSQL (Docker / Supabase)  
- Entity Framework Core  
- Npgsql  

## Features  
- CRUD operations for patients, diseases, medications, and examinations  
- Eager loading of related data  
- Code First approach and migrations  
- Custom ORM library:  
  - Reflection  
  - Mapping attributes  
  - Automatic SQL generation  
  - Change tracking  

## Architecture  
- Med.Api  
- MedCore  
- MedORM  
