using System;
using AutoMapper;
using Configuration.Resolver;
using Repositories.Models.Admin.User;
using Repositories.Models.ListHub;
using Repositories.Models.ViewModel;
using Repositories.Models.Common;
using System.Linq;
using Repositories.Models.Classified;
using Repositories.Models.NewHome;

namespace Configuration
{
    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {

            Mapper.CreateMap<Participant, User>()
                        .ForMember(dest => dest.Roles, opt => opt.ResolveUsing<RoleResolver>());
            Mapper.CreateMap<User, RegistartionViewModel>();
            Mapper.CreateMap<ListHubListing, ManagePropertyViewModel>()
                .ForMember(dest => dest.UniqueId, opt => opt.MapFrom(m => m.MlsNumber))
                .ForMember(dest => dest.AgentDescription,
                    opt => opt.MapFrom(m => m.ExtProperties == null ? "" : m.ExtProperties.AgentDescription))
                .ForMember(dest => dest.IsFeatured,
                    opt => opt.MapFrom(m => m.ExtProperties != null && m.ExtProperties.IsFeatured))
                .ForMember(dest => dest.IsPrinted,
                    opt => opt.MapFrom(m => m.ExtProperties != null && m.ExtProperties.IsPrinted));


            Mapper.CreateMap<ListHubListing, Repositories.Models.ListHub.Rental>()
                .ForMember(dest => dest.objId, opt => opt.Ignore())
                .ForMember(dest => dest.MlsNumber, opt => opt.MapFrom(m => m.MlsNumber))
                .ForMember(dest => dest.GeoLocation, opt => opt.MapFrom(m => m.Location.GeoPoint == null ? null : new GeoLocation { Coordinates = m.Location.GeoPoint.Coordinates, Type = m.Location.GeoPoint.Type }))
                .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(m => m.PropertyType == null ? string.Empty : m.PropertyType.Text))
                .ForMember(dest => dest.PropertySubType, opt => opt.MapFrom(m => m.PropertySubType == null ? null : m.PropertySubType))
                .ForMember(dest => dest.FullStreetAddress, opt => opt.MapFrom(m => m.Address.FullStreetAddress))
                .ForMember(dest => dest.FullAddress, opt => opt.MapFrom(m => m.Address.FullAddress))
                .ForMember(dest => dest.VirtualTour, opt => opt.MapFrom(m => m.VirtualTours.VirtualTour.MediaURL))
                .ForMember(dest => dest.NoOfBathRooms, opt => opt.MapFrom(m => m.Bathrooms))
                .ForMember(dest => dest.NoOfBedRooms, opt => opt.MapFrom(m => m.Bedrooms))
                .ForMember(dest => dest.FullBathrooms, opt => opt.MapFrom(m => m.FullBathrooms))
                .ForMember(dest => dest.ThreeQuarterBathrooms, opt => opt.MapFrom(m => m.ThreeQuarterBathrooms))
                .ForMember(dest => dest.NoOfHalfBathRooms, opt => opt.MapFrom(m => m.HalfBathrooms))
                .ForMember(dest => dest.OneQuarterBathrooms, opt => opt.MapFrom(m => m.OneQuarterBathrooms))
                .ForMember(dest => dest.PartialBathrooms, opt => opt.MapFrom(m => m.PartialBathrooms))
                .ForMember(dest => dest.LotSize, opt => opt.MapFrom(m => m.LotSize))
                .ForMember(dest => dest.LivingArea, opt => opt.MapFrom(m => m.LivingArea))
                .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(m => m.ProviderName))
                .ForMember(dest => dest.MarketingInformation, opt => opt.MapFrom(m => m.MarketingInformation))
                .ForMember(dest => dest.IsFeatured, opt => opt.MapFrom(m => m.ExtProperties == null ? false : m.ExtProperties.IsFeatured))
                .ForMember(dest => dest.IsSpotlight, opt => opt.MapFrom(m => m.ExtProperties == null ? false : m.ExtProperties.IsSpotlight))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(m => m.ListPrice.Text))
                .ForMember(dest => dest.CommunityName, opt => opt.MapFrom(m => m.CommunityName))
                .ForMember(dest => dest.YearBuilt, opt => opt.MapFrom(m => m.YearBuilt))
                .ForMember(dest => dest.EntryDate, opt => opt.MapFrom(m => DateTime.Now))
                .ForMember(dest => dest.BrokerageLogoUrl, opt => opt.MapFrom(m => m.Brokerage.LogoURL))
                .ForMember(dest => dest.ImageCount, opt => opt.MapFrom(m => m.Photos.Photo.Count))
                 .ForMember(dest => dest.ClassifiedExpireDate, opt => opt.MapFrom(m => m.ExpireDate))
                 .ForMember(dest => dest.AgentName, opt => opt.ResolveUsing<AgentNameResolver>())
                .ForMember(dest => dest.HasAddress, opt => opt.ResolveUsing<HasListHubAddressResolver>())
                .ForMember(dest => dest.IsClassified, opt => opt.ResolveUsing<IsClassifiedResolver>())
                .ForMember(dest => dest.DefaultPhoto, opt => opt.ResolveUsing<PhotoResolver>())
                .ForMember(dest => dest.Attributes, opt => opt.ResolveUsing<AttributeResolver>());



            Mapper.CreateMap<ListHubListing, Repositories.Models.ListHub.Purchase>()
                .ForMember(dest => dest.objId, opt => opt.Ignore())
                .ForMember(dest => dest.MlsNumber, opt => opt.MapFrom(m => m.MlsNumber))
                .ForMember(dest => dest.GeoLocation, opt => opt.MapFrom(m => m.Location.GeoPoint == null ? null : new GeoLocation { Coordinates = m.Location.GeoPoint.Coordinates, Type = m.Location.GeoPoint.Type }))
                .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(m => m.PropertyType == null ? string.Empty : m.PropertyType.Text))
                .ForMember(dest => dest.PropertySubType, opt => opt.MapFrom(m => m.PropertySubType == null ? null : m.PropertySubType))
                .ForMember(dest => dest.FullStreetAddress, opt => opt.MapFrom(m => m.Address.FullStreetAddress))
                .ForMember(dest => dest.FullAddress, opt => opt.MapFrom(m => m.Address.FullAddress))
                .ForMember(dest => dest.VirtualTour, opt => opt.MapFrom(m => m.VirtualTours.VirtualTour.MediaURL))
                .ForMember(dest => dest.NoOfBathRooms, opt => opt.MapFrom(m => m.Bathrooms))
                .ForMember(dest => dest.NoOfBedRooms, opt => opt.MapFrom(m => m.Bedrooms))
                .ForMember(dest => dest.FullBathrooms, opt => opt.MapFrom(m => m.FullBathrooms))
                .ForMember(dest => dest.ThreeQuarterBathrooms, opt => opt.MapFrom(m => m.ThreeQuarterBathrooms))
                .ForMember(dest => dest.NoOfHalfBathRooms, opt => opt.MapFrom(m => m.HalfBathrooms))
                .ForMember(dest => dest.OneQuarterBathrooms, opt => opt.MapFrom(m => m.OneQuarterBathrooms))
                .ForMember(dest => dest.PartialBathrooms, opt => opt.MapFrom(m => m.PartialBathrooms))
                .ForMember(dest => dest.LotSize, opt => opt.MapFrom(m => m.LotSize))
                .ForMember(dest => dest.LivingArea, opt => opt.MapFrom(m => m.LivingArea))
                .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(m => m.ProviderName))
                .ForMember(dest => dest.MarketingInformation, opt => opt.MapFrom(m => m.MarketingInformation))
                .ForMember(dest => dest.IsFeatured, opt => opt.MapFrom(m => m.ExtProperties == null ? false : m.ExtProperties.IsFeatured))
                .ForMember(dest => dest.IsSpotlight, opt => opt.MapFrom(m => m.ExtProperties == null ? false : m.ExtProperties.IsSpotlight))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(m => m.ListPrice.Text))
                .ForMember(dest => dest.YearBuilt, opt => opt.MapFrom(m => m.YearBuilt))
                .ForMember(dest => dest.EntryDate, opt => opt.MapFrom(m => DateTime.Now))
                .ForMember(dest => dest.ImageCount, opt => opt.MapFrom(m => m.Photos.Photo.Count))
                .ForMember(dest => dest.BrokerageLogoUrl, opt => opt.MapFrom(m => m.Brokerage.LogoURL))
                .ForMember(dest => dest.CommunityName, opt => opt.MapFrom(m => m.CommunityName))
                .ForMember(dest => dest.ClassifiedExpireDate, opt => opt.MapFrom(m => m.ExpireDate))
                 .ForMember(dest => dest.AgentName, opt => opt.ResolveUsing<AgentNameResolver>())
                .ForMember(dest => dest.DefaultPhoto, opt => opt.ResolveUsing<PhotoResolver>())
                .ForMember(dest => dest.IsClassified, opt => opt.ResolveUsing<IsClassifiedResolver>())
                .ForMember(dest => dest.HasAddress, opt => opt.ResolveUsing<HasListHubAddressResolver>())
                .ForMember(dest => dest.Attributes, opt => opt.ResolveUsing<AttributeResolver>());



            Mapper.CreateMap<Repositories.Models.NewHome.Plan, Repositories.Models.ListHub.NewHome>()
                .ForMember(dest => dest.objId, opt => opt.Ignore())
                .ForMember(dest => dest.Plans, opt => opt.Ignore())

                //.ForMember(dest => dest.CommunityName, opt => opt.Ignore())
                .ForMember(dest => dest.MlsNumber, opt => opt.MapFrom(m => m.PlanId))
                //.ForMember(dest => dest.BrokerageName, opt => opt.MapFrom(m => m.BrandName))
                .ForMember(dest => dest.FullStreetAddress, opt => opt.MapFrom(m => m.Communityaddress))
                .ForMember(dest => dest.FullAddress, opt => opt.MapFrom(m => m.FullAddress))
                .ForMember(dest => dest.Price, opt => opt.ResolveUsing<NewHomePriceResolver>())
                //.ForMember(dest => dest.MlsNumber, opt => opt.MapFrom(m => m.PlanId))
                //.ForMember(dest => dest.BrokerageLogoUrl, opt => opt.MapFrom(m => m.))  //plan>Homes>Home>broker_logo_url
                //.ForMember(dest => dest.ListingParticipantsName, opt => opt.MapFrom(m => m.ReportingName)) //builder>name_reporting
                .ForMember(dest => dest.NoOfBathRooms, opt => opt.MapFrom(m => m.Baths))
                .ForMember(dest => dest.NoOfBedRooms, opt => opt.MapFrom(m => m.Bedrooms))
                .ForMember(dest => dest.NoOfHalfBathRooms, opt => opt.MapFrom(m => m.Half_baths))
                .ForMember(dest => dest.LivingArea, opt => opt.MapFrom(m => m.Sqft_low))
                
                .ForMember(dest => dest.IsSpotlight, opt => opt.MapFrom(m => m.ExtProperties!=null? false : m.ExtProperties.IsSpotlight))
                .ForMember(dest => dest.IsFeatured, opt => opt.MapFrom(m => m.ExtProperties != null ? false : m.ExtProperties.IsFeatured))
                .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(m => m.Style))

                .ForMember(dest => dest.NoOfBathRooms_low, opt => opt.MapFrom(m => m.Baths_low))
                .ForMember(dest => dest.NoOfBedRooms_low, opt => opt.MapFrom(m => m.Bedrooms_low))
                .ForMember(dest => dest.NoOfHalfBathRooms_low, opt => opt.MapFrom(m => m.Half_baths_low))
                .ForMember(dest => dest.Communityprice_high, opt => opt.MapFrom(m => m.Communityprice_high))
                .ForMember(dest => dest.Communityprice_low, opt => opt.MapFrom(m => m.Communityprice_low))
                .ForMember(dest => dest.Communitysqft_high, opt => opt.MapFrom(m => m.Communitysqft_high))
                .ForMember(dest => dest.Communitysqft_low, opt => opt.MapFrom(m => m.Communitysqft_low))

                .ForMember(dest => dest.DefaultPhoto, opt => opt.ResolveUsing<NowHomeImages>())
                .ForMember(dest => dest.GeoLocation, opt => opt.MapFrom(m => m.GeoPoint == null ? null : new GeoLocation { Coordinates = m.GeoPoint.Coordinates, Type = m.GeoPoint.Type }))
                .ForMember(dest => dest.State, opt => opt.MapFrom(m => m.Communitystate))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(m => m.Communityzip))
                .ForMember(dest => dest.City, opt => opt.MapFrom(m => m.Communitycity))
                // .ForMember(dest => dest.CommunityName, opt => opt.MapFrom(m => m.CommunityName))
                .ForMember(dest => dest.EntryDate, opt => opt.MapFrom(m => DateTime.Now))
                .ForMember(dest => dest.ImageCount, opt => opt.MapFrom(m => m.Images.Image.Select(x => x.Reference).Count()))

                //.ForMember(dest => dest.Attributes, opt => opt.ResolveUsing<NewHomeAttributeResolver>())
                .ForMember(dest => dest.Attributes, opt => opt.ResolveUsing<NewHomeAttributeResolver>());

            #region "Added for ClassifiedFeed"
            Mapper.CreateMap<Properties, ListHubListing>()
                 .ForMember(dest => dest.Id, opt => opt.Ignore())
              .ForMember(dest => dest.MlsNumber, opt => opt.MapFrom(m => m.Property.AdId))
                //.ForMember(dest => dest.Address,
                //    opt => opt.MapFrom(m => m.Property.Address1 == null ? "" : m.Property.Address1))

              .ForMember(dest => dest.Bedrooms,
              opt => opt.MapFrom(m => Convert.ToInt16(string.IsNullOrEmpty(m.Property.Bedroomnumber) ? "0" : m.Property.Bedroomnumber)))
               .ForMember(dest => dest.Bathrooms,
               opt => opt.ResolveUsing<ClassifiedFeedTotalBathroomAttributeResolver>())
               .ForMember(dest => dest.FullBathrooms,
               opt => opt.ResolveUsing<ClassifiedFeedBathroomAttributeResolver>())
               .ForMember(dest => dest.HalfBathrooms,
                opt => opt.ResolveUsing<ClassifiedFeedHalfBathroomAttributeResolver>())
              .ForMember(dest => dest.CommunityName,
                  opt => opt.MapFrom(m => m.Property.Community == null ? new string[0] { } : new string[1] { m.Property.Community }))
              .ForMember(dest => dest.ListingDate,
                  opt => opt.MapFrom(m => m.Property.PostDate == null ? "" : m.Property.PostDate))
              .ForMember(dest => dest.PropertyType,
                  opt => opt.MapFrom(m => m.Property.PropertyType == null ? "" : m.Property.PropertyType))

              .ForMember(dest => dest.UpdatedDate,
                  opt => opt.MapFrom(m => m.Property.AvailableDate == null ? "" : m.Property.AvailableDate))

              .ForMember(dest => dest.Address, input => input.MapFrom(i => new Address
              {
                  City = i.Property.City,

                  StateOrProvince = i.Property.State,
                  Country = i.Property.Country,
                  PostalCode = i.Property.Zip,
                  FullStreetAddress = i.Property.Address1
              }))

              .ForMember(dest => dest.AdId,
                    opt => opt.MapFrom(m => m.Property.AdId == null ? "" : m.Property.AdId))

              .ForMember(dest => dest.LeadRoutingEmail,
                  opt => opt.MapFrom(m => m.Property.ContactEmail == null ? "" : m.Property.ContactEmail))

                //.ForMember(dest => dest.Address.Country,
                //    opt => opt.MapFrom(m => m.Property.Country == null ? "" : m.Property.Country))
                  .ForMember(dest => dest.PropertyType, opt => opt.ResolveUsing<ClassifiedFeedPropertyTypeAttributeResolver>())
                //.ForMember(dest => dest.PropertyType.OtherDescription,
                //    opt => opt.MapFrom(m => m.Property.Description == null ? "" : m.Property.Description))
              .ForMember(dest => dest.ExpireDate,
                  opt => opt.MapFrom(m => m.Property.ExpireDate))
              .ForMember(dest => dest.HiddenAdress,
                  opt => opt.MapFrom(m => m.Property.HiddenAdress == null ? "" : m.Property.HiddenAdress))
              .ForMember(dest => dest.Photos,
                  opt => opt.ResolveUsing<ClassifiedFeedImageAttributeResolver>())
              .ForMember(dest => dest.ListPrice,
                  opt => opt.ResolveUsing<ClassifiedFeedMinpriceAttributeResolver>())
                //.ForMember(dest => dest.ListPrice.Text,
                //    opt => opt.MapFrom(m => m.Property.Minprice == null ? "" : m.Property.Minprice))
               .ForMember(dest => dest.LivingArea,
                  opt => opt.MapFrom(m => string.IsNullOrEmpty(m.Property.SqFt) ? 0 : Convert.ToDouble(m.Property.SqFt)))

                //.ForMember(dest => dest.Address.StateOrProvince,
                //    opt => opt.MapFrom(m => m.Property.State == null ? "" : m.Property.State))
                .ForMember(dest => dest.ExtProperties, input =>
                    input.MapFrom(i => new ManagePropertyViewModel
                    {
                        IsFeatured = (String.IsNullOrEmpty(i.Property.UpsellFeaturedAd) ? false : i.Property.UpsellFeaturedAd == "0" ? true : false),
                        IsSpotlight = (String.IsNullOrEmpty(i.Property.UpsellSpotlightAd) ? false : i.Property.UpsellSpotlightAd == "0" ? true : false)
                    }))


                //.ForMember(dest => dest.Address.PostalCode,
                //    opt => opt.MapFrom(m => m.Property.Zip == null ? "" : m.Property.Zip))
             .ForMember(dest => dest.ListingCategory,
                opt => opt.ResolveUsing<ClassifiedFeedListingCategoryAttributeResolver>())
                .ForMember(dest => dest.HasAddress, opt => opt.ResolveUsing<HasAddressResolver>())
                .ForMember(dest => dest.IsClassified,opt => opt.MapFrom(m => true))
              .ForMember(dest => dest.ListingDescription, input => input.MapFrom(i => i.Property.Description));
            //.ForMember(dest => dest.Disclaimer.Text,
            //    opt => opt.MapFrom(m => m.Property.Terms == null ? "" : m.Property.Terms));
            #endregion

            
        }
    }
}