using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Runtime.InteropServices;
using System.IO.Compression;
using MessageDll;

namespace StormMeeting
{
    public class ShareScreen
    {
        private NetworkStream m_networkStream;
        Socket m_socket;
        IFormatter formatter = new BinaryFormatter();

        public ShareScreen(bool isPresenter)
        {
            IPAddress adress = IPAddress.Parse("192.168.0.112");
            IPEndPoint ipEnd = new IPEndPoint(adress, 5000);
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                m_socket.Connect(ipEnd);
            }
            catch (SocketException e)
            {
                m_socket.Close();
            }
            m_networkStream = new NetworkStream(m_socket);

            //if (isPresenter)
            //{
                Thread shareThread = new Thread(new ThreadStart(BeginShare));
                shareThread.Start();
            //}
            //else
            //{
                Thread receiveThread = new Thread(new ThreadStart(ReceiveImmage));
                receiveThread.Start();
            //}  
        }


        private void BeginShare ()
        {
            while (m_socket.Connected)
            {               
                PrintScreen();
            }
        }

        private void ReceiveImmage()
        {
            while (m_socket.Connected)
            {
               //Bitmap image = (Bitmap)
                byte[] pngBytes =(byte[])formatter.Deserialize(m_networkStream);
                using (MemoryStream ms = new MemoryStream(Compressor.Decompress(pngBytes)))
                {
                     Image image = Image.FromStream(ms);
                    
                        ClientImageReceivedEventArgs messageArgs = new ClientImageReceivedEventArgs(image);
                        OnImmageReceived(messageArgs);

                }
             }
        }

        private void PrintScreen()
        {

            using (Bitmap printscreen = CaptureScreen())
            {

                using (MemoryStream ms = new MemoryStream())
                {
                    printscreen.Save(ms, ImageFormat.Png);

                    byte[] bytes = ms.ToArray();

                    formatter.Serialize(m_networkStream, Compressor.Compress(bytes));
                }
                               
            }
         
        }

        #region capture screen

        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINTAPI
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll")]
        static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        const Int32 CURSOR_SHOWING = 0x00000001;

        public static Bitmap CaptureScreen()
        {
            Bitmap screen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format24bppRgb);
            try
            {
                using (Graphics graphics = Graphics.FromImage(screen))
                {
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

                    graphics.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);

                    CURSORINFO pci;
                    pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CURSORINFO));

                    if (GetCursorInfo(out pci))
                    {
                        if (pci.flags == CURSOR_SHOWING)
                        {
                            if (pci.hCursor.ToInt32() == 65541)
                                pci.hCursor = (IntPtr)65539;
                            DrawIcon(graphics.GetHdc(), pci.ptScreenPos.x, pci.ptScreenPos.y, pci.hCursor);
                            graphics.ReleaseHdc();
                        }
                    }             
                }
            }
            catch
            {
                screen = null;
            }

            return screen;
        }
        #endregion

        #region Events

        public event EventHandler ImmageReceived;

        protected void OnImmageReceived(EventArgs e)
        {
            if (ImmageReceived != null)
                ImmageReceived(this, e);
        }

        #endregion

        
    }
}
