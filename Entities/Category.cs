namespace MiniBlog.Entities;

public class Category
{
    public Category()
    {
        _idIterator++;
        Id = _idIterator;
    }
    public Category(string title, string body)
    {
        _idIterator++;
        Id = _idIterator;
        Title = title;
        Body = body;
    }
    public Category(Category category)
    {
        Id = category.Id;
        Title = category.Title;
        Body = category.Body;
        foreach (var item in category.Posts)
        {
            Posts.Add(item);
        }
    }
    public Category(string title, string body, List<Post> posts)
    {
        _idIterator++;
        Id = _idIterator;
        Title = title;
        Body = body;
        Posts = posts;
    }
    static int _idIterator;
    public int? Id { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
    public List<Post>? Posts = new();
}
