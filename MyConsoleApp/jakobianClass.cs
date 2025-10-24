using System.Collections.Generic;

namespace jakobianClass
{
    class ElemUniv
    {
        public int npc { get; private set; }
        public List<List<double>> dN_de { get; private set; }
        public List<List<double>> dN_dn { get; private set; }

        //UWAGA! KLASA JEST PISANA TYLKO DLA INPUTU TABLIC 2D!
        //chdzi o to żę przysyłamy tablice z gaussClass tą 2D, trzebaby nadpisac konstruktor by dzialal

        public ElemUniv(int npc, IReadOnlyList<double> gauss_pkt1D)
        {
            this.dN_de = new List<List<double>>();
            this.dN_dn = new List<List<double>>(0); //brak dN - 1 wymiar
            for (int i = 0; i < npc; i++)
            {
                dN_de.Add(new List<double> { -0.5, 0.5 });   //tutaj sa po prostu stale
            }
        }

        public ElemUniv(int npc, IReadOnlyList<IReadOnlyList<(double x, double y)>> gauss_pkt2D)
        {
            this.dN_de = new List<List<double>>();
            this.dN_dn = new List<List<double>>();
            this.npc = npc;

            //pozyskiwanie w odpowiedniej kolejnosci (lewyo->prawo, dół -> gora)
            for (int i = gauss_pkt2D.Count - 1; i >= 0; i--)
            {
                foreach (var wezel in gauss_pkt2D[i])
                {
                    //od razu dodajemy listy zwracane przez funkcje pomocnicze 
                    // zawierające odpowiedno po 4 elementy tak jak w pliku
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

}