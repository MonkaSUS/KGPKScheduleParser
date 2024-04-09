using Coravel;
using KGPKScheduleParser;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSingleton<Parser>();
builder.Services.AddScheduler();

var app = builder.Build();
Parser.GetAllTeachers();
app.Services.UseScheduler(s =>
{
    s.Schedule(() => Parser.GetAllTeachers())
    .Daily();
});
app.MapGet("schedule", async ([FromQuery(Name = "group")] string groupname) =>
{
    var res = Parser.GetScheduleForGroup(groupname);
    return Results.Json(res, options: new System.Text.Json.JsonSerializerOptions()
    {
        IncludeFields = true,
        ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
    }, statusCode: 200);

});
app.Run();

