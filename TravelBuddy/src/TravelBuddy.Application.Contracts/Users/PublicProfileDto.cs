using System;

namespace TravelBuddy.Users
{
    public class PublicProfileDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
