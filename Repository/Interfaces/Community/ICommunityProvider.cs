using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Interfaces.Base;
using Repositories.Models;
using Repositories.Models.Community;
using Repositories.Interfaces.DataTable;
using Repositories.Models.Admin.Community;
using Repositories.Models.Common;

namespace Repositories.Interfaces.Community
{
    public interface ICommunityProvider : IDataTable<Communities, CommunityDataTable>
    {
        void Process(string filePath);

        void InsertCommunities(List<Communities> communities);

        bool InsertFromFeed(List<Communities> communities);

        string[] GetCommunityName(string postalCode);
        List<Communities> GetCommunities();

        Communities GetCommunities(string p);
        void UpdateCommunities(Communities communities);
        void DeleteCommunities(string communityId);

        Communities GetCommunitiesByName(string communityName);
        //Communities GetCommunitiesBy(string p);
        bool Validation(Communities com);
        string GetPreviousImageUrl(string communityId);
    }
}
