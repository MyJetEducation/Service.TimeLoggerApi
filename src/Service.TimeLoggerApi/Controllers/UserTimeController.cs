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
using Service.Core.Client.Constants;
using Service.Core.Client.Services;
using Service.TimeLogger.Grpc;
using Service.TimeLogger.Grpc.Models;
using Service.TimeLoggerApi.Constants;
using Service.TimeLoggerApi.Models;
using Service.TimeLoggerApi.Settings;
using Service.UserInfo.Crud.Grpc;

namespace Service.TimeLoggerApi.Controllers
{
	[OpenApiTag("UserTime", Description = "User time logger")]
	[Route("/api/v1/time/user-time")]
	public class UserTimeController : BaseController
	{
		private readonly ITimeLoggerService _timeLoggerService;
		private readonly ILogger<UserTimeController> _logger;
		private readonly ISystemClock _systemClock;
		private readonly IEncoderDecoder _encoderDecoder;

		private static readonly ConcurrentDictionary<string, DateTime> TokenLifetimeDictionary;
		private static readonly BlockingCollection<TimeLogGrpcRequest> RequestQueue;
		private readonly MyTaskTimer _timer;
		private int _batchSize;
		private int _tokenExpire;
		private TimeSpan _interval;

		static UserTimeController()
		{
			RequestQueue = new BlockingCollection<TimeLogGrpcRequest>();
			TokenLifetimeDictionary = new ConcurrentDictionary<string, DateTime>();
		}

		public UserTimeController(IUserInfoService userInfoService,
			ITimeLoggerService timeLoggerService,
			IEncoderDecoder encoderDecoder,
			ISystemClock systemClock,
			ILogger<UserTimeController> logger) : base(userInfoService)
		{
			_timeLoggerService = timeLoggerService;
			_encoderDecoder = encoderDecoder;
			_systemClock = systemClock;
			_logger = logger;

			ReadSettings();

			_timer = new MyTaskTimer(typeof (UserTimeController), _interval, logger, ProcessQueue);
			_timer.Start();
		}

		[HttpPost("get")]
		[SwaggerResponse(HttpStatusCode.OK, typeof (DataResponse<int>), Description = "Ok")]
		public async ValueTask<IActionResult> GetTokenAsync()
		{
			Guid? userId = await GetUserIdAsync();
			if (userId == null)
				return StatusResponse.Error(ResponseCode.UserNotFound);

			DateTime now = _systemClock.Now;

			string token = _encoderDecoder.EncodeProto(new TimeLogGrpcRequest
			{
				UserId = userId.Value,
				StartDate = now
			});

			TokenLifetimeDictionary.TryAdd(token, now);

			return DataResponse<string>.Ok(token);
		}

		[HttpPost("log")]
		[SwaggerResponse(HttpStatusCode.OK, typeof (DataResponse<int>), Description = "Ok")]
		public IActionResult LogTime([FromBody, Required] string token)
		{
			var request = _encoderDecoder.DecodeProto<TimeLogGrpcRequest>(token);
			if (request == null)
				return StatusResponse.Error(TimeLoggerResponseCode.InvalidTimeToken);

			DateTime now = _systemClock.Now;

			if (!TokenLifetimeDictionary.TryGetValue(token, out DateTime lastRecieved) || now.Subtract(lastRecieved).Minutes >= _tokenExpire)
			{
				TokenLifetimeDictionary.Remove(token, out _);
				return StatusResponse.Error(TimeLoggerResponseCode.TimeTokenExpired);
			}

			TokenLifetimeDictionary[token] = now;
			RequestQueue.Add(request);

			return StatusResponse.Ok();
		}

		private Task ProcessQueue()
		{
			if (RequestQueue.Count >= _batchSize)
			{
				_logger.LogDebug("Queue has items, processing...");

				var requestsToSend = new List<TimeLogGrpcRequest>(_batchSize);

				while (RequestQueue.TryTake(out TimeLogGrpcRequest request))
				{
					requestsToSend.Add(request);
					if (requestsToSend.Count == _batchSize)
						break;
				}

				if (requestsToSend.Any())
				{
					try
					{
						_logger.LogDebug("Sending batch of {count} TimeLogGrpcRequest items to TimeLoggerService...", requestsToSend.Count);

						_timeLoggerService.ProcessRequests(requestsToSend.ToArray());
					}
					catch (Exception exception)
					{
						_logger.LogError("Error while sending batch of TimeLogGrpcRequest items to TimeLoggerService, message: {message}", exception.Message);
					}
				}

				ReadSettings();
			}

			_timer.ChangeInterval(_interval);

			return Task.CompletedTask;
		}

		private void ReadSettings()
		{
			SettingsModel settings = Program.GetSettings();
			_interval = TimeSpan.FromMilliseconds(settings.QueueCheckIntervalMilliseconds);
			_batchSize = settings.QueueSendBatchSize;
			_tokenExpire = settings.TokenExpireMinutes;
		}
	}
}