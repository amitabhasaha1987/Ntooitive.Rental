using Nest;
using Repositories.Models.Common;
using Repositories.Models.ListHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Implementation.ElasticSearch
{
    public static class GenerateQuery
    {
        public static List<QueryContainer> CreateQuery(AdvanceSearch advSearch)
        {
            try
            {
                var queries = new List<QueryContainer>();
                if (advSearch != null)
                {
                    if (!string.IsNullOrEmpty(advSearch.Location))
                    {
                        var matchQuery = Query<Purchase>.Match(x => x.OnField(m => m.Attributes).Query(advSearch.Location));
                        queries.Add(matchQuery);
                    }

                    if (!string.IsNullOrEmpty(advSearch.SearchTerm) && string.IsNullOrEmpty(advSearch.MlsNumber))
                    {
                        var matchQuery = Query<Purchase>.Match(x => x.OnField(m => m.Attributes).Query(advSearch.SearchTerm));
                        queries.Add(matchQuery);
                    }

                    if (!string.IsNullOrEmpty(advSearch.MlsNumber))
                    {
                        var matchQuery = Query<Purchase>.Match(x => x.OnField(m => m.MlsNumber).Query(advSearch.MlsNumber));
                        queries.Add(matchQuery);
                    }

                    if (!string.IsNullOrEmpty(advSearch.MinPrice) && !string.IsNullOrEmpty(advSearch.MaxPrice))
                    {
                        var termQuery = Query<PropertyListing>.Filtered(fq => fq.Query(q => q.MatchAll()).Filter(f => f.Range(ff => ff.OnField(x => x.Price).GreaterOrEquals(advSearch.MinPrice).LowerOrEquals(advSearch.MaxPrice))));
                        queries.Add(termQuery);
                    }
                    else if (!string.IsNullOrEmpty(advSearch.MinPrice) && string.IsNullOrEmpty(advSearch.MaxPrice))
                    {
                        var termQuery = Query<PropertyListing>.Filtered(fq => fq.Query(q => q.MatchAll()).Filter(f => f.Range(ff => ff.OnField(x => x.Price).GreaterOrEquals(advSearch.MinPrice))));
                        queries.Add(termQuery);
                    }
                    else if (string.IsNullOrEmpty(advSearch.MinPrice) && !string.IsNullOrEmpty(advSearch.MaxPrice))
                    {
                        var termQuery = Query<PropertyListing>.Filtered(fq => fq.Query(q => q.MatchAll()).Filter(f => f.Range(ff => ff.OnField(x => x.Price).LowerOrEquals(advSearch.MaxPrice))));
                        queries.Add(termQuery);
                    }

                    if (!string.IsNullOrEmpty(advSearch.NoOfBeds) && advSearch.NoOfBeds != "undefined")
                    {
                        var termQuery = Query<PropertyListing>.Filtered(fq => fq.Query(q => q.MatchAll()).Filter(f => f.Range(ff => ff.OnField(x => x.NoOfBedRooms).GreaterOrEquals(advSearch.NoOfBeds))));
                        queries.Add(termQuery);
                    }

                    if (!string.IsNullOrEmpty(advSearch.NoOfBathroom) && advSearch.NoOfBathroom != "undefined")
                    {

                        if (advSearch.NoOfBathroom.Contains('.'))
                        {
                            var termQuery = Query<PropertyListing>.Filtered(fq => fq.Query(q => q.MatchAll()).Filter(f => f.Range(ff => ff.OnField(x => x.NoOfBathRooms).GreaterOrEquals(advSearch.NoOfBathroom.Split('.')[0]))));
                            queries.Add(termQuery);
                            termQuery = Query<PropertyListing>.Filtered(fq => fq.Query(q => q.MatchAll()).Filter(f => f.Range(ff => ff.OnField(x => x.NoOfHalfBathRooms).Greater(0))));
                            queries.Add(termQuery);
                        }
                        else
                        {
                            var termQuery = Query<PropertyListing>.Filtered(fq => fq.Query(q => q.MatchAll()).Filter(f => f.Range(ff => ff.OnField(x => x.NoOfBathRooms).GreaterOrEquals(advSearch.NoOfBathroom))));
                            queries.Add(termQuery);
                        }
                    }

                    if (!string.IsNullOrEmpty(advSearch.Size) && advSearch.Size != "undefined")
                    {
                        var termQuery = Query<PropertyListing>.Filtered(fq => fq.Query(q => q.MatchAll()).Filter(f => f.Range(ff => ff.OnField(x => x.LivingArea).GreaterOrEquals(advSearch.Size))));
                        queries.Add(termQuery);
                    }


                    if (!string.IsNullOrEmpty(advSearch.LotSize) && advSearch.LotSize != "undefined")
                    {
                        var termQuery = Query<PropertyListing>.Filtered(fq => fq.Query(q => q.MatchAll()).Filter(f => f.Range(ff => ff.OnField(x => x.LotSize.Text).GreaterOrEquals(advSearch.LotSize))));
                        queries.Add(termQuery);
                    }


                    if (!string.IsNullOrEmpty(advSearch.HomeAge) && advSearch.HomeAge != "undefined")
                    {
                        if (Convert.ToInt32(advSearch.HomeAge) == 51 || Convert.ToInt32(advSearch.HomeAge) == 0)
                        {
                            var termQuery = Query<PropertyListing>.Filtered(fq => fq.Query(q => q.MatchAll()).Filter(f => f.Range(ff => ff.OnField(x => x.YearBuilt).GreaterOrEquals(advSearch.LotSize))));
                            queries.Add(termQuery);
                        }
                        else
                        {
                            var termQuery = Query<PropertyListing>.Filtered(fq => fq.Query(q => q.MatchAll()).Filter(f => f.Range(ff => ff.OnField(x => x.YearBuilt).LowerOrEquals(advSearch.LotSize))));
                            queries.Add(termQuery);
                        }
                    }
                    //if (!string.IsNullOrEmpty(advSearch.AgentName))
                    //{
                    //    var termQuery = Query<PropertyListing>.Filtered(fq => fq.Query(q => q.MatchAll()).Filter(f => f.Bool(ff => ff.Must(x=>x.Terms(y => y.AgentName,advSearch.AgentName)))));
                    //    queries.Add(termQuery);
                    //}
                    if (!string.IsNullOrEmpty(advSearch.KeyWards))
                    {
                        var matchQuery = Query<PropertyListing>.Match(x => x.OnField(m => m.Attributes).Query(advSearch.KeyWards));
                        queries.Add(matchQuery);
                    }
                }
                else
                {
                    queries = null;
                }
                return queries;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
