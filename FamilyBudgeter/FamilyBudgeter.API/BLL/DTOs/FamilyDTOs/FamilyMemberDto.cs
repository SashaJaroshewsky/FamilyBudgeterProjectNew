using FamilyBudgeter.API.Domain.Enums;

namespace FamilyBudgeter.API.BLL.DTOs.FamilyDTOs
{
    public class FamilyMemberDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string UserName { get; set; }
        public required string UserEmail { get; set; }
        public FamilyRole Role { get; set; }
    }
}
