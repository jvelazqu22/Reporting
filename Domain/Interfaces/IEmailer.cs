using Domain.Helper;

namespace Domain.Interfaces
{
    public interface IEmailer
    {
        bool SendEmail(EmailInformation emailInfo);
    }
}
