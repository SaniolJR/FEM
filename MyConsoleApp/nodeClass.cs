using System;
using System.Collections.Generic;
using Gauss__schamet_calk;
using jakobianClass;

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
        public Node[] nodes { get; }
        public List<Jakobian> jakobianList { get; }  //KAŻDY ELEMENT ZE SCHEMATU CAŁKOWANIA POWINIEN MIEĆ SWÓJ JAKOBIAN!

        //przypisywanie tych nodes z globalnych + przekazujemy juz wczeniej odpowiednie listy dla tego elementu!
        public Element(Node[] allNodes, int[] nodesList, schemat_calk gauss)
        {
            if (nodesList == null || nodesList.Length == 0)
                throw new ArgumentException("ELEMENT CLASS - nodesList is null or empty", nameof(nodesList));
            if (allNodes == null || allNodes.Length == 0)
                throw new ArgumentException("ELEMENT CLASS - allNodes is null or empty", nameof(allNodes));

            // sprawdzenei czy jest 0 based
            this.nodes = new Node[nodesList.Length];
            for (int idx = 0; idx < nodesList.Length; idx++)
            {
                int nodeIndex = nodesList[idx];
                if (nodeIndex < 0 || nodeIndex >= allNodes.Length)
                    throw new IndexOutOfRangeException("Problem z 0-based");
                this.nodes[idx] = allNodes[nodeIndex];
            }

            //obliczanie pochodnych
            var elemUni = new ElemUniv(gauss);
            var dN_de = elemUni.dN_de;
            var dN_dn = elemUni.dN_dn;

            if (dN_de == null || dN_dn == null)
            {
                throw new Exception("dN_de == null || dN_dn == null");
            }

            this.jakobianList = new List<Jakobian>();

            for (int i = 0; i < dN_de.Count; i++)
            {
                jakobianList.Add(new Jakobian(nodes, dN_de[i], dN_dn[i]));
            }

        }

    }

    class Grid
    {
        int nN; //liczba nodes
        int nE; //liczba elementów
        Element[] elements;
        Node[] nodes;

        //musi być przekazana lista jakas
        public Grid(int nn, int ne, List<List<double>> nodesList, List<List<int>> elementsWithNodes, schemat_calk gauss)
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

                elements[i] = new Element(this.nodes, elList.ToArray(), gauss);
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
                Console.WriteLine($"{i} wierzcholki: ");
                foreach (var n in elements[i].nodes)
                    Console.Write($"({n.x}, {n.y}) ");
                Console.WriteLine();

                if (elements[i].jakobianList != null)
                {
                    Console.WriteLine("\n\n**********************************************************");
                    Console.WriteLine("Jakobiany dla Elementu " + i);
                    for (int gp = 0; gp < elements[i].jakobianList.Count; gp++)
                    {
                        var jacobian = elements[i].jakobianList[gp];
                        Console.WriteLine($"\nPunkt calkowania gp={gp + 1}:");

                        jacobian.displayJacobian();
                    }
                    Console.WriteLine("**********************************************************");
                }
            }

        }
    }
}
