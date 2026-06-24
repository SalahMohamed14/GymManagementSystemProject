using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymManagementSystem.PL.Controllers
{
    [Authorize]
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var sessions = await _sessionService.GetAllSessionsAsync(ct);
            return View(sessions);
        }

        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateDropDownAysnc(ct);
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownAysnc(ct);
                return View(model);
            }

            var result = await _sessionService.CreateSessionAsync(model, ct);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Session Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.Error;
            await PopulateDropDownAysnc(ct);
            return View(model);
        }

        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var session = await _sessionService.GetSessionByIdAsync(id, ct);
            if(session is  null)
            {
                TempData["ErrorMessage"] = "Session Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var session = await _sessionService.GetSessionToUpdateAsync(id, ct);
            if (session is null) 
            {
                TempData["ErrorMessage"] = "Session Cannot be Edited or Not Found";
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropDownAysnc(ct);
            return View(session);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id,UpdateSessionViewModel model, CancellationToken ct)
        {
            if(!ModelState.IsValid)
            {
                await PopulateDropDownAysnc(ct);
                return View(model);
            }
            var result = await _sessionService.UpdateSessionAsync(id, model, ct);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Session Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.Error;
            await PopulateDropDownAysnc(ct);
            return View(model);
        }

        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        { 
            var session = await _sessionService.GetSessionByIdAsync(id, ct);
            if (session is null) 
            {
                TempData["ErrorMessage"] = "Session Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await _sessionService.RemoveSessionAsync(id, ct);
            if (result.Success)
                TempData["SuccessMessage"] = "Session Deleted Successfully";
            else
                TempData["ErrorMessage"] = result.Error;

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropDownAysnc(CancellationToken ct)
        {
            ViewBag.Trainers = new SelectList(await _sessionService.GetAllTrainersAsync(ct),"Id", "Name");
            ViewBag.categories = new SelectList(await _sessionService.GetAllCategoriesAsync(ct), "Id", "CategoryName");
        }
    }
}
