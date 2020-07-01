using System;

namespace DattingApp.api.Dtos
{
    public class MessageToReturnDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderKnownAs { get; set; }
        public string SenderPhotoUrl { get; set; }
        public string ReceptPhotoUrl { get; set; }
        public int ReceptId { get; set; }
        public string ReceptKnownAs{ get; set; }
        
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime MessageSent { get; set; }
        public DateTime? DateRead { get; set; }
  
    }
}