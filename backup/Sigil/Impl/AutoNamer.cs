namespace ScrubJay.Sigil.Impl;

// Just a dinky little class to automatically generate names for things like methods, labels, and locals
internal static class AutoNamer
{
    private static readonly object _nullKey = new object();
    private static readonly Dictionary<Tuple<object, string>, int> _state = new Dictionary<Tuple<object, string>, int>();

    public static string Next(string root)
    {
        return Next(_nullKey, root);
    }

    public static string Next(object on, string root, params IEnumerable<string>[] inUse)
    {
        var key = Tuple.Create(on, root);

        lock (_state)
        {
            int next;
            if (!_state.TryGetValue(key, out next))
            {
                next = 0;
                _state[key] = next;
            }

            _state[key]++;

            var ret = root + next;

            if (inUse.Any(a => a.Any(x => x == ret)))
            {
                return Next(on, root, inUse);
            }

            return ret;
        }
    }

    public static void Release(object on)
    {
        lock (_state)
        {
            var deadKeys = _state.Keys.Where(k => k.Item1 == on).ToList();

            foreach (var key in deadKeys)
            {
                _state.Remove(key);
            }
        }
    }
}
