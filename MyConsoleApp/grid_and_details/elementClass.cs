using Gauss__schamet_calk;
using Obliczenia_dla_pkt_calkowania;

namespace GridAndDetailsNamespace
{

    class Element
    {
        public int[] nodesIDX { get; }
        public Node[] nodes { get; }
        public List<PktCalkowania> punktyCalkowania { get; }
        public double[,] H { get; }

        public Element(Node[] allNodes, int[] nodesList, double K, schemat_calk kwadratura_gaussa)
        {

            if (nodesList == null || nodesList.Length == 0)
            {
                throw new ArgumentException("ELEMENT CLASS - nodesList is null or empty", nameof(nodesList));
            }
            if (allNodes == null || allNodes.Length == 0)
            {
                throw new ArgumentException("ELEMENT CLASS - allNodes is null or empty", nameof(allNodes));
            }


            this.nodes = new Node[nodesList.Length];
            for (int idx = 0; idx < nodesList.Length; idx++)
            {
                int nodeIndex = nodesList[idx];
                //jeśli input jest 1-based to jest blad, bo walidacja juz nastapila
                if (nodeIndex < 0 || nodeIndex >= allNodes.Length)
                {
                    throw new IndexOutOfRangeException("Problem z 0-based");
                }
                this.nodes[idx] = allNodes[nodeIndex];
            }

            //obliczanie pochodnych
            var pochodne_WspLokalne = new Pochodne_WspLokalne(kwadratura_gaussa);
            var dN_dKSi = pochodne_WspLokalne.dN_dKsi;
            var dN_dEta = pochodne_WspLokalne.dN_dEta;

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

        }
    }
}