namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    private void LocalReleased(SigilLocal sigilLocal)
    {
        FreedLocals.Add(sigilLocal);

        _currentLocals.Remove(sigilLocal.Name);

        sigilLocal.SetReleasedAt(_il.Index);
    }

    /// <summary>
    /// Declare a new local of the given type in the current method.
    ///
    /// Name is optional, and only provided for debugging purposes.  It has no
    /// effect on emitted IL.
    ///
    /// Be aware that each local takes some space on the stack, inefficient use of locals
    /// could lead to StackOverflowExceptions at runtime.
    ///
    /// Jil will reuse local index on the stack if the corresponding Local instance has been disposed.
    /// By default Jil will set reused locals to their default value, you can change this behavior
    /// by passing initializeReused = false.
    /// </summary>
    public SigilLocal DeclareLocal<T>(string name = null, bool initializeReused = true)
    {
        return DeclareLocal(typeof(T), name, initializeReused);
    }

    /// <summary>
    /// Declare a new local of the given type in the current method.
    ///
    /// Name is optional, and only provided for debugging purposes.  It has no
    /// effect on emitted IL.
    ///
    /// Be aware that each local takes some space on the stack, inefficient use of locals
    /// could lead to StackOverflowExceptions at runtime.
    ///
    /// Jil will reuse local index on the stack if the corresponding Local instance has been disposed.
    /// By default Jil will set reused locals to their default value, you can change this behavior
    /// by passing initializeReused = false.
    /// </summary>
    public SigilLocal DeclareLocal(Type type, string name = null, bool initializeReused = true)
    {
        if (type == null)
        {
            throw new ArgumentNullException("type");
        }

        name = name ?? AutoNamer.Next(this, "_local", Locals.Names, Labels.Names);

        if (_currentLocals.ContainsKey(name))
        {
            throw new InvalidOperationException("Local with name '" + name + "' already exists");
        }

        var existingLocal = FreedLocals.FirstOrDefault(l => l.LocalType == type);

        DeclareLocalDelegate local;
        ushort localIndex;

        if (existingLocal == null)
        {
            local = _il.DeclareLocal(type);

            localIndex = _nextLocalIndex;
            _nextLocalIndex++;
        }
        else
        {
            local = existingLocal.LocalDel;
            localIndex = existingLocal.Index;

            FreedLocals.Remove(existingLocal);
        }

        var ret = new SigilLocal(this, localIndex, type, local, name, LocalReleased, _il.Index);

        _unusedLocals.Add(ret);

        _allLocals.Add(ret);

        _currentLocals[ret.Name] = ret;

        // we need to initialize this local to it's default value, because otherwise
        //   it might be read from to get some non-sense
        if (existingLocal != null && initializeReused)
        {
            if (!type.IsValueType)
            {
                // reference types all get nulled
                LoadNull();
                StoreLocal(ret);
            }
            else
            {
                // handle known primitives better
                //   so as not to confuse the JIT
                var defaultLoaded = false;

                if (type == typeof(bool))
                {
                    LoadConstant(default(bool));
                    defaultLoaded = true;
                }

                if (type == typeof(byte))
                {
                    LoadConstant(default(byte));
                    defaultLoaded = true;
                }

                if (type == typeof(sbyte))
                {
                    LoadConstant(default(sbyte));
                    defaultLoaded = true;
                }

                if (type == typeof(short))
                {
                    LoadConstant(default(short));
                    defaultLoaded = true;
                }

                if (type == typeof(ushort))
                {
                    LoadConstant(default(ushort));
                    defaultLoaded = true;
                }

                if (type == typeof(int))
                {
                    LoadConstant(default(int));
                    defaultLoaded = true;
                }

                if (type == typeof(uint))
                {
                    LoadConstant(default(uint));
                    defaultLoaded = true;
                }

                if (type == typeof(long))
                {
                    LoadConstant(default(long));
                    defaultLoaded = true;
                }

                if (type == typeof(ulong))
                {
                    LoadConstant(default(ulong));
                    defaultLoaded = true;
                }

                if (type == typeof(float))
                {
                    LoadConstant(default(float));
                    defaultLoaded = true;
                }

                if (type == typeof(double))
                {
                    LoadConstant(default(double));
                    defaultLoaded = true;
                }

                if (defaultLoaded)
                {
                    StoreLocal(ret);
                }
                else
                {
                    // if it's use defined though, we've got little choice
                    LoadLocalAddress(ret);
                    InitializeObject(type);
                }
            }
        }

        return ret;
    }

    /// <summary>
    /// Declare a new local of the given type in the current method.
    ///
    /// Name is optional, and only provided for debugging purposes.  It has no
    /// effect on emitted IL.
    ///
    /// Be aware that each local takes some space on the stack, inefficient use of locals
    /// could lead to StackOverflowExceptions at runtime.
    ///
    /// Jil will reuse local index on the stack if the corresponding Local instance has been disposed.
    /// By default Jil will set reused locals to their default value, you can change this behavior
    /// by passing initializeReused = false.
    /// </summary>
    public Emit<TDelegateType> DeclareLocal<T>(out SigilLocal sigilLocal, string name = null, bool initializeReused = true)
    {
        return DeclareLocal(typeof(T), out sigilLocal, name, initializeReused);
    }

    /// <summary>
    /// Declare a new local of the given type in the current method.
    ///
    /// Name is optional, and only provided for debugging purposes.  It has no
    /// effect on emitted IL.
    ///
    /// Be aware that each local takes some space on the stack, inefficient use of locals
    /// could lead to StackOverflowExceptions at runtime.
    ///
    /// Jil will reuse local index on the stack if the corresponding Local instance has been disposed.
    /// By default Jil will set reused locals to their default value, you can change this behavior
    /// by passing initializeReused = false.
    /// </summary>
    public Emit<TDelegateType> DeclareLocal(Type type, out SigilLocal sigilLocal, string name = null, bool initializeReused = true)
    {
        sigilLocal = DeclareLocal(type, name, initializeReused);

        return this;
    }
}
