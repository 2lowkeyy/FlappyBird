namespace FlappyBird.Models
{
    public class Score
    {
        public int id = 0;
        public string nombre = "";
        public int pts = 0;

        public Score() { }

        public Score(string nombre, int pts)
        {
            this.nombre = nombre;
            this.pts = pts;
        }

        public override string ToString()
        {
            return nombre + " - " + pts + " pts";
        }
    }
}
