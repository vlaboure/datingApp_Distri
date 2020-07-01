namespace DattingApp.api.helpers
{
    public class UserParameters
    {
        public int PageNumber { get; set; }=1;
        private int maxPageSize = 50;
        
        private int pageSize = 10;

        public int PageSize
        {
            get {return pageSize;}
            set {pageSize = (value > maxPageSize)? maxPageSize : value; }
        }

        public int UserId { get; set; }
        public string Gender { get; set; }
        
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 99;
        public bool Likers{get;set;}= false;
        public bool Likees { get; set; } = false;

    }
}