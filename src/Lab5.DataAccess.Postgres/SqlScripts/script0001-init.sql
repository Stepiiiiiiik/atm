CREATE TABLE IF NOT EXISTS account
(
    account_id UUID PRIMARY KEY,
    pin_hash CHAR(64) NOT NULL,
    balance DECIMAL(15, 2) DEFAULT 0.00
);

CREATE TABLE IF NOT EXISTS operation_type
(
    operation_type_id SERIAL PRIMARY KEY,
    operation_name VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS session_type
(
    session_type_id SERIAL PRIMARY KEY,
    session_type_name TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS session
(
    session_key UUID PRIMARY KEY,
    session_type_id INTEGER,
    account_id UUID,

    FOREIGN KEY (session_type_id) REFERENCES session_type(session_type_id),
    FOREIGN KEY (account_id) REFERENCES account(account_id)
);

CREATE TABLE IF NOT EXISTS history_item
(
    history_item_id UUID PRIMARY KEY,
    account_id UUID NOT NULL,
    operation_type_id INTEGER NOT NULL,
    value DECIMAL(15, 2) NULL,
    session_key UUID,
    success BOOL NOT NULL DEFAULT false,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (account_id) REFERENCES account(account_id),
    FOREIGN KEY (operation_type_id) REFERENCES operation_type(operation_type_id),
    FOREIGN KEY (session_key) REFERENCES session(session_key)
);

INSERT INTO operation_type (operation_name) 
VALUES
('CREATE'),
('BALANCE'),
('DEPOSIT'),
('WITHDRAW'),
('HISTORY');

INSERT INTO session_type (session_type_name)
VALUES
('USER'),
('ADMIN');