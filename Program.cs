﻿using KarateGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Graphviz4Net.Graphs;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using System.Reflection.PortableExecutable;

namespace ADUFORET_TDUCOURAU_JESPINOS
{
    public class Program
    {
        /*public static int[,] creationMatrice(string chemin)
        {

            // création d'une matrice d'adjacence à partir d'un fichier de type .mtx

            int[,] matriceUsers = null;

            StreamReader lecteur = new(chemin);
            string ligne = "";
            ligne = lecteur.ReadLine();

            string[] header = ligne.Split(" "); //le header est la première ligne du fichier .mtx
            
            if (header[2] == "coordinate") // on vérfie que le format est coordinate
            {
                Console.WriteLine("format = coordinate");
                while ((ligne = lecteur.ReadLine()) != null)
                {
                    if (ligne != null && ligne[0] != '%' && ligne.Length > 0)//chaque comment lines commence par un % donc on ne passe pas dessus
                    {
                        string[] tabline = ligne.Split(' '); // on convertit chaque ligne dans un tableau pour avoir les données séparées
                        if (tabline.Length == 3) /// la ligne size ligne est de forme m n nonzeros avec m le nombre de ligne et n le nombre de colonnes
                        {
                            Console.WriteLine("Size Line = " + ligne);
                            matriceUsers = new int[Convert.ToInt32(tabline[0]), Convert.ToInt32(tabline[1])]; //On créer une matrice des dimensions précisées dans la size line.
                        }
                    }
                }
            }
            return matriceUsers;
        }
        public static int[,] RemplissageMatrice(int[,] matrice, string ligne, StreamReader fichier)
        {
            bool symetric = false;
            string[] header = ligne.Split(" ");
            if (header[4] == "symmetric")
            {
                symetric = true;
            }
            if (matrice != null)
            {
                Console.WriteLine("taille matrice : " + matrice.GetLength(0));
                while ((ligne = fichier.ReadLine()) != null)
                {
                    if (ligne != null && ligne[0] != '%' && ligne.Length > 0)
                    {

                        string[] tabline = ligne.Split(' ');

                        if (tabline.Length == 2)
                        {
                            matrice[Convert.ToInt32(tabline[0]) - 1, Convert.ToInt32(tabline[1]) - 1] = 1;
                            if (symetric)
                            {
                                matrice[Convert.ToInt32(tabline[1]) - 1, Convert.ToInt32(tabline[0]) - 1] = 1;
                            }

                        }
                    }
                }
            }
            return matrice;
        }
        public static Dictionary<int, List<int>> creationListeAdjacence(string chemin)
        {
            var adjacencyList = new Dictionary<int, List<int>>();
            int noeuds = 0;

            StreamReader lecteur = new(chemin);
            string ligne = "";
            ligne = lecteur.ReadLine();

            string[] header = ligne.Split(" "); //le header est la première ligne du fichier .mtx

            if (header[2] == "coordinate") // on vérfie que le format est coordinate
            {
                while ((ligne = lecteur.ReadLine()) != null)
                {
                    if (ligne != null && ligne[0] != '%' && ligne.Length > 0)//chaque comment lines commence par un % donc on ne passe pas dessus
                    {
                        string[] tabline = ligne.Split(' '); // on convertit chaque ligne dans un tableau pour avoir les données séparées
                        if (tabline.Length == 3) /// la ligne size ligne est de forme m n nonzeros avec m le nombre de ligne et n le nombre de colonnes
                        {
                            noeuds = int.Parse(tabline[0]); // le nombre de noeuds
                            break;
                        }
                    }
                }
            }
            //Initialisation de la liste d'adjacence
            for (int i = 1; i <= noeuds; i++)
            {
                adjacencyList[i] = new List<int>();
            }

            while ((ligne = lecteur.ReadLine()) != null)
            {
                if (ligne.Length > 0 && ligne[0] != '%') // Encore pour ignorer les commentaires
                {
                    string[] tabline = ligne.Split(' ');
                    if (tabline.Length >= 2) // Vérifier qu'on a bien des données
                    {
                        int u = int.Parse(tabline[0]);
                        int v = int.Parse(tabline[1]);

                        adjacencyList[u].Add(v);
                        adjacencyList[v].Add(u); // Graphe non orienté
                    }
                }
            }
            return adjacencyList;
        }
        public static void affichageMatrice(int[,] matrice)
        {
            //affichage de la matrice
            for (int i = 0; i < matrice.GetLength(0); i++)
            {
                for (int ii = 0; ii < matrice.GetLength(1); ii++)
                {
                    Console.Write(matrice[i, ii] + ", ");
                }
                Console.WriteLine();
            }
        }
        public static void affichageListe(Dictionary<int, List<int>> listeAdjacence)
        {
            foreach (var node in listeAdjacence)
            {
                Console.Write($"{node.Key}: ");
                Console.WriteLine(string.Join(", ", node.Value));
            }

        }
        public static int parcoursProfondeur(int[,] matrice, int depart, bool affichage)
        {
            int n = matrice.GetLength(0);
            bool[] visite = new bool[n];
            Pile pile = new Pile(); // j'ai créer une classe pile avant de me rendre compte que Stack<T> existe

            int result = DFS(depart, matrice, visite, pile, 0, affichage);
            return result;
        }
        private static int DFS(int sommet, int[,] matrice, bool[] visite, Pile pile, int c, bool affichage)
        {
            visite[sommet] = true;

            if(affichage)
            {
                Console.WriteLine(sommet);
            }
            
            c++;

            for (int i = 0; i < matrice.GetLength(0); i++)
            {
                if (matrice[sommet, i] == 1 && !visite[i])
                {
                    pile.add(i);
                    c = DFS(i, matrice, visite, pile, c, affichage); // appel récursif (on incrémente le compteur, il va servir pour estConnexe
                    pile.remove();
                }
            }
            return c;   
        }
        public static int parcoursLargeur(int[,] matrice, int depart)
        {
            int n = matrice.GetLength(0);
            int c = 0;
            bool[] visite = new bool[n]; // tableau des noeuds déjà visités
            Queue<int> file = new Queue<int>();

            visite[depart] = true;
            file.Enqueue(depart);

            while (file.Count > 0)
            {
                int sommet = file.Dequeue();
                Console.WriteLine(sommet);
                c++;

                for (int i = 0; i < matrice.GetLength(0); i++)
                {
                    if (matrice[sommet, i] !=0 && !visite[i])
                    {
                        visite[i] = true;
                        file.Enqueue(i);
                    }
                }
            }
            return c;
        }
        /*static void GenererImageGraphe(int[,] matrice)
        {

            string cheminDot = "graph.dot";
            string[] colors = { "red", "blue", "green", "yellow", "orange", "purple", "pink", "cyan", "gray" };
            Random rand = new Random();

            int n = matrice.GetLength(0);
            string dotContent = "graph G {\n";

            // Ajouter les nœuds avec des couleurs aléatoires
            for (int i = 0; i < n; i++)
            {
                string color = colors[rand.Next(colors.Length)];
                dotContent += $"    {i} [style=filled, fillcolor={color}];\n";
            }

            // Ajouter les arêtes
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++) // Graphe non orienté, éviter les doublons
                {
                    if (matrice[i, j] == 1)
                    {
                        dotContent += $"    {i} -- {j};\n";
                    }
                }
            }

            dotContent += "}";

            // Sauvegarder le fichier DOT
            File.WriteAllText(cheminDot, dotContent);
            Console.WriteLine($"Fichier DOT généré : {Path.GetFullPath(cheminDot)}");

            // génération de l'image avec Graphviz
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = @"C:\Program Files\Graphviz\bin\dot.exe", // je précise la location de DOT parce l'ide ne le trouvais pas
                Arguments = "-Tpng graph.dot -o graph.png",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                Process process = Process.Start(psi);
                if (process == null)
                {
                    Console.WriteLine("Le processus n'a pas pu être démarré.");
                }
                else
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string errors = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(errors))
                    {
                        Console.WriteLine("Erreurs Graphviz : " + errors);
                    }
                    else
                    {
                        Console.WriteLine("Graph généré avec succès : graph.png");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors du démarrage du processus : " + ex.Message);
            }
        }

        public static bool estConnexe(int[,] matrice)
        {
            bool resultat = true;

            for(int i = 0; i<matrice.GetLength(0);i++)
            {
                if(parcoursProfondeur(matrice, i, false) != matrice.GetLength(0)) //on fait un dfs depuis chaque noeud et on vérifie qu'il passe par tous les noeuds
                {
                    resultat = false;
                }
            }

            return resultat;
        }

        public static bool ContientCycle(int[,] matrice)
        {
            int n = matrice.GetLength(0);
            bool[] visite = new bool[n];

            // On parcourt tous les sommets pour vérifier s'il y a des cycles
            for (int i = 0; i < n; i++)
            {
                if (!visite[i] && RechercheCycle(i, matrice, visite, -1)) // Le -1 représente l'absence de parent pour le sommet initial
                {
                    return true;
                }
            }
            return false;
        }

        private static bool RechercheCycle(int sommet, int[,] matrice, bool[] visite, int parent)
        {
            visite[sommet] = true;

            for (int i = 0; i < matrice.GetLength(0); i++)
            {
                if (matrice[sommet, i] != 0)
                {
                    if (!visite[i])
                    {
                        if (RechercheCycle(i, matrice, visite, sommet))
                        {
                            return true;
                        }
                    }
                    else if (i != parent)
                    {
                        //le cas ou le sommet n'a pas été visité et n'est pas parent immédiat, on a un cycle.
                        return true;
                    }
                }
            }
            return false;
        }*/

