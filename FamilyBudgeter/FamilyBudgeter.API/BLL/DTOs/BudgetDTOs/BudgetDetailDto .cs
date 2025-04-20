using FamilyBudgeter.API.BLL.DTOs.BudgetLimitDTOs;
using FamilyBudgeter.API.BLL.DTOs.CategoryDTOs;
using FamilyBudgeter.API.BLL.DTOs.FinancialGoalDTOs;

namespace FamilyBudgeter.API.BLL.DTOs.BudgetDTOs
{
    public class BudgetDetailDto : BudgetDto
    {
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public List<FinancialGoalDto> FinancialGoals { get; set; } = new List<FinancialGoalDto>();
        public List<BudgetLimitDto> Limits { get; set; } = new List<BudgetLimitDto>();
    }
}
