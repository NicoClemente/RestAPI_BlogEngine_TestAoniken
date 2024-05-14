using RestAPi_BlogEngine_TestAoniken.Models;

namespace RestAPi_BlogEngine_TestAoniken.Repositories
{
    public interface IUserRepository
    {
        User GetUserByUsername(string username);
        IEnumerable<User> GetAllUsers();
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int userId);
    }
}