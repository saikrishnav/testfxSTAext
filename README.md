# TestFx STA Extension
STA extension for MSTest framework and adapter <br/>
Adds the ability to run tests in STA thread for unit tests that depend on COM, OLE and UI components. <br/>
<br/>
Usage 1: <br/>
```C#
[STATestClass] // Runs all tests [ test1, test2, test3 ] in this class in STA thread
public class SampleTestClass
{
    [TestMethod] 
    public void Test1 () {....}
    [TestMethod]
    public void Test2 () {....}
    [STATestMethod] // Using STATestMethod inside a class which is already marked STA is redundant
    public void Test3 () {....}
}
```
<br/>
Usage 2: <br/>
```C#
[TestClass] // Runs all tests except Test3 in this class as MTA
public class SampleTestClass
{
    [TestMethod] 
    public void Test1 () {....}
    [TestMethod]
    public void Test2 () {....}
    [STATestMethod] // Class is NOT marked as STA - so only this method runs in STA - above tests run in MTA
    public void Test3 () {....}
}
```
