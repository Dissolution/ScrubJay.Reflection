using ScrubJay.Functional;
using ScrubJay.Reflection.Adapting;

namespace ScrubJay.Reflection.Tests.RuntimeTests.PropertyAdapterTests;

public class SetterTests
{
    public class InstanceClassTests
    {
        [Fact]
        public void ExactWorks()
        {
            var idProperty = Prelude.Reflect<ClassEntity>().Properties<Guid>().OneOrThrow();
            var setter = PropertySetterAdapter<ClassEntity, Guid>.Instance.TryAdapt(idProperty).OkOrThrow();
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
            var idProperty = Prelude.Reflect<ClassEntity>().Properties<Guid>().OneOrThrow();
            var setter = PropertySetterAdapter<ClassEntity, object>.Instance.TryAdapt(idProperty).OkOrThrow();
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
            var entityProperty = Prelude
                .Reflect<EntityHolder>()
                .Properties<ClassEntity>()
                .OneOrThrow();
            var setter = PropertySetterAdapter<EntityHolder, NameStampClassEntity>.Instance.TryAdapt(entityProperty).OkOrThrow();
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
            var idProperty = Prelude.Reflect<StructEntity>().Properties<Guid>().OneOrThrow();
            var setter = PropertySetterAdapter<StructEntity, Guid>.Instance.TryAdapt(idProperty).OkOrThrow();
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
            var idProperty = Prelude.Reflect<StructEntity>().Properties<Guid>().OneOrThrow();
            var setter = PropertySetterAdapter<StructEntity, object>.Instance.TryAdapt(idProperty).OkOrThrow();
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
        var idProperty = Prelude.Reflect(typeof(StaticEntity)).Properties<Guid>().OneOrThrow();
        var setter = PropertySetterAdapter<None, Guid>.Instance.TryAdapt(idProperty).OkOrThrow();
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