using GlobalDataNamespace;
using GridAndDetailsNamespace;

static class Program
{
    static void Main(string[] args)
    {
        var globalData = new GlobalData("C:\\Users\\mateu\\Desktop\\mes\\Test3_31_31_kwadrat.txt");
        var grid = new Grid(globalData.nN, globalData.nE, globalData.nodesCoord, globalData.elementNodes);
        grid.displayData();
        Console.WriteLine("\n\n SimulationStepTime: " + globalData.SimulationStepTime + "\n");
    }
}