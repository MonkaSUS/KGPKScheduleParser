using Coravel;
using EasyCaching.Core;
using KGPKScheduleParser;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Parser>();
builder.Services.AddHttpClient();
builder.Services.AddScheduler();
builder.Services.AddEasyCaching(o =>
{
    o.UseInMemory(config =>
    {
        config.DBConfig = new()
        {
            SizeLimit = 1000,
            EnableReadDeepClone = true,
            EnableWriteDeepClone = false,
            ExpirationScanFrequency = 9999
        };
        config.MaxRdSecond = 254;
        config.LockMs = 5000;
        config.SleepMs = 321;
    }, "memoryCache");

});
var app = builder.Build();
app.UseHttpsRedirection();
Parser.GetAllTeachers();
List<string> cachedKeys = new();

//»сполнение кода по определЄнному графику с помощью Coravel, у них хороша€ документаци€.
app.Services.UseScheduler(s =>
{
    //¬ шесть часов утра каждый понедельник мы на вс€кий случай обновл€ем список преподавателей
    s.Schedule(() => Parser.GetAllTeachers())
    .DailyAtHour(6).Monday();
    //¬ дев€ть часов утра каждое воскресенье мы очищаем все записи из кеша. 
    //–асчЄт на то, что к этому времени уже должно быть новое раписание, и люди (пользователи(студенты)) будут интересоватьс€ своим расписанием на завтра.
    s.Schedule(async () =>
        {
            var prov = app.Services.GetService<IEasyCachingProvider>();
            await prov.RemoveAllAsync(cachedKeys);
        }).DailyAtHour(9).Sunday();
});
app.MapGet("schedule", async ([FromQuery(Name = "forGroup")] string groupname, IEasyCachingProvider prov) =>
{
    if (await prov.ExistsAsync(groupname)) //если в кеше существует запись дл€ искомой группы, вернуть еЄ, иначе спарсить
    {
        var cachedRes = await prov.GetAsync<ScheduleOfWeek>(groupname);
        return Results.Json(cachedRes, options: new System.Text.Json.JsonSerializerOptions()
        {
            IncludeFields = true,
            ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
        }, statusCode: 200);
    }
    var res = Parser.GetScheduleForGroup(groupname);
    await prov.SetAsync<ScheduleOfWeek>(groupname, res, TimeSpan.FromDays(6)); //перед возвратом расписани€ оно кешируетс€
    cachedKeys.Add(groupname);
    return Results.Json(res, options: new System.Text.Json.JsonSerializerOptions()
    {
        IncludeFields = true,
        ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
    }, statusCode: 200);

});
app.MapGet("/", () =>
{
    Parser.GetAllTeachers();
    return "hehehehe";
});
///
///  ороче, тема така€. Ётот запрос при первом исполнении занимает ~34-35 секунд. Ёто слишком много
/// ѕервый запрос дл€ каждой группы все ещЄ будет занимать столько времени, но потом результаты кешируютс€.
/// —рок годности кеша ради прикола поставил 6 дней, надо будет как-нибудь подумать о реальной жизни и помен€ть это значение.
/// –асчЄт на то, что расписание - объект неподвижный, не мен€етс€ посреди недели.
///  огда инфа берЄтс€ из кеша, выполнение запроса занимает меньше одной секунды. ѕо-моему это очень круто.
///
app.Run();

