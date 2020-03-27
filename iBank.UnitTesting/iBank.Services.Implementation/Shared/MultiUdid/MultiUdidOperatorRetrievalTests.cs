using System;
using System.Collections.Generic;

using Domain.Helper;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.MultiUdid;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.MultiUdid
{
    [TestClass]
    public class MultiUdidOperatorRetrievalTests
    {
        private ReportGlobals _globals;

        private AdvancedParameters _multiUdidParameters;

        private MultiUdidOperatorRetrieval _sut;

        [TestInitialize]
        public void Init()
        {
            _globals = new ReportGlobals();
            _multiUdidParameters = new AdvancedParameters();
            _sut = new MultiUdidOperatorRetrieval();
        }

        [TestMethod]
        public void GetOperatorType_AllAreEqual_AndOperator_ReturnAllEqualsAndOperator()
        {
            _multiUdidParameters.AndOr = AndOr.And;
            _multiUdidParameters.Parameters = new List<AdvancedParameter> { new AdvancedParameter { Operator = Operator.Equal } };
            _globals.MultiUdidParameters = _multiUdidParameters;

            var output = _sut.GetOpertorType(_globals);

            Assert.AreEqual(MultiUdidCriteria.OperatorType.AllEqualsAndOperator, output);
        }

        [TestMethod]
        public void GetOperatorType_AllAreEqual_OrOperator_ReturnAllEqualsOrOperator()
        {
            _multiUdidParameters.AndOr = AndOr.Or;
            _multiUdidParameters.Parameters = new List<AdvancedParameter> { new AdvancedParameter { Operator = Operator.Equal } };
            _globals.MultiUdidParameters = _multiUdidParameters;

            var output = _sut.GetOpertorType(_globals);

            Assert.AreEqual(MultiUdidCriteria.OperatorType.AllEqualsOrOperator, output);
        }

        [TestMethod]
        public void GetOperatorType_AllAreNotEqual_AndOperator_ReturnAllEqualsAndOperator()
        {
            _multiUdidParameters.AndOr = AndOr.And;
            _multiUdidParameters.Parameters = new List<AdvancedParameter> { new AdvancedParameter { Operator = Operator.NotEqual } };
            _globals.MultiUdidParameters = _multiUdidParameters;

            var output = _sut.GetOpertorType(_globals);

            Assert.AreEqual(MultiUdidCriteria.OperatorType.AllNotEqualAndOperator, output);
        }

        [TestMethod]
        public void GetOperatorType_AllAreNotEqual_OrOperator_ReturnAllEqualsOrOperator()
        {
            _multiUdidParameters.AndOr = AndOr.Or;
            _multiUdidParameters.Parameters = new List<AdvancedParameter> { new AdvancedParameter { Operator = Operator.NotEqual } };
            _globals.MultiUdidParameters = _multiUdidParameters;

            var output = _sut.GetOpertorType(_globals);

            Assert.AreEqual(MultiUdidCriteria.OperatorType.AllNotEqualOrOperator, output);
        }

        [TestMethod]
        public void GetOperatorType_MixedEqualNotEqual_AndOperator_ReturnMixedTypeAndOperator()
        {
            _multiUdidParameters.AndOr = AndOr.And;
            _multiUdidParameters.Parameters = new List<AdvancedParameter> { new AdvancedParameter { Operator = Operator.Equal }, new AdvancedParameter { Operator = Operator.NotEqual } };
            _globals.MultiUdidParameters = _multiUdidParameters;

            var output = _sut.GetOpertorType(_globals);

            Assert.AreEqual(MultiUdidCriteria.OperatorType.MixedTypeAndOperator, output);
        }

        [TestMethod]
        public void GetOperatorType_MixedEqualNotEqual_OrOperator_ReturnMixedTypeOrOperator()
        {
            _multiUdidParameters.AndOr = AndOr.Or;
            _multiUdidParameters.Parameters = new List<AdvancedParameter> { new AdvancedParameter { Operator = Operator.Equal }, new AdvancedParameter { Operator = Operator.NotEqual } };
            _globals.MultiUdidParameters = _multiUdidParameters;

            var output = _sut.GetOpertorType(_globals);

            Assert.AreEqual(MultiUdidCriteria.OperatorType.MixedTypeOrOperator, output);
        }
    }
}
