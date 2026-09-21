using System.Security.Claims;
using HotelManagementSystem.Application;
using HotelManagementSystem.Contracts;
using HotelManagementSystem.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers;

[ApiController, Route("api/reservations/{reservationId:int}/messages")]
[Authorize(Roles = "Guest")]
public class HostChatController(HostChatService chat) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> Get(int reservationId)
    {
        var guestId = int.Parse(User.FindFirstValue("guestId")!);
        var messages = await chat.Messages(reservationId, guestId);
        return Ok(new ApiResponse<object>(true, messages.Select(View)));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> Send(int reservationId, HostMessageRequest request)
    {
        var guestId = int.Parse(User.FindFirstValue("guestId")!);
        var messages = await chat.Send(reservationId, guestId, request.Body);
        return Ok(new ApiResponse<object>(true, messages.Select(View)));
    }

    static object View(HotelManagementSystem.Domain.HostMessage message) => new
    {
        message.Id, message.Sender, message.Body, message.SentAtUtc
    };
}
