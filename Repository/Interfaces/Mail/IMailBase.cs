namespace Repositories.Interfaces.Mail
{
    public interface IMailBase
    {
        bool SendMail(string[] toEmails, string subject, string body, string[] ccEmails= null);
    }
}
