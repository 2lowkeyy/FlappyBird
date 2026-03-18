using Microsoft.Data.Sqlite;
using FlappyBird.Models;

namespace FlappyBird.Data
{
    public class BaseDeDatos
    {
        string db = "Data Source=flappybird.db";

        public BaseDeDatos()
        {
            SqliteConnection con = new SqliteConnection(db);
            con.Open();
            SqliteCommand cmd = new SqliteCommand("CREATE TABLE IF NOT EXISTS Scores (Id INTEGER PRIMARY KEY, Nombre TEXT, Pts INTEGER)", con);
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void Guardar(Score s)
        {
            SqliteConnection con = new SqliteConnection(db);
            con.Open();
            SqliteCommand cmd = new SqliteCommand("INSERT INTO Scores (Nombre, Pts) VALUES (@n, @p)", con);
            cmd.Parameters.AddWithValue("@n", s.nombre);
            cmd.Parameters.AddWithValue("@p", s.pts);
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public Score[] Top10()
        {
            SqliteConnection con = new SqliteConnection(db);
            con.Open();
            SqliteCommand cmd = new SqliteCommand("SELECT Id, Nombre, Pts FROM Scores ORDER BY Pts DESC LIMIT 10", con);
            SqliteDataReader r = cmd.ExecuteReader();
            Score[] lista = new Score[10];
            int i = 0;
            while (r.Read())
            {
                Score s  = new Score();
                s.id = r.GetInt32(0);
                s.nombre = r.GetString(1);
                s.pts = r.GetInt32(2);
                lista[i] = s;
                i = i + 1;
            }
            con.Close();
            return lista;
        }
    }
}
