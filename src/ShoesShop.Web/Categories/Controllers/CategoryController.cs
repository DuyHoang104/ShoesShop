using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Crosscutting.Utilities.Attribute;
using ShoesShop.Domain.Categories.Dtos;
using ShoesShop.Domain.Categories.Enums;
using ShoesShop.Domain.Categories.Services;
using ShoesShop.Domain.Commons.Enums;

namespace ShoesShop.Web.Categories.Controllers;

[Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]
[Route("Admin/Categories")]
public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    private int GetCurrentAdminId()
    {
        return int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "adminId")?.Value, out var adminId)
            ? adminId
            : 0;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllAsync();
        return View("~/Categories/Views/apps-ecommerce-categories.cshtml", categories);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(CategoryDto dto)
    {
        var adminId = GetCurrentAdminId();

        dto.CreateBy = adminId;
        dto.LastActionBy = adminId;
        dto.CreateTimeStamp = DateTime.UtcNow;
        dto.LastActionTimeStamp = DateTime.UtcNow;
        dto.LastAction = LastAction.Create;
        dto.Status = CategoryStatus.Active;

        var result = await _categoryService.CreateAsync(dto);
        if (result == null)
        {
            return BadRequest(new { message = "Failed to create category" });
        }

        return Ok(new { message = "Created successfully" });
    }

    [HttpPost("Update")]
    public async Task<IActionResult> Update(CategoryDto dto)
    {
        var adminId = GetCurrentAdminId();

        dto.LastActionBy = adminId;
        dto.LastActionTimeStamp = DateTime.UtcNow;
        dto.LastAction = LastAction.Update;

        var result = await _categoryService.UpdateAsync(dto);
        if (!result)
        {
            return BadRequest(new { message = "Failed to update category" });
        }
        return Ok(new { message = "Updated successfully" });
    }

    [HttpPost("Delete")]
    [ValidateModel("index")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _categoryService.GetByIdAsync(id);
        var result = await _categoryService.DeleteAsync(dto.Id);

        if (!result)
        {
            return BadRequest(new { message = "Failed to delete category" });
        }

        return Ok(new { message = "Deleted successfully" });
    }
}
