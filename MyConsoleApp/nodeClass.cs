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
        public Node[] nodes { get; }     //pole publiczne ale tylko do odczytu (metoda get to umożliwia)
        public List<Jakobian> jakobianList { get; }  //KAŻDY ELEMENT ZE SCHEMATU CAŁKOWANIA POWINIEN MIEĆ SWÓJ JAKOBIAN!

        //przypisywanie tych nodes z globalnych + przekazujemy juz wczeniej odpowiednie listy dla tego elementu!
        public Element(Node[] allNodes, int[] nodesList, schemat_calk gauss)
        {

            this.nodes = new Node[nodesList.Length];

            for (int idx = 0; idx < nodesList.Length; idx++)
            {
                int nodeIndex = nodesList[idx];
                this.nodes[idx] = allNodes[nodeIndex];
            }
            var elemUni = new ElemUniv(gauss);
            var dN_de = elemUni.dN_de;
            var dN_dn = elemUni.dN_dn;

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
                elements[i] = new Element(this.nodes, elementsWithNodes[i].ToArray());
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
                    Console.Write(n + " ");
            }
        }

    }
}
