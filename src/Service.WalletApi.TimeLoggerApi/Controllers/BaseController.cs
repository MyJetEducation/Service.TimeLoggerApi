using System;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyJetWallet.Sdk.Authorization.Http;
using NSwag.Annotations;
using Service.Core.Client.Extensions;

namespace Service.WalletApi.TimeLoggerApi.Controllers
{
	[Authorize]
	[ApiController]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[SwaggerResponse(HttpStatusCode.Unauthorized, null, Description = "Unauthorized")]
	public abstract class BaseController : ControllerBase
	{
		protected Guid? GetUserId()
		{
			string clientId = this.GetClientId();
			if (clientId.IsNullOrWhiteSpace())
				return null;

			return Guid.TryParse(clientId, out Guid uid) ? (Guid?) uid : null;
		}
	}
}