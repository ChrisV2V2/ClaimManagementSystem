# Lecturer Hourly Claim System

## Table of Contents
- [Overview](#overview)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Installation](#installation)
  - [Prerequisites](#prerequisites)
  - [Steps to Install](#steps-to-install)
- [Usage](#usage)
  - [Lecturer Role](#lecturer-role)
  - [Admin Role](#admin-role)
- [Testing](#testing)
- [Admin Controls](#admin-controls)
- [Contribution](#contribution)
- [License](#license)

## Overview
The **Lecturer Hourly Claim System** is a web-based application designed for university lecturers to submit hourly claims for work completed. Administrators can approve or reject claims while providing relevant feedback. The system tracks all claims and calculates the total claim amount based on the lecturer's hourly rate.

## Features
- **Lecturer Claims Submission**: Lecturers can submit claims for work completed, specifying the number of hours worked.
- **Admin Approval/Reject**: Admins can approve or reject claims. If rejected, an admin comment is required.
- **Real-time Notifications**: Users receive notifications when their claims are approved or rejected.
- **Claim Status Tracking**: Lecturers can track the status of their submitted claims.
- **Validation**: Rejection requires a note to explain the reason.

## Technologies Used
- **ASP.NET Core MVC** (version 8.0)
- **C#**
- **Entity Framework Core** (for database operations)
- **Razor Views** (for front-end)
- **xUnit** (for unit testing)
- **JavaScript/jQuery** (for client-side validation)

## Installation

### Prerequisites
- Visual Studio 2022 or later
- .NET 8.0 SDK
- SQL Server or another database supported by Entity Framework Core

### Steps to Install
1. Clone the repository:
    ```bash
    git clone https://github.com/your-username/LecturerHourlyClaimSystem.git
    ```
2. Navigate to the project directory:
    ```bash
    cd LecturerHourlyClaimSystem
    ```
3. Restore the NuGet packages:
    ```bash
    dotnet restore
    ```
4. Update the database connection string in `appsettings.json`:
    ```json
    "ConnectionStrings": {
        "DefaultConnection": "Server=your-server;Database=LecturerClaimsDB;Trusted_Connection=True;"
    }
    ```
5. Apply migrations to set up the database:
    ```bash
    dotnet ef database update
    ```
6. Run the application:
    ```bash
    dotnet run
    ```

## Usage

### Lecturer Role
Lecturers can log in, submit claims with the number of hours worked and hourly rate, and then send the claim for approval.

### Admin Role
Admins can review the submitted claims, approve or reject them, and must provide a note if rejecting the claim.

### Testing
Unit tests are implemented using **xUnit**. To run the tests:
```bash
dotnet test
