# Go Sofia Events Portal

A modern platform that aggregates and presents all events happening in Sofia, Bulgaria. 
The portal provides a convenient way for users to discover, filter, and share events based on their interests.

## Features
- **Comprehensive Event Aggregation:** Crawls and collects events from multiple web sources for Sofia.
- **Powerful Filtering & Search:** Filter by date, period, event type, and personal interests.
- **Calendar View:** Browse events by day, week, or custom period.
- **Personalization:** User profiles, favorite events, and personalized recommendations.
- **Social Sharing:** Share events easily on social networks.
- **Admin Panel:** Manage users, events, and categories with role-based access (admin, event manager, user).
- **AI-Powered Categorization:** Uses Claude Haiku 4.5 for automatic event categorization and tagging.

## Architecture
- **Layered Architecture:** Clear separation of concerns between Data, Services, and Web layers.
- **Area-based Organization:** Distinct areas for Admin and Identity features.
- **Design Patterns:**
  - **Strategy Pattern:** For selecting the appropriate crawler for each event source.
  - **Repository Pattern:** For data access abstraction and testability.
  - **Service Layer:** For business logic and orchestration.
- **Authentication & Roles:** Supports multiple roles (admin, event manager, user) with secure access control.

## Project Structure
```
Events/
├── Events.Crawler    # Azure Function App: Time-triggered event crawling, AI categorization, DB recording
├── Events.Web        # ASP.NET Core Web App: UI for searching, filtering, personalization, sharing
├── Events.Data       # Class Library: EF Core DbContext, migrations
├── Events.Models     # Class Library: Domain models (Event, Tag, Category, Enums)
├── Events.Services   # Class Library: Business logic, repository operations
```

## Processing Pipeline / Workflow
1. **CrawlerService** → Collects raw events from various web sources
2. **EventProcessingService** → Processes and saves events
3. **ClaudeProcessingService** → AI categorizes and tags events
4. **Database** → Stores processed events with enriched metadata

## Tech Stack
- **.NET 8** (C# 12)
- **ASP.NET Core Razor Pages & MVC Controllers** (Web UI & API)
- **Azure Functions** (Crawler)
- **Entity Framework Core** (Data access)
- **SQL Server** (Database)
- **Anthropic Claude Haiku 4.5** (AI event categorization/tagging)
- **Terraform** (Infrastructure as Code)
- **GitHub Actions** (CI/CD pipeline)
- **Azure App Services** (Cloud hosting)
- **Custom Domain:** [https://www.go-sofia.com](https://www.go-sofia.com)

## Development & Deployment
- **Source Control:** [GitHub](https://github.com/hrimar/Events)
- **CI/CD:** Automated with GitHub Actions
- **IaC:** Infrastructure managed with Terraform
- **Cloud Hosting:** Azure App Services

## Live Demo
Production Custom domains: https://www.go-sofia.com

## Contributing
Contributions are welcome! Please open an issue or submit a pull request.

## License
MIT License
