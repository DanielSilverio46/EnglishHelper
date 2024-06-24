#pragma warning disable CS8605, CS8625

using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Data.Sqlite;

namespace EnglishHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public struct DataUser(uint points, uint fails, Label points_label = null, Label fails_label = null)
        {
            uint Points = points;
            uint Fails = fails;

            Label point_label = points_label;
            Label fail_label = fails_label;

            public void AddPoint()
            { Points++; }

            public void AddFail()
            { Fails++; }

            public uint ReturnPoints()
            { return Points; }

            public uint ReturnFails()
            { return Fails; }
 
            public void OnLabels(Label points, Label fails)
            {
                point_label = points;
                fail_label = fails;
            }

            public void UpdateLabel(string before_points = "", string after_points = "", string before_fails = "", string after_fails = "")
            {
                if (point_label != null && fail_label != null)
                {
                    point_label.Content = before_points + Points.ToString() + after_points;
                    fail_label.Content = before_fails + Fails.ToString() + after_fails;
                }
            }
        }

        public struct DataWords(string portuguese, string english)
        {
            public string Portuguese = portuguese;
            public string English = english;

            public bool PortugueseMatchWord(string word)
            {
                if (Portuguese == word)
                    return true;

                return false;
            }

            public void ChangeWords(string portuguese, string english)
            {
                Portuguese = portuguese;
                English = english;
            }
        };

        public class SQL
        {
            public static SqliteConnection CreateSqlConn()
            {
                return new SqliteConnection("Data Source=mydatabase.db");
            }

            public static void SetupSql()
            {
                using (SqliteConnection connection = CreateSqlConn())
                {
                    connection.Open();
                    SqliteCommand command = connection.CreateCommand();

                    command.CommandText = "CREATE TABLE IF NOT EXISTS words (id INTEGER NOT NULL, portuguese TEXT NOT NULL, english TEXT NOT NULL, PRIMARY KEY (id))";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT OR IGNORE INTO words (id, portuguese, english) VALUES (0, 'gato', 'cat')";
                    command.ExecuteNonQuery();
                  
                }
            }

            public static int ReturnTotalRows()
            {
                int result = 0;

                using (SqliteConnection connection = CreateSqlConn())
                {
                    connection.Open();
                    SqliteCommand command = connection.CreateCommand();

                    command.CommandText = "SELECT id FROM words ORDER BY id DESC LIMIT 1000";
                    result = (int)(long)command.ExecuteScalar();
                }

                return result + 1;
            }

            public static void RandomWord(out string portuguese_word, out string english_word)
            {
                portuguese_word = String.Empty;
                english_word = String.Empty;

                using (SqliteConnection connection = CreateSqlConn())
                {
                    connection.Open();
                    SqliteCommand command = connection.CreateCommand();

                    int random_index = new Random().Next(maxValue: ReturnTotalRows());

                    command.CommandText = "SELECT * FROM words WHERE id = $index";
                    command.Parameters.AddWithValue("$index", random_index);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            portuguese_word = reader.GetString(1);
                            english_word = reader.GetString(2);
                        }
                    }
                }
            }

            public static void WordsIntoBank(string str, uint index, string separator = ":")
            {
                if (str.IndexOf(separator) > -1) {
                    string[] words = str.Split(separator);

                    string port = words[0], eng = words[1];
                    using (SqliteConnection connection = CreateSqlConn())
                    {
                        connection.Open();
                        SqliteCommand command = connection.CreateCommand();

                        command.CommandText = @"INSERT INTO words (id, portuguese, english)
                        VALUES (@index, @portuguese, @english)
                        ON CONFLICT(id) DO UPDATE SET id = excluded.id, portuguese = excluded.portuguese, english = excluded.english";

                        List<SqliteParameter> list = new List<SqliteParameter>();
                        list.Add(new SqliteParameter("index", index));
                        list.Add(new SqliteParameter("portuguese", port));
                        list.Add(new SqliteParameter("english", eng));

                        command.Parameters.AddRange(list.ToArray<SqliteParameter>());
                        command.ExecuteNonQuery();
                    }

                } else {
                    throw new ArgumentException("Value of parameter is unusable", nameof(separator));
                }
            }

            public static int AllWordsToTextBox(uint maximum_words, TextBox box)
            {
                string words_formated = String.Empty;
                int index = -1;

                using (SqliteConnection connection = CreateSqlConn())
                {
                    connection.Open();
                    SqliteCommand command = connection.CreateCommand();

                    for (index = 0; index < maximum_words; index++)
                    {
                        command.CommandText = "SELECT * FROM words WHERE id = @index";
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@index", index);

                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                words_formated += reader.GetString(1) + ':' + reader.GetString(2) + ";\r\n";
                            }
                        }
                    }
                }

                box.Text = words_formated;

                return index;
            }
        }
    }
}
