namespace FamilyBudgeter.API.BLL.DTOs.FamilyDTOs
{
    public class FamilyDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? JoinCode { get; set; }
        public List<FamilyMemberDto> Members { get; set; } = new List<FamilyMemberDto>();
    }
}
