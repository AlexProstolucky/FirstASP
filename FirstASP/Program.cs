internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<ViewCounter>();

        var app = builder.Build();
        app.MapGet("/", () => "Main, Hi");
        app.MapGet("/counter", (ViewCounter counter) => counter.GetViewCount());

        app.MapGet("/current-date", () => DateTime.Now.ToShortDateString());
        app.MapGet("/current-time", () => DateTime.Now.ToShortTimeString());

        app.MapGet("/html-objects", (Microsoft.AspNetCore.Http.HttpContext context) =>
        {
            context.Response.ContentType = "text/html";
            return GetHtmlWithObjects();
        });

        app.MapGet("/headers", (Microsoft.AspNetCore.Http.HttpContext context) => GetHeaders(context));

        app.MapGet("/profile", (string name, string surname, string characteristic1, string characteristic2, string characteristic3, string characteristic4, string characteristic5) =>
            GetProfile(name, surname, characteristic1, characteristic2, characteristic3, characteristic4, characteristic5));

        app.Run();
    }


    public class ViewCounter
    {
        private int _count = 0;

        public string GetViewCount()
        {
            _count++;
            return $"View count incremented. Total Views: {_count}";
        }
    }

    private static string GetHtmlWithObjects()
    {
        List<string> objects = new List<string>();
        for (int i = 1; i <= 10; i++)
        {
            objects.Add($"<li>Object {i}</li>");
        }

        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <title>Objects Page</title>
        </head>
        <body>
            <h1>List of Objects</h1>
            <ul>
                {string.Join("", objects)}
            </ul>
        </body>
        </html>";
    }

    private static string GetHeaders(Microsoft.AspNetCore.Http.HttpContext context)
    {
        var headers = context.Request.Headers;
        List<string> headerLines = new List<string>();

        foreach (var header in headers)
        {
            headerLines.Add($"{header.Key}: {header.Value}");
        }

        return string.Join("\n", headerLines);
    }
    // profile?name=John&surname=Doe&characteristic1=Value1&characteristic2=Value2&characteristic3=Value3&characteristic4=Value4&characteristic5=Value5

    private static string GetProfile(string name, string surname, string characteristic1, string characteristic2, string characteristic3, string characteristic4, string characteristic5)
    {
        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(surname))
        {
            var profile = new
            {
                Name = name,
                Surname = surname,
                Characteristic1 = characteristic1,
                Characteristic2 = characteristic2,
                Characteristic3 = characteristic3,
                Characteristic4 = characteristic4,
                Characteristic5 = characteristic5
            };

            return System.Text.Json.JsonSerializer.Serialize(profile);
        }
        else
        {
            return "Please provide both name and surname in the query parameters.";
        }
    }
}