using Microsoft.AspNetCore.Mvc;
using VegetableStore.Models;
using VegetableStore.Rendering;

namespace VegetableStore;
//wassup
public class  Program
{
    // 2.1
    private static int _nextId = 6;

    private static readonly List<VegetableModel> Vegetables = new()
    {
        new(1, "Cucumber", "Vegetable", 3.5m),
        new(2, "Carrot",   "Crop",      2.50m),
        new(3, "Potato",   "Vegetable", 1.50m),
        new(4, "Pumpkin",  "Vegetable", 8m),
        new(5, "Pea",      "Beans",     1m)
    };

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        
        // 2.2
        builder.Services.AddSingleton<PageRenderer>();

        // 2.4

        var app = builder.Build();

        // працюємо
        app.UseStaticFiles();

        

        app.MapGet("/", Index);                                                        
        app.MapPost("/vegetables", AddVegetable).DisableAntiforgery();                
        app.MapGet("/vegetables/{id:int}/edit", EditForm);                             // update форма
        app.MapPost("/vegetables/{id:int}/edit", UpdateVegetable).DisableAntiforgery(); //update збереження
        app.MapPost("/vegetables/{id:int}/delete", DeleteVegetable);                   //delete

        app.Run();
    }

    
    private static IResult Index(PageRenderer pages)
    {
        return pages.Index(Vegetables);
    }


    private static IResult AddVegetable(
        [FromForm] string name,
        [FromForm] string type,
        [FromForm] decimal price)
    {
        //2.3
        Vegetables.Add(new VegetableModel(_nextId++, name, type, price));
        return Results.Redirect("/");
    }


    private static IResult EditForm(int id, PageRenderer pages)
    {
        var vegetable = Vegetables.FirstOrDefault(v => v.Id == id);
        if (vegetable is null)
            return pages.NotFound($"Овоч з id={id} не знайдено");

        return pages.Edit(vegetable);
    }


    private static IResult UpdateVegetable(
        int id,
        [FromForm] string name,
        [FromForm] string type,
        [FromForm] decimal price,
        PageRenderer pages)
    {
        var vegetable = Vegetables.FirstOrDefault(v => v.Id == id);
        if (vegetable is null)
            return pages.NotFound($"Овоч з id={id} не знайдено");

        // 2.3
        vegetable.Name = name;
        vegetable.Type = type;
        vegetable.Price = price;

        return Results.Redirect("/");
    }


    private static IResult DeleteVegetable(int id, PageRenderer pages)
    {
        var removed = Vegetables.RemoveAll(v => v.Id == id);
        if (removed == 0)
            return pages.NotFound($"Овоч з id={id} не знайдено");

        return Results.Redirect("/");
    }
}
