CREATE DATABASE LivInParis;
USE LivInParis;

CREATE TABLE Utilisateur (
  id_utilisateur INT AUTO_INCREMENT PRIMARY KEY,
  prenom VARCHAR(50) NOT NULL,
  nom VARCHAR(50) NOT NULL,
  adresse VARCHAR(255) NOT NULL,
  code_postal VARCHAR(10) NOT NULL,
  email VARCHAR(100) NOT NULL UNIQUE,
  mdp VARCHAR(255) NOT NULL,
  est_client BOOLEAN DEFAULT FALSE,
  est_cuisinier BOOLEAN DEFAULT FALSE,
  type_client ENUM('Particulier','Entreprise') DEFAULT NULL
);

CREATE TABLE Plat (
  id_plat INT AUTO_INCREMENT PRIMARY KEY,
  id_cuisinier INT NOT NULL,
  nom_plat VARCHAR(100) NOT NULL,
  nb_personnes INT NOT NULL,-- nombre de personnes que le plat sert
  date_fabrication DATE NOT NULL,
  prix_par_personne DECIMAL(8,2) NOT NULL,
  nationalite VARCHAR(50) NOT NULL,
  regime_alimentaire VARCHAR(50),
  FOREIGN KEY (id_cuisinier) REFERENCES Utilisateur(id_utilisateur)
);

CREATE TABLE Ingredient (
  id_ingredient INT AUTO_INCREMENT PRIMARY KEY,
  nom VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE PlatIngredients (
  id_plat INT,
  id_ingredient INT,
  PRIMARY KEY (id_plat, id_ingredient),
  FOREIGN KEY (id_plat) REFERENCES Plat(id_plat) ON DELETE CASCADE,
  FOREIGN KEY (id_ingredient) REFERENCES Ingredient(id_ingredient) ON DELETE CASCADE
);

CREATE TABLE Commande (
  id_commande INT AUTO_INCREMENT PRIMARY KEY,
  id_client INT NOT NULL,
  date_commande DATETIME NOT NULL,
  montant_total DECIMAL(10,2) NOT NULL,
  FOREIGN KEY (id_client) REFERENCES Utilisateur(id_utilisateur)
);

CREATE TABLE LigneCommande (
  id_ligne INT AUTO_INCREMENT PRIMARY KEY,
  id_commande INT NOT NULL,
  id_plat INT NOT NULL,
  quantite INT NOT NULL,
  FOREIGN KEY (id_commande) REFERENCES Commande(id_commande) ON DELETE CASCADE,
  FOREIGN KEY (id_plat) REFERENCES Plat(id_plat)
);

CREATE TABLE Avis (
  id_avis INT AUTO_INCREMENT PRIMARY KEY,
  id_commande INT NOT NULL,
  id_client INT NOT NULL,
  id_cuisinier INT NOT NULL,
  etoile INT CHECK (etoile BETWEEN 1 AND 5),
  commentaire VARCHAR(2000),
  date_avis DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (id_commande) REFERENCES Commande(id_commande) ON DELETE CASCADE,
  FOREIGN KEY (id_client) REFERENCES Utilisateur(id_utilisateur),
  FOREIGN KEY (id_cuisinier) REFERENCES Utilisateur(id_utilisateur)
);