# Todo Manager API

The **Todo Manager API** is a robust and feature-rich project designed to help developers manage tasks efficiently while showcasing modern development practices. This project implements several valuable features and patterns that can serve as a foundation or reference for your own applications.

---

## Features

1. **Authentication Service using JWT Tokens**  
   - Secure and widely used approach for authentication.  
   - Built using the [OpenIddict](https://documentation.openiddict.com/) implementation, which provides predefined entities and relationships.  

2. **Clean Architecture**  
   - Ensures a clear and modular structure.  
   - Enhances maintainability, scalability, and testability of the codebase.  

3. **Either Pattern**  
   - A functional approach to handle validation and error responses.  
   - Returns either a successful result or an error message, avoiding exceptions and null values.  

4. **Logging Middleware**  
   - Captures and logs each API endpoint call for better observability and debugging.

5. **Database Connection**  
   - Configured with **SQL Server** in the `program.cs` file.  
   - The `appsettings.json` file allows you to modify the connection string and use a different database if needed.  

---

## Project Highlights

### Authentication with JWT Tokens  
This project uses **JWT (JSON Web Token)** for secure and efficient authentication. The implementation leverages the OpenIddict library, which provides built-in entities and relationships, simplifying the authentication setup and ensuring compliance with best practices.

### Clean Architecture  
The project is structured using **Clean Architecture** principles, making it:  
- **Neat**: Logical separation of concerns.  
- **Maintainable**: Easy to update and extend.  
- **Scalable**: Handles increasing complexity gracefully.  

### Either Pattern in C#  
The **Either Pattern** is employed to elegantly manage validation and error handling. This functional approach:  
- Allows returning success or error results without throwing exceptions.  
- Ensures code readability and reduces error-prone null values.

### Logging Middleware  
A custom **logging middleware** is implemented to log details of all incoming requests and responses, providing better traceability and debugging capabilities.

---

## Getting Started

### Prerequisites  
- **.NET 8.0+**  
- **SQL Server** (default, or modify for other databases)  

### Configuration  
1. Update the `appsettings.json` file to configure your database connection.  
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "YourDatabaseConnectionString"
     }
   }

### Usage
```bash
dotnet run
```

Do not forget to star the project if you liked it :)
Happy Coding!
