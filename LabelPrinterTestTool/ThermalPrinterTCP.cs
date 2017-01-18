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

        private int printPosition = 0;

        public int PrintPosition
        {
            get { return printPosition; }
            set
            {
                printPosition = value;
                WriteCommand(0x1B, 0x24, (byte)((value & 0xFF00) / 0x100), (byte)(value & 0xFF));

                OnPropertyChanged("PrintPosition");
                OnPropertyChanged("CalculatedPrintPosition");
            }
        }

        public double CalculatedPrintPosition
        {
            get { return (double)PrintPosition * 0.141; }
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
        public enum NumericOptions
        {
            Zero = 0x00,
            One = 0x01,
            Two = 0x02,
            Three = 0x03,
            Four = 0x04,
            Five = 0x05,

        }

        public Dictionary<NumericOptions, string> UnderlineOptions
        {
            get
            {
                return new Dictionary<NumericOptions, string>() // Fix. Each time new dict.?
                {
                    {NumericOptions.Zero, "No Underline"},
                    {NumericOptions.One, "1 Pixel underline"},
                    {NumericOptions.Two, "2 Pixel underline"},
                };
            }
        }
        private NumericOptions underlined = NumericOptions.Zero;

        public NumericOptions Underlined
        {
            get { return underlined; }
            set
            {
                underlined = value;
                WriteCommand(0x1b, 0x2d, (byte)(value), null);
                OnPropertyChanged("Underlined");

            }
        }

        public Dictionary<NumericOptions, string> RotationOptions
        {
            get
            {
                return new Dictionary<NumericOptions, string>() // Fix. Each time new dict.?
                {
                    {NumericOptions.Zero, "No Rotation"},
                    {NumericOptions.One, "90° Clockwise Rotation"},
                };
            }
        }
        private NumericOptions rotated = NumericOptions.Zero;

        public NumericOptions Rotated
        {
            get { return rotated; }
            set
            {
                rotated = value;
                WriteCommand(0x1b, 0x56, (byte)(value), null);
                OnPropertyChanged("Rotated");

            }
        }

        public Dictionary<NumericOptions, string> UpsideDownOptions
        {
            get
            {
                return new Dictionary<NumericOptions, string>() // Fix. Each time new dict.?
                {
                    {NumericOptions.Zero, "Right Side Up"},
                    {NumericOptions.One, "Upside Down"},
                };
            }
        }
        private NumericOptions upsideDown = NumericOptions.Zero;

        public NumericOptions UpsideDown
        {
            get { return upsideDown; }
            set
            {
                upsideDown = value;
                WriteCommand(0x1b, 0x7b, (byte)(value), null);
                OnPropertyChanged("UpsideDown");

            }
        }

        public Dictionary<NumericOptions, string> EmphasisedOptions
        {
            get
            {
                return new Dictionary<NumericOptions, string>() // Fix. Each time new dict.?
                {
                    {NumericOptions.Zero, "Not Emphasised"},
                    {NumericOptions.One, "Emphasised"},
                };
            }
        }
        private NumericOptions emphasised = NumericOptions.Zero;

        public NumericOptions Emphasised
        {
            get { return emphasised; }
            set
            {
                emphasised = value;
                WriteCommand(0x1b, 0x45, (byte)(value), null);
                OnPropertyChanged("Emphasised");

            }
        }

        public Dictionary<NumericOptions, string> BlackWhiteOptions
        {
            get
            {
                return new Dictionary<NumericOptions, string>() // Fix. Each time new dict.?
                {
                    {NumericOptions.Zero, "Black On White"},
                    {NumericOptions.One, "White On Black"},
                };
            }
        }

        private NumericOptions blackonwhite = NumericOptions.Zero;

        public NumericOptions BlackOnWhite
        {
            get { return blackonwhite; }
            set
            {
                blackonwhite = value;
                WriteCommand(0x1D, 0x42, (byte)(value), null);
                OnPropertyChanged("BlackOnWhite");

            }
        }

        public Dictionary<NumericOptions, string> DoubleStrikeOptions
        {
            get
            {
                return new Dictionary<NumericOptions, string>() // Fix. Each time new dict.?
                {
                    {NumericOptions.Zero, "No Double Strike"},
                    {NumericOptions.One, "Double Strike"},
                };
            }
        }

        private NumericOptions doubleStrike = NumericOptions.Zero;

        public NumericOptions DoubleStrike
        {
            get { return doubleStrike; }
            set
            {
                doubleStrike = value;
                WriteCommand(0x1B, 0x47, (byte)(value), null);
                OnPropertyChanged("DoubleStrike");

            }
        }

        public Dictionary<NumericOptions, string> FontOptions
        {
            get
            {
                return new Dictionary<NumericOptions, string>() // Fix. Each time new dict.?
                {
                    {NumericOptions.Zero, "Font A (12 x 24)"},
                    {NumericOptions.One, "Font B (9 x 24)"},
                };
            }
        }

        private NumericOptions font = NumericOptions.Zero;

        public NumericOptions Font
        {
            get { return font; }
            set
            {
                font = value;
                WriteCommand(0x1B, 0x4D, (byte)(value), null);
                OnPropertyChanged("Font");

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
