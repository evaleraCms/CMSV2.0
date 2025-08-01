namespace CMS.Api.Models.Interfaces
{
    public interface IProcessBatch<TResult,TBatchData>
    {
        TResult ResultRecord { get; set; }
        TBatchData BatchData { get; set; }
    }
}
