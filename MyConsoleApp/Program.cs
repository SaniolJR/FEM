using GlobalDataNamespace;
using GridAndDetailsNamespace;
using Gauss__schamet_calk;
using agregacja_namespace;

static class Program
{
    static void Main(string[] args)
    {

        var globalData = new GlobalData("C:\\Users\\mateu\\Desktop\\studia\\SEMESTR 5\\mes\\Test2_4_4_MixGrid.txt");
        double K = globalData.Conductivity;
        double alfa = globalData.Alfa;
        var BC = globalData.BC;
        var tempOt = globalData.Tot;
        Console.WriteLine("ilosc wezlow: " + globalData.nN);
        Console.WriteLine(K);
        Console.WriteLine(alfa);
        Console.WriteLine(tempOt);

        AgregacjaSingleton HG = AgregacjaSingleton.getInstance(globalData.nN);

        var gauss = new schemat_calk_2pkt();
        gauss.displayPktCalkoania2D();
        var grid = new Grid(globalData.nN, globalData.nE, globalData.nodesCoord,
                             globalData.elementNodes, K, alfa, tempOt, BC, gauss);

        grid.displayData();
        AgregacjaSingleton.displayHG();
        AgregacjaSingleton.displayPG();
        AgregacjaSingleton.obliczTemp();
        AgregacjaSingleton.displayT();

        //wymuszone dla każdego elementu na podstaiwe jego indexu, hashset dla optymalizacji i braku podwojnych wynikow
        var hashOfNodes = new HashSet<int>();
        foreach (var r in globalData.elementNodes)
        {
            foreach (var idx in r)
            {
                hashOfNodes.Add(idx);
            }
        }
        foreach (var idx in hashOfNodes)
            AgregacjaSingleton.displayTforNode(idx);
    }
}