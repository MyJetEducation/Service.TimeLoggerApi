using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service.Tools;
using NSwag.Annotations;
using Service.Core.Client.Services;
using Service.TimeLogger.Grpc;
using Service.TimeLogger.Grpc.Models;
using Service.TimeLoggerApi.Models;
using Service.UserInfo.Crud.Grpc;

namespace Service.TimeLoggerApi.Controllers
{
	[OpenApiTag("UserTime", Description = "User time logger")]
	[Route("/api/v1/user-time")]
	public class UserTimeController : BaseController
	{
		private readonly ITimeLoggerService _timeLoggerService;
		private readonly ILogger<UserTimeController> _logger;
		private readonly MyTaskTimer _timer;

		private static readonly BlockingCollection<TimeLogGrpcRequest> RequestQueue;

		static UserTimeController() => RequestQueue = new BlockingCollection<TimeLogGrpcRequest>();

		public UserTimeController(IUserInfoService userInfoService,
			ITimeLoggerService timeLoggerService,
			IEncoderDecoder encoderDecoder,
			ISystemClock systemClock,
			ILogger<UserTimeController> logger) : base(userInfoService, encoderDecoder, systemClock)
		{
			_timeLoggerService = timeLoggerService;
			_logger = logger;

			_timer = new MyTaskTimer(typeof (UserTimeController), GetDuration(), logger, TimerAction);
			_timer.Start();
		}

		[HttpPost("get")]
		[SwaggerResponse(HttpStatusCode.OK, typeof (DataResponse<int>), Description = "Ok")]
		public async ValueTask<IActionResult> GetTokenAsync() => await GenerateTokenAsync((userId, nowDate) => new TimeLogGrpcRequest
		{
			UserId = userId,
			StartDate = nowDate
		});

		[HttpPost("log")]
		[SwaggerResponse(HttpStatusCode.OK, typeof (DataResponse<int>), Description = "Ok")]
		public IActionResult LogTime([FromBody, Required] string token)
		{
			TimeLogGrpcRequest request = EncoderDecoder.DecodeProto<TimeLogGrpcRequest>(token);
			if (request == null)
				return StatusResponse.Error();

			RequestQueue.Add(request);

			return StatusResponse.Ok();
		}

		private Task TimerAction()
		{
			if (RequestQueue.Any())
			{
				_logger.LogDebug("Queue has items, processing...");

				int batchSize = Program.ReloadedSettings(model => model.QueueSendBatchSize).Invoke();

				var requestsToSend = new List<TimeLogGrpcRequest>(batchSize);

				while (RequestQueue.TryTake(out TimeLogGrpcRequest request))
				{
					requestsToSend.Add(request);
					if (requestsToSend.Count == batchSize)
						break;
				}

				if (requestsToSend.Any())
				{
					_logger.LogDebug("Sending batch of {count} TimeLogGrpcRequest items to TimeLoggerService...", requestsToSend.Count);

					_timeLoggerService.ProcessRequests(requestsToSend.ToArray());
				}
			}

			_timer.ChangeInterval(GetDuration());

			return Task.CompletedTask;
		}

		private static TimeSpan GetDuration() => TimeSpan.FromMilliseconds(Program.ReloadedSettings(model => model.QueueCheckIntervalMilliseconds).Invoke());
	}
}