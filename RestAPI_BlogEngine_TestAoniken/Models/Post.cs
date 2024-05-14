using System.ComponentModel.DataAnnotations;

namespace RestAPi_BlogEngine_TestAoniken.Models
{
    public class Post
    {
        public Post()
        {
            Title = string.Empty;
            Content = string.Empty;
            AuthorName = string.Empty;
        }

        public int Id { get; set; } //unique identifier for the post
        public int UserId { get; set; }  //blog´s UserId
       
        [Required]
        [MinLength(10)]
        public string Title { get; set; } //post tittle 


        [Required]
        [MinLength(20)]
        public string Content { get; set; } //post content

        [Required]
        public string AuthorName { get; set; } //post´s Author name
        
        public DateTime SubmitDate { get; set; } //submission date of the post
        public PostStatus Status { get; set; } //post´s status ( PendingApproval,Published,Rejected,Correction)

    }

    public enum PostStatus
    {
        PendingApproval,
        Published,
        Rejected,
        Correction

    }
}
