using Dapper;
using Microsoft.Data.SqlClient;
using TamoeProyect.Models;

namespace TamoeProyect.Services
{
    public interface IRepositorioProducts
    {
        Task PostProduct(ProductViewModel modelo);
        Task<IEnumerable<ProductViewModel>> GetAllProducts();
    }
    public class RepositorioProducts : IRepositorioProducts
    {
        private readonly string connectionString;
        public RepositorioProducts(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
       public async Task PostProduct(ProductViewModel modelo)
       {
           
           using var connection = new SqlConnection(connectionString);

           var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Products(Name, Description, Price)
                                                            VALUES(@Name, @Description, @Price)
                                                            SELECT SCOPE_IDENTITY();"
                                                            ,new {modelo.Name, modelo.Description, modelo.Price});

            modelo.Id = id;
       }

        public async Task<IEnumerable<ProductViewModel>> GetAllProducts() 
        {
            Console.WriteLine("Repositorio");
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<ProductViewModel>(@"SELECT Name, Description, Price
                                                        FROM Products");

        }

    }
}
