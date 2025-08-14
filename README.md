🔷 ShoesShop – E-commerce Website for Sneakers (ASP.NET Core + Gemini AI)
ShoesShop is a modern and responsive e-commerce platform tailored for sneaker shopping, built with ASP.NET Core (C#) and HTML/CSS/JavaScript.
The project adopts a Domain-Driven Design (DDD) architecture to ensure modularity and maintainability.
It also features an intelligent AI-powered chatbox using the Gemini API (Google Generative AI) to assist users with personalized product recommendations through natural language conversations.

🧩 Key Highlights

  🛒 Product browsing, advanced filtering, cart & wishlist support
  
  🔐 Secure user authentication (login, registration, password recovery)
  
  💳 Simplified checkout process and order tracking
  
  🧠 AI chatbot integrated with Gemini for smart product consultation
  
  ⚙️ Admin dashboard for full control over products and orders
  
  🧱 Built with clean, scalable Domain-Driven Design (DDD) architecture

Designed to improve the online sneaker shopping experience with intelligent, user-friendly interaction.

🚀 Features

👤 User Capabilities

  Browse and filter sneaker products by size, brand, price, gender, etc.
  
  Add products to cart and wishlist
  
  User account management (register, login, forgot password)
  
  Chat with Gemini-powered AI assistant for tailored recommendations

🛠️ Admin Capabilities

  Manage products (add, edit, delete), brands, and categories
  
  View and manage customer orders and user information
  
  Track inventory and monitor order statuses
  
  Maintain full control through an intuitive admin dashboard

🤖 Gemini AI Chat Assistant
  The system includes a conversational AI chatbot integrated via the Gemini API, enabling:
  
  Understanding customer preferences via natural language input
  
  Searching the product catalog and providing personalized suggestions
  
  Enhancing user engagement and increasing conversion rates

🧱 Architecture – Domain-Driven Design (DDD)
  ShoesShop is structured using a clean, layered DDD architecture, with a clear separation of concerns between the domain logic, infrastructure, and presentation layers.
  
  ShoesShop/
  
  ├── CrossCutting/        # Shared logic (validation, exceptions, base classes, etc.)
  
  ├── Domain/              # Core business logic: Entities, Value Objects, Interfaces
  
  ├── Domain.Services/     # Domain-level business rules involving multiple Entities/Aggregates
  
  ├── Infrastructure/      # Technical implementations: EF Core, external APIs, Repositories
  
  ├── Web/                 # ASP.NET Core API layer: controllers, routing, dependency injection
  
  Each layer has a well-defined responsibility, enabling better testability, scalability, and long-term maintainability.
