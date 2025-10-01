namespace SynQcore.BlazorApp.Models
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<string> Tags { get; set; } = new();
        public int Likes { get; set; }
        public List<CommentModel> Comments { get; set; } = new();
    }

    public class CommentModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
