using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class CubeDataController : ControllerBase
{
    private readonly CubeDataService _cubeDataService;

    public CubeDataController(CubeDataService cubeDataService)
    {
        _cubeDataService = cubeDataService;
    }

     // Ruta base "/cubedata"
    [HttpGet]
    public IActionResult Index()
    {
        var queries = new List<string>
        {
            "1. Total de ventas por cliente y producto (Operación: Dice)",
            "2. Monto total de ventas por ubicación de envío (Operación: Slice)",
            "3. Costo total de envío por cada producto (Operación: Slice)",
            "4. Total de ventas por cada orden de compra (Operación: Roll Up)",
            "5. Monto total de impuestos por cliente (Operación: Dice)",
            "6. Cantidad total de productos enviados por ubicación de envío (Operación: Drill Down)",
            "7. Costo total de envío por cada orden de compra (Operación: Roll Up)",
            "8. Monto total de ventas en función de la ubicación de envío (Operación: Pivot)"
        };

        return Ok(new {
            Message = "Hay 8 consultas disponibles. Estas son las consultas:",
            AvailableQueries = queries
        });
    }
    // test
    [HttpGet]
    [Route("get-cube-data")]
    public IActionResult GetCubeData()
    {
        string query = """
            SELECT 
                NON EMPTY { [Measures].[Unit Price], [Measures].[Quantity] } ON COLUMNS, 
                NON EMPTY { ([Customer].[Country].[Country].ALLMEMBERS * 
                             [Product].[Brand Name].[Brand Name].ALLMEMBERS ) } 
                DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS 
            FROM [TecnoNicDW]
            """;
        var data = _cubeDataService.GetCubeData(query);
        return Ok(data);
    }
   // 1. Total de ventas por cliente y producto (Operación: Dice)
    [HttpGet]
    [Route("get-sales-by-customer-and-product")]
    public IActionResult GetSalesByCustomerAndProduct()
    {
        string query = """
            SELECT 
                NON EMPTY { [Measures].[Sales Amount], [Measures].[Quantity] } ON COLUMNS,
                NON EMPTY { ([Customer].[Customer Key].[Customer Key].ALLMEMBERS * 
                            [Product].[ProductHierarchy].[Product]) } 
                DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS
            FROM [TecnoNicDW]
            """;
        var data = _cubeDataService.GetCubeData(query);
        return Ok(data);
    }

    // 2. Monto total de ventas por ubicación de envío (Operación: Slice)
    [HttpGet]
    [Route("get-sales-by-shipment-location")]
    public IActionResult GetSalesByShipmentLocation()
    {
        string query = """
            SELECT 
                NON EMPTY { [Measures].[Sales Amount] } ON COLUMNS,
                NON EMPTY { 
                    ([Location].[Shipping City].[Shipping City],
                    [Location].[Shipping Country].[Shipping Country]) 
                } 
                DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS
            FROM [TecnoNicDW]
            """;
        var data = _cubeDataService.GetCubeData(query);
        return Ok(data);
    }

    // 3. Costo total de envío por cada producto (Operación: Slice)
    [HttpGet]
    [Route("get-shipping-cost-by-product")]
    public IActionResult GetShippingCostByProduct()
    {
        string query = """
            SELECT 
                NON EMPTY { [Measures].[Shipping Cost] } ON COLUMNS,
                NON EMPTY { [Product].[ProductHierarchy].[Product] *
                            [Product].[Brand Name].[Brand Name].ALLMEMBERS } 
                DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS
            FROM [TecnoNicDW]
            """;
        var data = _cubeDataService.GetCubeData(query);
        return Ok(data);
    }

    // 4. Total de ventas por cada orden de compra (Operación: Roll Up)
    [HttpGet]
    [Route("get-sales-by-purchase-order")]
    public IActionResult GetSalesByPurchaseOrder()
    {
        string query = """
            SELECT 
                NON EMPTY { [Measures].[Sales Amount], [Measures].[Quantity] } ON COLUMNS,
                NON EMPTY { [Order].[Status].[Status] } 
                DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS
            FROM [TecnoNicDW]	
            """;
        var data = _cubeDataService.GetCubeData(query);
        return Ok(data);
    }

    // 5. Monto total de impuestos por cliente (Operación: Dice)
    [HttpGet]
    [Route("get-taxes-by-customer")]
    public IActionResult GetTaxesByCustomer()
    {
        string query = """
            SELECT 
                NON EMPTY { [Measures].[Tax Amount] } ON COLUMNS,
                NON EMPTY { [Customer].[Customer Key].[Customer Key].ALLMEMBERS *
                            [Customer].[Last Name].[Last Name]} 
                DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS
            FROM [TecnoNicDW]	
            """;
        var data = _cubeDataService.GetCubeData(query);
        return Ok(data);
    }

    // 6. Cantidad total de productos enviados por ubicación de envío (Operación: Drill Down)
    [HttpGet]
    [Route("get-shipped-products-by-location")]
    public IActionResult GetShippedProductsByLocation()
    {
        string query = """
            SELECT 
                NON EMPTY { [Measures].[Quantity] } ON COLUMNS,
                NON EMPTY { 
                    ([Location].[Shipping City].[Shipping City],
                    [Location].[Shipping Country].[Shipping Country]) 
                } 
                DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS
            FROM [TecnoNicDW]
            """;
        var data = _cubeDataService.GetCubeData(query);
        return Ok(data);
    }

    // 7. Costo total de envío por cada orden de compra (Operación: Roll Up)
    [HttpGet]
    [Route("get-shipping-cost-by-purchase-order")]
    public IActionResult GetShippingCostByPurchaseOrder()
    {
        string query = """
            SELECT 
                NON EMPTY { [Measures].[Shipping Cost] } ON COLUMNS,
                NON EMPTY { [Order].[Status].[Status]} 
                DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS
            FROM [TecnoNicDW] 
            """;
        var data = _cubeDataService.GetCubeData(query);
        return Ok(data);
    }

    // 8. Monto total de ventas en función de la ubicación de envío (Operación: Pivot)
    [HttpGet]
    [Route("get-sales-by-shipment-location-pivot")]
    public IActionResult GetSalesByShipmentLocationPivot()
    {
        string query = """
            SELECT 
                NON EMPTY { [Measures].[Shipping Cost] } ON COLUMNS,
                NON EMPTY { 
                    [Location].[Shipping Address].[Shipping Address].ALLMEMBERS * 
                    [Location].[Shipping City].[Shipping City] *
                    [Location].[Shipping Country].[Shipping Country]
                } 
                DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS
            FROM [TecnoNicDW] 
            """;
        var data = _cubeDataService.GetCubeData(query);
        return Ok(data);
    }
}
