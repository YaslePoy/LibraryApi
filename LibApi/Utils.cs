using LibApi.Model;

namespace LibApi;

public static class Utils
{
    public static E CreateDBEntity<E, B>(B basic) where E : DbEntity
    {
        var e = Activator.CreateInstance<E>();
        var baseType = typeof(B);
        var entityType = typeof(E);
        var entityProps = entityType.GetProperties();
        foreach (var property in typeof(B).GetProperties())
        {
            var value = property.GetValue(basic);
            var curr = entityProps.FirstOrDefault(i => i.Name == property.Name);
            if(curr == null)
                continue;
            
            curr.SetValue(e, value);
        }

        return e;
    }
}