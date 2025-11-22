using System.Text;
using System.Globalization;
using GridAndDetailsNamespace;

namespace GlobalDataNamespace
{
    class GlobalData
    {
        public int SimulationTime { get; }
        public int SimulationStepTime { get; }
        public int Conductivity { get; }
        public int Alfa { get; }
        public int Tot { get; }
        public int InitialTemp { get; }
        public int Density { get; }
        public int SpecificHeat { get; }
        public int nN { get; }
        public int nE { get; }
        public List<List<double>> nodesCoord { get; }
        public List<List<int>> elementNodes { get; }
        public HashSet<int> BC { get; }

        //konstruktur zczytuje od razu dane i je zapisuje na podstawie linku do pliku
        public GlobalData(string URL)
        {
            //pozyskanie enumeratora (obiekt pozwalający sekwencyjnie przechodzić po kolekcji)
            var enumerator = File.ReadLines(URL).GetEnumerator();
            enumerator.MoveNext();
            //czytanie linia po linii
            this.SimulationTime = getParam("SimulationTime", enumerator);
            this.SimulationStepTime = getParam("SimulationStepTime", enumerator);
            this.Conductivity = getParam("Conductivity", enumerator);
            this.Alfa = getParam("Alfa", enumerator);
            this.Tot = getParam("Tot", enumerator);
            this.InitialTemp = getParam("InitialTemp", enumerator);
            this.Density = getParam("Density", enumerator);
            this.SpecificHeat = getParam("SpecificHeat", enumerator);
            this.nN = getParam("Nodes number", enumerator);
            this.nE = getParam("Elements number", enumerator);

            if (enumerator.Current.ToString().IndexOf("*Node") == -1)
                throw new Exception($"Bledne wczytanie, oznaczenie *Node jest w innym miejscu");
            else
                enumerator.MoveNext();

            this.nodesCoord = new List<List<double>>();
            int i = 0;
            //teraz jestesmy linie pod oznaczneiem *Node (powinnismy byc)
            while (true)
            {
                string line = enumerator.Current.ToString();
                //koniec jak wejdziemy na linie z oznaczeniem *Element
                if (line.IndexOf("*Element") != -1)
                    break;
                nodesCoord.Add(new List<double>());
                getNumsToList<double>(nodesCoord, "NODES", 2, line, i, s => double.Parse(s, CultureInfo.InvariantCulture));

                i++;
                if (!enumerator.MoveNext())
                    break;
            }

            //teraz powinniśmy byc na *Element - skipujemy
            enumerator.MoveNext();
            //i analogicznie jak dla nodes ładujemy dane
            this.elementNodes = new List<List<int>>();
            i = 0;


            while (true)
            {
                string line = enumerator.Current.ToString();
                //koniec jak wejdziemy na linie z oznaczeniem *BC
                if (line.IndexOf("*BC") != -1)
                {
                    try
                    {
                        if (!enumerator.MoveNext())
                            throw new Exception("Brak danych po oznaczeniu *BC.");

                        this.BC = new HashSet<int>(enumerator.Current.Split(',').Select(s => int.Parse(s) - 1).ToList());
                    }
                    catch
                    {
                        throw new Exception("Wczytywanie danych - problem z wczytaniem BC");
                    }
                    break;
                }

                // przygotuj miejsce dla wiersza elementu
                if (elementNodes.Count <= i)
                    elementNodes.Add(new List<int>());
                //ręczne parsowanie aby pasowało do kultury systemu (problemy z . a ,)
                getNumsToList<int>(elementNodes, "ELEMENTS", 4, line, i,
                                     s => int.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture));

                // normalizacja z 1-based do 0-based
                // Zakładamy, że getNumsToList dodał właśnie `nums` elementów do elementNodes[i]
                for (int k = 0; k < elementNodes[i].Count; k++)
                {
                    elementNodes[i][k] = elementNodes[i][k] - 1;
                }

                i++;
                if (!enumerator.MoveNext())
                    break;
            }

        }

        private int getParam(string param, IEnumerator<string> e)
        {
            string line = e.Current;
            int idx = line.IndexOf(param);
            if (idx == -1)
            {
                throw new Exception($"parametr o nazwie {param} nie znajduje się w tej linii \n linia:\n{line}");
            }
            idx += param.Length;
            //idx wychodzi poza spacje i wskazuje na początek liczby
            while (idx < line.Length && line[idx] == ' ')
                idx++;
            // szukaj końca liczby
            int endIdx = idx;
            while (endIdx < line.Length && !char.IsWhiteSpace(line[endIdx]))
                endIdx++;

            //iteracja linii w pliku - emurator to obj ref
            e.MoveNext();
            if (endIdx < idx)
                throw new Exception("[getParam]: end idx > idx");

            string numberStr = line.Substring(idx, endIdx - idx);
            return int.Parse(numberStr);
        }

        private void getNumsToList<T>(List<List<T>> list, string err, int nums, string line, int i, Func<string, T> parseFunc)
        {
            //dzieli linie na tablice, gdzie każda komórka to element miedzy przecinkami
            var tokens = line.Split(',');
            if (tokens.Length < nums + 1)
                throw new Exception($"[{err}]: oczekiwano {nums}, a jest {Math.Max(0, tokens.Length - 1)} w linii pliku: '{line}'");

            // upewnienie sie że są poprzednie elementy listy
            while (list.Count <= i)
                list.Add(new List<T>());

            for (int k = 0; k < nums; k++)
            {
                string token = tokens[k + 1].Trim(); //pomijamy pierwszy element bo to index
                try
                {
                    // jeśli jest problem z kulturą przy parsowaniu do double czy int i reszty wsm
                    if (typeof(T) == typeof(double))
                    {
                        //wymuszenie kultury neutralnej gdzie separatorem dziesiętnym jest KROPKA a tysięcznym PRZECINEK!!!
                        var d = (T)(object)double.Parse(token, CultureInfo.InvariantCulture);
                        list[i].Add(d);
                    }
                    else if (typeof(T) == typeof(int))
                    {
                        var parsed = int.Parse(token, NumberStyles.Integer, CultureInfo.InvariantCulture);
                        list[i].Add((T)(object)parsed);
                    }
                    else
                    {
                        //jak to nie jest ani int ani double
                        list[i].Add(parseFunc(token));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"[{err}] Problem przy parsowaniu tokenu '{token}' na {typeof(T).Name}: {ex.Message}");
                }
            }

        }

    }
}