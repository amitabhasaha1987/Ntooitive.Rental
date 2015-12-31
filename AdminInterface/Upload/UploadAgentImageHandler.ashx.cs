using Repositories.Interfaces.Admin.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Repositories.Interfaces.Admin.Users;
using Repositories.Interfaces.Mail;
using Repositories.Models.Admin.User;
using Repositories.Models.DataTable;
using Repositories.Models.ListHub;
using Repositories.Models.ViewModel;
using Configuration;
using Utility;

namespace AdminInterface.Upload
{
    /// <summary>
    /// Summary description for UploadAgentImageHandler
    /// </summary>
    public class UploadAgentImageHandler : IHttpHandler
    {
        private readonly JavaScriptSerializer js;

        private string StorageRoot
        {
            get { return Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/SDRE/")); } //Path should! always end with '/'
        }

        public UploadAgentImageHandler()
        {
            js = new JavaScriptSerializer();
            js.MaxJsonLength = 41943040;
       
        }

        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Cache-Control", "private, no-cache");

            HandleMethod(context);
        }

        // Handle request based on method
        private void HandleMethod(HttpContext context)
        {
            switch (context.Request.HttpMethod)
            {
                case "HEAD":
                case "GET":
                    DeleteFile(context);
                    break;

                case "POST":
                case "PUT":
                    UploadFile(context);
                    break;

                //case "DELETE":
                //    DeleteFile(context);
                //    break;

                case "OPTIONS":
                    ReturnOptions(context);
                    break;

                default:
                    context.Response.ClearHeaders();
                    context.Response.StatusCode = 405;
                    break;
            }
        }

        private static void ReturnOptions(HttpContext context)
        {
            context.Response.AddHeader("Allow", "DELETE,GET,HEAD,POST,PUT,OPTIONS");
            context.Response.StatusCode = 200;
        }

        // Delete file from the server
        private void DeleteFile(HttpContext context)
        {
            var headers = context.Request.Headers;
            var filePath = StorageRoot + headers["X-File-Name"];
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        // Upload file to the server
        private void UploadFile(HttpContext context)
        {
            var statuses = new List<UploadAgentFileStatus>();
            var headers = context.Request.Headers;
            var uniqueID = context.Request.Form["agentId"];
            var columnName = context.Request.Form["columnName"];
            var UpdateType = context.Request.Form["UpdateType"];
            Console.WriteLine(uniqueID);
            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                UploadWholeFile(context, columnName, UpdateType, uniqueID, statuses);
            }
            else
            {
                UploadPartialFile(headers["X-File-Name"], context, statuses);
            }

            WriteJsonIframeSafe(context, statuses);
        }

        // Upload partial file
        private void UploadPartialFile(string fileName, HttpContext context, List<UploadAgentFileStatus> statuses)
        {
            if (context.Request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            var inputStream = context.Request.Files[0].InputStream;
            var fullName = StorageRoot + Path.GetFileName(fileName);

            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }
            statuses.Add(new UploadAgentFileStatus(new FileInfo(fullName)));
        }

        // Upload entire file
        private void UploadWholeFile(HttpContext context, string columnName, string UpdateType, string uniqueId, List<UploadAgentFileStatus> statuses)
        {
            string StorageRoot1 = System.Web.HttpContext.Current.Server.MapPath("~/SDRE/" + uniqueId);
            bool exists = System.IO.Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/SDRE/" + uniqueId));
            if (!exists)
                System.IO.Directory.CreateDirectory(StorageRoot1);

            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                var file = context.Request.Files[i];
                var fileName = file.FileName;
                var fullPath = StorageRoot1 + "\\" + fileName;
                file.SaveAs(fullPath);
                var url = context.Request.Url.Scheme + "://" + context.Request.Url.Authority + "/SDRE/" + uniqueId + "/" + fileName;
                string fullName = Path.GetFileName(file.FileName);
                statuses.Add(new UploadAgentFileStatus(url, file.ContentLength, fullPath));
                var _agent = NinjectConfig.Get<IAgent>();
                
                _agent.UploadProfileImage(columnName,UpdateType,uniqueId, url);
            }
        }

        private void WriteJsonIframeSafe(HttpContext context, List<UploadAgentFileStatus> statuses)
        {
            context.Response.AddHeader("Vary", "Accept");
            try
            {
                if (context.Request["HTTP_ACCEPT"].Contains("application/json"))
                    context.Response.ContentType = "application/json";
                else
                    context.Response.ContentType = "text/plain";
            }
            catch
            {
                context.Response.ContentType = "text/plain";
            }

            var jsonObj = js.Serialize(statuses.ToArray());
            context.Response.Write(jsonObj);
        }

        private static bool GivenFilename(HttpContext context)
        {
            return !string.IsNullOrEmpty(context.Request["f"]);
        }

        private void DeliverFile(HttpContext context)
        {
            var filename = context.Request["f"];
            var filePath = StorageRoot + filename;

            if (File.Exists(filePath))
            {
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                context.Response.ContentType = "application/octet-stream";
                context.Response.ClearContent();
                context.Response.WriteFile(filePath);
            }
            else
                context.Response.StatusCode = 404;
        }

        private void ListCurrentFiles(HttpContext context)
        {
            var files =
                new DirectoryInfo(StorageRoot)
                    .GetFiles("*", SearchOption.TopDirectoryOnly)
                    .Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden))
                    .Select(f => new UploadAgentFileStatus(f))
                    .ToArray();

            string jsonObj = js.Serialize(files);
            context.Response.AddHeader("Content-Disposition", "inline; filename=\"files.json\"");
            context.Response.Write(jsonObj);
            context.Response.ContentType = "application/json";
        }
    }
}