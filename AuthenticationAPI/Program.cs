using AuthenticationAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

#region Add services to the container
builder.Services.AddControllers();

// Services Extensions
builder.Services.AddDbContextServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddAuthorizationServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
#endregion

var app = builder.Build();

#region Add Application Middleware
// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

// Authentication & Authorization Middleware.
app.UseAuthentication();

// CORS
app.UseCors();

app.UseAuthorization();

app.MapControllers();
#endregion

//Prepare Application before Run
app.Prepare(builder.Configuration);

app.Run();
