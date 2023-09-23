namespace Trivista.LoanApp.ApplicationCore.Entities.ValueObjects;

public sealed record ProfilePicture
{
    public string ProfilePictureFileName { get; set; }
    
    public string ProfilePictureFileType { get; set; }
    
    public long ProfilePictureFileLength { get; set; }
    
    public string ProfilePictureFile { get; set; }
}