namespace ScrubJay.Reflection.Runtime.Emission.Adaption;

public class FieldGetterAdapter<TInstance, TValue> : MemberDelegateAdapter<FieldInfo, Getter<TInstance, TValue>>
{
    public override Result<Getter<TInstance, TValue>, Exception> TryAdapt(FieldInfo field)
    {
        if (field is null)
            return new ArgumentNullException(nameof(field));
        
        // static fields
        if (field.IsStatic)
        {
            // has to have a 'static' instance type
            if (!IsStaticInstanceType<TInstance>())
                return GetArgAdaptException(field);
            
            // return type has to be compat
            //if (CanConvert(new Variable.Field(field), new Variable.Stack(typeof(TValue))))
                
        }

        throw new NotImplementedException();
    }
}