using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExercisePlanner.DAL
{
    public class DatabaseHelper
    {
        private const string ConnectionString = @"Data Source=C:\Users\PC\Desktop\dotnet projekt\ExercisePlanner\ExercisePlanner\Database\exercisePlanner.db;Version=3;";


        public class Exercise
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public int? Reps { get; set; }
            public string Day { get; set; }
        }

        public static List<Exercise> GetExercisesByUser(int userId)
        {
            List<Exercise> exercises = new List<Exercise>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT Id, Name, Category, Reps, Day FROM Exercises WHERE UserId = @UserId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            exercises.Add(new Exercise
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Category = reader["Category"].ToString(),
                                Reps = reader["Reps"] != DBNull.Value ? Convert.ToInt32(reader["Reps"]) : (int?)null,
                                Day = reader["Day"].ToString(),
                            });
                        }
                    }
                }
            }

            return exercises;
        }

        public static string AddExercise(int userId, string name, string category, int reps, string day)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "INSERT INTO Exercises (UserId, Name, Category, Reps, Day) VALUES (@UserId, @Name, @Category, @Reps, @Day)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@Reps", reps);
                    command.Parameters.AddWithValue("@Day", day);

                    command.ExecuteNonQuery();
                }
            }

            return "Exercise added successfully.";
        }

        public static string EditExercise(int exerciseId, string name, string category, int reps, string day)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "UPDATE Exercises SET Name = @Name, Category = @Category, Reps = @Reps, Day = @Day WHERE Id = @ExerciseId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ExerciseId", exerciseId);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@Reps", reps);
                    command.Parameters.AddWithValue("@Day", day);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0 ? "Exercise updated successfully." : "Exercise update failed.";
                }
            }
        }

        public static string DeleteExercise(int exerciseId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "DELETE FROM Exercises WHERE Id = @ExerciseId";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ExerciseId", exerciseId);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0 ? "Exercise deleted successfully." : "Exercise deletion failed.";
                }
            }
        }






        public static string AddUser(string username, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return "Username and password cannot be empty.";
            }

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // Sprawdź, czy użytkownik już istnieje
                string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                using (var command = new SQLiteCommand(checkUserQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    long count = (long)command.ExecuteScalar();
                    if (count > 0)
                    {
                        return "Username already exists.";
                    }
                }

                // Dodaj nowego użytkownika
                string insertQuery = "INSERT INTO Users (Username, Password, Role) VALUES (@Username, @Password, @Role)";
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Role", role);
                    command.ExecuteNonQuery();
                }
            }

            return "User added successfully.";
        }

        public static string ModifyUser(int userId, string newUsername, string newPassword, string newRole)
        {
            if (string.IsNullOrWhiteSpace(newUsername) || string.IsNullOrWhiteSpace(newPassword))
            {
                return "Username and password cannot be empty.";
            }

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // Sprawdź, czy użytkownik istnieje
                string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Id = @UserId";
                using (var command = new SQLiteCommand(checkUserQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    long count = (long)command.ExecuteScalar();
                    if (count == 0)
                    {
                        return "User not found.";
                    }
                }

                // Zaktualizuj dane użytkownika
                string updateQuery = "UPDATE Users SET Username = @Username, Password = @Password, Role = @Role WHERE Id = @UserId";
                using (var command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Username", newUsername);
                    command.Parameters.AddWithValue("@Password", newPassword);
                    command.Parameters.AddWithValue("@Role", newRole);
                    command.ExecuteNonQuery();
                }
            }

            return "User modified successfully.";
        }


        public static string DeleteUser(int userId)
        {
            if (userId <= 0)
            {
                return "Invalid user ID.";
            }

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // Sprawdź, czy użytkownik istnieje
                string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Id = @UserId";
                using (var command = new SQLiteCommand(checkUserQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    long count = (long)command.ExecuteScalar();
                    if (count == 0)
                    {
                        return "User not found.";
                    }
                }

                // Usuń użytkownika
                string deleteQuery = "DELETE FROM Users WHERE Id = @UserId";
                using (var command = new SQLiteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.ExecuteNonQuery();
                }
            }

            return "User deleted successfully.";
        }


        public class User
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Role { get; set; }
        }
        public static List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = "SELECT Id, Username, Role FROM Users";
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new User
                            {
                                Id = (int)(long)reader["Id"],  // Jawna konwersja na long, a potem na int
                                Username = (string)reader["Username"],
                                Role = (string)reader["Role"]
                            };
                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }




        // Funkcja do rejestracji użytkownika
        public static string RegisterUser(string username, string password)
        {
            // Sprawdzenie, czy pola nie są puste
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return "Username and password cannot be empty."; // Komunikat błędu
            }

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // Sprawdź, czy użytkownik już istnieje
                string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                using (var command = new SQLiteCommand(checkUserQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    long count = (long)command.ExecuteScalar();
                    if (count > 0)
                    {
                        return "Username already exists."; // Komunikat o istniejącym użytkowniku
                    }
                }

                // Dodaj nowego użytkownika
                string insertQuery = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password); // Pamiętaj o szyfrowaniu haseł w produkcji!
                    command.ExecuteNonQuery();
                }
            }

            return "Registration successful."; // Komunikat o udanej rejestracji
        }


        // Funkcja do logowania użytkownika
        public static User LoginUser(string username, string password)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT Id, Username, Role FROM Users WHERE Username = @Username AND Password = @Password";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Role = reader.GetString(2)
                            };
                        }
                    }
                }
            }
            return null; // Nie znaleziono użytkownika
        }


        public static void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // Tworzenie tabeli Users, jeśli nie istnieje
                string createUsersTable = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                Pa                             ssword TEXT NOT NULL,
                Role TEXT DEFAULT 'User'
            )";

                // Tworzenie tabeli Exercises, jeśli nie istnieje
                string createExercisesTable = @"
            CREATE TABLE IF NOT EXISTS Exercises (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                Name TEXT NOT NULL,
                Category TEXT,
                Reps INTEGER,
                Day TEXT,
                FOREIGN KEY(UserId) REFERENCES Users(Id)
            )";

                // Tworzenie tabeli
                using (var command = new SQLiteCommand(createUsersTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SQLiteCommand(createExercisesTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                // Sprawdzanie, czy kolumna Role już istnieje w tabeli Users
                string checkColumnQuery = @"
            PRAGMA table_info(Users);
        ";

                using (var command = new SQLiteCommand(checkColumnQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        bool roleColumnExists = false;

                        while (reader.Read())
                        {
                            if (reader["name"].ToString() == "Role")
                            {
                                roleColumnExists = true;
                                break;
                            }
                        }

                        // Jeśli kolumna Role nie istnieje, dodaj ją
                        if (!roleColumnExists)
                        {
                            string alterTableQuery = @"
                        ALTER TABLE Users ADD COLUMN Role TEXT DEFAULT 'User';
                    ";

                            using (var alterCommand = new SQLiteCommand(alterTableQuery, connection))
                            {
                                alterCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }

                // Sprawdzanie, czy admin istnieje
                string checkAdminQuery = "SELECT COUNT(*) FROM Users WHERE Role = 'Admin'";
                using (var command = new SQLiteCommand(checkAdminQuery, connection))
                {
                    long count = (long)command.ExecuteScalar();
                    if (count == 0)
                    {
                        // Jeśli nie ma admina, dodajemy go
                        string insertAdminQuery = "INSERT INTO Users (Username, Password, Role) VALUES (@Username, @Password, @Role)";
                        using (var insertCommand = new SQLiteCommand(insertAdminQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@Username", "admin");
                            insertCommand.Parameters.AddWithValue("@Password", "admin"); // Możesz ustawić inne hasło
                            insertCommand.Parameters.AddWithValue("@Role", "Admin");
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }


        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }
    }
}
