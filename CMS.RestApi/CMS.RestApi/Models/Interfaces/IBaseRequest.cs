namespace CMS.Api.Models.Interfaces
{
    public interface IBaseRequest { 
    
        Guid Token { get; set; }
    }
    public interface IBaseRequest<T>: IBaseRequest
    {
        T Request { get; set; }
    }
   
}
