using System.Collections.Generic;
using System.Linq;
using DattingApp.api.Models;
using Newtonsoft.Json;

namespace DattingApp.api.Data
{
    public class Seed
    {
        public static void SeedUsers(DataContext context)
        {
            if(!context.Users.Any()) // si la base est vide
            {
                // lecture dans le fichier json
                var userData = System.IO.File.ReadAllText("Data/userSeedData.json");
                // déserialisation du fichier dans users
                var users = JsonConvert.DeserializeObject<List<User>>(userData); 
                foreach (var user in users)
                {
                    // ajout dans l'objet 
                    byte[] passwordHash, passwordSalt;
                    CreatePassword("password", out passwordHash, out passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.UserName = user.UserName.ToLower();
                    context.Users.Add(user);
                }
                // ecriture dans la base
               context.SaveChanges();
            }   
        }
        private static void CreatePassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //creation du hash -- using pour gérer le try catch idisposable
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
                //crée la clé associée au password crypté
                passwordSalt = hmac.Key;
                //transorme chq caractère de password en crypté
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); 
            }
        }
    }
}


