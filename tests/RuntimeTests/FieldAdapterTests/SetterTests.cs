using ScrubJay.Functional;
using ScrubJay.Reflection.Adapting;

namespace ScrubJay.Reflection.Tests.RuntimeTests.FieldAdapterTests;

public class SetterTests
{
    public class InstanceClassTests
    {
        [Fact]
        public void ExactWorks()
        {
            var field = Prelude.Shard<ClassEntity>().Fields<Guid>().OneOrThrow();
            var setter = DelegateToFieldAdapter
                .TryAdapt<Setter<ClassEntity, Guid>>(field)
                .OkOrThrow();
            Assert.NotNull(setter);

            foreach (var guid in TestData.InfiniteGuids().Take(10))
            {
                var entity = new ClassEntity();
                Assert.NotEqual(guid, entity.Id);

                setter(ref entity, guid);
                Assert.Equal(guid, entity.Id);
            }
        }

        [Fact]
        public void FromObjectWorks()
        {
            var field = Prelude.Shard<ClassEntity>().Fields<Guid>().OneOrThrow();
            var setter = DelegateToFieldAdapter
                .TryAdapt<Setter<ClassEntity, object>>(field)
                .OkOrThrow();
            Assert.NotNull(setter);

            foreach (var guid in TestData.InfiniteGuids().Take(10))
            {
                var entity = new ClassEntity();
                Assert.NotEqual(guid, entity.Id);

                object guidobj = (object)guid;
                
                setter(ref entity, guidobj);
                Assert.Equal(guid, entity.Id);
            }
        }

        [Fact]
        public void FromSuperTypeWorks()
        {
            var field = Prelude
                .Shard<EntityHolder>()
                .Fields<ClassEntity>()
                .OneOrThrow();
            var setter = DelegateToFieldAdapter
                .TryAdapt<Setter<EntityHolder, NameStampClassEntity>>(field)
                .OkOrThrow();
            Assert.NotNull(setter);

            int i = 0;
            foreach (var guid in TestData.InfiniteGuids().Take(10))
            {
                var holder = new EntityHolder(Guid.NewGuid(), null);
                Assert.NotEqual(guid,holder.ClassEntity.Id);

                var nsce = new NameStampClassEntity(guid, $"{i++}");
                
                setter(ref holder, nsce);
                Assert.Equal(guid, holder.ClassEntity.Id);
            }
        }
    }

    public class InstanceStructTests
    {
        [Fact]
        public void ExactWorks()
        {
            var field = Prelude.Shard<StructEntity>().Fields<Guid>().OneOrThrow();
            var setter = DelegateToFieldAdapter
                .TryAdapt<Setter<StructEntity, Guid>>(field)
                .OkOrThrow();
            Assert.NotNull(setter);

            foreach (var guid in TestData.InfiniteGuids().Take(10))
            {
                var entity = new StructEntity();
                Assert.NotEqual(guid, entity.Id);

                setter(ref entity, guid);
                Assert.Equal(guid, entity.Id);
            }
        }

        [Fact]
        public void FromObjectWorks()
        {
            var field = Prelude.Shard<StructEntity>().Fields<Guid>().OneOrThrow();
            var setter = DelegateToFieldAdapter
                .TryAdapt<Setter<StructEntity, object>>(field)
                .OkOrThrow();
            Assert.NotNull(setter);

            foreach (var guid in TestData.InfiniteGuids().Take(10))
            {
                var entity = new StructEntity();
                Assert.NotEqual(guid, entity.Id);

                object guidobj = (object)guid;
                
                setter(ref entity, guidobj);
                Assert.Equal(guid, entity.Id);
            }
        }
    }


    [Fact]
    public void StaticExactSetterWorks()
    {
        var field = Prelude.Shard(typeof(StaticEntity)).Fields<Guid>().OneOrThrow();
        var setter = DelegateToFieldAdapter
            .TryAdapt<Setter<None, Guid>>(field)
            .OkOrThrow();
        Assert.NotNull(setter);

        foreach (var guid in TestData.InfiniteGuids().Take(10))
        {
            StaticEntity.Id = Guid.NewGuid();
            Assert.NotEqual(guid, StaticEntity.Id);

            setter(ref None.Ref, guid);
            Assert.Equal(guid, StaticEntity.Id);
        }
    }
}