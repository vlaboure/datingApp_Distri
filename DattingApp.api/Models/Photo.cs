using System;

namespace DattingApp.api.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set;}
        // photo de profil
        public bool IsMain { get; set; }

        public string PublicId { get; set; }
        //liaisons entre photo et user pour permettre la suppression en cascade
        public virtual User User { get; set; }
        public int UserId { get; set; }
    }
}