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

        private Boolean connected = false;

        public Boolean Connected
        {
            get { return connected; }
            set
            {
                connected = value;
                OnPropertyChanged("Connected");
            }
        }

        public Dictionary<UnderlineOption, string> UnderlineOptions
        {
            get
            {
                return new Dictionary<UnderlineOption, string>() // Fix. Each time new dict.?
                {
                    {UnderlineOption.None, "No Underline"},
                    {UnderlineOption.One, "1 Pixel underline"},
                    {UnderlineOption.Two, "2 Pixel underline"},
                };
            }
        }
        public enum UnderlineOption
        {
            None = 0x00,
            One = 0x01,
            Two = 0x02,

        }
        private UnderlineOption underlined = UnderlineOption.None;

        public UnderlineOption Underlined
        {
            get { return underlined; }
            set
            {
                underlined = value;
                WriteCommand(0x1b, 0x2d, (byte)(value), null);
                OnPropertyChanged("Underlined");

            }
        }

        public Dictionary<byte, string> FontXMultipliers
        {
            get
            {
                return new Dictionary<byte, string>() // Fix. Each time new dict.?
                {
                    {0, "Font multiplied by 1 in X axis"},
                    {1, "Font multiplied by 2 in X axis"},
                    {2, "Font multiplied by 3 in X axis"},
                    {3, "Font multiplied by 4 in X axis"},
                    {4, "Font multiplied by 5 in X axis"},
                    {5, "Font multiplied by 6 in X axis"},
                    {6, "Font multiplied by 7 in X axis"},
                    {7, "Font multiplied by 8 in X axis"},
                };
            }
        }
        private Byte fontXMultiplier = 0;

        public Byte FontXMultiplier
        {
            get { return fontXMultiplier; }
            set
            {
                fontXMultiplier = value;
                WriteCommand(0x1D, 0x21, (byte)((value * 0x10 ) + FontYMultiplier), null);
                OnPropertyChanged("FontXMultiplier");

            }
        }

        public Dictionary<byte, string> FontYMultipliers
        {
            get
            {
                return new Dictionary<byte, string>() // FiY. Each time new dict.?
                {
                    {0, "Font multiplied by 1 in Y axis"},
                    {1, "Font multiplied by 2 in Y axis"},
                    {2, "Font multiplied by 3 in Y axis"},
                    {3, "Font multiplied by 4 in Y axis"},
                    {4, "Font multiplied by 5 in Y axis"},
                    {5, "Font multiplied by 6 in Y axis"},
                    {6, "Font multiplied by 7 in Y axis"},
                    {7, "Font multiplied by 8 in Y axis"},
                };
            }
        }
        private Byte fontYMultiplier = 0;

        public Byte FontYMultiplier
        {
            get { return fontYMultiplier; }
            set
            {
                fontYMultiplier = value;
                WriteCommand(0x1D, 0x21, (byte)((FontXMultiplier * 0x10) + value), null);
                OnPropertyChanged("FontYMultiplier");

            }
        }


        public ThermalPrinterTCP()
        {

        }

        public void Connect()
        {
            printer = new TcpClient();
            printer.Connect(new System.Net.IPEndPoint(IPAddress.Parse(PrinterIPAddress), Port));
            Connected = printer.Connected;
        }

        public void Disconnect()
        {
            printer.GetStream().Close();
            Connected = printer.Connected;

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

        public void Reset()
        {
            printer.GetStream().Write(new byte[] { 0x1b, 0x40 }, 0, 2);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Feed(byte lines)
        {
            WriteCommand(0x1b, 0x64, lines, null);
        }


        private void WriteCommand(byte command, byte command2, byte n, byte? m)
        {
            if (printer.Connected)
            {

                if (m != null)
                {
                    printer.GetStream().Write(new byte[] { command, command2, n, (byte)m }, 0, 4);

                }
                else
                {
                    printer.GetStream().Write(new byte[] { command, command2, n }, 0, 3);

                }
            }
        }
    }
}
