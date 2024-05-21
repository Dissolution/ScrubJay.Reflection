# Emitter Notes

- All Emitters are implementations of `IILEmitter<TEmitter>` until the last possible instance
- `IGeneratorEmitter` corresponds to the non-`Emit` methods on `ILGenerator`
  - `ICleanGeneratorEmitter` is a cleaned-up version of `IGeneratorEmitter`
- `IOpCodeEmitter` corresponds to the `Emit` methods in `ILGenerator`
  - `IOperationEmitter` is a specified + limited version of `IOpCodeEmitter`
    - `ICleanOperationEmitter` is a cleaned-up version of `IOperationEmitter`