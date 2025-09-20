namespace ScrubJay.Reflection.Extensions;

[PublicAPI]
public static class MethodBaseExtensions
{
    extension(MethodBase)
    {
    }

    extension(MethodBase? method)
    {
        public Viz Visibility
        {
            get
            {
                Viz visibility = default;
                if (method is not null)
                {
                    visibility |= method.IsStatic ? Viz.Static : Viz.Instance;
                    if (method.IsPublic)
                        visibility |= Viz.Public;
                    if (method.IsAssembly || method.IsFamilyAndAssembly || method.IsFamilyOrAssembly)
                        visibility |= Viz.Internal;
                    if (method.IsFamily || method.IsFamilyAndAssembly || method.IsFamilyOrAssembly)
                        visibility |= Viz.Protected;
                    if (method.IsPrivate)
                        visibility |= Viz.Private;
                }

                return visibility;
            }
        }

        /// <summary>
        /// Can this <see cref="MethodBase"/> be overriden?
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <see href="https://stackoverflow.com/questions/38078948/check-if-a-classes-property-or-method-is-declared-as-sealed"/>
        public bool IsOverridable => method is not null && method.IsVirtual && !method.IsFinal;

        /// <summary>
        /// Is this <see cref="MethodBase"/> <c>sealed</c>?
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public bool IsSealed => method is not null && (method.IsFinal || !method.IsVirtual);

        public bool IsAsync
        {
            get
            {
                if (method is null)
                    return false;
                return typeof(IAsyncStateMachine).IsAssignableFrom(method.DeclaringType);
            }
        }


        /// <summary>
        /// Get the <see cref="Type">Types</see> of the parameters in this <see cref="MethodBase"/>
        /// </summary>
        public Type[] GetParameterTypes()
        {
            if (method is null) return [];

            var parameters = method.GetParameters();
            Type[] types = new Type[parameters.Length];
            // reverse to elide bounds checks
            for (var i = parameters.Length - 1; i >= 0; i--)
            {
                types[i] = parameters[i].ParameterType;
            }

            return types;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> returned by this <see cref="MethodBase"/>
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Type ReturnType()
        {
            return method switch
            {
                null => typeof(void),
                MethodInfo info => info.ReturnType,
                ConstructorInfo { IsStatic: true } => typeof(void),
                ConstructorInfo ctor => ctor.DeclaringType!,
                _ => throw new ArgumentException("Invalid Method", nameof(method)),
            };
        }
    }


    //
    //
    //
    //
    // public static ParameterInfo ReturnParameter(this MethodBase method)
    // {
    //     if (method is MethodInfo methodInfo)
    //     {
    //         var parameter = methodInfo.ReturnParameter;
    //         return parameter ?? new ReturnParameterInfo(methodInfo, methodInfo.ReturnType);
    //     }
    //
    //     if (method is ConstructorInfo constructorInfo)
    //     {
    //         if (constructorInfo.IsStatic)
    //         {
    //             return new ReturnParameterInfo(constructorInfo, typeof(void));
    //         }
    //         else
    //         {
    //             return new ReturnParameterInfo(constructorInfo, constructorInfo.DeclaringType!);
    //         }
    //     }
    //
    //     throw new ArgumentException("Invalid Method", nameof(method));
    // }
    //
    // public static DecompiledILMethod Decompile(this MethodBase method) => DecompiledILMethod.Decompile(method);
}