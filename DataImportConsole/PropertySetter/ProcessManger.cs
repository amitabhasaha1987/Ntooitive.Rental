using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Configuration;
using Repositories.Interfaces.Community;
using Repositories.Interfaces.Map;
using Repositories.Models.ListHub;
using Repositories.Models.Map;
using Repositories.Models.NewHome;

namespace DataImportConsole.PropertySetter
{
    public class AddressWithLatLong
    {
        public string Address { get; set; }
        public LatitudeLongitude Coordinates { get; set; }

    }
    public static class ProcessManger
    {
        public static void SetLatLong<T>(List<T> _entities)
        {
            Console.WriteLine("Location Finder Started.. ");
            var i = 0;
            var latLongService = NinjectConfig.Get<IFetchLatLong>();

            var addresslist = new HashSet<AddressWithLatLong>();

            if (_entities.GetType() == typeof(List<ListHubListing>))
            {
                var entities = _entities as List<ListHubListing>;
                var entitiesWithoutlatlong =
                entities.Where(m => m.Location == null || m.Location.Latitude == 0 || m.Location.Longitude == 0);

                foreach (var entity in entitiesWithoutlatlong)
                {
                    Console.WriteLine((i++) + " , ");
                    if (entity.Address != null && !string.IsNullOrEmpty(entity.Address.FullStreetAddress))
                    {
                        var fullstrtAddress = string.Empty;
                        if (entity.Address.FullStreetAddress.Contains(entity.Address.FullAddress.Replace(",", "")))
                        {
                            fullstrtAddress = entity.Address.FullStreetAddress;
                        }
                        else
                        {
                            fullstrtAddress = entity.Address.FullStreetAddress + "," + entity.Address.FullAddress;
                        }

                        var address = addresslist.FirstOrDefault(m => m.Address == fullstrtAddress);
                        if (address != null)
                        {
                            entity.Location.Latitude = Convert.ToDouble(address.Coordinates.lat,
                                CultureInfo.InvariantCulture);
                            entity.Location.Longitude = Convert.ToDouble(address.Coordinates.lng,
                                CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            var addressfromDb = latLongService.GetLatlong(fullstrtAddress);
                            if (addressfromDb != null)
                            {
                                #region "Added for ClassifiedFeed"

                                if (entity.Location != null)
                                {
                                    entity.Location.Latitude = Convert.ToDouble(addressfromDb[0], CultureInfo.InvariantCulture);
                                    entity.Location.Longitude = Convert.ToDouble(addressfromDb[1], CultureInfo.InvariantCulture);

                                }
                                else
                                {
                                    entity.Location = new Location()
                                    {
                                        Latitude = Convert.ToDouble(addressfromDb[0], CultureInfo.InvariantCulture)
                                        ,
                                        Longitude = Convert.ToDouble(addressfromDb[1], CultureInfo.InvariantCulture)
                                    };
                                }
                                #endregion
                                //entity.Location.Latitude = Convert.ToDouble(addressfromDb[0], CultureInfo.InvariantCulture);
                                //entity.Location.Longitude = Convert.ToDouble(addressfromDb[1], CultureInfo.InvariantCulture);
                                addresslist.Add(new AddressWithLatLong
                                {
                                    Address = fullstrtAddress,
                                    Coordinates = new LatitudeLongitude
                                    {
                                        lat = Convert.ToDouble(addressfromDb[0], CultureInfo.InvariantCulture),
                                        lng = Convert.ToDouble(addressfromDb[1], CultureInfo.InvariantCulture)
                                    }
                                });
                            }
                            else
                            {
                                var fetchedCoordinates =
                                    latLongService.GetLatitudeAndLongitude(fullstrtAddress);
                                if (fetchedCoordinates != null)
                                {
                                    latLongService.InsertLatlong(fullstrtAddress, fetchedCoordinates);

                                    //entity.Location.Latitude = Convert.ToDouble(fetchedCoordinates.Coordinate[0], CultureInfo.InvariantCulture);
                                    //entity.Location.Longitude = Convert.ToDouble(fetchedCoordinates.Coordinate[1], CultureInfo.InvariantCulture);
                                    #region "Added for ClassifiedFeed"

                                    if (entity.Location != null)
                                    {
                                        entity.Location.Latitude = Convert.ToDouble(fetchedCoordinates.Coordinate[0], CultureInfo.InvariantCulture);
                                        entity.Location.Longitude = Convert.ToDouble(fetchedCoordinates.Coordinate[1], CultureInfo.InvariantCulture);

                                    }
                                    else
                                    {
                                        entity.Location = new Location()
                                        {
                                            Latitude = Convert.ToDouble(fetchedCoordinates.Coordinate[0], CultureInfo.InvariantCulture),
                                            Longitude = Convert.ToDouble(fetchedCoordinates.Coordinate[0], CultureInfo.InvariantCulture)
                                        };
                                    }
                                    #endregion

                                    addresslist.Add(new AddressWithLatLong
                                    {
                                        Address = fullstrtAddress,
                                        Coordinates = new LatitudeLongitude
                                        {
                                            lat = Convert.ToDouble(fetchedCoordinates.Coordinate[0], CultureInfo.InvariantCulture),
                                            lng = Convert.ToDouble(fetchedCoordinates.Coordinate[1], CultureInfo.InvariantCulture)
                                        }
                                    });
                                }
                            }
                        }
                    }
                    else if (entity.Address != null && !string.IsNullOrEmpty(entity.Address.FullAddress))
                    {
                        var fullstrtAddress = entity.Address.FullAddress;
                        var address = addresslist.FirstOrDefault(m => m.Address == fullstrtAddress);

                        if (address != null)
                        {
                            #region "Added for ClassifiedFeed"

                            if (entity.Location != null)
                            {
                                entity.Location.Latitude = Convert.ToDouble(address.Coordinates.lat,
                                CultureInfo.InvariantCulture);
                                entity.Location.Longitude = Convert.ToDouble(address.Coordinates.lng,
                                CultureInfo.InvariantCulture);

                            }
                            else
                            {
                                entity.Location = new Location()
                                {
                                    Latitude = Convert.ToDouble(address.Coordinates.lat,
                                        CultureInfo.InvariantCulture),

                                    Longitude = Convert.ToDouble(address.Coordinates.lng,
                                        CultureInfo.InvariantCulture)
                                };
                            }

                            #endregion
                            //entity.Location.Latitude = Convert.ToDouble(address.Coordinates.lat,
                            //    CultureInfo.InvariantCulture);
                            //entity.Location.Longitude = Convert.ToDouble(address.Coordinates.lng,
                            //    CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            var addressfromDb = latLongService.GetLatlong(fullstrtAddress);
                            if (addressfromDb != null)
                            {
                                #region "Added for ClassifiedFeed"

                                if (entity.Location != null)
                                {
                                    entity.Location.Latitude = Convert.ToDouble(addressfromDb[0], CultureInfo.InvariantCulture);
                                    entity.Location.Longitude = Convert.ToDouble(addressfromDb[1], CultureInfo.InvariantCulture);

                                }
                                else
                                {
                                    entity.Location = new Location()
                                    {
                                        Latitude = Convert.ToDouble(addressfromDb[0], CultureInfo.InvariantCulture),
                                        Longitude = Convert.ToDouble(addressfromDb[1], CultureInfo.InvariantCulture)
                                    };
                                }

                                #endregion
                                //entity.Location.Latitude = Convert.ToDouble(addressfromDb[0], CultureInfo.InvariantCulture);
                                //entity.Location.Longitude = Convert.ToDouble(addressfromDb[1], CultureInfo.InvariantCulture);
                                addresslist.Add(new AddressWithLatLong
                                {
                                    Address = fullstrtAddress,
                                    Coordinates = new LatitudeLongitude
                                    {
                                        lat = Convert.ToDouble(addressfromDb[0], CultureInfo.InvariantCulture),
                                        lng = Convert.ToDouble(addressfromDb[1], CultureInfo.InvariantCulture)
                                    }
                                });
                            }
                            else
                            {

                                var fetchedCoordinates = latLongService.GetLatitudeAndLongitude(fullstrtAddress);
                                if (fetchedCoordinates != null)
                                {
                                    latLongService.InsertLatlong(entity.Address.FullAddress, fetchedCoordinates);
                                    #region "Added for ClassifiedFeed"

                                    if (entity.Location != null)
                                    {
                                        entity.Location.Latitude = Convert.ToDouble(fetchedCoordinates.Coordinate[0],
                                                CultureInfo.InvariantCulture);
                                        entity.Location.Longitude = Convert.ToDouble(fetchedCoordinates.Coordinate[1],
                                                CultureInfo.InvariantCulture);

                                    }
                                    else
                                    {
                                        entity.Location = new Location()
                                        {
                                            Latitude = Convert.ToDouble(fetchedCoordinates.Coordinate[0],
                                                CultureInfo.InvariantCulture),

                                            Longitude = Convert.ToDouble(fetchedCoordinates.Coordinate[1],
                                                CultureInfo.InvariantCulture)
                                        };
                                    }

                                    #endregion
                                    //entity.Location.Latitude = Convert.ToDouble(fetchedCoordinates.Coordinate[0],
                                    //    CultureInfo.InvariantCulture);
                                    //entity.Location.Longitude = Convert.ToDouble(fetchedCoordinates.Coordinate[1],
                                    //    CultureInfo.InvariantCulture);
                                    addresslist.Add(new AddressWithLatLong
                                    {
                                        Address = fullstrtAddress,
                                        Coordinates = new LatitudeLongitude
                                        {
                                            lat = Convert.ToDouble(fetchedCoordinates.Coordinate[0], CultureInfo.InvariantCulture),
                                            lng = Convert.ToDouble(fetchedCoordinates.Coordinate[1], CultureInfo.InvariantCulture)
                                        }
                                    });
                                }
                            }
                        }
                    }
                }
            }
            else if ((_entities.GetType() == typeof(List<Repositories.Models.NewHome.Community>)))
            {

                var entities = _entities as List<Repositories.Models.NewHome.Community>;
                var entitiesWithoutlatlong = entities.Where(m => m.Latitude == 0 || m.Longitude == 0);

                foreach (var entity in entitiesWithoutlatlong)
                {
                    Console.WriteLine((i++) + " , ");
                    if (!string.IsNullOrEmpty(entity.Address))
                    {
                        var fullstrtAddress = string.Empty;
                        fullstrtAddress = entity.Address + entity.City + entity.Zip;  //this logic is changed according to new xml

                        var address = addresslist.FirstOrDefault(m => m.Address == fullstrtAddress);
                        if (address != null)
                        {
                            entity.Latitude = Convert.ToDouble(address.Coordinates.lat,
                                CultureInfo.InvariantCulture);
                            entity.Longitude = Convert.ToDouble(address.Coordinates.lng,
                                CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            var addressfromDb = latLongService.GetLatlong(fullstrtAddress);
                            if (addressfromDb != null)
                            {
                                entity.Latitude = Convert.ToDouble(addressfromDb[0], CultureInfo.InvariantCulture);
                                entity.Longitude = Convert.ToDouble(addressfromDb[1], CultureInfo.InvariantCulture);
                                addresslist.Add(new AddressWithLatLong
                                {
                                    Address = fullstrtAddress,
                                    Coordinates = new LatitudeLongitude
                                    {
                                        lat = Convert.ToDouble(addressfromDb[0], CultureInfo.InvariantCulture),
                                        lng = Convert.ToDouble(addressfromDb[1], CultureInfo.InvariantCulture)
                                    }
                                });
                            }
                            else
                            {
                                var fetchedCoordinates =
                                    latLongService.GetLatitudeAndLongitude(fullstrtAddress);
                                if (fetchedCoordinates != null)
                                {
                                    latLongService.InsertLatlong(fullstrtAddress, fetchedCoordinates);

                                    entity.Latitude = Convert.ToDouble(fetchedCoordinates.Coordinate[0], CultureInfo.InvariantCulture);
                                    entity.Longitude = Convert.ToDouble(fetchedCoordinates.Coordinate[1], CultureInfo.InvariantCulture);


                                    addresslist.Add(new AddressWithLatLong
                                    {
                                        Address = fullstrtAddress,
                                        Coordinates = new LatitudeLongitude
                                        {
                                            lat = Convert.ToDouble(fetchedCoordinates.Coordinate[0], CultureInfo.InvariantCulture),
                                            lng = Convert.ToDouble(fetchedCoordinates.Coordinate[1], CultureInfo.InvariantCulture)
                                        }
                                    });
                                }
                            }
                        }
                    }
                }
            }



            Console.WriteLine("Location Finder Ended.. ");
        }

        public static void SetCommunities<T>(List<T> _entities)
        {
            Console.WriteLine("Community Setter  Started.. ");
            int i = 0;
            var communityNames = NinjectConfig.Get<ICommunityProvider>();
            try
            {
                if (_entities.GetType() == typeof(List<ListHubListing>))
                {
                    var entities = _entities as List<ListHubListing>;
                    foreach (var listHubListing in entities)
                    {
                        var comumnityName = communityNames.GetCommunityName(listHubListing.Address.PostalCode);
                        if (comumnityName.Any())
                        {
                            listHubListing.CommunityName = comumnityName;
                        }
                        i = i + 1;
                        Console.Write(i);
                        Console.WriteLine(Environment.NewLine);
                    }
                }
                else if ((_entities.GetType() == typeof(List<Repositories.Models.NewHome.Community>)))
                {
                    /*
                    var entities = _entities as List<NewHomeListing>;
                    foreach (var listHubListing in entities)
                    {
                        var comumnityName = communityNames.GetCommunityName(listHubListing.Zip);
                        if (comumnityName.Any())
                        {
                            listHubListing.CommunityName = comumnityName;
                        }
                        i = i + 1;
                        Console.Write(i);
                        Console.WriteLine(Environment.NewLine);
                    }*/
                }

                Console.WriteLine("Community Setter  Ended.. ");
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
