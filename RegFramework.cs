using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using System.IO;
using System.Management;
using System.Reflection;
using IOEx;

namespace MyMyanmar
{
    class RegFramework
    {
        private static String getKey(String Lock)
        {
            int[] keyholes = {12,98,37,40,92,49,32,87,45,99,37,59,79,70,94,12,94,70,98,58,74,02,37,40,92,34};
            String keyx = "";
            int y = 0;
            for (int i = 0; i < Lock.Length; i++)
            {
                int num = (int)Lock[i] - 48;
                num = ((num * keyholes[y]) / keyholes[y + 1]) + (keyholes[y + 2] / (i + 1));
                if (num < 0) num *= -1;
                keyx += num.ToString();
                y++;
                if (y >= keyholes.Length - 4)
                    y = 0;
            }
            return keyx;
        }


        internal static string getSerial()
        {
            try
            {
                FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\actv.dll",FileMode.Open ,FileAccess.Read);
                CrcStream stream = new CrcStream(file);
                StreamReader reader = new StreamReader(stream);
                string text = reader.ReadToEnd();
                file.Close();
                string s = stream.ReadCrc.ToString("X8");
                if (s == "BD1B9FAB")
                {
                    //Assembly ass = System.Reflection.Assembly.LoadFile(@"C:\MyMyanmar Unicode System\DriveInfoEx.dll");
                    DriveListEx dl = new DriveListEx();
                    Random r = new Random();
                    int y = r.Next();
                    int j = dl.Load(y);
                    long yy = ((y + 1035) / 3 + 666) * 2;
                    if (yy == j)
                    {
                        return getKey(dl[0].SerialNumber);
                    }
                    else
                    {
                        MessageBox.Show("Damaged Activation Control,19");
                        Application.Exit();
                        return null;
                    }
                    //return GetHardDriveId();
                }
                else
                {
                    MessageBox.Show("Damaged Activation Control,12");
                    Application.Exit();
                    return null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Damaged Activation Control,14");
                Application.Exit();
                return null;
            }
        }

/*
        public static string GetHardDriveId()
        {
            string cpuID = "";
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                cpuID = mo.Properties["ProcessorId"].Value.ToString();
                if (cpuID.Length > 0)
                    break;
            }
        }
        */
        public static bool validateProductNumber(string number)
        {
            string first = "";
            string second = "x";
            string third = "a";
            try
            {
                 first = number.Substring(0, number.IndexOf("-"));
                 second = number.Substring(number.IndexOf("-") + 1);
                 third = second.Substring(second.IndexOf("-") + 1);
                second = second.Substring(0, second.IndexOf("-"));
            }
            catch (Exception ex)
            {
                return false;
            }
            if (getKey(first) == second)
            {

                if ( dividesum(getKey(second)) == third)
                {
                    return true;
                }
                else
                {
                    //MessageBox.Show(dividesum(getKey(second)));
                }
            }
            else
            {
                //MessageBox.Show(getKey(first));
            }
            return false;
        }

        private static string dividesum(string s)
        {
            int i = s.Length / 2;
            string first = s.Substring(0, i);
            string second = s.Substring(i);
            int j = Convert.ToInt32(first) + Convert.ToInt32(second);
            j = j / 2;
            return j.ToString();
        }

        private static string fourthdividesum(string s)
        {
            int i = s.Length / 2;
            string first = s.Substring(0, i);
            string second = s.Substring(i);
            int j = first.Length / 2;
            int k = second.Length / 2;

            string one = first.Substring(0, j);
            string two = first.Substring(j);
            string three = second.Substring(0,k);
            string four = second.Substring(k);
            int res = Convert.ToInt32(one) + Convert.ToInt32(two) + Convert.ToInt32(three) + Convert.ToInt32(four);
            return res.ToString();
        }

        internal static string getActivation(string serial)
        {
            string s = getKey(fourthdividesum(serial));
            return s;
        }


        public static bool isActivated()
        {
            if (File.Exists(@"C:\MyMyanmar Unicode System\activation"))
            {
                string s = getSerial();
                StreamReader sr = new StreamReader(@"C:\MyMyanmar Unicode System\activation");
                string ac = sr.ReadLine();
                sr.Close();
                if (ac == getActivation(s))
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
            return false;
        }
    }

    public class CrcStream : Stream
    {
        /// <summary>
        /// Encapsulate a <see cref="System.IO.Stream" />.
        /// </summary>
        /// <param name="stream">The stream to calculate the checksum for.</param>
        public CrcStream(Stream stream)
        {
            this.stream = stream;
        }

        Stream stream;

        /// <summary>
        /// Gets the underlying stream.
        /// </summary>
        public Stream Stream
        {
            get { return stream; }
        }

        public override bool CanRead
        {
            get { return stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return stream.CanWrite; }
        }

        public override void Flush()
        {
            stream.Flush();
        }

        public override long Length
        {
            get { return stream.Length; }
        }

        public override long Position
        {
            get
            {
                return stream.Position;
            }
            set
            {
                stream.Position = value;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            count = stream.Read(buffer, offset, count);
            readCrc = CalculateCrc(readCrc, buffer, offset, count);
            return count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);

            writeCrc = CalculateCrc(writeCrc, buffer, offset, count);
        }

        uint CalculateCrc(uint crc, byte[] buffer, int offset, int count)
        {
            unchecked
            {
                for (int i = offset, end = offset + count; i < end; i++)
                    crc = (crc >> 8) ^ table[(crc ^ buffer[i]) & 0xFF];
            }
            return crc;
        }

        static private uint[] table = GenerateTable();

        static private uint[] GenerateTable()
        {
            unchecked
            {
                uint[] table = new uint[256];

                uint crc;
                const uint poly = 0xEDB88320;
                for (uint i = 0; i < table.Length; i++)
                {
                    crc = i;
                    for (int j = 8; j > 0; j--)
                    {
                        if ((crc & 1) == 1)
                            crc = (crc >> 1) ^ poly;
                        else
                            crc >>= 1;
                    }
                    table[i] = crc;
                }

                return table;
            }

        }

        uint readCrc = unchecked(0xFFFFFFFF);

        /// <summary>
        /// Gets the CRC checksum of the data that was read by the stream thus far.
        /// </summary>
        public uint ReadCrc
        {
            get { return unchecked(readCrc ^ 0xFFFFFFFF); }
        }

        uint writeCrc = unchecked(0xFFFFFFFF);

        /// <summary>
        /// Gets the CRC checksum of the data that was written to the stream thus far.
        /// </summary>
        public uint WriteCrc
        {
            get { return unchecked(writeCrc ^ 0xFFFFFFFF); }
        }

        /// <summary>
        /// Resets the read and write checksums.
        /// </summary>
        public void ResetChecksum()
        {
            readCrc = unchecked(0xFFFFFFFF);
            writeCrc = unchecked(0xFFFFFFFF);
        }
    }
}
