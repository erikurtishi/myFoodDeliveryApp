Food Delivery Web Application in .NET 8.0.0

*Overview

The Food Delivery Web Application is a platform built using .NET 8.0.0, enabling customers to browse restaurants, order food online, and track deliveries in real time. The application includes role-based access for customers, restaurant owners, and administrators.

*Progress


Once the Web Application is started you should Login using these credentials:

 Username: admin@admin.com 

 Password: Admin@123

Then you can create a Restaurant in the Admin Panel and Sign Out from the Admin Panel to navigate to the Restaurant you just created for further services!

If you want to create a Basic User you should use Register because the Register feature creates just Basic Users!


*Features

User Authentication & Authorization

Register, login, and manage user accounts

Role-based access (Customer, Restaurant Owner, Admin)

Restaurant Management

Add, update, and delete restaurant MenuItems

Manage restaurant menus and pricing also the Order for that Restaurant

Food Ordering System

Browse food categories and items

Add to Order and checkout

Payment Integration

Wallet balance management

Order Tracking

View real-time order status

Delivery person tracking

Admin Dashboard

Manage users and restaurants


*Technologies Used

Backend: .NET 8.0.0, ASP.NET Core, Entity Framework Core

Frontend: Razor Pages

Database: PostgreSQL

Authentication: Identity Framework


*Prerequisites

Ensure you have the following installed before running the application:

.NET 8.0.0 SDK

PostgreSQL

Visual Studio 2022+ or VS code or JetBrains Rider


*Installation

Clone the repository:

git clone https://github.com/erikurtishi/myFoodDeliveryApp.git
cd FoodDeliveryApp

Configure database:

Update appsettings.json with your database connection string.

Apply migrations and seed database:

dotnet ef database update

Run the application:

dotnet run

Access the application:
Open http://localhost:5000 in your browser or the default port that your IDE is redirecting.

*Information: The database is seeded after you start the application, the only important thing is to update the appsettings.json file and update the database!


*Contributing

Fork the repository.

Create a new feature branch: git checkout -b feature-name

Commit changes: git commit -m 'Add new feature'

Push to the branch: git push origin feature-name

Submit a pull request.


*Contact

For any inquiries, please contact erionerion64@gmail.com, ubejdqazimi3@gmail.com, menansali@gmail.com.


<!-- Security scan triggered at 2025-09-02 05:07:14 -->

<!-- Security scan triggered at 2025-09-09 05:43:54 -->

<!-- Security scan triggered at 2025-09-28 15:53:23 -->