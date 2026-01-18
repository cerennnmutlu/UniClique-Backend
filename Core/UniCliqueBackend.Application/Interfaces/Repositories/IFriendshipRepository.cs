using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCliqueBackend.Domain.Entities;
using UniCliqueBackend.Domain.Enums;

namespace UniCliqueBackend.Application.Interfaces.Repositories
{
    public interface IFriendshipRepository
    {
        Task<Friendship?> GetFriendshipAsync(Guid user1Id, Guid user2Id);
        Task<Friendship?> GetByIdAsync(Guid id);
        
        Task<IEnumerable<Friendship>> GetFriendRequestsAsync(Guid userId, bool incoming); // incoming=true -> received, false -> sent
        Task<IEnumerable<User>> GetFriendsAsync(Guid userId);
        
        Task AddAsync(Friendship friendship);
        Task UpdateAsync(Friendship friendship);
        Task DeleteAsync(Friendship friendship);
        
        Task<bool> AreFriendsAsync(Guid user1Id, Guid user2Id);
    }
}
