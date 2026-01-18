namespace UniCliqueBackend.Application.DTOs.Business
{
    public class BusinessStatsDto
    {
        public int TotalEvents { get; set; }
        public int TotalParticipants { get; set; }
        public int ActiveEvents { get; set; }
        public int FriendshipCount { get; set; } // Representing reach/followers
        public double AverageParticipantsPerEvent { get; set; }
    }
}
