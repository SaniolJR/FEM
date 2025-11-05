using Gauss__schamet_calk;

namespace GridAndDetailsNamespace
{
    class Grid
    {
        int nN; //liczba nodes
        int nE; //liczba elementów
        Element[] elements;
        Node[] nodes;

        //musi być przekazana lista jakas
        public Grid(int nn, int ne, List<List<double>> nodesList, List<List<int>> elementsWithNodes, double K, schemat_calk gauss)
        {
            this.nN = nn;
            this.nE = ne;
            this.elements = new Element[nE];
            this.nodes = new Node[nN];

            if (nN <= 0 || nE <= 0)
            {
                throw new Exception("GRID CALSS - nN || nE <= 0");
            }

            // inicjalizacja nodes z listy (lista nie tupla aby zapewnić wiekszą elastyczność)
            for (int i = 0; i < nN; i++)
            {
                if (nodesList[i] == null || nodesList[i].Count < 2)
                    throw new ArgumentException($"nodesList[{i}] does not contain at least two coordinates (x,y)");
                nodes[i] = new Node(nodesList[i][0], nodesList[i][1]);      //w razie większej ilości koordów zmienic!!!
            }

            // inicjalizacja elementów - przesyłamy liste nodes dla każdego elementu
            for (int i = 0; i < nE; i++)
            {
                var elList = elementsWithNodes[i];
                if (elList == null || elList.Count == 0)
                    throw new ArgumentException($"elementsWithNodes[{i}] is null or empty");

                elements[i] = new Element(this.nodes, elList.ToArray(), K, gauss);
            }
        }

        public void displayData()
        {
            Console.WriteLine("Liczba nodes: " + nN);
            Console.WriteLine("Nodes");
            for (int i = 0; i < nN; i++)
                Console.WriteLine($"{i} : X = {nodes[i].x} Y = {nodes[i].y}");

            Console.WriteLine("Liczba elementów: " + nE);
            Console.WriteLine("Elementy");
            for (int i = 0; i < nE; i++)
            {
                elements[i].displayElement();
            }
        }
    }
}
