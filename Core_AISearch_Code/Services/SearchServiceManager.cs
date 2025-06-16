using System.Reflection.Metadata;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Core_AISearch_Code.Models;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;

namespace Core_AISearch_Code.Services
{
    public class SearchServiceManager
    {
        AzureKeyCredential _credential;
        SearchIndexClient _indexClient;
        IConfiguration configuration;
        string searchKey, searchEndpoint;
        IConfiguration _configuration;

        public SearchServiceManager(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            searchKey = _configuration["AzureSettings:AzureAISearchKey"] ?? throw new ArgumentNullException("AzureSettings:AzureAISearchKey");
            searchEndpoint = _configuration["AzureSettings:AzureAISearchEndPoint"] ?? throw new ArgumentNullException("AzureSettings:AzureAISearchEndPoint");
            _credential = new AzureKeyCredential(searchKey);

            _indexClient = new SearchIndexClient(new Uri(searchEndpoint), _credential);
        }

        public void CreateIndex()
        {
            var fields = new FieldBuilder().Build(typeof(ShippingIndex));
            var definition = new SearchIndex("shipping-index", fields);
            _indexClient.CreateOrUpdateIndex(definition);

            var connectionString = _configuration["AzureSettings:AzureSQLDbConnectionString"] ?? throw new ArgumentNullException("AzureSettings:AzureSQLDbConnectionString");
            

            var dataSource = new SearchIndexerDataSourceConnection(
                name: "shipping-datasource",
                type: SearchIndexerDataSourceType.AzureSql,
                connectionString: connectionString,
                container: new SearchIndexerDataContainer("OrdersReport")
            );

            var indexerClient = new SearchIndexerClient(new Uri(searchEndpoint), _credential);
            indexerClient.CreateOrUpdateDataSourceConnection(dataSource);

            var indexer = new SearchIndexer(
                name: "sample-indexer",
                dataSourceName: dataSource.Name,
                targetIndexName: "shipping-index"
            );

            indexer.Parameters = new IndexingParameters
            {
                BatchSize = 1,
                MaxFailedItems = -1,
                MaxFailedItemsPerBatch = -1
            };

            indexerClient.CreateOrUpdateIndexer(indexer);
        }

        public SearchResults<SearchDocument> Search(string searchText)
        {
            var client = new SearchClient(new Uri(searchEndpoint), "shipping-index", _credential);
            var options = new SearchOptions
            {
                Size = 10
            };
            return client.Search<SearchDocument>(searchText, options);
        }
    }
}