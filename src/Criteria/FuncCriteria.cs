// using ScrubJay;
// using ScrubJay.Reflection;
//
// namespace Scratch.Criteria;
//
// public class FuncCriteria<T> : ICriteria<T>
// {
//     private readonly CriteriaMatches<T> _criteriaMatches;
//
//     public FuncCriteria(CriteriaMatches<T> matches)
//     {
//         _criteriaMatches = matches;
//     }
//
//     public FuncCriteria(Func<T, bool> matches)
//     {
//         _criteriaMatches = Criteria.Match(matches);
//     }
//
//     public Result<Unit, Exception> Matches(T value) => _criteriaMatches(value);
// }
//
//
//
// public class StatefulFuncCriteria<TState, T> : ICriteria<T>
// {
//     private readonly StatefulCriteriaMatches<TState, T> _match;
//     
//     public TState State { get; }
//
//     public StatefulFuncCriteria(TState state, StatefulCriteriaMatches<TState, T> match)
//     {
//         this.State = state;
//         _match = match;
//     }
//     
//     public StatefulFuncCriteria(TState state, Func<TState, T, bool> matches)
//     {
//         this.State = state;
//         _match = Criteria.Match(matches);
//     }
//
//     public Result<Unit, Exception> Matches(T value)
//     {
//         return _match(this.State, value);
//     }
// }