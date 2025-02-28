USE LivInParis;
INSERT INTO Personne (prenom, nom, codePostal, ville, email, mdp, menuAJour) 
VALUES 
('Alice', 'Dupont', '75001', 'Paris', 'alice.dupont@email.com', 'motdepasse1', TRUE),
('Bob', 'Martin', '69002', 'Lyon', 'bob.martin@email.com', 'motdepasse2', FALSE),
('Charlie', 'Durand', '13008', 'Marseille', 'charlie.durand@email.com', 'motdepasse3', TRUE),
('David', 'Lambert', '31000', 'Toulouse', 'david.lambert@email.com', 'motdepasse4', FALSE),
('Emma', 'Moreau', '44000', 'Nantes', 'emma.moreau@email.com', 'motdepasse5', TRUE),
('Fanny', 'Bertrand', '67000', 'Strasbourg', 'fanny.bertrand@email.com', 'motdepasse6', FALSE),
('Gabriel', 'Lemoine', '59000', 'Lille', 'gabriel.lemoine@email.com', 'motdepasse7', TRUE),
('Hugo', 'Rousseau', '06000', 'Nice', 'hugo.rousseau@email.com', 'motdepasse8', FALSE),
('Isabelle', 'Blanc', '34000', 'Montpellier', 'isabelle.blanc@email.com', 'motdepasse9', TRUE),
('Matéo', 'Dubreuil', '87000', 'Limoges', 'mateo.dubreuil@edu.devinci.fr', 'avecplaisir!', TRUE),
('Julien', 'Giraud', '80000', 'Amiens', 'julien.giraud@email.com', 'motdepasse10', FALSE);
INSERT INTO Clients (numPers, particulier, entrepriseLocal)
VALUES 
(1, TRUE, FALSE),
(2, FALSE, TRUE),
(3, TRUE, FALSE),
(4, TRUE, FALSE),
(5, TRUE, FALSE),
(6, TRUE, FALSE),
(7, TRUE, FALSE),
(8, TRUE, FALSE),
(9, TRUE, FALSE),
(10, TRUE, FALSE),
(11, TRUE, FALSE);
INSERT INTO Commandes (date_commande, heure_commande, client_id) 
VALUES 
('2025-02-15', '15:02:05', 5),
('2025-02-20', '12:15:00', 1),
('2025-02-21', '14:30:00', 2),
('2025-02-22', '19:45:00', 3),
('2025-02-23', '11:00:00', 4),
('2025-02-24', '13:20:00', 5),
('2025-02-25', '18:10:00', 6),
('2025-02-26', '20:00:00', 7),
('2025-02-27', '09:30:00', 8),
('2025-02-28', '17:50:00', 9),
('2025-03-01', '21:15:00', 10),
('2025-03-02', '22:12:00', 11);
INSERT INTO Ingredients (nom) 
VALUES 
('Tomate'),
('Mozzarella'),
('Basilic'),
('Poulet'),
('Champignon'),
('Oignon'),
('Piment'),
('Thon'),
('Fromage'),
('Olive'),
('Magret de canard'),
('Rhum'),
('Saumon'),
('Jambon'),
('Patates');
INSERT INTO Composition (num_commande, id_ingredient) 
VALUES 
(1, 1),
(1, 2),
(2, 3),
(2, 4),
(3, 5),
(4, 6),
(5, 7),
(6, 8),
(7, 9),
(8, 10),
(9, 10),
(10, 10),
(11, 2),
(11, 3),
(11, 6);
INSERT INTO Notes (etoile, commentaire, client_id, cuisinier_id) 
VALUES 
(5, 'Délicieux, rien à dire !', 1, 2),
(4, 'Très bon, mais un peu froid.', 2, 3),
(3, 'Correct mais peut être amélioré.', 3, 1),
(5, 'Incroyable, j\'ai adoré !', 4, 5),
(2, 'Trop salé, déçu...', 5, 4),
(4, 'Bon équilibre des saveurs.', 6, 2),
(1, 'Mauvaise expérience, plat trop sec.', 7, 3),
(5, 'Super plat, je recommande.', 8, 1),
(3, 'Pas mal mais manque d\'assaisonnement.', 9, 4),
(4, 'Très bon mais un peu cher. Les cannelés bordelais sont délicieux. La commande a rien à voir avec ça mais je le précise quand même.', 10, 5);
INSERT INTO Livraisons (date_livraison, num_commande) 
VALUES 
('2025-02-21', 1),
('2025-02-22', 2),
('2025-02-23', 3),
('2025-02-24', 4),
('2025-02-25', 5),
('2025-02-26', 6),
('2025-02-27', 7),
('2025-02-28', 8),
('2025-02-28', 9),
('2025-03-01', 10),
('2025-03-01', 11);