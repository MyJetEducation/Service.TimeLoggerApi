using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Service.Core.Client.Services;
using Service.TimeLogger.Grpc.Models;
using Service.TimeLoggerApi.Models;
using Service.UserInfo.Crud.Grpc;

namespace Service.TimeLoggerApi.Controllers
{
	[OpenApiTag("TaskTime", Description = "Task time logger")]
	[Route("/api/v1/task-time")]
	public class TaskTimeController : BaseController
	{
		public TaskTimeController(IUserInfoService userInfoService, IEncoderDecoder encoderDecoder, ISystemClock systemClock) :
			base(userInfoService, encoderDecoder, systemClock)
		{
		}

		[HttpPost("get")]
		[SwaggerResponse(HttpStatusCode.OK, typeof (DataResponse<int>), Description = "Ok")]
		public async ValueTask<IActionResult> GetTokenAsync(GetTaskTokenRequest request) => await GenerateTokenAsync((userId, nowDate) => new TaskTimeLogGrpcRequest
		{
			UserId = userId,
			StartDate = nowDate,
			Tutorial = request.Tutorial,
			Unit = request.Unit,
			Task = request.Task
		});
	}
}