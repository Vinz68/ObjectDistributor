namespace ObjectDistributorUsage_Example;

/// <summary>
/// Example object to distribute (can be any class/object)
/// </summary>
/// <param name="message"></param>
public class MessageExample1(string message)
{
    public string Message { get; set; } = message;
}


public class MessageExample2(int x, int y)
{
    public string Result { get; set; } = (x + y).ToString();
}
