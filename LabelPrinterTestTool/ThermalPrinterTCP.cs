using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LabelPrinterTestTool
{
    public class ThermalPrinterTCP : INotifyPropertyChanged
    {
        //add a comment
        // I see your comment
        //another comment as a collaborator
        TcpClient printer = new TcpClient();

        private String iPAddress = "";

        public String PrinterIPAddress
        {
            get { return iPAddress; }
            set
            {
                iPAddress = value;
                OnPropertyChanged("PrinterIPAddress");
            }
        }

        private String subnetMask = "";

        public String SubnetMask
        {
            get { return subnetMask; }
            set
            {
                subnetMask = value;
                OnPropertyChanged("SubnetMask");
            }
        }

        private String macAddress = "";

        public String MACAddress
        {
            get { return macAddress; }
            set
            {
                macAddress = value;
                OnPropertyChanged("MACAddress");
            }
        }

        private int port = 0;

        public int Port
        {
            get { return port; }
            set
            {
                port = value;
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
                WriteCommand(0x1B, 0x24, (byte)(value & 0xFF), (byte)((value & 0xFF00) / 0x100));

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
                WriteCommand(0x1b, 0x2d, (byte)(value));
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
                WriteCommand(0x1b, 0x56, (byte)(value));
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
                WriteCommand(0x1b, 0x7b, (byte)(value));
                OnPropertyChanged("UpsideDown");

            }
        }

        public Dictionary<NumericOptions, string> PageModeOptions
        {
            get
            {
                return new Dictionary<NumericOptions, string>() // Fix. Each time new dict.?
                {
                    {NumericOptions.Zero, "Standard Mode"},
                    {NumericOptions.One, "PageMode"},
                };
            }
        }
        private NumericOptions pageMode = NumericOptions.Zero;

        public NumericOptions PageMode
        {
            get { return pageMode; }
            set
            {
                pageMode = value;
                WriteCommand(0x1b, (byte)(value == NumericOptions.Zero ? 0x53 : 0x4C));
                OnPropertyChanged("PageMode");

            }
        }

        public Dictionary<NumericOptions, string> PageModePrintDirectionOptions
        {
            get
            {
                return new Dictionary<NumericOptions, string>() // Fix. Each time new dict.?
                {
                    {NumericOptions.Zero, "Left to right, start Upper left"},
                    {NumericOptions.One, "Bottom to top, start Lower left"},
                    {NumericOptions.Two, "Right to left, start Lower right"},
                    {NumericOptions.Three, "Top to bottom, start Upper right"},
                };
            }
        }
        private NumericOptions pageModePrintDirection = NumericOptions.Zero;

        public NumericOptions PageModePrintDirection
        {
            get { return pageModePrintDirection; }
            set
            {
                pageModePrintDirection = value;
                WriteCommand(0x1b, 0x54, (byte)(value));
                OnPropertyChanged("PageModePrintDirection");

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
                WriteCommand(0x1b, 0x45, (byte)(value));
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
                WriteCommand(0x1D, 0x42, (byte)(value));
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
                WriteCommand(0x1B, 0x47, (byte)(value));
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
                WriteCommand(0x1B, 0x4D, (byte)(value));
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
                WriteCommand(0x1D, 0x21, (byte)((value * 0x10) + FontYMultiplier));
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
                WriteCommand(0x1D, 0x21, (byte)((FontXMultiplier * 0x10) + value));
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
            WriteCommand(0x1b, 0x69);
        }

        public void Print()
        {
            WriteCommand(0x0c);
        }

        public void Reset()
        {
            WriteCommand(0x1b, 0x40);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Feed(byte lines)
        {
            WriteCommand(0x1b, 0x64, lines);
        }

        public IPAddress FindPrinter()
        {
            UdpClient finder = new UdpClient();// new IPEndPoint(IPAddress.Any, 48780));
            finder.Client.Bind(new IPEndPoint(IPAddress.Any, 48780));
            //finder.Connect(new IPEndPoint(IPAddress.Broadcast, 48781));
            finder.Client.MulticastLoopback = false;
            finder.Client.EnableBroadcast = true;
            finder.Send(Encoding.ASCII.GetBytes("FIND"), Encoding.ASCII.GetByteCount("FIND"), new IPEndPoint(IPAddress.Broadcast, 48781) );

            int retries = 5;
            while (finder.Client.Available == 0 && retries-- > 0)
            {
                System.Threading.Thread.Sleep(250);

            }
                byte[] buffer = new byte[255];
            while (finder.Client.Available > 0)
            {
                finder.Client.Receive(buffer);
            }
            if (Encoding.ASCII.GetString(new byte[] { buffer[0], buffer[1], buffer[2], buffer[3] }) == "IMIN")
            {
                PrinterIPAddress = new IPAddress(new byte[] { buffer[10], buffer[11], buffer[12], buffer[13] }).ToString();
                Port = buffer[22] * 0x100 + buffer[23];
                MACAddress = String.Format("{0:X2}:{1:X2}:{2:X2}:{3:X2}:{4:X2}:{5:X2}", buffer[4], buffer[5], buffer[6], buffer[7], buffer[8], buffer[9]);
                SubnetMask = new IPAddress(new byte[] { buffer[14], buffer[15], buffer[16], buffer[17] }).ToString();
           }

            //StringBuilder sb = new StringBuilder();
            //foreach (var b in buffer)
            //{
            //    sb.AppendFormat("{0:X2} ", b);
            //}
            //System.Windows.Clipboard.SetText(sb.ToString());

            return IPAddress.Any;
                
        }

        private void WriteCommand(byte command, byte? command2 = null, byte? n = null, byte? m = null)
        {
            if (printer.Connected)
            {
                if (command2 != null)
                {
                    if (n != null)
                    {
                        if (m != null)
                        {
                            printer.GetStream().Write(new byte[] { command, (byte)command2, (byte)n, (byte)m }, 0, 4);
                        }
                        else
                        {
                            printer.GetStream().Write(new byte[] { command, (byte)command2, (byte)n }, 0, 3);
                        }
                    }
                    else
                    {
                        printer.GetStream().Write(new byte[] { command, (byte)command2 }, 0, 2);
                    }
                }
                else
                {
                    printer.GetStream().Write(new byte[] { command }, 0, 1);
                }
            }
        }
    }
}
