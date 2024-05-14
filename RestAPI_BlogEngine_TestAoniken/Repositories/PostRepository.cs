using RestAPi_BlogEngine_TestAoniken.Models;
using CsvHelper;
using System.Globalization;
using RestAPi_BlogEngine_TestAoniken.Exceptions;
using System.Net;

namespace RestAPi_BlogEngine_TestAoniken.Repositories
{
    // PostRepository class for handling CRUD operations on Post data stored in a CSV file.
    public class PostRepository
    {

        private readonly string _csvFilePath;  //path to the csv file where the posts are stored

        //initialize the csv file path
        public PostRepository()
        {
            string projectDirectory = Directory.GetCurrentDirectory();
            string csvDirectory = Path.Combine(projectDirectory, "Data");
            _csvFilePath = Path.Combine(csvDirectory, "Posts.csv");
        }

        //create a new post and add it to the csv file
        public void CreatePost(Post post, int userId)
        {
            List<Post> posts;

            if (!File.Exists(_csvFilePath))
            {
                posts = new List<Post>();
            }
            else
            {
                using (var reader = new StreamReader(_csvFilePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    posts = csv.GetRecords<Post>().ToList();
                }
            }

            post.Id = posts.Any() ? posts.Max(p => p.Id) + 1 : 1;
            post.SubmitDate = DateTime.UtcNow;
            post.Status = PostStatus.PendingApproval;
            post.UserId = userId;
            posts.Add(post);

            SavePosts(posts);
        }

        // Update an existing post that is in "Correction" status.
        public void CorrectionPost(int postId, Post updatedPost)
        {
            var posts = LoadPosts();
            var existingPost = posts.FirstOrDefault(p => p.Id == postId);

            if (existingPost == null)
            {
                throw new ApiException($"Post with ID {postId} does not exist.", (int)HttpStatusCode.NotFound);
            }

            if (existingPost.Status != PostStatus.Correction)
            {
                throw new ApiException($"Cannot update post with ID {postId} because its status is not 'Correction'.", (int)HttpStatusCode.BadRequest);
            }

            existingPost.Title = updatedPost.Title;
            existingPost.Content = updatedPost.Content;
            existingPost.AuthorName = updatedPost.AuthorName;
            existingPost.SubmitDate = DateTime.UtcNow;
            existingPost.Status = updatedPost.Status;

            SavePosts(posts);
        }

        //update the status of the post in the csv file
        public void UpdatePostStatus(int postId, PostStatus status)
        {
            var posts = LoadPosts();
            var post = posts.FirstOrDefault(p => p.Id == postId);

            if (post == null)
            {
                throw new ApiException($"Post with ID {postId} does not exist.", (int)HttpStatusCode.NotFound);
            }

            if (status == PostStatus.Published && post.Status != PostStatus.PendingApproval)
            {
                throw new ApiException($"Cannot publish post with ID {postId} because its status is not 'PendingApproval'.", (int)HttpStatusCode.BadRequest);
            }

            if (status == PostStatus.Rejected && post.Status != PostStatus.PendingApproval)
            {
                throw new ApiException($"Cannot reject post with ID {postId} because its status is not 'PendingApproval'.", (int)HttpStatusCode.BadRequest);
            }

            if (status == PostStatus.Published)
            {
                post.Status = PostStatus.Published;
            }
            else if (status == PostStatus.Rejected)
            {
                post.Status = PostStatus.Correction;
            }

            SavePosts(posts);
        }

        //get posts pending for approval from the csv file
        public IEnumerable<Post> GetPendingPosts()
        {
            if (!File.Exists(_csvFilePath))
            {
                throw new ApiException("The posts file does not exist.", (int)HttpStatusCode.NotFound);
            }

            using var reader = new StreamReader(_csvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Read();
            csv.ReadHeader();
            var records = csv.GetRecords<Post>().Where(p => p.Status == PostStatus.PendingApproval).ToList();
            return records;
        }

        // Get posts by a specific writer from the CSV file.
        public IEnumerable<Post> GetPostsByWriter(int writerId)
        {
            using var reader = new StreamReader(_csvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Read();
            csv.ReadHeader();
            var records = csv.GetRecords<Post>().Where(p => p.UserId == writerId).ToList();
            return records;
        }

        // Get published posts from the CSV file.
        public IEnumerable<Post> GetPublishedPosts()
        {
            if (!File.Exists(_csvFilePath))
            {
                throw new ApiException("The posts file does not exist.", (int)HttpStatusCode.NotFound);
            }

            using var reader = new StreamReader(_csvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Read(); 
            csv.ReadHeader(); 
            var records = csv.GetRecords<Post>().Where(p => p.Status == PostStatus.Published).ToList();
            return records;
        }

        // Delete a post from the CSV file.
        public void DeletePost(int postId)
        {
            var posts = LoadPosts();
            var post = posts.FirstOrDefault(p => p.Id == postId);

            if (post == null)
            {
                throw new ApiException($"Post with ID {postId} does not exist.", (int)HttpStatusCode.NotFound);
            }

            posts.Remove(post);
            SavePosts(posts);
        }

        // Load all posts from the CSV file.
        private List<Post> LoadPosts()
        {
            if (!File.Exists(_csvFilePath))
            {
                throw new ApiException("The posts file does not exist.", (int)HttpStatusCode.NotFound);
            }

            var posts = new List<Post>();
            using (var reader = new StreamReader(_csvFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                posts = csv.GetRecords<Post>().ToList();
            }
            return posts;
        }

        // Save all posts to the CSV file.
        private void SavePosts(List<Post> posts)
        {
            using var writer = new StreamWriter(_csvFilePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(posts);
        }
    }
}