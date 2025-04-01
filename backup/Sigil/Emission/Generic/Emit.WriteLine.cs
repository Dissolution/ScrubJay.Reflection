namespace ScrubJay.Sigil.Emission.Generic;

public partial class Emit<TDelegateType>
{
    /// <summary>
    /// Emits IL that calls Console.WriteLine(string) for the given string if no locals are passed.
    ///
    /// If any locals are passed, line is treated as a format string and local values are used in a call
    /// to Console.WriteLine(string, object[]).
    /// </summary>
    public Emit<TDelegateType> WriteLine(string line, params SigilLocal[] locals)
    {
        if (line == null)
        {
            throw new ArgumentNullException("line");
        }

        if (locals == null)
        {
            throw new ArgumentNullException("locals");
        }

        var unowned = locals.OfType<IOwned>().FirstOrDefault(l => l.Owner != this);
        if (unowned != null)
        {
            FailOwnership(unowned);
        }

        if (locals.Length == 0)
        {
            LoadConstant(line);
            var consoleWriteLineStringMethod = typeof(Console)
                .GetMethod(
                    name: nameof(Console.WriteLine),
                    bindingAttr: BindingFlags.Public | BindingFlags.Static,
                    binder: null,
                    types: [typeof(string)],
                    modifiers: null);
            return Call(consoleWriteLineStringMethod);
        }

        LoadConstant(line);

        LoadConstant(locals.Length);
        NewArray<object>();

        for (var i = 0; i < locals.Length; i++)
        {
            Duplicate();
            LoadConstant(i);
            LoadLocal(locals[i]);

            if (locals[i].LocalType.IsValueType)
            {
                Box(locals[i].LocalType);
            }

            StoreElement<object>();
        }

        var consoleWriteLineFormatMethod = typeof(Console)
            .GetMethod(
                name: nameof(Console.WriteLine),
                bindingAttr: BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: [typeof(string), typeof(object[])],
                modifiers: null);
        return Call(consoleWriteLineFormatMethod);
    }
}
