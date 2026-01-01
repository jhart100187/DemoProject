
\set ON_ERROR_STOP on

BEGIN;

-- =====================================================
-- USERS (unique by email)
-- =====================================================
INSERT INTO demoproject.users (id, firstname, lastname, email)
SELECT *
FROM (VALUES
    ('11111111-1111-1111-1111-111111111111'::uuid, 'Alice', 'Johnson', 'alice@test.com'),
    ('22222222-2222-2222-2222-222222222222'::uuid, 'Bob',   'Smith',   'bob@test.com')
) AS v(id, firstname, lastname, email)
WHERE NOT EXISTS (
    SELECT 1
    FROM demoproject.users u
    WHERE u.email = v.email
);

-- =====================================================
-- USERS PASSWORDS (1–to–1 by user_id)
-- =====================================================
INSERT INTO demoproject.users_passwords (user_id, password_hash)
SELECT *
FROM (VALUES
    ('11111111-1111-1111-1111-111111111111'::uuid, '$2b$10$alicehash'),
    ('22222222-2222-2222-2222-222222222222'::uuid, '$2b$10$bobhash')
) AS v(user_id, password_hash)
WHERE NOT EXISTS (
    SELECT 1
    FROM demoproject.users_passwords up
    WHERE up.user_id = v.user_id
);

-- =====================================================
-- USERS SESSIONS (unique by id)
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
SELECT *
FROM (VALUES
    (
        'aaaaaaaa-1111-1111-1111-aaaaaaaaaaaa'::uuid,
        '11111111-1111-1111-1111-111111111111'::uuid,
        'sessionhash-alice',
        now() + interval '7 days',
        now(),
        '192.168.1.10'::inet,
        'Mozilla/5.0'
    )
) AS v(
    id,
    user_id,
    session_token_hash,
    expires_at,
    last_activity_at,
    ip_address,
    user_agent
)
WHERE NOT EXISTS (
    SELECT 1
    FROM demoproject.users_sessions s
    WHERE s.id = v.id
);

-- =====================================================
-- USERS ADDRESSES (unique by id)
-- =====================================================
INSERT INTO demoproject.users_addresses (
    id,
    user_id,
    address_line1,
    city,
    state,
    postal_code
)
SELECT *
FROM (VALUES
    (
        'bbbbbbbb-1111-1111-1111-bbbbbbbbbbbb'::uuid,
        '11111111-1111-1111-1111-111111111111'::uuid,
        '123 Main St',
        'Augusta',
        'GA',
        '30904'
    )
) AS v(
    id,
    user_id,
    address_line1,
    city,
    state,
    postal_code
)
WHERE NOT EXISTS (
    SELECT 1
    FROM demoproject.users_addresses a
    WHERE a.id = v.id
);

-- =====================================================
-- PRODUCTS (unique by sku)
-- =====================================================
INSERT INTO demoproject.products (
    id,
    sku,
    name,
    description
)
SELECT *
FROM (VALUES
    (
        'cccccccc-1111-1111-1111-cccccccccccc'::uuid,
        'SKU-001',
        'Golf Balls (12-pack)',
        'Premium golf balls'
    ),
    (
        'dddddddd-1111-1111-1111-dddddddddddd'::uuid,
        'SKU-002',
        'Golf Glove',
        'Leather golf glove'
    )
) AS v(
    id,
    sku,
    name,
    description
)
WHERE NOT EXISTS (
    SELECT 1
    FROM demoproject.products p
    WHERE p.sku = v.sku
);

-- =====================================================
-- PRODUCT PRICES (unique by id)
-- =====================================================
INSERT INTO demoproject.products_prices (
    id,
    product_id,
    price,
    effective_from
)
SELECT *
FROM (VALUES
    (
        'eeeeeeee-1111-1111-1111-eeeeeeeeeeee'::uuid,
        'cccccccc-1111-1111-1111-cccccccccccc'::uuid,
        29.99,
        now() - interval '30 days'
    ),
    (
        'ffffffff-1111-1111-1111-ffffffffffff'::uuid,
        'dddddddd-1111-1111-1111-dddddddddddd'::uuid,
        19.99,
        now() - interval '30 days'
    )
) AS v(
    id,
    product_id,
    price,
    effective_from
)
WHERE NOT EXISTS (
    SELECT 1
    FROM demoproject.products_prices pp
    WHERE pp.id = v.id
);

-- =====================================================
-- SHOPPING CARTS (1 per user)
-- =====================================================
INSERT INTO demoproject.shopping_carts (id, user_id)
SELECT *
FROM (VALUES
    (
        '99999999-1111-1111-1111-999999999999'::uuid,
        '11111111-1111-1111-1111-111111111111'::uuid
    )
) AS v(
    id,
    user_id
)
WHERE NOT EXISTS (
    SELECT 1
    FROM demoproject.shopping_carts c
    WHERE c.user_id = v.user_id
);

-- =====================================================
-- SHOPPING CART ITEMS (unique by id)
-- =====================================================
INSERT INTO demoproject.shopping_carts_items (
    id,
    shopping_cart_id,
    product_id,
    quantity,
    price_at_add
)
SELECT *
FROM (VALUES
    (
        'abababab-1111-1111-1111-abababababab'::uuid,
        '99999999-1111-1111-1111-999999999999'::uuid,
        'cccccccc-1111-1111-1111-cccccccccccc'::uuid,
        2,
        29.99
    ),
    (
        'cdcdcdcd-1111-1111-1111-cdcdcdcdcdcd'::uuid,
        '99999999-1111-1111-1111-999999999999'::uuid,
        'dddddddd-1111-1111-1111-dddddddddddd'::uuid,
        1,
        19.99
    )
) AS v(
    id,
    shopping_cart_id,
    product_id,
    quantity,
    price_at_add
)
WHERE NOT EXISTS (
    SELECT 1
    FROM demoproject.shopping_carts_items sci
    WHERE sci.id = v.id
);

COMMIT;
