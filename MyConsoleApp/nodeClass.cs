namespace GridAndDetailsNamespace
{
    class Node
    {
        public double x { get; }
        public double y { get; }
        public Node(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }

    class Element
    {
        public int[] nodes { get; }     //pole publiczne ale tylko do odczytu (metoda get to umożliwia)
        public Element(int[] nodesList)
        {
            this.nodes = nodesList;
        }

    }

    class Grid
    {
        int nN; //liczba nodes
        int nE; //liczba elementów
        Element[] elements;
        Node[] nodes;

        //musi być przekazana lista jakas
        public Grid(int nn, int ne, List<List<double>> nodesList, List<List<int>> elementsWithNodes)
        {
            this.nN = nn;
            this.nE = ne;
            this.elements = new Element[nE];
            this.nodes = new Node[nN];

            //inicjalizacja nodes z listy (lista nie tupla aby zapewnić wiekszą elastyczność)
            for (int i = 0; i < nN; i++)
                nodes[i] = new Node(nodesList[i][0], nodesList[i][1]);      //w razie większej ilości koordów zmienic!!!

            //inicjalizacja elementów - przesyłamy liste nodes dla każdego elementu
            for (int i = 0; i < nE; i++)
                elements[i] = new Element(elementsWithNodes[i].ToArray());
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
                Console.Write($"{i} wierzcholki: ");
                foreach (var n in elements[i].nodes)
                    Console.Write(n + " ");
            }
        }

    }
}
