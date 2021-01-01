using System;

namespace WebService
{
    public class Token
    {
        public string Jwt { get; set; }
        public string Refresh { get; set; }
    }

    public class RefreshData
    {
        public string Id { get; set; }
        public string Refresh { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}