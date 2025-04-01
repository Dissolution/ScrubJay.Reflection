namespace ScrubJay.Reflection.Utilities;

public static class AssemblyHelper
{
    private static readonly Lazy<IReadOnlyCollection<Type>> _lazyAssemblyTypes = new(LoadAssemblyTypes);
        
    public static IReadOnlyCollection<Type> AllAssemblyTypes => _lazyAssemblyTypes.Value;
    
    private static IReadOnlyCollection<Type> LoadAssemblyTypes()
    {
        HashSet<Type> allTypes = [];
        
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int a = 0; a < assemblies.Length; a++)
        {
            Type[] types;
            try
            {
                types = assemblies[a].GetTypes();
            }
            catch (Exception)
            {
                continue;
            }

            for (var t = 0; t < types.Length; t++)
            {
                try
                {
                    allTypes.Add(types[t]);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
        return allTypes;
    }
}