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
    
}