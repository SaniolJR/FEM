using System.Collections.Generic;

namespace jakobianClass
{
    class ElemUniv
    {
        public List<List<double>> dN_de { get; private set; }
        public List<List<double>> dN_dn { get; private set; }

        //UWAGA! KLASA JEST PISANA TYLKO DLA INPUTU TABLIC 2D!
        public ElemUniv(int npc, IReadOnlyList<IReadOnlyList<(double x, double y)>> gauss_pkt)
        {
            this.dN_de = new List<List<double>>();
            this.dN_dn = new List<List<double>>();

            //pozyskiwanie w odpowiedniej kolejnosci (lewyo->prawo, dół -> gora)
            for (int i = gauss_pkt.Count - 1; i >= 0; i--)
            {
                foreach (var j in gauss_pkt[i])
                {
                    //od razu dodajemy listy zwracane przez funkcje pomocnicze 
                    // zawierające odpowiedno po 4 elementy tak jak w pliku
                    dN_de.Add(ksiI(j.x));
                    dN_dn.Add(etaI(j.y));
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

}