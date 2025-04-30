# ChatterBox

**Beyond the Feed: Connect, Share, Engage.**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET Version](https://img.shields.io/badge/.NET-9.0-blueviolet.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
ChatterBox is a modern, full-featured social networking web application built with the power and flexibility of ASP.NET Core MVC. It provides a robust platform for users to create profiles, share their moments, interact with content, and connect privately through real-time messaging.

> **Mission:** To facilitate seamless online social interaction and content sharing within a structured, engaging, and secure environment.

---

## ✨ Visual Peek

Here's a glimpse of the ChatterBox's interface:

![Screenshot 2025-04-06 145100](https://github.com/user-attachments/assets/c0b9646c-3119-4b40-8113-5e2ec9203744)

![1744146056093](https://github.com/user-attachments/assets/9f4888a5-26d4-4189-adc1-9d2a6ebb0977)

![1744146068349](https://github.com/user-attachments/assets/6d2780d2-759b-4668-abec-1670b10288bd)

![1744146207201](https://github.com/user-attachments/assets/c3861a83-16c9-4a92-95a5-412f52f19ef5)

![1744146383264](https://github.com/user-attachments/assets/8859f9ff-ed30-4e2f-8210-ce4f498aea93)

![1744146297865](https://github.com/user-attachments/assets/8d5d21df-ec68-45bf-8e7c-4072736dede5)

![1744146425611](https://github.com/user-attachments/assets/6eaaf704-bcff-44f1-bb91-817fa9984258)

---

## Core Features

ChatterBox isn't just another feed. It's a dynamic space built with these key functionalities:

**Social & Connection:**

* **Robust User Authentication:** Secure registration, login, email confirmation, and password recovery powered by ASP.NET Core Identity.
* **Dynamic User Profiles:** Showcase user posts, follower/following counts, and personal information.
* **Following System:** Curate your feed by following and unfollowing other users.
* **User Search:** Easily find and connect with other members of the community.

**Content & Sharing:**

* **Post Creation:** Share text updates and media content with your followers.
* **Personalized Feed:** View a tailored feed displaying posts from users you follow.
* **Ephemeral Stories:** Share temporary (24-hour) photo or video stories.
* **Hashtag Discovery:** Automatic hashtag detection in posts, linking related content together.

**Interaction & Engagement:**

* **Likes & Comments:** Engage with posts through likes and comments.
* **Real-time Direct Messaging:** Engage in private, one-on-one conversations using SignalR.
* **Notifications:** Stay updated with real-time alerts for likes, comments, follows, and new messages.
* **Favorites:** Bookmark posts to easily find them later.

**Moderation & Management:**

* **Content Reporting:** Flag inappropriate posts or users for review.
* **Account Management:** Users can manage their profile details, change passwords, and request data deletion.

---

## Technology Stack

Built with a modern, scalable, and maintainable tech stack:

* **Backend Framework:** ASP.NET Core 9.0 MVC (C#)
* **Data Access:** Entity Framework Core 9.0
* **Authentication:** ASP.NET Core Identity
* **Database:** Microsoft SQL Server (Easily configurable for other EF Core providers)
* **Frontend:** Razor Views, Tailwind CSS, JavaScript (jQuery, UIkit)
* **Real-time Communication:** ASP.NET Core SignalR (for Chat & Notifications)
* **Background Tasks:** .NET BackgroundService (e.g., for Story cleanup)
* **Utilities:** Email Sending Service, Hashtag Parsing Logic

---

## 🚀 Getting Started

Follow these steps to get ChatterBox running locally:

**1. Prerequisites:**

* [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
* [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express, Developer, or other editions) or another configured EF Core compatible database.

**2. Clone the Repository:**

```bash
# Replace YOUR_USERNAME/ChatterBox.git with your actual repository URL
git clone [https://github.com/YOUR_USERNAME/ChatterBox.git](https://github.com/YOUR_USERNAME/ChatterBox.git)
cd ChatterBox
3. Configure Application Secrets:

Database Connection: It's highly recommended to use User Secrets for sensitive data like connection strings.

Bash

dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\mssqllocaldb;Database=ChatterBoxDb_Dev;Trusted_Connection=True;MultipleActiveResultSets=true"
(Adjust the connection string for your specific SQL Server instance if needed)

Email Settings (Optional): If you plan to test email features (confirmation, password reset), configure your email provider settings via User Secrets:

Bash

dotnet user-secrets set "EmailSettings:SmtpServer" "smtp.example.com"
dotnet user-secrets set "EmailSettings:Port" "587"
dotnet user-secrets set "EmailSettings:SenderName" "ChatterBox Notifications"
dotnet user-secrets set "EmailSettings:SenderEmail" "noreply@example.com"
dotnet user-secrets set "EmailSettings:Username" "your_smtp_username"
dotnet user-secrets set "EmailSettings:Password" "your_smtp_password"
(Alternatively, configure these in appsettings.Development.json for local development only, but avoid committing credentials).

4. Restore Dependencies:

Bash

dotnet restore
5. Apply Database Migrations:

This command creates the database (if it doesn't exist) and applies the schema based on the EF Core migrations.

Bash

dotnet ef database update
6. Run the Application:

Bash

dotnet run
The application will typically be available at https://localhost:xxxx and http://localhost:yyyy. Check the console output for the exact URLs.

7. Explore! Open your browser and navigate to the provided URL. Register a new account and start exploring ChatterBox.

Contributing
Contributions are welcome! If you'd like to help improve ChatterBox, please feel free to:

Fork the repository.
Create a new branch for your feature or bug fix (git checkout -b feature/your-feature-name).
Commit your changes (git commit -am 'Add some feature').
Push to the branch (git push origin feature/your-feature-name).
Open a Pull Request.
Please check for any existing CONTRIBUTING.md guidelines (or create one!) and make sure your code follows the project's style. Report bugs or suggest features using the GitHub Issues tab.

License
This project is licensed under the MIT License.

A copy of the license is available in the LICENSE file (you should create this file and include the MIT license text).
