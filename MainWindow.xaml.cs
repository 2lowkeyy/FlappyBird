using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using FlappyBird.Data;
using FlappyBird.Models;

namespace FlappyBird
{
    public partial class MainWindow : Window
    {
        Bird pajaro;
        Pipe[] tubos = new Pipe[5];
        Image[] imgTop = new Image[5];
        Image[] imgBot = new Image[5];

        int score = 0;
        bool jugando = false;
        double contador = 0;

        DispatcherTimer timer;
        Random rand = new Random();
        BaseDeDatos db = new BaseDeDatos();
        BitmapImage top;
        BitmapImage bot;

        public MainWindow()
        {
            InitializeComponent();

            top = new BitmapImage(new Uri("Assets/pipe_top.png", UriKind.Relative));
            bot = new BitmapImage(new Uri("Assets/pipe_bottom.png", UriKind.Relative));

            // esto dibuja los tubos
            for (int i = 0; i < 5; i++)
            {
                tubos[i] = new Pipe();

                imgTop[i] = new Image();
                imgTop[i].Source = top;
                imgTop[i].Width = Pipe.w;
                imgTop[i].Stretch = System.Windows.Media.Stretch.Fill;
                imgTop[i].Visibility = Visibility.Collapsed;
                Canvas.Children.Insert(0, imgTop[i]);

                imgBot[i] = new Image();
                imgBot[i].Source = bot;
                imgBot[i].Width = Pipe.w;
                imgBot[i].Stretch = System.Windows.Media.Stretch.Fill;
                imgBot[i].Visibility = Visibility.Collapsed;
                Canvas.Children.Insert(0, imgBot[i]);
            }

            BirdImg.Visibility = Visibility.Collapsed;
            TxtPuntos.Visibility = Visibility.Collapsed;
        }

        void Jugar()
        {
            if (timer != null)
                timer.Stop();

            for (int i = 0; i < 5; i++)
            {
                tubos[i].vivo = false;
                imgTop[i].Visibility = Visibility.Collapsed;
                imgBot[i].Visibility = Visibility.Collapsed;
            }

            // reset de variables
            pajaro = new Bird();
            score = 29;
            contador = 0;
            jugando = true;
            Pipe.vel = 3;

            TxtPuntos.Text = "0";
            BirdImg.Visibility = Visibility.Visible;
            TxtPuntos.Visibility = Visibility.Visible;
            PanelInicio.Visibility = Visibility.Collapsed;
            PanelGameOver.Visibility = Visibility.Collapsed;
            PanelRecords.Visibility = Visibility.Collapsed;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16);
            timer.Tick += Loop;
            timer.Start();
        }

         // Loop del Juego
        void Loop(object sender, EventArgs e)
        {
            pajaro.Mover();

            if (score >= 30)
                Pipe.vel = 5;

            MoverTubos();

            contador = contador + 16;
            if (contador >= 1800)
            {
                NuevoTubo();
                contador = 0;
            }

            if (Choco())
            {
                GameOver();
                return;
            }

            // renderizar el juego
            Dibujar();
        }

        // Logica pa crear los tubos
        void NuevoTubo()
        {
            for (int i = 0; i < 5; i++)
            {
                if (tubos[i].vivo == false)
                {
                    tubos[i].Activar(rand.Next(150, 390), rand.Next(2) == 0);
                    imgTop[i].Visibility = Visibility.Visible;
                    imgBot[i].Visibility = Visibility.Visible;
                    return;
                }
            }
        }

        void MoverTubos()
        {
            for (int i = 0; i < 5; i++)
            {
                if (tubos[i].vivo == false)
                    continue;

                tubos[i].Mover();

                if (tubos[i].pase == false && pajaro.x > tubos[i].x + Pipe.w)
                {
                    tubos[i].pase = true;
                    score = score + 1;
                    TxtPuntos.Text = score.ToString();
                }

                if (tubos[i].FueraDePantalla())
                {
                    tubos[i].vivo = false;
                    imgTop[i].Visibility = Visibility.Collapsed;
                    imgBot[i].Visibility = Visibility.Collapsed;
                }
            }
        }

        bool Choco()
        {
            // la colision del suelo
            if (pajaro.y + 15 >= 540) return true;

            // lacolision de arriba
            if (pajaro.y - 15 <= 0) return true;

            for (int i = 0; i < 5; i++)
            {
                if (tubos[i].vivo == true && tubos[i].Choco(pajaro))
                    return true;
            }

            return false;
        }

        void Dibujar()
        {
            // mover el pajaro
            Canvas.SetLeft(BirdImg, pajaro.x - 15);
            Canvas.SetTop(BirdImg, pajaro.y - 15);

            // mover los tubos
            for (int i = 0; i < 5; i++)
            {
                if (tubos[i].vivo == false)
                    continue;

                double altoArriba = tubos[i].centro - Pipe.gap / 2;
                double dondeAbajo = tubos[i].centro + Pipe.gap / 2;
                double altoAbajo = 540 - dondeAbajo;

                imgTop[i].Height = altoArriba;
                Canvas.SetLeft(imgTop[i], tubos[i].x);
                Canvas.SetTop(imgTop[i], 0);

                imgBot[i].Height = altoAbajo;
                Canvas.SetLeft(imgBot[i], tubos[i].x);
                Canvas.SetTop(imgBot[i], dondeAbajo);
            }
        }

        // Cuando el jugador pierde 
        void GameOver()
        {
            jugando = false;
            timer.Stop();
            TxtPuntajeFinal.Text = "Puntaje: " + score;
            PanelGameOver.Visibility = Visibility.Visible;
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (jugando == true && (e.Key == Key.Space || e.Key == Key.Up))
                pajaro.Saltar();
        }

        // Logica de los botones
        void Jugar_Click(object sender, RoutedEventArgs e)
        {
            Jugar();
        }

        void Guardar_Click(object sender, RoutedEventArgs e)
        {
            db.Guardar(new Score(TxtNombre.Text, score));
            Jugar();
        }

        void VerRecords_Click(object sender, RoutedEventArgs e)
        {
            PanelGameOver.Visibility = Visibility.Collapsed;
            MostrarRecords();
        }

        void VerRecordsInicio_Click(object sender, RoutedEventArgs e)
        {
            PanelInicio.Visibility = Visibility.Collapsed;
            MostrarRecords();
        }

        void JugarDeNuevo_Click(object sender, RoutedEventArgs e)
        {
            PanelRecords.Visibility = Visibility.Collapsed;
            Jugar();
        }

        void Salir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        void MostrarRecords()
        {
            PanelRecords.Visibility = Visibility.Visible;
            ListaRecords.Items.Clear();
            Score[] lista = db.Top10();
            for (int i = 0; i < lista.Length; i++)
            {
                if (lista[i] != null)
                    ListaRecords.Items.Add(lista[i].ToString());
            }
        }
    }
}