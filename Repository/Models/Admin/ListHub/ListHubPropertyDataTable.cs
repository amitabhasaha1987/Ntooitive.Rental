namespace Repositories.Models.Admin.ListHub
{
    public class ListHubPropertyDataTable
    {
        public bool isImageSearchable { get; set; }
        public bool isMlsSearchable { get; set; }
        public bool isPriceSearchable { get; set; }
        public bool isPropertySearchable { get; set; }
        public bool isLivingArearSearchable { get; set; }
        public bool isBathSearchable { get; set; }
        public bool isBedSearchable { get; set; }
        public bool isNewConstuctionSearchable { get; set; }
        public bool isFeaturedSearchable { get; set; }
        public bool isAddressSearchable { get; set; }

        public bool isImageSortable { get; set; }
        public bool isMlsSortable { get; set; }
        public bool isPriceSortable { get; set; }
        public bool isPropertySortable { get; set; }
        public bool isLivingArearSortable { get; set; }
        public bool isBathSortable { get; set; }
        public bool isBedSortable { get; set; }
        public bool isNewConstuctionSortable { get; set; }
        public bool isFeaturedSortable { get; set; }
        public bool isAddressSortable { get; set; }


        public int sortColumnIndex { get; set; }
        public string sortDirection { get; set; }
    }
}