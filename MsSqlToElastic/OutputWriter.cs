using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace MsSqlToElastic
{
    public class OutputWriter
    {
        public OutputWriter(string elasticServerUrl, string index)
        {
            _client = new ElasticClient(new ConnectionSettings(new Uri(elasticServerUrl)));
            _index = index;
        }
        public virtual IBulkResponse BulkUpsert(IEnumerable<object> documents)
        {
            return _client.IndexMany(documents, _index);

        }
        public virtual void RecreateIndex()
        {
            _client.DeleteIndex(_index);
            _client.CreateIndex(_index, c => c.NumberOfReplicas(1).NumberOfShards(1));
        }

        private readonly ElasticClient _client;
        private readonly string _index;
    }
}
