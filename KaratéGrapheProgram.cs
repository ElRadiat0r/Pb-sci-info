using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KarateGraphe
{
    public class Program
    {
        static void Main(string[] args)
        {
            string chemin = "soc-karate.mtx";
            StreamReader reader = new(chemin);

            string ligne = "";
            ligne =reader.ReadLine();
            string[] header = ligne.Split(" ");

            if (header[2] == "coordinate")
            {
                while(reader.ReadLine() != null)
                {
                    ligne = reader.ReadLine();
                    if (ligne[0] != '%')
                    {
                        string[] tabline = ligne.Split(' ');
                        int[,] matriceUsers = null;

                        if (tabline.Length == 3)
                        {
                            matriceUsers = new int[Convert.ToInt32(tabline[0]), Convert.ToInt32(tabline[1])];
                        }
                        else
                        {
                            matriceUsers[tabline[0], tabline[1]] == 1;
                        }
                    }
                }
            }
            Console.WriteLine(ligne);

            string matriceKarate = reader.ReadToEnd();

        }
    }
}
