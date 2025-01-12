
using AjeeviIoT.Models;
using Microsoft.EntityFrameworkCore;

namespace CommonAPI.Services
{
    public class UserService
    {

        private readonly ferrodbContext _dbContext;

        // Constructor injection of the DB context
        public UserService(ferrodbContext dBContext)
        {
            _dbContext = dBContext;
        }

        // Method to get all Data

    

        public List<UserLogin> GetAllUser()
        {
            try
            {
                return _dbContext.UserLogins.ToList();
            }
            catch
            {
                return new List<UserLogin>();
            }
        }

        // Method to get by  id Data
        public UserLogin? GetUserById(long id)
        {
            try
            {
                return _dbContext.UserLogins.Find(id);
            }
            catch
            {
                return null;
            }
        }

        // Method to add a new User
     
     
        public UserLogin? Login(string username, string password)
        {
            return _dbContext.UserLogins.Include(i => i.Userrole).Where
                (w => w.Email == username && w.Password == password && w.Status == true).FirstOrDefault();
        }


    }
}