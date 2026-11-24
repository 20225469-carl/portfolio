using Npgsql;
using portfolio.Models;

namespace portfolio.Data;

public class ProjectRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public ProjectRepository(NpgsqlDataSource dataSource) => _dataSource = dataSource;

    public async Task<List<Project>> GetAllAsync(long? courseId = null)
    {
        var sql =
            "SELECT p.project_id, p.course_id, c.code, c.title, p.title, p.summary, p.video_url, p.repo_url, p.year_created " +
            "FROM portfolio.project p " +
            "JOIN portfolio.course c ON p.course_id = c.course_id " +
            (courseId.HasValue ? "WHERE p.course_id = @cid " : "") +
            "ORDER BY p.year_created DESC, p.project_id DESC;";

        var list = new List<Project>();
        await using var cmd = _dataSource.CreateCommand(sql);
        if (courseId.HasValue) cmd.Parameters.AddWithValue("cid", courseId.Value);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Project
            {
                ProjectId = reader.GetInt64(0),
                CourseId = reader.GetInt64(1),
                CourseCode = reader.GetString(2),
                CourseTitle = reader.GetString(3),
                Title = reader.GetString(4),
                Summary = reader.GetString(5),
                VideoUrl = reader.IsDBNull(6) ? null : reader.GetString(6),
                RepoUrl = reader.IsDBNull(7) ? null : reader.GetString(7),
                YearCreated = reader.GetInt32(8)
            });
        }
        return list;
    }

    public async Task<Project?> GetByIdAsync(long id)
    {
        const string sql =
            "SELECT p.project_id, p.course_id, c.code, c.title, p.title, p.summary, p.video_url, p.repo_url, p.year_created " +
            "FROM portfolio.project p " +
            "JOIN portfolio.course c ON p.course_id = c.course_id " +
            "WHERE p.project_id = @id;";

        await using var cmd = _dataSource.CreateCommand(sql);
        cmd.Parameters.AddWithValue("id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return new Project
        {
            ProjectId = reader.GetInt64(0),
            CourseId = reader.GetInt64(1),
            CourseCode = reader.GetString(2),
            CourseTitle = reader.GetString(3),
            Title = reader.GetString(4),
            Summary = reader.GetString(5),
            VideoUrl = reader.IsDBNull(6) ? null : reader.GetString(6),
            RepoUrl = reader.IsDBNull(7) ? null : reader.GetString(7),
            YearCreated = reader.GetInt32(8)
        };
    }

    public async Task CreateWithImagesAsync(Project project, List<string> imageUrls)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var tx = await conn.BeginTransactionAsync();

        try
        {
            const string insertProjSql =
                "INSERT INTO portfolio.project (course_id, title, summary, video_url, repo_url, year_created) " +
                "VALUES (@cid, @title, @summary, @video, @repo, @year) RETURNING project_id;";

            long newProjectId;
            await using (var cmd = new NpgsqlCommand(insertProjSql, conn, tx))
            {
                cmd.Parameters.AddWithValue("cid", project.CourseId!.Value);
                cmd.Parameters.AddWithValue("title", project.Title);
                cmd.Parameters.AddWithValue("summary", project.Summary);
                cmd.Parameters.AddWithValue("video", (object?)project.VideoUrl ?? DBNull.Value);
                cmd.Parameters.AddWithValue("repo", (object?)project.RepoUrl ?? DBNull.Value);
                cmd.Parameters.AddWithValue("year", project.YearCreated);
                newProjectId = Convert.ToInt64(await cmd.ExecuteScalarAsync());
            }

            foreach (var url in imageUrls)
            {
                await using var imgCmd = new NpgsqlCommand(
                    "INSERT INTO portfolio.project_image (project_id, image_url) VALUES (@pid, @url);", conn, tx);
                imgCmd.Parameters.AddWithValue("pid", newProjectId);
                imgCmd.Parameters.AddWithValue("url", url);
                await imgCmd.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateWithImagesAsync(Project project, List<string> imageUrls)
    {
        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var tx = await conn.BeginTransactionAsync();

        try
        {
            const string updateProjSql =
                "UPDATE portfolio.project SET course_id = @cid, title = @title, summary = @summary, " +
                "video_url = @video, repo_url = @repo, year_created = @year WHERE project_id = @pid;";

            await using (var cmd = new NpgsqlCommand(updateProjSql, conn, tx))
            {
                cmd.Parameters.AddWithValue("pid", project.ProjectId);
                cmd.Parameters.AddWithValue("cid", project.CourseId!.Value);
                cmd.Parameters.AddWithValue("title", project.Title);
                cmd.Parameters.AddWithValue("summary", project.Summary);
                cmd.Parameters.AddWithValue("video", (object?)project.VideoUrl ?? DBNull.Value);
                cmd.Parameters.AddWithValue("repo", (object?)project.RepoUrl ?? DBNull.Value);
                cmd.Parameters.AddWithValue("year", project.YearCreated);
                await cmd.ExecuteNonQueryAsync();
            }

            await using (var delCmd = new NpgsqlCommand("DELETE FROM portfolio.project_image WHERE project_id = @pid;", conn, tx))
            {
                delCmd.Parameters.AddWithValue("pid", project.ProjectId);
                await delCmd.ExecuteNonQueryAsync();
            }

            foreach (var url in imageUrls)
            {
                await using var imgCmd = new NpgsqlCommand(
                    "INSERT INTO portfolio.project_image (project_id, image_url) VALUES (@pid, @url);", conn, tx);
                imgCmd.Parameters.AddWithValue("pid", project.ProjectId);
                imgCmd.Parameters.AddWithValue("url", url);
                await imgCmd.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteAsync(long id)
    {
        const string sql = "DELETE FROM portfolio.project WHERE project_id = @id;";
        await using var cmd = _dataSource.CreateCommand(sql);
        cmd.Parameters.AddWithValue("id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}