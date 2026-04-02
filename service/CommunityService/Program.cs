using Tudormobile.CommunityService;
using Tudormobile.Dbx;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy for local testing and specific domains
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins(
                "https://www.tudormobile.com",
                "https://www.tudorzone.com",
                "https://tudormobile.com",
                "https://tudorzone.com",
                "https://localhost:5162",
                "http://localhost:5162")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add Dbx and authorization for running under TudormoibleAPI and Dbx for storage.
builder.Services.AddAuthorization();
builder.Services.AddDbx();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");
app.UseAuthorization();

// map for use in the Tudormobile API host (testing purposes)
app.UseCommunityService();

app.Run();
