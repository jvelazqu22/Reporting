using System.Collections.Generic;
using System.Linq;

using Moq;

namespace iBank.UnitTesting.TestingHelpers.MockHelpers
{
    public class MockHelper
    {
        public static Mock<IQueryable<T>> GetListAsQueryable<T>(IList<T> list)
        {
            var mock = new Mock<IQueryable<T>>();
            mock.SetupIQueryable(list.AsQueryable());
            return mock;
        }
    }
}
