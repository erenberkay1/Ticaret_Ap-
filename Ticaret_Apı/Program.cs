var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AtolyeIzni",
        policy =>
        {
            policy.AllowAnyOrigin()  
                  .AllowAnyHeader()  
                  .AllowAnyMethod(); 
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AtolyeIzni");
app.UseStaticFiles();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000"; 
app.Urls.Add($"http://*:{port}"); 
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();