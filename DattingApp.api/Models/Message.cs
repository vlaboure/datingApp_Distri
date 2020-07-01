using System;

namespace DattingApp.api.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public virtual User Sender { get; set; }
        public int ReceptId { get; set; }
        public virtual User Recept{ get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime MessageSent { get; set; }
        public DateTime? DateRead { get; set; }
        public bool SenderDeleted { get; set; }
        public bool ReceptDeleted { get; set; }
    }
}