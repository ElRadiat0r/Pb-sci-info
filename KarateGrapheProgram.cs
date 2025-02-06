using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaratéGraphe
{
    public class KarateGrapheProgram
    {
        static void Main(string[] args)
        {
            int[,] Test = new int[,] { {0, 3, 0 }, {3, 2, 43 }, {1, 3, 0 } };
            Graphe UnGraphe = new Graphe(Test);
            UnGraphe.AfficherGraphe();
        }
    }
}
