using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyJetWallet.Sdk.Authorization.Http;
using NSwag.Annotations;
using Service.Core.Client.Services;
using Service.Education.Helpers;
using Service.TimeLogger.Grpc.Models;
using Service.WalletApi.TimeLoggerApi.Controllers.Contracts;
using Service.Web;

namespace Service.WalletApi.TimeLoggerApi.Controllers
{
	[Authorize]
	[ApiController]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[SwaggerResponse(HttpStatusCode.Unauthorized, null, Description = "Unauthorized")]
	[OpenApiTag("TaskTime", Description = "Task time logger")]
	[Route("/api/v1/time/task-time")]
	public class TaskTimeController : ControllerBase
	{
		private readonly ISystemClock _systemClock;
		private readonly IEncoderDecoder _encoderDecoder;

		public TaskTimeController(ISystemClock systemClock, IEncoderDecoder encoderDecoder)
		{
			_systemClock = systemClock;
			_encoderDecoder = encoderDecoder;
		}

		[HttpPost("get")]
		[SwaggerResponse(HttpStatusCode.OK, typeof (DataResponse<int>), Description = "Ok")]
		public IActionResult GetToken(GetTaskTokenRequest request)
		{
			if (EducationHelper.GetTask(request.Tutorial, request.Unit, request.Task) == null)
				return StatusResponse.Error(ResponseCode.NotValidEducationRequestData);

			string userId = this.GetClientId();
			if (userId == null)
				return StatusResponse.Error(ResponseCode.UserNotFound);

			string token = _encoderDecoder.EncodeProto(new TaskTimeLogGrpcRequest
			{
				UserId = userId,
				StartDate = _systemClock.Now,
				Tutorial = request.Tutorial,
				Unit = request.Unit,
				Task = request.Task
			});

			return DataResponse<string>.Ok(token);
		}
	}
}