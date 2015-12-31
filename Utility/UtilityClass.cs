using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Utility
{
    public static class UtilityClass
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static string GetUniqueKey()
        {
            const int maxSize = 8;
            var a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var chars = a.ToCharArray();
            var data = new byte[1];
            var crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            const int size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            var result = new StringBuilder(size);
            foreach (byte b in data)
            { result.Append(chars[b % (chars.Length - 1)]); }
            return result.ToString();
        }
        public static string GetsortColumn(string ColumnName)
        {
            string colName = null;
            if (ColumnName == "Bedrooms")
            {
                colName = "NoOfBedRooms";
            }
            else if (ColumnName == "Bathrooms")
            {
                colName = "NoOfBathRooms";
            }
            else if (ColumnName == "Price")
            {
                colName = "Price";
            }
            return colName;
        }
        public static string CreateQueryString(string minPrice, string maxPrice, string noOfBeds, string noOfBathroom, string size, string lotSize, string homeAge, List<string> lstPropertyType)
        {
            try
            {
                string queryString = null;
                if (!string.IsNullOrEmpty(minPrice) && !string.IsNullOrEmpty(maxPrice))
                {
                    queryString = ",'ListPrice.Text': {'$gte': " + minPrice + ",'$lte': " + maxPrice + "}";
                }
                else if (!string.IsNullOrEmpty(minPrice) && string.IsNullOrEmpty(maxPrice))
                {
                    queryString = ",'ListPrice.Text': {'$gte': " + minPrice + "}";
                }
                else if (string.IsNullOrEmpty(minPrice) && !string.IsNullOrEmpty(maxPrice))
                {
                    queryString = ",'ListPrice.Text': {'$lte': " + maxPrice + "}";
                }


                if (!string.IsNullOrEmpty(noOfBeds) && noOfBeds != "undefined")
                {
                    if (Convert.ToInt32(noOfBeds) >= 5 || Convert.ToInt32(noOfBeds) == 0)
                    {
                        queryString = (!string.IsNullOrEmpty(queryString)) ? queryString + "," + "'Bedrooms': {'$gte': " + noOfBeds + "}" : "'Bedrooms': {'$gte': " + noOfBeds + "}";
                    }
                    else
                    {
                        queryString = (!string.IsNullOrEmpty(queryString)) ? queryString + "," + "'Bedrooms': " + noOfBeds + "" : "'Bedrooms': " + noOfBeds + "";
                    }
                }


                if (!string.IsNullOrEmpty(noOfBathroom) && noOfBathroom != "undefined")
                {
                    if (Convert.ToDecimal(noOfBathroom) >= 5 || Convert.ToDecimal(noOfBathroom) == 0)
                    {
                        queryString = (!string.IsNullOrEmpty(queryString)) ? queryString + "," + "'Bathrooms': {'$gte': " + noOfBathroom + "}" : ",'Bathrooms': {'$gte': " + noOfBathroom + "}";
                    }
                    else
                    {
                        if (noOfBathroom.Contains('.'))
                        {
                            queryString = (!string.IsNullOrEmpty(queryString)) ? queryString + "," + "'Bathrooms': {'$gte': " + noOfBathroom.Split('.')[0] + "},'HalfBathrooms': {'$gte': " + (Convert.ToInt16(noOfBathroom.Split('.')[1]) > 0 ? 1 : 0) + "}" : ",'Bathrooms': {'$gte': " + noOfBathroom.Split('.')[0] + "},'HalfBathrooms': {'$gte': " + (Convert.ToInt16(noOfBathroom.Split('.')[1]) > 0 ? 1 : 0) + "}";
                        }
                        else
                        {
                            queryString = (!string.IsNullOrEmpty(queryString)) ? queryString + "," + "'Bathrooms': {'$gte': " + noOfBathroom + "}" : ",'Bathrooms': {'$gte': " + noOfBathroom + "}";
                        }
                    }
                }


                if (!string.IsNullOrEmpty(size) && size != "undefined")
                {
                    queryString = (!string.IsNullOrEmpty(queryString)) ? queryString + "," + "'LivingArea': {'$gte': " + size + "}" : "'LivingArea': {'$gte': " + size + "}";
                }


                if (!string.IsNullOrEmpty(lotSize) && lotSize != "undefined")
                {
                    queryString = (!string.IsNullOrEmpty(queryString)) ? queryString + "," + "'LotSize.Text': {'$gte': " + lotSize + "}" : "'LotSize.Text': {'$gte': " + lotSize + "}";
                }


                if (!string.IsNullOrEmpty(homeAge) && homeAge != "undefined")
                {
                    if (Convert.ToInt32(homeAge) == 51 || Convert.ToInt32(homeAge) == 0)
                    {
                        queryString = (!string.IsNullOrEmpty(queryString)) ? queryString + "," + "'YearBuilt': {'$gte': " + homeAge + "}" : "'YearBuilt': {'$gte': " + homeAge + "}";
                    }
                    else
                    {
                        queryString = (!string.IsNullOrEmpty(queryString)) ? queryString + "," + "'YearBuilt': {'$lte': " + homeAge + "}" : "'YearBuilt': {'$lte': " + homeAge + "}";
                    }
                }

                if (lstPropertyType.Count > 0)
                {
                    string propertyType = null;
                    foreach (string item in lstPropertyType)
                    {
                        propertyType = string.IsNullOrEmpty(item) ? null : (string.IsNullOrEmpty(propertyType) ? "{ 'PropertyType.Text' : '" + item + "'}" : propertyType + ",{ 'PropertyType.Text' : '" + item + "'}");
                    }
                    queryString = string.IsNullOrEmpty(propertyType) ? queryString : queryString + ",$or : [ " + propertyType + "]";
                }

                return queryString;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static string CreateQueryStringForNewListing(string minPrice, string maxPrice, string noOfBeds, string noOfBathroom, string size, string lotSize, string homeAge, List<string> lstPropertyType)
        {
            try
            {
                string queryString = null;
                if (!string.IsNullOrEmpty(minPrice) && !string.IsNullOrEmpty(maxPrice))
                {
                    queryString = ",'Subdivision.Plan.BasePrice.Text': {'$gte': " + minPrice + ",'$lte': " + maxPrice + "}";
                }
                else if (!string.IsNullOrEmpty(minPrice) && string.IsNullOrEmpty(maxPrice))
                {
                    queryString = ",'Subdivision.Plan.BasePrice.Text': {'$gte': " + minPrice + "}";
                }
                else if (string.IsNullOrEmpty(minPrice) && !string.IsNullOrEmpty(maxPrice))
                {
                    queryString = ",'Subdivision.Plan.BasePrice.Text': {'$lte': " + maxPrice + "}";
                }


                if (!string.IsNullOrEmpty(noOfBeds) && noOfBeds != "undefined")
                {
                    if (Convert.ToInt32(noOfBeds) >= 5 || Convert.ToInt32(noOfBeds) == 0)
                    {
                        queryString = (!string.IsNullOrEmpty(queryString)) ? queryString + "," + "'Subdivision.Plan.Bedrooms.Text': {'$gte': " + noOfBeds + "}" : ",'Subdivision.Plan.Bedrooms.Text': {'$gte': " + noOfBeds + "}";
                    }
                    else
                    {
                        queryString = (!string.IsNullOrEmpty(queryString)) ? queryString + "," + "'Subdivision.Plan.Bedrooms.Text':{'$gte':  " + noOfBeds + "}" : ",'Subdivision.Plan.Bedrooms.Text': {'$gte': " + noOfBeds + "}";
                    }
                }


                if (!string.IsNullOrEmpty(noOfBathroom) && noOfBathroom != "undefined")
                {
                    if (Convert.ToDecimal(noOfBathroom) >= 5 || Convert.ToDecimal(noOfBathroom) == 0)
                    {
                        queryString = (!string.IsNullOrEmpty(queryString)) ? queryString + "," + "'Subdivision.Plan.Baths': {'$gte': " + noOfBathroom + "}" : ",'Subdivision.Plan.Baths': {'$gte': " + noOfBathroom + "}";
                    }
                    else
                    {
                        if (noOfBathroom.Contains('.'))
                        {
                            queryString = (!string.IsNullOrEmpty(queryString)) ? queryString + "," + "'Subdivision.Plan.Baths': {'$gte': " + noOfBathroom.Split('.')[0] + "},'Subdivision.Plan.HalfBaths': {'$gte': " + (Convert.ToInt16(noOfBathroom.Split('.')[1]) > 0 ? 1 : 0) + "}" : ",'Subdivision.Plan.Baths': {'$gte': " + noOfBathroom.Split('.')[0] + "},'Subdivision.Plan.HalfBaths': {'$gte': " + (Convert.ToInt16(noOfBathroom.Split('.')[1]) > 0 ? 1 : 0) + "}";
                        }
                        else
                        {
                            queryString = (!string.IsNullOrEmpty(queryString)) ? queryString + "," + "'Subdivision.Plan.Baths': {'$gte': " + noOfBathroom + "}" : ",'Subdivision.Plan.Baths': {'$gte': " + noOfBathroom + "}";
                        }
                    }
                }


                if (!string.IsNullOrEmpty(size) && size != "undefined")
                {
                    queryString = (!string.IsNullOrEmpty(queryString)) ? queryString + "," + "'Subdivision.Plan.BaseSqft': {'$gte': " + size + "}" : ",'Subdivision.Plan.BaseSqft': {'$gte': " + size + "}";
                }


                if (!string.IsNullOrEmpty(lotSize) && lotSize != "undefined")
                {
                    queryString = (!string.IsNullOrEmpty(queryString)) ? queryString + "," + "'LotSize.Text': {'$gte': " + lotSize + "}" : "'LotSize.Text': {'$gte': " + lotSize + "}";
                }


                if (!string.IsNullOrEmpty(homeAge) && homeAge != "undefined")
                {
                    if (Convert.ToInt32(homeAge) == 51 || Convert.ToInt32(homeAge) == 0)
                    {
                        queryString = (!string.IsNullOrEmpty(queryString)) ? queryString + "," + "'YearBuilt': {'$gte': " + homeAge + "}" : "'YearBuilt': {'$gte': " + homeAge + "}";
                    }
                    else
                    {
                        queryString = (!string.IsNullOrEmpty(queryString)) ? queryString + "," + "'YearBuilt': {'$lte': " + homeAge + "}" : "'YearBuilt': {'$lte': " + homeAge + "}";
                    }
                }

                if (lstPropertyType.Count > 0)
                {
                    string propertyType = null;
                    foreach (string item in lstPropertyType)
                    {
                        propertyType = string.IsNullOrEmpty(item) ? null : (string.IsNullOrEmpty(propertyType) ? "{ 'Subdivision.Plan.Type' : '" + item + "'}" : propertyType + ",{ 'Subdivision.Plan.Type' : '" + item + "'}");
                    }
                    queryString = string.IsNullOrEmpty(propertyType) ? queryString : queryString + ",$or : [ " + propertyType + "]";
                }

                return queryString;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void UploadWholeFile(HttpFileCollectionBase httpFileCollectionBase, string uniqueId)
        {
            string StorageRoot1 = System.Web.HttpContext.Current.Server.MapPath("~/SDRE/" + uniqueId);
            bool exists = System.IO.Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/SDRE/" + uniqueId));
            if (!exists)
                System.IO.Directory.CreateDirectory(StorageRoot1);

            for (int i = 0; i < httpFileCollectionBase.Count; i++)
            {
                var file = httpFileCollectionBase[i];
                var fileName = file.FileName;
                var fullPath = StorageRoot1 + "\\" + fileName;
                file.SaveAs(fullPath);
                //var url = context.Request.Url.Scheme + "://" + context.Request.Url.Authority + "/SDRE/" + uniqueId + "/" + fileName;
                //string fullName = Path.GetFileName(file.FileName);
                //statuses.Add(new UploadAgentFileStatus(url, file.ContentLength, fullPath));
                //var _agent = NinjectConfig.Get<IAgent>();
                ////_agent = new MongoDbRepository.Implementation.Admin
                //_agent.UploadProfileImage(uniqueId, url);
            }
        }
        
    }
}
