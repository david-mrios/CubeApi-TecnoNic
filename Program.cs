
using Microsoft.AnalysisServices.AdomdClient; // Para conectar con SSAS y trabajar con CellSet
using System.Collections.Generic; // Para usar List y Dictionary
using Microsoft.AspNetCore.Builder; // Para la configuración del WebApplication
using Microsoft.Extensions.DependencyInjection; // Para el servicio del API explorer
using Microsoft.Extensions.Hosting; // Para el manejo del ambiente (Development, Production)



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();


app.MapGet("/cubedata", () =>
{
    string connectionString = "Data Source=LOCALHOST;Catalog=CubeTecnoNic;";  // Ajusta la cadena de conexión
    using (AdomdConnection connection = new AdomdConnection(connectionString))
    {
        connection.Open();
        //string query = "SELECT [Measures].[YourMeasure] ON COLUMNS FROM [YourCube]";
        //string myquery = " SELECT NON EMPTY { [Measures].[Sales Amount] } ON COLUMNS, NON EMPTY { ([Customer].[Education].[Education].ALLMEMBERS ) } DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS FROM [AWDW] CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";
        //string myquery = " SELECT NON EMPTY { [Measures].[Sales Amount] } ON COLUMNS, NON EMPTY { ([Customer].[Education].[Education].ALLMEMBERS * [Customer].[Gender].[Gender].ALLMEMBERS ) } DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS FROM [AWDW] CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";
        string myquery = """
    SELECT 
        NON EMPTY { [Measures].[Unit Price], [Measures].[Quantity] } ON COLUMNS, 
        NON EMPTY { ([Customer].[Country].[Country].ALLMEMBERS * 
                     [Product].[Brand Name].[Brand Name].ALLMEMBERS ) } 
        DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS 
    FROM [TecnoNicDW]
    """;
        using (AdomdCommand command = new AdomdCommand(myquery, connection))
        {
            var result = command.ExecuteCellSet();
            
            // Transforma el resultado en JSON
            var jsonResult = TransformToJSON(result);
            return Results.Ok(jsonResult);
        }
    }

List<Dictionary<string, object>> TransformToJSON(CellSet result)
    {
        var jsonData = new List<Dictionary<string, object>>();
        int cellIndex = 0; // Índice para rastrear las celdas correctamente

        // Iterar a través de las filas dinámicamente
        foreach (var rowPosition in result.Axes[1].Positions)  // Eje de filas (Dimensiones)
        {
            var dataPoint = new Dictionary<string, object>();

            // Agregar las dimensiones (desde el eje de filas)
            for (int i = 0; i < rowPosition.Members.Count; i++)
            {
                var dimensionName = result.Axes[1].Set.Hierarchies[i].Name;
                dataPoint[dimensionName] = rowPosition.Members[i].Caption; // Añadir nombre de la dimensión y valor
            }

            // Añadir las medidas correspondientes (desde el eje de columnas)
            for (int colIndex = 0; colIndex < result.Axes[0].Positions.Count; colIndex++)
            {
                var measureName = result.Axes[0].Positions[colIndex].Members[0].Caption; // Captura el nombre de la medida
                var cellValue = result.Cells[cellIndex].Value; // Captura el valor correcto de la celda

                dataPoint[measureName] = cellValue;
                cellIndex++; // Aumentar el índice de la celda
            }

            jsonData.Add(dataPoint);
        }

        return jsonData;
    }
})
.WithName("GetCubeData")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Función auxiliar para transformar los datos del cubo en formato JSON


