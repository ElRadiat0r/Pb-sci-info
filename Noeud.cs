using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaratéGraphe
{
    public class Noeud
    {
        private int NodeID;
        private int EnteringDegree;
        private int OutgoingDegree;
        public Noeud(int ID)
        {
            NodeID = ID;
            EnteringDegree = 0;
            OutgoingDegree = 0;
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
        public void DegreesDefinition(int[,] matriceAdjacence)
        {
            for (int i = 0; i < matriceAdjacence.GetLength(0); i++)
            {
                if (matriceAdjacence[NodeID, i] != 0)
                {
                    OutgoingDegree++;
                }
                if (matriceAdjacence[i, NodeID] != 0)
                {
                    EnteringDegree++;
                }
            }
        }
        public override string ToString()
        {
            return "Numéro du noeud : " + NodeID + "\nDegré entrant : " + EnteringDegree + "\nDegré sortant : " + OutgoingDegree;
        }
    }
}