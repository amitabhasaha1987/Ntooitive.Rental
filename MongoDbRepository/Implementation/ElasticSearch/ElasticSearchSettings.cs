using System;
using System.Collections.Generic;
using System.Configuration;
using Nest;
using Repositories.Interfaces.ElasticSearch;
using Repositories.Models.ListHub;

namespace Core.Implementation.ElasticSearch
{
    public class ElasticSearchSettings : ElasticSearchConnection, IElasticSearchSettings
    {
        public bool CreateSettings()
        {
            var indexName = ConfigurationManager.AppSettings["ElasticSearch:Index_Name"];
            var synonymsPath = ConfigurationManager.AppSettings["ElasticSearch:SynonymsPath"];
            var stopwordsPath = ConfigurationManager.AppSettings["ElasticSearch:StopwordsPath"];

            bool isAcknowledged = false;
            if (!IndexExists(indexName))
            {

                isAcknowledged = GetConnection().Value.CreateIndex(indexName, c => c
                    .Analysis(des => des
                        .Analyzers(bases => bases
                            .Add("attributes_analyzer", new CustomAnalyzer()
                            {
                                Filter = new List<string>
                                {
                                    "stemmer",
                                    "lowercase",
                                    "stopword",
                                    "synonym",
                                    "kstem",
                                    "shingle",
                                    "charreplace",
                                    "charreplaceend",
                                    "trim",
                                    "unique",
                                    "length",
                                    //"commonwords",
                                    "keeptype"
                                },
                                Tokenizer = "standard"
                            }))
                        .TokenFilters(filter => filter

                            .Add("stopword", new StopTokenFilter
                            {
                                IgnoreCase = true,
                                StopwordsPath = stopwordsPath
                            })

                            .Add("stemmer", new StemmerTokenFilter
                            {

                                Language = "possessive_english"
                            })

                            .Add("synonym", new SynonymTokenFilter
                            {
                                IgnoreCase = true,
                                SynonymsPath = synonymsPath,
                                Tokenizer = "keyword"
                            })
                            .Add("shingle", new ShingleTokenFilter
                            {
                                MinShingleSize = 2,
                                MaxShingleSize = 3,
                                OutputUnigrams = true,
                                OutputUnigramsIfNoShingles = false
                            })
                            .Add("length", new LengthTokenFilter
                            {
                                Minimum = 4

                            })
                            .Add("commonwords", new CommonGramsTokenFilter
                            {
                                CommonWords = new List<string> {"floor", "parking", "architecture", "appliance"},
                                IgnoreCase = true,
                                QueryMode = true
                            })
                            .Add("keeptype", new KeepTypesTokenFilter
                            {
                                Types = new List<string>() {"shingle", "<ALPHANUM>", "<NUM>"}
                            })
                            .Add("charreplace", new PatternReplaceTokenFilter
                            {
                                Pattern = "^_(.*)",
                                Replacement = "$1"
                            })
                            .Add("charreplaceanywhere", new PatternReplaceTokenFilter
                            {
                                Pattern = "[_]",
                                Replacement = ""
                            })
                            //
                            .Add("charreplaceend", new PatternReplaceTokenFilter
                            {
                                Pattern = "_$",
                                Replacement = ""
                            })

                            .Add("unique", new UniqueTokenFilter
                            {
                                OnlyOnSamePosition = false
                            }))
                    )
                    .AddMapping<Rental>(m =>
                        m.Properties(props => props
                            .String(sprop => sprop.Name(prop => prop.Attributes).Analyzer("attributes_analyzer"))
                            .Number(sprop => sprop.Name(prop => prop.Price).Index(NonStringIndexOption.NotAnalyzed))))

                            

                    .AddMapping<Repositories.Models.ListHub.NewHome>(m =>
                        m.Properties(props => props
                            .String(sprop => sprop.Name(prop => prop.Attributes).Analyzer("attributes_analyzer"))
                            .Number(sprop => sprop.Name(prop => prop.Price).Index(NonStringIndexOption.NotAnalyzed))))


                    .AddMapping<Purchase>(m =>
                        m.Properties(props => props
                            .String(sprop => sprop.Name(prop => prop.Attributes).Analyzer("attributes_analyzer"))
                            .Number(sprop => sprop.Name(prop => prop.Price).Index(NonStringIndexOption.NotAnalyzed)))))
                    .Acknowledged;

            }
            return isAcknowledged;
        }


        public Lazy<ElasticClient> GetConnection()
        {
            return new Lazy<ElasticClient>(BuildConnection);
        }

        public bool IndexExists(string indexName)
        {
            return GetConnection().Value.IndexExists(i => i.Index(indexName)).Exists;
        }
    }
}
