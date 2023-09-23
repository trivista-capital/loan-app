namespace Trivista.LoanApp.ApplicationCore.Commons.Helpers;

public static class FileHelper
{
    public static string ExtractMailTemplate(string fileName)
    {
        var mailTemplate = Path.Combine(Directory.GetCurrentDirectory(), $"Email_Templates_Borrow_Ease/{fileName}");
        string[] lines = File.ReadAllLines(mailTemplate);
        var template = StringHelper.ConvertArrayToString(lines);
        return template;
    }
}