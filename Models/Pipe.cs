namespace FlappyBird.Models
{
    public class Pipe
    {
        public double x = 500;
        public double centro = 300;
        public double moveY  = 2;    // vel. vertical
        public bool pase = false;
        public bool vivo = false;

        public static double w = 60;
        public static double gap = 150;
        public static double vel = 3;  // vel. horizontal

        public void Activar(double pos, bool subeOBaja)
        {
            x = 400;
            centro = pos;
            pase = false;
            vivo = true;
            moveY = subeOBaja ? 2 : -2;  // dir. aleatoria
        }

        // funcion pa mover el tubo despues de los 30 puntos
        public void Mover()
        {
            x = x - vel;

            if (vel > 3)
            {
                centro = centro + moveY;
                if (centro < 150 || centro > 430)
                    moveY = moveY * -1;
            }
        }

        public bool FueraDePantalla()
        {
            return x + w < 0;
        }

        public bool Choco(Bird b)
        {
            bool enX = b.x + 15 > x && b.x - 15 < x + w;
            bool enY = b.y - 15 < centro - gap / 2 || b.y + 15 > centro + gap / 2;
            return enX && enY;
        }
    }
}
