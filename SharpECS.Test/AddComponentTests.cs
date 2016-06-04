using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpECS.Samples.Components;
using System;
using System.Diagnostics;
using System.Linq;

namespace SharpECS.Test
{
    [TestClass]
    public class AddComponentTests
    {
        private EntityPool _entityPool;

        [TestInitialize]
        public void Setup()
        {
            _entityPool = EntityPool.New("EntityPool");
        }

        [TestCleanup]
        public void CleanUp()
        {
            _entityPool.WipeCache();
            _entityPool.WipeEntities();
        }

        [TestMethod]
        public void T1_ComponentsAddedProperly()
        {
            var entity = _entityPool.CreateEntity("entity");
            entity += new TransformComponent();
            entity += new GraphicsComponent();

            Assert.AreEqual(2, entity.Components.Count);
        }

        [TestMethod]
        public void T2_AddNoMoreThanOneComponentOfAType()
        {
            try
            {
                var entity = _entityPool.CreateEntity("entity");
                entity += new TransformComponent();
                entity += new TransformComponent();
                Assert.IsFalse(false);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(true);
            }
        }
    }
}