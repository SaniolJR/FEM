using GlobalDataNamespace;
using GridAndDetailsNamespace;
using Gauss__schamet_calk;
using System.Runtime.CompilerServices;

static class Program
{
    static void Main(string[] args)
    {
        /*
        var globalData = new GlobalData("C:\\Users\\mateu\\Desktop\\mes\\Test3_31_31_kwadrat.txt");
        var grid = new Grid(globalData.nN, globalData.nE, globalData.nodesCoord, globalData.elementNodes);
        grid.displayData();
        Console.WriteLine("\n\n SimulationStepTime: " + globalData.SimulationStepTime + "\n");
        */

        //funkcja 𝑓 𝑥 = 5𝑥2 + 3𝑥 + 6
        double f1(double x) => 5.0 * x * x + 3.0 * x + 6;
        //funkcja 𝑓 𝑥, 𝑦 = 5𝑥2𝑦2 + 3𝑥𝑦 + 6
        double f2(double x, double y) => 5.0 * x * x * y * y + 3.0 * x * y + 6;

        var pkt = new schemat_calk_2pkt();
        var wezly = pkt.Wezly2D;
        for (int i = wezly.Count - 1; i >= 0; i--)
        {
            foreach (var j in wezly[i])
                Console.Write($"{j}\t");
            Console.WriteLine();
        }

        /*
        var pkt2 = new schemat_calk_2pkt();
        var pkt3 = new schemat_calk_3pkt();

        double wynik_1D = pkt2.kalkulacja1D(f1);
        double wynik_2D = pkt2.kalkulacja2D(f2);


        double wynik2_1D = pkt.kalkulacja1D(f1);
        double wynik2_2D = pkt.kalkulacja2D(f2);

        double wynik3_1D = pkt3.kalkulacja1D(f1);
        double wynik3_2D = pkt3.kalkulacja2D(f2);

        Console.WriteLine("Wyniki dla funkcji f(x) = 5𝑥2 + 3𝑥 + 6");
        Console.WriteLine("\t2 punktowy:\t" + wynik2_1D);
        Console.WriteLine("\t3 punktowy:\t" + wynik3_1D);
        Console.WriteLine("Wyniki dla funkcji f(x,y) = 5𝑥2𝑦2 + 3𝑥𝑦 + 6");
        Console.WriteLine("\t2 punktowy:\t" + wynik2_2D);
        Console.WriteLine("\t3 punktowy:\t" + wynik3_2D);
        //wyniki się zgadzają - mathDF
        */
    }
}