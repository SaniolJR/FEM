using Gauss__schamet_calk;

namespace Obliczenia_dla_pkt_calkowania
{
    //klasa do obliczenai pochodnych względem współrzędnych naturalnych gaussa
    //czyli (eta,ksi) -> (x,y)
    class Pochodne_WspLokalne
    {
        //liczba pkt całkowania
        public int npc { get; private set; }
        public List<List<double>> dN_dKsi { get; private set; }
        public List<List<double>> dN_dEta { get; private set; }


        //UWAGA! KLASA JEST PISANA TYLKO DLA INPUTU TABLIC 2D!
        //chdzi o to żę przysyłamy tablice z gaussClass tą 2D
        // trzebaby nadpisac konstruktor by dzialal w 1D

        public Pochodne_WspLokalne(schemat_calk schemat_gaussa)
        {
            this.dN_dKsi = new List<List<double>>();
            this.dN_dEta = new List<List<double>>();
            var wezlyAll = schemat_gaussa.Wezly2D;

            this.npc = wezlyAll.Count * wezlyAll[0].Count;
            //Console.WriteLine("\n\n\n\n");
            foreach (var wezlyList in wezlyAll)
            {
                foreach (var wezel in wezlyList)
                {
                    //Console.Write("ksi: " + wezel.x + " ");     //ksi to to e
                    //Console.WriteLine("eta: " + wezel.y);       //eta to to n
                    dN_dKsi.Add(ksiI(wezel.y));
                    dN_dEta.Add(etaI(wezel.x));
                }
            }
            //Console.WriteLine("\n\n\n\n");
        }

        List<double> ksiI(double eta)
        {
            // -1 / 1 - definuje czy lewo czy prawo od (0,0)
            // -eta / eta   - definuje czy dol czy gora wzgledem (0,0)
            return new List<double>{
                -0.25 * (1.0 - eta),        //lewo  ksi - e
                0.25 * (1.0 - eta),         //prawo
                0.25 * (1 + eta),           //prawo
                -0.25 * (1 + eta) };        //lewo
        }

        List<double> etaI(double ksi)
        {
            //-1 / 1 - definuje czy dol czy gora wzgledem (0,0)
            //-ksi / ksi - definuje czy lewo czy prawo (0,0)
            return new List<double>{
                -0.25 * (1.0 - ksi),    //dol   eta - n
                -0.25 * (1.0 + ksi),    //dol
                0.25 * (1 + ksi),       //gora
                0.25 * (1 - ksi) };     //gora
        }
    }

    class Pochodne_WspGlobalne
    {

        public double[] dN_dx { get; private set; }
        public double[] dN_dy { get; private set; }
        public double[,] Hpc { get; private set; }

        public Pochodne_WspGlobalne(Jakobian jakobian, List<double> dN_dKsi, List<double> dN_dEta)
        {
            var J1 = jakobian.J1;
            var DetJ = jakobian.DetJ;


            //pozyskiwanie dndx i dndy
            this.dN_dx = new double[4];
            this.dN_dy = new double[4];
            for (int i = 0; i < 4; i++)
            {
                this.dN_dx[i] = J1[0, 0] * dN_dKsi[i] + J1[0, 1] * dN_dEta[i];
                this.dN_dy[i] = J1[1, 0] * dN_dKsi[i] + J1[1, 1] * dN_dEta[i];
            }
        }
    }
}