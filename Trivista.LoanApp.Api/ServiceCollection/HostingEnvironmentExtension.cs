namespace Trivista.LoanApp.Api.ServiceCollection;

public static class HostingEnvironmentExtension
{
    private const string LocalEnvironment = "Local";
    private const string TestEnvironment = "Test";

    public static bool IsLocal(this IWebHostEnvironment hostingEnvironment)
    {
        return hostingEnvironment.IsEnvironment(LocalEnvironment);
    }
    public static bool IsTest(this IWebHostEnvironment hostingEnvironment)
    {
        return hostingEnvironment.IsEnvironment(TestEnvironment);
    }
}