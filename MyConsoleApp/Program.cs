using GlobalDataNamespace;
using GridAndDetailsNamespace;
using Gauss__schamet_calk;
using System.Runtime.CompilerServices;

static class Program
{
    static void Main(string[] args)
    {

        var globalData = new GlobalData("C:\\Users\\mateu\\Desktop\\studia\\SEMESTR 5\\mes\\Test1_4_4.txt");
        var gauss = new schemat_calk_2pkt();
        var grid = new Grid(globalData.nN, globalData.nE, globalData.nodesCoord, globalData.elementNodes, gauss);

        int i = 1;
        foreach (var el in grid.elements)
        {
            Console.Write($"Element: {i}: ");
            foreach (var n in el.nodes)
            {
                Console.Write(n.x + " " + n.y + " ");
            }
            Console.WriteLine();
            i++;
        }
        grid.displayData();
    }
}