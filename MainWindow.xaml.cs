using System.Data.Common;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PictureConcatenater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScaleTransform _scaleTransform;

        public MainWindow()
        {
            InitializeComponent();

            _scaleTransform = new ScaleTransform();
        }

        record class DragTag 
        {
            public bool IsDragging;
            public Point Position;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            var filenames = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string filename in filenames)
            {
                try
                {
                    var image = new Image()
                    {
                        Source = new BitmapImage(new Uri(filename))
                    };
                    image.Tag = new DragTag() { IsDragging = false, Position = new Point() };
                    image.MouseLeftButtonDown += (sender, e) => {
                        var tag = (DragTag)image.Tag;
                        tag.IsDragging = true;
                        tag.Position = e.GetPosition(canvas);
                        foreach (UIElement i in canvas.Children)
                            Panel.SetZIndex(i, 0);
                        Panel.SetZIndex(image, 99);
                    };
                    image.MouseLeftButtonUp += (sender, e) => {
                        var tag = (DragTag)image.Tag;   
                        tag.IsDragging = false;
                    };
                    image.MouseMove += (sender, e) => {
                        var tag = (DragTag)image.Tag;
                        if (!tag.IsDragging)
                            return;
                        var current = e.GetPosition(canvas);

                        var offsetX = current.X - tag.Position.X;
                        var offsetY = current.Y - tag.Position.Y;

                        double left = Canvas.GetLeft(image);
                        double top = Canvas.GetTop(image);
                        Canvas.SetLeft(image, left + offsetX);
                        Canvas.SetTop(image, top + offsetY);

                        tag.Position = current;
                    };
                    image.RenderTransform = _scaleTransform;
                    Canvas.SetLeft(image, 0);
                    Canvas.SetTop(image, 0);
                    canvas.Children.Add(image);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private void canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                _scaleTransform.ScaleX *= 1.1;
                _scaleTransform.ScaleY *= 1.1;
            }
            else
            {
                _scaleTransform.ScaleX /= 1.1;
                _scaleTransform.ScaleY /= 1.1;
            }
        }
    }
}