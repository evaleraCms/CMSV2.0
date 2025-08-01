using CMS.Api.Models.Interfaces;

namespace CMS.Api.Models
{
    public class GetProcessBatchResponse<TResultItem>:BaseBatch, IGetProcessBatchResponse<TResultItem>
    {
        public TResultItem ResultItem { get; set; } 
        
    }
    
}
