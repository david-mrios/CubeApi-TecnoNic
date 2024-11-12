using Microsoft.AnalysisServices.AdomdClient;

public class CubeConnection
{
    private readonly string _connectionString;

    public CubeConnection()
    {
        // Ajustar la cadena de conexión a tu servidor SSAS
        _connectionString = "Provider=MSOLAP;Data Source=localhost\\SSASTABULAR;Catalog=MiModeloTabular;";
    }

    public AdomdConnection GetConnection()
    {
        var connection = new AdomdConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
