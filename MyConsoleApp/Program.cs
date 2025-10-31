using GlobalDataNamespace;
using GridAndDetailsNamespace;
using Gauss__schamet_calk;
using System.Runtime.CompilerServices;

static class Program
{
    static void Main(string[] args)
    {

        var globalData = new GlobalData("C:\\Users\\mateu\\Desktop\\studia\\SEMESTR 5\\mes\\Test1_4_4.txt");
        double K = globalData.Conductivity;
        Console.WriteLine(K);
        var gauss = new schemat_calk_2pkt();
        var grid = new Grid(globalData.nN, globalData.nE, globalData.nodesCoord, globalData.elementNodes, K, gauss);

        grid.displayData();
    }
}