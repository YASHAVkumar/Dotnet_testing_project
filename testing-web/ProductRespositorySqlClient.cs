using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace testing_web;

public class ProductRespositorySqlClient(IConfiguration configuration) : IProductRepo
{
    private readonly string _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection connection string was not found.");

    public async Task<Product> CreateProduct(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        const string sql =
        """
            INSERT INTO [dbo].[Products]
                ([Name]
                ,[Date]
                ,[Desc]
                ,[IsActive])
            OUTPUT INSERTED.Id
            VALUES
            (
                @Name,
                @Date,
                @Desc,
                @IsActive
            );
        """;

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.OpenAsync();

        await using var transaction =
            await connection.BeginTransactionAsync();

        try
        {
            int productId;

            await using (var command =
                new SqlCommand(
                    sql,
                    connection,
                    (SqlTransaction)transaction))
            {
                command.Parameters.Add(
                    "@Name",
                    SqlDbType.NVarChar,
                    200).Value = product.Name;

                command.Parameters.Add(
                    "@Date",
                    SqlDbType.DateTime2).Value = product.Date;

                command.Parameters.Add(
                    "@Desc",
                    SqlDbType.NVarChar,
                    1000).Value =
                        (object?)product.Desc ?? DBNull.Value;

                command.Parameters.Add(
                    "@IsActive",
                    SqlDbType.Bit).Value =
                        product.IsActive;

                productId = Convert.ToInt32(
                    await command.ExecuteScalarAsync());
            }


            await transaction.CommitAsync();

            product.Id = productId;

            return product;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteProduct(int id)
    {
        const string sql = """
            UPDATE Products
            SET IsActive = 0
            WHERE Id = @Id;
            """;

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.OpenAsync();

        await using var command =
            new SqlCommand(sql, connection);

        command.Parameters.Add(
            "@Id",
            SqlDbType.Int).Value = id;

        var rowsAffected =
            await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }

    public async Task<Product?> GetProductById(int id)
    {
        const string productSql = """
            SELECT
                [Id],
                [Name],
                [Date],
                [Desc],
                [IsActive]
            FROM Products
            WHERE Id = @Id;
            """;

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.OpenAsync();

        Product? product = null;

        // Get Product
        await using (var command =
            new SqlCommand(productSql, connection))
        {
            command.Parameters.Add(
                "@Id",
                SqlDbType.Int).Value = id;

            await using var reader =
                await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                product = MapProduct(reader);
            }
        }

        if (product == null)
            return null;


        return product;
    }

    public async Task<IReadOnlyList<Product>> GetProducts()
    {
        const string productSql =
        """
            SELECT [Id]
                ,[Name]
                ,[Date]
                ,[Desc]
                ,[IsActive]
            FROM [Products]
            ORDER BY 1 DESC
        """;

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.OpenAsync();

        var products = new List<Product>();

        await using (var command =
            new SqlCommand(productSql, connection))
        {
            await using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(MapProduct(reader));
            }
        }
        return products;
    }

    public async Task<bool> ProductExists(int id)
    {
        const string sql = """
            SELECT COUNT(1)
            FROM Products
            WHERE Id = @Id;
            """;

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.OpenAsync();

        await using var command =
            new SqlCommand(sql, connection);

        command.Parameters.Add(
            "@Id",
            SqlDbType.Int).Value = id;

        var result =
            await command.ExecuteScalarAsync();

        return Convert.ToInt32(result) > 0;
    }

    public async Task<bool> UpdateProduct(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        const string updateProductSql = """
              UPDATE Products
              SET
                [Name] = @Name,
                [Date] = @Date,
                [Desc] = @Desc,
                [IsActive] = @IsActive
               WHERE Id = @Id;
            """;

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.OpenAsync();

        await using var transaction =
            await connection.BeginTransactionAsync();

        try
        {
            int rowsAffected;

            // Update Product
            await using (var command =
                new SqlCommand(
                    updateProductSql,
                    connection,
                    (SqlTransaction)transaction))
            {
                command.Parameters.Add(
                    "@Id",
                    SqlDbType.Int).Value = product.Id;

                command.Parameters.Add(
                    "@Name",
                    SqlDbType.NVarChar,
                    200).Value = product.Name;

                command.Parameters.Add(
                    "@Date",
                    SqlDbType.DateTime2).Value = product.Date;

                command.Parameters.Add(
                    "@Desc",
                    SqlDbType.NVarChar,
                    1000).Value =
                        (object?)product.Desc ?? DBNull.Value;

                command.Parameters.Add(
                    "@IsActive",
                    SqlDbType.Bit).Value =
                        product.IsActive;

                rowsAffected =
                    await command.ExecuteNonQueryAsync();
            }

            // Product doesn't exist
            if (rowsAffected == 0)
            {
                await transaction.RollbackAsync();
                return false;
            }


            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static Product MapProduct(
        SqlDataReader reader)
    {
        return new Product
        {
            Id = reader.GetInt32(
                reader.GetOrdinal("Id")),

            Name = reader.GetString(
                reader.GetOrdinal("Name")),

            Date = reader.GetDateTime(
                reader.GetOrdinal("Date")),

            Desc = reader.IsDBNull(reader.GetOrdinal("Desc"))
                ? null
                : reader.GetString(
                    reader.GetOrdinal("Desc")),

            IsActive = reader.GetBoolean(
                reader.GetOrdinal("IsActive"))
        };
    }
}