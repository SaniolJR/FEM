using GridAndDetailsNamespace;

namespace obliczemia_m_glob_namespace
{
    class bokHBC
    {

        private double detJ;
        public double[,] HBC { get; }
        public bokHBC(BokElementu bok, Node n1, Node n2, int alfa)
        {
            if (!bok.boundary)
                throw new Exception("[Obliczenia HBC] próba liczenia HBC dla boku który nie jest na powierzchni!");

            //oblcizanie wyznazcnika jakobianu
            //z racji ze jestesmy w 1D to jest po proste dlugoscBokuIRL/dlugoscBokuSchemat (zawsze 2)
            this.detJ = Math.Sqrt((n1.x - n2.x) * (n1.x - n2.x) + (n1.y - n2.y) * (n1.y - n2.y)) / 2.0;
            this.HBC = new double[4, 4];

            //iteracja po wszyskich punktach calkowania - liczenie HBC
            for (int k = 0; k < bok.funkcjeKsztaltu.Count; k++)
            {
                var currPkt = bok.funkcjeKsztaltu[k];
                //wymnażanie wektorów - zwykłi i jego transponowany z uwzględnieniem wspolczynnik i alfy
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                        this.HBC[i, j] += currPkt[i] * currPkt[j] * bok.wspolczynniki[k] * alfa * detJ;
                }
            }
        }


    }
}