using System.Text.RegularExpressions;
using TamoeProyect.Models;

namespace TamoeProyect.Services
{
    public interface IRepositorioValidation
    {
        bool ValidationEmail(User user);
        bool ValidationPassword(User user);
    }
    public class RepositorioValidation:IRepositorioValidation
    {
        public  bool ValidationEmail(User user)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if(user.Email == null) {
                return false;
            }

            if (Regex.IsMatch(user.Email, pattern))
            {
                return true;
            }


            return false;
        }

        public  bool ValidationPassword(User user)
        {
            if(user.Password == null)
            {
                return false;
            }
            if(user.Password.Length == 0)
            {
                return false;
            }
            return true;
        }
    }
}
