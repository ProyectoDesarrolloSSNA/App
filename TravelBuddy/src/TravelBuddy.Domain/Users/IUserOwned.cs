using System;

namespace TravelBuddy.Users
{
    public interface IUserOwned
    {
        Guid UserId { get; set; }
    }
}