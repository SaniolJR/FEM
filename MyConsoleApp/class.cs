public abstract class schemat_calk
{
    protected List<double> wezly;
    protected List<double> wspolczynniki;

    protected List<List<(double, double)>> wezly2D;
    protected List<List<(double, double)>> wspolczynniki2D;

    protected int N;

    //ustawienie możliwości TYLKO DO ODCZYTU!
    public IReadOnlyList<double> Wezly => wezly;
    public IReadOnlyList<double> Wspolczynniki => wspolczynniki;

    //2D wiec musi byc bardziej skomplikowane, aby każda lista wewnetrzna była readonly
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

        this.wezly2D = new List<List<(double, double)>>();
        this.wspolczynniki2D = new List<List<(double, double)>>();
        for (int i = 0; i < N; i++)
        {
            wezly2D.Add(new List<(double x, double y)>());
            wspolczynniki2D.Add(new List<(double x, double y)>());

            for (int j = N - 1; j >= 0; j--)
            {
                wezly2D[i].Add((wezly[i], wezly[j]));   //x = wezly[i] y = wezly[j] (od tylu jest j)
                wspolczynniki2D[i].Add((wspolczynniki[i], wspolczynniki[j]));   //analogicznie
            }
        }
    }

}

public class schemat_calk_1pkt : schemat_calk
{
    public schemat_calk_1pkt()
    {
        this.N = 2;
        this.wezly = new List<double> { -1.0 / Math.Sqrt(3), 1.0 / Math.Sqrt(3) };
        this.wspolczynniki = new List<double> { 1.0, 1.0 };
        build2D();
    }
}


public class schemat_calk_2pkt : schemat_calk
{
    public schemat_calk_2pkt()
    {
        this.N = 3;
        this.wezly = new List<double> { -1 * Math.Sqrt(3.0 / 5.0), 0.0, Math.Sqrt(3.0 / 5.0) };
        this.wspolczynniki = new List<double> { 5.0 / 9.0, 8.0 / 9.0, 5.0 / 9.0 };
        build2D();
    }
}

public class schemat_calk_3pkt : schemat_calk
{
    public schemat_calk_3pkt()
    {
        this.N = 4;
        this.wezly = new List<double> { -0.861136, -0.339981, 0.339981, 0.861136 };
        this.wspolczynniki = new List<double> { 0.347855, 0.652145, 0.652145, 0.347855 };
        build2D();
    }


}