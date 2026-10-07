var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

var productCatalog = new List<Product>
{
    new(1, "Classic Sneakers", "classic-sneakers", "Footwear", 89.99m, 18, "Minimal daily sneakers for work and travel.", true),
    new(2, "Urban Backpack", "urban-backpack", "Accessories", 129.00m, 12, "Water-resistant backpack with multiple compartments.", true),
    new(3, "Smart Watch", "smart-watch", "Electronics", 249.99m, 8, "Fitness tracker with health insights and notifications.", true),
    new(4, "Premium Mug", "premium-mug", "Home", 19.50m, 32, "Ceramic mug built for coffee and tea rituals.", false)
};

var orders = new List<Order>();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "ECommercePlatform",
    timestamp = DateTime.UtcNow
}));

app.MapGet("/api/products", () => Results.Ok(productCatalog));

app.MapGet("/api/products/{id:int}", (int id) =>
{
    var product = productCatalog.FirstOrDefault(item => item.Id == id);
    return product is null ? Results.NotFound(new { message = "Product not found." }) : Results.Ok(product);
});

app.MapPost("/api/products", (CreateProductRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.BadRequest(new { message = "Product name is required." });
    }

    var nextId = productCatalog.Count == 0 ? 1 : productCatalog.Max(p => p.Id) + 1;
    var slug = string.IsNullOrWhiteSpace(request.Slug)
        ? request.Name.ToLowerInvariant().Replace(" ", "-")
        : request.Slug;

    var product = new Product(
        nextId,
        request.Name,
        slug,
        request.Category,
        request.Price,
        request.Stock,
        request.Description,
        request.IsFeatured);

    productCatalog.Add(product);
    return Results.Created($"/api/products/{product.Id}", product);
});

app.MapGet("/api/orders", () => Results.Ok(orders));

app.MapPost("/api/orders", (CreateOrderRequest request) =>
{
    if (request.Items.Count == 0)
    {
        return Results.BadRequest(new { message = "Order must contain at least one item." });
    }

    var orderItems = new List<OrderItem>();
    foreach (var item in request.Items)
    {
        var product = productCatalog.FirstOrDefault(p => p.Id == item.ProductId);
        if (product is null)
        {
            return Results.BadRequest(new { message = $"Product {item.ProductId} not found." });
        }

        if (product.Stock < item.Quantity)
        {
            return Results.BadRequest(new { message = $"Insufficient stock for {product.Name}." });
        }

        productCatalog.Remove(product);
        productCatalog.Add(product with { Stock = product.Stock - item.Quantity });
        orderItems.Add(new OrderItem(product.Id, product.Name, item.Quantity, product.Price));
    }

    var total = orderItems.Sum(item => item.Quantity * item.UnitPrice);
    var order = new Order(
        orders.Count + 1,
        request.CustomerName,
        request.CustomerEmail,
        orderItems,
        total,
        DateTime.UtcNow);

    orders.Add(order);
    return Results.Ok(order);
});

app.MapGet("/api/dashboard", () => Results.Ok(new
{
    totalProducts = productCatalog.Count,
    featuredProducts = productCatalog.Count(p => p.IsFeatured),
    totalOrders = orders.Count,
    inventoryUnits = productCatalog.Sum(p => p.Stock),
    revenue = orders.Sum(order => order.Total)
}));

app.Run();

public record Product(int Id, string Name, string Slug, string Category, decimal Price, int Stock, string Description, bool IsFeatured);
public record CreateProductRequest(string Name, string Category, decimal Price, int Stock, string Description, string? Slug = null, bool IsFeatured = false);
public record OrderItem(int ProductId, string ProductName, int Quantity, decimal UnitPrice);
public record CreateOrderRequest(string CustomerName, string CustomerEmail, List<OrderItemRequest> Items);
public record OrderItemRequest(int ProductId, int Quantity);
public record Order(int Id, string CustomerName, string CustomerEmail, List<OrderItem> Items, decimal Total, DateTime CreatedAt);
