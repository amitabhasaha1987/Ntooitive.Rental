using Repositories.Models.UserContact;

namespace Repositories.Interfaces.UserContact
{
    public interface IUserContactDetails
    {
        bool InsertUpdateDetailsAgainstUser(UserContactAndFindDetails userContactAndFindDetails, string Command, string SubCommand, string SelectedRow);
    }
}
