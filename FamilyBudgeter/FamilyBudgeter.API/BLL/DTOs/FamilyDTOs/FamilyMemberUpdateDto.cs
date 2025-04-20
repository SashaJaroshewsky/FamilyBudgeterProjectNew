using FamilyBudgeter.API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FamilyBudgeter.API.BLL.DTOs.FamilyDTOs
{
    public class FamilyMemberUpdateDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public FamilyRole Role { get; set; }
    }
}
