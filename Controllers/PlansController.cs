using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.PlanViewModels;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Classes;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystemProject.Controllers
{
    public class PlansController : Controller
    {
        //private readonly GymDbContext _dbContext;
        //private readonly IGenericRepository<Plan> _planRepo;

        //public PlansController(IGenericRepository<Plan> planRepo)
        //{
        //    //_dbContext = new GymDbContext();
        //    //_planRepo = new PlanRepository();
        //    _planRepo = planRepo;
        //}

        private readonly IPlanService _planService;
        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }

        // Get baseUrl/Plans/Index
        // Get baseUrl/Plans
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            return View(await _planService.GetAllPlansAsync(ct));
        }

        // Get baseUrl/Plans/Details/1
        public async Task<IActionResult> Details(int id, CancellationToken ct) 
        {
            var plan = await _planService.GetPlanByIdAsync(id, ct);
            if (plan == null) 
            {
                TempData["ErrorMessage"] = "Plan Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        // Edit (GET)
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var plan = await _planService.GetPlanToUpdateAsync(id, ct);

            if (plan is null)
            {
                TempData["ErrorMessage"] = "Plan cannot be edited (not found, inactive, or has active members).";
                return RedirectToAction(nameof(Index));
            }

            return View(plan);
        }

        //Edit (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdatePlanViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _planService.UpdatePlanAsync(id, model, ct);

            if (result)
            {
                TempData["SuccessMessage"] = "Plan updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Failed to update the plan.";
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Activate(int id, CancellationToken ct)
        {
            var result = await _planService.ToggleActivationAsync(id, ct);

            if (result)
            {
                TempData["SuccessMessage"] = "Plan status changed successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to toggle plan status.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
