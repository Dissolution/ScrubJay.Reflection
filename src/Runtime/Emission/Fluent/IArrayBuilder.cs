namespace ScrubJay.Reflection.Runtime.Emission.Fluent;

public interface IArrayBuilder<TEmitter>
    where TEmitter : IFluentEmitter<TEmitter>
{
    TEmitter LoadLength();
    TEmitter LoadItem(Type itemType, bool asAddress = false);
    TEmitter LoadItem<TItem>(bool asAddress = false);
    
    TEmitter StoreItem(Type itemType);
    TEmitter StoreItem<TItem>();

    TEmitter New(Type itemType);
    TEmitter New<TItem>();
}