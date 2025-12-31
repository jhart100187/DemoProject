
\set ON_ERROR_STOP on

CREATE INDEX IF NOT EXISTS ix_users_passwords_user_id
ON demoproject.users_passwords (user_id);

CREATE UNIQUE INDEX IF NOT EXISTS ux_users_sessions_token_hash
ON demoproject.users_sessions (session_token_hash);

CREATE INDEX IF NOT EXISTS ix_users_sessions_user_id
ON demoproject.users_sessions (user_id);

CREATE INDEX IF NOT EXISTS ix_users_sessions_expires_at
ON demoproject.users_sessions (expires_at);

CREATE INDEX IF NOT EXISTS ix_products_sku
ON demoproject.products (sku);

CREATE INDEX IF NOT EXISTS ix_products_prices_product_id
ON demoproject.products_prices (product_id);

CREATE INDEX IF NOT EXISTS ix_shopping_carts_items_shopping_cart_id
ON demoproject.shopping_carts_items (shopping_cart_id);