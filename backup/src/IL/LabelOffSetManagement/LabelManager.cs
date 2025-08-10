namespace ScrubJay.Reflection.IL.LabelOffSetManagement;

public class LabelManager
{
    private static readonly Func<int, Label> _newLabel;

    static LabelManager()
    {
        var labelCtor = Shard<Label>()
            .Constructors()
            .Instance()
            .WithParameters<int>()
            .TryGetOnly().OkOrThrow();
        
        // use ILGen because all emitters depend on LabelManager
        _newLabel = RuntimeBuilder.TryGenerateDelegate<Func<int, Label>>(gen =>
        {
            gen.Emit(OpCodes.Ldarg_0); // loads the id
            gen.Emit(OpCodes.Newobj, labelCtor); // calls the constructor
            gen.Emit(OpCodes.Ret); // return the newly constructed label
        }).OkOrThrow();
    }
    
    
    private readonly List<(ILLabel ILLabel, Label Label)> _labels = [];

    public ILLabel Declare(string? name = null)
    {
        int id = _labels.Count;
        Label label = _newLabel(id);
        ILLabel ilLabel = new ILLabel(id, name);
        _labels.Add((ilLabel, label));
        return ilLabel;
    }
    
    public ILLabel Declare(Label label, string? name = null)
    {
        int index = label.GetHashCode();
        if (index != _labels.Count)
            throw new ArgumentException(null, nameof(label));
        var ilLabel = new ILLabel(label.GetHashCode(), name);
        _labels.Add((ilLabel, label));
        return ilLabel;
    }

    public Option<Label> Declared(ILLabel ilLabel)
    {
        int index = ilLabel.Id;
        if ((uint)index >= (uint)_labels.Count)
            return None();
        return Some(_labels[index].Label);
    }
    
    public ILLabel Mark(ILOffset offset, ILLabel ilLabel)
    {
        if (offset.IsUnknown)
            throw new ArgumentException(null, nameof(offset));
        
        int index = ilLabel.Id;
        if ((uint)index >= (uint)_labels.Count)
            throw new ArgumentException(null, nameof(ilLabel));

        if (ilLabel.Offset != ILOffset.Unknown &&
            ilLabel.Offset != offset)
            throw new ArgumentException(null, nameof(ilLabel));

        ilLabel.Offset = offset;
        return ilLabel;
    }
}