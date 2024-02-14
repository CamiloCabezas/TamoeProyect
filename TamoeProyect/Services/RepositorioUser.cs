using TamoeProyect.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace TamoeProyect.Services
{
    public interface IRepositorioUser
    {
        Task<string> PostUser(User user);
    }
    public class RepositorioUser: IRepositorioUser
    {
        private readonly string connectionString;
        
        private readonly IRepositorioValidation repositorioValidation;

        public RepositorioUser(IConfiguration configuration,
                               
                               IRepositorioValidation repositorioValidation)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
            
            this.repositorioValidation = repositorioValidation;
        }

        public static string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return $"{Convert.ToBase64String(salt)}:{hashed}";
        }

        public static bool VerifyPassword(string hashedPassword, string password)
        {
            var parts = hashedPassword.Split(':');
            if (parts.Length != 2)
            {
                return false;
            }

            var salt = Convert.FromBase64String(parts[0]);
            var hashedPasswordAttempt = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return parts[1] == hashedPasswordAttempt;
        }



        public async Task<string> PostUser(User user)
        {
            if (!repositorioValidation.ValidationEmail(user))
            {
                Console.WriteLine("El email proporcionado no es válido.");
                return "El email proporcionado no es válido.";
            }

            if (!repositorioValidation.ValidationPassword(user))
            {
                Console.WriteLine("La contraseña proporcionada no es válida.");
                return "La contraseña proporcionada no es válida.";
            }

            try
            {
                using var connection = new SqlConnection(connectionString);

                var Password = HashPassword(user.Password);

                //if(user.Role == null)
                //{
                  // user.Role = "UserNormal";
                //}

                Console.WriteLine($"{user.Role} aca esta el role");

                var id = await connection.QuerySingleAsync<int>(@"INSERT INTO [Tamoe].[dbo].[User] (Name, LastName, Email, Password)
                                                      VALUES(@Name, @LastName, @Email, @Password)
                                                      SELECT SCOPE_IDENTITY();",
                                                                  new { user.Name, user.LastName, user.Email, Password});

                user.Id = id;
                
                
                return "Creación Exitosa!";
            }
            catch (SqlException ex)
            {
                return "Error al crear usuario: " + ex.Message;
            }
            catch (Exception ex)
            {
                return "Error inesperado al crear usuario: " + ex.Message;
            }
        }

    }
}
