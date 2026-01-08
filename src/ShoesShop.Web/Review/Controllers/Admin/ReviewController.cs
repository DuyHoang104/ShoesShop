using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Domain.Shares.Review.Enums;
using ShoesShop.Domain.Shares.Review.Services;

namespace ShoesShop.Web.Review.Controllers.Admin;

[Route("Admin/Reviews")]
[Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]

public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }
    
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var reviews = await _reviewService.GetAllReviewsAsync();
        return View("~/Review/Views/Admin/Index.cshtml", reviews);
    }

    [HttpGet("Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var review = await _reviewService.GetReviewDetailsAsync(id);
        if (review == null)
            return NotFound();

        return View("~/Review/Views/Admin/Details.cshtml", review);
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _reviewService.DeleteAsync(id);

        if (!success)
        {
            return NotFound();
        }

        TempData["Success"] = "Review deleted successfully";
        return RedirectToAction("Index");
    }
   
    [HttpPost("UpdateStatus/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, int status)
    {
        var newStatus = (ReviewStatus)status;

        // Cập nhật review cha
        var updated = await _reviewService.UpdateStatusAsync(id, newStatus);

        if (!updated)
            return NotFound();

        // Nếu muốn toggle cả review con, thêm code cập nhật con ở đây
        // await _reviewService.UpdateChildStatusesAsync(id, newStatus);

        return Ok(new { success = true, status = newStatus.ToString() });
    }

}
