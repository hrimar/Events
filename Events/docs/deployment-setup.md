# GitHub Secrets Setup Instructions

## Required Secrets in GitHub Repository Settings > Secrets and variables > Actions:

### 1. AZURE_CREDENTIALS
Service Principal JSON for Azure authentication:
```json
{
  "clientId": "your-service-principal-client-id",
  "clientSecret": "your-service-principal-secret", 
  "subscriptionId": "your-subscription-id",
  "tenantId": "your-tenant-id"
}
```

### 2. AZURE_SQL_CONNECTION_STRING
Connection string for database migrations (with admin rights):
```
Server=tcp:gosofia-prod-sql.database.windows.net,1433;Initial Catalog=gosofia-prod-db;User ID=your-admin-user;Password=your-admin-password;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

## Azure Service Principal Creation:
```bash
az ad sp create-for-rbac --name "github-actions-sp" --role contributor --scopes /subscriptions/{subscription-id}/resourceGroups/{resource-group} --sdk-auth
```

## Database Admin User:
- Create separate SQL admin user with DDL rights for migrations
- Different from the Managed Identity used by the app
- Use this connection string only in the pipeline

## Security Note:
The app's Managed Identity should only have db_datareader + db_datawriter rights
The pipeline uses a separate admin connection for schema changes