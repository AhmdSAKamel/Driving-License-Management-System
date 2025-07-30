# Driving License Management System - WinForms & SQL Server

A full-featured C# Windows Forms application designed to manage driving license operations such as applications, testing, issuance, and user accounts. 
It uses SQL Server as the backend.


## 🚀 Features

- **People**
  - Add new people with personal details and an image.
  - Capture image using the local camera or upload from disk.
  - Images saved in `DVLD_Images` folder with unique GUID names.
  
- **Users**
  - Persons can become system users.
  - Secure password storage using hash encryption.

- **Driver Management**
  - A person can become a driver by owning at least one license.
  - Driver details linked to their person record.
   
- **Application Handling**
  - Supports 7 application types:
    1. New Local License  
    2. Renew License  
    3. Replacement for Damaged License  
    4. Replacement for Lost License  
    5. Release Detained License  
    6. New International License  
    7. Retake Failed Test  

- **Test Management**
  - 3 mandatory tests in order: Vision ➝ Written ➝ Practical.
  - Retakes allowed upon failure.

- **License Classes & Validation**
  - 7 driving license classes:
    - Class 1: Small Motorcycle
    - Class 2: Heavy Motorcycle
    - Class 3: Ordinary Driving
    - Class 4: Commercial
    - Class 5: Agricultural
    - Class 6: Small & Medium Bus
    - Class 7: Truck & Heavy Vehicle
  - Age requirements:
    - Class 1 & 3: Min age 18
    - Other classes: Min age 21
  - Validations ensure conditions are met before applying.

- **Detained Licenses**
  - Manage detained licenses, pay fines, and release them.

---

## 🧠 Technologies Used

- **C# - Windows Forms**
- **.NET Framework**
- **SQL Server (2022)**
- **ADO.NET**
 
Programming Techniques 
- **Delegates & User Controls** — to enhance code reusability and communication between forms.
- **App Configuration Editor** — for editing database connection strings via UI.

---

## 🏁 Getting Started

### 1. Clone the Repository: [https://github.com/AhmdSAKamel/DrivingLicense-Management-System.git](https://github.com/AhmdSAKamel/DrivingLicenseManagementSystem.git)

2. Restore the Database
The project includes a backup file of the SQL Server database: DVLDDB.bak

Open SQL Server Management Studio.

Restore the .bak file into a new database named DVLDDB. (Or any)

Make sure you're using SQL Server 2022 or compatible to avoid compatibility issues.

3. Configure the Connection String
The application will prompt you to enter database connection settings on the first launch (And if the connection string is not configured yet).

You'll be asked to enter:

Server: localhost (or your SQL Server instance)
Database: DVLDDB
Username: me (For ex)
Password: 123456789 (or your SQL login)

The connection will be saved to the App.config file for future use.

4. Login with Demo Credentials
A demo user is available to log in:    Username: user1      Password: 1234
