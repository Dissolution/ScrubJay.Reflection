using ScrubJay.Functional;
using ScrubJay.Reflection.Adapting;

namespace ScrubJay.Reflection.Tests.RuntimeTests.PropertyAdapterTests;

public class GetterTests
{
    public class InstanceClassTests
    {
        [Fact]
        public void ExactWorks()
        {
            var property = Prelude.Reflect<ClassEntity>().Properties<Guid>().OneOrThrow();
            var getter = DelegateToPropertyAdapter.TryAdapt<Getter<ClassEntity, Guid>>(property).OkOrThrow();
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
            var property = Prelude.Reflect<ClassEntity>().Properties<Guid>().OneOrThrow();
            var getter = DelegateToPropertyAdapter.TryAdapt<Getter<ClassEntity, object>>(property).OkOrThrow();
            Assert.NotNull(getter);

            foreach (var guid in TestData.InfiniteGuids().Take(10))
            {
                var entity = new ClassEntity(guid);
                Assert.Equal(guid, entity.Id);

                object? result = getter(ref entity);
                Assert.NotNull(result);
                Assert.IsType<Guid>(result);
                Assert.Equal(guid, (Guid)result);
            }
        }

        [Fact]
        public void AsSubTypeWorks()
        {
            var property = Prelude
                .Reflect<EntityHolder>()
                .Properties<NameStampClassEntity>()
                .OneOrThrow();
            var getter = DelegateToPropertyAdapter.TryAdapt<Getter<EntityHolder, ClassEntity>>(property).OkOrThrow();
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
            var property = Prelude
                .Reflect<EntityHolder>()
                .Properties<NameStampClassEntity>()
                .OneOrThrow();
            var getter = DelegateToPropertyAdapter.TryAdapt<Getter<EntityHolder, IEntity>>(property).OkOrThrow();
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
            var property = Prelude.Reflect<StructEntity>().Properties<Guid>().OneOrThrow();
            var getter = DelegateToPropertyAdapter.TryAdapt<Getter<StructEntity, Guid>>(property).OkOrThrow();
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
            var property = Prelude.Reflect<StructEntity>().Properties<Guid>().OneOrThrow();
            var getter = DelegateToPropertyAdapter.TryAdapt<Getter<StructEntity, object>>(property).OkOrThrow();
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
            var property = Prelude
                .Reflect<EntityHolder>()
                .Properties<StructEntity>()
                .OneOrThrow();
            var getter = DelegateToPropertyAdapter.TryAdapt<Getter<EntityHolder, IEntity>>(property).OkOrThrow();
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
        var property = Prelude.Reflect(typeof(StaticEntity)).Properties<Guid>().OneOrThrow();
        var getter = DelegateToPropertyAdapter.TryAdapt<Getter<None, Guid>>(property).OkOrThrow();
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