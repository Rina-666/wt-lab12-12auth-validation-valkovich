using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using wt_lab8_8razor_postgresql_valkovich.Data;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
    WebRootPath = "wwwroot"
});

// --- 1. Настройка подключения к PostgreSQL ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Логика для Render: если есть переменная DATABASE_URL, используем её
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrWhiteSpace(databaseUrl))
{
    if (databaseUrl.StartsWith("postgres://"))
    {
        databaseUrl = databaseUrl.Replace("postgres://", "postgresql://");
    }
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', 2);
    var dbPort = uri.Port > 0 ? uri.Port : 5432;

    connectionString = $"Host={uri.Host};Port={dbPort};Database={uri.AbsolutePath.TrimStart('/')};" +
                       $"Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Disable;";
}

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// --- 2. Настройка раздачи видеофайлов ---
builder.Services.Configure<StaticFileOptions>(options =>
{
    options.ServeUnknownFileTypes = true;
    options.DefaultContentType = "application/octet-stream";
});

var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".mp4"] = "video/mp4";
provider.Mappings[".webm"] = "video/webm";
builder.Services.AddSingleton<IContentTypeProvider>(provider);

// --- 3. Поддержка Razor Pages ---
builder.Services.AddRazorPages();

var app = builder.Build();

// --- 4. Автоматическое применение миграций при запуске ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

// --- 5. Запуск для Render ---
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}");