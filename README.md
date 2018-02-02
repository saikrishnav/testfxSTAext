# TestFx STA Extension
STA extension for MSTest framework and adapter  
Adds the ability to run tests in STA thread for unit tests that depend on COM, OLE and UI components.  
  
### Usage 1:
  
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
  
### Usage 2:
  
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
