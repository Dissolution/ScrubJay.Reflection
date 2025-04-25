using ScrubJay.Functional;
using ScrubJay.Reflection.Runtime;

namespace ScrubJay.Reflection.Tests.RuntimeTests.FieldThingsTests;

public class SetterTests
{
    public class InstanceClassTests
    {
        [Fact]
        public void ExactWorks()
        {
            var idField = Prelude.Reflect<ClassEntity>().Fields<Guid>().OneOrThrow();
            var setter = FieldThings.CreateSetter<ClassEntity, Guid>(idField);
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
            var idField = Prelude.Reflect<ClassEntity>().Fields<Guid>().OneOrThrow();
            var setter = FieldThings.CreateSetter<ClassEntity, object>(idField);
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
            var entityField = Prelude
                .Reflect<EntityHolder>()
                .Fields<ClassEntity>()
                .OneOrThrow();
            var setter = FieldThings.CreateSetter<EntityHolder, NameStampClassEntity>(entityField);
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

        [Fact]
        public void AsInterfaceWorks()
        {
            var entityField = Prelude
                .Reflect<EntityHolder>()
                .Fields<NameStampClassEntity>()
                .OneOrThrow();
            var setter = FieldThings.CreateSetter<EntityHolder, IEntity>(entityField);
            Assert.NotNull(setter);

            int i = 0;
            foreach (var guid in TestData.InfiniteGuids().Take(10))
            {
                var holder = new EntityHolder(guid, $"{i++}");

                IEntity result = setter(ref holder);
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
            var setter = FieldThings.CreateSetter<StructEntity, Guid>(idField);
            Assert.NotNull(setter);

            foreach (var guid in TestData.InfiniteGuids().Take(10))
            {
                var entity = new StructEntity(guid);
                Assert.Equal(guid, entity.Id);

                var result = setter(ref entity);
                Assert.Equal(guid, result);
            }
        }

        [Fact]
        public void AsObjectWorks()
        {
            var idField = Prelude.Reflect<StructEntity>().Fields<Guid>().OneOrThrow();
            var setter = FieldThings.CreateSetter<StructEntity, object>(idField);
            Assert.NotNull(setter);

            foreach (var guid in TestData.InfiniteGuids().Take(10))
            {
                var entity = new StructEntity(guid);
                Assert.Equal(guid, entity.Id);

                object? result = setter(ref entity);
                Assert.NotNull(result);
                Assert.IsType(typeof(Guid), result);
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
            var setter = FieldThings.CreateSetter<EntityHolder, IEntity>(entityField);
            Assert.NotNull(setter);

            int i = 0;
            foreach (var guid in TestData.InfiniteGuids().Take(10))
            {
                var holder = new EntityHolder(guid, $"{i++}");

                IEntity result = setter(ref holder);
                Assert.NotNull(result);
                Assert.Equal(holder.StructEntity, result);
            }
        }
    }


    [Fact]
    public void StaticExactSetterWorks()
    {
        var idField = Prelude.Reflect(typeof(StaticEntity)).Fields<Guid>().OneOrThrow();
        var setter = FieldThings.CreateSetter<None, Guid>(idField);
        Assert.NotNull(setter);

        foreach (var guid in TestData.InfiniteGuids().Take(10))
        {
            StaticEntity.Id = guid;
            Assert.Equal(guid, StaticEntity.Id);

            var result = setter(ref None.Ref);
            Assert.Equal(guid, result);
        }
    }
}