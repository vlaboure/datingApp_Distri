using System;

namespace DattingApp.api.Dtos
{
    public class MessageForCreationDto
    {
        public int SenderId { get; set; }
        public int ReceptId { get; set; }
        public DateTime MessageSent { get; set; }
        public string Content { get; set; }

        public MessageForCreationDto()
        {
            MessageSent = DateTime.Now;
        }
    }
}