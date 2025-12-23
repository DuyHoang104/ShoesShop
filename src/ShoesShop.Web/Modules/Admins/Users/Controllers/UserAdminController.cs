using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Crosscutting.Utilities.Attribute;
using ShoesShop.Domain.Modules.User.Users.Enums;
using ShoesShop.Domain.Modules.User.Users.Services;
using ShoesShop.Web.Modules.Admins.Orders.Dtos;
using ShoesShop.Web.Modules.Admins.Users.Dtos;

namespace ShoesShop.Web.Modules.Admins.Users.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]
    [Route("Admin/Users")]
    public class UserAdminController : Controller
    {
        private readonly IUserService _userService;
        public UserAdminController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            var userList = users.Select(user => new UserAdminDto
            {
                ID = user.ID,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                DateOfBirth = user.DateOfBirth,
                Status = user.Status,
                Role = user.Role,
                AvatarUrl = user.AvatarUrl,
                Addresses = user.Addresses?.Select(a => new AddressAdminDto
                {
                    AddressLine1 = a.AddressLine1,
                    City = a.City,
                    Country = a.Country,
                    IsDefault = a.IsDefault
                }).ToList() ?? []
            }).ToList();

            return View("~/Modules/Admins/Users/Views/apps-ecommerce-customers.cshtml", userList);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetAllInfoUsersAsync(id);
            if (user == null)
                return NotFound();

            var userDto = new GetAllInfoUsersAdminDto
            {
                ID = user.ID,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                DateOfBirth = user.DateOfBirth,
                Status = user.Status,
                Role = user.Role,
                Gender = user.Gender,
                AvatarUrl = user.AvatarUrl,

                Addresses = user.Addresses?.Select(a => new AddressAdminDto
                {
                    AddressLine1 = a.AddressLine1,
                    City = a.City,
                    Country = a.Country,
                    IsDefault = a.IsDefault
                }).ToList() ?? new List<AddressAdminDto>(),

                Orders = user.Orders?.Select(o => new OrderAdminDto
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    ShippingFee = o.ShippingFee,
                    Discount = o.Discount,
                    TotalAmount = o.TotalAmount,
                    PaymentMethod = o.PaymentMethod,
                    PaymentStatus = o.PaymentStatus,
                    SubTotal = o.OrderDetails?.Sum(od => od.UnitPrice * od.Quantity) ?? 0m,

                    OrderDetails = o.OrderDetails?.Select(od => new OrderDetailItemAdminModalDto
                    {
                        ProductId = od.ProductId,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice
                    }).ToList() ?? new List<OrderDetailItemAdminModalDto>()
                }).ToList() ?? new List<OrderAdminDto>()
            };

            return View("~/Modules/Admins/Users/Views/apps-ecommerce-customers-details.cshtml", userDto);
        }

        [HttpPost("UpdateStatus/{id}")]
        [ValidateModel("Admin/Users")]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if(user == null)
                return Json(new { success = false, message = "User not found" });

            if(!Enum.TryParse<UserStatus>(newStatus, true, out var status))
                return Json(new { success = false, message = "Invalid status" });

            user.Status = status;
            await _userService.UpdateStatusAsync(user);

            return Json(new { success = true, message = "User status updated successfully" });
        }

        [HttpGet("UpdateRole/{id}")]
        [ValidateModel("Admin/Users/Details")]
        public async Task<IActionResult> UpdateRole(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return Json(new { success = false, message = "User not found" });

            if(user.Role == UserAccountRole.Admin)
                user.Role = UserAccountRole.Customer;
            else
                user.Role = UserAccountRole.Admin;

            await _userService.UpdateRoleAsync(user);

            return Json(new { success = true, message = "User role updated successfully" });
        }
    }
}