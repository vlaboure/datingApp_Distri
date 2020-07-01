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
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // propriété type interface du repository d'accès au DbContext
        public IDattingRepository _repo { get; }
        private readonly IMapper _mapper;
                            // injection de IDattingRepository et AutoMapper
                            //services.AddScoped et services. AddAutoMapper
        public UsersController(IDattingRepository repo, IMapper mapper)
        {
            _mapper = mapper;  
            _repo = repo;
        }

        [HttpGet]
            // [FromQuery] permet d'envoyer une requête sans les paramètres de pagination
        public async Task<IActionResult> GetUsers([FromQuery]UserParameters parameters)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _repo.GetUser(currentUserId);

            parameters.UserId = currentUserId;            
        // si sexe non renseigné récupération du sexe opposé de celui du user logué
            if(string.IsNullOrEmpty(parameters.Gender))
            {
                parameters.Gender = userFromRepo.Gender == "male" ? "female":"male"; 
            }

            var users = await _repo.GetUsers(parameters);
            // users avec le mapping .Map<destination>(source)
            // donc IEnumerable de UserForListDto
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            // appel de la methode de helpers
            // AddPagination comme this on l'appelle avec l'objet courant
            // comme le GetUsers est changé, on accède aux paramètres de pages 
            Response.AddPagination(users.CurrentPage, users.PageSize, 
                                users.TotalCount, users.TotalPages);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            // utilisation de automapp
            var userToReturn = _mapper.Map<UserForDetailDto>(user);
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            // vérif si le token correspond à l'id reçu dans la requête
  
            var userFromRepo = await _repo.GetUser(id);
            // map paramètre source , paramètre dest
            _mapper.Map(userForUpdateDto, userFromRepo);
            if(await _repo.SaveAll())
                return NoContent();      
            throw new Exception($"Erreur sauvegarde id {id}");        
        }

            // http://localhost:5000/api/users/userId/like/user à liker
        [HttpPost("{id}/Like/{receptId}")]
        public async Task<IActionResult> LikeUser(int id, int receptId)
        {
                      // vérif si le token correspond à l'id reçu dans la requête
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                return Unauthorized();

            // verif si like existe
            var like = await _repo.GetLike(id, receptId);
            if (like != null)
                return BadRequest("user déjà liké");

            // vérif si user à liker existe                
            if (await _repo.GetUser(receptId) == null)
                return NotFound();
                
                // OK --> création de l'objet
            like = new Like
            {
                LikerId = id,
                LikeeId = receptId
            };
                // ajout dans la base
            _repo.Add<Like>(like);
                // sauvegarde de la base
            if (await _repo.SaveAll())
                return Ok();
            return BadRequest("erreur lors du like");
        }
    }
}