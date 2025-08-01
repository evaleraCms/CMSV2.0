using CMS.Core.Enums;

namespace CMS.Api.Models.Interfaces
{
    public interface IBaseResp<T>:IBaseResponse<T>
    {
        string Massage { get; set; }
        ResponseStatus Status { get; set; }
    }
    

    public interface IBaseResponse<T> 
    {
        CMSStatus CMSStatus { get; set; }
        T Result { get; set; }
    }

    public static class BaseRespExtensions
    {
        const string sucessMassage = "Request Processed Successully";
        public static void SetMessage<T>(this IBaseResp<T> resp, string message,ResponseStatus responseStatus = ResponseStatus.Fail)
        {
            resp.Massage = message;
            resp.Status = ResponseStatus.Error;
            resp.SetMessage(ResponseStatus.Error, message);

        }

        public static void SetMessage<T>(this IBaseResp<T> resp, CMSValidationResult validationResult)
        {
            SetMessage(resp, new List<CMSValidationResult> { validationResult });
        }

        public static void SetMessage<T>(this IBaseResp<T> resp, List<CMSValidationResult> validationResults)
        {
            var errors = validationResults.SelectMany(vr => vr.ValidationFailures).SelectMany(q=>
            {
                string prefix = "";
                if((q.PropertyName ?? "") != string.Empty)
                {
                    prefix = $"{q.PropertyName}: ";
                }

                return q.ErrorMassages.Select(s => $"{prefix}{s}");
            }).ToList();
            string message = string.Join(",", errors);
            SetMessage(resp,message);
            BaseRespExtensions.SetMessage(resp, message);
        }

        public static void SetResult<T>(this IBaseResp<T> resp, T result)
        {
            resp.Result = result;
            resp.Status = ResponseStatus.Sussess;
            resp.Massage = sucessMassage;
            resp.SetMessage(ResponseStatus.Sussess,sucessMassage);
        }

        public static void SetSuccessNotResult<T>(this IBaseResp<T> resp)
        {
           
            resp.Status = ResponseStatus.Sussess;
            resp.Massage = sucessMassage;
            resp.SetMessage(ResponseStatus.Sussess, sucessMassage);
        }

        public static void SetMessage<T>(this IBaseResp<T> resp, ResponseStatus responseStatus, string message)
        {
            resp.CMSStatus = new CMSStatus(responseStatus, message);
        }

        public static bool IsSuccess<T>(this IBaseResponse<T> response)
        {
            return response.CMSStatus.CMSStatusCode == ResponseStatus.Sussess;
        }
        public static bool IsFail<T>(this IBaseResponse<T> response)
        {
            return response.CMSStatus.CMSStatusCode == ResponseStatus.Fail;
        }
    }
}