        public static List<int> Dijkstra(int depart, int arrivee, int[,] matriceUsers)
        {
            int n = matriceUsers.GetLength(0);
            int[] distances = new int[n];
            int[] predecessors = new int[n];
            bool[] visited = new bool[n];

            for (int i = 0; i < n; i++)
            {
                distances[i] = int.MaxValue;
                predecessors[i] = -1;
                visited[i] = false;
            }

            distances[depart] = 0;

            for (int count = 0; count < n - 1; count++)
            {
                int u = MinDistance(distances, visited, n);
                if (u == -1) break;
                visited[u] = true;

                for (int v = 0; v < n; v++)
                {
                    if (!visited[v] && matriceUsers[u, v] != 0 && distances[u] != int.MaxValue
                        && distances[u] + matriceUsers[u, v] < distances[v])
                    {
                        distances[v] = distances[u] + matriceUsers[u, v];
                        predecessors[v] = u;
                    }
                }
            }

            return chemin(predecessors, arrivee);
        }

        private static int MinDistance(int[] distances, bool[] visited, int n)
        {
            int min = int.MaxValue, minIndex = -1;

            for (int v = 0; v < n; v++)
            {
                if (!visited[v] && distances[v] <= min)
                {
                    min = distances[v];
                    minIndex = v;
                }
            }
            return minIndex;
        }

