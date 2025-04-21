namespace AngleParse.Test.Resource.ElementResource;

public class InvalidElementResource() : AngleParse.Resource.ElementResource(Body)
{
    // AngleSharp tries to parse even broken html, so that never throws exception. 
    private const string Body = """!r3jmr23.30r u9f3.2$2394div><#/!"DFASh1>Title<h 2></#!"3ein>""";
}