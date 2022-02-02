using Autofac;
using Service.Core.Client.Services;
using Service.TimeLogger.Client;
using Service.UserInfo.Crud.Client;

namespace Service.TimeLoggerApi.Modules
{
	public class ServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterTimeLoggerClient(Program.Settings.TimeLoggerServiceUrl);
			builder.RegisterType<SystemClock>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterUserInfoCrudClient(Program.Settings.UserInfoCrudServiceUrl);

			builder.Register(context => new EncoderDecoder(Program.EncodingKey))
				.As<IEncoderDecoder>()
				.SingleInstance();
		}
	}
}