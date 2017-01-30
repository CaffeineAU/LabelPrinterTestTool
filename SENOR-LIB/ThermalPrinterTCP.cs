using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;


namespace SENOR_LIB
{
    public class GTP_250 : INotifyPropertyChanged
    {
        //add a comment
        // I see your comment
        //another comment as a collaborator
        TcpClient printer = new TcpClient();

        private Boolean appendLF = true;

        public Boolean AppendLF
        {
            get { return appendLF; }
            set
            {
                appendLF = value;
                OnPropertyChanged("AppendLF");
            }
        }


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

        private int lineSpacing = 0;

        public int LineSpacing
        {
            get { return lineSpacing; }
            set
            {
                if (value < 0 || value > 255)
                {
                    WriteCommand(0x1B, 0x32); // default

                }
                else
                {
                    lineSpacing = value;
                    WriteCommand(0x1B, 0x33, (byte)(value));
                }

                OnPropertyChanged("LineSpacing");
                OnPropertyChanged("CalculatedLineSpacing");
            }
        }

        public double CalculatedLineSpacing
        {
            get { return (double)LineSpacing * 0.0705; }
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

        public GTP_250()
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
            if (AppendLF)
            {
                text += "\n";
            }

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

        public void PrintBitImage()
        {
            WriteAsciiString(GetLogo(@"hi5.bmp"));
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
            finder.Send(Encoding.ASCII.GetBytes("FIND"), Encoding.ASCII.GetByteCount("FIND"), new IPEndPoint(IPAddress.Broadcast, 48781));

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

        public string GetLogo(string file)
        {
            //http://stackoverflow.com/questions/14099239/printing-a-bit-map-image-to-pos-printer-via-comport-in-c-sharp

            string logo = "";
            if (!File.Exists(file))
                return null;
            BitmapData data = GetBitmapData(file);
            BitArray dots = data.Dots;
            byte[] width = BitConverter.GetBytes(data.Width);

            int offset = 0;
            MemoryStream stream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(stream);

            bw.Write((char)0x1B);
            bw.Write('@');

            bw.Write((char)0x1B);
            bw.Write('3');
            bw.Write((byte)24);

            while (offset < data.Height)
            {
                bw.Write((char)0x1B);
                bw.Write('*');         // bit-image mode
                bw.Write((byte)33);    // 24-dot double-density
                bw.Write(width[0]);  // width low byte
                bw.Write(width[1]);  // width high byte

                for (int x = 0; x < data.Width; ++x)
                {
                    for (int k = 0; k < 3; ++k)
                    {
                        byte slice = 0;
                        for (int b = 0; b < 8; ++b)
                        {
                            int y = (((offset / 8) + k) * 8) + b;
                            // Calculate the location of the pixel we want in the bit array.
                            // It'll be at (y * width) + x.
                            int i = (y * data.Width) + x;

                            // If the image is shorter than 24 dots, pad with zero.
                            bool v = false;
                            if (i < dots.Length)
                            {
                                v = dots[i];
                            }
                            slice |= (byte)((v ? 1 : 0) << (7 - b));
                        }

                        bw.Write(slice);
                    }
                }
                offset += 24;
                bw.Write((char)0x0A);
            }
            // Restore the line spacing to the default of 30 dots.
            bw.Write((char)0x1B);
            bw.Write('3');
            bw.Write((byte)30);

            bw.Flush();
            byte[] bytes = stream.ToArray();
            return logo + Encoding.Default.GetString(bytes);
        }

        public BitmapData GetBitmapData(string bmpFileName)
        {
            using (var bitmap = (Bitmap)Bitmap.FromFile(bmpFileName))
            {
                var threshold = 127;
                var index = 0;
                double multiplier = 570; // this depends on your printer model. for Beiyang you should use 1000
                double scale = (double)(multiplier / (double)bitmap.Width);
                int xheight = (int)(bitmap.Height * scale);
                int xwidth = (int)(bitmap.Width * scale);
                var dimensions = xwidth * xheight;
                var dots = new BitArray(dimensions);

                for (var y = 0; y < xheight; y++)
                {
                    for (var x = 0; x < xwidth; x++)
                    {
                        var _x = (int)(x / scale);
                        var _y = (int)(y / scale);
                        var color = bitmap.GetPixel(_x, _y);
                        var luminance = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                        dots[index] = (luminance < threshold);
                        index++;
                    }
                }

                return new BitmapData()
                {
                    Dots = dots,
                    Height = (int)(bitmap.Height * scale),
                    Width = (int)(bitmap.Width * scale)
                };
            }
        }

        public class BitmapData
        {
            public BitArray Dots
            {
                get;
                set;
            }

            public int Height
            {
                get;
                set;
            }

            public int Width
            {
                get;
                set;
            }
        }
    }


}
