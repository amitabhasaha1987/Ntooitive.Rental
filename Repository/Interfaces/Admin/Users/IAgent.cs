using System.Collections.Generic;
using Repositories.Interfaces.DataTable;
using Repositories.Models.Admin.User;
using Repositories.Models.Admin.Office;
using Repositories.Models.ListHub;
using Repositories.Models.Common;

namespace Repositories.Interfaces.Admin.Users
{
    public interface IAgent : IDataTable<User, UserDataTable>
    {
        bool UpdateUser(string participantId, string pathToUpdate, bool value);
        bool InsertBulkUsers(List<User> users);
        bool Insert(User user);

        bool DeleteUser(string uniquid, bool isDeleted);
        bool UpSertFromFeed(User user);
        bool UpdateUser(User user);
        User Login(string email, string password);
        User GetAgentDetails(string participantId);
        User GetAgentDetailsByEmail(string AgentEmail);
        User GetAgentDetailsByName(string name);
        bool InitiateRegistration(string participateId, string email);
        bool SetPassword(string participateId, string email, string hashPassword);
        bool DeactiveAgent(string uniquid, bool isActive);
        bool UploadProfileImage(string columnName, string UpdateType, string uniquid, string url);

        bool ResetPassword(string uniquid, string oldpwd, string newpwd);
        bool FeaturedAgent(string uniquid, bool isActivated);
        bool CertifiedAgent(string uniquid, bool isActivated);
        User IsAgentFeatured(string emailId);
        bool UpdateAgentListHub(User user);
        List<KeyValuePair<string, string>> GetBuilderList();

        List<User> GetAgents(string officeId);

        List<User> GetAgents();
        bool Validation(User user);

        List<User> GetCertifiedAgents(int count);

        List<User> GetFeaturedAgents(int count);
        IEnumerable<AutoCompleteDetails> GetAgentList(string searchKey);

        string GetPreviousImageUrl(string columnName, string uniqueId);
    }
}
