CREATE DATABASE EstateProManager
GO

USE EstateProManager
GO

-- Table des roles
CREATE TABLE Roles(
  Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  libelle NVARCHAR(MAX)
)
GO

-- Table des utilisateurs
CREATE TABLE Utilisateurs (
  Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  CIN NVARCHAR(MAX),
  Nom NVARCHAR(MAX) NOT NULL,
  Prenom NVARCHAR(MAX) NOT NULL,
  Email NVARCHAR(MAX) NOT NULL,
  MotDePasse NVARCHAR(MAX) NOT NULL,
  birthdate DATE NOT NULL,
  NumeroDeTelephone NVARCHAR(MAX) NULL,
  AdressePostale NVARCHAR(MAX) NULL,
  IdRole BIGINT NOT NULL,
  FOREIGN KEY (IdRole) REFERENCES Roles (Id),
);
GO

-- Table des propriétés
CREATE TABLE Proprietes (
  Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  Type NVARCHAR(MAX) NOT NULL,
  Taille BIGINT NOT NULL,
  Emplacement NVARCHAR(MAX) NOT NULL,
  Caracteristiques NVARCHAR(MAX) NOT NULL,
  Images NVARCHAR(MAX) NOT NULL,
  Documents NVARCHAR(MAX) NOT NULL,
  Valeur DECIMAL(10,2) NULL,
  DateDeConstruction DATE NULL
);
GO

-- Table des contrats de location
CREATE TABLE Contrats (
  Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  IdPropriete BIGINT NOT NULL,
  IdLocataire BIGINT NOT NULL,
  DateDebut DATE NOT NULL,
  DateFin DATE NOT NULL,
  PaiementMensuel DECIMAL(10,2) NOT NULL,
  FOREIGN KEY (IdPropriete) REFERENCES Proprietes (Id),
  FOREIGN KEY (IdLocataire) REFERENCES Utilisateurs (Id)
);
GO

-- Table des ventes
CREATE TABLE Ventes (
  Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  IdPropriete BIGINT NOT NULL,
  IdAcheteur BIGINT NOT NULL,
  IdVendeur BIGINT NOT NULL,
  PrixVente DECIMAL(10,2) NOT NULL,
  DateVente DATE NOT NULL,
  FOREIGN KEY (IdPropriete) REFERENCES Proprietes (Id),
  FOREIGN KEY (IdAcheteur) REFERENCES Utilisateurs (Id),
  FOREIGN KEY (IdVendeur) REFERENCES Utilisateurs (Id)
);
GO

-- Table des demandes
CREATE TABLE Demandes (
  Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  IdPropriete BIGINT NOT NULL,
  IdUtilisateur BIGINT NOT NULL,
  TypeDemande NVARCHAR(MAX) NOT NULL,
  DetailsDemande NVARCHAR(MAX) NOT NULL,
  FOREIGN KEY (IdPropriete) REFERENCES Proprietes (Id),
  FOREIGN KEY (IdUtilisateur) REFERENCES Utilisateurs (Id)
);
GO

-- Table des tâches
CREATE TABLE Taches (
  Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  IdDemande BIGINT NOT NULL,
  IdPrestataire BIGINT NOT NULL,
  DetailsTache NVARCHAR(MAX) NOT NULL,
  StatutTache NVARCHAR(MAX) NOT NULL,
  FOREIGN KEY (IdDemande) REFERENCES Demandes (Id),
  FOREIGN KEY (IdPrestataire) REFERENCES Utilisateurs (Id)
);

-- Inserting data into Roles table
INSERT INTO Roles (libelle)
VALUES ('Administrateur'), ('Client');
GO

-- Inserting data into Utilisateurs table
INSERT INTO Utilisateurs (CIN, Nom, Prenom, Email, MotDePasse, birthdate, NumeroDeTelephone, AdressePostale, IdRole)
VALUES 
  ('123456789', 'Yassine', 'Belouchi', 'yassine@gmail.com', '$2a$10$mVGu1ToVAUz9FxESU5KoUeeRbpoxZ.AScRslVTMBy4pw.ZhPLb8ie', '1990-01-01', '1234567890', '123 Main St', 1),
  ('987654321', 'Belouchi', 'Yassine', 'belouchi@gmail.com', '$2a$10$mVGu1ToVAUz9FxESU5KoUeeRbpoxZ.AScRslVTMBy4pw.ZhPLb8ie', '1985-05-15', '9876543210', '456 Oak St', 2);
GO


-- Inserting data into Contrats table
INSERT INTO Contrats (IdPropriete, IdLocataire, DateDebut, DateFin, PaiementMensuel)
VALUES 
  (1, 2, '2024-02-01', '2025-01-31', 2000.00),
  (2, 1, '2024-03-15', '2025-03-14', 1500.00);
GO

-- Inserting data into Ventes table
INSERT INTO Ventes (IdPropriete, IdAcheteur, IdVendeur, PrixVente, DateVente)
VALUES 
  (1, 2, 1, 230000.00, '2023-12-10'),
  (2, 1, 2, 140000.00, '2024-01-25');
GO

-- Inserting data into Demandes table
INSERT INTO Demandes (IdPropriete, IdUtilisateur, TypeDemande, DetailsDemande)
VALUES 
  (1, 2, 'Maintenance', 'Leaky roof repair'),
  (2, 1, 'Information', 'Request for property details');
GO

-- Inserting data into Taches table
INSERT INTO Taches (IdDemande, IdPrestataire, DetailsTache, StatutTache)
VALUES 
  (1, 2, 'Fix the leaky roof as soon as possible', 'Pending'),
  (2, 2, 'Provide detailed information about the property', 'Completed');