using System;

namespace DattingApp.api.Dtos
{
    public class PhotoForDetailDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set;}
        // photo de profil
        public bool IsMain { get; set; }
    }
}