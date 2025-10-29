using System;
using System.Collections.Generic;
using System.Linq;
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
            Console.WriteLine("\n\n\n\n");
            foreach (var wezlyList in wezlyAll)
            {
                foreach (var wezel in wezlyList)
                {
                    Console.Write("ksi: " + wezel.x + " ");     //ksi to to e
                    Console.WriteLine("eta: " + wezel.y);       //eta to to n
                    dN_de.Add(ksiI(wezel.y));
                    dN_dn.Add(etaI(wezel.x));
                }
            }

            Console.WriteLine("\n\n\n\n");
        }

        List<double> ksiI(double eta)
        {
            return new List<double>{
                -0.25 * (1.0 - eta),
                0.25 * (1.0 - eta),
                0.25 * (1 + eta),
                -0.25 * (1 + eta) };
        }

        List<double> etaI(double ksi)
        {
            return new List<double>{
                -0.25 * (1.0 - ksi),
                -0.25 * (1.0 + ksi),
                0.25 * (1 + ksi),
                0.25 * (1 - ksi) };
        }
    }

    class Jakobian
    {
        // publiczne właściwości do odczytu z zewnątrz
        public double DetJ { get; private set; }
        public double[,] J { get; private set; }    //przystosowane do obliczen 2d
        public double[,] J1 { get; private set; }

        public double[] dNdx { get; private set; }
        public double[] dNdy { get; private set; }

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

            Console.WriteLine("\n");
            Console.WriteLine("wypisywanie dN_de (ksi)");
            for (int i = 0; i < 4; i++)
                Console.Write($"{dN_de[i]:F6} ");
            Console.WriteLine();
            //ksi zamienione z eta i kolejnosc jest 1, 3, 4, 2
            Console.WriteLine("wypisywanie dN_dn (eta)");
            for (int i = 0; i < 4; i++)
                Console.Write($"{dN_dn[i]:F6} ");
            Console.WriteLine("\n");


            for (int i = 0; i < 4; i++)
            {
                double x = nodes[i].x;
                double y = nodes[i].y;
                dy_dn += y * dN_dn[i];
                dy_ds += y * dN_de[i];
                dx_dn += x * dN_dn[i];
                dx_ds += x * dN_de[i];
            }

            this.J = new double[,] { { dx_ds, dy_ds },
                                     { dx_dn, dy_dn } };
            this.J1 = new double[,] { { dy_dn, -dy_ds },
                                      { -dx_dn, dx_ds } };
            this.DetJ = J[0, 0] * J[1, 1] - (J[0, 1] * J[1, 0]);

            //pozyskiwanie dndx i dndy
            this.dNdx = new double[4];
            this.dNdy = new double[4];
            for (int i = 0; i < 4; i++)
            {
                double a = dN_de[i];
                double b = dN_dn[i];
                this.dNdx[i] = (dy_dn * a - dy_ds * b) / this.DetJ;
                this.dNdy[i] = (-dx_dn * a + dx_ds * b) / this.DetJ;
            }
        }

        public void displayJacobian()
        {
            // dN/dx
            /*
            Console.WriteLine("wartosc dN/dx rowna sie");
            for (int i = 0; i < dNdx.Length; i++)
                Console.Write($"{dNdx[i]:F6}{(i < dNdx.Length - 1 ? ", " : ",")}");
            Console.WriteLine();

            // dN/dy
            Console.WriteLine("wartosc dN/dy rowna sie");
            for (int i = 0; i < dNdy.Length; i++)
                Console.Write($"{dNdy[i]:F6}{(i < dNdy.Length - 1 ? ", " : ",")}");
            Console.WriteLine();
            */

            // macierz J
            Console.WriteLine("\nMacierz Jakobiego dla punktu calkowania");
            Console.WriteLine($"{J[0, 0]:F7} {J[0, 1]:F7}");
            Console.WriteLine($"{J[1, 0]:F7} {J[1, 1]:F7}");

            // detJ
            Console.WriteLine($"DetJ = {DetJ:F9}\n");
        }
    }

}