namespace ScrubJay.Reflection.IL.Emission;

using Emit = Action<Emitter>;

public sealed class Emissions : List<Emit>
{
    public Emit Combine()
    {
        if (Count == 0)
            return emitter => { };
        if (Count == 1)
            return this[0];

        return combine;

        void combine(Emitter emitter)
        {
            foreach (var emission in this)
            {
                emission(emitter);
            }
        }
    }
}