#!/bin/bash
# Production User Creation Script
# Run this ONCE after initial deployment

echo "Creating production admin user..."

# You'll need to set these environment variables or replace with actual values
ADMIN_EMAIL="${ADMIN_EMAIL:-admin@yourdomain.com}"
ADMIN_PASSWORD="${ADMIN_PASSWORD:-YourSecurePassword123!}"

# Connect to your Azure SQL Database and run:
# You can use Azure CLI or SQL Management Studio

echo "Manual steps required:"
echo "1. Connect to your Azure SQL Database"
echo "2. Use the admin panel to create your first admin user"
echo "3. Or use the ASP.NET Identity UI to register and manually promote to Administrator role"
echo ""
echo "Recommended approach:"
echo "1. Deploy the application"
echo "2. Register your admin account through /Identity/Account/Register"
echo "3. Connect to database and run:"
echo "   UPDATE AspNetUserRoles SET RoleId = (SELECT Id FROM AspNetRoles WHERE Name = 'Administrator') WHERE UserId = (SELECT Id FROM AspNetUsers WHERE Email = '$ADMIN_EMAIL')"
echo "4. Insert the role assignment if it doesn't exist:"
echo "   INSERT INTO AspNetUserRoles (UserId, RoleId) SELECT u.Id, r.Id FROM AspNetUsers u CROSS JOIN AspNetRoles r WHERE u.Email = '$ADMIN_EMAIL' AND r.Name = 'Administrator'"