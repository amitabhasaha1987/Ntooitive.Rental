using System;
using System.Configuration;
using System.Net;
using Repositories.Interfaces.Mail;
using RestSharp;
using RestSharp.Authenticators;

namespace Core.Implementation.Mail
{
 public   class MailgunBase : IMailBase
    {
        public bool SendMail(string[] toEmails,  string subject,string body,string[] ccEmails= null)
        {
            
            var baseUrl = ConfigurationManager.AppSettings["Mailgun:BaseUrl"];
            var apiKey = ConfigurationManager.AppSettings["Mailgun:APIKey"];
            var domain = ConfigurationManager.AppSettings["Mailgun:Domain"];
            var form = ConfigurationManager.AppSettings["Mailgun:From"];
            var smtpDetails = ConfigurationManager.AppSettings["Mailgun:SMTP"];

            var client = new RestClient
            {
                BaseUrl = new Uri(baseUrl),
                Authenticator = new HttpBasicAuthenticator("api", apiKey)
            };
            var request = new RestRequest();
            request.AddParameter("domain", domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", form + "<" + smtpDetails + ">");

            foreach (var email in toEmails)
            {
                request.AddParameter("to", email);
            }

            if (ccEmails != null)
                foreach (var ccEmail in ccEmails)
                {
                    request.AddParameter("cc", ccEmail);
                }

            request.AddParameter("subject", subject);
           
            request.AddParameter("html", body);
            request.Method = Method.POST;
            var result = client.Execute(request);

            return result.StatusCode == HttpStatusCode.OK;
        }
    }
}
