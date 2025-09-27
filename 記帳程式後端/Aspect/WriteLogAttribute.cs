namespace 記帳程式後端.Aspect
{
    [AspectInjector.Broker.Injection(typeof(WriteLog))]
    public class WriteLogAttribute : Attribute
    {
    }
}
