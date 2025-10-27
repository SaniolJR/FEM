namespace Gauss__schamet_calk
{

    public abstract class schemat_calk
    {
        protected List<double> wezly;
        protected List<double> wspolczynniki;

        protected List<List<(double, double)>> wezly2D;
        protected List<List<(double, double)>> wspolczynniki2D;

        public int N { get; protected set; }

        //ustawienie możliwości TYLKO DO ODCZYTU!
        public IReadOnlyList<double> Wezly => wezly;
        public IReadOnlyList<double> Wspolczynniki => wspolczynniki;

        //2D wiec musi byc bardziej skomplikowane, aby każda lista wewnetrzna była readonly
        //-------------------------------------------------wezły2D to punkty całkowania w 2D 
        public IReadOnlyList<IReadOnlyList<(double x, double y)>> Wezly2D =>
                            wezly2D == null ? null :
                            wezly2D.Select(r => (IReadOnlyList<(double x, double y)>)r.AsReadOnly()).ToList().AsReadOnly();
        public IReadOnlyList<IReadOnlyList<(double x, double y)>> Wspolczynniki2D =>
                            wspolczynniki2D == null ? null :
                            wspolczynniki2D.Select(r => (IReadOnlyList<(double x, double y)>)r.AsReadOnly()).ToList().AsReadOnly();


        //metoda pomocnicza inicjalizująca odpowiedniej wielkosci "plansze 2D zgodnie ze schamatem"
        protected void build2D()
        {
            if (N <= 0)
                throw new InvalidOperationException("[schemat_calk class]: N jest niezainicjalizowane lub niepoprawne");
            if (wezly == null || wezly.Count < N)
                throw new InvalidOperationException("[schemat_calk]: wezly zle zainicjalizowane");
            if (wspolczynniki == null || wspolczynniki.Count < N)
                throw new InvalidOperationException("[schemat_calk]: wspolczynniki zle zainicjalizowane");


            this.wezly2D = new List<List<(double x, double y)>>();
            this.wspolczynniki2D = new List<List<(double x, double y)>>();

            //robiona tak aby łatwo było sie odnalezc powtorzonym   for(int i = 0; i <n; i++)
            for (int j = 0; j < N; j++)
            {

                var rowWezly = new List<(double x, double y)>(N);
                var rowWagi = new List<(double x, double y)>(N);

                for (int i = 0; i < N; i++)
                {
                    rowWezly.Add((wezly[i], wezly[j]));
                    rowWagi.Add((wspolczynniki[i], wspolczynniki[j]));
                }
                wezly2D.Add(rowWezly);
                wspolczynniki2D.Add(rowWagi);
            }
        }

        public double kalkulacja1D(Func<double, double> f)
        {
            double res = 0;
            for (int i = 0; i < N; i++)
            {
                res += f(wezly[i]) * wspolczynniki[i];
            }
            return res;
        }

        public double kalkulacja2D(Func<double, double, double> f)
        {
            double res = 0;
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    res += f(wezly2D[i][j].Item1, wezly2D[i][j].Item2) * wspolczynniki2D[i][j].Item1 * wspolczynniki2D[i][j].Item2;
                }
            }
            return res;
        }
    }

    public class schemat_calk_2pkt : schemat_calk
    {
        public schemat_calk_2pkt()
        {
            this.wezly = new List<double> { -1.0 / Math.Sqrt(3), 1.0 / Math.Sqrt(3) };
            this.wspolczynniki = new List<double> { 1.0, 1.0 };
            this.N = wezly.Count;
            build2D();
        }
    }


    public class schemat_calk_3pkt : schemat_calk
    {
        public schemat_calk_3pkt()
        {
            this.wezly = new List<double> { -1 * Math.Sqrt(3.0 / 5.0), 0.0, Math.Sqrt(3.0 / 5.0) };
            this.wspolczynniki = new List<double> { 5.0 / 9.0, 8.0 / 9.0, 5.0 / 9.0 };
            this.N = wezly.Count;
            build2D();
        }
    }

    public class schemat_calk_4pkt : schemat_calk
    {
        public schemat_calk_4pkt()
        {
            this.wezly = new List<double> { -0.861136, -0.339981, 0.339981, 0.861136 };
            this.wspolczynniki = new List<double> { 0.347855, 0.652145, 0.652145, 0.347855 };
            this.N = wezly.Count;
            build2D();
        }


    }
}