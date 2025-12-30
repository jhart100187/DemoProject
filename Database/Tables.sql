
\set ON_ERROR_STOP on

CREATE TABLE IF NOT EXISTS demoproject.users (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    firstname varchar(50) NOT NULL,
    lastname varchar(50) NOT NULL,
    email varchar(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS demoproject.users_passwords (
    user_id uuid NOT NULL,
    password_hash text NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT pk_users_passwords PRIMARY KEY (user_id),
    CONSTRAINT fk_users_passwords_users
        FOREIGN KEY (user_id)
        REFERENCES demoproject.users (id)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS demoproject.users_sessions (
    id uuid PRIMARY KEY,
    user_id uuid NOT NULL,
    session_token_hash text NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now(),
    expires_at timestamptz NOT NULL,
    last_activity_at timestamptz,
    ip_address inet,
    user_agent text,
    is_revoked boolean NOT NULL DEFAULT false,

    CONSTRAINT fk_users_sessions_users
        FOREIGN KEY (user_id)
        REFERENCES demoproject.users (id)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS demoproject.users_addresses (
    id uuid PRIMARY KEY,
    user_id uuid NOT NULL,
    address_line1 varchar(100) NOT NULL,
    address_line2 varchar(100),
    city varchar(50) NOT NULL,
    state char(2) NOT NULL,
    postal_code varchar(10) NOT NULL,
    country char(2) NOT NULL DEFAULT 'US',
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT fk_users_addresses_users
        FOREIGN KEY (user_id)
        REFERENCES demoproject.users (id)
        ON DELETE CASCADE,

    CONSTRAINT chk_users_addresses_state
        CHECK (state ~ '^[A-Z]{2}$'),

    CONSTRAINT chk_users_addresses_postal_code
        CHECK (postal_code ~ '^[0-9]{5}(-[0-9]{4})?$'),

    CONSTRAINT chk_users_addresses_country
        CHECK (country = 'US')
);

CREATE TABLE IF NOT EXISTS demoproject.products (
    id uuid PRIMARY KEY,
    sku varchar(50) NOT NULL UNIQUE,
    name varchar(100) NOT NULL,
    description text,
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS demoproject.products_prices (
    id uuid PRIMARY KEY,
    product_id uuid NOT NULL,
    price numeric(10,2) NOT NULL,
    currency char(3) NOT NULL DEFAULT 'USD',
    effective_from timestamptz NOT NULL,
    effective_to timestamptz,

    CONSTRAINT fk_products_prices_products
        FOREIGN KEY (product_id)
        REFERENCES demoproject.products (id)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS demoproject.products_shopping_carts (
    id uuid PRIMARY KEY,
    user_id uuid NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT fk_products_shopping_carts_users
        FOREIGN KEY (user_id)
        REFERENCES demoproject.users (id)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS demoproject.products_shopping_carts_items (
    id uuid PRIMARY KEY,
    shopping_cart_id uuid NOT NULL,
    product_id uuid NOT NULL,
    quantity integer NOT NULL CHECK (quantity > 0),
    price_at_add numeric(10,2) NOT NULL,
    currency char(3) NOT NULL DEFAULT 'USD',
    added_at timestamptz NOT NULL DEFAULT now(),

    CONSTRAINT fk_cart_items_carts
        FOREIGN KEY (shopping_cart_id)
        REFERENCES demoproject.products_shopping_carts (id)
        ON DELETE CASCADE,

    CONSTRAINT fk_cart_items_products
        FOREIGN KEY (product_id)
        REFERENCES demoproject.products (id)
);
