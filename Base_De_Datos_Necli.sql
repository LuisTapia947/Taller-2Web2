USE necli;

DROP TABLE IF EXISTS Transacciones;
DROP TABLE IF EXISTS Cuentas;
DROP TABLE IF EXISTS Usuarios;

CREATE TABLE Usuarios (
    Id VARCHAR(20) PRIMARY KEY,
    Contraseña VARCHAR(255), 
    NombreUsuario VARCHAR(50),
    ApellidoUsuario VARCHAR(50),
    Correo VARCHAR(100) UNIQUE
);

CREATE TABLE Cuentas (
    Numero BIGINT PRIMARY KEY IDENTITY(1,1),
    UsuarioId VARCHAR(20),
    Saldo DECIMAL(10,2) DEFAULT 0.00,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
);

CREATE TABLE Transacciones (
    NumeroTransaccion INT PRIMARY KEY IDENTITY(1,1), 
    FechaTransaccion DATETIME DEFAULT GETDATE(),
    NumeroCuentaOrigen BIGINT ,
    NumeroCuentaDestino BIGINT,
    Monto DECIMAL(10,2)  CHECK (Monto BETWEEN 1000 AND 5000000),
    Tipo TINYINT CHECK (Tipo IN (1, 2)),
    FOREIGN KEY (NumeroCuentaOrigen) REFERENCES Cuentas(Numero),
    FOREIGN KEY (NumeroCuentaDestino) REFERENCES Cuentas(Numero),
    CONSTRAINT chk_cuentas_diferentes CHECK (NumeroCuentaOrigen <> NumeroCuentaDestino)
);


INSERT INTO Usuarios (Id, Contraseña, NombreUsuario, ApellidoUsuario, Correo)
VALUES 
('1001', HASHBYTES('SHA2_256', 'Clave123'), 'Juan', 'Pérez', 'juan.perez@email.com'),
('1002', HASHBYTES('SHA2_256', 'Clave123'), 'María', 'Gómez', 'maria.gomez@email.com'),
('1003', HASHBYTES('SHA2_256', 'Clave123'), 'Carlos', 'Rodríguez', 'carlos.rodriguez@email.com'),
('1004', HASHBYTES('SHA2_256', 'Clave123'), 'Ana', 'Fernández', 'ana.fernandez@email.com'),
('1005', HASHBYTES('SHA2_256', 'Clave123'), 'Pedro', 'López', 'pedro.lopez@email.com'),
('1006', HASHBYTES('SHA2_256', 'Clave123'), 'Sofía', 'Martínez', 'sofia.martinez@email.com'),
('1007', HASHBYTES('SHA2_256', 'Clave123'), 'Diego', 'Sánchez', 'diego.sanchez@email.com'),
('1008', HASHBYTES('SHA2_256', 'Clave123'), 'Laura', 'Torres', 'laura.torres@email.com'),
('1009', HASHBYTES('SHA2_256', 'Clave123'), 'Javier', 'Ramírez', 'javier.ramirez@email.com'),
('1010', HASHBYTES('SHA2_256', 'Clave123'), 'Camila', 'Ortega', 'camila.ortega@email.com');

INSERT INTO Cuentas (UsuarioId, Saldo, FechaCreacion)
VALUES 
('1001', 50000, GETDATE()),
('1002', 200000, GETDATE()),
('1003', 75000, GETDATE()),
('1004', 120000, GETDATE()),
('1005', 300000, GETDATE()),
('1006', 500000, GETDATE()),
('1007', 45000, GETDATE()),
('1008', 150000, GETDATE()),
('1009', 90000, GETDATE()),
('1010', 250000, GETDATE());


select * from Usuarios;