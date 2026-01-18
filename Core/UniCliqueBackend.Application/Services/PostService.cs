using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniCliqueBackend.Application.DTOs.Post;
using UniCliqueBackend.Application.Interfaces.Repositories;
using UniCliqueBackend.Application.Interfaces.Services;
using UniCliqueBackend.Domain.Entities;

namespace UniCliqueBackend.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IFriendshipRepository _friendshipRepository;

        public PostService(
            IPostRepository postRepository,
            IUserRepository userRepository,
            IEventRepository eventRepository,
            IFriendshipRepository friendshipRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _friendshipRepository = friendshipRepository;
        }

        public async Task<PostDto?> CreatePostAsync(CreatePostDto model, string userId)
        {
            if (!Guid.TryParse(userId, out var uid)) return null;

            var user = await _userRepository.GetByIdAsync(uid);
            if (user == null) return null;

            var evt = await _eventRepository.GetByIdAsync(model.EventId);
            if (evt == null) return null;
            
            // Should prompt verify if user joined event?
            // Requirement says "Katıldığı etkinlik hakkında gönderi paylaşma".
            // Implementation: Verify participation.
            var participant = await _eventRepository.GetParticipantAsync(model.EventId, uid);
            // Allowing owners to post too
            if (participant == null && evt.OwnerId != uid) return null; // Must be participant or owner

            var post = new Post
            {
                UserId = uid,
                EventId = model.EventId,
                Content = model.Content,
                PhotoUrl = model.PhotoUrl,
                CreatedAt = DateTime.UtcNow
            };

            await _postRepository.AddAsync(post);

            // Gamification
            user.InteractionScore += 5; 
            await _userRepository.UpdateAsync(user);

            return MapToDto(post);
        }

        public async Task<bool> DeletePostAsync(Guid postId, string userId)
        {
            if (!Guid.TryParse(userId, out var uid)) return false;

            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null) return false;

            if (post.UserId != uid) return false; // Only owner can delete

            await _postRepository.DeleteAsync(post);
            return true;
        }

        public async Task<IEnumerable<PostDto>> GetPostsByEventIdAsync(Guid eventId)
        {
            var posts = await _postRepository.GetByEventIdAsync(eventId);
            return posts.Select(MapToDto);
        }

        public async Task<IEnumerable<PostDto>> GetMyPostsAsync(string userId)
        {
             if (!Guid.TryParse(userId, out var uid)) return Enumerable.Empty<PostDto>();
             var posts = await _postRepository.GetByUserIdAsync(uid);
             return posts.Select(MapToDto);
        }
        
        public async Task<IEnumerable<PostDto>> GetUserPostsAsync(string userId)
        {
             if (!Guid.TryParse(userId, out var uid)) return Enumerable.Empty<PostDto>();
             var posts = await _postRepository.GetByUserIdAsync(uid);
             return posts.Select(MapToDto);
        }

        public async Task<IEnumerable<PostDto>> GetFeedAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var uid)) return Enumerable.Empty<PostDto>();

            // 1. Get friends
            var friends = await _friendshipRepository.GetFriendsAsync(uid);
            var friendIds = friends.Select(f => f.Id).ToList();
            
            // 2. Add self to see own posts? Usually feed shows friends.
            // Let's stick to friends only as per "Friend activity feed".
            
            if (!friendIds.Any()) return Enumerable.Empty<PostDto>();

            var posts = await _postRepository.GetFeedAsync(friendIds);
            return posts.Select(MapToDto);
        }
        
        private PostDto MapToDto(Post post)
        {
            return new PostDto
            {
                Id = post.Id,
                UserId = post.UserId,
                UserName = post.User != null ? post.User.FullName : "Unknown",
                UserProfilePhoto = post.User != null ? post.User.ProfilePhotoUrl : null,
                EventId = post.EventId,
                EventTitle = post.Event != null ? post.Event.Title : "Unknown Event",
                Content = post.Content,
                PhotoUrl = post.PhotoUrl,
                CreatedAt = post.CreatedAt
            };
        }
    }
}
