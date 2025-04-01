// using ScrubJay.Fluent;
// using ScrubJay.Reflection.Runtime;
//
// namespace ScrubJay.Reflection.Scratch;
//
// public record class RuntimeMethodInfo
// {
//     private Module _module = RuntimeBuilder.ModuleBuilder;
//     private string? _name = null;
//     private RuntimeParameter? _returnParam = null;
//     private RuntimeParameter[]? _argParams = null;
//
//     [AllowNull, NotNull]
//     public Module Module
//     {
//         get => _module;
//         set
//         {
//             if (value is not null)
//             {
//                 _module = value;
//             }
//         }
//     }
//
//     public string Name
//     {
//         get
//         {
//             if (_name is null)
//             {
//
//             }
//         }
//         set
//         {
//             if (!string.IsNullOrWhiteSpace(value))
//             {
//                 _name = value;
//             }
//         }
//     }
//
//     private string CreateName()
//     {
//         // genericize
//
//     }
//
//
//     public DynamicMethod GetDynamicMethod()
//     {
//         var dm = new DynamicMethod(
//             name: Name ??= Guid.NewGuid().ToString(),
//             attributes: MethodAttributes.Public | MethodAttributes.Static,
//             callingConvention: CallingConventions.Standard,
//             returnType: ReturnType,
//             parameterTypes: ArgTypes,
//             m: Module ?? RuntimeBuilder.ModuleBuilder,
//             skipVisibility: true);
//         return dm;
//     }
// }
//
//
// public class DynamicMethodBuilder : FluentBuilder<DynamicMethodBuilder>
// {
//     internal RuntimeMethodInfo _methodInfo = new();
//
//     public DynamicMethodBuilder Name(string name)
//     {
//         Validate.ThrowIfEmpty(name);
//         _methodInfo.Name = name;
//         return this;
//     }
//
//     public DynamicMethodBuilder ReturnType(Type? type)
//     {
//         _methodInfo.ReturnParameter = RuntimeParameterBuilder.New
//             .ValueType(type)
//             .GetParameter();
//         return _builder;
//     }
//
//     public DynamicMethodBuilder ReturnParameter(RuntimeParameter returnParameter)
//     {
//         _methodInfo.ReturnParameter = returnParameter;
//         return _builder;
//     }
//
//     public DynamicMethodBuilder ReturnParameter(Action<RuntimeParameterBuilder> buildReturn)
//     {
//         _methodInfo.ReturnParameter = RuntimeParameterBuilder.New.Execute(buildReturn).GetParameter();
//         return _builder;
//     }
//
//     public DynamicMethodBuilder ParameterTypes(params Type[] types)
//     {
//         var count = types.Length;
//         var parameters = new RuntimeParameter[count];
//         for (var i = 0; i < count; i++)
//         {
//             parameters[i] = RuntimeParameterBuilder.New
//                 .Position(i)
//                 .ValueType(types[i])
//                 .GetParameter();
//         }
//
//         _methodInfo.ArgParameters = parameters;
//         return _builder;
//     }
//
//     public DynamicMethodBuilder Parameters(params RuntimeParameter[] parameters)
//     {
//         _methodInfo.ArgParameters = parameters;
//         return _builder;
//     }
//
//     public DynamicMethodBuilder Parameters(params Action<RuntimeParameterBuilder>[] buildArgs)
//     {
//         var count = buildArgs.Length;
//         var parameters = new RuntimeParameter[count];
//         for (var i = 0; i < count; i++)
//         {
//             parameters[i] = RuntimeParameterBuilder.New
//                 .Position(i)
//                 .Execute(buildArgs[i])
//                 .GetParameter();
//         }
//
//         _methodInfo.ArgParameters = parameters;
//         return _builder;
//     }
//
//     public DynamicMethodBuilder Module(Module module)
//     {
//         _methodInfo.Module = module;
//         return this;
//     }
//
//     public DynamicMethod GetDynamicMethod()
//     {
//         var dm = new DynamicMethod(
//             name: _name ?? Guid.NewGuid().ToString(),
//             attributes: MethodAttributes.Public | MethodAttributes.Static,
//             callingConvention: CallingConventions.Standard,
//             returnType: _returnType ?? typeof(void),
//             parameterTypes: _argParameters ?? [],
//             m: _module ?? RuntimeBuilder.ModuleBuilder,
//             skipVisibility: true);
//
//         dm.DefineParameter(0,
//
//         return dm;
//     }
//
//
//
// }