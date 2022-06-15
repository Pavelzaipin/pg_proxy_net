
namespace NetProxy
{


    public class ProcFsReader
    {

        protected char[] m_charArray;
        protected long m_currentPosition;
        protected long m_length;


        public ProcFsReader(string content)
        {
            this.m_charArray = content.ToCharArray();
            this.m_length = this.m_charArray.Length;
            this.m_currentPosition = 0;
        }


        public char? Peek(long offset)
        {
            if (offset < 0 || offset >= this.m_length)
                return null;

            return this.m_charArray[offset];
        }


        public char? Peek()
        {
            return Peek(this.m_currentPosition);
        }


        public char? PeekRelativeOffset(long offset)
        {
            long pos = this.m_currentPosition + offset;
            return Peek(pos);
        }


        public char? Current
        {
            get { return this.Peek(); }
        }


        public char? Next
        {
            get
            {
                return PeekRelativeOffset(1);
            }
        }

        public char? Previous
        {
            get
            {
                return PeekRelativeOffset(-1);
            }
        }


        public bool GoToNext()
        {
            bool ret = true;
            long pos = this.m_currentPosition + 1;
            if (pos < 0 || pos >= this.m_length)
                ret = false;

            this.m_currentPosition = pos;

            return ret;
        }


        public bool GoToPrevious()
        {
            bool ret = true;
            long pos = this.m_currentPosition - 1;
            if (pos < 0 || pos >= this.m_length)
                ret = false;

            this.m_currentPosition = pos;

            return ret;
        }


        public bool IsNewLine
        {
            get
            {
                return (
                  this.Current == '\n'
                  );
            }
        }


        public bool IsWhiteSpace
        {
            get
            {
                char? c = this.Current;
                if (!c.HasValue)
                    return false;

                return (
                  this.Current == '\r' ||
                  this.Current == '\t' ||
                  this.Current == '\v' ||
                  this.Current == '\r' ||
                  this.Current == '\f' ||
                  this.Current == '\u00A0' || // ASCII 0xA0 (160: non-breaking space)
                  this.Current == '\uFEFF' || // Unicode Character 'ZERO WIDTH NO-BREAK SPACE' (U+FEFF)
                  this.Current == ' '
                  );
            }
        }


        public string ReadWhiteSpace()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            while (this.IsWhiteSpace)
            {
                sb.Append(this.Current);
                if (!this.GoToNext())
                    break;
            }

            return sb.ToString();
        }


        public void SkipWhiteSpace()
        {
            while (this.IsWhiteSpace)
            {
                if (!this.GoToNext())
                    break;
            }

        }


        public string? ReadNonWhiteSpace()
        {
            bool hasData = false;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            while (!this.IsWhiteSpace && !this.IsNewLine)
            {
                hasData = true;

                sb.Append(this.Current);
                if (!this.GoToNext())
                    break;
            }

            if (hasData)
                return sb.ToString();

            return null;
        }


        public bool HasNext
        {
            get
            {
                long a = this.m_length - this.m_currentPosition;
                return a > 0;
            }
        }


        public static System.Collections.Generic.List<System.Collections.Generic.List<string>> ReadContent(string content)
        {
            // content = Substitute(content);

            ProcFsReader fsr = new ProcFsReader(content);

            System.Collections.Generic.List<System.Collections.Generic.List<string>> lsLines = new System.Collections.Generic.List<System.Collections.Generic.List<string>>();
            System.Collections.Generic.List<string> ls = new System.Collections.Generic.List<string>();

            while (fsr.HasNext)
            {
                string white = fsr.ReadWhiteSpace();
                string? text = fsr.ReadNonWhiteSpace();
                if (text != null)
                    ls.Add(text);

                if (fsr.IsNewLine)
                {
                    lsLines.Add(ls);
                    ls = new System.Collections.Generic.List<string>();
                } // End if (fsr.IsNewLine) 

                fsr.GoToNext();
            } // Whend 

            lsLines.Add(ls);
            return lsLines;
        } // End Function ReadFile 


        public static System.Collections.Generic.List<System.Collections.Generic.List<string>> ReadFile(string path)
        {
            string content = "";
            try
            {
                // https://zetcode.com/csharp/readtext/
                using (System.IO.Stream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(fs, System.Text.Encoding.UTF8))
                    {
                        content = sr.ReadToEnd();
                    } // End Using sr

                } // End Using fs 

            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine(ex.StackTrace);
                System.Console.Error.WriteLine(ex.Message);
                System.Console.Error.WriteLine(ex.StackTrace);
            }

            return ReadContent(content);
        } // End Function ReadFile 


        public static System.Collections.Generic.List<System.Collections.Generic.List<string>> ReadProcNetTcp()
        {
            // https://www.kernel.org/doc/Documentation/networking/proc_net_tcp.txt

            return ReadFile("/proc/net/tcp");
        } // End Function ReadProcNetTcp 


        internal static void Test()
        {
            string proc = @"  sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode
0: 0100007F:0BEA 00000000:0000 0A 00000000:00000000 00:00000000 00000000   130        0 30439 1 ffff8ff112da71c0 100 0 0 10 0";

            System.Collections.Generic.List<System.Collections.Generic.List<string>> lsLines = ReadContent(proc);

            foreach (System.Collections.Generic.List<string> thisLine in lsLines)
            {
                foreach (string thisValue in thisLine)
                {
                    System.Console.Write(thisValue);
                    System.Console.Write("\t");
                } // Next thisValue 

                System.Console.WriteLine();
            } // Next thisLine 

        } // End Sub Test 


    } // End Class ProcFsReader 


} // End Namespace 
