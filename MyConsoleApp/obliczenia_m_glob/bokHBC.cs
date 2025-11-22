using GridAndDetailsNamespace;

namespace obliczemia_m_glob_namespace
{
    class bokHBC
    {

        private double detJ;
        public bokHBC(BokElementu bok, Node n1, Node n2)
        {
            if (!bok.boundary)
                throw new Exception("[Obliczenia HBC] próba liczenia HBC dla boku który nie jest na powierzchni!");

            //oblcizanie jakobianu
            //z racji ze jestesmy w 1D to jest po proste dlugoscBokuIRL/dlugoscBokuSchemat (zawsze 2)
            this.detJ = Math.Sqrt((n1.x - n2.x) * (n1.x - n2.x) + (n1.y - n2.y) * (n1.y - n2.y)) / 2.0;


        }


    }
}