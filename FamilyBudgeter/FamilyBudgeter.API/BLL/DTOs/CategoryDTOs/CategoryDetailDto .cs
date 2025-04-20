using FamilyBudgeter.API.BLL.DTOs.BudgetLimitDTOs;
using FamilyBudgeter.API.BLL.DTOs.TransactionDTOs;

namespace FamilyBudgeter.API.BLL.DTOs.CategoryDTOs
{
    public class CategoryDetailDto : CategoryDto
    {
        public List<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();
        public List<BudgetLimitDto> Limits { get; set; } = new List<BudgetLimitDto>();
        public decimal CurrentAmount { get; set; }
        public decimal? BudgetLimit { get; set; }
        public decimal? PercentOfLimit { get; set; }
    }
}
