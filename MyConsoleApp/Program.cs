using GlobalDataNamespace;
using GridAndDetailsNamespace;
using Gauss__schamet_calk;

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
        var schemat = new schemat_calk_1pkt();
        var wspolczynniki = schemat.Wspolczynniki2D;
        var wezly = schemat.Wezly2D;
        int n = schemat.N;

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
                Console.Write(wspolczynniki[i][j].x + " " + wspolczynniki[i][j].y + "\t");

            Console.WriteLine("\n");
        }

        Console.WriteLine("\n\nWEZLY NIZEJ\n\n");
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
                Console.Write(wezly[i][j].x + " " + wezly[i][j].y + "\t");

            Console.WriteLine("\n");
        }
    }
}