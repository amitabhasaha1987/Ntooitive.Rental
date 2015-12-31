using MongoDB.Bson;

namespace Repositories.Models.Base
{
    public abstract class BaseEntity
    {
        public virtual ObjectId Id { get; set; }
        public string ListedBy { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public bool IsUpdateByPortal { get; set; }
        public bool IsDeletedByPortal { get; set; }
    }
}
