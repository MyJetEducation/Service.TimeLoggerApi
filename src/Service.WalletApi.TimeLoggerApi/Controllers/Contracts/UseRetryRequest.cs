using System.ComponentModel.DataAnnotations;
using Service.Education.Structure;

namespace Service.WalletApi.TimeLoggerApi.Controllers.Contracts
{
	public class UseRetryRequest
	{
		[Required]
		public EducationTutorial Tutorial { get; set; }

		[Required]
		public int Unit { get; set; }

		[Required]
		public int Task { get; set; }
	}
}