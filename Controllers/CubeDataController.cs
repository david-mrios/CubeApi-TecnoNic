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

    // 1. Ventas Totales por Año
    [HttpGet]
    [Route("get-total-sales-by-year")]
    public IActionResult GetTotalSalesByYear()
    {
        string query = """
            SELECT 
                [Measures].[KPI Total_Price] ON COLUMNS,
                [Date].[HierarchyDate].[Year].MEMBERS ON ROWS
            FROM [Model]
        """;
        var data = _cubeDataService.GetCubeData(query);
        return Ok(data);
    }

    // 2. Cantidad de Órdenes por Método de Pago
    [HttpGet]
    [Route("get-orders-by-payment-method")]
    public IActionResult GetOrdersByPaymentMethod()
    {
        string query = """
            SELECT 
                [Measures].[Sum of Quantity] ON COLUMNS,
                [Order].[HierarchyOrder].[Payment_Method].MEMBERS ON ROWS
            FROM [Model]
        """;
        var data = _cubeDataService.GetCubeData(query);
        return Ok(data);
    }

    // 3. Ventas por País de Envío
    [HttpGet]
    [Route("get-sales-by-shipping-country")]
    public IActionResult GetSalesByShippingCountry()
    {
        string query = """
            SELECT 
                [Measures].[Sales Amount by ship] ON COLUMNS,
                [Location].[HierarchyShip].[Shipping_Country].MEMBERS ON ROWS
            FROM [Model]
        """;
        var data = _cubeDataService.GetCubeData(query);
        return Ok(data);
    }

    // 4. Impuesto Total por Estado de Envío
    [HttpGet]
    [Route("get-total-tax-by-shipping-state")]
    public IActionResult GetTotalTaxByShippingState()
    {
        string query = """
            SELECT 
                [Measures].[Sum of Tax_Amount] ON COLUMNS,
                [Location].[HierarchyShip].[Shipping_State].MEMBERS ON ROWS
            FROM [Model]
        """;
        var data = _cubeDataService.GetCubeData(query);
        return Ok(data);
    }

    // 5. Cantidad Total por Categoría de Producto
    [HttpGet]
    [Route("get-total-quantity-by-product-category")]
    public IActionResult GetTotalQuantityByProductCategory()
    {
        string query = """
            SELECT 
                [Measures].[Sum of Quantity] ON COLUMNS,
                [Product].[HierarchyClassification].[Category_Name].MEMBERS ON ROWS
            FROM [Model]
        """;
        var data = _cubeDataService.GetCubeData(query);
        return Ok(data);
    }

    // 6. Costos de Envío por Año Fiscal
    [HttpGet]
    [Route("get-shipping-cost-by-fiscal-year")]
    public IActionResult GetShippingCostByFiscalYear()
    {
        string query = """
            SELECT 
                [Measures].[Sum of Shipping_Cost] ON COLUMNS,
                [Date].[HierarchyFiscal].[FiscalYear].MEMBERS ON ROWS
            FROM [Model]
        """;
        var data = _cubeDataService.GetCubeData(query);
        return Ok(data);
    }

    // 7. Cantidad de Productos Vendidos por Marca
    [HttpGet]
    [Route("get-products-sold-by-brand")]
    public IActionResult GetProductsSoldByBrand()
    {
        string query = """
            SELECT 
                {[Measures].[KPI Total_Price], [Measures].[Sum of Quantity]} ON COLUMNS,
                [Product].[HierarchyClassification].[Brand_Name].MEMBERS ON ROWS
            FROM [Model]
        """;
        var data = _cubeDataService.GetCubeData(query);
        return Ok(data);
    }

    // 8. Ventas Totales por Compañía de Envío
    [HttpGet]
    [Route("get-total-sales-by-shipping-company")]
    public IActionResult GetTotalSalesByShippingCompany()
    {
        string query = """
            SELECT 
                [Measures].[Sales Amount by ship] ON COLUMNS,
                [Fact_Shipping].[Shipping_Company].[Shipping_Company].MEMBERS ON ROWS
            FROM [Model]
        """;
        var data = _cubeDataService.GetCubeData(query);
        return Ok(data);
    }

    // 9. Órdenes por Estado del Pedido
    [HttpGet]
    [Route("get-orders-by-order-status")]
    public IActionResult GetOrdersByOrderStatus()
    {
        string query = """
            SELECT 
                [Measures].[KPI_Order_Quantity_Delivery] ON COLUMNS,
                [Order].[HierarchyOrder].[Status].MEMBERS ON ROWS
            FROM [Model]
        """;
        var data = _cubeDataService.GetCubeData(query);
        return Ok(data);
    }

    // 10. Ventas y Cantidad por Año y Mes
    [HttpGet]
    [Route("get-sales-and-quantity-by-month")]
    public IActionResult GetSalesAndQuantityByYearAndMonth()
    {
        string query = """
        SELECT 
            {[Measures].[KPI Total_Price], [Measures].[Sum of Quantity]} ON COLUMNS,
            DESCENDANTS([Date].[HierarchyDate].[Year].MEMBERS, [Date].[HierarchyDate].[Month]) ON ROWS
        FROM [Model]


        """;
        var data = _cubeDataService.GetCubeData(query);
        return Ok(data);
    }
}
