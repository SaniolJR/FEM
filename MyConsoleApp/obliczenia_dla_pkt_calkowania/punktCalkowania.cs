using GridAndDetailsNamespace;

namespace Obliczenia_dla_pkt_calkowania
{
    class PktCalkowania
    {
        public double waga1 { get; private set; }
        public double waga2 { get; private set; }
        public double DetJ { get; private set; }
        public double[,] J { get; private set; }
        public double[,] J1 { get; private set; }
        public double[] dN_dX { get; private set; }
        public double[] dN_dY { get; private set; }
        public List<double> dN_dKsi { get; private set; }
        public List<double> dN_dEta { get; private set; }
        public double[,] Hpc { get; private set; }
        public double[,] Cpc { get; private set; }

        public PktCalkowania(double k, List<double> dN_dKsi, List<double> dN_dEta,
                             Node[] wezlyElementu, double w1, double w2, double ksi, double eta)
        {
            this.dN_dKsi = dN_dKsi;
            this.dN_dEta = dN_dEta;
            this.waga1 = w1;
            this.waga2 = w2;

            Jakobian jakobianKlasa = new Jakobian(wezlyElementu, dN_dKsi, dN_dEta);

            this.J = jakobianKlasa.J;
            this.DetJ = jakobianKlasa.DetJ;
            this.J1 = jakobianKlasa.J1;

            Pochodne_WspGlobalne pochodne_WspGlobalne = new Pochodne_WspGlobalne(jakobianKlasa, dN_dKsi, dN_dEta);
            this.dN_dX = pochodne_WspGlobalne.dN_dx;
            this.dN_dY = pochodne_WspGlobalne.dN_dy;

            if (dN_dX == null || dN_dY == null)
                throw new Exception("dN_dX == null || dN_dY == null\n\n");

            //oblcizanie macierzy Hpc
            this.Hpc = new double[4, 4];
            var dNdx_H = WxWT(this.dN_dX);
            var dNdy_H = WxWT(this.dN_dY);

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Hpc[i, j] = k * (dNdx_H[i, j] + dNdy_H[i, j]) * DetJ;
                }
            }

            //obliczanie macierzy Cpc
            //TODO: punkt całkowania musi znac swoje ksi i eta
        }

        private double[,] WxWT(double[] pkt_calk)
        {
            try
            {
                if (pkt_calk.Length < 4)
                    throw new Exception("[obliczanie wektora * wektor transponowany]pkt_calk.Length < 4");
                double[,] res = new double[4, 4];

                for (int i = 0; i < 4; i++)
                {
                    //wsm nie trzeba transponowac ig - przemnazam kazde pole wektora przez jego wszyskie?
                    for (int j = 0; j < 4; j++)
                    {
                        res[i, j] = pkt_calk[i] * pkt_calk[j];
                    }
                }
                return res;
            }
            catch
            {
                throw new Exception("Błąd wywołania WxWT");
            }
        }

        public void displayJ()
        {
            Console.WriteLine($"{this.J[0, 0]:F6} {this.J[0, 1]:F6}");
            Console.WriteLine($"{this.J[1, 0]:F6} {this.J[1, 1]:F6}");
        }

        public void displayPochGlob()
        {
            Console.WriteLine("wWartosc dN/dx rowna sie:");
            foreach (var pochodna in this.dN_dX)
            {
                Console.Write($"{pochodna:F6}\t");
            }
            Console.WriteLine();
            Console.WriteLine("Wartosc dN/dy rowna sie:");
            foreach (var pochodna in this.dN_dY)
            {
                Console.Write($"{pochodna:F6}\t");
            }
            Console.WriteLine("\n");
        }

    }

}