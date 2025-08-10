// namespace ScrubJay.Reflection.Collections;
//
// public enum NodeAccess
// {
//     TwoWay,
//     OneWay,
// }
//
// public class RopeNode<T>
// {
//     private RopeNode<T>? _head;
//     
//     // these are one-way
//     private RopeNode<T>? _prev;
//     private RopeNode<T>? _next;
//     
//     internal T _value;
//     
//
//     public bool IsHead => _head == this;
//
//     public Option<RopeNode<T>> Head
//         => Option.NotNull(_head);
//
//     public Option<RopeNode<T>> Previous(NodeAccess nodeAccess = NodeAccess.TwoWay)
//     {
//         if (nodeAccess != NodeAccess.TwoWay && this == _head) 
//             return None();
//         return Option.NotNull(_prev);
//     }
//     
//     public Option<RopeNode<T>> Next(NodeAccess nodeAccess = NodeAccess.TwoWay)
//     {
//         if (nodeAccess != NodeAccess.TwoWay && _next == _head) 
//             return None();
//         return Option.NotNull(_next);
//     }
//
//     public T Value
//     {
//         get => _value;
//         set => _value = value;
//     }
//     
//     public ref T RefValue => ref _value;
//
//     public void Delete()
//     {
//         Debug.Assert(_head is not null);
//         
//         // Is this the only node?
//         if ()
//     }
//
//     public void Append(RopeNode<T> node)
//     {
//         
//     }
// }