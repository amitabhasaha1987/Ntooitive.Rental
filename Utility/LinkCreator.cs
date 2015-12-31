using System;
using System.Configuration;
using System.Text;

namespace Utility
{
    public static class LinkCreator
    {
        public static string CreateAgentInvitationLink(string uniqueid)
        {
            var currentDomain = ConfigurationManager.AppSettings["URL"];

            var hashUniqueId = UtilityClass.Base64Encode(uniqueid);
            return currentDomain + "agent/activate/"+ hashUniqueId;
        }
    }
}