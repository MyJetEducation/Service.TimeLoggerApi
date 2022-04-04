using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.WalletApi.TimeLoggerApi.Settings
{
	public class SettingsModel
	{
		[YamlProperty("TimeLoggerApi.SeqServiceUrl")]
		public string SeqServiceUrl { get; set; }

		[YamlProperty("TimeLoggerApi.ZipkinUrl")]
		public string ZipkinUrl { get; set; }

		[YamlProperty("TimeLoggerApi.ElkLogs")]
		public LogElkSettings ElkLogs { get; set; }

		[YamlProperty("TimeLoggerApi.EnableApiTrace")]
		public bool EnableApiTrace { get; set; }

		[YamlProperty("TimeLoggerApi.MyNoSqlReaderHostPort")]
		public string MyNoSqlReaderHostPort { get; set; }

		[YamlProperty("TimeLoggerApi.AuthMyNoSqlReaderHostPort")]
		public string AuthMyNoSqlReaderHostPort { get; set; }

		[YamlProperty("TimeLoggerApi.SessionEncryptionKeyId")]
		public string SessionEncryptionKeyId { get; set; }

		[YamlProperty("TimeLoggerApi.MyNoSqlWriterUrl")]
		public string MyNoSqlWriterUrl { get; set; }

		[YamlProperty("TimeLoggerApi.TimeLoggerServiceUrl")]
		public string TimeLoggerServiceUrl { get; set; }

		[YamlProperty("TimeLoggerApi.QueueSendBatchSize")]
		public int QueueSendBatchSize { get; set; }

		[YamlProperty("TimeLoggerApi.QueueCheckIntervalMilliseconds")]
		public int QueueCheckIntervalMilliseconds { get; set; }

		[YamlProperty("TimeLoggerApi.TokenExpireMinutes")]
		public int TokenExpireMinutes { get; set; }
	}
}