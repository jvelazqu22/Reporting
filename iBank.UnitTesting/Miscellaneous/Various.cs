using AutoMapper;
using iBank.Entities.MasterEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace iBank.UnitTesting.Miscellaneous
{
    
    public class FooEntity { string Foo { get; set; } }
    public class FooViewModel { string Foo { get; set; } }
    public class BarEntity { string Bar { get; set; } }
    public class BarViewModel { string Bar { get; set; } }

    [TestClass]
    public class Various
    {

        [TestMethod]
        public void TestMethod1()
        {
            // arrange
            var QueuedRecord = new bcstque4();

            // act
            var result = QueuedRecord?.UserNumber ?? -1;

            // assert
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        [ExpectedException(typeof(AutoMapperMappingException), "Missing type map configuration or unsupported mapping.")]
        public void ReproduceProductionAutoMapperMappingException()
        {
            // arrange
            Mapper.Initialize(cfg => cfg.CreateMap<FooEntity, FooViewModel>());
            Mapper.Initialize(cfg => cfg.CreateMap<BarEntity, BarViewModel>()); // Culprit!
            var entity = new FooEntity();

            // act
            // AutoMapper.AutoMapperMappingException: Missing type map configuration or unsupported mapping.  
            Mapper.Map<FooViewModel>(entity);

            //  assert
        }

        [TestMethod]
        public void FixProductionAutoMapperMappingException()
        {
            // arrange
            Mapper.Initialize(cfg => {
                cfg.CreateMap<FooEntity, FooViewModel>();
                cfg.CreateMap<BarEntity, BarViewModel>();
            }); var entity = new FooEntity();

            // act
            var model = Mapper.Map<FooViewModel>(entity);

            //  assert
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void DateCheck()
        {
            // arrange
            var today = DateTime.Today;
            var tomorrow= today.AddDays(1);

            // act
            var isTomorrowGreaterThanToday = tomorrow > today;

            //  assert
            Assert.IsTrue(isTomorrowGreaterThanToday);
        }
    }
}
