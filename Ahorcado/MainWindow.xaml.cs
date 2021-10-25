using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ahorcado
{

    public partial class MainWindow : Window
    {
        private int numLetrasPorAcertar;
        private int numVidas = 7;
        private bool partidaActiva = true;
        private string letrasYaUsadas = "";

        public MainWindow()
        {
            InitializeComponent();
            CrearLetrasBotones();
            CrearPalabraSecreta();
        }

        private void TecladoKeyDown(object sender, KeyEventArgs e)
        {
            if (partidaActiva)
            {
                string letra = e.Key == Key.Oem3 ? "Ñ" : e.Key.ToString();

                //Compruebo el tamaño del string para que no se cuenten las teclas que no son letras
                if (!letrasYaUsadas.Contains(letra) && letra.Length == 1)
                {
                    BuscarLetra(letra);
                    letrasYaUsadas += letra;

                    foreach (AspectRatioLayoutDecorator item in LetrasUniformGrid.Children)
                    {
                        Button boton = item.Child as Button;

                        if (boton.Tag.ToString() == letra) boton.IsEnabled = false;
                    }
                }
            }
        }
        private void LetrasClick(object sender, RoutedEventArgs e)
        {
            string letra = (sender as Button).Tag.ToString();

            BuscarLetra(letra);

            (sender as Button).IsEnabled = false;
        }

        private void RendirseClick(object sender, RoutedEventArgs e)
        {
            PartidaAcabada();
        }

        private void NuevaClick(object sender, RoutedEventArgs e)
        {
            numVidas = 7;
            partidaActiva = true;
            letrasYaUsadas = "";

            PalabraOcultaDockPanel.Children.Clear();

            CrearPalabraSecreta();

            foreach (AspectRatioLayoutDecorator decorador in LetrasUniformGrid.Children)
            {
                Button boton = decorador.Child as Button;
                boton.IsEnabled = true;
            }
        }

        private void BajarVidas()
        {
            numVidas--;
            if (numVidas < 0) PartidaAcabada();

            /*
             * Falta cambiar la imagen con cada vida
             * Imagen.Source = new BitmapImage(new Uri(@"/yourApp;component/img.png", UriKind.Relative));
             */
        }

        private void BuscarLetra(string letra)
        {
            bool letraEncontrada = false;

            foreach (TextBlock letraDisplay in PalabraOcultaDockPanel.Children)
            {
                if (letraDisplay.Tag.ToString() == letra)
                {
                    letraDisplay.Text = letraDisplay.Tag.ToString();
                    numLetrasPorAcertar--;
                    letraEncontrada = true;
                    if (numLetrasPorAcertar <= 0) PartidaAcabada();
                }
            }

            if (!letraEncontrada) BajarVidas();
        }

        private void PartidaAcabada()
        {
            foreach (TextBlock letraDisplay in PalabraOcultaDockPanel.Children)
            {
                letraDisplay.Text = letraDisplay.Tag.ToString();
            }

            foreach(AspectRatioLayoutDecorator decorador in LetrasUniformGrid.Children)
            {
                Button boton = decorador.Child as Button;
                boton.IsEnabled = false;
            }

            partidaActiva = false;
        }

        private void CrearPalabraSecreta()
        {
            string[] coleccionPalabras = { "SAÑA", "MONA", "GATO", "VISITACION", "DIVERSAS", "ENTREDICHO", "ZONA", "FAMA", "CRONOPIO",
                "HIPERESPACIO", "HUNDIRSE", "VELOCIDAD", "SACUDIDA", "MANTO", "HUECA", "ENTONCES", "TERRAPLANISTA", "DADO", "PLATEADO", "DIMETRODON", "THRINAXODON",
                "BROOMISTEGA", "HERMANAS", "HOMBRES", "LINAJE", "REGRESAR", "CUCAÑA", "MAÑO", "ESPAÑITA", "ABANICO", "SULFITO", "SED", "VITELO", "MANUAL", 
                "IMPERIO", "PARAMO", "RECONCILIACION", "MOSAICO", "MONJE", "BANQUERO", "LAVANDA", "DECANTACION", "PALABRA", "ASTERION", "PENACHO", "PSICOPOMPO"};
            Random semilla = new Random();
            string palabraSecreta = coleccionPalabras[semilla.Next(coleccionPalabras.Length)] ;
            numLetrasPorAcertar = palabraSecreta.Count();

            foreach (char letra in palabraSecreta)
            {
                PalabraOcultaDockPanel.Children.Add(new TextBlock
                {
                    Tag = letra,
                    Text = "_",
                    Style = (Style)Resources["display"]
                });
            }
            
            //Una pista para las palabras largas
            if (palabraSecreta.Length > 6)
            {
                string pista = palabraSecreta[semilla.Next(palabraSecreta.Length)].ToString();

                BuscarLetra(pista);
                letrasYaUsadas += pista;

                foreach (AspectRatioLayoutDecorator item in LetrasUniformGrid.Children)
                {
                    Button boton = item.Child as Button;

                    if (boton.Tag.ToString() == pista) boton.IsEnabled = false;
                }
            }
   
        }

        private void CrearLetrasBotones()
        {
            int numeroFilas = LetrasUniformGrid.Rows;
            int numeroColumnas = LetrasUniformGrid.Columns;
            string abecedario = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ";
            int contador = 0;

            for (int fila = 0; fila < numeroFilas; fila++)
            {
                for (int columna = 0; columna < numeroColumnas; columna++, contador++)
                {
                    Viewbox view = new Viewbox
                    {
                        Child = new TextBlock { Text = abecedario[contador].ToString() }
                    };

                    Button botonNuevo = new Button
                    {
                        Tag = abecedario[contador],
                        Content = view,
                        Style = (Style) Resources["botonLetra"]
                    };

                    AspectRatioLayoutDecorator decorador = new AspectRatioLayoutDecorator
                    {
                        AspectRatio = 1.1,
                        Child = botonNuevo
                    };

                    _ = LetrasUniformGrid.Children.Add(decorador);
                }
            }
        }

        public class AspectRatioLayoutDecorator : Decorator
        {
            public static readonly DependencyProperty AspectRatioProperty =
               DependencyProperty.Register(
                  "AspectRatio",
                  typeof(double),
                  typeof(AspectRatioLayoutDecorator),
                  new FrameworkPropertyMetadata
                     (
                        1d,
                        FrameworkPropertyMetadataOptions.AffectsMeasure
                     ),
                  ValidateAspectRatio);

            private static bool ValidateAspectRatio(object value)
            {
                if (!(value is double))
                {
                    return false;
                }

                var aspectRatio = (double)value;
                return aspectRatio > 0
                         && !double.IsInfinity(aspectRatio)
                         && !double.IsNaN(aspectRatio);
            }

            public double AspectRatio
            {
                get { return (double)GetValue(AspectRatioProperty); }
                set { SetValue(AspectRatioProperty, value); }
            }

            protected override Size MeasureOverride(Size constraint)
            {
                if (Child != null)
                {
                    constraint = SizeToRatio(constraint, false);
                    Child.Measure(constraint);

                    if (double.IsInfinity(constraint.Width)
                       || double.IsInfinity(constraint.Height))
                    {
                        return SizeToRatio(Child.DesiredSize, true);
                    }

                    return constraint;
                }

                // we don't have a child, so we don't need any space
                return new Size(0, 0);
            }

            public Size SizeToRatio(Size size, bool expand)
            {
                double ratio = AspectRatio;

                double height = size.Width / ratio;
                double width = size.Height * ratio;

                if (expand)
                {
                    width = Math.Max(width, size.Width);
                    height = Math.Max(height, size.Height);
                }
                else
                {
                    width = Math.Min(width, size.Width);
                    height = Math.Min(height, size.Height);
                }

                return new Size(width, height);
            }

            protected override Size ArrangeOverride(Size arrangeSize)
            {
                if (Child != null)
                {
                    var newSize = SizeToRatio(arrangeSize, false);

                    double widthDelta = arrangeSize.Width - newSize.Width;
                    double heightDelta = arrangeSize.Height - newSize.Height;

                    double top = 0;
                    double left = 0;

                    if (!double.IsNaN(widthDelta)
                       && !double.IsInfinity(widthDelta))
                    {
                        left = widthDelta / 2;
                    }

                    if (!double.IsNaN(heightDelta)
                       && !double.IsInfinity(heightDelta))
                    {
                        top = heightDelta / 2;
                    }

                    var finalRect = new Rect(new Point(left, top), newSize);
                    Child.Arrange(finalRect);
                }

                return arrangeSize;
            }
        }
    }
}
