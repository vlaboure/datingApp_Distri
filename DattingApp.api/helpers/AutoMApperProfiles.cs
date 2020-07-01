using System.Linq;
using AutoMapper;
using DattingApp.api.Dtos;
using DattingApp.api.Models;

namespace DattingApp.api.helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // création de mapp entre les classes
            CreateMap<User,UserForDetailDto>()   
                .ForMember(dest => dest.PhotoUrl, opt => opt
                    .MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                // utilisatation de this.CalcAge pour transformer date naissance en age 
                .ForMember(dest => dest.Age, opt => opt
                    .MapFrom(src => src.DateOfBirth.CalcAge())); //CalcAge statique de Extension           s
            CreateMap<User,UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => 
                            src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => 
                            src.DateOfBirth.CalcAge()));           
            CreateMap<Photo,PhotoForDetailDto>(); //source - destination    
            CreateMap<UserForUpdateDto,User>();  //source - destination   
            CreateMap<UserForLoginDto,User>();  
            CreateMap<Photo,PhotoForReturnDto>();//source - destination
            CreateMap<PhotoForCreationDto,Photo>();
            CreateMap<UsersForRegisterDto,User>();
            CreateMap<MessageForCreationDto,Message>();
            // CHAUD !!!!!!
            CreateMap<Message,MessageToReturnDto>()
              .ForMember(m => m.SenderPhotoUrl, opt => opt
                    .MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
              .ForMember(m => m.ReceptPhotoUrl, opt => opt
                    .MapFrom(u => u.Recept.Photos.FirstOrDefault(p => p.IsMain).Url));
        }
    }
}