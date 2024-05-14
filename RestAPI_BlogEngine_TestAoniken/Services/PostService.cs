using RestAPi_BlogEngine_TestAoniken.Exceptions;
using RestAPi_BlogEngine_TestAoniken.Models;
using RestAPi_BlogEngine_TestAoniken.Repositories;
using System.Net;

namespace RestAPi_BlogEngine_TestAoniken.Services
{
    public class PostService
    {
        private readonly PostRepository _postRepository;

        public PostService()
        {
            _postRepository = new PostRepository();
        }

        public IEnumerable<Post> GetPendingPosts()
        {
            return _postRepository.GetPendingPosts();
        }

        public IEnumerable<Post> GetPublishedPosts()
        {
            return _postRepository.GetPublishedPosts();
        }

        public IEnumerable<Post> GetPostsByWriter(int userId)
        {
            return _postRepository.GetPostsByWriter(userId);
        }

        public void CreatePost(Post post, int userId)
        {
            if (post == null)
            {
                throw new ApiException("Post object cannot be null.", (int)HttpStatusCode.BadRequest);
            }

            _postRepository.CreatePost(post, userId);
        }

        public void CorrectionPost(int postId, Post updatedPost)
        {
            _postRepository.CorrectionPost(postId, updatedPost);
        }

        public void UpdatePostStatus(int postId, PostStatus status)
        {
            if (postId <= 0)
            {
                throw new ApiException("Invalid post ID.", (int)HttpStatusCode.BadRequest);
            }

            if (!Enum.IsDefined(typeof(PostStatus), status))
            {
                throw new ApiException("Invalid post status.", (int)HttpStatusCode.BadRequest);
            }

            _postRepository.UpdatePostStatus(postId, status);
        }

        public void DeletePost(int postId)
        {
            if (postId <= 0)
            {
                throw new ApiException("Invalid post ID.", (int)HttpStatusCode.BadRequest);
            }

            _postRepository.DeletePost(postId);
        }
    }
}