using Showmax.Server.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Showmax.Server.Data;
using AppUser = Showmax.Shared.Models.User;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<CloudinaryService>();

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Add CORS (allow Blazor client to talk to API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "http://localhost:7062",
                "https://localhost:7062"
               
              )
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 524288000; // 500MB
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 524288000; // 500MB
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Seed sample content
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Update existing thumbnails
    var allContent = context.Contents.ToList();
    foreach (var item in allContent)
    {
        item.ThumbnailUrl = item.Title switch
        {
            "The Crown" => "https://picsum.photos/seed/crown/300/450",
            "Black Panther" => "https://picsum.photos/seed/panther/300/450",
            "Squid Game" => "https://picsum.photos/seed/squid/300/450",
            "Coming 2 America" => "https://picsum.photos/seed/america/300/450",
            _ => item.ThumbnailUrl
        };
    }
    await context.SaveChangesAsync();

    // Add new content if not exists
    if (!context.Contents.Any(c => c.Title == "The Crown"))
    {
        context.Contents.AddRange(
            new Showmax.Shared.Models.Content
            {
                Title = "The Crown",
                Synopsis = "The story of Queen Elizabeth II and the political and personal events that shaped her reign.",
                Type = "Series",
                Genre = "Drama",
                Language = "English",
                ReleaseYear = 2016,
                Rating = "16+",
                ThumbnailUrl = "https://picsum.photos/seed/crown/300/450",
                VideoUrl = ""
            },
            new Showmax.Shared.Models.Content
            {
                Title = "Black Panther",
                Synopsis = "T'Challa returns home to Wakanda to take his place as king.",
                Type = "Movie",
                Genre = "Action",
                Language = "English",
                ReleaseYear = 2018,
                Rating = "13+",
                ThumbnailUrl = "https://picsum.photos/seed/panther/300/450",
                VideoUrl = ""
            },
            new Showmax.Shared.Models.Content
            {
                Title = "Squid Game",
                Synopsis = "Hundreds of cash-strapped players compete in children's games for a prize.",
                Type = "Series",
                Genre = "Thriller",
                Language = "Korean",
                ReleaseYear = 2021,
                Rating = "18+",
                ThumbnailUrl = "https://picsum.photos/seed/squid/300/450",
                VideoUrl = ""
            },
            new Showmax.Shared.Models.Content
            {
                Title = "Coming 2 America",
                Synopsis = "African Prince Akeem learns he has a son in America.",
                Type = "Movie",
                Genre = "Comedy",
                Language = "English",
                ReleaseYear = 2021,
                Rating = "13+",
                ThumbnailUrl = "https://picsum.photos/seed/america/300/450",
                VideoUrl = ""
            }
        );
        await context.SaveChangesAsync();
    }

}

// Seed Admin Role
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    if (!await roleManager.RoleExistsAsync("User"))
        await roleManager.CreateAsync(new IdentityRole("User"));

    // Create default admin account
    var adminEmail = "admin@showmax.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new AppUser
        {
            FullName = "Admin",
            Email = adminEmail,
            UserName = adminEmail
        };
        await userManager.CreateAsync(adminUser, "Admin@1234");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}



//app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();