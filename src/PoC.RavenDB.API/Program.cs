using Raven.Client.Documents;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDocumentStore>(serviceProvider =>
{
    var store = new DocumentStore
    {
        Urls = new[] { "http://localhost:8080" },
        Database = "DEPLOY"
    };

    store.Initialize();

    return store;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
