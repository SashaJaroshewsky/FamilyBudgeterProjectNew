using System.ComponentModel.DataAnnotations;

namespace FamilyBudgeter.API.BLL.DTOs.FamilyDTOs
{
    public class JoinFamilyDto
    {
        [Required]
        public required string JoinCode { get; set; }
    }
}
