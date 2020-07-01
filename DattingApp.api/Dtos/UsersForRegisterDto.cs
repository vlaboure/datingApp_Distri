using System;
using System.ComponentModel.DataAnnotations;

namespace DattingApp.api.Dtos
{
    //DTO == Data Transfet Object
        // supplément aux models qui représentent la BDD
        // pour les données nécessaires pour l'affichage
    public class UsersForRegisterDto
    {
        [Required]
         public string UserName { get; set; }
        [Required]
        [StringLength(8,MinimumLength = 4, ErrorMessage= "Taille du mot de passe entre 4 et 8")]
         public string Password { get; set; }
         public string Gender{get;set;}
         public string KnownAs{get;set;}
         [Required]
         public DateTime DateOfBirth { get; set; }
 
         public DateTime Created { get; set; }
         public DateTime LastActive { get; set; }
         [Required]
         public string City { get; set; }
         [Required]
         public string Country { get; set; }
         public bool Reset { get; set; } = false;// pour reset password

         UsersForRegisterDto()
         {
             Created = DateTime.Now;
             LastActive = DateTime.Now;

         }
    }
}