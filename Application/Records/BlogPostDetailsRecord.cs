namespace Application.Records;

public record BlogPostDetailsRecord : UserActions
{
    public Guid BlogId { get; init; }
        
    public string Title { get; init; }
        
    public string Body { get; init; }
        
    public string UploadedTimePeriod { get; init; }
        
    public bool IsEdited { get; init; }
        
    public int PopularityPoints { get; init; }
        
    public DateTime CreatedAt { get; init; }
        
    public List<string> Images { get; init; }
        
    public List<PostComments> Comments { get; init; }
}

public record PostComments : UserActions
{
    public Guid CommentId { get; init; }
        
    public string ImageUrl { get; init; }
        
    public string CommentedBy { get; init; }
        
    public string CommentedTimePeriod { get; init; }
        
    public string Comment { get; init; }
        
    public bool IsUpdated { get; init; }
        
    public List<PostComments> Comments { get; init; }
}

public record UserActions
{
    public int UpVotes { get; init; }
        
    public int DownVotes { get; init; }
        
    public bool IsUpVotedByUser { get; init; }
        
    public bool IsDownVotedByUser { get; init; }
}