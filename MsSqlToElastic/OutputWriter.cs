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
        public OutputWriter()
        {

        }

        public virtual IBulkResponse write(BulkOutputRequest request)
        {
            if(request.hasIdFieldName)
            {
                return BulkUpsert(request);
            }
            return BulkInsert(request);
        }
        public virtual void ResetIndex(BulkOutputRequest request)
        {
            request.client.DeleteIndex(request.index);
            request.client.CreateIndex(request.index, c => c.NumberOfReplicas(1).NumberOfShards(1));
        }
        private  IBulkResponse BulkUpsert(BulkOutputRequest request)
        {
            var descriptor = new BulkDescriptor();
            
            foreach(var doc in request.documents)
            {
                descriptor.Index<object>(op => op.Index(request.index)
                                                    .Type(request.type)
                                                    .Object(doc)
                                                    .Id(((IDictionary<string,object>)doc)[request.idFieldName].ToString()));
            }
            return request.client.Bulk(descriptor);
        }
        private IBulkResponse BulkInsert(BulkOutputRequest request)
        {
            return request.client.IndexMany(request.documents, request.index, request.type);
        }
   
    }

    public class BulkOutputRequest
    {
        public ElasticClient client { get; set; }
        public IEnumerable<object> documents { get; set; }
        public bool hasIdFieldName
        {
            get
            {
                return !idFieldName.isNullEmptyOrSpaces();
            }
        }
        public string index { get; set; }
        public string idFieldName { get; set; }
        public string type { get; set; }

        public static BulkOutputRequest create(Command command)
        {
            return new BulkOutputRequest()
            {
                client = new ElasticClient(new ConnectionSettings(new Uri(command.elasticUrl))),
                idFieldName = command.id,
                index = command.index,
                type = command.type
            };
        }
    }
}
