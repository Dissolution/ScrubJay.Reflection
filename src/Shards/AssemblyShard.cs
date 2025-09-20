
namespace ScrubJay.Reflection.Shards;

[PublicAPI]
public record class AssemblyShard
{
    public static Result<AssemblyShard> TryCreate(Assembly assembly)
    {
        if (assembly is null)
            return new ArgumentNullException(nameof(assembly));
        
        return Ok(new AssemblyShard(assembly));
        
        
    }


    
    private readonly Assembly _assembly;
    private Attribute[]? _attributes;
    private string? _author;
    private string? _company;
    private string? _configuration;
    private string? _copyright;
    private string? _culture;
    private string? _description;
    private Dictionary<string, string?>? _metadata;
    private string? _name;
    private string? _product;
    private string? _title;
    private string? _trademark;
    private string? _version;
    
    
    public Assembly Assembly => _assembly;

    public Attribute[] Attributes => _attributes ??= Attribute.GetCustomAttributes(_assembly);
    
    public string Company => _company ??= GetAttr<AssemblyCompanyAttribute, string>(a => a.Company);

    public string Configuration => _configuration ??= GetAttr<AssemblyConfigurationAttribute, string>(a => a.Configuration);
    
    public string Copyright => _copyright ??= GetAttr<AssemblyCopyrightAttribute, string>(a => a.Copyright);
    
    public string Culture => _culture ??= GetAttr<AssemblyCultureAttribute, string>(a => a.Culture);
    
    public string Description => _description ??= GetAttr<AssemblyDescriptionAttribute, string>(a => a.Description);

    public Option<MethodInfo> EntryPoint => Option.NotNull(_assembly.EntryPoint);

    public AssemblyFlags Flags => GetAttr<AssemblyFlagsAttribute, AssemblyFlags>(a => (AssemblyFlags)a.AssemblyFlags);
    
    public string? FullName => _assembly.FullName;

    public string Location => _assembly.Location;

    public IReadOnlyDictionary<string, string?> Metadata
    {
        get
        {
            if (_metadata is null)
            {
                _metadata = _assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                    .ToDictionary(a => a.Key, a => a.Value);
            }
            return _metadata;
        }
    }
    
    public string? Name => _name ??= _assembly.GetName().Name;
    
    public string Product => _product ??= GetAttr<AssemblyProductAttribute, string>(a => a.Product);
    
    public string Title => _title ??= GetAttr<AssemblyTitleAttribute, string>(a => a.Title);
    
    public string Trademark => _trademark ??= GetAttr<AssemblyTrademarkAttribute, string>(a => a.Trademark);
    
    public string Version => _version ??= GetAttr<AssemblyVersionAttribute, string>(a => a.Version, _assembly.GetName().Version?.ToString());

    public string FileVersion => GetAttr<AssemblyFileVersionAttribute, string>(a => a.Version);
    
    public string InformationalVersion => GetAttr<AssemblyInformationalVersionAttribute, string>(a => a.InformationalVersion);
    
    private AssemblyShard(Assembly assembly)
    {
        _assembly = assembly;
    }


    private T GetAttr<A, T>(Func<A, T> getProperty, T? fallback = default)
        where A : Attribute
    {
        var attribute = _assembly.GetCustomAttributes<A>()
            .FirstOrDefault();
        if (attribute is null)
            return fallback;
        return getProperty(attribute);
    }
}