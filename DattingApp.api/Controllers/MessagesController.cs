using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DattingApp.api.Data;
using DattingApp.api.Dtos;
using DattingApp.api.helpers;
using DattingApp.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DattingApp.api.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMapper _mapper;
        public IDattingRepository _repo { get; }
        public MessagesController(IDattingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        [HttpGet ("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int id)
        {
            // verification user autorisé
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
            return Unauthorized();
            var messageFromRepo = await _repo.GetMessage(id);

            if(messageFromRepo == null)
                return NotFound();
            
            return Ok(messageFromRepo);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId,[FromQuery]MessageParameters messageParameters)
        {
                        // verification user autorisé
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
            return Unauthorized();

            messageParameters.UserId = userId;
            var messagesFromRepo = await _repo.GetMessagesForUser(messageParameters);
            
            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);
            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize,
                                messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);
            return Ok(messages);
        }

        [HttpGet("thread/{receptId}")]
        public async Task<IActionResult> GetMessagesThread(int userId, int receptId)
        {
                          // verification user autorisé
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
            return Unauthorized();
            var messagesFromRepo = await _repo.GetMessagesThread(userId, receptId);
            var messagesThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);
            return Ok(messagesThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {            
            var senderUser = await _repo.GetUser(userId);
            if (senderUser.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
            return Unauthorized();
            messageForCreationDto.SenderId = userId;
            
            var recept = await _repo.GetUser(messageForCreationDto.ReceptId);
            if(recept == null)
                return BadRequest("user non trouvé");

            var message = _mapper.Map<Message>(messageForCreationDto);//dest, source

            _repo.Add(message);

            if(await _repo.SaveAll())
            {                
                var messageToReturn = _mapper.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage",new{userId, id = message.Id},messageToReturn);
            }
            return BadRequest("impossible d'envoyer le message");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                return Unauthorized();
            var messageFromRepo = await _repo.GetMessage(id);
            if(messageFromRepo.SenderId == userId)
                messageFromRepo.SenderDeleted = true;
            
            if(messageFromRepo.ReceptId == userId)
                messageFromRepo.ReceptDeleted = true;
            
            if(messageFromRepo.SenderDeleted && messageFromRepo.ReceptDeleted)
                _repo.Delete(messageFromRepo);
            
            if(await _repo.SaveAll())
            
                return NoContent();
            
            throw new Exception("erreur lors de la suppression du message");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageRead(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                return Unauthorized();
            var messageFromRepo = await _repo.GetMessage(id);
            if (messageFromRepo.ReceptId != userId)
                return Unauthorized();
            messageFromRepo.IsRead = true;
            messageFromRepo.DateRead = DateTime.Now;
            await _repo.SaveAll();
            return NoContent();  
        }
    }
}