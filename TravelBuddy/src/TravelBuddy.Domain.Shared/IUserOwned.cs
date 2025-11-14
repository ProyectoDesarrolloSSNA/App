using System;

namespace TravelBuddy
{
    public interface IUserOwned
    {
        Guid UserId { get; set; }
    }
}
