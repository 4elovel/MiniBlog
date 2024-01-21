using MiniBlog.Entities;
using System.Text.RegularExpressions;

namespace MiniBlog;

public class Program
{
    public static void DeletePostById(int id, ref List<Category> cls, ref List<Post> pls)
    {

        int idx = pls.IndexOf(GetPostById(id, pls));
        var cats = pls[idx].Categories;
        var clsCopy = new List<Category>(cls);
        foreach (var i in clsCopy)
        {
            foreach (var j in cats)
            {
                if (j.Title == i.Title)
                {
                    int clsidx = cls.IndexOf(i);
                    cls[clsidx].Posts.Remove(GetPostById(id, cls[clsidx].Posts));
                }
            }
        }
        pls[idx].Categories.Clear();
        pls.Remove(GetPostById(id, pls));
    }
    public static void DeleteCategoryById(int id, ref List<Post> pls, ref List<Category> cls)//bad naming
    {

        int idx = cls.IndexOf(GetCategoryById(id, cls));
        var cats = cls[idx].Posts;
        var plsCopy = new List<Post>(pls);
        foreach (var i in plsCopy)
        {
            foreach (var j in cats)
            {
                if (j.Title == i.Title)
                {
                    int plsidx = pls.IndexOf(i);
                    pls[plsidx].Categories.Remove(GetCategoryById(id, pls[plsidx].Categories));
                }
            }
        }
        cls[idx].Posts.Clear();
        cls.Remove(GetCategoryById(id, cls));
    }
    public static void NewCatToPost(ref List<Category> cls, ref List<string> newCategory, ref Post newPost)
    {
        bool leaver = false;

        newPost.Categories.Clear();
        var ListCatCopy = new List<Category>(cls);
        for (int i = 0; i < newCategory.Count; i++)
        {
            newCategory[0] = newCategory[0].Replace(" ", "");
            newCategory[0] = newCategory[0].Replace(",", "");
            newCategory[0] = newCategory[0].Replace(".", "");
        }
        foreach (string category in newCategory)
        {
            leaver = false;
            foreach (var i in ListCatCopy)
            {
                if (i.Title == category)
                {

                    newPost.Categories.Add(i);
                    i.Posts.Add(newPost);
                    leaver = true;
                    break;
                }
            }
            if (leaver) continue;
            cls.Add(new Category { Title = category });
            cls.Last().Posts.Add(newPost);
            newPost.Categories.Add(cls.Last());

        }
    }
    public static Post GetPostById(int PostId, List<Post> pls)
    {
        foreach (var i in pls)
        {
            if (i.Id == PostId) return i;
        }
        return null;
    }
    public static Category GetCategoryById(int CatId, List<Category> cls)
    {
        foreach (var i in cls)
        {
            if (i.Id == CatId) return i;
        }
        return null;
    }
    public static void NewPostToCat(ref List<Post> pls, ref List<string> newPosts, ref Category newCat)
    {
        bool leaver = false;

        newCat.Posts.Clear();
        var ListPostCopy = new List<Post>(pls);
        for (int i = 0; i < newPosts.Count; i++)
        {
            newPosts[0] = newPosts[0].Replace(" ", "");
            newPosts[0] = newPosts[0].Replace(",", "");
            newPosts[0] = newPosts[0].Replace(".", "");
        }
        foreach (string post in newPosts)
        {
            leaver = false;
            foreach (var i in ListPostCopy)
            {
                if (i.Title == post)
                {

                    newCat.Posts.Add(i);
                    i.Categories.Add(newCat);
                    leaver = true;
                    break;
                }
            }
            if (leaver) continue;
            pls.Add(new Post { Title = post });
            pls.Last().Categories.Add(newCat);
            newCat.Posts.Add(pls.Last());

        }
    }

