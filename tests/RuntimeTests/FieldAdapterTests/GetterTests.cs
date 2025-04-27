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
            var idField = Prelude.Reflect<ClassEntity>().Fields<Guid>().OneOrThrow();
            var getter = FieldGetterAdapter<ClassEntity, Guid>.Instance.TryAdapt(idField).OkOrThrow();
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
            var idField = Prelude.Reflect<ClassEntity>().Fields<Guid>().OneOrThrow();
            var getter = FieldGetterAdapter<ClassEntity, object>.Instance.TryAdapt(idField).OkOrThrow();
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
            var entityField = Prelude
                .Reflect<EntityHolder>()
                .Fields<NameStampClassEntity>()
                .OneOrThrow();
            var getter = FieldGetterAdapter<EntityHolder, ClassEntity>.Instance.TryAdapt(entityField).OkOrThrow();
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
            var entityField = Prelude
                .Reflect<EntityHolder>()
                .Fields<NameStampClassEntity>()
                .OneOrThrow();
            var getter = FieldGetterAdapter<EntityHolder, IEntity>.Instance.TryAdapt(entityField).OkOrThrow();
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
            var idField = Prelude.Reflect<StructEntity>().Fields<Guid>().OneOrThrow();
            var getter = FieldGetterAdapter<StructEntity, Guid>.Instance.TryAdapt(idField).OkOrThrow();
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
            var idField = Prelude.Reflect<StructEntity>().Fields<Guid>().OneOrThrow();
            var getter = FieldGetterAdapter<StructEntity, object>.Instance.TryAdapt(idField).OkOrThrow();
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
            var entityField = Prelude
                .Reflect<EntityHolder>()
                .Fields<StructEntity>()
                .OneOrThrow();
            var getter = FieldGetterAdapter<EntityHolder, IEntity>.Instance.TryAdapt(entityField).OkOrThrow();
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
        var idField = Prelude.Reflect(typeof(StaticEntity)).Fields<Guid>().OneOrThrow();
        var getter = FieldGetterAdapter<None, Guid>.Instance.TryAdapt(idField).OkOrThrow();
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