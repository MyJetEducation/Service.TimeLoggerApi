using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Service.Education.Structure;

namespace Service.TimeLoggerApi.Models
{
	public class GetTaskTokenRequest
	{
		[Required]
		[Range(1, 9)]
		[DefaultValue(EducationTutorial.PersonalFinance)]
		[EnumDataType(typeof(EducationTutorial))]
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