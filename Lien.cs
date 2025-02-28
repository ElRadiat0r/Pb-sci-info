using System;

namespace KarateGraphe
{
    public class Lien
    {
        public int StartingNode;
        public int EndingNode;
        public int TripValue;
        public bool Orientation;
        public Lien(int Departure, int Destination, int Trip, bool orientation = false)
        {
            StartingNode = Departure;
            EndingNode = Destination;
            TripValue = Trip;
            Orientation = orientation;
        }
        public override string ToString()
        {
            return StartingNode + (Orientation ? " ---> " : " <---> ") + EndingNode + "\nPoids : " + TripValue + "\n";
        }
    }
}
