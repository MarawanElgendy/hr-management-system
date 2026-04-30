# Human Resource Management System (HRMS)

## Overview
This project is a comprehensive Human Resource Management System (HRMS) developed as part of a database and systems course.

The system is designed to centralize and automate core HR operations such as employee management, attendance tracking, payroll processing, leave management, and shift scheduling. It supports multiple roles including employees, managers, HR administrators, payroll officers, and system administrators.   

The goal of the system is to improve efficiency, consistency, and transparency across HR processes by integrating all employee-related data into a single platform.

---

## Features
The system supports a wide range of operations based on different user roles:

### Employee
- View and update personal profile  
- Submit leave requests and track status  
- View attendance and payroll history  
- Submit reimbursement requests  
- Manage skills and certifications  

### HR Admin
- Manage employee profiles and contracts  
- Approve/reject leave requests  
- Configure leave policies and entitlements  
- Assign missions and manage departments  
- Generate reports (e.g., diversity, employee statistics)  

### System Admin
- Add/update employee records  
- Assign roles and manage access  
- Configure shifts and system settings  
- View organizational hierarchy  

### Payroll Officer
- Generate payroll for employees  
- Manage allowances, deductions, and bonuses  
- Apply payroll policies  
- Generate payroll summaries and reports  

### Manager
- Approve team leave requests  
- Assign shifts to employees  
- View team attendance and statistics  
- Send notifications to team members  

These features are derived from detailed system requirements and user stories defined in the project specification.   

---

## Tech Stack
- Language: C#  
- Framework: ASP.NET Core MVC  
- Database: SQL (schema and stored procedures)  
- Frontend: Razor Views (HTML, CSS, JavaScript)  

---

## Project Structure
text ├── Controllers/     # Handles HTTP requests and routing ├── Models/          # Core entities and database mappings ├── Services/        # Business logic and application services ├── DTOs/            # Data transfer objects between layers ├── Database/        # SQL schema, data, and stored procedures ├── Exceptions/      # Custom exception handling ├── Views/           # UI (Razor pages) ├── ViewModels/      # Data prepared for UI rendering ├── wwwroot/         # Static assets (CSS, JS, images) 

---

## Key Highlights
- Layered architecture (Controllers → Services → Models)  
- Support for multiple user roles with different permissions  
- Use of stored procedures for database operations  
- Extensive custom exception handling  
- Covers a wide range of real-world HR operations  

---

## Notes
This project focuses on backend logic, database design, and system structure rather than advanced UI design. It was developed as part of an academic project to simulate a real-world enterprise HR system.