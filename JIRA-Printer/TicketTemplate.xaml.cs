using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
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
using System.Windows.Shapes;

namespace JIRA_Printer
{
    /// <summary>
    /// Interaction logic for TicketTemplate.xaml
    /// </summary>
    public partial class TicketTemplate : Window, INotifyPropertyChanged
    {
        private Ticket theTicket;

        public Ticket TheTicket
        {
            get { return theTicket; }
            set
            {
                theTicket = value;
                SourceImage.BeginInit();
                SourceImage.UriSource = new Uri(TheTicket.StatusIcon, UriKind.Absolute);
                SourceImage.EndInit();
                SourceImage.DownloadCompleted += delegate
                {
                    ExportToPng(String.Format("{0}{1}.png", System.IO.Path.GetTempPath(), TheTicket.Key), MainCanvas);
                    DownloadComplete?.Invoke(this, new DownloadEventArgs(TheTicket.Key));
                };

                OnPropertyChanged("TheTicket");
            }
        }
        private BitmapImage sourceImage = new BitmapImage();

        public BitmapImage SourceImage
        {
            get
            {
                return sourceImage;
            }
            set
            {
                sourceImage = value;
                OnPropertyChanged("SourceImage");
            }
        }

        public TicketTemplate()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        public delegate void DownloadCompleteEventHandler(object sender, DownloadEventArgs e);
        public event DownloadCompleteEventHandler DownloadComplete;


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Export(string path)
        {

            //ExportToPng(path, MainCanvas);
        }

        void ExportToPng(string path, Border surface)
        {
            if (path == null) return;

            // Save current canvas transform
            Transform transform = surface.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            surface.LayoutTransform = null;

            // Get the size of canvas
            Size size = new Size(surface.Width, surface.Height);
            // Measure and arrange the surface
            // VERY IMPORTANT
            surface.Measure(size);
            surface.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(surface);

            // Create a file stream for saving image
            using (FileStream outStream = new FileStream(path, FileMode.Create))
            {
                // Use png encoder for our data
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                // push the rendered bitmap to it
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                // save the data to the stream
                encoder.Save(outStream);
            }

            // Restore previously saved layout
            surface.LayoutTransform = transform;
        }


    }

    public class ProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? 0 : (double)((int)value) * 246 / 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class DownloadEventArgs : EventArgs
    {
        public string FileName { get; private set; }

        public DownloadEventArgs(string fileName)
        {
            FileName = fileName;
        }
    }
}
