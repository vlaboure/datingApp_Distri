using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DattingApp.api.Data;
using DattingApp.api.Dtos;
using DattingApp.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DattingApp.api.Controllers
{
    [Route("api/[controller]")]
    //[apiController]--> très important évite des contrôles et des [FromBody]...&
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _rep;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public IDattingRepository _repo {get;}


                            // injection de IAuthRepository et AutoMapper et IConfiguration
                            //services.AddScoped et services. AddAutoMapper
        public AuthController(IAuthRepository rep, IConfiguration config, IMapper mapper,IDattingRepository repo)
        {
            _mapper = mapper;
            _rep = rep;
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        /*
            UsersForRegisterDto userForRegisterDto==> si pas de [apiController]---> obligation de mettre [FromBody]
        */
        public async Task<IActionResult> Register(UsersForRegisterDto userForRegisterDto)
        {

            // !! si pas de [apiController]---> test necessaire
            /*
             if(!ModelState.IsValid)
                 return BadRequest(ModelState);
             */
            userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();
            if (await _rep.UserExists(userForRegisterDto.UserName))//utilisation du repo AuthRepository
                return BadRequest("user exists");

            var userToCreate = _mapper.Map<User>(userForRegisterDto);
            //utilisation du repo AuthRepository
            var createdUser = await _rep.Register(userToCreate, userForRegisterDto.Password,false);
            var userToReturn = _mapper.Map<UserForDetailDto>(createdUser);
            
            return CreatedAtRoute("GetUser", new{Controller="Users",Id = createdUser.Id}, userToReturn);
        }

        [HttpPut("resetUser")]
            //tentative reset password
            // NE FONCTIONNE PAS 12/06/2020
        public async Task<IActionResult> ResetUser(UserForLoginDto userLog)
        {
            //recup du user
            var userToReset = await _repo.GetUserName(userLog.UserName);
            if(userToReset == null)
                return Unauthorized(); 
            userToReset = _mapper.Map(userLog,userToReset );                 
            var ResetedUser = await _rep.Register(userToReset, userLog.Password, true);     
            
            
            //if(await _repo.SaveAll())
                return NoContent();      
           // return BadRequest("Impossible de modifier le mot de passe");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _rep.Login(userForLoginDto.UserName.ToLower(), userForLoginDto.Password);
            if (userFromRepo == null)
                return Unauthorized();
            // login accepté
            //tableau pour le corps de token            
            var claims = new[]
            {   //id --> ClaimTypes.NameIdentifier
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                //UserName -->ClaimTypes.Name
                new Claim(ClaimTypes.Name,userFromRepo.UserName)
            };
            //création de la clé en encodant la valeur de token dans AppSettings
            // clé de cryptage :
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            // cryptage
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            //écriture dans la variable pour le jeton
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            //jwt ---> jeton jwtSecurityTokenHandler --> handler de jeton
            var tokenHandler = new JwtSecurityTokenHandler();
            // création du token
            var token = tokenHandler.CreateToken(tokenDescriptor);

    //********* pour photo dans la barre menu map de userFromRepo avec UserForListDto --> photoUrl
            var user = _mapper.Map<UserForListDto>(userFromRepo);
    //*************************************************** 
            return Ok(new
            {   // ajout du user au token
                token = tokenHandler.WriteToken(token),user
            });
        }
    }
}