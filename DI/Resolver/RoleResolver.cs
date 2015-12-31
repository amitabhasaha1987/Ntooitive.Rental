using AutoMapper;
using Repositories.Models.Classified;
using Repositories.Models.Common;
using Repositories.Models.ListHub;
using Repositories.Models.NewHome;
using Repositories.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Configuration.Resolver
{
    public class RoleResolver : ValueResolver<Participant, string[]>
    {
        protected override string[] ResolveCore(Participant source)
        {
            return new[] { "Agent", source.Role };
        }
    }

    public class PhotoResolver : ValueResolver<ListHubListing, string[]>
    {
        protected override string[] ResolveCore(ListHubListing m)
        {
            if (m.Photos != null && m.Photos.Photo.Count > 0)
            {
                return new string[] { m.Photos.Photo[0].MediaURL };
            }
            else
            {
                return new string[] { };
            }
        }
    }
    public class AgentNameResolver : ValueResolver<ListHubListing, string>
    {
        protected override string ResolveCore(ListHubListing m)
        {
            if (m.ListingParticipants != null)
            {
                if (m.ListingParticipants.Participant != null)
                {
                    return string.IsNullOrEmpty(m.ListingParticipants.Participant.LastName) ? m.ListingParticipants.Participant.FirstName : m.ListingParticipants.Participant.FirstName + " " + m.ListingParticipants.Participant.LastName;
                }
                else
                {
                    return string.Empty;
                }
                //return new string[] { m.Photos.Photo[0].MediaURL };
            }
            else
            {
                return string.Empty;
            }
        }
    }
    public class IsClassifiedResolver : ValueResolver<ListHubListing, bool>
    {
        protected override bool ResolveCore(ListHubListing m)
        {
            if (m.ExpireDate != null && m.ExpireDate.Value != DateTime.MaxValue.Date)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class HasListHubAddressResolver : ValueResolver<ListHubListing, bool>
    {
        protected override bool ResolveCore(ListHubListing m)
        {
            if (m.Address != null)
            {
                if ((!string.IsNullOrEmpty(m.Address.FullStreetAddress)
                    && !string.IsNullOrEmpty(m.Address.StateOrProvince)
                    && !string.IsNullOrEmpty(m.Address.City))
                    || !string.IsNullOrEmpty(m.Address.PostalCode))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
    }
    public class HasAddressResolver : ValueResolver<Properties, bool>
    {
        protected override bool ResolveCore(Properties m)
        {
            if (m.Property != null)
            {
                if ((!string.IsNullOrEmpty(m.Property.Address1)
                    && !string.IsNullOrEmpty(m.Property.State)
                    && !string.IsNullOrEmpty(m.Property.City))
                    || !string.IsNullOrEmpty(m.Property.Zip))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
    }

    /*
    public class NewHomePhotoResolver : ValueResolver<NewHomeListing, string[]>
    {
        protected override string[] ResolveCore(NewHomeListing m)
        {
            List<string> lstPhoto = new List<string>();
            if (m.Subdivision.SubImage != null && m.Subdivision.SubImage.Count > 0)
            {
                return new string[] { m.Subdivision.SubImage[0].Text };
            }
            else
            {
                return new string[] { };
            }
        }
    }
    */
    public class AttributeResolver : ValueResolver<ListHubListing, string[]>
    {
        protected override string[] ResolveCore(ListHubListing m)
        {
            List<string> attribute = new List<string>();
            if (!String.IsNullOrEmpty(m.Address.FullStreetAddress))
            {
                attribute.Add(m.Address.FullStreetAddress.ToLower());
            }

            if (!String.IsNullOrEmpty(m.Address.StateOrProvince))
            {
                attribute.Add(m.Address.StateOrProvince.ToLower());
            }

            if (!String.IsNullOrEmpty(m.Address.City))
            {
                attribute.Add(m.Address.City.ToLower());
            }
            if (!String.IsNullOrEmpty(m.Address.PostalCode))
            {
                attribute.Add(m.Address.PostalCode.ToLower());
            }
            if (!String.IsNullOrEmpty(m.Address.Country))
            {
                attribute.Add(m.Address.Country.ToLower());
            }
            if (m.Bedrooms != 0)
            {
                attribute.Add(m.Bedrooms.ToString() + " " + Abbreviation.Bedrooms);
            }

            if (m.Bathrooms != 0)
            {
                attribute.Add(m.Bathrooms.ToString() + " " + Abbreviation.Bathrooms);
            }


            if (m.PartialBathrooms != 0)
            {
                attribute.Add(m.PartialBathrooms.ToString() + " " + Abbreviation.PartialBathrooms);
            }

            if (m.OneQuarterBathrooms != 0)
            {
                attribute.Add(m.OneQuarterBathrooms.ToString() + " " + Abbreviation.OneQuarterBathrooms);
            }

            if (m.HalfBathrooms != 0)
            {
                attribute.Add(m.HalfBathrooms.ToString() + " " + Abbreviation.HalfBathrooms);
            }

            if (m.ThreeQuarterBathrooms != 0)
            {
                attribute.Add(m.ThreeQuarterBathrooms.ToString() + " " + Abbreviation.ThreeQuarterBathrooms);
            }

            if (m.DetailedCharacteristics != null)
            {
                if (m.DetailedCharacteristics.Appliances != null)
                {
                    m.DetailedCharacteristics.Appliances.Appliance.ForEach(x => attribute.Add(x.ToLower()));
                }
                if (m.DetailedCharacteristics.ArchitectureStyle != null)
                {
                    if (m.DetailedCharacteristics.ArchitectureStyle.OtherDescription != null)
                    {
                        if (!attribute.Contains(m.DetailedCharacteristics.ArchitectureStyle.OtherDescription.ToLower() + " " + Abbreviation.ArchitectureStyle))
                        {
                            if (!string.IsNullOrEmpty(m.DetailedCharacteristics.ArchitectureStyle.OtherDescription))
                                attribute.Add(m.DetailedCharacteristics.ArchitectureStyle.OtherDescription.ToLower() + " " + Abbreviation.ArchitectureStyle);

                        }
                    }

                    if (m.DetailedCharacteristics.ArchitectureStyle.Text != null)
                    {
                        if (!attribute.Contains(m.DetailedCharacteristics.ArchitectureStyle.Text.ToLower() + " " + Abbreviation.ArchitectureStyle))
                        {
                            if (!string.IsNullOrEmpty(m.DetailedCharacteristics.ArchitectureStyle.Text))
                                attribute.Add(m.DetailedCharacteristics.ArchitectureStyle.Text.ToLower() + " " + Abbreviation.ArchitectureStyle);
                        }
                    }


                }
                if (m.DetailedCharacteristics.CoolingSystems != null)
                {
                    attribute.Add(m.DetailedCharacteristics.CoolingSystems.CoolingSystem.ToLower());
                    attribute.Add(Abbreviation.CoolingSystems);
                }
                if (m.DetailedCharacteristics.ExteriorTypes != null)
                {
                    m.DetailedCharacteristics.ExteriorTypes.ExteriorType.ForEach(x => attribute.Add(x.ToLower()));
                }
                if (m.DetailedCharacteristics.HasBasement)
                {
                    attribute.Add(Abbreviation.BaseMent);
                }
                if (m.DetailedCharacteristics.HeatingSystems != null)
                {
                    attribute.Add(m.DetailedCharacteristics.HeatingSystems.HeatingSystem.ToLower());
                    attribute.Add(Abbreviation.HeatingSystems);

                }

                if (m.DetailedCharacteristics.IsNewConstruction)
                {
                    attribute.Add(Abbreviation.NewConstruction);
                }

                if (m.DetailedCharacteristics.NumFloors != 0.0)
                {
                    attribute.Add(m.DetailedCharacteristics.NumFloors + " " + Abbreviation.Floors);
                }

                if (m.DetailedCharacteristics.NumParkingSpaces != null)
                {
                    attribute.Add(m.DetailedCharacteristics.NumParkingSpaces + " " + Abbreviation.Parkings);
                }

                if (m.DetailedCharacteristics.ParkingTypes != null)
                {
                    m.DetailedCharacteristics.ParkingTypes.ParkingType.ForEach(x => attribute.Add(x.ToLower()));
                }

                if (m.DetailedCharacteristics.RoofTypes != null)
                {
                    if (!string.IsNullOrEmpty(m.DetailedCharacteristics.RoofTypes.RoofType))
                        attribute.Add(m.DetailedCharacteristics.RoofTypes.RoofType.ToLower() + " " + Abbreviation.RoofTypes);

                }

                if (m.DetailedCharacteristics.Rooms != null)
                {
                    var distinctRoomType = m.DetailedCharacteristics.Rooms.Room.Distinct();

                    attribute.AddRange(
                        from roomType in distinctRoomType
                        where roomType.ToLower() != "bedroom"
                        && roomType.ToLower() != "full bath"
                        && roomType.ToLower() != "three-quarter bath"
                        && roomType.ToLower() != "one-quarter bath"
                        && roomType.ToLower() != "half bath"
                        select roomType.ToLower());
                }
            }


            if (m.Location != null && m.Location.Community != null)
            {
                m.Location.Community.Schools.School.ForEach(s =>
                {
                    if (!string.IsNullOrEmpty(s.Name))
                        attribute.Add(s.Name.ToLower());
                });
            }

            if (m.CommunityName != null)
            {
                m.CommunityName.ToList().ForEach(s =>
                {
                    if (!string.IsNullOrEmpty(s))
                        attribute.Add(s.ToLower());
                    if (!attribute.Contains(s.ToLower()))
                        attribute.Add(s.ToLower());
                });
            }
            if (m.ListingParticipants != null)
            {
                if (!String.IsNullOrEmpty(m.ListingParticipants.Participant.LastName))
                {
                    attribute.Add(m.ListingParticipants.Participant.FirstName.ToLower() + " " + m.ListingParticipants.Participant.LastName.ToLower());
                }
                else
                {
                    attribute.Add(m.ListingParticipants.Participant.FirstName.ToLower());
                }
            }
            return attribute.ToArray();
        }
    }

    public class NowHomeImages : ValueResolver<Plan, string[]>
    {
        protected override string[] ResolveCore(Plan source)
        {
            List<string> attribute = new List<string>();
            if (source.Images != null && source.Images.Image != null && source.Images.Image.Count > 0)
            {
                source.Images.Image.Select(x => x.Reference).ToList().ForEach(x => attribute.Add(x));
            }
            return attribute.ToArray();
        }
    }

    public class NewHomeAttributeResolver : ValueResolver<Plan, string[]>
    {
        protected override string[] ResolveCore(Plan m)
        {
            List<string> attribute = new List<string>();

            if (!string.IsNullOrEmpty(m.CommunityName))
            {
                attribute.Add(m.CommunityName.ToLower());
            }
            if (!string.IsNullOrEmpty(m.FullAddress))
            {
                attribute.Add(m.FullAddress.ToLower());
            }
            if (!string.IsNullOrEmpty(m.Communityaddress))
            {
                attribute.Add(m.Communityaddress.ToLower());
            }
            if (!string.IsNullOrEmpty(m.Communitystate))
            {
                attribute.Add(m.Communitystate.ToLower());
            }
            if (!string.IsNullOrEmpty(m.Communitycity))
            {
                attribute.Add(m.Communitycity.ToLower());
            }
            if (!string.IsNullOrEmpty(m.Communityzip))
            {
                attribute.Add(m.Communityzip.ToLower());
            }


            if (m.Bedrooms != 0)
            {
                attribute.Add(Convert.ToString(m.Bedrooms) + " " + Abbreviation.Bedrooms);
            }
            if (m.Baths != 0)
            {
                attribute.Add(Convert.ToString(m.Baths) + " " + Abbreviation.Bathrooms);
            }
            if (m.Half_baths != 0)
            {
                attribute.Add(m.Half_baths + " " + Abbreviation.HalfBathrooms);
            }
            if (m.Garage != 0)
            {
                attribute.Add(m.Garage + " " + Abbreviation.Parkings);
            }

            if (!String.IsNullOrEmpty(m.BuilderName))
            {
                attribute.Add(m.BuilderName.ToLower());
            }

            return attribute.ToArray();
        }
    }
    public class AgentNameAttributeResolver : ValueResolver<ListHubListing, string>
    {
        protected override string ResolveCore(ListHubListing m)
        {
            string Name = string.Empty;
            if (!String.IsNullOrEmpty(m.ListingParticipants.Participant.LastName))
            {
                return m.ListingParticipants.Participant.FirstName + " " + m.ListingParticipants.Participant.LastName;
            }
            else
            {
                return m.ListingParticipants.Participant.FirstName;
            }

        }
    }
    public class NewHomePriceResolver : ValueResolver<Plan, double>
    {
        protected override double ResolveCore(Plan m)
        {
            string Name = string.Empty;
            if (!String.IsNullOrEmpty(m.Base_price))
            {
                return Convert.ToDouble(m.Base_price);
            }
            else
            {
                return 0;
            }

        }
    }
    #region "Added for ClassifiedFeed"


    public class ClassifiedFeedBathroomAttributeResolver : ValueResolver<Properties, int>
    {
        protected override int ResolveCore(Properties m)
        {
            int size = 0;
            if (!String.IsNullOrEmpty(m.Property.Bathroomnumber))
            {
                if (m.Property.Bathroomnumber.Contains('.'))
                {

                    string[] strArr = m.Property.Bathroomnumber.Split('.');

                    return Convert.ToInt16(strArr[0]);
                }
                else
                {
                    return Convert.ToInt16(m.Property.Bathroomnumber);
                }
            }
            else
            {
                return size;
            }

        }
    }

    public class ClassifiedFeedHalfBathroomAttributeResolver : ValueResolver<Properties, int>
    {
        protected override int ResolveCore(Properties m)
        {
            int size = 0;
            if (!String.IsNullOrEmpty(m.Property.Bathroomnumber))
            {
                if (m.Property.Bathroomnumber.Contains('.'))
                {

                    string[] strArr = m.Property.Bathroomnumber.Split('.');
                    size = Convert.ToInt16(strArr[1]);
                    if (size == 5)
                        return 1;
                    else
                        return Convert.ToInt16(strArr[1]);
                }
                else
                {
                    return size;
                }
            }
            else
            {
                return size;
            }

        }
    }

    public class ClassifiedFeedTotalBathroomAttributeResolver : ValueResolver<Properties, int>
    {
        protected override int ResolveCore(Properties m)
        {
            int size = 0;
            if (!String.IsNullOrEmpty(m.Property.Bathroomnumber))
            {
                if (m.Property.Bathroomnumber.Contains('.'))
                {

                    string[] strArr = m.Property.Bathroomnumber.Split('.');
                    for (int i = 0; i < strArr.Length; i++)
                    {
                        if (i == 1 && strArr[i] == "5")
                            size = Convert.ToInt16(size + 1);
                        else
                            size = size + Convert.ToInt16(strArr[i]);
                    }
                    
                    //size = Convert.ToInt16(strArr[1]);
                    //if (size == 5)
                    //    return 1;
                    //else
                    //    return Convert.ToInt16(strArr[1]);
                    return Convert.ToInt16(size);
                }
                else
                {
                    return size;
                }
            }
            else
            {
                return size;
            }

        }
    }

    public class ClassifiedFeedListingCategoryAttributeResolver : ValueResolver<Properties, string>
    {
        protected override string ResolveCore(Properties m)
        {

            if (!String.IsNullOrEmpty(m.Property.Terms))
            {
                if (m.Property.Terms == "rent")
                {
                    return "Rent";
                }
                else if (m.Property.Terms == "sell")
                {
                    return "Purchase";
                }
                else
                {
                    return m.Property.Terms;
                }
            }
            else
            {
                return String.Empty;
            }

        }
    }

    public class ClassifiedFeedImageAttributeResolver : ValueResolver<Properties, Photos>
    {
        protected override Photos ResolveCore(Properties m)
        {
            Photos tempPhotos = new Photos() { Photo = new List<Photo>() };

            if (!String.IsNullOrEmpty(m.Property.Images.Image1))
            {
                tempPhotos.Photo.Add(new Photo() { MediaURL = m.Property.Images.Image1 });
            }
            if (!String.IsNullOrEmpty(m.Property.Images.Image2))
            {
                tempPhotos.Photo.Add(new Photo() { MediaURL = m.Property.Images.Image2 });
            }
            if (!String.IsNullOrEmpty(m.Property.Images.Image3))
            {
                tempPhotos.Photo.Add(new Photo() { MediaURL = m.Property.Images.Image3 });
            }
            if (!String.IsNullOrEmpty(m.Property.Images.Image4))
            {
                tempPhotos.Photo.Add(new Photo() { MediaURL = m.Property.Images.Image4 });
            }
            if (!String.IsNullOrEmpty(m.Property.Images.Image5))
            {
                tempPhotos.Photo.Add(new Photo() { MediaURL = m.Property.Images.Image5 });
            }
            if (!String.IsNullOrEmpty(m.Property.Images.Image6))
            {
                tempPhotos.Photo.Add(new Photo() { MediaURL = m.Property.Images.Image6 });
            }
            if (!String.IsNullOrEmpty(m.Property.Images.Image7))
            {
                tempPhotos.Photo.Add(new Photo() { MediaURL = m.Property.Images.Image7 });
            }
            if (!String.IsNullOrEmpty(m.Property.Images.Image8))
            {
                tempPhotos.Photo.Add(new Photo() { MediaURL = m.Property.Images.Image8 });
            }
            if (!String.IsNullOrEmpty(m.Property.Images.Image9))
            {
                tempPhotos.Photo.Add(new Photo() { MediaURL = m.Property.Images.Image9 });
            }
            if (!String.IsNullOrEmpty(m.Property.Images.Image10))
            {
                tempPhotos.Photo.Add(new Photo() { MediaURL = m.Property.Images.Image10 });
            }

            if (tempPhotos.Photo.Count > 0)
                return tempPhotos;
            else
                return null;
        }
    }

    public class ClassifiedFeedMinpriceAttributeResolver : ValueResolver<Properties, ListPrice>
    {
        protected override ListPrice ResolveCore(Properties m)
        {
            ListPrice lst = new ListPrice();
            double size = 0;
            if (!String.IsNullOrEmpty(m.Property.Minprice))
            {

                if (m.Property.Minprice.Contains('-'))
                {

                    string[] strArr = m.Property.Minprice.Split('-');
                    size = Convert.ToDouble(strArr[0]);
                    lst.Text = size;
                    return lst;
                }
                else
                {
                    lst.Text = Convert.ToDouble(m.Property.Minprice);
                    return lst;
                }
            }
            else
            {
                return lst;
            }

        }
    }

    public class ClassifiedFeedPropertyTypeAttributeResolver : ValueResolver<Properties, PropertyType>
    {
        protected override PropertyType ResolveCore(Properties m)
        {
            PropertyType objPropertyType = new PropertyType();

            if (!String.IsNullOrEmpty(m.Property.PropertyType))
            {
                objPropertyType.Text = m.Property.PropertyType;
                objPropertyType.OtherDescription = m.Property.PropertyType;
                return objPropertyType;
            }
            else
            {
                return null;
            }

        }
    }

    #endregion
}
