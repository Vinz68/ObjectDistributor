namespace ObjectDistributorUsage_Example;

public class MessageExample1
{
    public MessageExample1(string message)
    {
        Message = message;
    }
    public string Message { get; set; }
}


public class MessageExample2
{
    public MessageExample2(int x, int y)
    {
        Result = (x + y).ToString();
    }
    public string Result { get; set; }
}