        private static List<int> chemin(int[] predecessors, int arrivee)
        {
            List<int> path = new List<int>();
            for (int i = arrivee; i != -1; i = predecessors[i])
            {
                path.Insert(0, i);
            }
            return path;
        }
        static void Main(string[] args)
        {
            /*string chemin = "soc-karate.mtx";
            StreamReader fichier = new(chemin);
            int[,] matriceUsers = creationMatrice(chemin); //création de la matrice avec une fonction

            string ligne = fichier.ReadLine();
            //remplissage de la matrice :
            matriceUsers = RemplissageMatrice(matriceUsers, ligne, fichier);

            //affichage de la matrice :
            Console.WriteLine("Matrice d'adjacence : ");
            affichageMatrice(matriceUsers);

            Console.WriteLine("\n Liste d'adjacence : ");
            //création et affichage d'une liste d'adjacence
            Dictionary<int, List<int>> listeAdjacence = creationListeAdjacence(chemin);
            affichageListe(listeAdjacence);

            //création d'un graphe
            Graphe UnGraphe = new Graphe(matriceUsers);
            //UnGraphe.AfficherGraphe();

            Console.WriteLine("\n on effectue un parcours en profondeur :");
            int nbNoeudsprofondeur = parcoursProfondeur(matriceUsers, 0, true);
            Console.WriteLine("\n on effectue un parcours en largeur :");
            int nbNoeudslargeur = parcoursLargeur(matriceUsers, 0);

            //Console.WriteLine("création de l'image du graphe : ");
            //GenererImageGraphe(matriceUsers);

            //On vérifie si le graphe est connexe :
            Console.WriteLine("Le graphe est-il connexe ? " + estConnexe(matriceUsers));
            //On vérifie si le graphe contient des cycles :
            Console.WriteLine("Le graphe contient-il des cylces ? " + ContientCycle(matriceUsers));*/
            string PathWayToDatabase = "server=localhost;user=root;password=root;database=LivInParis;";
            using (MySqlConnection Connection = new MySqlConnection(PathWayToDatabase))
            {
                try
                {
                    Connection.Open();
                    Console.WriteLine("Database LivInParis connectée.");
                    System.Threading.Thread.Sleep(3000);
                    MainMenu(Connection);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
            }
            static void MainMenu(MySqlConnection Connection)
            {
                bool exit = false;
                while (!exit)
                {
                    Console.Clear();
                    Console.WriteLine("=== Menu Principal ===");
                    Console.WriteLine();
                    Console.WriteLine("1. Client");
                    Console.WriteLine("2. Cuisinier");
                    Console.WriteLine("3. Commandes");
                    Console.WriteLine("4. Statistiques");
                    Console.WriteLine("5. Autres");
                    Console.WriteLine("0. Quitter");
                    Console.Write("Choisissez une option : ");
                    string mainChoice = Console.ReadLine();

                    switch (mainChoice)
                    {
                        case "1":
                            ClientMenu(Connection);
                            break;
                        case "2":
                            CuisinierMenu(Connection);
                            break;
                        case "3":
                            CommandeMenu(Connection);
                            break;
                        case "4":
                            Stats(Connection);
                            break;
                        case "5":
                            AutresMenu(Connection);
                            break;
                        case "0":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Option invalide. Appuyez sur une touche pour réessayer...");
                            Console.ReadKey();
                            break;
                    }
                }
            }
            static void ClientMenu(MySqlConnection Connection)
            {
                bool back = false;
                while (!back)
                {
                    Console.Clear();
                    Console.WriteLine("=== Menu Client ===");
                    Console.WriteLine();
                    Console.WriteLine("1. Ajouter un client");
                    Console.WriteLine("2. Modifier un client");
                    Console.WriteLine("3. Supprimer un client");
                    Console.WriteLine("4. Afficher les informations client");
                    Console.WriteLine("0. Retour au menu principal");
                    Console.Write("Choisissez une option : ");
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            AddUser(Connection);
                            break;
                        case "2":
                            EditUser(Connection);
                            break;
                        case "3":
                            DeleteUser(Connection);
                            break;
                        case "4":
                            InformationsClient(Connection);
                            break;
                        case "0":
                            back = true;
                            break;
                        default:
                            Console.WriteLine("Option invalide.");
                            break;
                    }
                    if (!back)
                    {
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                    }
                }
            }
            static void AddUser(MySqlConnection Connection)
            {
                Console.Clear();
                Console.WriteLine("=== Ajouter un client ===");
                Console.WriteLine();
                Console.Write("Prénom : ");
                string prenom = Console.ReadLine();
                Console.Write("Nom : ");
                string nom = Console.ReadLine();
                Console.Write("Adresse : ");
                string adresse = Console.ReadLine();
                Console.Write("Code postal : ");
                string code_postal = Console.ReadLine();
                Console.Write("Email : ");
                string email = Console.ReadLine();
                Console.Write("Mot de passe : ");
                string mdp = Console.ReadLine();
                Console.Write("Est client (y/n) ? ");
                bool est_client = Console.ReadLine().Trim().ToLower() == "y";
                Console.Write("Est cuisinier (y/n) ? ");
                bool est_cuisinier = Console.ReadLine().Trim().ToLower() == "y";
                Console.Write("Particulier ou Entreprise ? (p/e) : ");
                string type_client_input = Console.ReadLine().Trim().ToLower();
                string type_client = type_client_input == "p" ? "Particulier" : type_client_input == "e" ? "Entreprise" : null;
                try
                {
                    string Instruction = "INSERT INTO Utilisateur (prenom, nom, adresse, code_postal, email, mdp, est_client, est_cuisinier, type_client) " + "VALUES (@prenom, @nom, @adresse, @codePostal, @email, @mdp, @est_client, @est_cuisinier, @type_client);";
                    MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                    Command.Parameters.AddWithValue("@prenom", prenom);
                    Command.Parameters.AddWithValue("@nom", nom);
                    Command.Parameters.AddWithValue("@adresse", adresse);
                    Command.Parameters.AddWithValue("@codePostal", code_postal);
                    Command.Parameters.AddWithValue("@email", email);
                    Command.Parameters.AddWithValue("@mdp", mdp);
                    Command.Parameters.AddWithValue("@est_client", est_client);
                    Command.Parameters.AddWithValue("@est_cuisinier", est_cuisinier);
                    Command.Parameters.AddWithValue("@type_client", type_client);

                    int rowsAffected = Command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Client ajouté avec succès !");
                    }
                    else
                    {
                        Console.WriteLine("Échec de l'ajout du client.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
            }
            static void EditUser(MySqlConnection Connection)
            {
                Console.Clear();
                Console.WriteLine("=== Modifier un client ===");
                Console.WriteLine();
                Console.Write("Entrez l'ID du client à modifier : ");
                if (int.TryParse(Console.ReadLine(), out int id_utilisateur))
                {
                    Console.Write("Nouveau prénom : ");
                    string prenom = Console.ReadLine();
                    Console.Write("Nouveau nom : ");
                    string nom = Console.ReadLine();
                    Console.Write("Nouvelle adresse : ");
                    string adresse = Console.ReadLine();
                    Console.Write("Nouveau code postal : ");
                    string code_postal = Console.ReadLine();
                    Console.Write("Nouvel email : ");
                    string email = Console.ReadLine();
                    Console.Write("Nouveau mot de passe : ");
                    string mdp = Console.ReadLine();
                    Console.Write("Est client (y/n) ? ");
                    bool est_client = Console.ReadLine().Trim().ToLower() == "y";
                    Console.Write("Est cuisinier (y/n) ? ");
                    bool est_cuisinier = Console.ReadLine().Trim().ToLower() == "y";
                    Console.Write("Particulier ou Entreprise ? (p/e) : ");
                    string type_client_input = Console.ReadLine().Trim().ToLower();
                    string type_client = type_client_input == "p" ? "Particulier" : type_client_input == "e" ? "Entreprise" : null;

                    string Instruction = "UPDATE Utilisateur SET prenom = @prenom, nom = @nom, adresse = @adresse, code_postal = @codePostal, email = @email, mdp = @mdp, est_client = @est_client, est_cuisinier = @est_cuisinier, type_client = @type_client WHERE id_utilisateur = @id;";
                    MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                    Command.Parameters.AddWithValue("@prenom", prenom);
                    Command.Parameters.AddWithValue("@nom", nom);
                    Command.Parameters.AddWithValue("@adresse", adresse);
                    Command.Parameters.AddWithValue("@codePostal", code_postal);
                    Command.Parameters.AddWithValue("@email", email);
                    Command.Parameters.AddWithValue("@mdp", mdp);
                    Command.Parameters.AddWithValue("@est_client", est_client);
                    Command.Parameters.AddWithValue("@est_cuisinier", est_cuisinier);
                    Command.Parameters.AddWithValue("@type_client", type_client);
                    Command.Parameters.AddWithValue("@id", id_utilisateur);

                    int rowsAffected = Command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        Console.WriteLine("Client modifié avec succès !");
                    else
                        Console.WriteLine("Aucun client trouvé avec cet ID.");
                }
                else
                {
                    Console.WriteLine("ID invalide.");
                }
            }
            static void DeleteUser(MySqlConnection Connection)
            {
                Console.Clear();
                Console.WriteLine("=== Supprimer un client ===");
                Console.WriteLine();
                Console.Write("Entrez l'ID du client à supprimer : ");

                if (int.TryParse(Console.ReadLine(), out int id_utilisateur))
                {
                    try
                    {
                        string deleteOrders = "DELETE FROM commande WHERE id_client = @id;";
                        MySqlCommand CommandDeleteOrders = new MySqlCommand(deleteOrders, Connection);
                        CommandDeleteOrders.Parameters.AddWithValue("@id", id_utilisateur);
                        CommandDeleteOrders.ExecuteNonQuery();

                        string deleteClient = "DELETE FROM Utilisateur WHERE id_utilisateur = @id;";
                        MySqlCommand CommandDeleteClient = new MySqlCommand(deleteClient, Connection);
                        CommandDeleteClient.Parameters.AddWithValue("@id", id_utilisateur);
                        int rowsAffected = CommandDeleteClient.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Client supprimé avec succès !");
                        }
                        else
                        {
                            Console.WriteLine("Aucun client trouvé avec cet ID.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erreur : " + ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("ID invalide.");
                }
            }
            static void InformationsClient(MySqlConnection Connection)
            {
                Console.Clear();
                Console.WriteLine("=== Liste des Clients par Nom, Prénom (ordre alphabétique) ===");
                Console.WriteLine();
                string Instruction = "SELECT id_utilisateur, prenom, nom, adresse, code_postal, email FROM Utilisateur WHERE est_client = TRUE ORDER BY nom ASC, prenom ASC;";
                MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                MySqlDataReader reader = Command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string prenom = reader.GetString(1);
                    string nom = reader.GetString(2);
                    string adresse = reader.GetString(3);
                    string codePostal = reader.GetString(4);
                    string email = reader.GetString(5);
                    Console.WriteLine($"ID: {id}, Nom: {nom}, Prénom: {prenom}, Adresse: {adresse}, Code Postal: {codePostal}, Email: {email}");
                }
                reader.Close();
                Console.WriteLine("Appuyez sur une touche pour continuer...");
                Console.ReadKey();
                Console.Clear();
                Console.WriteLine("=== Liste des Clients par Adresse (ordre alphabétique) ===");
                Console.WriteLine();
                Instruction = "SELECT id_utilisateur, prenom, nom, adresse, code_postal, email FROM Utilisateur WHERE est_client = TRUE ORDER BY adresse ASC;";
                Command = new MySqlCommand(Instruction, Connection);
                reader = Command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string prenom = reader.GetString(1);
                    string nom = reader.GetString(2);
                    string adresse = reader.GetString(3);
                    string codePostal = reader.GetString(4);
                    string email = reader.GetString(5);
                    Console.WriteLine($"ID: {id}, Nom: {nom}, Prénom: {prenom}, Adresse: {adresse}, Code Postal: {codePostal}, Email: {email}");
                }
                reader.Close();
                Console.WriteLine("Appuyez sur une touche pour continuer...");
                Console.ReadKey();
                Console.Clear();
                Console.WriteLine("=== Liste des Meilleurs Clients ===");
                Console.WriteLine();
                Instruction = "SELECT u.id_utilisateur, u.prenom, u.nom, COUNT(c.id_commande) AS nb_commandes, SUM(c.montant_total) AS total_achats FROM Utilisateur u JOIN Commande c ON u.id_utilisateur = c.id_client WHERE u.est_client = TRUE GROUP BY u.id_utilisateur, u.prenom, u.nom ORDER BY total_achats DESC;";
                Command = new MySqlCommand(Instruction, Connection);
                reader = Command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string prenom = reader.GetString(1);
                    string nom = reader.GetString(2);
                    int NbOrders = reader.GetInt32(3);
                    decimal TotalOrders = reader.GetDecimal(4);
                    Console.WriteLine($"ID: {id}, Nom: {nom}, Prénom: {prenom}, Nombre de commandes: {NbOrders}, Total: {TotalOrders}EUR");
                }
                reader.Close();
            }
            static void CuisinierMenu(MySqlConnection Connection)
            {
                bool back = false;
                while (!back)
                {
                    Console.Clear();
                    Console.WriteLine("=== Menu Cuisinier ===");
                    Console.WriteLine();
                    Console.WriteLine("1. Ajouter un cuisinier");
                    Console.WriteLine("2. Modifier un cuisinier");
                    Console.WriteLine("3. Supprimer un cuisinier");
                    Console.WriteLine("4. Afficher les informations cuisinier");
                    Console.WriteLine("0. Retour au menu principal");
                    Console.Write("Choisissez une option : ");
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            AddUser(Connection);
                            break;
                        case "2":
                            EditUser(Connection);
                            break;
                        case "3":
                            DeleteUser(Connection);
                            break;
                        case "4":
                            InformationsCuisinier(Connection);
                            break;
                        case "0":
                            back = true;
                            break;
                        default:
                            Console.WriteLine("Option invalide.");
                            break;
                    }
                    if (!back)
                    {
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                    }
                }
            }
            static void InformationsCuisinier(MySqlConnection Connection)
            {
                Console.Clear();
                Console.Write("Entrez l'ID d'un cuisinier pour obtenir ses informations : ");
                if (int.TryParse(Console.ReadLine(), out int id_cuisinier))
                {
                    try
                    {
                        string InspectCook = "SELECT DISTINCT u.id_utilisateur, u.prenom, u.nom, u.email FROM Utilisateur u JOIN Commande c ON u.id_utilisateur = c.id_client JOIN LigneCommande lc ON c.id_commande = lc.id_commande JOIN Plat p ON lc.id_plat = p.id_plat WHERE p.id_cuisinier = @id_cuisinier;";
                        MySqlCommand CommandInspectCook = new MySqlCommand(InspectCook, Connection);
                        CommandInspectCook.Parameters.AddWithValue("@id_cuisinier", id_cuisinier);
                        MySqlDataReader reader = CommandInspectCook.ExecuteReader();
                        bool HasReasults = false;
                        Console.WriteLine("=== Liste des Clients servi par ce Cuisinier ===");
                        Console.WriteLine();
                        while (reader.Read())
                        {
                            HasReasults = true;
                            Console.WriteLine($"ID: {reader["id_utilisateur"]}, Prénom: {reader["prenom"]}, Nom: {reader["nom"]}, Email: {reader["email"]}");
                        }
                        reader.Close();
                        if (!HasReasults)
                        {
                            Console.WriteLine("Aucun client servi par ce cuisinier.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erreur : " + ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("ID invalide.");
                }
                Console.WriteLine("Appuyez sur une touche pour continuer...");
                Console.ReadKey();
                Console.Clear();
                try
                {
                    string Instruction = "SELECT nom_plat, date_fabrication FROM Plat WHERE id_cuisinier = @id_cuisinier ORDER BY date_fabrication DESC;";
                    MySqlCommand ExecuteInstruction = new MySqlCommand(Instruction, Connection);
                    ExecuteInstruction.Parameters.AddWithValue("@id_cuisinier", id_cuisinier);
                    MySqlDataReader reader = ExecuteInstruction.ExecuteReader();
                    bool HasReasults = false;
                    Console.WriteLine("=== Liste des Plats préparés par ce Cuisinier ===");
                    Console.WriteLine();
                    while (reader.Read())
                    {
                        HasReasults = true;
                        Console.WriteLine($"Nom du plat: {reader["nom_plat"]}, Date de préparation: {reader["date_fabrication"]}");
                    }
                    reader.Close();
                    if (!HasReasults)
                    {
                        Console.WriteLine("Aucun plat préparé par ce cuisinier.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
                Console.WriteLine("Appuyez sur une touche pour continuer...");
                Console.ReadKey();
                Console.Clear();
                try
                {
                    string Instruction = "SELECT nom_plat, date_fabrication FROM Plat WHERE id_cuisinier = @id_cuisinier ORDER BY date_fabrication DESC LIMIT 1;";
                    MySqlCommand ExecuteInstruction = new MySqlCommand(Instruction, Connection);
                    ExecuteInstruction.Parameters.AddWithValue("@id_cuisinier", id_cuisinier);
                    MySqlDataReader reader = ExecuteInstruction.ExecuteReader();
                    bool HasReasults = false;
                    Console.WriteLine("=== Plat du jour proposé par ce cuisinier ===");
                    Console.WriteLine();
                    while (reader.Read())
                    {
                        HasReasults = true;
                        Console.WriteLine($"Nom du plat: {reader["nom_plat"]}, Date de préparation: {reader["date_fabrication"]}");
                    }
                    reader.Close();
                    if (!HasReasults)
                    {
                        Console.WriteLine("Aucun plat préparé par ce cuisinier.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
            }
            static void Stats(MySqlConnection Connection)
            {
                Console.Clear();
                Console.WriteLine("=== Nombre de livraisons par cuisinier ===");
                Console.WriteLine();
                try
                {
                    string Instruction = "SELECT u.id_utilisateur AS id_cuisinier, u.prenom, u.nom, COUNT(DISTINCT c.id_commande) AS nb_livraisons FROM Utilisateur u JOIN Plat p ON u.id_utilisateur = p.id_cuisinier JOIN LigneCommande lc ON p.id_plat = lc.id_plat JOIN Commande c ON lc.id_commande = c.id_commande WHERE u.est_cuisinier = TRUE GROUP BY u.id_utilisateur, u.prenom, u.nom ORDER BY nb_livraisons DESC;";
                    MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                    MySqlDataReader reader = Command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader["id_cuisinier"]}, Prénom: {reader["prenom"]}, Nom: {reader["nom"]}, Livraisons: {reader["nb_livraisons"]}");
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
                Console.WriteLine("Appuyez sur une touche pour continuer...");
                Console.ReadKey();
                Console.Clear();

                Console.WriteLine("=== Commandes du mois en cours ===");
                Console.WriteLine();
                try
                {
                    string Instruction = "SELECT id_commande, date_commande, montant_total FROM Commande WHERE MONTH(date_commande) = MONTH(CURRENT_DATE()) AND YEAR(date_commande) = YEAR(CURRENT_DATE()) ORDER BY date_commande DESC;";
                    MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                    MySqlDataReader reader = Command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine($"Commande ID: {reader["id_commande"]}, Date: {reader["date_commande"]}, Montant: {reader["montant_total"]}EUR");
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
                Console.WriteLine("Appuyez sur une touche pour continuer...");
                Console.ReadKey();
                Console.Clear();

                Console.WriteLine("=== Prix moyen d'une commande ===");
                Console.WriteLine();
                try
                {
                    string Instruction = "SELECT AVG(montant_total) AS moyenne FROM Commande;";
                    MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                    MySqlDataReader reader = Command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine($"Prix moyen d'une commande: {reader["moyenne"]}EUR");
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
                Console.WriteLine("Appuyez sur une touche pour continuer...");
                Console.ReadKey();
                Console.Clear();

                Console.WriteLine("=== Total moyen dépensé par client ===");
                Console.WriteLine();
                try
                {
                    string Instruction = "SELECT AVG(total) AS total_moyen FROM (SELECT id_client, SUM(montant_total) AS total FROM Commande GROUP BY id_client) AS calcul_preli;";
                    MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                    MySqlDataReader reader = Command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine($"Total moyen dépensé par client depuis son inscription: {reader["total_moyen"]}EUR");
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
                Console.WriteLine("Appuyez sur une touche pour continuer...");
                Console.ReadKey();
                Console.Clear();

                Console.Write("Entrez l'ID d'un client pour obtenir les informations sur ses commandes : ");
                if (int.TryParse(Console.ReadLine(), out int id_client))
                {
                    try
                    {
                        string Instruction = "SELECT c.id_commande, c.date_commande, p.nationalite FROM Commande c JOIN LigneCommande lc ON c.id_commande = lc.id_commande JOIN Plat p ON lc.id_plat = p.id_plat WHERE c.id_client = @id_client GROUP BY c.id_commande, p.nationalite ORDER BY c.date_commande DESC;";
                        MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                        Command.Parameters.AddWithValue("@id_client", id_client);
                        MySqlDataReader reader = Command.ExecuteReader();
                        bool HasReasults = false;
                        Console.WriteLine("=== Commandes du Client selon la nationnalité des plats ===");
                        Console.WriteLine();
                        while (reader.Read())
                        {
                            HasReasults = true;
                            Console.WriteLine($"Commande {reader["id_commande"]}, Date: {reader["date_commande"]}, Nationalité: {reader["nationalite"]}");
                        }
                        reader.Close();
                        if (!HasReasults)
                        {
                            Console.WriteLine("Aucune commandes effectuées par ce client.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erreur : " + ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("ID invalide.");
                }
                Console.WriteLine("Appuyez sur une touche pour continuer...");
                Console.ReadKey();
                Console.Clear();
            }
            static void AutresMenu(MySqlConnection Connection)
            {
                bool back = false;
                while (!back)
                {
                    Console.Clear();
                    Console.WriteLine("=== Menu Autres ===");
                    Console.WriteLine();
                    Console.WriteLine("1. Afficher la carte végétarienne");
                    Console.WriteLine("2. Afficher la plus grosse commande");
                    Console.WriteLine("3. Afficher nos dix clients les plus fidèles");
                    Console.WriteLine("4. Afficher nos dix plats les plus populaires");
                    Console.WriteLine("5. Afficher le chiffre d'affaires par cuisinier");
                    Console.WriteLine("0. Retour au menu principal");
                    Console.Write("Choisissez une option : ");
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            Vegetarien(Connection);
                            break;
                        case "2":
                            BigCommande(Connection);
                            break;
                        case "3":
                            MerciAuClientFidele(Connection);
                            break;
                        case "4":
                            BestPlats(Connection);
                            break;
                        case "5":
                            CACuisiniers(Connection);
                            break;
                        case "0":
                            back = true;
                            break;
                        default:
                            Console.WriteLine("Option invalide.");
                            break;
                    }
                    if (!back)
                    {
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                    }
                }
            }
            static void Vegetarien(MySqlConnection Connection)
            {
                Console.Clear();
                Console.WriteLine("=== Carte Végétarienne ===");
                Console.WriteLine();
                try
                {
                    string Instruction = "SELECT nom_plat, prix_par_personne, nationalite, regime_alimentaire FROM Plat WHERE regime_alimentaire LIKE '%Végétarien%';";
                    MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                    MySqlDataReader reader = Command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine($"Nom du plat: {reader["nom_plat"]}, Prix: {reader["prix_par_personne"]}, Nationalité: {reader["nationalite"]}, Caractéristiques: {reader["regime_alimentaire"]}");
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
            }
            static void BigCommande(MySqlConnection Connection)
            {
                Console.Clear();
                Console.WriteLine("=== Notre plus grosse commande ===");
                Console.WriteLine();
                try
                {
                    string Instruction = "SELECT c.montant_total, c.date_commande, u.prenom, u.nom FROM Commande c JOIN Utilisateur u ON c.id_client = u.id_utilisateur ORDER BY c.montant_total DESC LIMIT 1;";
                    MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                    MySqlDataReader reader = Command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine($"Prénom: {reader["prenom"]}, Nom: {reader["nom"]}, Date: {reader["date_commande"]}, Montant: {reader["montant_total"]}EUR");
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
            }
            static void MerciAuClientFidele(MySqlConnection Connection)
            {
                Console.Clear();
                Console.WriteLine("=== Classement des Clients fidèles ===");
                Console.WriteLine();
                try
                {
                    string Instruction = "SELECT u.id_utilisateur, u.prenom, u.nom, COUNT(c.id_commande) AS nb_commandes FROM Utilisateur u JOIN Commande c ON u.id_utilisateur = c.id_client GROUP BY u.id_utilisateur ORDER BY nb_commandes DESC LIMIT 10;";
                    MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                    MySqlDataReader reader = Command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader["id_utilisateur"]}, Prénom: {reader["prenom"]}, Nom: {reader["nom"]}, Nombre de commandes: {reader["nb_commandes"]}");
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
            }
            static void BestPlats(MySqlConnection Connection)
            {
                Console.Clear();
                Console.WriteLine("=== Nos Plats les plus populaires ===");
                Console.WriteLine();
                try
                {
                    string Instruction = "SELECT p.nom_plat, COUNT(lc.id_plat) AS nb_commandes FROM LigneCommande lc JOIN Plat p ON lc.id_plat = p.id_plat GROUP BY p.nom_plat ORDER BY nb_commandes DESC LIMIT 10;";
                    MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                    MySqlDataReader reader = Command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine($"Nom du plat: {reader["nom_plat"]}, Nombre de commandes: {reader["nb_commandes"]}");
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
            }
            static void CACuisiniers(MySqlConnection Connection)
            {
                Console.Clear();
                Console.WriteLine("=== Chiffre d'affaires des Cuisiniers ===");
                Console.WriteLine();
                try
                {
                    string Instruction = "SELECT u.prenom, u.nom, SUM(c.montant_total) AS ca FROM Commande c JOIN LigneCommande lc ON c.id_commande = lc.id_commande JOIN Plat p ON lc.id_plat = p.id_plat JOIN Utilisateur u ON p.id_cuisinier = u.id_utilisateur GROUP BY p.id_cuisinier ORDER BY ca DESC;";
                    MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                    MySqlDataReader reader = Command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine($"Prénom: {reader["prenom"]}, Nom: {reader["nom"]}, Chiffre d'affaires: {reader["ca"]}EUR");
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
            }
            static void CommandeMenu(MySqlConnection Connection)
            {
                bool back = false;
                while (!back)
                {
                    Console.Clear();
                    Console.WriteLine("=== Menu Commmande ===");
                    Console.WriteLine();
                    Console.WriteLine("1. Créer une commande");
                    Console.WriteLine("2. Modifier une commande");
                    Console.WriteLine("3. Afficher une commande");
                    Console.WriteLine("0. Retour au menu principal");
                    Console.Write("Choisissez une option : ");
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            AddCommande(Connection);
                            break;
                        case "2":
                            BigCommande(Connection);
                            break;
                        case "3":
                            MerciAuClientFidele(Connection);
                            break;
                        case "0":
                            back = true;
                            break;
                        default:
                            Console.WriteLine("Option invalide.");
                            break;
                    }
                    if (!back)
                    {
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                    }
                }
            }
            static void AddCommande(MySqlConnection Connection)
            {
                Console.Clear();
                Console.WriteLine("=== Ajouter une commande ===");
                Console.Write("Entrez l'ID du client : ");
                if (!int.TryParse(Console.ReadLine(), out int id_client))
                {
                    Console.WriteLine("ID invalide.");
                    return;
                }
                string Instruction = "SELECT est_client FROM Utilisateur WHERE id_utilisateur = @id_client;";
                MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                Command.Parameters.AddWithValue("@id_client", id_client);
                bool isClient = false;
                using (MySqlDataReader reader = Command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        isClient = reader.GetBoolean("est_client");
                    }
                }
                if (!isClient)
                {
                    Console.WriteLine("L'ID spécifié ne correspond pas à un client existant.");
                    Console.WriteLine("Veuillez créer un nouvel utilisateur client.");
                    return;
                }
                List<(int platId, int quantity)> OrderLines = new List<(int, int)>();
                decimal MontantTotal = 0m;

                while (1 == 1)//Oui c'est pas très propre et ça fait un peu bricolage mais je ne voyais pas comment faire sinon vu qu'avec un booléen si j'utilise pas le break le programme va continuer et essayer d'ajouter un plat ID0 qui n'existe pas.
                {
                    Console.Write("Entrez l'ID du plat à ajouter (0 pour terminer) : ");
                    if (!int.TryParse(Console.ReadLine(), out int id_plat))
                    {
                        Console.WriteLine("ID de plat invalide.");
                        continue;
                    }
                    if (id_plat == 0)
                    {
                        break;
                    }
                    string InstructionPlat = "SELECT nom_plat, prix_par_personne FROM Plat WHERE id_plat = @id_plat;";
                    MySqlCommand CommandPlat = new MySqlCommand(InstructionPlat, Connection);
                    CommandPlat.Parameters.AddWithValue("@id_plat", id_plat);
                    using (MySqlDataReader platReader = CommandPlat.ExecuteReader())
                    {
                        if (platReader.Read())
                        {
                            string NomPlat = platReader.GetString("nom_plat");
                            decimal prix = platReader.GetDecimal("prix_par_personne");
                            Console.WriteLine($"Plat sélectionné: {NomPlat}, Prix: {prix}EUR");
                            Console.Write("Entrez la quantité: ");
                            if (!int.TryParse(Console.ReadLine(), out int quantite) || quantite <= 0)
                            {
                                Console.WriteLine("Quantité invalide. Ce plat ne sera pas ajouté.");
                                continue;//Sinon ça plante donc je reviens directement au début du while en ignorant le code qui suit.
                            }
                            Console.WriteLine($"Vous avez sélectionné {quantite}X {NomPlat}.");
                            OrderLines.Add((id_plat, quantite));
                            MontantTotal += prix * quantite;
                        }
                        else
                        {
                            Console.WriteLine("Aucun plat trouvé avec cet ID.");
                        }
                    }
                }
                if (OrderLines.Count == 0)
                {
                    Console.WriteLine("Aucun plat ajouté, commande annulée.");
                    return;
                }
                DateTime date_commande = DateTime.Now;
                string InstructionOrder = "INSERT INTO Commande (id_client, date_commande, montant_total) VALUES (@id_client, @date_commande, @montant_total);";
                MySqlCommand CommandOrder = new MySqlCommand(InstructionOrder, Connection);
                CommandOrder.Parameters.AddWithValue("@id_client", id_client);
                CommandOrder.Parameters.AddWithValue("@date_commande", date_commande);
                CommandOrder.Parameters.AddWithValue("@montant_total", MontantTotal);
                try
                {
                    int rowsAffected = CommandOrder.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        long NewOrderID = CommandOrder.LastInsertedId;
                        Console.WriteLine($"Commande ajoutée avec succès ! ID commande : {NewOrderID}, Montant: {MontantTotal}EUR");
                        foreach (var line in OrderLines)
                        {
                            string CommandLigne = "INSERT INTO LigneCommande (id_commande, id_plat, quantite) VALUES (@id_commande, @id_plat, @quantite);";
                            MySqlCommand InstructionLigne = new MySqlCommand(CommandLigne, Connection);
                            InstructionLigne.Parameters.AddWithValue("@id_commande", NewOrderID);
                            InstructionLigne.Parameters.AddWithValue("@id_plat", line.platId);
                            InstructionLigne.Parameters.AddWithValue("@quantite", line.quantity);
                            InstructionLigne.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Erreur lors de l'insertion des ligne de la commande.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
            }
        }
    }
}