using System;
using System.Threading.Tasks;
using DattingApp.api.Models;
using Microsoft.EntityFrameworkCore;

namespace DattingApp.api.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public DataContext Context { get; }

        public async Task<User> Login(string userName, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.UserName==userName);
            if(user == null)return null;
            if(!VerifyPasswordHash(password,user.PasswordHash,user.PasswordSalt))
                return null;
            return user;

        }

        // la clé de cryptage est passée pour contrôler
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
                using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){      
                //transorme chq caractère de password en crypté
                var computedHash = hmac.ComputeHash(System.Text.Encoding.                                                                                                                                                                                                                                                                                                                                   UTF8.GetBytes(password)); 
                for(int i =0;i<computedHash.Length;i++){
                    //comparer chq caractère du tableau de byte
                    if(computedHash[i]!=passwordHash[i])return false;
                }                
                return true;
            }
        }


        public async Task<User> Register(User user, string password,bool reset)
        {
            byte[] passwordHash, passwordSalt;
            //passwordHash passwordSalt passés par réference
            CreatePassword(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            if(!reset)
                await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }
        public async Task<User> ResetUser(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            //passwordHash passwordSalt passés par réference
            CreatePassword(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //creation du hash -- using pour gérer le try catch idisposable
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
                //crée la clé associée au password crypté
                passwordSalt = hmac.Key;
                //transorme chq caractère de password en crypté
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); 
            }
        }

        public async Task<bool> UserExists(string userName)
        {
            if(await _context.Users.AnyAsync(x=>x.UserName == userName))
                return true;
            return false;
        }
    }
}