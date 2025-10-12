namespace ExpenseTracker.Aspect
{
    [AspectInjector.Broker.Injection(typeof(WriteLog))]
    public class WriteLogAttribute : Attribute
    {
    }
}
