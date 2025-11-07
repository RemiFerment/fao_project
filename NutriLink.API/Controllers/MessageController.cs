using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriLink.API.Data;
using NutriLink.API.Models;
using NutriLink.API.Services;


namespace NutriLink.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserService _userService;
    public MessageController(AppDbContext context, UserService userService)
    {
        _context = context;
        _userService = userService;
    }

    [HttpGet("{uuid}/to/{receiverUuid}")]
    [Authorize(Policy = "SameUser")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMyMessages(string uuid, string receiverUuid)
    {
        var senderId = await _userService.GetIdByUuidAsync(uuid);
        if (senderId == null) return NotFound(new { message = "User not found." });

        var receiverId = await _userService.GetIdByUuidAsync(receiverUuid);
        if (receiverId == null) return NotFound(new { message = "Recipient not found." });

        var messages = await _context.Messages
            .AsNoTracking()
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m =>
                (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                (m.SenderId == receiverId && m.ReceiverId == senderId))
            .OrderBy(m => m.DateTime)
            .ToListAsync();

        var messageDtos = messages.Select(m => new MessageDTO
        {
            Id = m.Id,
            DateTime = m.DateTime,
            SenderUuid = m.Sender!.UUID,
            ReceiverUuid = m.Receiver!.UUID,
            Content = m.Content
        });

        return Ok(messageDtos);
    }


    [HttpPost("{uuid}/to/{recieverUuid}")]
    [Authorize(Policy = "SameUser")]
    public async Task<ActionResult<MessageDTO>> SendMessage(string uuid, string recieverUuid, [FromBody] MessageDTO messageDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (uuid == recieverUuid) return BadRequest(new { message = "Cannot send message to oneself." });
        var sender = await _userService.GetByUuidAsync(uuid);
        if (sender == null) return NotFound(new { message = "Sender not found." });

        var receiver = await _userService.GetByUuidAsync(recieverUuid);
        if (receiver == null) return NotFound(new { message = "Recipient not found." });
        if (receiver.UUID != messageDto.ReceiverUuid) return BadRequest(new { message = "Receiver UUID mismatch." });


        var message = new Message
        {
            DateTime = DateTime.UtcNow,
            SenderId = sender.Id,
            Sender = sender,
            ReceiverId = receiver.Id,
            Receiver = receiver,
            Content = messageDto.Content,
            IsRead = false
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMyMessages), new { uuid = sender.UUID, receiverUuid = receiver.UUID }, messageDto);
    }

    [HttpPatch("{uuid}/messages/{messageId}/read")]
    [Authorize(Policy = "SameUser")]
    public async Task<ActionResult> MarkMessageAsRead(string uuid, int messageId, [FromBody] MessageIsReadDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userService.GetByUuidAsync(uuid);
        if (user == null)
            return NotFound(new { message = "User not found." });

        var message = await _context.Messages
            .FirstOrDefaultAsync(m => m.Id == messageId && (m.ReceiverId == user.Id || m.SenderId == user.Id));

        if (message == null)
            return NotFound(new { message = "Message not found or access denied." });

        if (message.ReceiverId != user.Id)
            return Forbid("Only the receiver can mark a message as read.");

        message.IsRead = dto.IsRead;
        _context.Messages.Update(message);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Message read status updated successfully.", dto.IsRead });
    }

}