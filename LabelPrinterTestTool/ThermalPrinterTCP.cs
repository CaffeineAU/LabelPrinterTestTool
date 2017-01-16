using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LabelPrinterTestTool
{
    public class ThermalPrinterTCP : INotifyPropertyChanged
    {
        TcpClient printer = new TcpClient();

        private String m_IPAddress = "172.21.114.185";

        public String PrinterIPAddress
        {
            get { return m_IPAddress; }
            set
            {
                m_IPAddress = value;
                OnPropertyChanged("PrinterIPAddress");
            }
        }

        private int m_port = 9100;

        public int Port
        {
            get { return m_port; }
            set
            {
                m_port = value;
                OnPropertyChanged("Port");
            }
        }

        public Boolean Connected
        {
            get { return printer.Connected; }
        }

        public ThermalPrinterTCP()
        {
        }

        public void Connect()
        {
            printer.Connect(new System.Net.IPEndPoint(IPAddress.Parse(PrinterIPAddress), Port));
        }

        public void Disconnect()
        {
            printer.Close();
        }

        public void WriteAsciiString(String text)
        {
            text += "\n";

            printer.GetStream().Write(Encoding.ASCII.GetBytes(text), 0, Encoding.ASCII.GetBytes(text).Length);
        }

        public void Cut()
        {
            printer.GetStream().Write(new byte[] { 0x1b, 0x69 }, 0, 2);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        internal void Feed(byte lines)
        {
            printer.GetStream().Write(new byte[] { 0x1b, 0x64, lines }, 0, 3);
        }
    }
}
