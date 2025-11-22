using GridAndDetailsNamespace;

namespace obliczemia_m_glob_namespace
{
    class bokHBC
    {

        static public double[,] obliczHBC(BokElementu bok, double alfa)
        {
            if (bok == null)
                throw new Exception("[obliczHBC]: bok == null");
            if (alfa == 0)
                throw new Exception("[obliczHBC]: alfa == 0");

            if (!bok.boundary)
                throw new Exception("[Obliczenia HBC] próba liczenia HBC dla boku który nie jest na powierzchni!");

            //oblcizanie wyznazcnika jakobianu
            //z racji ze jestesmy w 1D to jest po proste dlugoscBokuIRL/dlugoscBokuSchemat (zawsze 2)
            Node n1 = bok.node1;
            Node n2 = bok.node2;
            if (n1 == null || n2 == null)
                throw new Exception("[Obliczenia HBC] n1 == null || n2 == null");

            double detJ = Math.Sqrt((n1.x - n2.x) * (n1.x - n2.x) + (n1.y - n2.y) * (n1.y - n2.y)) / 2.0;
            double[,] HBC = new double[4, 4];

            //iteracja po wszyskich punktach calkowania - liczenie HBC
            for (int k = 0; k < bok.funkcjeKsztaltu.Count; k++)
            {
                var currPkt = bok.funkcjeKsztaltu[k];
                //wymnażanie wektorów - zwykłi i jego transponowany z uwzględnieniem wspolczynnik i alfy
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                        HBC[i, j] += currPkt[i] * currPkt[j] * bok.wspolczynniki[k] * alfa * detJ;
                }
            }
            return HBC;
        }
    }
}