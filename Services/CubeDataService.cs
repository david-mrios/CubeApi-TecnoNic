using Microsoft.AnalysisServices.AdomdClient;

public class CubeDataService
{
    private readonly CubeConnection _cubeConnection;

    public CubeDataService(CubeConnection cubeConnection)
    {
        _cubeConnection = cubeConnection;
    }

    public List<Dictionary<string, object>> GetCubeData(string query)
    {
        using (var connection = _cubeConnection.GetConnection())
        using (var command = new AdomdCommand(query, connection))
        {
            var result = command.ExecuteCellSet();
            return TransformToJSON(result);
        }
    }

    private List<Dictionary<string, object>> TransformToJSON(CellSet result)
    {
        var jsonData = new List<Dictionary<string, object>>();
        int cellIndex = 0;

        foreach (var rowPosition in result.Axes[1].Positions)
        {
            var dataPoint = new Dictionary<string, object>();

            for (int i = 0; i < rowPosition.Members.Count; i++)
            {
                var dimensionName = result.Axes[1].Set.Hierarchies[i].Name;
                dataPoint[dimensionName] = rowPosition.Members[i].Caption;
            }

            for (int colIndex = 0; colIndex < result.Axes[0].Positions.Count; colIndex++)
            {
                var measureName = result.Axes[0].Positions[colIndex].Members[0].Caption;
                var cellValue = result.Cells[cellIndex].Value;

                dataPoint[measureName] = cellValue;
                cellIndex++;
            }

            jsonData.Add(dataPoint);
        }

        return jsonData;
    }
}
