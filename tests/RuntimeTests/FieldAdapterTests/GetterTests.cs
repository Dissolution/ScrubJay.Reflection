using ScrubJay.Functional;
using ScrubJay.Reflection.Adapting;

namespace ScrubJay.Reflection.Tests.RuntimeTests.FieldAdapterTests;

public class GetterTests
{
    public class InstanceClassTests
    {
        [Fact]
        public void ExactWorks()
        {
            var field = Prelude.Reflect<ClassEntity>().Fields<Guid>().OneOrThrow();
            var getter = DelegateToFieldAdapter
                .TryAdapt<Getter<ClassEntity, Guid>>(field)
                .OkOrThrow();
            
            Assert.NotNull(getter);

            foreach (var guid in TestData.InfiniteGuids().Take(10))
            {
                var entity = new ClassEntity(guid);
                Assert.Equal(guid, entity.Id);

                var result = getter(ref entity);
                Assert.Equal(guid, result);
            }
        }

        [Fact]
        public void AsObjectWorks()
        {
            var field = Prelude.Reflect<ClassEntity>().Fields<Guid>().OneOrThrow();
            var getter = DelegateToFieldAdapter
                .TryAdapt<Getter<ClassEntity, object>>(field)
                .OkOrThrow();
            Assert.NotNull(getter);

            foreach (var guid in TestData.InfiniteGuids().Take(10))
            {
                var entity = new ClassEntity(guid);
                Assert.Equal(guid, entity.Id);

                object? result = getter(ref entity);
                Assert.NotNull(result);
                Assert.IsType(typeof(Guid), result);
                Assert.Equal(guid, (Guid)result);
            }
        }

        [Fact]
        public void AsSubTypeWorks()
        {
            var field = Prelude
                .Reflect<EntityHolder>()
                .Fields<NameStampClassEntity>()
                .OneOrThrow();
            var getter = DelegateToFieldAdapter
                .TryAdapt<Getter<EntityHolder, ClassEntity>>(field)
                .OkOrThrow();
            Assert.NotNull(getter);

            int i = 0;
            foreach (var guid in TestData.InfiniteGuids().Take(10))
            {
                var holder = new EntityHolder(guid, $"{i++}");

                ClassEntity result = getter(ref holder);
                Assert.NotNull(result);
                Assert.Equal(holder.NameStampClassEntity, result);
            }
        }

        [Fact]
        public void AsInterfaceWorks()
        {
            var field = Prelude
                .Reflect<EntityHolder>()
                .Fields<NameStampClassEntity>()
                .OneOrThrow();
            var getter = DelegateToFieldAdapter
                .TryAdapt<Getter<EntityHolder, IEntity>>(field)
                .OkOrThrow();
            Assert.NotNull(getter);

            int i = 0;
            foreach (var guid in TestData.InfiniteGuids().Take(10))
            {
                var holder = new EntityHolder(guid, $"{i++}");

                IEntity result = getter(ref holder);
                Assert.NotNull(result);
                Assert.Equal(holder.NameStampClassEntity, result);
            }
        }
    }

    public class InstanceStructTests
    {
        [Fact]
        public void ExactWorks()
        {
            var field = Prelude.Reflect<StructEntity>().Fields<Guid>().OneOrThrow();
            var getter = DelegateToFieldAdapter
                .TryAdapt<Getter<StructEntity, Guid>>(field)
                .OkOrThrow();
            Assert.NotNull(getter);

            foreach (var guid in TestData.InfiniteGuids().Take(10))
            {
                var entity = new StructEntity(guid);
                Assert.Equal(guid, entity.Id);

                var result = getter(ref entity);
                Assert.Equal(guid, result);
            }
        }

        [Fact]
        public void AsObjectWorks()
        {
            var field = Prelude.Reflect<StructEntity>().Fields<Guid>().OneOrThrow();
            var getter = DelegateToFieldAdapter
                .TryAdapt<Getter<StructEntity, object>>(field)
                .OkOrThrow();
            Assert.NotNull(getter);

            foreach (var guid in TestData.InfiniteGuids().Take(10))
            {
                var entity = new StructEntity(guid);
                Assert.Equal(guid, entity.Id);

                object? result = getter(ref entity);
                Assert.NotNull(result);
                Assert.IsType<Guid>(result);
                Assert.Equal(guid, (Guid)result);
            }
        }

        [Fact]
        public void AsInterfaceWorks()
        {
            var field = Prelude
                .Reflect<EntityHolder>()
                .Fields<StructEntity>()
                .OneOrThrow();
            var getter = DelegateToFieldAdapter
                .TryAdapt<Getter<EntityHolder, IEntity>>(field)
                .OkOrThrow();
            Assert.NotNull(getter);

            int i = 0;
            foreach (var guid in TestData.InfiniteGuids().Take(10))
            {
                var holder = new EntityHolder(guid, $"{i++}");

                IEntity result = getter(ref holder);
                Assert.NotNull(result);
                Assert.Equal(holder.StructEntity, result);
            }
        }
    }


    [Fact]
    public void StaticExactGetterWorks()
    {
        var field = Prelude.Reflect(typeof(StaticEntity)).Fields<Guid>().OneOrThrow();
        var getter = DelegateToFieldAdapter
            .TryAdapt<Getter<None, Guid>>(field)
            .OkOrThrow();
        Assert.NotNull(getter);

        foreach (var guid in TestData.InfiniteGuids().Take(10))
        {
            StaticEntity.Id = guid;
            Assert.Equal(guid, StaticEntity.Id);

            var result = getter(ref None.Ref);
            Assert.Equal(guid, result);
        }
    }
}