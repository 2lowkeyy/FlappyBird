namespace FlappyBird.Models
{
    public class Bird
    {
        public double x = 100;
        public double y = 270;
        public double vel = 0;

        public void Mover()
        {
            vel = vel + 0.5;
            y = y + vel;
        }

        public void Saltar()
        {
            vel = -8;
        }
    }
}
