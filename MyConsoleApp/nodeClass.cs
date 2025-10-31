using System;
using System.Collections.Generic;
using System.Net.WebSockets;
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
        public double[,] H { get; }

        //przypisywanie tych nodes z globalnych + przekazujemy juz wczeniej odpowiednie listy dla tego elementu!
        public Element(Node[] allNodes, int[] nodesList, double K, schemat_calk gauss)
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
                jakobianList.Add(new Jakobian(nodes, dN_de[i], dN_dn[i], K));
            }

            this.H = obliczH(gauss);
        }

        private double[,] obliczH(schemat_calk gauss)
        {
            double[,] res = new double[4, 4];
            //parsowanie wag, tak aby sledziły punkty całkowania:
            var wagi = new List<(double waga_ksi, double waga_eta)>();
            //dodawanie kolejno
            foreach (var row in gauss.Wspolczynniki2D)
            {
                foreach (var waga in row)
                {
                    wagi.Add((waga.x, waga.y));
                }
            }

            if (this.jakobianList == null || this.jakobianList.Count == 0)
            {
                throw new Exception("[OBLICZ H] JakobianList nie istnieje!");
            }
            if (this.jakobianList.Count != wagi.Count)
            {
                throw new Exception("[OBLICZ H]: Jakobianów jest inna liczba niż par wag");

            }
            int n = jakobianList.Count;

            //dla każdego wiersza trzeba dodać wartosci tego wiersza kazdego jakobiana * wagi
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        // Use the local 'res' array and index into the wagi list.
                        // TODO: replace the '1.0' multiplier with the actual jacobian/integrand contribution.
                        res[i, j] += jakobianList[k].Hpc[i, j] * wagi[k].waga_ksi * wagi[k].waga_eta;
                    }
                }
            }
            return res;
        }

        public void displayElement()
        {
            foreach (var n in this.nodes)
                Console.Write($"({n.x}, {n.y}) ");
            Console.WriteLine();
            //wypisywanie jakobianow
            if (this.jakobianList == null)
            {
                throw new Exception("[Display Element]: jakobianList == null");
            }
            Console.WriteLine("\n\n**********************************************************");
            Console.WriteLine("Jakobiany:");
            for (int gp = 0; gp < this.jakobianList.Count; gp++)
            {
                var jacobian = this.jakobianList[gp];
                Console.WriteLine($"\nPunkt calkowania gp={gp + 1}:");

                jacobian.displayJacobian();
            }
            Console.WriteLine("**********************************************************");

            //wypisywanie macierzy H
            Console.WriteLine("****************MACIERZ H***************");
            if (this.H == null)
            {
                throw new Exception("[Display Element]: H == null");
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                    Console.Write($"{H[i, j]:F6} \t");
                Console.WriteLine();
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
