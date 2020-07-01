using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DattingApp.api.Dtos;
using DattingApp.api.helpers;
using DattingApp.api.Models;
using Microsoft.EntityFrameworkCore;

namespace DattingApp.api.Data
{
    public class DattingRepository : IDattingRepository
    {
        public DattingRepository(DataContext context)
        {
            _context = context;
        }

        public DataContext _context { get; }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Like> GetLike(int userId, int receptId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == receptId);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u=> u.UserId == userId).
                    FirstOrDefaultAsync(p=>p.IsMain == true);
        }

        public async Task<User> GetUserName(string name)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == name);
             //userChange;
        }
        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u=>u.Id ==id);
            return user;
        }

        // ajout des parametres de header à GetUsers
        public async Task<PagedList<User>> GetUsers(UserParameters parameters)
        {
            // modification de la requête au lieu de renvoyer un ToList
            // utilisation de CreateAsync avec le user comme parametre et
            // les pages et taille de page
                                    // AsQuerryable pour le Linq
            var users = _context.Users.AsQueryable();
            // faire 2 lignes si pb 
                users = users.Where(u => u.Id != parameters.UserId);
                users = users.Where(u => u.Gender == parameters.Gender);
                if (parameters.Likers)
                {
                    var userLikers = await GetUserLikes(parameters.UserId, parameters.Likers);
                    users = users.Where(u => userLikers.Contains(u.Id));
                }
                
                if (parameters.Likees)
                {
                    var userLikees = await GetUserLikes(parameters.UserId, parameters.Likers);
                    users = users.Where(u => userLikees.Contains(u.Id));
                }

                if(parameters.MinAge != 18 || parameters.MaxAge != 99)
                {
                    var maxDatBirth = DateTime.Today.AddYears( - parameters.MinAge);
                    var minDatBirth = DateTime. Today.AddYears( - parameters.MaxAge -1);
                    users = users.Where(u => u.DateOfBirth <= maxDatBirth && u.DateOfBirth >= minDatBirth);
                }
           
            return await PagedList<User>.CreateAsync(users, parameters.PageNumber,
                                        parameters.PageSize);
        }//inversé pnumber et psize

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
          var user = await _context.Users
          .FirstOrDefaultAsync(u => u.Id == id);
          if (likers)
          {
              return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
          }   
          else
          {
              return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
          }   
        }
         public async Task<bool> SaveAll()
        {
            var tt =await _context.SaveChangesAsync() > 0;
            return tt;
        }

        public async Task<Message> GetMessage(int id)
        {
            var message = await _context.Messages.FirstOrDefaultAsync(u =>u.Id == id);
            return message;
        }

        public Task<PagedList<Message>> GetMessagesForUser(MessageParameters messageParameters)
        {
            // asQueryable pour faire des requetes après
<<<<<<< HEAD
            var messages = _context.Messages                
=======
            var messages = _context.Messages
>>>>>>> 56034acd22d0259637fc99b17463fae12366e83b
                .AsQueryable();
            
            switch(messageParameters.Contener)
            {
                case "Inbox" : 
                    messages = messages.Where(m => m.ReceptId == messageParameters.UserId && m.ReceptDeleted == false);
                    break;
                case "Outbox" : 
                    messages = messages.Where(m => m.SenderId == messageParameters.UserId && m.SenderDeleted == false);
                    break;
                default : 
                    messages = messages.Where(m => m.ReceptId == messageParameters.UserId && m.IsRead == false
                     && m.ReceptDeleted == false);
                    break;                    
            };
            messages = messages.OrderByDescending(m =>m.MessageSent);

            return PagedList<Message>.CreateAsync(messages, messageParameters.PageNumber,messageParameters.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessagesThread(int userId, int receptId)
        {
                var messages = await _context.Messages
                .Where(m => m.ReceptId == userId && m.SenderId == receptId && m.ReceptDeleted == false
                        || m.SenderId == userId && m.ReceptId == receptId && m.SenderDeleted == false)
                .OrderByDescending(m => m.MessageSent)
                .ToListAsync();   
                return messages;
        }
    }
}