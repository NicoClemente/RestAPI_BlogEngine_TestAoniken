using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPi_BlogEngine_TestAoniken.Exceptions;
using RestAPi_BlogEngine_TestAoniken.Models;
using RestAPi_BlogEngine_TestAoniken.Repositories;
using RestAPi_BlogEngine_TestAoniken.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace RestAPI_BlogEngine_TestAoniken.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService; // Service for post-related operations
        private readonly AuthService _authService; // Service for authentication-related operations
        private readonly IUserRepository _userRepository; // Repository for user-related data operations


        // Constructor to initialize the controller with the necessary services and repositories
        public PostController(PostService postService, AuthService authService, IUserRepository userRepository)
        {
            _postService = postService;
            _authService = authService;
            _userRepository = userRepository;
            _userRepository = userRepository;
        }

        // Helper method to get the user ID of the authenticated user
        private int GetUserIdFromAuthenticatedUser()
        {
                var username = User.Identity.Name;
                var user = _userRepository.GetUserByUsername(username);
                return user.Id;
                       
        }

        //Post Endpoints

        [HttpPost]
        [Authorize(Roles = "Writer")]
        [SwaggerOperation(
            Summary = "Create a new post by an authenticated Writer",
            Description = "This endpoint allows Writers to create a new post. The post will be created with a 'PendingApproval' status and associated with the Writer's ID."
        )]
        [SwaggerResponse(StatusCodes.Status201Created, "Returns the created post object.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "If the post object is null or invalid.")]
        public ActionResult<Post> CreatePost([FromBody] Post post)
        {
            if (post == null)
            {
                throw new ApiException("Post object cannot be null.", (int)HttpStatusCode.BadRequest);
            }

            if (!ModelState.IsValid)
            {
                throw new ApiException("Invalid post data.", (int)HttpStatusCode.BadRequest);
            }

            var userId = GetUserIdFromAuthenticatedUser();
            post.UserId = userId;

            _postService.CreatePost(post, userId);
            return CreatedAtAction(nameof(GetPendingPosts), new { id = post.Id }, post);
        }


        [HttpGet("my-posts")]
        [Authorize(Roles = "Writer")]
        [SwaggerOperation(
           Summary = "Get posts created by the authenticated Writer",
           Description = "This endpoint allows Writers to retrieve a list of posts they have created, including pending, published, and rejected posts."
       )]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns a list of posts created by the authenticated Writer.")]
        public ActionResult<IEnumerable<Post>> GetPostsByWriter()
        {
            var userId = _authService.GetUserId(User.Identity.Name);
            var writerPosts = _postService.GetPostsByWriter(userId);
            return Ok(writerPosts);
        }


        [HttpPut("Correction/{postId}")]
        [Authorize(Roles = "Writer")]
        [SwaggerOperation(
            Summary = "Update a rejected post by the authenticated Writer ",
            Description = "This endpoint allows Writers to update a post that was previously rejected (in 'Correction' status). The post's SubmitDate will be automatically updated to the current date and time."
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "If the post was updated successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "If the post object is null or invalid.")]
        public ActionResult CorrectionPost(int postId, [FromBody] Post updatedPost)
        {
            if (updatedPost == null)
            {
                throw new ApiException("Post object cannot be null.", (int)HttpStatusCode.BadRequest);
            }

            if (!ModelState.IsValid)
            {
                throw new ApiException("Invalid post data.", (int)HttpStatusCode.BadRequest);
            }

            var userId = _authService.GetUserId(User.Identity.Name);
            updatedPost.UserId = userId;

            _postService.CorrectionPost(postId, updatedPost);
            return NoContent();
        }


        [HttpGet("pending")]
        [Authorize(Roles = "Editor")]
        [SwaggerOperation(
            Summary = "Get posts pending for approval by an authenticated Editor",
            Description = "This endpoint allows Editors to retrieve a list of posts with the 'PendingApproval' status."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns a list of posts pending for approval.")]
        public ActionResult<IEnumerable<Post>> GetPendingPosts()
        {
            var pendingPosts = _postService.GetPendingPosts();
            return Ok(pendingPosts);
        }


        [HttpPut("{postId}/status/{status}")]
        [Authorize(Roles = "Editor")]
        [SwaggerOperation(
            Summary = "Update the status of a post by an authenticated Editor",
            Description = "This endpoint allows Editors to update the status of a post to 'Published' or 'Correction'."
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "If the post status was updated successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "If the post ID or status is invalid.")]
        public ActionResult UpdatePostStatus(int postId, PostStatus status)
        {
            _postService.UpdatePostStatus(postId, status);
            return NoContent();
        }


        [Authorize(Roles = "Editor")]
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete a post by an authenticated Editor",
            Description = "This endpoint allows Editors to delete a post."
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "If the post was deleted successfully.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "If the post with the specified ID was not found.")]
        public ActionResult DeletePost(int id)
        {
            _postService.DeletePost(id);
            return NoContent();
        }


        [HttpGet("published")]
        [AllowAnonymous]
        [SwaggerOperation(
           Summary = "Get published posts by any User and NonUser",
           Description = "This endpoint allows anonymous users to retrieve a list of published posts."
       )]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns a list of published posts.")]
        public IActionResult GetPublishedPosts()
        {
            var publishedPosts = _postService.GetPublishedPosts();
            return Ok(publishedPosts);
        }
    }
}