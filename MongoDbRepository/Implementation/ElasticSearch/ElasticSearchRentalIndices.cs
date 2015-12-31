using System.Collections.Generic;
using System.Linq;
using Nest;
using Repositories.Interfaces.ElasticSearch;
using Repositories.Models.Common;
using Repositories.Models.ListHub;
using System;

namespace Core.Implementation.ElasticSearch
{
    public class ElasticSearchRentalIndices : IElasticSearchIndices<Rental>
    {
        private readonly IElasticSearchSettings _elasticSearchSettings;

        public ElasticSearchRentalIndices(IElasticSearchSettings elasticSearchSettings)
        {
            _elasticSearchSettings = elasticSearchSettings;

        }



        /// <summary>
        /// MlsNumber is mapped with id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Rental Get(string Id)
        {
            var response = _elasticSearchSettings.GetConnection().Value.Get<Rental>(Id);
            return response.Source;
        }

        public bool CreateIndex(Rental entity)
        {
            bool finalResult = false;
            if (Get(entity.MlsNumber) != null)
            {
                finalResult = UpdateIndex(entity);
            }
            else
            {
                var result = _elasticSearchSettings.GetConnection().Value.Index(entity);
                finalResult = result.Created;
            }
            return finalResult;
        }

        public void CreateBulkIndex(List<Rental> entites)
        {
            foreach (var entite in entites)
            {
                this.CreateIndex(entite);
            }
        }
        public bool RemoveIndex(string mlsid)
        {
            var response = _elasticSearchSettings.GetConnection().Value.Delete<Rental>(mlsid);

            return response != null && response.Found;
        }
        public bool UpdateIndex(Rental request)
        {
            var response = _elasticSearchSettings.GetConnection().Value.Update<Rental, object>(u => u
                .Id(request.MlsNumber)
                .Doc(request)
                .DocAsUpsert()
                .Refresh(true)
                );
            return response != null;
        }

