# PizzaShop - Use Case and Class Diagram Overview

## Project Summary

PizzaShop is a restaurant management system with role-based access and a focus on modular operations such as menu handling, customer management, kitchen operations, and order processing. The system supports three roles: **Admin**, **Account Manager**, and **Chef**, each with distinct permissions and functionalities. The system ensures smooth order flow, kitchen communication via KOT (Kitchen Order Ticket), and management of dining areas with sections and tables.

## Actors (Roles)

* **Admin**
* **Account Manager**
* **Chef**

## Major Functional Modules & Class Diagrams

### 1. **Login & Authentication**

**Use Case:** Secure authentication, password reset, and access control.
**Class Diagram:**

* User
* LoginManager
* PasswordResetToken
* EmailService

### 2. **Dashboard**

**Use Case:** Central view with module navigation for authorized users.
**Class Diagram:**

* Dashboard
* Widget (UserStats, OrderStats, etc.)
* AccessControl

### 3. **User Management** (Admin only)

**Use Case:** Manage user accounts and roles.
**Class Diagram:**

* User
* Role
* Permission
* UserService

### 4. **Role & Permission Management** (Admin only)

**Use Case:** Assign/restrict module-level permissions.
**Class Diagram:**

* Role
* Permission
* Module
* RolePermissionMap

### 5. **Menu Management** (Admin, Account Manager)

**Use Case:** Handle categories, items, modifier groups, and modifiers.
**Class Diagram:**

* MenuCategory
* MenuItem
* ModifierGroup
* Modifier
* MenuService

### 6. **Tax Management** (Admin, Account Manager)

**Use Case:** Manage applicable taxes.
**Class Diagram:**

* Tax
* TaxService
* Invoice

### 7. **Table/Section Management** (Admin, Account Manager)

**Use Case:** Manage seating layout by section.
**Class Diagram:**

* Section
* Table
* TableAssignmentService

### 8. **Order Management** (Admin, Account Manager)

**Use Case:** Process and filter orders, generate invoices.
**Class Diagram:**

* Order
* OrderItem
* Invoice
* FilterCriteria
* ExportService

### 9. **Customer Management** (Admin, Account Manager)

**Use Case:** Manage customer info and generate reports.
**Class Diagram:**

* Customer
* CustomerService
* ExportService

### 10. **Order App** (Account Manager only)

**Use Case:** Customer seat assignment, order generation, and invoicing.
**Class Diagram:**

* Table
* WaitingToken
* Order
* Invoice
* Payment
* Feedback

### 11. **Waiting List Management** (Account Manager only)

**Use Case:** Manage customers waiting for a table.
**Class Diagram:**

* WaitingToken
* Section
* Table
* Customer

### 12. **Kitchen Order Token (KOT)** (Chef, Account Manager)

**Use Case:** Track item prep status from in-process to ready.
**Class Diagram:**

* KOT
* OrderItem
* Chef
* OrderStatusTracker

## Additional Notes

* All routes are protected via role-based access.
* Only permitted users can access certain modules (e.g., only Admin can access user list).
* Each functional module is isolated and scalable.

---
