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
using System.ComponentModel;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace ADUFORET_TDUCOURAU_JESPINOS_LivInParis
{
    public class Program
    {
        public static int[,] creationMatriceMtx(string chemin)
        {
            /// <summary>
            ///création d'une matrice d'adjacence à partir d'un fichier de type .mtx
            /// </summary>
            
            int[,]? matriceUsers = null;
            StreamReader lecteur = new(chemin);
            string? ligne = lecteur.ReadLine();
            string[] header = ligne.Split(" "); 

            if (header[2] == "coordinate") 
            {
                Console.WriteLine("format = coordinate");
                while ((ligne = lecteur.ReadLine()) != null)
                {
                    if (ligne != null && ligne[0] != '%' && ligne.Length > 0)
                    {
                        string[] tabline = ligne.Split(' '); 
                        if (tabline.Length == 3) 
                        {
                            Console.WriteLine("Size Line = " + ligne);
                            matriceUsers = new int[Convert.ToInt32(tabline[0]), Convert.ToInt32(tabline[1])]; 
                        }
                    }
                }
            }
            return matriceUsers;
        }
        public static int[,] RemplissageMatrice(int[,] matrice, string? ligne, StreamReader fichier)
        {
            /// <summary>
            ///permet de remplir la matrice à partir du fichier mtx
            /// <\summary>
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
        public static int[,] creationMatriceCSV(Graphe graphe)
        {
            /// <summary>
            ///créer une matrice d'adjacence à partir d'un objet Graphe
            /// <\summary>
            
            /// Récupérer les noeuds valides (ID != 0)
            var noeudsValid = graphe.AllNodes
            .Where(n => n.ID != 0)
            .OrderBy(n => n.ID)
            .ToList();

            int n = noeudsValid.Count;
            int[,] matrice = new int[n, n];

            ///Création d'un dictionnaire pour accéder rapidement à l'indice de chaque noeud par son ID
            Dictionary<int, int> idToIndex = new();
            for (int i = 0; i < n; i++)
            {
                idToIndex[noeudsValid[i].ID] = i;
            }

            /// Remplissage de la matrice
            foreach (var lien in graphe.AllLinks)
            {
                int a = lien.startingNode;
                int b = lien.endingNode;

                /// Exclure les liens impliquant le noeud 0
                if (a == 0 || b == 0) continue;

                if (idToIndex.ContainsKey(a) && idToIndex.ContainsKey(b))
                {
                    int i = idToIndex[a];
                    int j = idToIndex[b];
                    matrice[i, j] = lien.tripValue;
                    matrice[j, i] = lien.tripValue; 
                }
            }

            return matrice;
        }
        public static int[,] creationMatriceJson(string cheminFichier)
        {
            /// <summary>
            /// créer la matrice d'adjacence d'un graphe à partir d'un fichier Json
            /// <\summary>
            string json = File.ReadAllText(cheminFichier);
            JObject root = JObject.Parse(json);
            var utilisateurs = root["utilisateur"];
            var commandes = root["commande"];
            var ligneCommandes = root["lignecommande"];

            Dictionary<int, int> idVersIndex = new();
            int index = 0;

            /// Créer un dictionnaire d'index des utilisateurs
            foreach (var utilisateur in utilisateurs)
            {
                int id = (int)utilisateur["id_utilisateur"];
                if (!idVersIndex.ContainsKey(id))
                {
                    idVersIndex[id] = index++;
                }
            }

            int n = idVersIndex.Count;
            int[,] matrice = new int[n, n];

            /// Associer les commandes aux plats et aux utilisateurs
            foreach (var ligneCommande in ligneCommandes)
            {
                int idCommande = (int)ligneCommande["id_commande"];
                int idPlat = (int)ligneCommande["id_plat"];
                int quantite = (int)ligneCommande["quantite"];

                /// Trouver les id_client et id_cuisinier associés au plat
                var plat = root["plat"].FirstOrDefault(p => (int)p["id_plat"] == idPlat);
                if (plat != null)
                {
                    int cuisinierId = (int)plat["id_cuisinier"];
                    var commande = commandes.FirstOrDefault(c => (int)c["id_commande"] == idCommande);
                    if (commande != null)
                    {
                        int clientId = (int)commande["id_client"];

                        /// Si les utilisateurs existent dans le dictionnaire, mettre à jour la matrice
                        if (idVersIndex.ContainsKey(clientId) && idVersIndex.ContainsKey(cuisinierId))
                        {
                            int i = idVersIndex[clientId];
                            int j = idVersIndex[cuisinierId];
                            matrice[i, j] += quantite;
                        }
                    }
                }
            }

            return matrice;
        }
        public static Dictionary<int, List<int>> creationListeAdjacence(string chemin)
        {
            /// <summary>
            /// créer la liste d'adjacence d'un graphe depuis un fichier mtx
            /// <\summary>
            var adjacencyList = new Dictionary<int, List<int>>();
            int noeuds = 0;

            StreamReader lecteur = new(chemin);
            string? ligne = lecteur.ReadLine();
            string[] header = ligne.Split(" "); ///le header est la première ligne du fichier .mtx

            if (header[2] == "coordinate") /// on vérfie que le format est coordinate
            {
                while ((ligne = lecteur.ReadLine()) != null)
                {
                    if (ligne != null && ligne[0] != '%' && ligne.Length > 0)///chaque comment lines commence par un % donc on ne passe pas dessus
                    {
                        string[] tabline = ligne.Split(' '); /// on convertit chaque ligne dans un tableau pour avoir les données séparées
                        if (tabline.Length == 3) /// la ligne size ligne est de forme m n nonzeros avec m le nombre de ligne et n le nombre de colonnes
                        {
                            noeuds = int.Parse(tabline[0]); /// le nombre de noeuds
                            break;
                        }
                    }
                }
            }
            ///Initialisation de la liste d'adjacence
            for (int i = 1; i <= noeuds; i++)
            {
                adjacencyList[i] = new List<int>();
            }

            while ((ligne = lecteur.ReadLine()) != null)
            {
                if (ligne.Length > 0 && ligne[0] != '%') /// Encore pour ignorer les commentaires
                {
                    string[] tabline = ligne.Split(' ');
                    if (tabline.Length >= 2) /// Vérifier qu'on a bien des données
                    {
                        int u = int.Parse(tabline[0]);
                        int v = int.Parse(tabline[1]);

                        adjacencyList[u].Add(v);
                        adjacencyList[v].Add(u); /// Graphe non orienté
                    }
                }
            }
            return adjacencyList;
        }
        public static void affichageMatrice(int[,] matrice)
        {
            /// <summary>
            ///affichage de la matrice
            /// <\summary>
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
            /// <summary>
            /// affichage d'une liste
            /// <\summary>
            foreach (var node in listeAdjacence)
            {
                Console.Write($"{node.Key}: ");
                Console.WriteLine(string.Join(", ", node.Value));
            }

        }
        public static int parcoursProfondeur(int[,] matrice, int depart, bool affichage)
        {
            /// <summary>
            /// parcours en profondeur d'un graphe depuis une matrice d'adjacence
            /// <\summary>
            int n = matrice.GetLength(0);
            bool[] visite = new bool[n];
            Pile pile = new Pile(); /// j'ai créer une classe pile avant de me rendre compte que Stack<T> existe

            int result = DFS(depart, matrice, visite, pile, 0, affichage);
            return result;
        }
        private static int DFS(int sommet, int[,] matrice, bool[] visite, Pile pile, int c, bool affichage)
        {
            visite[sommet] = true;

            if (affichage)
            {
                Console.WriteLine(sommet);
            }

            c++;

            for (int i = 0; i < matrice.GetLength(0); i++)
            {
                if (matrice[sommet, i] == 1 && !visite[i])
                {
                    pile.add(i);
                    c = DFS(i, matrice, visite, pile, c, affichage); /// appel récursif (on incrémente le compteur, il va servir pour estConnexe
                    pile.remove();
                }
            }
            return c;
        }
        public static int parcoursLargeur(int[,] matrice, int depart)
        {
            /// <summary>
            /// parcours en largeur à partir d'une matrice d'adjacence
            /// <\summary>
            int n = matrice.GetLength(0);
            int c = 0;
            bool[] visite = new bool[n]; /// tableau des noeuds déjà visités
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
                    if (matrice[sommet, i] != 0 && !visite[i])
                    {
                        visite[i] = true;
                        file.Enqueue(i);
                    }
                }
            }
            return c;
        }
        public static void GenererImageGraphe(Graphe graphe)
        {
            /// <summary>
            /// génération d'une image png à partir d'un objet de la classe Graphe
            /// <\summary>
            StringBuilder dot = new StringBuilder();
            dot.AppendLine("graph G {");
            dot.AppendLine("    node [shape=circle, style=filled, fillcolor=white, fontname=\"Arial\"];");

            foreach (var node in graphe.AllNodes)
            {
                if (node.ID == 0) continue; /// Ignore le noeud 0

                dot.AppendLine($"    {node.ID} [label=\"{node.ID}\"];");
                dot.AppendLine($"    label_{node.ID} [shape=none, label=\"{node.libelleStation}\", fontsize=10];");
                dot.AppendLine($"    {node.ID} -- label_{node.ID} [style=invis];");
            }
            Dictionary<int, string> couleurParLigne = new();
            string[] palette = { "red", "blue", "green", "orange", "purple", "brown", "cyan", "magenta", "gray", "pink" };
            int couleurIndex = 0;

            HashSet<string> liensTraites = new HashSet<string>();

            foreach (var lien in graphe.AllLinks)
            {
                int a = lien.startingNode;
                int b = lien.endingNode;

                if (a == 0 || b == 0) continue;

                string key = a < b ? $"{a}-{b}" : $"{b}-{a}";

                if (!liensTraites.Contains(key))
                {
                    var noeud = graphe.AllNodes.FirstOrDefault(n => n.ID == a);
                    if (noeud == null)
                    {
                        Console.WriteLine($"Noeud avec ID {a} introuvable pour le lien {a}-{b}, lien ignoré.");
                        continue;
                    }

                    if (noeud.libelleLigne.Count == 0)
                    {
                        Console.WriteLine($"Noeud {a} n'a pas de ligne associée, lien ignoré.");
                        continue;
                    }

                    int ligne = noeud.libelleLigne.First();
                    
                    if (!couleurParLigne.ContainsKey(ligne))
                    {
                        couleurParLigne[ligne] = palette[couleurIndex % palette.Length];
                        couleurIndex++;
                    }

                    string couleur = couleurParLigne[ligne];

                    dot.AppendLine($"    {a} -- {b} [label=\"{lien.tripValue}\", color=\"{couleur}\", fontcolor=\"{couleur}\"];");
                    liensTraites.Add(key);
                }
            }

            dot.AppendLine("}");

            ///Écriture fichier .dot
            string dotPath = "graphe.dot";
            File.WriteAllText(dotPath, dot.ToString());

            ///Génération image PNG avec Graphviz
            string imagePath = "graphe.png";
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "dot",
                Arguments = $"-Tpng {dotPath} -o {imagePath}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    Console.WriteLine("Image générée : " + imagePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }
        public static bool estConnexe(int[,] matrice)
        {
            /// <summary>
            /// permet de vérifier si un graphe est connexe
            /// prend une matrice d'adjacence en paramètre
            /// <\summary>
            bool resultat = true;

            for (int i = 0; i < matrice.GetLength(0); i++)
            {
                if (parcoursProfondeur(matrice, i, false) != matrice.GetLength(0)) ///on fait un dfs depuis chaque noeud et on vérifie qu'il passe par tous les noeuds
                {
                    resultat = false;
                }
            }

            return resultat;
        }
        public static bool ContientCycle(int[,] matrice)
        {
            /// <summary>
            /// permet de vérifier si le graphe contient des cycles
            /// <\summary>
            int n = matrice.GetLength(0);
            bool[] visite = new bool[n];

            /// On parcourt tous les sommets pour vérifier s'il y a des cycles
            for (int i = 0; i < n; i++)
            {
                if (!visite[i] && RechercheCycle(i, matrice, visite, -1)) /// Le -1 représente l'absence de parent pour le sommet initial
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
                        ///le cas ou le sommet n'a pas été visité et n'est pas parent immédiat, on a un cycle.
                        return true;
                    }
                }
            }
            return false;
        }
        public static List<int> Dijkstra(int depart, int arrivee, Graphe graphe)
        {
            /// <summary>
            /// algorithme du plus court chemin Dikstra
            /// <\summary>
            var distances = new Dictionary<int, int>();
            var precedent = new Dictionary<int, int>();
            var nonVisites = new HashSet<int>();

            foreach (var noeud in graphe.AllNodes.Where(n => n.ID != 0))
            {
                distances[noeud.ID] = int.MaxValue;
                precedent[noeud.ID] = -1;
                nonVisites.Add(noeud.ID);
            }
            distances[depart] = 0;

            while (nonVisites.Count > 0)
            {
                int courant = nonVisites.OrderBy(n => distances[n]).First();

                nonVisites.Remove(courant);

                foreach (var lien in graphe.AllLinks)
                {
                    if (lien.stationId == 0) continue;

                    var voisins = new List<int>();

                    if (lien.startingNode == courant && lien.stationId != 0)
                        voisins.Add(lien.stationId);
                    if (lien.endingNode == courant && lien.stationId != 0)
                        voisins.Add(lien.stationId);
                    if (lien.stationId == courant)
                    {
                        if (lien.startingNode != 0)
                            voisins.Add(lien.startingNode);
                        if (lien.endingNode != 0)
                            voisins.Add(lien.endingNode);
                    }

                    foreach (int voisin in voisins)
                    {
                        if (!nonVisites.Contains(voisin)) continue;

                        int alt = distances[courant] + lien.tripValue;
                        if (alt < distances[voisin])
                        {
                            distances[voisin] = alt;
                            precedent[voisin] = courant;
                        }
                    }
                }
            }

            var chemin = new List<int>();
            int node = arrivee;

            while (node != -1)
            {
                chemin.Insert(0, node);
                node = precedent.ContainsKey(node) ? precedent[node] : -1;
            }

            if (chemin.First() != depart)
                return new List<int>();

            return chemin;
        }
        public static List<int> BellmanFord(int depart, int arrivee, int[,] matriceUsers)
        {
            int taille = matriceUsers.GetLength(0);
            List<int> result = new List<int>();
            int[] dist = new int[taille];
            int[] precedent = new int[taille];
            for (int i = 0; i < taille; i++)
            {
                dist[i] = int.MaxValue;
                precedent[i] = -1;
            }
            dist[depart] = 0;

            for (int i = 0; i < taille - 1; i++)
            {
                for (int ii = 0; ii < taille; ii++)
                {
                    for (int iii = 0; iii < taille; iii++)
                    {
                        if (matriceUsers[ii, iii] != 0 && dist[ii] != int.MaxValue && dist[ii] + matriceUsers[ii, iii] < dist[iii])
                        {
                            dist[iii] = dist[ii] + matriceUsers[ii, iii];
                            precedent[iii] = ii;
                        }
                    }
                }
            }

            for (int i = 0; i < taille; i++)
            {
                for (int ii = 0; ii < taille; ii++)
                {
                    if (matriceUsers[i, ii] != 0 && dist[i] != int.MaxValue && dist[i] + matriceUsers[i, ii] < dist[ii])
                    {
                        throw new Exception("Le graphe contient un cycle de poids négatif.");
                    }
                }
            }

            if (dist[arrivee] == int.MaxValue)
                return result;

            for (int i = arrivee; i != -1; i = precedent[i])
            {
                result.Insert(0, i);
            }

            return result;
        }
        public static List<int> FloydWarshall(int[,] graph, int source, int target)
        {
            /// <summary>
            /// algorithme de plus court chemin Floydmarshall
            /// <\summary>
            int V = graph.GetLength(0);
            int[,] dist = new int[V, V];
            int[,] next = new int[V, V];

            for (int i = 0; i < V; i++)
            {
                for (int j = 0; j < V; j++)
                {
                    if (graph[i, j] != 0 || i == j)
                    {
                        dist[i, j] = graph[i, j] != 0 ? graph[i, j] : (i == j ? 0 : int.MaxValue);
                        next[i, j] = graph[i, j] != 0 ? j : -1;
                    }
                    else
                    {
                        dist[i, j] = int.MaxValue;
                        next[i, j] = -1;
                    }
                }
            }

            for (int k = 0; k < V; k++)
            {
                for (int i = 0; i < V; i++)
                {
                    for (int j = 0; j < V; j++)
                    {
                        if (dist[i, k] != int.MaxValue && dist[k, j] != int.MaxValue && dist[i, k] + dist[k, j] < dist[i, j])
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                            next[i, j] = next[i, k];
                        }
                    }
                }
            }

            if (next[source, target] == -1)
                return new List<int>();

            List<int> path = new List<int>();
            int current = source;
            while (current != target)
            {
                if (current == -1)
                    return new List<int>();
                path.Add(current);
                current = next[current, target];
            }
            path.Add(target);

            return path;
        }
        public static void livraison(Graphe graphe)
        {
            /// <summary>
            /// fonction qui permet la gestion d'une livraison
            /// prend en paramètre un objet graphe
            /// fait les appel du plus court chemin ainsi que la création d'image du graphe
            /// <\summary>
            List<Noeud> listeStations = new List<Noeud>();
            int arrivee = 0;
            int depart = 0;
            string tempArrivee = "";
            string tempDepart = "";
            string stationDepart = "";
            string stationArrivee = "";
            int numLigne = -1;
            int numStation = -1;


            ///choix adresse de départ
            while (numLigne <= 0 || numLigne > 14)
            {
                Console.WriteLine("choix de l'adresse de départ : entrez numéro de la ligne (entre 1 et 14) :");
                numLigne = Convert.ToInt32(Console.ReadLine());
            }

            for (int i = 0; i < graphe.AllNodes.Count; i++)
            {
                if (graphe.AllNodes[i].libelleLigne.Contains(numLigne))
                {
                    listeStations.Add(graphe.AllNodes[i]);
                    tempDepart = graphe.AllNodes[i].libelleStation;
                    Console.WriteLine("numéro station : " + graphe.AllNodes[i].NodeID + " : " + tempDepart);
                }
            }
            Console.WriteLine("choix de la station : entrez le numéro de la station : ");
            numStation = Convert.ToInt32(Console.ReadLine());
            for (int i = 0; i < graphe.AllNodes.Count; i++)
            {
                if (graphe.AllNodes[i].NodeID == numStation)
                {
                    depart = graphe.AllNodes[i].NodeID;
                    stationDepart = graphe.AllNodes[i].libelleStation;
                    Console.WriteLine("station de départ : " + stationDepart);
                }
            }

            ///choix adresse de livraison
            Console.WriteLine();
            listeStations = new List<Noeud>();
            numLigne = -1;
            while (numLigne <= 0 || numLigne > 14)
            {
                Console.WriteLine("choix de l'adresse de livraison : entrez numéro de la ligne (entre 1 et 14) :");
                numLigne = Convert.ToInt32(Console.ReadLine());
            }
            for (int i = 0; i < graphe.AllNodes.Count; i++)
            {

                if (graphe.AllNodes[i].libelleLigne.Contains(numLigne))
                {
                    listeStations.Add(graphe.AllNodes[i]);
                    tempArrivee = graphe.AllNodes[i].libelleStation;
                    Console.WriteLine("numéro station : " + graphe.AllNodes[i].NodeID + " : " + tempArrivee);
                }


            }
            Console.WriteLine("choix de la station : entrez le numéro de la station : ");
            numStation = Convert.ToInt32(Console.ReadLine());
            for (int i = 0; i < graphe.AllNodes.Count; i++)
            {
                if (graphe.AllNodes[i].NodeID == numStation)
                {
                    arrivee = graphe.AllNodes[i].NodeID;
                    stationArrivee = graphe.AllNodes[i].libelleStation;
                    Console.WriteLine("station de livraison : " + stationArrivee);
                }
            }


            Console.WriteLine();

            List<int> cheminLePLusCourt = Dijkstra(depart, arrivee, graphe);

            Console.WriteLine("le chemin le plus court entre la station " + stationDepart + " et la station " + stationArrivee + " est : ");
            Console.WriteLine(cheminLePLusCourt.Count);
            for (int i = 0; i < cheminLePLusCourt.Count; i++)
            {


                Console.WriteLine("Station n° " + i + " est la station d'ID : " + cheminLePLusCourt[i]);
            }
        }
        public static void ColorationWelshPowell(int[,] matriceUtilisateurs)
        {
            /// <summary>
            /// coloration du graphe client cuisinier
            /// prend une matrice d'adjacence en paramètres
            /// <\summary>
            int n = matriceUtilisateurs.GetLength(0);
            int[] degres = new int[n];

            /// Calcul des degrés
            for (int i = 0; i < n; i++)
            {
                int deg = 0;
                for (int j = 0; j < n; j++)
                    if (matriceUtilisateurs[i, j] != 0)
                        deg++;
                degres[i] = deg;
            }

            /// Trie des sommets par degré décroissant
            List<int> sommets = new List<int>();
            bool[] dejaAjoute = new bool[n];

            for (int d = n; d >= 0; d--)
            {
                for (int i = 0; i < n; i++)
                {
                    if (!dejaAjoute[i] && degres[i] == d)
                    {
                        sommets.Add(i);
                        dejaAjoute[i] = true;
                    }
                }
            }

            int[] couleurs = new int[n]; /// 0 = non coloré
            int couleurActuelle = 1;

            while (sommets.Any(i => couleurs[i] == 0))
            {
                foreach (int sommet in sommets)
                {
                    if (couleurs[sommet] == 0)
                    {
                        bool peutColorer = true;

                        for (int i = 0; i < n; i++)
                        {
                            if (matriceUtilisateurs[sommet, i] != 0 || matriceUtilisateurs[i, sommet] != 0)
                            {
                                if (couleurs[i] == couleurActuelle)
                                {
                                    peutColorer = false;
                                    break;
                                }
                            }
                        }

                        if (peutColorer)
                            couleurs[sommet] = couleurActuelle;
                    }
                }

                couleurActuelle++;
            }

            int nbCouleurs = couleurs.Max();
            Console.WriteLine($"Nombre de couleurs minimales nécessaires : {nbCouleurs}");
            Console.WriteLine("Nombre de couleurs minimales nécessaires : " + nbCouleurs);

            Console.WriteLine(nbCouleurs == 2 ? "Le graphe est biparti." : "Le graphe n'est pas biparti.");
            if (nbCouleurs == 2)
            {
                Console.WriteLine("Le graphe est biparti");
            }
            else
            {
                Console.WriteLine("Le graphe n'est pas biparti");
            }


            int nbSommets = n;
            int nbArêtes = 0;

            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    if (matriceUtilisateurs[i, j] != 0 || matriceUtilisateurs[j, i] != 0)
                        nbArêtes++;

            bool planaire = nbArêtes <= (3 * nbSommets - 6);
            Console.WriteLine(planaire ? "Le graphe est planaire." : "Le graphe n'est pas planaire.");

            Console.WriteLine("\nGroupes indépendants (même couleur, aucun lien entre eux) :");
            for (int c = 1; c <= nbCouleurs; c++)
            {
                var groupe = Enumerable.Range(0, n).Where(i => couleurs[i] == c).ToList();
                Console.WriteLine($"Couleur {c} : " + string.Join(", ", groupe));
            }
        }
        public static List<int> ChuLiuEdmonds(int[,] matriceUtilisateurs, int racine)
        {
            int n = matriceUtilisateurs.GetLength(0);
            List<int> utilisateursInclus = new List<int>();

            for (int j = 0; j < n; j++)
            {
                if (j == racine) continue;

                int minSource = -1;
                int minCout = int.MaxValue;

                for (int i = 0; i < n; i++)
                {
                    if (i == j) continue;
                    int cout = matriceUtilisateurs[i, j];

                    if (cout < minCout)
                    {
                        minCout = cout;
                        minSource = i;
                    }
                }

                if (minSource != -1 && minCout != int.MaxValue)
                {
                    utilisateursInclus.Add(j);
                }
            }

            utilisateursInclus.Add(racine);

            return utilisateursInclus;
        }
        static void Main(string[] args)
        {


            string mdp = "root";
            string PathWayToDatabase = "server=localhost;user=root;password=" + mdp + ";database=livinparis;";
            using (MySqlConnection Connection = new MySqlConnection(PathWayToDatabase))
            {
                try
                {
                    Connection.Open();
                    Console.WriteLine("Database LivInParis connectée.");
                    System.Threading.Thread.Sleep(3000);
                    UserMenu(Connection);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
            }
            /// <summary>
            /// Génère une coordonnée géographique aléatoire située dans les limites de la ville de Paris.
            /// </summary>
            /// <returns>
            /// Une chaîne représentant un point géographique au format "POINT(longitude latitude)".
            /// </returns>
            static string CoordonneesParis()
            {
                Random Rand = new Random();
                double minLat = 48.8156;
                double maxLat = 48.9021;
                double minLon = 2.2242;
                double maxLon = 2.4699;
                double lat = Rand.NextDouble() * (maxLat - minLat) + minLat;
                double lon = Rand.NextDouble() * (maxLon - minLon) + minLon;
                var culture = System.Globalization.CultureInfo.InvariantCulture;
                return $"POINT({lon.ToString(culture)} {lat.ToString(culture)})";
            }
            static void UserMenu(MySqlConnection Connection)
            {
                bool exit = false;
                while (!exit)
                {
                    Console.Clear();
                    Console.WriteLine("=== Choix Connexion ===");
                    Console.WriteLine();
                    Console.WriteLine("1. Administrateur");
                    Console.WriteLine("2. Client");
                    Console.WriteLine("3. Cuisinier");
                    Console.WriteLine("0. Quitter");
                    Console.Write("Choisissez une option : ");
                    string mainChoice = Console.ReadLine();

                    switch (mainChoice)
                    {
                        case "1":
                            MainMenuADMIN(Connection);
                            break;
                        case "2":
                            MainMenuCLIENT(Connection);
                            break;
                        case "3":
                            MainMenuCUISINIER(Connection);
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
            static void MainMenuADMIN(MySqlConnection Connection)
            {
                bool exit = false;
                while (!exit)
                {
                    Console.Clear();
                    Console.WriteLine("=== Menu Principal (ADMINISTRATEUR) ===");
                    Console.WriteLine();
                    Console.WriteLine("1. Client");
                    Console.WriteLine("2. Cuisinier");
                    Console.WriteLine("3. Commandes");
                    Console.WriteLine("4. Statistiques");
                    Console.WriteLine("5. Autres");
                    Console.WriteLine("6. Exports BDD");
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
                        case "6":
                            ExportMenu(Connection);
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
            static void MainMenuCLIENT(MySqlConnection Connection)
            {
                bool exit = false;
                while (!exit)
                {
                    Console.Clear();
                    Console.WriteLine("=== Menu Principal (CLIENT) ===");
                    Console.WriteLine();
                    Console.WriteLine("1. Passer commande");
                    Console.WriteLine("2. Voir commande");
                    Console.WriteLine("0. Quitter");
                    Console.Write("Choisissez une option : ");
                    string mainChoice = Console.ReadLine();

                    switch (mainChoice)
                    {
                        case "1":
                            AddCommande(Connection);
                            break;
                        case "2":
                            ViewCommande(Connection);
                            break;
                        case "0":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Option invalide. Appuyez sur une touche pour réessayer...");
                            Console.ReadKey();
                            break;
                    }
                    if (!exit)
                    {
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
                    }
                }
            }
            static void MainMenuCUISINIER(MySqlConnection Connection)
            {
                bool exit = false;
                while (!exit)
                {
                    Console.Clear();
                    Console.WriteLine("=== Menu Principal (CUISINIER) ===");
                    Console.WriteLine();
                    Console.WriteLine("1. Chiffre d'affaires");
                    Console.WriteLine("2. Meilleurs plats");
                    Console.WriteLine("3. Voir commande");
                    Console.WriteLine("0. Quitter");
                    Console.Write("Choisissez une option : ");
                    string mainChoice = Console.ReadLine();

                    switch (mainChoice)
                    {
                        case "1":
                            CACuisiniers(Connection);
                            break;
                        case "2":
                            BestPlats(Connection);
                            break;
                        case "3":
                            ViewCommande(Connection);
                            break;
                        case "0":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Option invalide. Appuyez sur une touche pour réessayer...");
                            Console.ReadKey();
                            break;
                    }
                    if (!exit)
                    {
                        Console.WriteLine("Appuyez sur une touche pour continuer...");
                        Console.ReadKey();
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
            /// <summary>
            /// Affiche un formulaire dans la console pour ajouter un utilisateur, récupère les données saisies,
            /// génère des coordonnées géographiques aléatoires situées à Paris, et insère l'utilisateur dans la base de données.
            /// </summary>
            static void AddUser(MySqlConnection Connection)
            {
                Console.Clear();
                Console.WriteLine("=== Ajouter un client ===");
                Console.WriteLine();
                Console.Write("Prénom : ");
                string prenom = Console.ReadLine();
                Console.Write("Nom : ");
                string nom = Console.ReadLine();
                string positionGeo = CoordonneesParis();
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
                    string geom = positionGeo.Replace("'", "''");
                    string instruction =
                    "INSERT INTO Utilisateur (prenom, nom, coordonnees_geographiques, email, mdp, est_client, est_cuisinier, type_client) " +
                    $"VALUES (@prenom, @nom, ST_SRID(ST_GeomFromText('{geom}'), 4326), @email, @mdp, @est_client, @est_cuisinier, @type_client);";

                    MySqlCommand command = new MySqlCommand(instruction, Connection);
                    command.Parameters.AddWithValue("@prenom", prenom);
                    command.Parameters.AddWithValue("@nom", nom);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@mdp", mdp);
                    command.Parameters.AddWithValue("@est_client", est_client);
                    command.Parameters.AddWithValue("@est_cuisinier", est_cuisinier);
                    command.Parameters.AddWithValue("@type_client", type_client);

                    Console.WriteLine("Requête envoyée :");
                    Console.WriteLine(instruction);

                    int rowsAffected = command.ExecuteNonQuery();
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
                    string positionGeo = CoordonneesParis();
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
                    try
                    {
                        string Instruction = "UPDATE Utilisateur SET prenom = @prenom, nom = @nom, coordonnees_geographiques = ST_GeomFromText(@coordonnees_geographiques), email = @email, mdp = @mdp, est_client = @est_client, est_cuisinier = @est_cuisinier, type_client = @type_client WHERE id_utilisateur = @id;";
                        MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                        Command.Parameters.AddWithValue("@prenom", prenom);
                        Command.Parameters.AddWithValue("@nom", nom);
                        Command.Parameters.AddWithValue("@coordonnees_geographiques", positionGeo);
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
            /// <summary>
            /// Affiche la liste des clients (et non des cuisiniers) triés par nom et prénom dans l'ordre alphabétique,
            /// avec leurs informations principales : ID, nom, prénom, coordonnées géographiques et email.
            /// </summary>
            static void InformationsClient(MySqlConnection Connection)
            {
                Console.Clear();
                Console.WriteLine("=== Liste des Clients par Nom, Prénom (ordre alphabétique) ===");
                Console.WriteLine();
                string Instruction = "SELECT id_utilisateur, prenom, nom, ST_AsText(coordonnees_geographiques) AS coordonnees, email FROM Utilisateur WHERE est_client = TRUE ORDER BY nom ASC, prenom ASC;";
                MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                MySqlDataReader reader = Command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string prenom = reader.GetString(1);
                    string nom = reader.GetString(2);
                    string coordonnees = reader.GetString(3);
                    string email = reader.GetString(4);
                    Console.WriteLine($"ID: {id}, Nom: {nom}, Prénom: {prenom}, Coordonnées: {coordonnees}, Email: {email}");
                }
                reader.Close();
                Console.WriteLine("Appuyez sur une touche pour continuer...");
                Console.ReadKey();
                Console.Clear();
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

                Console.WriteLine("graphe clients -- cuisiniers : ");
                int[,] matriceUtilisateurs = creationMatriceJson("JsonExportBDD.json");
                affichageMatrice(matriceUtilisateurs);
                Console.WriteLine();
                Console.WriteLine("algo de Chu-Liu Edmonds donnant l'arborescance couvrante de poids minimum : ");
                List<int> result = ChuLiuEdmonds(matriceUtilisateurs, 3);
                Console.WriteLine(string.Join(", ", result));

                Console.WriteLine("coloration du graphe : ");
                ColorationWelshPowell(matriceUtilisateurs);
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
            /// <summary>
            /// affichage de la liste des plats végétariens
            /// utilisation SQL avec WHERE LIKE pour filtrer les plats
            /// </summary>
            static void Vegetarien(MySqlConnection Connection)
            {
                Console.Clear();
                Console.WriteLine("=== Carte Végétarienne ===");
                Console.WriteLine();
                try
                {
                    string Instruction = "SELECT nom_plat, prix_plat, nationalite, regime_alimentaire FROM Plat WHERE regime_alimentaire LIKE '%Végétarien%';";
                    MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                    MySqlDataReader reader = Command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine($"Nom du plat: {reader["nom_plat"]}, Prix: {reader["prix_plat"]}, Nationalité: {reader["nationalite"]}, Caractéristiques: {reader["regime_alimentaire"]}");
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

            /// <summary>
            /// methode qui permet d'afficher les 10 clients les plus fidèles
            ///requete SQL avec utilisation de COUNT pour le nombre total de commande par ID client
            ///on tri dans l'ordre decroissant afin de faire apparaitre le plus fidèle en premier
            /// </summary>
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
            
            /// <summary>
            /// utilisation de GROUP BY et COUNT pour agrérer les donnés pas plat et le nombre de commandes
            /// on utilise ORDER BY ... DESC afin d'obtenir les 10 plats les plus populaires
            /// </summary>
            static void BestPlats(MySqlConnection Connection)
            {
                Console.Clear();
                Console.WriteLine("=== Nos Plats les plus populaires ===");
                Console.WriteLine();
                try
                {
                    string Instruction = "SELECT p.nom_plat, COUNT(lc.id_plat) AS nb_commandes " +
                        "FROM LigneCommande lc JOIN Plat p ON lc.id_plat = p.id_plat " +
                        "GROUP BY p.nom_plat ORDER BY nb_commandes DESC LIMIT 10;";
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
            /// <summary>
            /// Affiche le chiffre d'affaires réalisé par chaque cuisinier, calculé à partir du total des commandes associées à ses plats.
            /// Le résultat est trié par chiffre d'affaires décroissant.
            /// </summary>
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
                            EditCommande(Connection);
                            break;
                        case "3":
                            ViewCommande(Connection);
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
            /// <summary>
            /// Ajoute une commande pour un client existant. La fonction vérifie que l'ID du client correspond à un client (et pas un cuisinier),
            /// puis permet de sélectionner des plats et de spécifier les quantités. Enfin, elle calcule le montant total de la commande 
            /// et insère la commande dans la base de données avec les lignes de commande associées.
            /// </summary>
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
                Console.Clear();
                Instruction = "SELECT ST_X(coordonnees_geographiques), ST_Y(coordonnees_geographiques) FROM Utilisateur WHERE id_utilisateur = @id_client;";
                Command = new MySqlCommand(Instruction, Connection);
                Command.Parameters.AddWithValue("@id_client", id_client);
                double latitude = 0;
                double longitude = 0;
                using (MySqlDataReader geoReader = Command.ExecuteReader())
                {
                    if (geoReader.Read())
                    {
                        latitude = geoReader.GetDouble(0);
                        longitude = geoReader.GetDouble(1);
                    }
                }
                Console.WriteLine($"Position géographique du client : Latitude = {latitude}, Longitude = {longitude}");
                Console.WriteLine("=== Carte des Plats ===");
                string InstructionCarte = "SELECT id_plat, nom_plat, prix_plat FROM Plat ORDER BY id_plat;";
                MySqlCommand CommandCarte = new MySqlCommand(InstructionCarte, Connection);
                using (MySqlDataReader readerCarte = CommandCarte.ExecuteReader())
                {
                    while (readerCarte.Read())
                    {
                        int idPlat = readerCarte.GetInt32("id_plat");
                        string nomPlat = readerCarte.GetString("nom_plat");
                        decimal prix = readerCarte.GetDecimal("prix_plat");
                        Console.WriteLine($"ID: {idPlat}, Nom: {nomPlat}, Prix: {prix} EUR");
                    }
                }
                List<(int platId, int quantity)> OrderLines = new List<(int, int)>();
                decimal MontantTotal = 0m;
                while (true)
                {
                    Console.Write("\nEntrez l'ID du plat à ajouter (0 pour terminer) : ");
                    if (!int.TryParse(Console.ReadLine(), out int id_plat))
                    {
                        Console.WriteLine("ID de plat invalide.");
                        continue;
                    }
                    if (id_plat == 0)
                    {
                        break;
                    }
                    string InstructionPlat = "SELECT nom_plat, prix_plat FROM Plat WHERE id_plat = @id_plat;";
                    MySqlCommand CommandPlat = new MySqlCommand(InstructionPlat, Connection);
                    CommandPlat.Parameters.AddWithValue("@id_plat", id_plat);
                    using (MySqlDataReader platReader = CommandPlat.ExecuteReader())
                    {
                        if (platReader.Read())
                        {
                            string NomPlat = platReader.GetString("nom_plat");
                            decimal prix = platReader.GetDecimal("prix_plat");
                            Console.WriteLine($"Plat sélectionné: {NomPlat}, Prix: {prix}EUR");
                            Console.Write("Entrez la quantité: ");
                            if (!int.TryParse(Console.ReadLine(), out int quantite) || quantite <= 0)
                            {
                                Console.WriteLine("Quantité invalide. Ce plat ne sera pas ajouté.");
                                continue;
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
                        // Appel de livraison et génération de graphe
                        Graphe metro = new Graphe("MetroParisNoeuds.csv", "MetroParisArcs.csv");
                        livraison(metro);
                    }
                    else
                    {
                        Console.WriteLine("Erreur lors de l'insertion de la commande.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
            }
            static void EditCommande(MySqlConnection Connection)
            {
                Console.Clear();
                Console.Write("Entrez l'ID de la commande à modifier : ");
                if (int.TryParse(Console.ReadLine(), out int idCommande))
                {
                    try
                    {
                        string Instruction = "SELECT id_client, date_commande FROM Commande WHERE id_commande = @idCommande;";
                        MySqlCommand CommandCommande = new MySqlCommand(Instruction, Connection);
                        CommandCommande.Parameters.AddWithValue("@idCommande", idCommande);
                        MySqlDataReader reader = CommandCommande.ExecuteReader();
                        if (!reader.Read())
                        {
                            Console.WriteLine("Aucune commande trouvée avec cet ID.");
                            reader.Close();
                            return;
                        }
                        int idClient = Convert.ToInt32(reader["id_client"]);
                        DateTime DateCommande = DateTime.Now;
                        reader.Close();
                        Console.WriteLine($"Commande trouvée : Client ID: {idClient}, Date: {DateCommande}");
                        string DeleteOldInstruction = "DELETE FROM LigneCommande WHERE id_commande = @idCommande;";
                        MySqlCommand DeleteOldCommand = new MySqlCommand(DeleteOldInstruction, Connection);
                        DeleteOldCommand.Parameters.AddWithValue("@idCommande", idCommande);
                        DeleteOldCommand.ExecuteNonQuery();
                        Console.Clear();
                        Console.WriteLine("=== Carte des Plats ===");
                        string InstructionCarte = "SELECT id_plat, nom_plat, prix_plat FROM Plat ORDER BY id_plat;";
                        MySqlCommand CommandCarte = new MySqlCommand(InstructionCarte, Connection);
                        using (MySqlDataReader readerCarte = CommandCarte.ExecuteReader())
                        {
                            while (readerCarte.Read())
                            {
                                int idPlat = readerCarte.GetInt32("id_plat");
                                string nomPlat = readerCarte.GetString("nom_plat");
                                decimal prixPlat = readerCarte.GetDecimal("prix_plat");
                                Console.WriteLine($"ID: {idPlat}, Nom: {nomPlat}, Prix: {prixPlat} EUR");
                            }
                        }
                        Console.WriteLine("=== Fin de la carte ===");
                        Console.WriteLine();
                        List<(int idPlat, int quantite)> NouvelleCommande = new List<(int, int)>();
                        decimal MontantTotal = 0m;
                        while (true)
                        {
                            Console.Write("Entrez l'ID du plat (ou 0 pour terminer) : ");
                            if (!int.TryParse(Console.ReadLine(), out int idPlat) || idPlat == 0)
                            {
                                break;
                            }
                            Console.Write("Quantité : ");
                            if (!int.TryParse(Console.ReadLine(), out int quantite) || quantite <= 0)
                            {
                                Console.WriteLine("Quantité invalide.");
                                continue;
                            }
                            string InstructionPlat = "SELECT nom_plat, prix_plat FROM Plat WHERE id_plat = @idPlat;";
                            MySqlCommand CommandPlat = new MySqlCommand(InstructionPlat, Connection);
                            CommandPlat.Parameters.AddWithValue("@idPlat", idPlat);
                            using (MySqlDataReader PlatReader = CommandPlat.ExecuteReader())
                            {
                                if (PlatReader.Read())
                                {
                                    string NomPlat = PlatReader["nom_plat"].ToString();
                                    decimal prix = Convert.ToDecimal(PlatReader["prix_plat"]);
                                    MontantTotal += prix * quantite;
                                    NouvelleCommande.Add((idPlat, quantite));
                                    Console.WriteLine($"Ajouté : {quantite}X {NomPlat}, Total: {prix * quantite} EUR");
                                }
                                else
                                {
                                    Console.WriteLine("ID du plat invalide.");
                                }
                            }
                        }
                        foreach (var (idPlat, quantite) in NouvelleCommande)
                        {
                            string NewLineOrder = "INSERT INTO LigneCommande (id_commande, id_plat, quantite) VALUES (@idCommande, @idPlat, @quantite);";
                            MySqlCommand InstructionOrder = new MySqlCommand(NewLineOrder, Connection);
                            InstructionOrder.Parameters.AddWithValue("@idCommande", idCommande);
                            InstructionOrder.Parameters.AddWithValue("@idPlat", idPlat);
                            InstructionOrder.Parameters.AddWithValue("@quantite", quantite);
                            InstructionOrder.ExecuteNonQuery();
                        }
                        string updateCommandeQuery = "UPDATE Commande SET date_commande = @newDate, montant_total = @montantTotal WHERE id_commande = @idCommande;";
                        MySqlCommand updateCmd = new MySqlCommand(updateCommandeQuery, Connection);
                        updateCmd.Parameters.AddWithValue("@newDate", DateCommande);
                        updateCmd.Parameters.AddWithValue("@montantTotal", MontantTotal);
                        updateCmd.Parameters.AddWithValue("@idCommande", idCommande);
                        updateCmd.ExecuteNonQuery();
                        Console.WriteLine($"Commande mise à jour avec succès ! Nouveau total : {MontantTotal} EUR");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erreur : " + ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("ID de commande invalide.");
                }
            }
            
            /// <summary>
            /// verifie la commande en fonction de l'ID avec l'utilisation de la bibliothèque
            /// jointure SQl entre table commande, utilisateur, LigneCommande et Plat pour récupérer les informations
            /// </summary>
            static void ViewCommande(MySqlConnection Connection)
            {
                Console.Clear();
                Console.Write("Entrez l'ID de la commande à afficher : ");
                if (int.TryParse(Console.ReadLine(), out int IDCommande))
                {
                    try
                    {
                        string Instruction = "SELECT c.id_commande, c.date_commande, c.montant_total, u.id_utilisateur, u.prenom, u.nom FROM Commande c JOIN Utilisateur u ON c.id_client = u.id_utilisateur WHERE c.id_commande = @idCommande;";
                        MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                        Command.Parameters.AddWithValue("@idCommande", IDCommande);
                        MySqlDataReader Reader = Command.ExecuteReader();
                        if (!Reader.Read())
                        {
                            Console.WriteLine("Aucune commande trouvée avec cet ID.");
                            Reader.Close();
                            return;
                        }
                        int IDClient = Convert.ToInt32(Reader["id_utilisateur"]);
                        string Prenom = Reader["prenom"].ToString();
                        string Nom = Reader["nom"].ToString();
                        DateTime DateCommande = Convert.ToDateTime(Reader["date_commande"]);
                        decimal MontantTotal = Convert.ToDecimal(Reader["montant_total"]);
                        Reader.Close();
                        Console.WriteLine($"Détails de la commande {IDCommande}.\n");
                        Console.WriteLine($"Client: {Prenom} {Nom} (ID: {IDClient})");
                        Console.WriteLine($"Date: {DateCommande}");
                        Console.WriteLine($"Montant total: {MontantTotal}EUR");
                        string InstructionContenuCommande = "SELECT p.nom_plat, p.prix_plat, lc.quantite FROM LigneCommande lc JOIN Plat p ON lc.id_plat = p.id_plat WHERE lc.id_commande = @idCommande;";
                        MySqlCommand CommandePlat = new MySqlCommand(InstructionContenuCommande, Connection);
                        CommandePlat.Parameters.AddWithValue("@idCommande", IDCommande);
                        MySqlDataReader ReaderPlats = CommandePlat.ExecuteReader();
                        Console.WriteLine("Plats commandés :");
                        while (ReaderPlats.Read())
                        {
                            string NomPlat = ReaderPlats["nom_plat"].ToString();
                            decimal prix = Convert.ToDecimal(ReaderPlats["prix_plat"]);
                            int quantite = Convert.ToInt32(ReaderPlats["quantite"]);
                            Console.WriteLine($"- {quantite}X {NomPlat}, {prix}EUR/unité, {prix * quantite}EUR");
                        }
                        ReaderPlats.Close();
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
            
            static void ExportMenu(MySqlConnection Connection)
            {
                bool back = false;
                while (!back)
                {
                    Console.Clear();
                    Console.WriteLine("=== Menu Export ===");
                    Console.WriteLine();
                    Console.WriteLine("1. Exporter la BDD en Json");
                    Console.WriteLine("2. Exporter la BDD en XML");
                    Console.WriteLine("0. Retour au menu principal");
                    Console.Write("Choisissez une option : ");
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            JsonExport(Connection, "JsonExportBDD.json");
                            break;
                        case "2":
                            XMLExport(Connection, "XMLExportBDD.xml");
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
            /// <summary>
            /// Exporte l'intégralité de la base de données connectée au format JSON dans un fichier.
            /// Chaque table est représentée comme une liste de dictionnaires (ligne = dictionnaire de colonnes).
            /// </summary>
            static void JsonExport(MySqlConnection Connection, string FileName)
            {
                Dictionary<string, List<Dictionary<string, object>>> AllBDD = new Dictionary<string, List<Dictionary<string, object>>>();
                try
                {
                    List<string> TablesNames = new List<string>();
                    string Instruction = "SHOW TABLES;";
                    MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                    MySqlDataReader reader = Command.ExecuteReader();
                    while (reader.Read())
                    {
                        TablesNames.Add(reader.GetString(0));
                    }
                    reader.Close();
                    foreach (string TableByTable in TablesNames)
                    {
                        List<Dictionary<string, object>> lignes = new List<Dictionary<string, object>>();
                        string InstructionLignes = $"SELECT * FROM {TableByTable};";
                        MySqlCommand CommandLignes = new MySqlCommand(InstructionLignes, Connection);
                        MySqlDataReader readerLignes = CommandLignes.ExecuteReader();
                        while (readerLignes.Read())
                        {
                            Dictionary<string, object> ligne = new Dictionary<string, object>();
                            for (int i = 0; i < readerLignes.FieldCount; i++)
                            {
                                string ColonneName = readerLignes.GetName(i);
                                object valeur = readerLignes.GetValue(i);
                                ligne[ColonneName] = valeur;
                            }

                            lignes.Add(ligne);
                        }
                        readerLignes.Close();
                        AllBDD[TableByTable] = lignes;
                    }
                    JsonSerializerOptions Identation = new JsonSerializerOptions { WriteIndented = true };
                    string json = JsonSerializer.Serialize(AllBDD, Identation);
                    File.WriteAllText(FileName, json);
                    Console.WriteLine($"Export JSON terminé ! Fichier : {FileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors de l’export : " + ex.Message);
                }
            }
            /// <summary>
            /// Exporte l'intégralité de la base de données connectée au format XML dans un fichier.
            /// Chaque table est représentée comme une liste de dictionnaires (ligne = dictionnaire de colonnes).
            /// </summary>
            static void XMLExport(MySqlConnection Connection, string FileName)
            {
                try
                {
                    XElement databaseElement = new XElement("Database");
                    List<string> TablesNames = new List<string>();
                    string Instruction = "SHOW TABLES;";
                    MySqlCommand Command = new MySqlCommand(Instruction, Connection);
                    MySqlDataReader reader = Command.ExecuteReader();
                    while (reader.Read())
                    {
                        TablesNames.Add(reader.GetString(0));
                    }
                    reader.Close();

                    foreach (string TableByTable in TablesNames)
                    {
                        XElement tableElement = new XElement(TableByTable);
                        string InstructionLignes = $"SELECT * FROM {TableByTable};";
                        MySqlCommand CommandLignes = new MySqlCommand(InstructionLignes, Connection);
                        MySqlDataReader readerLignes = CommandLignes.ExecuteReader();
                        while (readerLignes.Read())
                        {
                            XElement rowElement = new XElement("Row");
                            for (int i = 0; i < readerLignes.FieldCount; i++)
                            {
                                string ColonneName = readerLignes.GetName(i);
                                object valeur;
                                if (readerLignes.IsDBNull(i))
                                {
                                    valeur = null;
                                }
                                else
                                {
                                    valeur = readerLignes.GetValue(i);
                                }
                                rowElement.Add(new XElement(ColonneName, valeur));
                            }
                            tableElement.Add(rowElement);
                        }
                        databaseElement.Add(tableElement);
                        readerLignes.Close();
                    }
                    databaseElement.Save(FileName);
                    Console.WriteLine($"Export XML terminé ! Fichier : {FileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors de l’export : " + ex.Message);
                }
            }
        }
    }
}
