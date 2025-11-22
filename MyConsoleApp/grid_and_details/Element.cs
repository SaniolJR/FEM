using Gauss__schamet_calk;
using Obliczenia_dla_pkt_calkowania;
using obliczemia_m_glob_namespace;

namespace GridAndDetailsNamespace
{

    class Element
    {
        public int[] nodesIDX { get; }
        public Node[] nodes { get; }
        public List<PktCalkowania> punktyCalkowania { get; }
        public double[,] H { get; }
        private List<List<double>> dN_dKSi;
        private List<List<double>> dN_dEta;
        private BokElementu[] boki;

        private double[,] HBC;

        public Element(Node[] allNodes, int[] nodesList, double K, double alfa,
                         schemat_calk kwadratura_gaussa, HashSet<int> BC)
        {

            #region Walidacja inputu
            if (nodesList == null || nodesList.Length == 0)
            {
                throw new ArgumentException("ELEMENT CLASS - nodesList is null or empty", nameof(nodesList));
            }
            if (allNodes == null || allNodes.Length == 0)
            {
                throw new ArgumentException("ELEMENT CLASS - allNodes is null or empty", nameof(allNodes));
            }
            #endregion

            #region inicjalizacja tablicy nodes
            this.nodes = new Node[nodesList.Length];
            this.nodesIDX = nodesList;
            this.boki = new BokElementu[4];      //lewy

            for (int idx = 0; idx < nodesList.Length; idx++)
            {
                int nodeIndex = nodesList[idx];
                if (nodesIDX[idx] >= allNodes.Length)
                {
                    throw new IndexOutOfRangeException("Problem z 0-based");
                }
                //jeśli input jest 1-based to jest blad, bo walidacja juz nastapila
                if (nodeIndex < 0 || nodeIndex >= allNodes.Length)
                {
                    throw new IndexOutOfRangeException("Problem z 0-based");
                }

                this.nodes[idx] = allNodes[nodeIndex];
            }
            #endregion

            #region inicjalizacja boków

            //wg standartu MES:
            boki[0] = new BokElementu(kwadratura_gaussa, true, -1, nodesIDX[0], nodesIDX[1], BC, nodes[0], nodes[1]);        //dolny
            boki[1] = new BokElementu(kwadratura_gaussa, false, 1, nodesIDX[1], nodesIDX[2], BC, nodes[1], nodes[2]);        //prawy
            boki[2] = new BokElementu(kwadratura_gaussa, true, 1, nodesIDX[2], nodesIDX[3], BC, nodes[2], nodes[3]);         //gorny
            boki[3] = new BokElementu(kwadratura_gaussa, false, -1, nodesIDX[3], nodesIDX[0], BC, nodes[3], nodes[0]);
            #endregion

            //obliczanie pochodnych
            var pochodne_WspLokalne = Pochodne_WspLokalne.getInstance(kwadratura_gaussa);
            this.dN_dKSi = pochodne_WspLokalne.dN_dKsi;
            this.dN_dEta = pochodne_WspLokalne.dN_dEta;

            if (dN_dKSi == null || dN_dEta == null)
            {
                throw new Exception("dN_de == null || dN_dn == null");
            }

            this.punktyCalkowania = new List<PktCalkowania>();

            List<(double w1, double w2)> wagi = wagiPunktowTab(kwadratura_gaussa);

            //dN_dKsi.Count jest bezpieczniejsze raczej niż N z pochodne_WspLokalne bo to faktyczna długosc używanej tablicy
            for (int i = 0; i < dN_dKSi.Count; i++)
            {
                punktyCalkowania.Add(new PktCalkowania(K, dN_dKSi[i], dN_dEta[i], nodes, wagi[i].w1, wagi[i].w2));
            }

            this.H = obliczH(kwadratura_gaussa);
            this.HBC = new double[4, 4];
            //liczenie macierzy HBC i dodanie do H
            foreach (var bok in boki)
            {
                if (!bok.boundary)
                    continue;
                Console.WriteLine("obliczanie hbc" + bok);
                if (bok == null)
                    throw new Exception("[obliczH]: bokElementu == null");

                var hbc_bok = bokHBC.obliczHBC(bok, alfa);
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        HBC[i, j] += hbc_bok[i, j];
                        H[i, j] += hbc_bok[i, j];
                    }
                }
            }

        }

        private double[,] obliczH(schemat_calk kwadratura_gaussa)
        {
            double[,] res = new double[4, 4];


            if (this.punktyCalkowania == null || this.punktyCalkowania.Count == 0)
            {
                throw new Exception("[OBLICZ H] JakobianList nie istnieje!");
            }

            int n = punktyCalkowania.Count;

            //dla każdego wiersza trzeba dodać wartosci tego wiersza kazdego jakobiana * wagi
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        var w1 = punktyCalkowania[k].waga1;
                        res[i, j] += punktyCalkowania[k].Hpc[i, j] * punktyCalkowania[k].waga1 * punktyCalkowania[k].waga2;
                    }
                }
            }

            return res;
        }
        //funkcja zwracająca tablice wag, gdzie wagi na index i będzia odpowaidac punkowi i w tabelach pochodnych
        private List<(double, double)> wagiPunktowTab(schemat_calk kwadratura_gaussa)
        {
            var wagi = new List<(double waga_ksi, double waga_eta)>();
            //kolejnosc jak przy punktach calkowania w pochodnych lokalnych
            foreach (var row in kwadratura_gaussa.Wspolczynniki2D)
            {
                foreach (var waga in row)
                {
                    wagi.Add((waga.x, waga.y));
                }
            }

            return wagi;
        }

        public void displayElement()
        {
            //wyswietlanie detJ i J i pochodne w ukłądize globalnym
            for (int i = 0; i < punktyCalkowania.Count; i++)
            {
                Console.WriteLine("\nMacierz jakobiego dla " + i + " pkt całkowania:");
                Console.WriteLine("Wyznaznik J:" + punktyCalkowania[i].DetJ);
                punktyCalkowania[i].displayJ();
                //wyswietlanie pochodnych globalnych
                punktyCalkowania[i].displayPochGlob();
            }

            //macierz H
            Console.WriteLine("-- Wyświetlanie macierzy H --");
            for (int i = 0; i < this.H.GetLength(0); i++)
            {
                for (int j = 0; j < this.H.GetLength(1); j++)
                {
                    Console.Write($"{H[i, j]:F6}\t");
                }
                Console.WriteLine();
            }

            Console.WriteLine("-- Wyświetlanie macierzy HBC --");
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Console.Write($"{HBC[i, j]:F6}\t");
                }
                Console.WriteLine();
            }
        }
    }
}