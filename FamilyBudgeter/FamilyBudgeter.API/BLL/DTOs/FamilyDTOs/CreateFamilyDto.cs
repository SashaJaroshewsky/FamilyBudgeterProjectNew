using System.ComponentModel.DataAnnotations;

namespace FamilyBudgeter.API.BLL.DTOs.FamilyDTOs
{
    public class CreateFamilyDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public required string Name { get; set; }
    }
}
