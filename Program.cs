using KarateGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Graphviz4Net.Graphs;
using System.Diagnostics;



namespace KarateGraphe
{
    public class Program
    {
        public static int[,] creationMatrice(string chemin)
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
        public static int[,] remplissageMatrice(int[,] matrice, string ligne, StreamReader fichier)
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
        static void GenererImageGraphe(int[,] matrice)
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
                if (!visite[i] && rechercheCycle(i, matrice, visite, -1)) // Le -1 représente l'absence de parent pour le sommet initial
                {
                    return true;
                }
            }
            return false;
        }

        private static bool rechercheCycle(int sommet, int[,] matrice, bool[] visite, int parent)
        {
            visite[sommet] = true;

            for (int i = 0; i < matrice.GetLength(0); i++)
            {
                if (matrice[sommet, i] != 0)
                {
                    if (!visite[i])
                    {
                        if (rechercheCycle(i, matrice, visite, sommet))
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
        }


        static void Main(string[] args)
        {
            string chemin = "soc-karate.mtx";
            StreamReader fichier = new(chemin);
            int[,] matriceUsers = creationMatrice(chemin); //création de la matrice avec une fonction


            string ligne = fichier.ReadLine();
            //remplissage de la matrice :
            matriceUsers = remplissageMatrice(matriceUsers, ligne, fichier);

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

            Console.WriteLine("création de l'image du graphe : ");
            GenererImageGraphe(matriceUsers);

            //On vérifie si le graphe est connexe :
            Console.WriteLine("la matrice est elle connexe ? " + estConnexe(matriceUsers));

            //On vérifie si le graphe contient des cycles :
            Console.WriteLine("la matrice contient des cylces ? " + ContientCycle(matriceUsers));
        }
    }
}
