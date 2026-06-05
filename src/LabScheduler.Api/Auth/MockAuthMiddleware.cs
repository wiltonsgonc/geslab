using System.Text;
using System.Text.Json;

namespace LabScheduler.Api.Auth
{
    public class MockAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Dictionary<string, (string Name, string Role)> _users = new()
        {
            ["admin"] = ("Administrador", "Admin"),
            ["lab"] = ("Técnico de Laboratório", "Lab"),
            ["viewer"] = ("Visualizador", "Viewer")
        };

        public MockAuthMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/auth/login", StringComparison.OrdinalIgnoreCase)
                && context.Request.Method == "POST")
            {
                var body = await JsonSerializer.DeserializeAsync<JsonElement>(context.Request.Body);
                var username = body.GetProperty("username").GetString() ?? "";
                var password = body.GetProperty("password").GetString() ?? "";

                if (_users.TryGetValue(username, out var user) && password == "lab123")
                {
                    var payload = JsonSerializer.Serialize(new { username, name = user.Name, role = user.Role });
                    var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));

                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        token,
                        name = user.Name,
                        username,
                        role = user.Role
                    }));
                    return;
                }

                context.Response.StatusCode = 401;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Credenciais inválidas" }));
                return;
            }

            if (context.Request.Path.StartsWithSegments("/api/auth/me", StringComparison.OrdinalIgnoreCase))
            {
                if (TryGetUser(context, out var userData))
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(userData));
                    return;
                }

                context.Response.StatusCode = 401;
                return;
            }

            await _next(context);
        }

        public static bool TryGetUser(HttpContext context, out (string username, string name, string role)? user)
        {
            user = null;
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader?.StartsWith("Bearer ") != true) return false;

            try
            {
                var token = authHeader["Bearer ".Length..];
                var json = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var data = JsonSerializer.Deserialize<JsonElement>(json);

                user = (
                    data.GetProperty("username").GetString() ?? "",
                    data.GetProperty("name").GetString() ?? "",
                    data.GetProperty("role").GetString() ?? ""
                );
                return true;
            }
            catch { return false; }
        }
    }
}
