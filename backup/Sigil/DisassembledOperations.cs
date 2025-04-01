using ScrubJay.Sigil.Emission.Generic;

namespace ScrubJay.Sigil;

/// <summary>
/// Represents a decompiled delegate.
///
/// The operations of the decompiled delegate can be inspected, and it can be replayed to a new Emit.
/// </summary>
public sealed class DisassembledOperations<TDelegateType> : IEnumerable<Operation<TDelegateType>>
{
    /// <summary>
    /// The total number of operations that were decompiled.
    /// </summary>
    public int Count { get { return _operations.Count; } }

    /// <summary>
    /// The parameters the decompiled delegate takes.
    /// </summary>
    public IEnumerable<SigilParameter> Parameters { get; private set; }

    /// <summary>
    /// The locals the decompiled delegate declared and uses.
    /// </summary>
    public IEnumerable<SigilLocal> Locals { get; private set; }

    /// <summary>
    /// The labels the decompile delegate uses.
    /// </summary>
    public IEnumerable<SigilLabel> Labels { get; private set; }

    private readonly object _usageLock = new object();
    private volatile IEnumerable<OperationResultUsage<TDelegateType>> _usage;
    /// <summary>
    /// Traces where values produced by certain operations are used.
    ///
    /// This is roughly equivalent to having built the disassembled delegate via Sigil originally,
    /// and saving the results of TraceOperationResultUsage().
    /// </summary>
    public IEnumerable<OperationResultUsage<TDelegateType>> Usage
    {
        get
        {
            if (_usage != null) return _usage;

            lock (_usageLock)
            {
                if (_usage != null) return _usage;

                var e1 = EmitFrom(0, this.Count);
                _usage = e1.TraceOperationResultUsage();

                return _usage;
            }
        }
    }

    /// <summary>
    /// Returns true if a call to EmitAll will succeed.
    ///
    /// This property will be false if the delegate that was disassembled closed over it's environment,
    /// thereby adding an implicit `this` that cannot be represented (and thus cannot be returned).
    /// </summary>
    public bool CanEmit { get; private set; }

    private readonly List<Operation<TDelegateType>> _operations;
    /// <summary>
    /// Returns the operation that would be emitted at the given index.
    /// </summary>
    public Operation<TDelegateType> this[int index]
    {
        get
        {
            if (index < 0 || index >= _operations.Count)
            {
                if (_operations.Count == 0)
                {
                    throw new IndexOutOfRangeException("DecompiledOperations is empty");
                }

                throw new IndexOutOfRangeException("Expected index between 0 and " + (_operations.Count - 1) + ", inclusive; found " + index);
            }

            return _operations[index];
        }
    }

    internal DisassembledOperations(
        List<Operation<TDelegateType>> ops,
        IEnumerable<SigilParameter> ps,
        IEnumerable<SigilLocal> locs,
        IEnumerable<SigilLabel> labels,
        bool canEmit)
    {
        _operations = ops;
        Parameters = ps;

        Locals = locs;
        Labels = labels;

        CanEmit = canEmit;

        foreach (var loc in Locals)
        {
            loc.SetOwner(this);
        }

        foreach(var lab in Labels)
        {
            lab.SetOwner(this);
        }
    }

    private void Apply(int i, Emit<TDelegateType> emit)
    {
        if (i == 0)
        {
            foreach (var l in Locals)
            {
                emit.DeclareLocal(l.LocalType, l.Name);
            }

            foreach (var l in Labels)
            {
                emit.DefineLabel(l.Name);
            }
        }

        this[i].Apply(emit);
    }

    private Emit<TDelegateType> EmitFrom(int from, int length, string name = null, ModuleBuilder module = null)
    {
        if (from < 0 || from > _operations.Count)
        {
            throw new InvalidOperationException("from must be between 0 and " + _operations.Count + ", inclusive; found " + from);
        }

        if (length < 0)
        {
            throw new InvalidOperationException("length must be non-negative; found " + length);
        }

        if (from + length > _operations.Count)
        {
            throw new InvalidOperationException("from + length must be less than " + _operations.Count + "; found " + (from + length));
        }

        var e1 =
            Emit<TDelegateType>.DisassemblerDynamicMethod(
                Parameters.Select(p => p.ParameterType).ToArray(),
                name,
                module
            );

        for (var i = 0; i < length; i++)
        {
            Apply(from + i, e1);
        }

        return e1;
    }

    private Emit<TDelegateType> Emit(int length, string name = null, ModuleBuilder module = null)
    {
        if (!CanEmit)
        {
            throw new InvalidOperationException("Cannot emit this DisassembledOperations object, check CanEmit before calling any Emit methods");
        }

        if(length < 0 || length > _operations.Count)
        {
            throw new InvalidOperationException("length must be between 0 and "+_operations.Count+", inclusive; found "+length);
        }

        return EmitFrom(0, length, name, module);
    }

    /// <summary>
    /// Emits the disassembled instructions into a new Emit.
    /// </summary>
    public Emit<TDelegateType> EmitAll(string name = null, ModuleBuilder module = null)
    {
        return Emit(this.Count, name, module);
    }

    /// <summary>
    /// Returns an enumerator which steps over the Operations that are in this DisassembledOperations.
    /// </summary>
    public IEnumerator<Operation<TDelegateType>> GetEnumerator()
    {
        return _operations.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator which steps over the Operations that are in this DisassembledOperations.
    /// </summary>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return ((System.Collections.IEnumerable)_operations).GetEnumerator();
    }
}
