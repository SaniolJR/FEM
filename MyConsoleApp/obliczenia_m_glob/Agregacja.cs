using GridAndDetailsNamespace;

namespace agregacja_namespace
{
    //singleton - macierz globalna jest jedna!
    class AgregacjaSingleton
    {
        public double[][] HG { get; protected set; }
        public double[][] CG { get; protected set; }
        public double[] PG { get; protected set; }
        public double[] T { get; protected set; }
        private static AgregacjaSingleton instance;

        private AgregacjaSingleton(int N)
        {
            HG = new double[N][];
            CG = new double[N][];
            for (int i = 0; i < N; i++)
            {
                HG[i] = new double[N];  //powinno inicjalizować zerami!
                CG[i] = new double[N];
            }
            PG = new double[N];
            T = new double[N];
        }

        public static AgregacjaSingleton getInstance(int N)
        {
            if (instance == null)
            {
                instance = new AgregacjaSingleton(N);
            }
            return instance;
        }

        public static void dodajElement(Element element)
        {
            if (instance == null || instance.HG == null)
                throw new Exception("[HG_dodajElement]: instance == null || HG == null");
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (element.nodesIDX == null)
                throw new Exception("[HG_dodajElement]: element.nodesIDX == null");
            if (element.H == null)
                throw new Exception("[HG_dodajElement]: element.H == null (H niepoliczone)");

            //agregacja do HG i CG jednoczesnie
            int n = element.nodesIDX.Length;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    int I = element.nodesIDX[i];
                    int J = element.nodesIDX[j];
                    if (I >= instance.HG.Length || J >= instance.HG.Length)
                        throw new Exception("I >= instance.HG.Length || J >= instance.HG.Length");
                    if (I >= instance.CG.Length || J >= instance.CG.Length)
                        throw new Exception("I >= instance.CG.Length || J >= instance.CG.Length");

                    instance.HG[I][J] += element.H[i, j];
                    instance.CG[I][J] += element.C[i, j];
                }
            }
            //agregacja do PG
            for (int i = 0; i < n; i++)
            {
                int I = element.nodesIDX[i];
                instance.PG[I] += element.P[i];
            }
        }

        public static void obliczTemp()
        {
            if (instance == null || instance.HG == null || instance.PG == null)
                throw new Exception("[HG_dodajElement]: instance == null || HG == null || instance.PG == null");
            instance.T = EliminacjaGaussa.wyznaczWektorTemperatury(instance.HG, instance.PG);
        }
        public static void displayHG()
        {
            if (instance == null || instance.HG == null)
                throw new Exception("[Wyswietlanie HG]: instance == null || instance.HG == null");

            Console.WriteLine("\n\n---- Wyświetlanie macierzy globalnej H ----");
            foreach (var row in instance.HG)
            {
                foreach (var val in row)
                    Console.Write($"{val:F4}\t");
                Console.WriteLine();
            }
            Console.WriteLine("\n");
        }


        public static void displayCG()
        {
            if (instance == null || instance.CG == null)
                throw new Exception("[Wyswietlanie CG]: instance == null || instance.CG == null");

            Console.WriteLine("\n\n---- Wyświetlanie macierzy globalnej C ----");
            foreach (var row in instance.CG)
            {
                foreach (var val in row)
                    Console.Write($"{val:F4}\t");
                Console.WriteLine();
            }
            Console.WriteLine("\n");
        }

        public static void displayPG()
        {
            if (instance == null || instance.PG == null)
                throw new Exception("[Wyswietlanie PG]: instance == null || instance.PG == null");

            Console.WriteLine("\n\n---- Wyświetlanie wektora P globalnego ----");
            foreach (var val in instance.PG)
            {
                Console.Write($"{val:F4}\t");
            }
            Console.WriteLine("\n");
        }

        public static void displayT()
        {
            if (instance == null || instance.T == null)
                throw new Exception("[Wyswietlanie T]: instance == null || instance.T == null");

            Console.WriteLine("\n\n---- Wyświetlanie wektora Temperatury ----");
            foreach (var val in instance.T)
            {
                Console.Write($"{val:F4}\t");
            }
            Console.WriteLine("\n");
        }

        public static void displayTforNode(int idx)
        {
            if (instance == null || instance.T == null)
                throw new Exception("[Wyswietlanie T dla elementu {idx}]: instance == null || instance.T == null");
            if (instance.T.Length <= idx)
                throw new Exception($"[Wyswietlanie T dla elementu {idx}]: instance.T.Length <= idx");
            Console.WriteLine($"Temperatura węzła {idx}: {instance.T[idx]:F4}\t");
        }
    }
}