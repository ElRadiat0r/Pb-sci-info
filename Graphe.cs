using System;
using System.Collections.Generic;

namespace KarateGraphe
{
    public class Graphe
    {
        public List<Noeud> AllNodes { get; private set; }
        public List<Lien> AllLinks { get; private set; }
        public Graphe(int[,] TheMatrix, bool Oriented = false)
        {
            AllNodes = new List<Noeud>();
            AllLinks = new List<Lien>();
            for (int i = 0; i < TheMatrix.GetLength(0); i++)
            {
                Noeud node = new Noeud(i);
                node.DegreesDefinition(TheMatrix);
                AllNodes.Add(node);
            }
            for (int i = 0; i < TheMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < TheMatrix.GetLength(0); j++)
                {
                    if (TheMatrix[i, j] != 0 && Oriented || i >= j)
                    {
                        AllLinks.Add(new Lien(i, j, TheMatrix[i, j], Oriented));
                    }
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
