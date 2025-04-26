using ScrubJay.Functional;
using ScrubJay.Reflection.Adapting;
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
            var setter = FieldSetterAdapter<ClassEntity, Guid>.Instance.TryAdapt(idField).OkOrThrow();
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
            var setter = FieldSetterAdapter<ClassEntity, object>.Instance.TryAdapt(idField).OkOrThrow();
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
            var setter = FieldSetterAdapter<EntityHolder, NameStampClassEntity>.Instance.TryAdapt(entityField).OkOrThrow();
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
            var idField = Prelude.Reflect<StructEntity>().Fields<Guid>().OneOrThrow();
            var setter = FieldSetterAdapter<StructEntity, Guid>.Instance.TryAdapt(idField).OkOrThrow();
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
            var idField = Prelude.Reflect<StructEntity>().Fields<Guid>().OneOrThrow();
            var setter = FieldSetterAdapter<StructEntity, object>.Instance.TryAdapt(idField).OkOrThrow();
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
        var idField = Prelude.Reflect(typeof(StaticEntity)).Fields<Guid>().OneOrThrow();
        var setter = FieldSetterAdapter<None, Guid>.Instance.TryAdapt(idField).OkOrThrow();
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