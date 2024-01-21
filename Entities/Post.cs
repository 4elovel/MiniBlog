namespace MiniBlog.Entities;

public class Post
{
    public Post()
    {
        _idIterator++;
        Id = _idIterator;
    }
    public Post(Post post)
    {
        Id = post.Id;
        Title = post.Title;
        Body = post.Body;
        foreach (var item in post.Categories)
        {
            Categories.Add(item);
        }
    }
    public Post(string title, string body)
    {
        _idIterator++;
        Id = _idIterator;
        Title = title;
        Body = body;
    }
    public Post(string title, string body, List<Category> categories)
    {
        _idIterator++;
        Id = _idIterator;
        Title = title;
        Body = body;
        Categories = categories;
    }
    static int _idIterator;
    public int? Id { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
    public List<Category>? Categories = new();
}
