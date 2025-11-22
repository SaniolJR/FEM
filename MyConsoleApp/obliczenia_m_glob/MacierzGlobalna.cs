using GridAndDetailsNamespace;

namespace obliczemia_m_glob_namespace
{
    //singleton - macierz globalna jest jedna!
    class MacierzGlobalna
    {
        private double[][] HG;
        private static MacierzGlobalna instance;

        private MacierzGlobalna(int N)
        {
            HG = new double[N][];
            for (int i = 0; i < N; i++)
            {
                HG[i] = new double[N];  //powinno inicjalizować zerami!
            }

        }

        public static MacierzGlobalna getInstance(int N)
        {
            if (instance == null)
            {
                instance = new MacierzGlobalna(N);
            }
            return instance;
        }

        public static void HG_dodajElement(Element element)
        {
            if (instance == null || instance.HG == null)
                throw new Exception("[HG_dodajElement]: instance == null || HG == null");
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (element.nodesIDX == null)
                throw new Exception("[HG_dodajElement]: element.nodesIDX == null");
            if (element.H == null)
                throw new Exception("[HG_dodajElement]: element.H == null (H niepoliczone)");

            int n = element.nodesIDX.Length;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    int I = element.nodesIDX[i];
                    int J = element.nodesIDX[j];
                    if (I >= instance.HG.Length || J >= instance.HG.Length)
                        throw new Exception("I >= instance.HG.Length || J >= instance.HG.Length");

                    instance.HG[I][J] += element.H[i, j];
                }
            }
        }

        public static void displayHG()
        {
            if (instance == null || instance.HG == null)
                throw new Exception("[Wyswietlanie HG]: instance == null || instance.HG == null");

            Console.WriteLine("\n\n---- Wyświetlanie macierzy globalnej ----");
            foreach (var row in instance.HG)
            {
                foreach (var val in row)
                    Console.Write($"{val:F6}\t");
                Console.WriteLine();
            }
            Console.WriteLine("\n");
        }

    }
}