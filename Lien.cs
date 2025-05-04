using System;

namespace ADUFORET_TDUCOURAU_JESPINOS_LivInParis
{
    public class Lien
    {
        public int stationId;
        public string libelleStation;
        public int startingNode;
        public int endingNode;
        public int tripValue;
        public bool orientation;
        public Lien(int Stationid, string Libellestation, int Departure, int Destination, int TripValue, bool orientation = false)
        {
            stationId = Stationid;
            libelleStation = Libellestation;
            startingNode = Departure;
            endingNode = Destination;
            tripValue = TripValue;
        }
        public override string ToString()
        {
            return startingNode + (orientation ? " ---> " : " <---> ") + endingNode + "\nPoids : " + tripValue + "\n";
        }
    }
}
