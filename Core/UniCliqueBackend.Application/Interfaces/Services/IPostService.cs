using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCliqueBackend.Application.DTOs.Post;

namespace UniCliqueBackend.Application.Interfaces.Services
{
    public interface IPostService
    {
        Task<PostDto?> CreatePostAsync(CreatePostDto model, string userId);
        Task<bool> DeletePostAsync(Guid postId, string userId);
        
        Task<IEnumerable<PostDto>> GetPostsByEventIdAsync(Guid eventId);
        Task<IEnumerable<PostDto>> GetMyPostsAsync(string userId);
        Task<IEnumerable<PostDto>> GetUserPostsAsync(string userId);
        
        Task<IEnumerable<PostDto>> GetFeedAsync(string userId);
    }
}
