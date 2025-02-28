using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarateGraph
{
    internal class Pile
    {
        public List<int> pile;

        public Pile() 
        { 
            pile = new List<int>();
        }

        public void add(int valeur)
        {
            this.pile.Add(valeur);
        }

        public int remove()
        {
            int result = pile[pile.Count - 1];
            this.pile.Remove(pile.Count - 1);
            return result;
        }

        public void ToString()
        {
            for(int i = 0; i < pile.Count; i++)
            {
                Console.WriteLine(pile[i]);
            }
        }
    }
}