    public static string PostToString(Post post)
    {
        string res = "";
        res += $"{post.Id} {post.Title} - {post.Body} ";
        if (post.Categories.Count == 0) return res;
        res += "{";
        foreach (var category in post.Categories)
        {
            res += $"{category.Title}, ";
        }
        res = res.Remove(res.LastIndexOf(res.Last()));
        res = res.Remove(res.LastIndexOf(res.Last()));
        res += "}\n";

        return res;
    }
    public static string CategoryToString(Category category)
    {
        string res = "";

        res += $"{category.Id} {category.Title} - {category.Body} ";
        if (category.Posts.Count == 0) return res;
        res += "{";
        foreach (var post in category.Posts)
        {
            res += $"{post.Title}, ";
        }
        res = res.Remove(res.LastIndexOf(res.Last()));
        res = res.Remove(res.LastIndexOf(res.Last()));
        res += "}\n";

        return res;
    }

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();
        var wwwroot = Directory.GetCurrentDirectory() + "\\wwwroot\\";
        List<Post> posts = new List<Post> { new Post("Anime", "Perfect solution"), new Post("Serial", "Well oportunity"), new Post("Film", "Good option") };
        List<Category> categories = new List<Category> { new Category("Multiplication","can be cartoon", new List<Post> { posts[0] }),
        new Category("Series","have many series", new List<Post> {posts[0], posts[1] }),
        new Category("Watch","can be watched", new List<Post> { posts[0], posts[1], posts[2] }) };
        posts.ForEach(p => { p.Categories.Add(categories[2]); });
        posts[0].Categories.Add(categories[0]);
        posts[0].Categories.Add(categories[1]);
        posts[1].Categories.Add(categories[1]);


        app.MapGet("/", () => "Hello World!");
        app.MapGet("/about", async (context) =>
        {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync($"{wwwroot}about.html");
        });
        app.MapGet("/posts/q", async (HttpContext context, string title) =>
        {

            if (title != null && title != "")
                foreach (var i in posts)
                {
                    if (i.Title == title) await context.Response.WriteAsync(PostToString(i));
                }
        });
        app.MapGet("/posts", async (context) =>
        {
            foreach (var post in posts) await context.Response.WriteAsync(PostToString(post));
        });
        app.MapGet("/posts/add", async (context) =>
        {

            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync($"{wwwroot}addPost.html");
        });
        app.MapGet("/posts/delete", async (context) =>
        {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync($"{wwwroot}deletePost.html");
            foreach (var post in posts)
            {
                context.Response.WriteAsync("<br><br>");
                await context.Response.WriteAsync(PostToString(post));
                context.Response.WriteAsync("<br><br>");
            };

        });
        app.MapPost("/posts/delete", async (context) =>
        {

            string id = context.Request.Form["id"];
            int idx = posts.IndexOf(GetPostById(Int16.Parse(id), posts));
            if (idx != -1) DeletePostById(Int16.Parse(id), ref categories, ref posts);

            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync($"{wwwroot}deletePost.html");
            foreach (var post in posts)
            {
                context.Response.WriteAsync("<br><br>");
                await context.Response.WriteAsync(PostToString(post));
                context.Response.WriteAsync("<br><br>");
            };
        });
        app.MapGet("/categories/delete", async (context) =>
        {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync($"{wwwroot}deleteCategory.html");
            foreach (var category in categories)
            {
                context.Response.WriteAsync("<br><br>");
                await context.Response.WriteAsync(CategoryToString(category));
                context.Response.WriteAsync("<br><br>");
            };

        });
        app.MapPost("/categories/delete", async (context) =>
        {

            string id = context.Request.Form["id"];
            int idx = categories.IndexOf(GetCategoryById(Int16.Parse(id), categories));
            if (idx != -1) DeleteCategoryById(Int16.Parse(id), ref posts, ref categories);

            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync($"{wwwroot}deleteCategory.html");
            foreach (var category in categories)
            {
                context.Response.WriteAsync("<br><br>");
                await context.Response.WriteAsync(CategoryToString(category));
                context.Response.WriteAsync("<br><br>");
            };
        });
        app.MapPost("/posts/add", async (context) =>
        {
            string title = context.Request.Form["title"];
            string body = context.Request.Form["body"];
            string cats = context.Request.Form["categories"];
            string regexPattern = @"^[A-Za-z0-9,. ]+$";
            if (Regex.IsMatch(title, regexPattern) && Regex.IsMatch(cats, regexPattern) && title != null && body != null)
            {
                List<string> categoryls = new List<string>(cats.Split(','));
                Post newPost = new Post { Title = title, Body = body };

                NewCatToPost(ref categories, ref categoryls, ref newPost);
                posts.Add(newPost);

                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync($"{wwwroot}addPost.html");
            }

        });
        app.MapGet("/posts/edit", async (context) =>
        {

            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync($"{wwwroot}editPost.html");
        });
        app.MapPost("/posts/edit", async (context) =>
        {
            string id = context.Request.Form["id"];
            string title = context.Request.Form["title"];
            string body = context.Request.Form["body"];
            string cats = context.Request.Form["categories"];
            string regexPattern = @"^[A-Za-z0-9,. ]+$";
            Post OldPost = new Post(GetPostById(Int16.Parse(id), posts));
            if ((Regex.IsMatch(title, regexPattern) || title == "") && (Regex.IsMatch(cats, regexPattern) || cats == "") && OldPost != null)
            {
                if (title != "") OldPost.Title = title;
                if (body != "") OldPost.Body = body;
                DeletePostById(Int16.Parse(id), ref categories, ref posts);
                if (cats != "")
                {
                    List<string> categoryls = new List<string>(cats.Split(','));// To Add
                    NewCatToPost(ref categories, ref categoryls, ref OldPost);

                }
            }
            posts.Add(OldPost);
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync($"{wwwroot}editPost.html");
        });
        app.MapGet("/categories/edit", async (context) =>
        {

            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync($"{wwwroot}editCategory.html");
        });
        app.MapPost("/categories/edit", async (context) =>
        {
            string id = context.Request.Form["id"];
            string title = context.Request.Form["title"];
            string body = context.Request.Form["body"];
            string catPosts = context.Request.Form["posts"];
            string regexPattern = @"^[A-Za-z0-9,. ]+$";
            Category OldCat = new Category(GetCategoryById(Int16.Parse(id), categories));
            if ((Regex.IsMatch(title, regexPattern) || title == "") && (Regex.IsMatch(catPosts, regexPattern) || catPosts == "") && OldCat != null)
            {
                if (title != "") OldCat.Title = title;
                if (body != "") OldCat.Body = body;
                DeleteCategoryById(Int16.Parse(id), ref posts, ref categories);
                if (catPosts != "")
                {
                    List<string> postls = new List<string>(catPosts.Split(','));// To Add
                    NewPostToCat(ref posts, ref postls, ref OldCat);

                }
            }
            categories.Add(OldCat);
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync($"{wwwroot}editCategory.html");
        });

