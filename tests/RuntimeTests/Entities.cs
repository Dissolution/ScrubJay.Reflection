using ScrubJay.Reflection.Extensions;

namespace ScrubJay.Reflection.Tests.RuntimeTests;

public class EntityHolder
{
    public NameStampClassEntity NameStampClassEntity { get; }
    
    public ClassEntity ClassEntity  { get;}
    
    public StructEntity StructEntity { get; }

    public EntityHolder(Guid id, string? name)
    {
        NameStampClassEntity = new(id, name);
        ClassEntity = new(id);
        StructEntity = new(id, name);
    }
}

public static class StaticEntity
{
    public static Guid Id { get; set; } = Guid.NewGuid();
    public static string? Name { get; set; }
}

public interface IEntity : IEquatable<IEntity>
{
    Guid Id { get; }

    bool IEquatable<IEntity>.Equals(IEntity? other)
        => other?.Id == this.Id;
}

public class ClassEntity : IEntity, 
    IEquatable<ClassEntity>
{
    public Guid Id { get; }

    public ClassEntity()
    {
        this.Id = Guid.NewGuid();
    }
    
    public ClassEntity(Guid id)
    {
        this.Id = id;
    }

    public bool Equals(ClassEntity? other) => other is not null && other.Id == this.Id;

    public override string ToString()
    {
        return Id.ToUpperDigitsString();
    }
}

public class NameStampClassEntity : ClassEntity, IEntity
{
    public string? Name { get; }
    
    public DateTime Created { get; } = DateTime.Now;

    public NameStampClassEntity(Guid id, string? name = null)
        : base(id)
    {
        this.Name = name;
    }
}

public struct StructEntity : IEntity
{
    public Guid Id { get; }

    public string? Name { get; }

    public StructEntity(Guid id, string? name = null)
    {
        this.Id = id;
        this.Name = name;
    }

    public override string ToString()
    {
        return $"Id #{Id:N} \"{Name}\"";
    }
}

public readonly ref struct RRStructEntity : IEntity
{
    public Guid Id { get; }
    
    public ReadOnlySpan<char> Name { get; }

    public RRStructEntity(Guid id,  ReadOnlySpan<char> name)
    {
        this.Id = id;
        this.Name = name;
    }

    public bool Equals(IEntity? other)
    {
        return other is not null && other.Id == this.Id;
    }
}