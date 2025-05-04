namespace ADUFORET_TDUCOURAU_JESPINOS_LivInParis
{
    public class Noeud
    {
        public int NodeID;
        private int EnteringDegree;
        private int OutgoingDegree;
        public List<int> libelleLigne;
        public string libelleStation;
        public double latitude;
        public double longitude;

        public Noeud(int ID, int Libelleligne, string Libellestation, double Longitude, double Latitude)
        {
            NodeID = ID;
            libelleLigne = new List<int>();
            libelleLigne.Add(Libelleligne);
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
