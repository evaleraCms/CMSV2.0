namespace CMS.Api.Models.Interfaces
{
    public interface IGetProcessBatchResponse<TResultItem>
    {
        TResultItem ResultItem { get; set; }
    }
}
