// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace STAExtensions.UnitTests
{
    using Microsoft.TestFx.STAExtensions.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting.STAExtensions;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestClass]
    public class STATestMethodAttributeTests
    {
        [TestMethod]
        public void STATestMethodAttributeMustHaveDefaultConstructorAndInheritTestMethodAttribute()
        {
            var attr = new STATestMethodAttribute();
            Assert.IsInstanceOfType(attr, typeof(TestMethodAttribute), "STATestMethodAttribute Must Inherit TestMethodAttribute.");
        }

        [TestMethod]
        public void ExecuteMustCallTheFactoryAndUseSTAThreadManagerToExecuteTheMethod()
        {
            var mockFactory = new Mock<IThreadManagerFactory>();
            var attr = new STATestMethodAttribute(null, mockFactory.Object);
            var mockITestMethod = new Mock<ITestMethod>();

            var mockThreadManager = new Mock<IThreadManager<TestResult[]>>();
            mockFactory.Setup(mf => mf.STAThreadManager).Returns(mockThreadManager.Object);

            attr.Execute(mockITestMethod.Object);

            mockThreadManager.Verify(mf => mf.Execute(It.IsAny<Func<TestResult[]>>()), Times.Once);
        }


        [TestMethod]
        public void ExecuteMustSucceedIfTestMethodAttributeInstanceIsNotProvided()
        {
            var mockFactory = new Mock<IThreadManagerFactory>();
            var attr = new STATestMethodAttribute(null, mockFactory.Object);
            var mockITestMethod = new Mock<ITestMethod>();

            var mockThreadManager = new Mock<IThreadManager<TestResult[]>>();
            mockFactory.Setup(mf => mf.STAThreadManager).Returns(mockThreadManager.Object);

            mockThreadManager.Setup(mf => mf.Execute(It.IsAny<Func<TestResult[]>>())).Callback((Func<TestResult[]> func) =>
            {
                func.Invoke();
            });

            attr.Execute(mockITestMethod.Object);

            mockITestMethod.Verify(mi => mi.Invoke(It.IsAny<object[]>()), Times.Once);
        }

        [TestMethod]
        public void ExecuteMustSucceedByCallingProvidedTestMethodAttributeInstanceIfProvided()
        {
            var mockFactory = new Mock<IThreadManagerFactory>();
            var mockAttr = new Mock<TestMethodAttribute>();
            var attr = new STATestMethodAttribute(mockAttr.Object, mockFactory.Object);
            var mockITestMethod = new Mock<ITestMethod>();

            var mockThreadManager = new Mock<IThreadManager<TestResult[]>>();
            mockFactory.Setup(mf => mf.STAThreadManager).Returns(mockThreadManager.Object);

            mockThreadManager.Setup(mf => mf.Execute(It.IsAny<Func<TestResult[]>>())).Callback((Func<TestResult[]> func) =>
            {
                func.Invoke();
            });

            attr.Execute(mockITestMethod.Object);

            mockAttr.Verify(ma => ma.Execute(mockITestMethod.Object), Times.Once);
        }
    }
}
