namespace ScrubJay.Reflection.Cloning;

public interface ICloneable<out TSelf> : ICloneable
    where TSelf : ICloneable<TSelf>
{
    new TSelf Clone();
}