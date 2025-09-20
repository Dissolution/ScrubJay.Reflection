namespace ScrubJay.Reflection.Extensions;

[PublicAPI]
public static class PropertyInfoExtensions
{
    extension(PropertyInfo)
    {
    }

    extension(PropertyInfo? property)
    {
        public Viz Visibility
        {
            get
            {
                Viz visibility = default;
                if (property is not null)
                {
                    visibility |= property.GetMethod.Visibility;
                    visibility |= property.SetMethod.Visibility;
                }

                return visibility;
            }
        }
    }
}