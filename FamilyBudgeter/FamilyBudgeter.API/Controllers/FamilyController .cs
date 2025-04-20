using FamilyBudgeter.API.BLL.DTOs.FamilyDTOs;
using FamilyBudgeter.API.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyBudgeter.API.Controllers
{
    [Authorize]
    public class FamilyController : BaseApiController
    {
        private readonly IFamilyService _familyService;

        public FamilyController(IFamilyService familyService)
        {
            _familyService = familyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyFamilies()
        {
            try
            {
                var userId = GetCurrentUserId();
                var families = await _familyService.GetUserFamiliesAsync(userId);
                return Ok(families);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFamily(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var family = await _familyService.GetFamilyByIdAsync(id, userId);
                return Ok(family);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFamily([FromBody] CreateFamilyDto familyDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var family = await _familyService.CreateFamilyAsync(familyDto, userId);
                return CreatedAtAction(nameof(GetFamily), new { id = family.Id }, family);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFamily(int id, [FromBody] UpdateFamilyDto familyDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var family = await _familyService.UpdateFamilyAsync(id, familyDto, userId);
                return Ok(family);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFamily(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _familyService.DeleteFamilyAsync(id, userId);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("{id}/regenerate-join-code")]
        public async Task<IActionResult> RegenerateJoinCode(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var newCode = await _familyService.RegenerateJoinCodeAsync(id, userId);
                return Ok(new { joinCode = newCode });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinFamily([FromBody] JoinFamilyDto joinDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var family = await _familyService.JoinFamilyByCodeAsync(joinDto.JoinCode, userId);
                return Ok(family);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}/members")]
        public async Task<IActionResult> GetFamilyMembers(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var members = await _familyService.GetFamilyMembersAsync(id, userId);
                return Ok(members);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id}/members/{memberUserId}")]
        public async Task<IActionResult> UpdateMemberRole(int id, int memberUserId, [FromBody] FamilyMemberUpdateDto memberDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                memberDto.UserId = memberUserId;
                var member = await _familyService.UpdateMemberRoleAsync(id, memberDto, userId);
                return Ok(member);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}/members/{memberUserId}")]
        public async Task<IActionResult> RemoveFamilyMember(int id, int memberUserId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _familyService.RemoveFamilyMemberAsync(id, memberUserId, userId);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("{id}/leave")]
        public async Task<IActionResult> LeaveFamily(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _familyService.LeaveFamilyAsync(id, userId);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
