using System.Threading.Tasks;

namespace Cuscino
{
    public interface ICouchClientAsync
    {
        Task<CouchRequestResult> CreateDatabaseIfNotExistsAsync();

        Task<CouchRequestResult> DeleteDatabaseIfExistsAsync();

        Task<CouchRequestResult> PostDocumentAsync(string content);

        Task<CouchRequestResult> DeleteDocumentAsync(string docid, string revision);

        Task<CouchRequestResult> PostEntityAsync<T>(T entity) where T : CouchDoc;

        Task<T> GetDocumentAsEntityAsync<T>(string docid) where T : CouchDoc;

        Task<CouchViewResult<T>> QueryViewAsEntityAsync<T>(string viewPath);

        Task<CouchViewResult<T>> QueryViewAsEntityAsync<T>(string viewPath, QueryOptions queryOptions);
    }
}