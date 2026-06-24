using GymManagementSystem.BLL.Services.AttachmentService;
using GymManagementSystem.BLL.Services.Classes;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.MemberViewModel;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagementSystem.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IAttachmentService _attachmentService;

        public MembersController(IMemberService memberService, IAttachmentService attachmentService)
        {
            _memberService = memberService;
            _attachmentService = attachmentService;
        }

        #region Base Actions
        // Get BaseUrl/Members/Index
        // Index - List All Members
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberService.GetAllMembersAsync(ct: ct);
            return View(members);
        }

        // Get BaseUrl/Members/MemberDetails/{Id}
        // MemberDetails - Displays one Member Profile Details
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberDetailsAsync(id, ct);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // Get BaseUrl/Members/HealthRecordDetails/{Id}
        // HealthRecordDetails - Display one Health Record Details
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var healthRecord = await _memberService.GetMemberHealthRecordAsync(id, ct);

            if (healthRecord is null)
            {
                TempData["ErrorMessage"] = "HealthRecord Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(healthRecord);
        } 
        #endregion

        #region Create Member
        // Get BaseUrl/Members/Create
        // Create - Displays Create Form
        public IActionResult Create() 
        {
            return View();
        }

        // Post BaseUrl/Members/Create Member 
        // Create Submit Crate Form
        [HttpPost]
        public async Task<IActionResult> Create(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _memberService.CreateMembersAsync(model, ct: ct);
            if(result)
            {
                TempData["SuccessMessage"] = "Member Created Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Create Member";
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Edit IMemebr
        // Get BaseUrl/Members/Edit/{Id}
        // Edit - Displays Edit Form
        public async Task<IActionResult> EditMember(int id, CancellationToken ct) 
        {
            var member = await _memberService.GetMemberToUpdateAsync(id, ct: ct);

            if (member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // Post BaseUrl/Members/Edit Member 
        // Edit - Submit Edit Form
        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute]int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            if(!ModelState.IsValid) 
                return View(model);

            var result = await _memberService.UpdateMemberDetailsAsync(id, model, ct: ct);

            if (result == false)
            {
                TempData["ErrorMessage"] = "Failed To Update Member";
                return View(model);
            }

            TempData["SuccessMessage"] = "Member Updaed Successfully";
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Delete Member
        // Get BaseUrl/Members/Delete/{Id} 
        // Delete - Display Delete Form
        public async Task<ActionResult> Delete(int id, CancellationToken ct)
        { 
            var member = await _memberService.GetMemberDetailsAsync(id, ct: ct);

            if (member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // Post BaseUrl/Members/DeleteConfirmation/{Id}
        // DeleteConfirmation - Submit Delete Form
        [HttpPost]
        public async Task<ActionResult> DeleteConfirmed(int id, CancellationToken ct)
        { 
            var result = await _memberService.RemoveMemberAsync(id, ct: ct);
            if (result)
                TempData["SuccessMessage"] = "Member Deleted Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Delete Member";

            return RedirectToAction(nameof(Index)); 
        }
        #endregion

        public async Task<IActionResult> Picture(int id)
        {
            var member = await _memberService.GetMemberDetailsAsync(id);
            if (member is null || string.IsNullOrEmpty(member.Photo))
                return NotFound();

            var result = _attachmentService.GetFile(member.Photo, "MembersPctures");
            if(result is null)
                return NotFound();

            return File(result.Value.Stream, result.Value.CntentType);
        }
    }
}
