using HotelManagementSystem.Application;
using HotelManagementSystem.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace HotelManagementSystem.Controllers;
[ApiController, Route("api/auth")]
public class AuthController(AuthService auth) : ControllerBase
{
 [HttpPost("register")] public async Task<ActionResult<ApiResponse<object>>> Register(GuestRegisterRequest request)=>Ok(new ApiResponse<object>(true,await auth.RegisterGuest(request)));
 [HttpPost("confirm-email")] public async Task<ActionResult<ApiResponse<object>>> ConfirmEmail(EmailCodeRequest request)=>Ok(new ApiResponse<object>(true,await auth.ConfirmEmail(request)));
 [HttpPost("resend-confirmation")] public async Task<ActionResult<ApiResponse<object>>> ResendConfirmation(EmailCodeRequest request)=>Ok(new ApiResponse<object>(true,await auth.ResendEmailConfirmation(request.Email)));
 [HttpPost("forgot-password")] public async Task<ActionResult<ApiResponse<object>>> ForgotPassword(EmailCodeRequest request)=>Ok(new ApiResponse<object>(true,await auth.ForgotPassword(request.Email)));
 [Authorize, HttpPut("change-password")] public async Task<ActionResult<ApiResponse<object>>> ChangePassword(ChangePasswordRequest request)=>Ok(new ApiResponse<object>(true,await auth.ChangePassword(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),request)));
 [HttpPost("login")] public async Task<ActionResult<ApiResponse<object>>> Login(LoginRequest request)=>Ok(new ApiResponse<object>(true,await auth.Login(request)));
 [HttpPost("confirm-login")] public async Task<ActionResult<ApiResponse<object>>> ConfirmLogin(EmailCodeRequest request)=>Ok(new ApiResponse<object>(true,await auth.ConfirmLogin(request)));
}
