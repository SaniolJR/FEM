using GlobalDataNamespace;
using GridAndDetailsNamespace;
using Gauss__schamet_calk;
using System.Runtime.CompilerServices;

static class Program
{
    static void Main(string[] args)
    {

        var globalData = new GlobalData("C:\\Users\\mateu\\Desktop\\studia\\SEMESTR 5\\mes\\Test2_4_4_MixGrid.txt");
        var gauss = new schemat_calk_2pkt();
        var grid = new Grid(globalData.nN, globalData.nE, globalData.nodesCoord, globalData.elementNodes, gauss);
        grid.displayData();
        Console.WriteLine("\n\n SimulationStepTime: " + globalData.SimulationStepTime + "\n");


    }
}