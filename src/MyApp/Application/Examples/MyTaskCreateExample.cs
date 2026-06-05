using MyApp.Application.DTOs.MyTask;
using Swashbuckle.AspNetCore.Filters;

public class MyTaskCreateExample : IExamplesProvider<MyTaskCreateDto>
{
    public MyTaskCreateDto GetExamples()
    {
        return new MyTaskCreateDto
        {
            Title = "Comprar comida",
            Description = "Ir al supermercado y comprar leche, pan y huevos",
            StartDate = DateTime.UtcNow.AddDays(2)
        };
    }
}
