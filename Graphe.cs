using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Graphviz4Net.Graphs;
using System.Diagnostics;
using System.IO;
using Org.BouncyCastle.Crypto.Engines;

namespace KarateGraphe
{
    public class Graphe
    {
        public List<Noeud> AllNodes;
        public List<Lien> AllLinks;
        public Graphe(string cheminStation, string cheminArcs)
        {
            AllNodes = new List<Noeud>();
            AllLinks = new List<Lien>();

            StreamReader lecteurstation = new(cheminStation);
            StreamReader lecteurarcs = new(cheminArcs);
            string ligne = "";
            ligne = lecteurstation.ReadLine();

            string[] tabLine = ligne.Split(";");

            while ((ligne = lecteurstation.ReadLine()) != null)
            {
                tabLine = ligne.Split(";");
                
                Noeud gare = new Noeud(Convert.ToInt32(tabLine[0]), Convert.ToInt32(tabLine[1]), tabLine[2], double.Parse(tabLine[3]), double.Parse(tabLine[4]));
                if(gare != null)
                {
                    AllNodes.Add(gare);
                }
               
            }

            ligne = lecteurarcs.ReadLine();
            int stationId = -1;
            int Departure = -1;
            int Destination = -1;
            int tripValue = -1;

            while ((ligne = lecteurarcs.ReadLine()) != null)
            {
                tabLine = ligne.Split(";");

                Lien arcs = new Lien(Convert.ToInt32(tabLine[0]), tabLine[1], Convert.ToInt32(tabLine[0]), Convert.ToInt32(tabLine[3]), Convert.ToInt32(tabLine[4]));
                if (arcs != null)
                {
                    AllLinks.Add(arcs);
                }
            }



        }
        public void AfficherGraphe()
        {
            foreach (var node in AllNodes)
            {
                Console.WriteLine(node);
            }
            foreach (var link in AllLinks)
            {
                Console.WriteLine(link);
            }
        }
    }
}