        app.MapGet("/categories/add", async (context) =>
        {

            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync($"{wwwroot}addCategory.html");
        });
        app.MapPost("/categories/add", async (context) =>
        {
            string title = context.Request.Form["title"];
            string body = context.Request.Form["body"];
            string formPosts = context.Request.Form["posts"];
            string regexPattern = @"^[A-Za-z0-9,. ]+$";
            if (Regex.IsMatch(title, regexPattern) && Regex.IsMatch(formPosts, regexPattern) && title != null && body != null)
            {
                List<string> categoryls = new List<string>(formPosts.Split(','));
                Category newCategory = new Category { Title = title, Body = body };

                NewPostToCat(ref posts, ref categoryls, ref newCategory);
                categories.Add(newCategory);

                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync($"{wwwroot}addCategory.html");
            }

        });

        app.MapGet("/posts/{id:int}", (HttpContext context, int id) =>
        {
            var bi = context.Request.Path;
            foreach (var post in posts)
            {
                if (post.Id == id)
                {
                    string buf = post.Body.Replace(" ", "-");
                    context.Response.Redirect($"/posts/{id}/{buf}");
                }

            }

        });
        app.MapGet("/posts/{id:int}/{slug}", (HttpContext context, int id, string slug) =>
        {
            foreach (var post in posts)
            {
                if (post.Body.Replace(" ", "-") == slug)
                {
                    string buf = post.Body.Replace(" ", "-");
                    return PostToString(post);
                }
            }
            return "";
        });
        app.MapGet("/posts/{categoryName}", (HttpContext context, string categoryName) =>
        {
            string res = "";
            foreach (var post in posts)
            {
                post.Categories.ForEach(c => { if (c.Title == categoryName) { res += PostToString(post); } });
            }
            return res;
        });
        app.MapGet("/categories", async (context) =>
        {
            context.Response.ContentType = "html";
            string res = "";
            foreach (var category in categories)
            {
                res += $"<div>{category.Id} <a href=\"http://localhost:5262/posts/{category.Title}\"> {category.Title}</a> {category.Body}</div><br>\n";
            }
            await context.Response.WriteAsync(res);
        });



        app.Run();
    }
}
