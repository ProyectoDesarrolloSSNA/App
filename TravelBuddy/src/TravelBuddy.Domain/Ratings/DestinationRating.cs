using System;
using Volo.Abp.Domain.Entities.Auditing;
using TravelBuddy.Users;

namespace TravelBuddy.Ratings
{
    public class DestinationRating : AuditedAggregateRoot<Guid>, IUserOwned   // <-- PUBLIC
    {
        public Guid DestinationId { get; private set; }
        public int Score { get; private set; }
        public string? Comment { get; private set; }
        public Guid UserId { get; set; }

        protected DestinationRating() { }          // OK para EF

        public DestinationRating(                 // <-- CONSTRUCTOR PUBLIC
            Guid id,
            Guid destinationId,
            int score,
            string? comment,
            Guid userId) : base(id)
        {
            SetScore(score);
            DestinationId = destinationId;
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
            UserId = userId;
        }

        public void SetScore(int score)
        {
            if (score < 1 || score > 5)
                throw new ArgumentOutOfRangeException(nameof(score), "Score debe estar entre 1 y 5.");
            Score = score;
        }

        public void Update(string? comment, int? score = null)
        {
            if (score.HasValue) SetScore(score.Value);
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        }
    }
}
