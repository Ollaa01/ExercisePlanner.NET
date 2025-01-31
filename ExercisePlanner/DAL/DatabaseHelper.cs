using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace ExercisePlanner.DAL
{ 
    public class DatabaseHelper
    {
        //private const string ConnectionString = @"Data Source=C:\Users\kaboo\OneDrive\Pulpit\ExercisePlanner.NET\ExercisePlanner\Database\exercisePlanner.db;Version=3;";
        //private const string ConnectionString = @"Host=localhost;Port=5432;Username=postgres;Password=3435;Database=exercise_planner;";
        private const string ConnectionString = @"Host=ep-dark-hill-a9itn34i-pooler.gwc.azure.neon.tech;Port=5432;Username=neondb_owner;Password=npg_Ro5UeJpL6nQC;Database=neondb";

        public class SamplePlan
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string SampleExerciseIds { get; set; }
        }



        public static List<SamplePlan> GetSamplePlans()
        {
            var samplePlans = new List<SamplePlan>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT Id, Name, Description, SampleExerciseIds FROM SamplePlans";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            samplePlans.Add(new SamplePlan
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                SampleExerciseIds = reader["SampleExerciseIds"].ToString()
                            });
                        }
                    }
                }
            }

            return samplePlans;
        }


        public static void AddPlanToUserExercises(int userId, int planId, string day)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = @"
                    SELECT se.Id, se.Name, se.Category, se.Reps, se.Sets
                    FROM SampleExercise se
                    JOIN SamplePlans sp ON sp.Id = @PlanId
                    WHERE sp.SampleExerciseIds LIKE '%' || se.Id || '%'";

                        List<(string Name, string Category, int Reps, int Sets)> exercisesToInsert = new List<(string, string, int, int)>();

                        using (var command = new NpgsqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@PlanId", planId);

                            using (var reader = command.ExecuteReader())
                            {
                                if (!reader.HasRows)
                                {
                                    MessageBox.Show("No exercises found for the selected plan.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }

                                while (reader.Read())
                                {
                                    exercisesToInsert.Add((
                                        reader["Name"].ToString(),
                                        reader["Category"].ToString(),
                                        Convert.ToInt32(reader["Reps"]),
                                        Convert.ToInt32(reader["Sets"])
                                    ));
                                }
                            }
                        }

                        using (var insertConnection = new NpgsqlConnection(ConnectionString))
                        {
                            insertConnection.Open();
                            using (var insertTransaction = insertConnection.BeginTransaction())
                            {
                                foreach (var exercise in exercisesToInsert)
                                {
                                    string insertQuery = "INSERT INTO Exercises (Name, Category, Reps, Sets, Day, UserId) " +
                                                         "VALUES (@Name, @Category, @Reps, @Sets, @Day, @UserId)";

                                    using (var insertCommand = new NpgsqlCommand(insertQuery, insertConnection))
                                    {
                                        insertCommand.Parameters.AddWithValue("@Name", exercise.Name);
                                        insertCommand.Parameters.AddWithValue("@Category", exercise.Category);
                                        insertCommand.Parameters.AddWithValue("@Reps", exercise.Reps);
                                        insertCommand.Parameters.AddWithValue("@Sets", exercise.Sets);
                                        insertCommand.Parameters.AddWithValue("@Day", day);
                                        insertCommand.Parameters.AddWithValue("@UserId", userId);
                                        insertCommand.ExecuteNonQuery();
                                    }
                                }

                                insertTransaction.Commit();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public static List<string> GetPlansContainingExercise(int exerciseId)
        {
            var planNames = new List<string>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
            SELECT Name
            FROM SamplePlans
            WHERE SampleExerciseIds LIKE '%' || @ExerciseId || '%'";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ExerciseId", exerciseId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            planNames.Add(reader["Name"].ToString());
                        }
                    }
                }
            }

            return planNames;
        }



        public static List<SampleExercise> GetSampleExercises()
        {
            var sampleExercises = new List<SampleExercise>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT Id, Name, Category, Reps, Sets, Day FROM SampleExercise";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sampleExercises.Add(new SampleExercise
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Category = reader["Category"].ToString(),
                                Reps = Convert.ToInt32(reader["Reps"]),
                                Sets = Convert.ToInt32(reader["Sets"]),
                                Day = reader["Day"].ToString()
                            });
                        }
                    }
                }
            }

            return sampleExercises;
        }


        public static void AddSamplePlan(string name, string description, string sampleExerciseIds)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "INSERT INTO SamplePlans (Name, Description, SampleExerciseIds) VALUES (@Name, @Description, @SampleExerciseIds)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@SampleExerciseIds", sampleExerciseIds);
                    command.ExecuteNonQuery();
                }
            }
        }


        public static void AddSampleExercise(string name, string category, int reps, int sets, string day)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "INSERT INTO SampleExercise (Name, Category, Reps, Sets, Day) VALUES (@Name, @Category, @Reps, @Sets, @Day)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@Reps", reps);
                    command.Parameters.AddWithValue("@Sets", sets);
                    command.Parameters.AddWithValue("@Day", day);
                    command.ExecuteNonQuery();
                }
            }
        }


        public static void UpdateSamplePlan(int id, string name, string description, string sampleExerciseIds)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "UPDATE SamplePlans SET Name = @Name, Description = @Description, SampleExerciseIds = @SampleExerciseIds WHERE Id = @Id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@SampleExerciseIds", sampleExerciseIds);
                    command.ExecuteNonQuery();
                }
            }
        }


        public static void UpdateSampleExercise(int id, string name, string category, int reps, int sets, string day)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "UPDATE SampleExercise SET Name = @Name, Category = @Category, Reps = @Reps, Sets = @Sets, Day = @Day WHERE Id = @Id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@Reps", reps);
                    command.Parameters.AddWithValue("@Sets", sets);
                    command.Parameters.AddWithValue("@Day", day);
                    command.ExecuteNonQuery();
                }
            }
        }


        public static void DeleteSamplePlan(int id)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "DELETE FROM SamplePlans WHERE Id = @Id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }


        public static void DeleteSampleExercise(int id)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "DELETE FROM SampleExercise WHERE Id = @Id";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }


        public static List<int> GetAllExerciseIds()
        {
            var exerciseIds = new List<int>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "SELECT Id FROM SampleExercise";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            exerciseIds.Add(reader.GetInt32(0));
                        }
                    }
                }
            }

            return exerciseIds;
        }


        public class SampleExercise
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public int? Reps { get; set; }
            public string Day { get; set; }
            public int? Sets { get; set; }  
        }


        public class Exercise
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public int? Reps { get; set; }
            public string Day { get; set; }
            public int? Sets { get; set; }  
        }
        public static List<Exercise> GetExercisesByUser(int userId)
        {
            List<Exercise> exercises = new List<Exercise>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT Id, Name, Category, Reps, Sets, Day FROM Exercises WHERE UserId = @UserId";
                using (var command = new NpgsqlCommand(query, connection))
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
                                Sets = reader["Sets"] != DBNull.Value ? Convert.ToInt32(reader["Sets"]) : (int?)null,
                                Day = reader["Day"].ToString(),
                            });
                        }
                    }
                }
            }

            return exercises;
        }



        public static string AddExercise(int userId, string name, string category, int reps, int sets, string day)
        {
            using (var connection = new NpgsqlConnection(ConnectionString)) 
            {
                connection.Open();
                string query = "INSERT INTO Exercises (UserId, Name, Category, Reps, Sets, Day) VALUES (@UserId, @Name, @Category, @Reps, @Sets, @Day)";
                using (var command = new NpgsqlCommand(query, connection)) 
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@Reps", reps);
                    command.Parameters.AddWithValue("@Sets", sets);
                    command.Parameters.AddWithValue("@Day", day);

                    command.ExecuteNonQuery();
                }
            }

            return "Exercise added successfully.";
        }



        public static string EditExercise(int exerciseId, string name, string category, int reps, int sets, string day)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "UPDATE Exercises SET Name = @Name, Category = @Category, Reps = @Reps, Sets = @Sets, Day = @Day WHERE Id = @ExerciseId";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ExerciseId", exerciseId);
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@Reps", reps);
                    command.Parameters.AddWithValue("@Sets", sets);
                    command.Parameters.AddWithValue("@Day", day);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0 ? "Exercise updated successfully." : "Exercise update failed.";
                }
            }
        }



        public static string DeleteExercise(int exerciseId)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "DELETE FROM Exercises WHERE Id = @ExerciseId";
                using (var command = new NpgsqlCommand(query, connection))
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

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                using (var command = new NpgsqlCommand(checkUserQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    long count = (long)command.ExecuteScalar();
                    if (count > 0)
                    {
                        return "Username already exists.";
                    }
                }

                string insertQuery = "INSERT INTO Users (Username, Password, Role) VALUES (@Username, @Password, @Role)";
                using (var command = new NpgsqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Role", role);
                    command.ExecuteNonQuery();
                }
            }

            return "User added successfully.";
        }

        //USERS
        public static string ModifyUser(int userId, string newUsername, string newPassword, string newRole)
        {
            if (string.IsNullOrWhiteSpace(newUsername) || string.IsNullOrWhiteSpace(newPassword))
            {
                return "Username and password cannot be empty.";
            }

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Id = @UserId";
                using (var command = new NpgsqlCommand(checkUserQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    long count = (long)command.ExecuteScalar();
                    if (count == 0)
                    {
                        return "User not found.";
                    }
                }

                string updateQuery = "UPDATE Users SET Username = @Username, Password = @Password, Role = @Role WHERE Id = @UserId";
                using (var command = new NpgsqlCommand(updateQuery, connection))
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

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Id = @UserId";
                using (var command = new NpgsqlCommand(checkUserQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    long count = (long)command.ExecuteScalar();
                    if (count == 0)
                    {
                        return "User not found.";
                    }
                }

                string deleteQuery = "DELETE FROM Users WHERE Id = @UserId";
                using (var command = new NpgsqlCommand(deleteQuery, connection))
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

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "SELECT Id, Username, Role FROM Users";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new User
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Role = reader.GetString(2)
                            };
                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }




        public static string RegisterUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return "Username and password cannot be empty.";
            }

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                using (var command = new NpgsqlCommand(checkUserQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    long count = (long)command.ExecuteScalar();
                    if (count > 0)
                    {
                        return "Username already exists.";
                    }
                }

                string insertQuery = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
                using (var command = new NpgsqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.ExecuteNonQuery();
                }
            }

            return "Registration successful.";
        }


        public static User LoginUser(string username, string password)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT Id, Username, Role FROM Users WHERE Username = @Username AND Password = @Password";
                using (var command = new NpgsqlCommand(query, connection))
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
            return null;
        }


        public static void InitializeDatabase()
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                Console.WriteLine("Połączono z bazą danych: " + connection.Database);

                string createUsersTable = @"
        CREATE TABLE IF NOT EXISTS Users (
            Id SERIAL PRIMARY KEY,
            Username TEXT NOT NULL UNIQUE,
            Password TEXT NOT NULL,
            Role TEXT DEFAULT 'User'
        )";

                string createExercisesTable = @"
        CREATE TABLE IF NOT EXISTS Exercises (
            Id SERIAL PRIMARY KEY,
            UserId INTEGER NOT NULL,
            Name TEXT NOT NULL,
            Category TEXT,
            Reps INTEGER,
            Sets INTEGER,
            Day TEXT,
            FOREIGN KEY(UserId) REFERENCES Users(Id)
        )";

                string createSampleExercisesTable = @"
        CREATE TABLE IF NOT EXISTS SampleExercise (
            Id SERIAL PRIMARY KEY,
            Name TEXT NOT NULL,
            Category TEXT,
            Reps INTEGER,
            Sets INTEGER,
            Day TEXT
        )";

                string createSamplePlansTable = @"
        CREATE TABLE IF NOT EXISTS SamplePlans (
            Id SERIAL PRIMARY KEY,
            Name TEXT NOT NULL,
            Description TEXT,
            SampleExerciseIds TEXT
        )";

                using (var command = new NpgsqlCommand(createUsersTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new NpgsqlCommand(createExercisesTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new NpgsqlCommand(createSampleExercisesTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new NpgsqlCommand(createSamplePlansTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                string checkColumnQuery = @"
        SELECT column_name FROM information_schema.columns 
        WHERE table_name = 'users' AND column_name = 'role';
        ";

                using (var command = new NpgsqlCommand(checkColumnQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        bool roleColumnExists = reader.HasRows;

                        if (!roleColumnExists)
                        {
                            string alterTableQuery = @"
                    ALTER TABLE Users ADD COLUMN Role TEXT DEFAULT 'User';
                    ";

                            using (var alterCommand = new NpgsqlCommand(alterTableQuery, connection))
                            {
                                alterCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }

                string checkAdminQuery = "SELECT COUNT(*) FROM Users WHERE Role = 'Admin';";

                using (var command = new NpgsqlCommand(checkAdminQuery, connection))
                {
                    long count = (long)command.ExecuteScalar();
                    if (count == 0)
                    {
                        string insertAdminQuery = "INSERT INTO Users (Username, Password, Role) VALUES (@Username, @Password, @Role);";
                        using (var insertCommand = new NpgsqlCommand(insertAdminQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@Username", "admin");
                            insertCommand.Parameters.AddWithValue("@Password", "admin");
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

