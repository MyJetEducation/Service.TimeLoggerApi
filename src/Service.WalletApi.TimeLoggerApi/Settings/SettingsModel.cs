using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.WalletApi.TimeLoggerApi.Settings
{
	public class SettingsModel
	{
		[YamlProperty("WalletApiEducation.SeqServiceUrl")]
		public string SeqServiceUrl { get; set; }

		[YamlProperty("WalletApiEducation.ZipkinUrl")]
		public string ZipkinUrl { get; set; }

		[YamlProperty("WalletApiEducation.ElkLogs")]
		public LogElkSettings ElkLogs { get; set; }

		[YamlProperty("WalletApiEducation.EnableApiTrace")]
		public bool EnableApiTrace { get; set; }

		[YamlProperty("WalletApiEducation.MyNoSqlReaderHostPort")]
		public string MyNoSqlReaderHostPort { get; set; }

		[YamlProperty("WalletApiEducation.AuthMyNoSqlReaderHostPort")]
		public string AuthMyNoSqlReaderHostPort { get; set; }

		[YamlProperty("WalletApiEducation.SessionEncryptionKeyId")]
		public string SessionEncryptionKeyId { get; set; }

		[YamlProperty("WalletApiEducation.MyNoSqlWriterUrl")]
		public string MyNoSqlWriterUrl { get; set; }

		[YamlProperty("WalletApiEducation.TimeLoggerServiceUrl")]
		public string TimeLoggerServiceUrl { get; set; }

		[YamlProperty("WalletApiEducation.TimeLoggerQueueSendBatchSize")]
		public int TimeLoggerQueueSendBatchSize { get; set; }

		[YamlProperty("WalletApiEducation.TimeLoggerQueueCheckIntervalMilliseconds")]
		public int TimeLoggerQueueCheckIntervalMilliseconds { get; set; }

		[YamlProperty("WalletApiEducation.TimeLoggerTokenExpireMinutes")]
		public int TimeLoggerTokenExpireMinutes { get; set; }
	}
}