using CMS.Api.Models.Interfaces;
using JetBrains.Annotations;
using System.Diagnostics.Eventing.Reader;


namespace CMS.Api.Models
{
    public class BaseGetManyApis
    {
        public abstract class BaseGetManyApi<TRequest, TRequestItem, TResponse, TResult, TResultItem> : BaseBatch
        where TRequest : IBaseRequest<IEnumerable<TRequestItem>>
        where TResponse : IBaseResp<IEnumerable<TResultItem>>, new()
        where TResult : IBaseResp<TResultItem>,new()
        {
            protected BaseGetManyApi()
            {
                
            }

            protected abstract GetProcessBatchResponse<ICollection<TResultItem>> GetProcessRecords(TRequestItem requestItem);
            protected virtual (bool edited,TResult result) OnProcessBatchError(ICollection<TResultItem> resultItem)
            {

                return (false, new TResult());
            }

            [NotNull]
            public TResponse Process([NotNull] TRequest request)
            { 
                var result = new List<TResult>();
                foreach (var r in request.Request)
                {
                    try
                    {
                        var processRecord = GetProcessRecords(r);
                        if (processRecord.IsValid)
                        {
                            foreach (var resultItem in processRecord.ResultItem)
                            {
                                var record = new TResult();
                                record.SetResult(resultItem);
                                result.Add(record);
                            }

                        }
                        else { 
                            var onProcessRecord = OnProcessBatchError(processRecord.ResultItem);
                            var record = new TResult();
                            if (onProcessRecord.edited) { 
                                record = onProcessRecord.result;
                            }
                            record.SetMessage(processRecord.GetMessages());
                            result.Add(record);
                        }
                    }
                    catch (Exception ex)
                    {
                       
                        List<string> messages = new List<string>();
                        Exception exception = ex;
                        var record = new TResult();
                        while (exception != null) { 
                            messages.Add(exception.Message);
                            exception = exception.InnerException;
                        }
                        record.SetMessage(string.Join(", ", messages.ToArray()));
                        result.Add(record);
                    }
                }

                var resp = new TResponse();
       
                resp.SetResult(result.Cast<TResultItem>().ToList()); // Explicitly cast the result to match the expected type
                if (result.Any(x => x.Status == CMS.Core.Enums.ResponseStatus.Fail ||
                                    x.Status == CMS.Core.Enums.ResponseStatus.Error))
                {
                    resp.SetMessage("Response contains failures.");
                    resp.CMSStatus.CMSStatusCode = CMS.Core.Enums.ResponseStatus.Error;
                }

                return resp; // Ensure the method returns the response
            }
        }
    }
}
