using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Service.Core.Client.Constants;
using Service.Core.Client.Services;
using Service.TimeLoggerApi.Models;
using Service.UserInfo.Crud.Grpc;
using Service.UserInfo.Crud.Grpc.Models;

namespace Service.TimeLoggerApi.Controllers
{
	[Authorize]
	[ApiController]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[SwaggerResponse(HttpStatusCode.Unauthorized, null, Description = "Unauthorized")]
	public abstract class BaseController : ControllerBase
	{
		private readonly IUserInfoService _userInfoService;
		protected readonly IEncoderDecoder EncoderDecoder;
		private readonly ISystemClock _systemClock;

		protected BaseController(IUserInfoService userInfoService, IEncoderDecoder encoderDecoder, ISystemClock systemClock)
		{
			_userInfoService = userInfoService;
			_systemClock = systemClock;
			EncoderDecoder = encoderDecoder;
		}

		private async ValueTask<Guid?> GetUserIdAsync()
		{
			UserInfoResponse userInfoResponse = await _userInfoService.GetUserInfoByLoginAsync(new UserInfoAuthRequest
			{
				UserName = User.Identity?.Name
			});

			return userInfoResponse?.UserInfo?.UserId;
		}

		protected async ValueTask<IActionResult> GenerateTokenAsync(Func<Guid, DateTime, object> grpcFunc)
		{
			Guid? userId = await GetUserIdAsync();
			if (userId == null)
				return StatusResponse.Error(ResponseCode.UserNotFound);

			object grpcObject = grpcFunc.Invoke(userId.Value, _systemClock.Now);

			string token = EncoderDecoder.EncodeProto(grpcObject);

			return DataResponse<string>.Ok(token);
		}
	}
}