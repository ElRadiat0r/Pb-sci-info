using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaratéGraphe
{
    public class Graphe
    {
        public List<Noeud> AllNodes { get; private set; }
        public Graphe(int[,] TheMatrix)
        {
            AllNodes = new List<Noeud>();
            for (int i = 0; i < TheMatrix.GetLength(0); i++)
            {
                Noeud Node = new Noeud(i);
                Node.DegreesDefinition(TheMatrix);
                AllNodes.Add(Node);
            }
        }
        public void AfficherGraphe()
        {
            foreach (var Node in AllNodes)
            {
                Console.WriteLine(Node.ToString());
            }
        }
    }
}
