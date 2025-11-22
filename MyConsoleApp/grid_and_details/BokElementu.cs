using Gauss__schamet_calk;

namespace GridAndDetailsNamespace
{

    //klasa reprezentująca jeden konkretny bok danego elementu
    class BokElementu
    {
        //tablica z wartościami N1, N2,.. dal wszysich punktów całkowania
        //czyli wartości funkcji kształtu we wszyskich punktach całkowania
        public List<List<double>> arr_N { get; }
        private List<double> wezly;
        private List<double> wspolczynniki;

        public BokElementu(schemat_calk schemat, bool osPozioma, double val_os)
        {
            this.wezly = schemat.wezly;
            this.wspolczynniki = schemat.wspolczynniki;
            this.arr_N = new List<List<double>>();

            if (osPozioma)
            {
                foreach (var wezel in wezly)
                    this.arr_N.Add(oblicz_N_dlaPkt(wezel, val_os));
            }
            else
            {
                foreach (var wezel in wezly)
                    this.arr_N.Add(oblicz_N_dlaPkt(val_os, wezel));
            }
        }

        private List<double> oblicz_N_dlaPkt(double ksi, double eta)
        {
            return new List<double>
            {
                0.25 * (1 - ksi) * (1 - eta),
                0.25 * (1 + ksi) * (1 - eta),
                0.25 * (1 + ksi) * (1 + eta),
                0.25 * (1 - ksi) * (1 + eta)
            };
        }

    }
}