using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Model.Entities;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
    }
}
