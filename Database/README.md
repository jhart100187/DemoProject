# DemoProject – Core Domain Database Schema

This database schema represents the **core domain model** for a modern, scalable web application that supports:

- User identity and authentication
- Secure session management
- User profile and address data
- Product catalog and pricing
- Shopping cart functionality

The schema is designed with **security, normalization, and extensibility** in mind and follows best practices commonly used in production systems.

---

## High-Level Goals

The goals of this schema are to:

- Separate **authentication concerns** from user profile data
- Support **secure, revocable user sessions**
- Model **products and pricing independently**
- Allow **shopping carts to persist and track price history**
- Enforce data integrity through foreign keys and constraints

---

## User & Identity Domain

### `users`
The `users` table stores **core user identity information**.

**Responsibilities:**
- Represents a single user in the system
- Acts as the parent entity for authentication, sessions, and addresses

**Key Design Choices:**
- UUID primary key for scalability
- No password data stored here (security separation)

---

### `users_passwords`
Stores **hashed user passwords**, separated from the `users` table.

**Responsibilities:**
- Secure storage of password hashes
- Supports password rotation or history if extended later

**Why Separate This Table?**
- Reduces exposure of sensitive data
- Allows alternative authentication strategies (OAuth, SSO)
- Follows security best practices

---

### `users_sessions`
Tracks **authenticated user sessions**.

**Responsibilities:**
- Supports multiple active sessions per user
- Enables session expiration and revocation
- Stores metadata for security auditing

**Supported Features:**
- Token hashing (no raw tokens stored)
- Session expiration
- Revocation (logout, security events)
- Optional device and IP tracking

---

### `users_addresses`
Stores **one or more addresses per user**.

**Responsibilities:**
- Supports shipping and billing use cases
- Enforces U.S.-specific address validation

**Key Constraints:**
- Valid U.S. state codes
- Valid ZIP or ZIP+4 postal codes
- Country restricted to `US` (can be expanded later)

---

## Product Catalog Domain

### `products`
Represents **products available for purchase**.

**Responsibilities:**
- Stores product identity and descriptive data
- Controls product availability via `is_active`

**Key Design Choices:**
- SKU enforced as unique
- No pricing stored directly in this table

---

### `products_prices`
Manages **product pricing over time**.

**Responsibilities:**
- Allows price changes without overwriting history
- Supports future promotions and price schedules

**Why Separate Pricing?**
- Prevents breaking existing carts
- Supports historical pricing and audits
- Enables advanced pricing strategies

---

## Shopping Cart Domain

### `products_shopping_carts`
Represents a **user’s active shopping cart**.

**Responsibilities:**
- Associates a cart with a user
- Tracks cart lifecycle timestamps

**Design Intent:**
- One or more carts per user (historically)
- Can be extended to support session-based carts

---

### `products_shopping_carts_items`
Stores **items added to a shopping cart**.

**Responsibilities:**
- Tracks which products are in a cart
- Stores quantity and price at the time of add

**Why Store `price_at_add`?**
- Protects users from price changes mid-session
- Preserves pricing accuracy during checkout
- Enables consistent order creation later

---

## Data Integrity & Best Practices

This schema enforces:

- Referential integrity via foreign keys
- Cascade deletes for dependent data
- Input validation using CHECK constraints
- UUID-based identifiers for distributed systems

---

## Intended Use

This schema is designed to support:
- ASP.NET / C# backend services
- REST or GraphQL APIs
- Secure authentication workflows
- E-commerce or transactional platforms

It is intentionally modular and can be extended to include:
- Orders and order items
- Inventory tracking
- Promotions and discounts
- Payment processing records

---

## Summary

This schema demonstrates:
- Thoughtful domain modeling
- Security-first design
- Real-world production patterns
- Clear separation of concerns

It serves as a strong foundation for a **full-stack demo application** showcasing backend, frontend, and infrastructure skills.
