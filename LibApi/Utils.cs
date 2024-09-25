using LibApi.Model;

namespace LibApi;

public static class Utils
{
    public static E TransferData<E, B>(B basic)
    {
        var e = Activator.CreateInstance<E>();
        return TransferData<E, B>(e, basic);
    }
    
    public static E TransferData<E, B>(E to, B from)
    {
        var baseType = typeof(B);
        var entityType = typeof(E);
        var entityProps = entityType.GetProperties();
        foreach (var property in typeof(B).GetProperties())
        {
            var value = property.GetValue(from);
            var curr = entityProps.FirstOrDefault(i => i.Name == property.Name);
            if(curr == null)
                continue;
            
            curr.SetValue(to, value);
        }
        return to;
    }
    
    static int Minimum(int a, int b, int c) => (a = a < b ? a : b) < c ? a : c;
    
    public static int LevenshteinDistance(string firstWord, string secondWord)
    {
        var n = firstWord.Length + 1;
        var m = secondWord.Length + 1;
        var matrixD = new int[n, m];

        const int deletionCost = 1;
        const int insertionCost = 1;

        for (var i = 0; i < n; i++)
        {
            matrixD[i, 0] = i;
        }

        for (var j = 0; j < m; j++)
        {
            matrixD[0, j] = j;
        }

        for (var i = 1; i < n; i++)
        {
            for (var j = 1; j < m; j++)
            {
                var substitutionCost = firstWord[i - 1] == secondWord[j - 1] ? 0 : 1;

                matrixD[i, j] = Minimum(matrixD[i - 1, j] + deletionCost,          // удаление
                    matrixD[i, j - 1] + insertionCost,         // вставка
                    matrixD[i - 1, j - 1] + substitutionCost); // замена
            }
        }

        return matrixD[n - 1, m - 1];
    }
}