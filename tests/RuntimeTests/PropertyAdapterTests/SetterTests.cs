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
            var property = Prelude.Reflect<ClassEntity>().Properties<Guid>().OneOrThrow();
            var setter = DelegateToPropertyAdapter.TryAdapt<Setter<ClassEntity, Guid>>(property).OkOrThrow();
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
            var property = Prelude.Reflect<ClassEntity>().Properties<Guid>().OneOrThrow();
            var setter = DelegateToPropertyAdapter.TryAdapt<Setter<ClassEntity, object>>(property).OkOrThrow();
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
            var property = Prelude
                .Reflect<EntityHolder>()
                .Properties<ClassEntity>()
                .OneOrThrow();
            var setter = DelegateToPropertyAdapter.TryAdapt<Setter<EntityHolder, NameStampClassEntity>>(property).OkOrThrow();
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
            var property = Prelude.Reflect<StructEntity>().Properties<Guid>().OneOrThrow();
            var setter = DelegateToPropertyAdapter.TryAdapt<Setter<StructEntity, Guid>>(property).OkOrThrow();
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
            var property = Prelude.Reflect<StructEntity>().Properties<Guid>().OneOrThrow();
            var setter = DelegateToPropertyAdapter.TryAdapt<Setter<StructEntity, object>>(property).OkOrThrow();
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
        var property = Prelude.Reflect(typeof(StaticEntity)).Properties<Guid>().OneOrThrow();
        var setter = DelegateToPropertyAdapter.TryAdapt<Setter<None, Guid>>(property).OkOrThrow();
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