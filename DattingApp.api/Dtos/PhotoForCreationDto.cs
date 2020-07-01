using System;
using Microsoft.AspNetCore.Http;

namespace DattingApp.api.Dtos
{
    public class PhotoForCreationDto
    {
        public string Url { get; set; }
        // IFormFile--> représente la transcription http d'une photo en bit
        public IFormFile File { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        // ID pour le cloudinary
        public string PublicId { get; set; }

        public PhotoForCreationDto()
        {
            DateAdded = DateTime.Now;            
        }
    }
}