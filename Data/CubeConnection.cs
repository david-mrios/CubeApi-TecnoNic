using Microsoft.AnalysisServices.AdomdClient;

public class CubeConnection
{
    private readonly string _connectionString;

    public CubeConnection()
    {
        // Ajustar la cadena de conexión a tu servidor SSAS
        _connectionString = "Data Source=LOCALHOST;Catalog=CubeTecnoNic;";
    }

    public AdomdConnection GetConnection()
    {
        var connection = new AdomdConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
