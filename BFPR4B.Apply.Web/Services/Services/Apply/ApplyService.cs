using BFPR4B.Apply.Web.Services.IServices.Apply;
using BFPR4B.Apply.Web.Services.IServices.Gentable;
using BFPR4B.Apply.Web.Services.Services.Base;

namespace BFPR4B.Apply.Web.Services.Services.Apply
{
    public class ApplyService : BaseService, IApplyService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string _apiURL;

        public ApplyService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            _apiURL = configuration.GetValue<string>("ServiceUrls:BFPR4B.Apply.API");
        }




    }
}
