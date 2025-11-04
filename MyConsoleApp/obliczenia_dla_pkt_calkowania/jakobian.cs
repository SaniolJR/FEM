using GridAndDetailsNamespace;

namespace Obliczenia_dla_pkt_calkowania
{
    class Jakobian
    {
        public double[,] J { get; private set; }
        public double[,] J1 { get; private set; }
        public double DetJ { get; private set; }

        public Jakobian(Node[] wspWezlow, List<double> dN_dKsi, List<double> dN_dEta)
        {
            //walidacja inputu
            if (wspWezlow == null || wspWezlow.Length < 4)
            {
                throw new Exception("Jakobian - nodes < 4");
            }
            if (dN_dKsi == null || dN_dKsi.Count < 4)
            {
                throw new Exception("Jakobian - dN_de < 4");
            }
            if (dN_dEta == null || dN_dEta.Count < 4)
            {
                throw new Exception("Jakobian - dN_dn < 4");
            }

            double dY_dEta = 0.0;   //jak bardzo zmienia się y jak przesuwasz sie w Eta itp..
            double dY_dKsi = 0.0;   //ksi to to E'
            double dX_dEta = 0.0;   //eta to to n'
            double dX_dKsi = 0.0;

            //obliczanie jakobianu
            for (int i = 0; i < 4; i++)
            {
                double x = wspWezlow[i].x;
                double y = wspWezlow[i].y;
                dY_dEta += y * dN_dEta[i];
                dY_dKsi += y * dN_dKsi[i];
                dX_dEta += x * dN_dEta[i];
                dX_dKsi += x * dN_dKsi[i];
            }

            this.J = new double[,]
            {
                {dX_dKsi,   dY_dKsi},
                {dX_dEta,   dY_dKsi}
            };

            //obliczanie wspolczynnika
            this.DetJ = J[0, 0] * J[1, 1] - (J[0, 1] * J[1, 0]);

            //obliczanie jakobianu odwrotnego
            this.J1 = new double[,]
            {
                {dY_dEta / DetJ,    -dY_dKsi / DetJ},
                {-dX_dEta / DetJ,    dX_dKsi / DetJ}
            };
        }

    }
}