        public dynamic SearchQuery(AdvanceSearch advSearch, int startIndex, int limit, out int totalcount, bool IsOpenHouse = false)
        {
            var resultDoc = new List<PropertyListing>();
            int count = 0;
            List<QueryContainer> queries = GenerateQuery.CreateQuery(advSearch);
            string queryPropertyContainer = null;
            if (advSearch.SelectedProperty.Count > 0)
            {
                foreach (string item in advSearch.SelectedProperty)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        queryPropertyContainer = string.IsNullOrEmpty(queryPropertyContainer) ? item : queryPropertyContainer + " " + item;
                    }
                }
            }
            string querySubPropertyContainer = null;
            if (advSearch.SelectedSubProperty.Count > 0)
            {
                foreach (string item in advSearch.SelectedSubProperty)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        querySubPropertyContainer = string.IsNullOrEmpty(querySubPropertyContainer) ? item : querySubPropertyContainer + " " + item;
                    }
                }
            }

            if (advSearch.SortBy == "0" || advSearch.SortBy == "Price_Asc")
            {
                var result = _elasticSearchSettings.GetConnection().Value.Search<Rental>
                    (
                         body => body.Query(q =>
                                                 q.Filtered(bq =>
                                                         bq.Query(x => x.Bool(y => y.Must(queries.ToArray())


                                                            .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))))
                                                       )
                                               )
                                               .Filter(x =>
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertyType).Query(queryPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertySubType.Text).Query(querySubPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.AgentName).Query(advSearch.AgentName).Operator(Operator.And))) &&
                                                   x.Range(range => range.OnField(z => z.ClassifiedExpireDate).GreaterOrEquals(DateTime.Today.Date))
                                                   )
                                               .Sort(sort => sort.OnField("price").Ascending()).Skip(startIndex).Take(limit)
                     );

                resultDoc = result.Documents.ToList<PropertyListing>();
                count = (int)result.Total;
            }
            else if (advSearch.SortBy == "Price_Desc")
            {
                var result = _elasticSearchSettings.GetConnection().Value.Search<Rental>
                    (
                         body => body.Query(q =>
                                                 q.Filtered(bq =>
                                                         bq.Query(x => x.Bool(y => y.Must(queries.ToArray())
                                                            .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))))
                                                       )
                                               )
                                               .Filter(x =>
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertyType).Query(queryPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertySubType.Text).Query(querySubPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.AgentName).Query(advSearch.AgentName).Operator(Operator.And))) &&
                                                   x.Range(range => range.OnField(z => z.ClassifiedExpireDate).GreaterOrEquals(DateTime.Today.Date))
                                                   )
                                               .Sort(sort => sort.OnField("price").Descending()).Skip(startIndex).Take(limit)
                     );
                resultDoc = result.Documents.ToList<PropertyListing>();
                count = (int)result.Total;
            }
            else if (advSearch.SortBy == "Newest_Asc")
            {
                var result = _elasticSearchSettings.GetConnection().Value.Search<Rental>
                    (
                         body => body.Query(q =>
                                                 q.Filtered(bq =>
                                                         bq.Query(x => x.Bool(y => y.Must(queries.ToArray())
                                                            .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))))
                                                       )
                                               )
                                               .Filter(x =>
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertyType).Query(queryPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertySubType.Text).Query(querySubPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.AgentName).Query(advSearch.AgentName).Operator(Operator.And))) &&
                                                   x.Range(range => range.OnField(z => z.ClassifiedExpireDate).GreaterOrEquals(DateTime.Today.Date))
                                                   )
                                               .Sort(sort => sort.OnField("entryDate").Ascending()).Skip(startIndex).Take(limit)
                     );
                resultDoc = result.Documents.ToList<PropertyListing>();
                count = (int)result.Total;
            }
            else if (advSearch.SortBy == "Newest_Desc")
            {
                var result = _elasticSearchSettings.GetConnection().Value.Search<Rental>
                    (
                         body => body.Query(q =>
                                                 q.Filtered(bq =>
                                                         bq.Query(x => x.Bool(y => y.Must(queries.ToArray())
                                                            .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))))
                                                       )
                                               )
                                               .Filter(x =>
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertyType).Query(queryPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertySubType.Text).Query(querySubPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.AgentName).Query(advSearch.AgentName).Operator(Operator.And))) &&
                                                   x.Range(range => range.OnField(z => z.ClassifiedExpireDate).GreaterOrEquals(DateTime.Today.Date))
                                                   )
                                               .Sort(sort => sort.OnField("entryDate").Descending()).Skip(startIndex).Take(limit)
                     );
                resultDoc = result.Documents.ToList<PropertyListing>();
                count = (int)result.Total;
            }
            else if (advSearch.SortBy == "OpenHouseDate")
            {
                var result = _elasticSearchSettings.GetConnection().Value.Search<Rental>
                    (
                         body => body.Query(q =>
                                                 q.Filtered(bq =>
                                                         bq.Query(x => x.Bool(y => y.Must(queries.ToArray())
                                                            .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))))
                                                       )
                                               )
                                               .Filter(x =>
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertyType).Query(queryPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertySubType.Text).Query(querySubPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.AgentName).Query(advSearch.AgentName).Operator(Operator.And))) &&
                                                   x.Range(range => range.OnField(z => z.ClassifiedExpireDate).GreaterOrEquals(DateTime.Today.Date))
                                                   )
                                               .Sort(sort => sort.OnField("dateTimeRanges.openHouseStartDateTime").Ascending()).Skip(startIndex).Take(limit)
                     );
                resultDoc = result.Documents.ToList<PropertyListing>();
                count = (int)result.Total;
            }
            else if (advSearch.SortBy == "Classified")
            {
                var result = _elasticSearchSettings.GetConnection().Value.Search<Rental>
                    (
                         body => body.Query(q =>
                                             q.Filtered(bq =>
                                                     bq.Query(x => x.Bool(y => y.Must(queries.ToArray())
                                                        .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))))
                                                   )
                                           )
                                               .Filter(x =>
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertyType).Query(queryPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertySubType.Text).Query(querySubPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.AgentName).Query(advSearch.AgentName).Operator(Operator.And))) &&
                                                   x.Range(range => range.OnField(z => z.ClassifiedExpireDate).GreaterOrEquals(DateTime.Today.Date))
                                                   )
                                               .Sort(sort => sort.OnField("classifiedExpireDate").Descending()).Skip(startIndex).Take(limit)
                     );
                resultDoc = result.Documents.ToList<PropertyListing>();
                count = (int)result.Total;
            }
            totalcount = count;
            return resultDoc;
        }

        //Rental have no openhouse feature 
        public dynamic SearchFeaturedQuery(AdvanceSearch advSearch, int startIndex, int limit, out int totalcount, bool IsOpenHouse = false)
        {
            var resultDoc = new List<PropertyListing>();
            int count = 0;
            List<QueryContainer> queries = GenerateQuery.CreateQuery(advSearch);
            var termQuery = Query<Rental>
                 .Term("isFeatured", "TRUE", 5d);
            queries.Add(termQuery);

            string queryPropertyContainer = null;
            if (advSearch.SelectedProperty.Count > 0)
            {
                foreach (string item in advSearch.SelectedProperty)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        queryPropertyContainer = string.IsNullOrEmpty(queryPropertyContainer) ? item : queryPropertyContainer + " " + item;
                    }
                }
            }

            string querySubPropertyContainer = null;
            if (advSearch.SelectedSubProperty.Count > 0)
            {
                foreach (string item in advSearch.SelectedSubProperty)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        querySubPropertyContainer = string.IsNullOrEmpty(querySubPropertyContainer) ? item : querySubPropertyContainer + " " + item;
                    }
                }
            }

            if (advSearch.SortBy == "0" || advSearch.SortBy == "Price_Asc")
            {
                var result = _elasticSearchSettings.GetConnection().Value.Search<Rental>
                    (
                         body => body.Query(q =>
                                                 q.Filtered(bq =>
                                                         bq.Query(x => x.Bool(y => y.Must(queries.ToArray())
                                                            .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))))
                                                       )
                                               )
                                               .Filter(x =>
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertyType).Query(queryPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertySubType.Text).Query(querySubPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.AgentName).Query(advSearch.AgentName).Operator(Operator.And))) &&
                                                   x.Range(range => range.OnField(z => z.ClassifiedExpireDate).GreaterOrEquals(DateTime.Today.Date))
                                                   )
                                               .Sort(sort => sort.OnField("price").Ascending()).Skip(startIndex).Take(limit)
                     );
                resultDoc = result.Documents.ToList<PropertyListing>();
                count = (int)result.Total;
            }
            else if (advSearch.SortBy == "Price_Desc")
            {
                var result = _elasticSearchSettings.GetConnection().Value.Search<Rental>
                    (
                         body => body.Query(q =>
                                                 q.Filtered(bq =>
                                                         bq.Query(x => x.Bool(y => y.Must(queries.ToArray())
                                                            .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))))
                                                       )
                                               )
                                               .Filter(x =>
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertyType).Query(queryPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertySubType.Text).Query(querySubPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.AgentName).Query(advSearch.AgentName).Operator(Operator.And))) &&
                                                   x.Range(range => range.OnField(z => z.ClassifiedExpireDate).GreaterOrEquals(DateTime.Today.Date))
                                                   )
                                               .Sort(sort => sort.OnField("price").Descending()).Skip(startIndex).Take(limit)
                     );
                resultDoc = result.Documents.ToList<PropertyListing>();
                count = (int)result.Total;
            }
            else if (advSearch.SortBy == "Newest_Asc")
            {
                var result = _elasticSearchSettings.GetConnection().Value.Search<Rental>
                    (
                         body => body.Query(q =>
                                                 q.Filtered(bq =>
                                                         bq.Query(x => x.Bool(y => y.Must(queries.ToArray())
                                                            .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))))
                                                       )
                                               )
                                               .Filter(x =>
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertyType).Query(queryPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertySubType.Text).Query(querySubPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.AgentName).Query(advSearch.AgentName).Operator(Operator.And))) &&
                                                   x.Range(range => range.OnField(z => z.ClassifiedExpireDate).GreaterOrEquals(DateTime.Today.Date))
                                                   )
                                               .Sort(sort => sort.OnField("entryDate").Ascending()).Skip(startIndex).Take(limit)
                     );
                resultDoc = result.Documents.ToList<PropertyListing>();
                count = (int)result.Total;
            }
            else if (advSearch.SortBy == "Newest_Desc")
            {
                var result = _elasticSearchSettings.GetConnection().Value.Search<Rental>
                    (
                         body => body.Query(q =>
                                                 q.Filtered(bq =>
                                                         bq.Query(x => x.Bool(y => y.Must(queries.ToArray())
                                                            .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))))
                                                       )
                                               )
                                               .Filter(x =>
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertyType).Query(queryPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertySubType.Text).Query(querySubPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.AgentName).Query(advSearch.AgentName).Operator(Operator.And))) &&
                                                   x.Range(range => range.OnField(z => z.ClassifiedExpireDate).GreaterOrEquals(DateTime.Today.Date))
                                                   )
                                               .Sort(sort => sort.OnField("entryDate").Descending()).Skip(startIndex).Take(limit)
                     );
                resultDoc = result.Documents.ToList<PropertyListing>();
                count = (int)result.Total;
            }
            else if (advSearch.SortBy == "OpenHouseDate")
            {
                var result = _elasticSearchSettings.GetConnection().Value.Search<Rental>
                    (
                         body => body.Query(q =>
                                                 q.Filtered(bq =>
                                                         bq.Query(x => x.Bool(y => y.Must(queries.ToArray())
                                                            .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))))
                                                       )
                                               )
                                               .Filter(x =>
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertyType).Query(queryPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertySubType.Text).Query(querySubPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.AgentName).Query(advSearch.AgentName).Operator(Operator.And))) &&
                                                   x.Range(range => range.OnField(z => z.ClassifiedExpireDate).GreaterOrEquals(DateTime.Today.Date))
                                                   )
                                               .Sort(sort => sort.OnField("dateTimeRanges.openHouseStartDateTime").Ascending()).Skip(startIndex).Take(limit)
                     );
                resultDoc = result.Documents.ToList<PropertyListing>();
                count = (int)result.Total;
            }
            else if (advSearch.SortBy == "Classified")
            {
                var result = _elasticSearchSettings.GetConnection().Value.Search<Rental>
                    (
                         body => body.Query(q =>
                                                 q.Filtered(bq =>
                                                         bq.Query(x => x.Bool(y => y.Must(queries.ToArray())
                                                            .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))))
                                                       )
                                               )
                                               .Filter(x =>
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertyType).Query(queryPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.PropertySubType.Text).Query(querySubPropertyContainer).Operator(Operator.Or))) &&
                                                   x.Query(y => y.Match(z => z.OnField(m => m.AgentName).Query(advSearch.AgentName).Operator(Operator.And))) &&
                                                   x.Range(range => range.OnField(z => z.ClassifiedExpireDate).GreaterOrEquals(DateTime.Today.Date))
                                                   )
                                               .Sort(sort => sort.OnField("classifiedExpireDate").Descending()).Skip(startIndex).Take(limit)
                     );
                resultDoc = result.Documents.ToList<PropertyListing>();
                count = (int)result.Total;
            }

            totalcount = count;
            return resultDoc;
        }


        public dynamic GetMlsNumber(AdvanceSearch advSearch, int index)
        {
            string mlsNumber = null;

            List<QueryContainer> queries = GenerateQuery.CreateQuery(advSearch);
            string queryContainer = null;
            if (advSearch.SelectedProperty.Count > 0)
            {
                int i = 0;
                foreach (string item in advSearch.SelectedProperty)
                {
                    i++;
                    if (!string.IsNullOrEmpty(item))
                    {
                        queryContainer = string.IsNullOrEmpty(queryContainer) ? item : queryContainer + " " + item;
                    }
                }
            }

            if (advSearch.SortBy == "0")
            {
                var result = _elasticSearchSettings.GetConnection().Value.Search<Rental>
                  (
                       body => body.Query(q =>
                                             q.Filtered(bq =>
                                                     bq.Query(x => x.Bool(y => y.Must(queries.ToArray())
                                                        .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))))
                                                   )
                                           ).Filter(x => x.Query(y => y.Match(z => z.OnField(m => m.PropertyType).Query(queryContainer).Operator(Operator.Or))))
                                           .Sort(sort => sort.OnField("price").Ascending()).Skip(index).Take(1)
                   );
                mlsNumber = result.Documents.Select(m => m.MlsNumber).ToList<string>().FirstOrDefault();
            }
            else if (advSearch.SortBy == "Price_Asc")
            {

                var result = _elasticSearchSettings.GetConnection().Value.Search<Rental>
                    (
                         body => body.Query(q =>
                                             q.Filtered(bq =>
                                                     bq.Query(x => x.Bool(y => y.Must(queries.ToArray())
                                                        .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))))
                                                   )
                                           ).Filter(x => x.Query(y => y.Match(z => z.OnField(m => m.PropertyType).Query(queryContainer).Operator(Operator.Or))))
                                               .Sort(sort => sort.OnField("price").Ascending()).Skip(index).Take(1)
                   );
                mlsNumber = result.Documents.Select(m => m.MlsNumber).ToList<string>().FirstOrDefault();
            }
            else if (advSearch.SortBy == "Price_Desc")
            {
                var result = _elasticSearchSettings.GetConnection().Value.Search<Rental>
                    (
                         body => body.Query(q =>
                                             q.Filtered(bq =>
                                                     bq.Query(x => x.Bool(y => y.Must(queries.ToArray())
                                                        .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))))
                                                   )
                                           ).Filter(x => x.Query(y => y.Match(z => z.OnField(m => m.PropertyType).Query(queryContainer).Operator(Operator.Or))))
                                               .Sort(sort => sort.OnField("price").Descending()).Skip(index).Take(1)
                   );
                mlsNumber = result.Documents.Select(m => m.MlsNumber).ToList<string>().FirstOrDefault();
            }
            else if (advSearch.SortBy == "Newest_Asc")
            {
                var result = _elasticSearchSettings.GetConnection().Value.Search<Rental>
                    (
                         body => body.Query(q =>
                                             q.Filtered(bq =>
                                                     bq.Query(x => x.Bool(y => y.Must(queries.ToArray())
                                                        .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))))
                                                   )
                                           ).Filter(x => x.Query(y => y.Match(z => z.OnField(m => m.PropertyType).Query(queryContainer).Operator(Operator.Or))))
                                               .Sort(sort => sort.OnField("entryDate").Ascending()).Skip(index).Take(1)
                   );
                mlsNumber = result.Documents.Select(m => m.MlsNumber).ToList<string>().FirstOrDefault();
            }
            else if (advSearch.SortBy == "Newest_Desc")
            {
                var result = _elasticSearchSettings.GetConnection().Value.Search<Rental>
                    (
                         body => body.Query(q =>
                                             q.Filtered(bq =>
                                                     bq.Query(x => x.Bool(y => y.Must(queries.ToArray())
                                                        .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))))
                                                   )
                                           ).Filter(x => x.Query(y => y.Match(z => z.OnField(m => m.PropertyType).Query(queryContainer).Operator(Operator.Or))))
                                               .Sort(sort => sort.OnField("entryDate").Descending()).Skip(index).Take(1)
                   );
                mlsNumber = result.Documents.Select(m => m.MlsNumber).ToList<string>().FirstOrDefault();
            }
            else if (advSearch.SortBy == "OpenHouseDate")
            {
                var result = _elasticSearchSettings.GetConnection().Value.Search<Rental>
                    (
                         body => body.Query(q =>
                                             q.Filtered(bq =>
                                                     bq.Query(x => x.Bool(y => y.Must(queries.ToArray())
                                                        .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))))
                                                   )
                                           ).Filter(x => x.Query(y => y.Match(z => z.OnField(m => m.PropertyType).Query(queryContainer).Operator(Operator.Or))))
                                               .Sort(sort => sort.OnField("dateTimeRanges.openHouseStartDateTime").Ascending()).Skip(index).Take(1)
                   );
                mlsNumber = result.Documents.Select(m => m.MlsNumber).ToList<string>().FirstOrDefault();
            }
            else if (advSearch.SortBy == "Classified")
            {
                var result = _elasticSearchSettings.GetConnection().Value.Search<Rental>
                    (
                         body => body.Query(q =>
                                             q.Filtered(bq =>
                                                     bq.Query(x => x.Bool(y => y.Must(queries.ToArray())
                                                        .Should(sh => sh.Term("isSpotlight", "TRUE", 5d))))
                                                   )
                                           ).Filter(x => x.Query(y => y.Match(z => z.OnField(m => m.PropertyType).Query(queryContainer).Operator(Operator.Or))) &&
                                               x.Range(range => range.OnField(z => z.ClassifiedExpireDate).GreaterOrEquals(DateTime.Today.Date)))
                                            .Sort(sort => sort.OnField("classifiedExpireDate").Descending()).Skip(index).Take(1)
                   );
                mlsNumber = result.Documents.Select(m => m.MlsNumber).ToList<string>().FirstOrDefault();
            }
            return mlsNumber;
        }

        public List<Rental> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
