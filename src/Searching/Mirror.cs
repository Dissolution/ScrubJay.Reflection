using ScrubJay.Reflection.Searching.Criteria;

namespace ScrubJay.Reflection.Searching;

public static class Mirror
{
    public static class Search
    {
        public static IEnumerable<MemberInfo> FindMembers(Type type, MemberCriteria criteria)
        {
            var members = type.GetMembers(criteria.BindingFlags);
            foreach (MemberInfo member in members)
            {
                if (criteria.Matches(member))
                    yield return member;
            }
        }

        public static MemberInfo? FindMember(Type type, MemberCriteria criteria)
            => FindMembers(type, criteria).OneOrDefault();

        public static IEnumerable<TMember> FindMembers<TMember>(Type type, Func<MemberCriteriaBuilder, ICriteria<TMember>> buildCriteria)
            where TMember : MemberInfo
        {
            var builder = new MemberCriteriaBuilder();
            ICriteria<TMember> memberCriteria = buildCriteria(builder);
            var members = type
                .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                .OfType<TMember>();
            foreach (TMember member in members)
            {
                if (memberCriteria.Matches(member))
                    yield return member;
            }
        }

        public static TMember? FindMember<TMember>(Type type, Func<MemberCriteriaBuilder, ICriteria<TMember>> buildCriteria)
            where TMember : MemberInfo
        {
            return FindMembers<TMember>(type, buildCriteria).OneOrDefault();
        }

        public static IEnumerable<TMember> FindMembers<T, TMember>(Func<MemberCriteriaBuilder, ICriteria<TMember>> buildCriteria)
            where TMember : MemberInfo
        {
            return FindMembers<TMember>(typeof(T), buildCriteria);
        }

        public static TMember? FindMember<T, TMember>(Func<MemberCriteriaBuilder, ICriteria<TMember>> buildCriteria)
            where TMember : MemberInfo
        {
            return FindMembers<TMember>(typeof(T), buildCriteria).OneOrDefault();
        }

        public static Result<TMember, MemberAccessException> TryFindMember<TMember>(
            Type type,
            Func<MemberCriteriaBuilder, ICriteria<TMember>> buildCriteria)
            where TMember : MemberInfo
        {
            var mcBuilder = new MemberCriteriaBuilder();
            var criteria = buildCriteria(mcBuilder);
            var member = type
                .AllMembers()
                .OfType<TMember>()
                .Where(m => criteria.Matches(m))
                .OneOrDefault();
            if (member is not null)
                return member;
            return new MemberAccessException($"""
                Criteria:
                {criteria}
                Matched non-1 members
                """);
        }
    }

    public static class Search<T>
    {
        public static IEnumerable<TMember> FindMembers<TMember>(Func<MemberCriteriaBuilder, ICriteria<TMember>> buildCriteria)
            where TMember : MemberInfo
        {
            return Search.FindMembers<TMember>(typeof(T), buildCriteria);
        }

        public static TMember? FindMember<TMember>(Func<MemberCriteriaBuilder, ICriteria<TMember>> buildCriteria)
            where TMember : MemberInfo
        {
            return Search.FindMembers<TMember>(typeof(T), buildCriteria).OneOrDefault();
        }

        public static Result<TMember, MemberAccessException> TryFindMember<TMember>(
            Func<MemberCriteriaBuilder, ICriteria<TMember>> buildCriteria)
            where TMember : MemberInfo
        {
            var mcBuilder = new MemberCriteriaBuilder();
            var criteria = buildCriteria(mcBuilder);
            var member = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .OfType<TMember>()
                .Where(m => criteria.Matches(m))
                .OneOrDefault();
            if (member is not null)
                return member;
            return new MemberAccessException($"""
                Criteria:
                {criteria}
                Matched non-1 members
                """);
        }
    }
}