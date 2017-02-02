// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.VisualStudio.TestTools.UnitTesting.STAExtensions
{
    using Microsoft.TestFx.STAExtensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Threading;

    /// <summary>
    /// STATesClassAttribute defines [STATestClass] which runs all the test methods of the class in question under STAThread
    /// Note: STATestMethod Attribute is not required on test method if this attribute is defined at class level
    /// </summary>
    public class STATestClassAttribute : TestClassAttribute
    {
        public override TestMethodAttribute GetTestMethodAttribute(TestMethodAttribute testMethodAttribute)
        {
            // User decorated both testclass and testmethod as STATestClass and STATestMethod respectively
            // this is redundant - just return the method attr
            if (testMethodAttribute is STATestMethodAttribute) return testMethodAttribute;
            // either default testmethod attr or some other derived testmethod
            // Ensure we keep the given test method attr instance to ensure chained extensions: (For example: STATestClass -> DataTestMethod)
            else return new STATestMethodAttribute(testMethodAttribute, ThreadManagerFactory.Instance);
        }
    }
}
