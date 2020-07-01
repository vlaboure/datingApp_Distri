namespace DattingApp.api.helpers
{
    public class MessageParameters
    {
        public int PageNumber { get; set; }=1;
        private int maxPageSize = 50;
        
        private int pageSize = 10;

        public int PageSize
        {
            get {return pageSize;}
            set {pageSize = (value > maxPageSize)? maxPageSize : value; }
        }

        public string Contener { get; set; }="Unread";
        public int UserId { get; set; }        
    }
}