using Newtonsoft.Json;

namespace Trivista.LoanApp.ApplicationCore.Services.Mail;

public class MailRoot
{
    public Recipients Recipients { get; set; }
    public Content Content { get; set; }
    public Options Options { get; set; }
}

   public class Attachment
    {
        public string BinaryContent { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string Size { get; set; }
    }

    public class MailBodyBody
    {
        public string ContentType { get; set; }
        public string Content { get; set; }
        public string Charset { get; set; }
    }

    public class Content
    {
        public List<MailBodyBody> Body { get; set; }
        public Merge Merge { get; set; }
        public List<Attachment> Attachments { get; set; }
        public Headers Headers { get; set; }
        public string Postback { get; set; }
        public string EnvelopeFrom { get; set; }
        public string From { get; set; }
        public string ReplyTo { get; set; }
        public string Subject { get; set; }
        public string TemplateName { get; set; }
        public string AttachFiles { get; set; }
        public Utm Utm { get; set; }
    }

    public class Headers
    {
        public string city { get; set; }
        public string age { get; set; }
    }

    public class Merge
    {
        public string city { get; set; }
        public string age { get; set; }
    }

    public class Options
    {
        public object TimeOffset { get; set; }
        public string PoolName { get; set; }
        public string ChannelName { get; set; }
        public string Encoding { get; set; }
        public string TrackOpens { get; set; }
        public string TrackClicks { get; set; }
    }

    public class Recipients
    {
        public List<string> To { get; set; }
        public List<string> CC { get; set; }
        public List<string> BCC { get; set; }
    }

public class Utm
    {
        public string Source { get; set; }
        public string Medium { get; set; }
        public string Campaign { get; set; }
        public string Content { get; set; }
    }

public class MailObject
{
    [JsonProperty("apikey")]
    public string ApiKey { get; set; }
    
    [JsonProperty("bodyAmp")]
    public string BodyAmp { get; set; }
    
    [JsonProperty("charset")]
    public string CharSet { get; set; }
    
    [JsonProperty("from")]
    public string From { get; set; }
    
    [JsonProperty("isTransactional")]
    public bool IsTransactional { get; set; } = true;
    
    // [JsonProperty("msgTo")]
    // public List<string> MsgTo { get; set; }
    
    //[JsonProperty("msgTo")]
    public string To { get; set; }
    
    [JsonProperty("sender")]
    public string Sender { get; set; }
    
    [JsonProperty("senderName")]
    public string SenderName { get; set; }
    
    [JsonProperty("subject")]
    public string Subject { get; set; }
}

public class MailObjectResponse
{
    public string TransactionID { get; set; }
    
    public string MessageID { get; set; }
}