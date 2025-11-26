using obliczemia_m_glob_namespace;

public class EliminacjaGaussa
{
    public static double[] wyznaczWektorTemperatury(double[][] HG, double[] PG)
    {
        int N = PG.Length;
        double[] temperatury = new double[N];
        //podstaiwenie do kopii aby nie działać na macierzach orginalnych
        var A = (double[][])HG.Clone();
        var B = (double[])PG.Clone();

        //metoda eliminacji Gaussa - tworzenie macierzy trójkątnej z H
        //k - kolumna w której aktualnie liczymy - pivot
        for (int k = 0; k < N - 1; k++)
        {
            //iteracja po wierszach poniżej przekątnej - zerujemy te elementy
            for (int i = k + 1; i < N; i++)
            {
                if (Math.Abs(A[k][k]) < 1e-12)
                    throw new Exception("Eliminacja Gaussa - Zerowy element na przekątnej w wierszu");
                //wspołczynnii, czyli przez co trzeba pomnożyć element nad aktualnym elementem by wyzerować aktualny
                double factor = A[i][k] / A[k][k];
                //odejmowanie - cały wiersz górny od aktualnego
                for (int j = k; j < N; j++)
                {
                    A[i][j] -= factor * A[k][j];
                }
                //to samo dla wektora B - rozszerzona macierz układu
                B[i] -= factor * B[k];
            }
        }


        //podstawienie wsteczne
        for (int i = N - 1; i >= 0; i--)
        {
            double sum = 0;
            //obliczanie sumy wszyskich znanych wyrazów macierzy A 
            //wraz z przeniesieniem na 2ga strone rownania
            for (int j = i + 1; j < N; j++)
            {
                sum += A[i][j] * temperatury[j];
            }
            //wyliczanie temperatury na podstawie podstawienia
            temperatury[i] = (B[i] - sum) / A[i][i];
        }

        return temperatury;
    }
}