using Npgsql;
using portfolio.Models;

namespace portfolio.Data;

public class CourseRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public CourseRepository(NpgsqlDataSource dataSource) => _dataSource = dataSource;

    public async Task<List<Course>> GetAllAsync()
    {
        const string sql = "SELECT course_id, code, title, term, description FROM portfolio.course ORDER BY code;";
        var list = new List<Course>();
        await using var cmd = _dataSource.CreateCommand(sql);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Course
            {
                CourseId = reader.GetInt64(0),
                Code = reader.GetString(1),
                Title = reader.GetString(2),
                Term = reader.GetString(3),
                Description = reader.IsDBNull(4) ? null : reader.GetString(4)
            });
        }
        return list;
    }

    public async Task<Course?> GetByIdAsync(long id)
    {
        const string sql = "SELECT course_id, code, title, term, description FROM portfolio.course WHERE course_id = @id;";
        await using var cmd = _dataSource.CreateCommand(sql);
        cmd.Parameters.AddWithValue("id", id);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;

        return new Course
        {
            CourseId = reader.GetInt64(0),
            Code = reader.GetString(1),
            Title = reader.GetString(2),
            Term = reader.GetString(3),
            Description = reader.IsDBNull(4) ? null : reader.GetString(4)
        };
    }

    public async Task AddAsync(Course course)
    {
        const string sql = "INSERT INTO portfolio.course (code, title, term, description) VALUES (@code, @title, @term, @desc);";
        await using var cmd = _dataSource.CreateCommand(sql);
        cmd.Parameters.AddWithValue("code", course.Code);
        cmd.Parameters.AddWithValue("title", course.Title);
        cmd.Parameters.AddWithValue("term", course.Term);
        cmd.Parameters.AddWithValue("desc", (object?)course.Description ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(Course course)
    {
        const string sql = "UPDATE portfolio.course SET code = @code, title = @title, term = @term, description = @desc WHERE course_id = @id;";
        await using var cmd = _dataSource.CreateCommand(sql);
        cmd.Parameters.AddWithValue("id", course.CourseId);
        cmd.Parameters.AddWithValue("code", course.Code);
        cmd.Parameters.AddWithValue("title", course.Title);
        cmd.Parameters.AddWithValue("term", course.Term);
        cmd.Parameters.AddWithValue("desc", (object?)course.Description ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(long id)
    {
        const string sql = "DELETE FROM portfolio.course WHERE course_id = @id;";
        await using var cmd = _dataSource.CreateCommand(sql);
        cmd.Parameters.AddWithValue("id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}