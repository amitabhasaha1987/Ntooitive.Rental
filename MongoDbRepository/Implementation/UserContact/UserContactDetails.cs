using System;
using System.Collections.Generic;
using Core.Repository;
using MongoDB.Driver;
using Repositories.Interfaces.ListHub;
using Repositories.Interfaces.NewHome;
using Repositories.Interfaces.UserContact;
using Repositories.Models.Common;
using Repositories.Models.NewHome;
using Repositories.Models.UserContact;
using Utility;

namespace Core.Implementation.UserContact
{
    public class UserContactDetails : Repository<UserContactAndFindDetails>, IUserContactDetails
    {
        private new const string CollectionName = "";
        IListHub _realEstate;
        INewHome _newHome;
        public UserContactDetails()
            : base(CollectionName)
        {
        }
        public UserContactDetails(IMongoDatabase database, IListHub realEstate, INewHome newHome)
            : base(database, CollectionName)
        {
            _realEstate = realEstate;
            _newHome = newHome;
        }
        public UserContactDetails(string connectionString, string databaseName)
            : base(connectionString, databaseName, CollectionName)
        {
        }
        public bool InsertUpdateDetailsAgainstUser(UserContactAndFindDetails userContactAndFindDetails, string Command, string SubCommand, string SelectedRow)
        {
            try
            {
                base.CollectionName = Convert.ToString(DbCollections.UserContactDetails);

                //var result = Command == "SearchPurchase" ? _realEstate.GetPurchasePropertyDetailsByMLSNumber(userContactAndFindDetails.MlsNumber) : _realEstate.GetRentPropertyDetailsByMLSNumber(userContactAndFindDetails.MlsNumber);
               PropertyDetails result=null;
                if (Command == "SearchPurchase")
                {
                    result = _realEstate.GetPurchaseRecordDetailsByMLSNumber(userContactAndFindDetails.MlsNumber);
                }
                else if (Command == "SearchRent")
                {
                    result = _realEstate.GetRentRecordDetailsByMLSNumber(userContactAndFindDetails.MlsNumber);
                }
                else if (Command == "SearchHome")
                {
                    result = _newHome.GetNewHomeRecordDetailsByMLSNumber(userContactAndFindDetails.MlsNumber);
                    if (result.Plans.Count > 0)
                    {
                        foreach (Plan item in result.Plans)
                        {
                            //if (item.PlanNumber == SelectedRow)
                            //{
                            /*
                                result.Bedrooms = item.Bedrooms.Text;
                                result.Bathrooms = item.Baths;
                                result.FullBathrooms = item.Baths;
                                result.HalfBathrooms = item.HalfBaths;
                                result.ListPrice = item.BasePrice.Text;
                                //.ToString("#,#", CultureInfo.InvariantCulture);
                                result.LivingArea = Convert.ToDouble(item.BaseSqft);
                                List<string> lstStr = new List<string>();
                                foreach (var item1 in item.PlanAmenity)
                                {
                                    lstStr.Add(item1.Text + " " + item1.Type);
                                }
                                result.Appliance = lstStr.ToArray();
                                result.ParkingType = new string[] { item.Garage.Text + " " + item.Garage.Entry };
                                result.NoOfParkingSpace = Convert.ToString(item.Garage.Text);
                                result.ListingDescription = item.Description;
                                result.PropertyType = item.Type;
                                result.ElevationImages = item.PlanImages.ElevationImage == null ? "" : item.PlanImages.ElevationImage.Text;
                                result.VirtualTour = item.PlanImages.VirtualTour;
                                result.PlanViewer = item.PlanImages.PlanViewer;
                                break;
                             */
                            //}
                        }
                    }
                }
                else
                {
                    if (SubCommand == "Purchase")
                    {
                        result = _realEstate.GetPurchaseRecordDetailsByMLSNumber(userContactAndFindDetails.MlsNumber);
                    }
                    else if (SubCommand == "Rent")
                    {
                        result = _realEstate.GetRentRecordDetailsByMLSNumber(userContactAndFindDetails.MlsNumber);
                    }
                    else
                    {
                        result = _newHome.GetNewHomeRecordDetailsByMLSNumber(userContactAndFindDetails.MlsNumber);
                        if (result.Plans.Count > 0)
                        {
                            foreach (Plan item in result.Plans)
                            {
                                //if (item.PlanNumber == SelectedRow)
                                //{
                                /*
                                    result.Bedrooms = item.Bedrooms.Text;
                                    result.Bathrooms = item.Baths;
                                    result.FullBathrooms = item.Baths;
                                    result.HalfBathrooms = item.HalfBaths;
                                    result.ListPrice = item.BasePrice.Text;
                                    result.LivingArea = Convert.ToDouble(item.BaseSqft);
                                    List<string> lstStr = new List<string>();
                                    foreach (var item1 in item.PlanAmenity)
                                    {
                                        lstStr.Add(item1.Text + " " + item1.Type);
                                    }
                                    result.Appliance = lstStr.ToArray();
                                    result.ParkingType = new string[] { item.Garage.Text + " " + item.Garage.Entry };
                                    result.NoOfParkingSpace = Convert.ToString(item.Garage.Text);
                                    result.ListingDescription = item.Description;
                                    result.PropertyType = item.Type;
                                    result.ElevationImages = item.PlanImages.ElevationImage == null ? "" : item.PlanImages.ElevationImage.Text;
                                    result.VirtualTour = item.PlanImages.VirtualTour;
                                    result.PlanViewer = item.PlanImages.PlanViewer;
                                    break;
                                 */
                                //}
                            }
                        }
                    }
                }
                var filter = Builders<UserContactAndFindDetails>.Filter.Eq("Email", userContactAndFindDetails.Email);
                var update = Builders<UserContactAndFindDetails>.Update
                    .Push("PreferenceList", result)
                    .Set("PhoneNo",userContactAndFindDetails.PhoneNo)
                    .Set("LastModifiedDate", DateTime.UtcNow);

                var results = GetCollection().UpdateOneAsync(filter, update, new UpdateOptions
                {
                    IsUpsert = true
                }).Result;

                return results.ModifiedCount > 0;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
