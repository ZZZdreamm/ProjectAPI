using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectAPI.DTOs;
using ProjectAPI.Entities;
using ProjectAPI.Helpers;
using ProjectAPI.Migrations;
using System.ComponentModel;
using System.Linq;

namespace ProjectAPI.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessageController : ControllerBase
    {

        private readonly UserManager<IdentityUser> userManager;
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;

        public MessageController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration, ApplicationDBContext context, IMapper mapper, IFileStorageService fileStorageService)
        {
            this.userManager = userManager;
            this.context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
        }
        [HttpPost("send")]
        public async Task<ActionResult> SendMessage([FromForm] MessageCreationDTO messageCreationDTO)
        {
            var message = mapper.Map<Message>(messageCreationDTO);
            if (message.ImageContent != null)
            {
                message.ImageContent = await fileStorageService.SaveManyFiles("messages", messageCreationDTO.ImageContent);
                if (message.ImageContent.Contains('Ω'))
                {
                    var images = message.ImageContent.Split('Ω');
                    foreach (var image in images)
                    {
                        var mes = message;
                        if (mes.Id != 0)
                        {
                            mes.Id = 0;
                        }
                        mes.ImageContent = image;
                        context.Add(mes);
                        context.SaveChanges();
                    }
                    return NoContent();
                }
            }

            if (messageCreationDTO.TextContent == null && messageCreationDTO.ImageContent == null)
            {
                message.ImageContent = "/like.png";
            }
            context.Add(message);
            context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("get/{userId}/{friendId}/{numberOfMessagesStacks}")]

        public async Task<ActionResult<List<MessageDTO>>> GetMessages([FromRoute] string userId, [FromRoute] string friendId, [FromRoute] int numberOfMessagesStacks = 1)
        {
            var messages = context.Messages.Where(x => (x.SenderId == userId && x.ReceiverId == friendId)
            || x.SenderId == friendId && x.ReceiverId == userId).ToList().OrderByDescending(x => x.Id).Take(numberOfMessagesStacks*15).OrderBy(x => x.Id);

            var mes = mapper.Map<List<MessageDTO>>(messages);

            return mes;
        }


        [HttpGet("getnewest/{userId}/{friendId}/{amount}")]

        public async Task<ActionResult<List<MessageDTO>>> GetNewestMessage([FromRoute] string userId, [FromRoute] string friendId, [FromRoute] int amount = 1)
        {
            var messages = context.Messages.Where(x => (x.SenderId == userId && x.ReceiverId == friendId)
            || x.SenderId == friendId && x.ReceiverId == userId).ToList().OrderByDescending(x => x.Id).Take(amount).OrderBy(x => x.Id);

            var mes = mapper.Map<List<MessageDTO>>(messages);

            return mes;
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteMessage([FromRoute] int id)
        {
            var messageToDelete = context.Messages.Where(x => x.Id == id).ToList();
            context.Remove(messageToDelete[0]);
            context.SaveChangesAsync();

            return NoContent();
        }
    }
}
