
\set ON_ERROR_STOP on

-- =========================================
-- Run database-level changes (NO TRANSACTION)
-- =========================================
\echo 'Running Databases.sql'
\i ./Databases.sql

-- =========================================
-- Begin transaction for schema-level changes
-- =========================================
\echo 'Beginning transaction'
BEGIN;

\echo 'Running Extensions.sql'
\i ./Extensions.sql

\echo 'Running Schemas.sql'
\i ./Schemas.sql

\echo 'Running Tables.sql'
\i ./Tables.sql

\echo 'Running Indexes.sql'
\i ./Indexes.sql

-- =========================================
-- Commit transaction
-- =========================================
COMMIT;
\echo 'Transaction committed successfully'
