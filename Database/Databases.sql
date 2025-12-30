
\set ON_ERROR_STOP on

DO
$$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_database
        WHERE datname = 'demoproject'
    ) THEN
        CREATE DATABASE demoproject;
    END IF;
END
$$;
