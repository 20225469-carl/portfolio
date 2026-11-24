using Npgsql;
using portfolio.Models;

namespace portfolio.Data;

public class ProjectImageRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public ProjectImageRepository(NpgsqlDataSource dataSource) => _dataSource = dataSource;

    public async Task<List<ProjectImage>> GetByProjectIdAsync(long projectId)
    {
        const string sql = "SELECT image_id, project_id, image_url FROM portfolio.project_image WHERE project_id = @pid ORDER BY image_id;";
        var list = new List<ProjectImage>();
        await using var cmd = _dataSource.CreateCommand(sql);
        cmd.Parameters.AddWithValue("pid", projectId);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new ProjectImage
            {
                ImageId = reader.GetInt64(0),
                ProjectId = reader.GetInt64(1),
                ImageUrl = reader.GetString(2)
            });
        }
        return list;
    }

    public async Task DeleteAsync(long imageId)
    {
        const string sql = "DELETE FROM portfolio.project_image WHERE image_id = @id;";
        await using var cmd = _dataSource.CreateCommand(sql);
        cmd.Parameters.AddWithValue("id", imageId);
        await cmd.ExecuteNonQueryAsync();
    }
}