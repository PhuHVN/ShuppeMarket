# 🛒 Shopee Mini – E-Commerce Backend API

A lightweight Shopee-style e-commerce backend supporting **Buyer**, **Seller**, and **Admin** roles.

This project includes functionalities for product management, cart handling, orders, payments, vouchers, and product reviews.

---

## 🚀 Key Features

* 🔐 **JWT Authentication**: Secure user authentication.
* 👤 **Buyer & Seller roles**: Role-based user access.
* 🏪 **Seller shop management**: Tools for sellers to manage their shops.
* 📦 **Product + Variants + Images**: Comprehensive product management, including variants and images.
* 🛒 **Cart & Checkout**: Cart management and the checkout process.
* 📑 **Orders & Order Details**: Handling orders and their details.
* 💳 **Payment handling**: Processing payment transactions.
* 🎟 **Voucher discount**: Applying discount vouchers.
* ⭐ **Product reviews**: Allowing buyers to submit product reviews.

---

## 🛠 Tech Stack

* **ASP.NET Core**
* **Entity Framework Core**
* **SQL Server**
* **JWT Authentication**
* **Swagger (OpenAPI)**: For API documentation and testing.

---

## 🔥 Business Flow

1.  **Register / Login**
2.  **Buyer → Seller** (Create shop)
3.  **Seller** adds a product
4.  **Buyer** adds product to cart
5.  **Checkout** (with/without voucher)
6.  **Order + OrderDetails** created
7.  **Payment confirmation**
8.  **Seller** processes the order
9.  **Buyer** reviews the product

---

## 📦 Setup & Run

### 1. Clone Project

```bash
git clone <your-repo-url>
cd <project-folder>
