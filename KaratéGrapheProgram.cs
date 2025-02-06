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
        public static int[,] creationMatrice(string chemin)
        {
            int[,] matriceUsers = null;

            StreamReader lecteur = new(chemin);
            string ligne = "";
            ligne = lecteur.ReadLine();

            string[] header = ligne.Split(" ");

            if (header[2] == "coordinate")
            {
                Console.WriteLine("format = coordinate");
                while ((ligne = lecteur.ReadLine()) != null)
                {
                    if (ligne!=null && ligne[0] != '%' && ligne.Length > 0)
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

        static void Main(string[] args)
        {
            string chemin = "soc-karate.mtx";
            StreamReader fichier = new(chemin);
            string ligne = fichier.ReadLine();
            int[,] matriceUsers = creationMatrice(chemin);
            if (matriceUsers != null)
            {
                Console.WriteLine("taille matrice : " + matriceUsers.GetLength(0));
                while ((ligne = fichier.ReadLine()) != null)
                {
                    if (ligne != null && ligne[0] != '%' && ligne.Length > 0)
                    {

                        string[] tabline = ligne.Split(' ');
                        
                        if (tabline.Length == 2)
                        {
                            matriceUsers[Convert.ToInt32(tabline[0])-1,Convert.ToInt32(tabline[1])-1] = 1;
                        }
                    }
                }
                for (int i = 0; i < matriceUsers.GetLength(0); i++)
                {
                    for (int ii = 0; ii < matriceUsers.GetLength(1); ii++)
                    {
                        Console.Write(matriceUsers[i,ii] + ", ");
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
