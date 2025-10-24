using System.Collections.Generic;
using GridAndDetailsNamespace;
using Gauss__schamet_calk;


namespace jakobianClass
{
    class ElemUniv
    {
        public int npc { get; private set; }
        public List<List<double>> dN_de { get; private set; }
        public List<List<double>> dN_dn { get; private set; }

        //UWAGA! KLASA JEST PISANA TYLKO DLA INPUTU TABLIC 2D!
        //chdzi o to żę przysyłamy tablice z gaussClass tą 2D, trzebaby nadpisac konstruktor by dzialal

        public ElemUniv(schemat_calk gauss)
        {
            this.dN_de = new List<List<double>>();
            this.dN_dn = new List<List<double>>();
            var wezlyAll = gauss.Wezly2D;

            this.npc = wezlyAll.Count * wezlyAll[0].Count;
            //UWAGA NA KOLEJNOSC TUTAJ PRZY TESTACH
            foreach (var wezlyList in wezlyAll)
            {
                foreach (var wezel in wezlyList)
                {
                    dN_de.Add(ksiI(wezel.x));
                    dN_dn.Add(etaI(wezel.y));
                }
            }
        }

        List<double> ksiI(double ksi)
        {
            return new List<double>{
                -0.25 * (1.0 - ksi),
                -0.25 * (1.0 + ksi),
                0.25 * (1 + ksi),
                0.25 * (1 - ksi) };
        }

        List<double> etaI(double eta)
        {
            return new List<double>{
                -0.25 * (1.0 - eta),
                0.25 * (1.0 - eta),
                0.25 * (1 + eta),
                -0.25 * (1 + eta) };
        }
    }

    class Jakobian
    {
        double detJ { get; }
        double[,] J { get; }    //przystosowane do obliczen 2d
        double[,] J1 { get; }

        public Jakobian(Node[] nodes, List<double> dN_de, List<double> dN_dn)
        {
            double dy_dn = 0.0;
            double dy_ds = 0.0;
            double dx_dn = 0.0;
            double dx_ds = 0.0;

            if (nodes == null || nodes.Length < 4)
                throw new Exception("Jakobian - nodes < 4");
            if (dN_de == null || dN_de.Count < 4)
                throw new Exception("Jakobian - dN_de < 4");
            if (dN_dn == null || dN_dn.Count < 4)
                throw new Exception("Jakobian - dN_dn < 4");

            for (int i = 0; i < 4; i++)
            {
                double x = nodes[i].x;
                double y = nodes[i].y;
                dy_dn += y * dN_dn[i];
                dy_ds += y * dN_de[i];
                dx_dn += x * dN_dn[i];
                dx_ds += x * dN_de[i];
            }

            this.J = new double[,] { { dx_ds, dx_dn }, { dy_ds, dy_dn } };
            this.J1 = new double[,] { { dy_dn, -dx_dn }, { -dy_ds, dx_ds } };
            this.detJ = J[0, 0] * J[1, 1] - (J[0, 1] * J[1, 0]);
        }
    }

}