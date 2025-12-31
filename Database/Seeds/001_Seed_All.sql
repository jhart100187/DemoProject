
\set ON_ERROR_STOP on

BEGIN;

-- =====================================================
-- USERS
-- =====================================================
INSERT INTO demoproject.users (id, firstname, lastname, email)
VALUES
    ('11111111-1111-1111-1111-111111111111', 'Alice', 'Johnson', 'alice@test.com'),
    ('22222222-2222-2222-2222-222222222222', 'Bob', 'Smith', 'bob@test.com')
ON CONFLICT (id) DO NOTHING;


-- =====================================================
-- USERS PASSWORDS
-- (example hashes – NOT real)
-- =====================================================
INSERT INTO demoproject.users_passwords (user_id, password_hash)
VALUES
    ('11111111-1111-1111-1111-111111111111', '$2b$10$alicehash'),
    ('22222222-2222-2222-2222-222222222222', '$2b$10$bobhash')
ON CONFLICT (user_id) DO NOTHING;


-- =====================================================
-- USERS SESSIONS
-- =====================================================
INSERT INTO demoproject.users_sessions (
    id,
    user_id,
    session_token_hash,
    expires_at,
    last_activity_at,
    ip_address,
    user_agent
)
VALUES
    (
        'aaaaaaaa-1111-1111-1111-aaaaaaaaaaaa',
        '11111111-1111-1111-1111-111111111111',
        'sessionhash-alice',
        now() + interval '7 days',
        now(),
        '192.168.1.10',
        'Mozilla/5.0'
    )
ON CONFLICT (id) DO NOTHING;


-- =====================================================
-- USERS ADDRESSES
-- =====================================================
INSERT INTO demoproject.users_addresses (
    id,
    user_id,
    address_line1,
    city,
    state,
    postal_code
)
VALUES
    (
        'bbbbbbbb-1111-1111-1111-bbbbbbbbbbbb',
        '11111111-1111-1111-1111-111111111111',
        '123 Main St',
        'Augusta',
        'GA',
        '30904'
    )
ON CONFLICT (id) DO NOTHING;


-- =====================================================
-- PRODUCTS
-- =====================================================
INSERT INTO demoproject.products (
    id,
    sku,
    name,
    description
)
VALUES
    (
        'cccccccc-1111-1111-1111-cccccccccccc',
        'SKU-001',
        'Golf Balls (12-pack)',
        'Premium golf balls'
    ),
    (
        'dddddddd-1111-1111-1111-dddddddddddd',
        'SKU-002',
        'Golf Glove',
        'Leather golf glove'
    )
ON CONFLICT (id) DO NOTHING;


-- =====================================================
-- PRODUCT PRICES
-- =====================================================
INSERT INTO demoproject.products_prices (
    id,
    product_id,
    price,
    effective_from
)
VALUES
    (
        'eeeeeeee-1111-1111-1111-eeeeeeeeeeee',
        'cccccccc-1111-1111-1111-cccccccccccc',
        29.99,
        now() - interval '30 days'
    ),
    (
        'ffffffff-1111-1111-1111-ffffffffffff',
        'dddddddd-1111-1111-1111-dddddddddddd',
        19.99,
        now() - interval '30 days'
    )
ON CONFLICT (id) DO NOTHING;


-- =====================================================
-- SHOPPING CARTS
-- =====================================================
INSERT INTO demoproject.shopping_carts (
    id,
    user_id
)
VALUES
    (
        '99999999-1111-1111-1111-999999999999',
        '11111111-1111-1111-1111-111111111111'
    )
ON CONFLICT (id) DO NOTHING;


-- =====================================================
-- SHOPPING CART ITEMS
-- =====================================================
INSERT INTO demoproject.shopping_carts_items (
    id,
    shopping_cart_id,
    product_id,
    quantity,
    price_at_add
)
VALUES
    (
        'abababab-1111-1111-1111-abababababab',
        '99999999-1111-1111-1111-999999999999',
        'cccccccc-1111-1111-1111-cccccccccccc',
        2,
        29.99
    ),
    (
        'cdcdcdcd-1111-1111-1111-cdcdcdcdcdcd',
        '99999999-1111-1111-1111-999999999999',
        'dddddddd-1111-1111-1111-dddddddddddd',
        1,
        19.99
    )
ON CONFLICT (id) DO NOTHING;

COMMIT;
