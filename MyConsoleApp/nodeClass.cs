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
            if (nodesList == null || nodesList.Length == 0)
                throw new ArgumentException("ELEMENT CLASS - nodesList is null or empty", nameof(nodesList));
            if (allNodes == null || allNodes.Length == 0)
                throw new ArgumentException("ELEMENT CLASS - allNodes is null or empty", nameof(allNodes));

            // Do NOT perform automatic 1-based->0-based conversion. Require 0-based indices.
            this.nodes = new Node[nodesList.Length];
            for (int idx = 0; idx < nodesList.Length; idx++)
            {
                int nodeIndex = nodesList[idx];
                if (nodeIndex < 0 || nodeIndex >= allNodes.Length)
                    throw new IndexOutOfRangeException($"Element constructor: node index {nodeIndex} is out of range (0..{allNodes.Length - 1}). Input indices must be 0-based.");
                this.nodes[idx] = allNodes[nodeIndex];
            }
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
            if (nodesList == null)
                throw new ArgumentNullException(nameof(nodesList), "nodesList is null");
            if (nodesList.Count < nN)
                throw new ArgumentException($"nodesList.Count ({nodesList.Count}) is less than expected nN ({nN})", nameof(nodesList));

            // inicjalizacja nodes z listy (lista nie tupla aby zapewnić wiekszą elastyczność)
            for (int i = 0; i < nN; i++)
            {
                if (nodesList[i] == null || nodesList[i].Count < 2)
                    throw new ArgumentException($"nodesList[{i}] does not contain at least two coordinates (x,y)");
                nodes[i] = new Node(nodesList[i][0], nodesList[i][1]);      //w razie większej ilości koordów zmienic!!!
            }

            if (elementsWithNodes == null)
                throw new ArgumentNullException(nameof(elementsWithNodes), "elementsWithNodes is null");
            if (elementsWithNodes.Count < nE)
                throw new ArgumentException($"elementsWithNodes.Count ({elementsWithNodes.Count}) is less than expected nE ({nE})", nameof(elementsWithNodes));

            // inicjalizacja elementów - przesyłamy liste nodes dla każdego elementu
            for (int i = 0; i < nE; i++)
            {
                var elList = elementsWithNodes[i];
                if (elList == null || elList.Count == 0)
                    throw new ArgumentException($"elementsWithNodes[{i}] is null or empty");

                // Detect likely 1-based indices for clearer error message (do NOT convert automatically).
                bool containsZero = false;
                bool containsIndexEqualNN = false; // e.g. index == nN indicates 1-based indexing (last index == nN)
                for (int j = 0; j < elList.Count; j++)
                {
                    int idx = elList[j];
                    if (idx == 0) containsZero = true;
                    if (idx == nN) containsIndexEqualNN = true;
                }

                if (containsIndexEqualNN && !containsZero)
                {
                    throw new IndexOutOfRangeException($"Grid constructor: element {i} appears to use 1-based indices (found index == {nN}).\n" +
                        $"Input indices must be 0-based. Element indices: [{string.Join(", ", elList)}]");
                }

                // Validate indices are 0-based and in range. Do NOT convert.
                for (int j = 0; j < elList.Count; j++)
                {
                    int idx = elList[j];
                    if (idx < 0 || idx >= nN)
                        throw new IndexOutOfRangeException($"Grid constructor: element {i} has node index {idx} out of range (0..{nN - 1}). Input indices must be 0-based.");
                }

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
            }
        }

    }
}
