namespace macierzGlobalna
{
    class macierzGlobalna
    {
        List<List<double>> H_glob;

        public macierzGlobalna(int ilosc_wezlow)
        {
            int wymiar = ilosc_wezlow * 4;
            this.H_glob = new List<List<double>>();

            //robienie macierzy wymiar * wymiar i wypelnienie elementami 0.0
            for (int i = 0; i < wymiar; i++)
            {
                this.H_glob.Add(new List<double>());
                for (int j = 0; j < wymiar; j++)
                {
                    this.H_glob[i].Add(0.0);
                }
            }


        }
    }
}