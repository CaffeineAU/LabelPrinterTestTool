using Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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

                OnPropertyChanged("TheTicket");

                //SourceImage.DownloadCompleted += delegate
                //{
                //};
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

        public void Export()
        {
            if (!File.Exists(String.Format("{0}{1}.png", System.IO.Path.GetTempPath(), TheTicket.IssueType)))
            {
                using (WebClient wc = new WebClient())
                {
                    string authorization = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(String.Format("{0}:{1}", Properties.Settings.Default.JIRAUsername, Properties.Settings.Default.JIRAPassword)));
                    wc.Headers.Add("Authorization", authorization);
                    wc.DownloadFile(TheTicket.IssueTypeIcon, String.Format("{0}{1}.png", System.IO.Path.GetTempPath(), TheTicket.IssueType));
                    if (File.ReadAllText(String.Format("{0}{1}.png", System.IO.Path.GetTempPath(), TheTicket.IssueType)).StartsWith("<?xml")) // it's an SVG file
                    {
                        var svgDocument = SvgDocument.Open(String.Format("{0}{1}.png", System.IO.Path.GetTempPath(), TheTicket.IssueType));
                        var bitmap = svgDocument.Draw(48, 48);
                        bitmap.Save(String.Format("{0}{1}.png", System.IO.Path.GetTempPath(), TheTicket.IssueType), ImageFormat.Png);
                    }
                }
            }

            SourceImage.BeginInit();
            SourceImage.UriSource = new Uri(String.Format("{0}{1}.png", System.IO.Path.GetTempPath(), TheTicket.Key + TheTicket.IssueType), UriKind.Absolute);
            SourceImage.EndInit();

            ExportToPng(String.Format("{0}{1}.png", System.IO.Path.GetTempPath(), TheTicket.Key), MainCanvas);
            //File.Delete(String.Format("{0}{1}.png", System.IO.Path.GetTempPath(), TheTicket.Key + TheTicket.IssueType));
            DownloadComplete?.Invoke(this, new DownloadEventArgs(TheTicket.Key));


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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Initialized(object sender, EventArgs e)
        {

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
