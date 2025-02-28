CREATE DATABASE LivInParis;
USE LivInParis;
CREATE TABLE Personne (
  numPers INT AUTO_INCREMENT PRIMARY KEY,
  prenom VARCHAR(50) NOT NULL,
  nom VARCHAR(50) NOT NULL,
  codePostal VARCHAR(10),
  ville VARCHAR(50),
  email VARCHAR(100),
  mdp VARCHAR(255),
  menuAJour BOOLEAN
);
CREATE TABLE Clients (
  numPers INT PRIMARY KEY,
  particulier BOOLEAN,
  entrepriseLocal BOOLEAN,
  FOREIGN KEY (numPers) REFERENCES Personne(numPers)
);
CREATE TABLE Commandes (
  num_commande INT AUTO_INCREMENT PRIMARY KEY,
  date_commande DATE,
  heure_commande TIME,
  client_id INT,
  FOREIGN KEY (client_id) REFERENCES Clients(numPers)
);
CREATE TABLE Ingredients (
  id_ingredient INT AUTO_INCREMENT PRIMARY KEY,
  nom VARCHAR(100)
);
CREATE TABLE Composition (
  num_commande INT,
  id_ingredient INT,
  PRIMARY KEY (num_commande, id_ingredient),
  FOREIGN KEY (num_commande) REFERENCES Commandes(num_commande),
  FOREIGN KEY (id_ingredient) REFERENCES Ingredients(id_ingredient)
);
CREATE TABLE Notes (
  id_note INT AUTO_INCREMENT PRIMARY KEY,
  etoile INT,
  commentaire VARCHAR(2000),
  client_id INT,
  cuisinier_id INT,
  FOREIGN KEY (client_id) REFERENCES Clients(numPers),
  FOREIGN KEY (cuisinier_id) REFERENCES Personne(numPers)
);
CREATE TABLE Livraisons (
  id_livraison INT AUTO_INCREMENT PRIMARY KEY,
  date_livraison DATE,
  num_commande INT,
  FOREIGN KEY (num_commande) REFERENCES Commandes(num_commande)
);