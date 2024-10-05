// using ScrubJay;
// using ScrubJay.Reflection;
//
// namespace Scratch.Criteria;
//
// public sealed record class EqualityMatchCriteria<T> : ICriteria<T>
// {
//     public T? Value { get; set; } = default;
//     public IEqualityComparer<T>? Comparer { get; set; } = null;
//
//     public EqualityMatchCriteria()
//     {
//
//     }
//     public EqualityMatchCriteria(T? value)
//     {
//         this.Value = value;
//     }
//     public EqualityMatchCriteria(T? value, IEqualityComparer<T>? valueComparer)
//     {
//         this.Value = value;
//         this.Comparer = valueComparer;
//     }
//
//     public Result<Unit, Exception> Matches(T value)
//     {
//         if (Comparer is null)
//         {
//             if (EqualityComparer<T>.Default.Equals(this.Value!, value!))
//                 return Ok();
//             return Reflexception.Create<ArgumentException>($"{value} != {this.Value}", nameof(value));
//         }
//         else
//         {
//             if (Comparer.Equals(this.Value!, value!))
//                 return Ok();
//             return Reflexception.Create<ArgumentException>($"!{Comparer}.Equals({value}, {this.Value})", nameof(value));
//         }
//     }
// }