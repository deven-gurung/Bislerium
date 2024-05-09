using System.Net;

namespace SocialFeed.Models;

public abstract class BaseClass<T>
{
    public HttpStatusCode StatusCode { get; set; }

    public string Status { get; set; }

    public string Message { get; set; }

    public int? TotalCount { get; set; }
    
    public T Result { get; set; }
}
