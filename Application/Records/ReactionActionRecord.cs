namespace Application.Records;

public class ReactionActionRecord
{
    public Guid? BlogId { get; set; }

    public Guid? CommentId { get; set; }

    public int? ReactionId { get; set; }

    public string? Comment { get; set; }
}