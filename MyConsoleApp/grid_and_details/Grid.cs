using Gauss__schamet_calk;

namespace GridAndDetailsNamespace
{
    class Grid
    {
        int nN; //liczba nodes
        int nE; //liczba elementów
        Element[] elements;
        Node[] nodes;

        public Grid(int nN, int nE,
                    List<List<double>> nodesList,
                    List<List<int>> elementsWithNodes,
                    double K, double alfa, double tempOt, double c, double ro,
                     HashSet<int> BC, schemat_calk gauss)
        {
            this.nN = nN;
            this.nE = nE;
            this.elements = new Element[this.nE];
            this.nodes = new Node[this.nN];

            if (this.nN <= 0 || this.nE <= 0)
            {
                throw new Exception("GRID CALSS - nN || nE <= 0");
            }

            #region inicjalizacja nodes z listy
            for (int i = 0; i < this.nN; i++)
            {
                if (nodesList[i] == null || nodesList[i].Count < 2)
                    throw new ArgumentException($"nodesList[{i}] ma inny wymiar niz (x,y)");
                nodes[i] = new Node(nodesList[i][0], nodesList[i][1]);      //w razie większej ilości koordów zmienic!!!
            }
            #endregion

            #region inicjalizacja elementów - przesyłamy liste nodes dla każdego elementu
            for (int i = 0; i < this.nE; i++)
            {
                var elList = elementsWithNodes[i];

                if (elList == null || elList.Count == 0)
                    throw new ArgumentException($"elementsWithNodes[{i}] is null or empty");

                elements[i] = new Element(this.nodes, elList.ToArray(), K, alfa, tempOt, c, ro, gauss, BC);

                agregacja_namespace.AgregacjaSingleton.dodajElement(elements[i]);

            }
            #endregion
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
                Console.WriteLine($"\n\n----- Element {i} -----");
                elements[i].displayElement();
            }
        }
    }
}
