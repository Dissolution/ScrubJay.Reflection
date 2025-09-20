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
        
        /// <summary>
        /// Determines if this property is marked as init-only.
        /// </summary>
        public bool IsInitOnly
        {
            get
            {
                if (property is null) 
                    return false;
                
                if (!property.CanWrite)
                    return false;

                var setMethod = property.SetMethod;

                if (setMethod is null)
                    return false;
                
                // `init` properties have a special custom modifier
                return setMethod.ReturnParameter
                    .GetRequiredCustomModifiers()
                    .Contains(typeof(IsExternalInit));
            }
        }
    }
}