using GlobalDataNamespace;
using GridAndDetailsNamespace;
using Gauss__schamet_calk;
using agregacja_namespace;

static class Program
{
    static void Main(string[] args)
    {

        var globalData = new GlobalData("C:\\Users\\mateu\\Desktop\\studia\\SEMESTR 5\\mes\\Test1_4_4.txt");
        double K = globalData.Conductivity;
        double alfa = globalData.Alfa;
        var BC = globalData.BC;
        var tempOt = globalData.Tot;
        var c = globalData.SpecificHeat;
        var ro = globalData.Density;
        Console.WriteLine("ilosc wezlow: " + globalData.nN);
        Console.WriteLine(K);
        Console.WriteLine(alfa);
        Console.WriteLine(tempOt);
        Console.WriteLine(c);
        Console.WriteLine(ro);

        AgregacjaSingleton HG = AgregacjaSingleton.getInstance(globalData.nN);

        var gauss = new schemat_calk_2pkt();
        gauss.displayPktCalkoania2D();
        var grid = new Grid(globalData.nN, globalData.nE, globalData.nodesCoord,
                             globalData.elementNodes, K, alfa, tempOt, c, ro, BC, gauss);

        grid.displayData();
        AgregacjaSingleton.displayHG();
        AgregacjaSingleton.displayCG();
        AgregacjaSingleton.displayPG();
        AgregacjaSingleton.obliczTemp();
        AgregacjaSingleton.displayT();

        // pobranie danych do liczenia temperatury w czasie
        var instance = AgregacjaSingleton.getInstance(globalData.nN);
        double dt = globalData.SimulationStepTime;

        // z lewej strony macierz A = H + C/dt
        var A = new double[globalData.nN][];
        for (int i = 0; i < globalData.nN; i++)
        {
            A[i] = (double[])instance.HG[i].Clone();
            for (int j = 0; j < globalData.nN; j++)
            {
                A[i][j] += instance.CG[i][j] / dt;
            }
        }

        // inicjalizacja wektora temperatury t0
        double[] t0 = new double[globalData.nN];
        Array.Fill(t0, globalData.InitialTemp);

        // petla po timestep - petla czasu
        for (int s = (int)dt; s <= globalData.SimulationTime; s += (int)dt)
        {
            double[] B = new double[globalData.nN];

            for (int i = 0; i < globalData.nN; i++)
            {
                double tmp = 0;
                for (int j = 0; j < globalData.nN; j++)
                {
                    // (C / dt) * t0
                    tmp += (instance.CG[i][j] / dt) * t0[j];
                }
                // dodanie P do prawej strony
                B[i] = tmp + instance.PG[i];
            }

            /*
            Console.WriteLine($"===== Iteracja {s / dt} =====");
            Console.WriteLine($"===== macierz H + C/dt =====");
            foreach (var row in A)
            {
                foreach (var val in row)
                {
                    // czytelne formatowanie z separatorem tab zamiast przypadkowego dodawania znaku do liczby
                    Console.Write($"{val:F6}\t");
                }
                Console.WriteLine();
            }

            Console.WriteLine($"===== wektor ((C / dt) * t0) + P =====");
            foreach (var val in B)
            {
                Console.Write($"{val:F6}\t");
            }
            Console.WriteLine();
            */

            var t1 = EliminacjaGaussa.wyznaczWektorTemperatury(A, B);

            Console.WriteLine($"Czas {s}: Min {t1.Min():F4} Max {t1.Max():F4}");
            t0 = t1;
        }

    }
}