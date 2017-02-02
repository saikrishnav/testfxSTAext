// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.VisualStudio.TestTools.UnitTesting.STAExtensions
{
    using Microsoft.TestFx.STAExtensions;
    using Microsoft.TestFx.STAExtensions.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class STATestMethodAttribute : TestMethodAttribute
    {
        private TestMethodAttribute actualTestMethodAttribute;

        private IThreadManagerFactory threadManagerFactory;

        public STATestMethodAttribute()
        {
            // Default constructor for reflection - Type.GetCustomAttributes API
        }

        internal STATestMethodAttribute(TestMethodAttribute actualTestMethodAttribute, IThreadManagerFactory threadManagerFactory)
        {
            // Store the actual test method attr
            this.actualTestMethodAttribute = actualTestMethodAttribute;
            this.threadManagerFactory = threadManagerFactory;
        }

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            Func<TestResult[]> func = () =>
                (this.actualTestMethodAttribute != null
                // If user used a custom testmethod attribute use its impl to execute test method
                ? this.actualTestMethodAttribute.Execute(testMethod)
                // user used STATestMethod attribute directly but not on class
                : base.Execute(testMethod));

            return this.threadManagerFactory.STAThreadManager.Execute(func);
        }
    }
}
