
using Play.Common.MongoDB;
using Play.Inventory.Service.Entities;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

Play.Common.MongoDB.Extensions.AddMongo(builder.Services).AddMongoRepository<InventoryItem>("inventoryitems");

builder.Services.AddControllers(options =>
                                {
                                    options.SuppressAsyncSuffixInActionNames = false;
                                });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger UI (do this even outside Development when you're working locally)
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();