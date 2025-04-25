namespace AngleParse.Resource;

public class StringResource(string str) : ObjectResource(str)
{
    public string String => str;
}