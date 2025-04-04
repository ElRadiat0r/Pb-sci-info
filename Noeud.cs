using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarateGraphe
{
    public class Noeud
    {
        public int NodeID;
        private int EnteringDegree;
        private int OutgoingDegree;
        public int libelleLigne;
        public string libelleStation;
        public double latitude;
        public double longitude;

        public Noeud(int ID, int Libelleligne, string Libellestation, double Longitude, double Latitude)
        {
            NodeID = ID;
            libelleLigne = Libelleligne;
            libelleStation = Libellestation;
            longitude = Longitude;
            latitude = Latitude;

        }
        public int ID
        {
            get { return NodeID; }
        }
        public int Entering
        {
            get { return EnteringDegree; }
        }
        public int Outgoing
        {
            get { return OutgoingDegree; }
        }
        
        public override string ToString()
        {
            return "Numéro du noeud : " + NodeID + "\nDegré entrant : " + EnteringDegree + "\nDegré sortant : " + OutgoingDegree + "\n";
        }
    }
}
