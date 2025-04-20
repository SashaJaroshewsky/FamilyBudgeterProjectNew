using FamilyBudgeter.API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FamilyBudgeter.API.BLL.DTOs.CategoryDTOs
{
    public class UpdateCategoryDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public required string Name { get; set; }

        public string? Icon { get; set; }

        [Required]
        public CategoryType Type { get; set; }
    }
}
