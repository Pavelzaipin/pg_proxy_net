
namespace ExpressProfiler
{
    
    
    public class SqlStringReader 
        : System.IO.TextReader
    {
        private string? _s;
        private int _pos;
        private int _length;

        public SqlStringReader(string s)
        {
            if (s is null)
            {
                throw new System.ArgumentNullException(nameof(s));
            }

            _s = s;
            _length = s.Length;
        }

        public override void Close()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            _s = null;
            _pos = 0;
            _length = 0;
            base.Dispose(disposing);
        }

        // Returns the next available character without actually reading it from
        // the underlying string. The current position of the StringReader is not
        // changed by this operation. The returned value is -1 if no further
        // characters are available.
        //
        public override int Peek()
        {
            if (_s == null)
            {
                throw new System.ObjectDisposedException(null, "ObjectDisposed-Reader is closed");
            }

            if (_pos == _length)
            {
                return -1;
            }

            return _s[_pos];
        }

        // Reads the next character from the underlying string. The returned value
        // is -1 if no further characters are available.
        //
        public override int Read()
        {
            if (_s == null)
            {
                throw new System.ObjectDisposedException(null, "ObjectDisposed-Reader is closed");
            }

            if (_pos == _length)
            {
                return -1;
            }

            return _s[_pos++];
        }

        // Reads a block of characters. This method will read up to count
        // characters from this StringReader into the buffer character
        // array starting at position index. Returns the actual number of
        // characters read, or zero if the end of the string is reached.
        //
        public override int Read(char[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new System.ArgumentNullException(nameof(buffer), "Argument is NULL: buffer");
            }

            if (index < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(index), "Need mon-negative number.");
            }

            if (count < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(count), "Need mon-negative number.");
            }

            if (buffer.Length - index < count)
            {
                throw new System.ArgumentException("Invalid Off Len");
            }

            if (_s == null)
            {
                throw new System.ObjectDisposedException(null, "ObjectDisposed-Reader is closed");
            }

            int n = _length - _pos;
            if (n > 0)
            {
                if (n > count)
                {
                    n = count;
                }

                _s.CopyTo(_pos, buffer, index, n);
                _pos += n;
            }

            return n;
        }

        public override int Read(System.Span<char> buffer)
        {
            if (GetType() != typeof(SqlStringReader))
            {
                // This overload was added after the Read(char[], ...) overload, and so in case
                // a derived type may have overridden it, we need to delegate to it, which the base does.
                return base.Read(buffer);
            }

            if (_s == null)
            {
                throw new System.ObjectDisposedException(null, "ObjectDisposed-Reader is closed");
            }

            int n = _length - _pos;
            if (n > 0)
            {
                if (n > buffer.Length)
                {
                    n = buffer.Length;
                }
                
                System.MemoryExtensions.AsSpan(_s, _pos, n).CopyTo(buffer);
                _pos += n;
            }

            return n;
        }

        public override int ReadBlock(System.Span<char> buffer) => Read(buffer);

        public override string ReadToEnd()
        {
            if (_s == null)
            {
                throw new System.ObjectDisposedException(null, "ObjectDisposed-Reader is closed");
            }

            string s;
            if (_pos == 0)
            {
                s = _s;
            }
            else
            {
                s = _s.Substring(_pos, _length - _pos);
            }

            _pos = _length;
            return s;
        }

        // Reads a line. A line is defined as a sequence of characters followed by
        // a carriage return ('\r'), a line feed ('\n'), or a carriage return
        // immediately followed by a line feed. The resulting string does not
        // contain the terminating carriage return and/or line feed. The returned
        // value is null if the end of the underlying string has been reached.
        //
        public override string? ReadLine()
        {
            if (_s == null)
            {
                throw new System.ObjectDisposedException(null, "ObjectDisposed-Reader is closed");
            }

            int i = _pos;
            while (i < _length)
            {
                char ch = _s[i];
                if (ch == '\r' || ch == '\n')
                {
                    string result = _s.Substring(_pos, i - _pos);
                    _pos = i + 1;
                    if (ch == '\r' && _pos < _length && _s[_pos] == '\n')
                    {
                        _pos++;
                    }

                    return result;
                }

                i++;
            }

            if (i > _pos)
            {
                string result = _s.Substring(_pos, i - _pos);
                _pos = i;
                return result;
            }

            return null;
        }
    }
}