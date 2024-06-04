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

//���������� ���� �� ������������ ������� � ������� Coravel, � ��� ������� ������������.
app.Services.UseScheduler(s =>
{
    //� ����� ����� ���� ������ ����������� �� �� ������ ������ ��������� ������ ��������������
    s.Schedule(() => Parser.GetAllTeachers())
    .DailyAtHour(6).Monday();
    //� ������ ����� ���� ������ ����������� �� ������� ��� ������ �� ����. 
    //������ �� ��, ��� � ����� ������� ��� ������ ���� ����� ���������, � ���� (������������(��������)) ����� �������������� ����� ����������� �� ������.
    s.Schedule(async () =>
        {
            var prov = app.Services.GetService<IEasyCachingProvider>();
            await prov.RemoveAllAsync(cachedKeys);
        }).DailyAtHour(9).Sunday();
});
app.MapGet("schedule", async ([FromQuery(Name = "forGroup")] string groupname, IEasyCachingProvider prov) =>
{
    if (await prov.ExistsAsync(groupname)) //���� � ���� ���������� ������ ��� ������� ������, ������� �, ����� ��������
    {
        var cachedRes = await prov.GetAsync<ScheduleOfWeek>(groupname);
        return Results.Json(cachedRes, options: new System.Text.Json.JsonSerializerOptions()
        {
            IncludeFields = true,
            ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
        }, statusCode: 200);
    }
    var res = Parser.GetScheduleForGroup(groupname);
    await prov.SetAsync<ScheduleOfWeek>(groupname, res, TimeSpan.FromDays(6)); //����� ��������� ���������� ��� ����������
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
/// ������, ���� �����. ���� ������ ��� ������ ���������� �������� ~34-35 ������. ��� ������� �����
/// ������ ������ ��� ������ ������ ��� ��� ����� �������� ������� �������, �� ����� ���������� ����������.
/// ���� �������� ���� ���� ������� �������� 6 ����, ���� ����� ���-������ �������� � �������� ����� � �������� ��� ��������.
/// ������ �� ��, ��� ���������� - ������ �����������, �� �������� ������� ������.
/// ����� ���� ������ �� ����, ���������� ������� �������� ������ ����� �������. ��-����� ��� ����� �����.
///
app.Run();

