using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Service.Core.Client.Constants;
using Service.Core.Client.Services;
using Service.Education.Helpers;
using Service.Grpc;
using Service.TimeLogger.Grpc.Models;
using Service.TimeLoggerApi.Models;
using Service.UserInfo.Crud.Grpc;

namespace Service.TimeLoggerApi.Controllers
{
	[OpenApiTag("TaskTime", Description = "Task time logger")]
	[Route("/api/v1/time/task-time")]
	public class TaskTimeController : BaseController
	{
		private readonly ISystemClock _systemClock;
		private readonly IEncoderDecoder _encoderDecoder;

		public TaskTimeController(IGrpcServiceProxy<IUserInfoService> userInfoService, ISystemClock systemClock, IEncoderDecoder encoderDecoder) :
			base(userInfoService)
		{
			_systemClock = systemClock;
			_encoderDecoder = encoderDecoder;
		}

		[HttpPost("get")]
		[SwaggerResponse(HttpStatusCode.OK, typeof (DataResponse<int>), Description = "Ok")]
		public async ValueTask<IActionResult> GetTokenAsync(GetTaskTokenRequest request)
		{
			if (EducationHelper.GetTask(request.Tutorial, request.Unit, request.Task) == null)
				return StatusResponse.Error(ResponseCode.NotValidEducationRequestData);

			Guid? userId = await GetUserIdAsync();
			if (userId == null)
				return StatusResponse.Error(ResponseCode.UserNotFound);

			string token = _encoderDecoder.EncodeProto(new TaskTimeLogGrpcRequest
			{
				UserId = userId.Value,
				StartDate = _systemClock.Now,
				Tutorial = request.Tutorial,
				Unit = request.Unit,
				Task = request.Task
			});

			return DataResponse<string>.Ok(token);
		}
	}
}