using Azure.Search.Documents.Models;
using Core_AISearch_Code.Models;
using Core_AISearch_Code.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<SearchServiceManager>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

 

app.MapGet("/index", () =>
{
    
    var searchServiceManager = app.Services.GetRequiredService<SearchServiceManager>();
    searchServiceManager.CreateIndex();
    return Results.Ok("Index created successfully.");
});


app.MapGet("/search", (string searchText) =>
{

    var searchServiceManager = app.Services.GetRequiredService<SearchServiceManager>();
    var results = searchServiceManager.Search(searchText);
    var output = new List<SearchDocument>();
    foreach (var result in results.GetResults())
    {
        output.Add(result.Document);
    }
    return Results.Ok(output);
    
});

app.MapPost("/data", (Query query) =>
{

    var searchServiceManager = app.Services.GetRequiredService<SearchServiceManager>();
    var results = searchServiceManager.Search(query.SearchText);
    var output = new List<SearchDocument>();
    foreach (var result in results.GetResults())
    {
        output.Add(result.Document);
    }
    return Results.Ok(output);

});

app.Run();

 