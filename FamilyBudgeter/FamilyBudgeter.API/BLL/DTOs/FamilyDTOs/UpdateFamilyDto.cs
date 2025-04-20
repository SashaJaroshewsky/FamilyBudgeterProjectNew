using System.ComponentModel.DataAnnotations;

namespace FamilyBudgeter.API.BLL.DTOs.FamilyDTOs
{
    public class UpdateFamilyDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public required string Name { get; set; }
    }
}
