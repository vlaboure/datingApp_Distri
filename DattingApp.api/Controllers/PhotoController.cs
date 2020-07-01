using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DattingApp.api.Data;
using DattingApp.api.Dtos;
using DattingApp.api.helpers;
using DattingApp.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace DattingApp.api.Controllers
{
    [Authorize]
    [Route("api/users/{userid}/photos")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private Cloudinary _cloudinary;

        public IDattingRepository _repo { get; set; }
        public IMapper _mapper { get; set; }
        public IOptions<CloudinarySettings> _cloudinaryConfig { get; set; }
        public PhotoController(IDattingRepository repo, IMapper mapper        
                           , IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _repo = repo;
                // on renseigne les données de CloudinarySettings de appsettings.json
            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );
            
            _cloudinary = new Cloudinary(acc);

        }

        [HttpGet ("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId,
                [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            //********************
           //même code que dans userController
            // vérif si le token correspond à l'id reçu dans la requête
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                return Unauthorized();
            var userFromRepo = await _repo.GetUser(userId);
            var file = photoForCreationDto.File;
            //*****************
                                // methode de cloudinary contenant l'image du cloud avec parametres
            var uploadResult = new ImageUploadResult();
            if (file.Length >0)
            {
                //lecture de fichier image
                using(var stream = file.OpenReadStream())
                {               // methode de cloudinary pour les paramètres de l'image
                    var uploadParams = new ImageUploadParams
                    {           // methode de cloudinary pour les fichiers       
                        File = new FileDescription(file.Name,stream),             /*initialement :.Gravity("face")*/
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    // dans la variable d'image uploadResult on charge les paramètres d'affichage et l'image
                    // avec les paramètres : nom chemin, taille...
                            //chargement de la photo sur le cloud
                    uploadResult = _cloudinary.Upload(uploadParams);
                       
                };
            }
            // on transmet au DTO les valeurs récupérées depuis le coudinary
            photoForCreationDto.Url = uploadResult.Uri.ToString();
            // publicId fourni par le cloudinary
            photoForCreationDto.PublicId = uploadResult.PublicId;
            // enfin on mappe photo / photoForUpdate
            //mapp avec type de retour<> et passage de parmètre
            var photo = _mapper.Map<Photo>(photoForCreationDto);
            userFromRepo.Photos.Add(photo);
            // si une seule photo
            if (!userFromRepo.Photos.Any(u => u.IsMain))
                photo.IsMain = true;
            if (await _repo.SaveAll())
            {
               var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
               return CreatedAtRoute("GetPhoto", new {userId = userId, id = photo.Id},photoToReturn);
            }
            return BadRequest("Impossible d'ajouter la photo");
        }

        [HttpPost ("{id}/setMain")]
        public async Task<IActionResult>  SetMainPhoto(int userId, int id)
        {
            //même code que dans userController
            // vérif si le token correspond à l'id reçu dans la requête
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                return Unauthorized();
           
            var userFromRepo = await _repo.GetUser(userId);
            if(!userFromRepo.Photos.Any(p =>p.Id == id))
                return Unauthorized();
            
            // récupération de la photo choisie
            var photoFromRepo = await _repo.GetPhoto(id);
            if(photoFromRepo.IsMain)
                return BadRequest("Photo déjà en profil");
            
            // récupération de la photo de profil et changement de IsMain
            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;
            
            photoFromRepo.IsMain = true;

            if(await _repo.SaveAll())
                return NoContent();
            return BadRequest("Erreur lors du chargment de la photo de profil");    
        }

        [HttpDelete ("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            // idem SetMainPhoto
             //même code que dans userController
            // vérif si le token correspond à l'id reçu dans la requête
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                return Unauthorized();
           
            var userFromRepo = await _repo.GetUser(userId);
            if(!userFromRepo.Photos.Any(p =>p.Id == id))
                return Unauthorized("Photo non trouvée");
            
            // récupération de la photo choisie
            var photoFromRepo = await _repo.GetPhoto(id);
            if(photoFromRepo.IsMain)
               return BadRequest("Impossible de supprimer sa photo de profil");
                // si la photo provient de cloudinary
            if (photoFromRepo.PublicId !=null)
            {
                var deletionParams = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deletionParams);
                if(result.Result == "ok")
                    _repo.Delete(photoFromRepo);
                
            };   
            if (photoFromRepo.PublicId ==null)
            {
                 _repo.Delete(photoFromRepo);
            }   
            if(await _repo.SaveAll())
                return Ok();
            return BadRequest("Erreur durant la suppression");
        }
    }
}