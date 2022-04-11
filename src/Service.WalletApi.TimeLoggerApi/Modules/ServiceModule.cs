using Autofac;
using MyJetWallet.ApiSecurityManager.Autofac;
using MyJetWallet.Sdk.RestApiTrace;
using Service.Core.Client.Services;
using Service.TimeLogger.Client;

namespace Service.WalletApi.TimeLoggerApi.Modules
{
	public class ServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterEncryptionServiceClient();

			builder.RegisterTimeLoggerClient(Program.Settings.TimeLoggerServiceUrl);
			builder.RegisterType<SystemClock>().AsImplementedInterfaces().SingleInstance();

			builder.Register(context => new EncoderDecoder(Program.EncodingKey))
				.As<IEncoderDecoder>()
				.SingleInstance();

			if (Program.Settings.EnableApiTrace)
			{
				builder
					.RegisterInstance(new ApiTraceManager(Program.Settings.ElkLogs, "api-trace",
						Program.LoggerFactory.CreateLogger("ApiTraceManager")))
					.As<IApiTraceManager>()
					.As<IStartable>()
					.AutoActivate()
					.SingleInstance();
			}
		}
	}
}