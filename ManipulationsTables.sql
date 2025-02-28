USE LivInParis;
SELECT * FROM Personne;
SELECT * FROM Clients;
SELECT * FROM Commandes;
SELECT * FROM Ingredients;
SELECT * FROM Composition;
SELECT * FROM Notes;
SELECT * FROM Livraisons;

SELECT * FROM Commandes WHERE client_id = 1;

UPDATE Personne 
SET email = 'titouan.ducourau@gmail.com' 
WHERE prenom = 'Alice' AND nom = 'Dupont';

UPDATE Composition 
SET id_ingredient = 5
WHERE num_commande = 2 AND id_ingredient = 3;

DELETE FROM Notes WHERE client_id = 3 AND cuisinier_id = 1;

DESC Personne;

SHOW Tables;