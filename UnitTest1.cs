namespace ADUFORET_TDUCOURAU_JESPINOS_LivInParis
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void parcoursprofond()
        {
            int[,] mat = new int[,]
            {
                {1,2,0,3},
                {1,2,3,4},
                {0,0,1,0},
                {0,1,0,2},
            };
            int result = Program.parcoursProfondeur(mat, 0, false);
            Assert.AreEqual(1, 1);
        }

        [TestMethod]
        public void cycletest()
        {
            int[,] mat = new int[,]
            {
                {1,1,1},
                {1,0,1},
                {1,1,1},
            };
            Assert.IsTrue(Program.ContientCycle(mat));
        }

        [TestMethod]
        public void Bellmanford()
        {
            int[,] matrice = new int[,]
            {
                {0,3,1},
                {0,0,0},
                {0,1,0},
            };
        
            int depart = 0;
            int arrivee = 2;
        
            List<int> chemin = Program.BellmanFord(depart, arrivee, matrice);
            List<int> attendu = new List<int> { 0, 2, 1 };
        
            Assert.AreEqual(3, attendu.Count);
            Assert.AreEqual(0, attendu[0]);
            Assert.AreEqual(2, attendu[1]);
            Assert.AreEqual(1, attendu[2]);
        }
                   

        [TestMethod]
        public void connexité()
        {
            int[,] mat = new int[,]
            {
                {1,2,0,3},
                {1,2,3,4},
                {0,0,1,0},
                {0,1,2,2},
            };
            Assert.IsTrue(Program.estConnexe(mat));
        }

         [TestMethod]
         public void Parcourslargeurtest()
         {
             int[,] mat = new int[,]
             {
                 {1,2,0,3},
                 {1,2,3,4},
                 {0,0,1,0},
                 {0,1,0,2},
             };
             int result = Program.parcoursLargeur(mat, 1);
             Assert.AreEqual(4, result);
         }
    }
}
