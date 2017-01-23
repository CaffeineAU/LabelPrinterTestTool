using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace LabelPrinterTestTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {


        private ThermalPrinterTCP printer = new ThermalPrinterTCP();

        public ThermalPrinterTCP Printer
        {
            get { return printer; }
            set
            {
                printer = value;
                OnPropertyChanged("Printer");
            }
        }


        private String textToSend = "";

        public String TextToSend
        {
            get { return textToSend; }
            set
            {
                textToSend = value;
                OnPropertyChanged("TextToSend");
            }
        }

        private Byte feedCount = 5;

        public Byte FeedCount
        {
            get { return feedCount; }
            set
            {
                feedCount = value;
                OnPropertyChanged("FeedCount");
            }
        }



        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void SendTextCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !String.IsNullOrEmpty(TextToSend) && Printer.Connected;
        }

        private void SendTextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
             printer.WriteAsciiString(TextToSend);
       }


        //private void ConnectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = !Printer.Connected;
        //}

        private void ConnectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Printer.Connected)
            {
                Printer.Disconnect();
            }
            else
            {
                Printer.Connect();
            }
        }

        private void CutCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Printer.Connected;
        }

        private void CutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            printer.Cut();
        }


        private void FeedCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Printer.Connected;
        }

        private void FeedCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            printer.Feed(FeedCount);
        }

        private void ResetCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Printer.Connected;
        }

        private void ResetCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            printer.Reset();
        }

        private void PrintCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Printer.Connected && Printer.PageMode == ThermalPrinterTCP.NumericOptions.One;
        }

        private void PrintCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            printer.Print();
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


    }
}
