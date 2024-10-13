using AutoMapper.Internal;
using BFPR4B.Apply.Web.Models.System;

namespace BFPR4B.Apply.Web.Services.IServices.Base
{
	public interface IBaseService
	{
		APIResponse responseModel { get; set; }
		Task<T> SendAsync<T>(APIRequest apiRequest);
	}
}
