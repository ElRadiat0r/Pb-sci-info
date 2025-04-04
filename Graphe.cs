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

            string[] header = ligne.Split(";");

            while ((ligne = lecteurstation.ReadLine()) != null)
            {
                header = ligne.Split(";");
                Noeud gare = new Noeud(ligne[0], ligne[1], ligne[2], ligne[3], ligne[4]);
                if(gare != null)
                {
                    AllNodes.Add(gare);
                }
               
            }

            while ((ligne = lecteurarcs.ReadLine()) != null)
            {
                header = ligne.Split(";");

                Lien arcs = new Lien(ligne[0], ligne[1], ligne[2], ligne[3], ligne[4]);
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
