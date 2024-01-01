namespace Mvp.Project.MvpSite
{
    public class Constants
    {
        public static class GraphQlQueries
        {
            //the following query tested to return lookupfield displaynames
            public const string GetSitemapQuery = @"query SitemapQuery(
              $rootItemId: String
              $language: String
              $pageSize: Int = 100
              $hasLayout: String = ""true""
              $after: String
            ) {
              search(
                where: {
                  AND: [
                    { name: ""_path"", value: $rootItemId, operator: CONTAINS } 
                    { name: ""_language"", value: $language }
                    { name: ""_hasLayout"", value: $hasLayout }
                    { name: ""IncludeinSitemap"", value: ""true"" }
                  ]
                }
                first: $pageSize
                after: $after
              ) {
                total
                pageInfo {
                  endCursor
                  hasNext
                }
                results {
                  updateddatetime: field(name: ""__updated"") {
                    value
                  }
                  url {
                    path
                  }
                  name
                  ... on _Sitemap {
                    priority {
                      targetItem {
                        displayName
                      }
                    }
                    changeFrequency {
                      targetItem {
                        displayName
                      }
                    }
                  }
                }
              }
            }";

            //public const string GetSitemapXmlData = @"query {
            //  sitemapQuery: item(path: ""908351357A914446A08D17436464FFAF"", language: ""en"") {
            //    displayName
            //    ... on Sitemap {
            //      sitemapXml {
            //        value
            //      }
            //    }
            //  }
            //}";

            //public const string GetSitemapXmlData = @"query sitemapQuery(
            //  $itemId: String = ""908351357A914446A08D17436464FFAF""
            //  $language: String = ""en""
            //) {
            //  item(path: $itemId, language: $language) {
            //    displayName
            //    ... on _SitemapXmlData {
            //      myfield {
            //        value
            //      }
            //      sitemapXml {
            //        value
            //      }
            //    }
            //  }
            //}";

            public const string GetSitemapXmlData = @"query(
              $itemId: String = ""908351357A914446A08D17436464FFAF""
              $language: String = ""en""
            ) {
              item(path: $itemId, language: $language) {
                ... on _SitemapXmlData {
                  fields {
                    name
                    id
                    value
                  }
                }
              }
            }";

            public const string GetSitemapSearchQuery = @"# query that returns the mvp site map field in content tree with id 90835135-7A91-4446-A08D-17436464FFAF
                    query SitemapQuery(
                      $rootItemId: String = ""90835135-7A91-4446-A08D-17436464FFAF""
                      $language: String = ""en""
                      $pageSize: Int = 1
                      $hasLayout: String = ""false""
                      $after: String
                    ) {
                      search(
                        where: {
                          AND: [
                            { name: ""_path"", value: $rootItemId, operator: EQ }        
                            { name: ""_language"", value: $language }
                            { name: ""_hasLayout"", value: $hasLayout }        
                          ]
                        }
                        first: $pageSize
                        after: $after
                      ) {
                        total
                        pageInfo {
                          endCursor
                          hasNext
                        }
                        results {
                          field(name: ""__updated"") {
                            value
                          }
                          url {
                            path
                          }
                          name
                          ... on _SitemapXmlData {
                            sitemapXml {          
                                value          
                            }        
                          }
                        }
                      }
                    }";

            //public const string GetSitemapXmlData = @"query sitemapQuery(
            //  $itemId: String = ""908351357A914446A08D17436464FFAF""
            //  $language: String = ""en""
            //) {
            //  item(path: $itemId, language: $language) {
            //    displayName 
            //    myfield:field(name: ""myfield"") {
            //      ... on ItemField {
            //        value
            //      }
            //    }
            //     sitemapxml:field(name: ""sitemapxml"") {
            //      ... on ItemField {
            //        value
            //      }
            //    }
            //  }
            //}";

        }
    }
}
