using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Service.Core.Client.Education;

namespace Service.TimeLoggerApi.Models
{
	public class GetTaskTokenRequest
	{
		[Required]
		[Range(1, 9)]
		[DefaultValue(EducationTutorial.PersonalFinance)]
		public EducationTutorial Tutorial { get; set; }

		[Required]
		[Range(1, 5)]
		[DefaultValue(1)]
		public int Unit { get; set; }

		[Required]
		[Range(1, 6)]
		[DefaultValue(1)]
		public int Task { get; set; }
	}
}