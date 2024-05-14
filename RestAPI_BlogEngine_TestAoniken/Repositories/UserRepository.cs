using CsvHelper;
using CsvHelper.Configuration;
using RestAPi_BlogEngine_TestAoniken.Models;
using System.Globalization;

namespace RestAPi_BlogEngine_TestAoniken.Repositories
{
    // UserRepository class for handling CRUD operations on User data stored in a CSV file.
    public class UserRepository : IUserRepository
    {
        // Constructor initializes the path to the CSV file.
        private readonly string _csvFilePath;

        public UserRepository()
        {
            string projectDirectory = Directory.GetCurrentDirectory();
            string csvDirectory = Path.Combine(projectDirectory, "Data");
            _csvFilePath = Path.Combine(csvDirectory, "Users.csv");
        }

        // Retrieves a user by their username.
        public User GetUserByUsername(string username)
        {
            var users = LoadUsers();
            return users.FirstOrDefault(u => u.Username == username);
        }

        // Retrieves all users from the CSV file.
        public IEnumerable<User> GetAllUsers()
        {
            return LoadUsers();
        }

        // Adds a new user to the CSV file.
        public void AddUser(User user)
        {
            var users = LoadUsers();
            user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
            users.Add(user);
            SaveUsers(users);
        }

        // Updates an existing user in the CSV file.
        public void UpdateUser(User user)
        {
            var users = LoadUsers();
            var existingUser = users.FirstOrDefault(u => u.Id == user.Id);

            if (existingUser != null)
            {
                existingUser.Username = user.Username;
                existingUser.Password = user.Password;
                existingUser.Role = user.Role;
                SaveUsers(users);
            }
        }

        // Deletes a user from the CSV file by their ID.
        public void DeleteUser(int userId)
        {
            var users = LoadUsers();
            var userToDelete = users.FirstOrDefault(u => u.Id == userId);

            if (userToDelete != null)
            {
                users.Remove(userToDelete);
                SaveUsers(users);
            }
        }

        // Loads all users from the CSV file.
        private List<User> LoadUsers()
        {
            var users = new List<User>();

            if (File.Exists(_csvFilePath))
            {
                using (var reader = new StreamReader(_csvFilePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read(); // Omitir la primera línea (encabezados)
                    csv.ReadHeader(); // Leer encabezados de columna
                    users = csv.GetRecords<User>().ToList();
                }
            }

            return users;
        }

        // Saves all users to the CSV file.
        private void SaveUsers(List<User> users)
        {
            using (var writer = new StreamWriter(_csvFilePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(users);
            }
        }
    }
}