
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


    public class TestLexer
    {
        private readonly Sqltokens m_Tokens = new Sqltokens();

        private int m_StringLen;
        private int m_TokenPos;
        private string m_Token = "";
        private string m_Line;
        private int m_Run;

        private YukonLexer.TokenKind m_TokenId;

        private enum SqlRange
        {
            rsUnknown,
            rsComment,
            rsString
        }

        private SqlRange Range { get; set; }


        public TestLexer()
        {
            System.Array.Sort(m_IdentifiersArray);
        } // End Constructor 


        private string Line
        {
            set
            {
                Range = SqlRange.rsUnknown;
                m_Line = value;
                m_Run = 0;
                Next();
            }
        }


        private YukonLexer.TokenKind TokenId
        {
            get { return m_TokenId; }
        }

        private string Token
        {
            get
            {
                /*int len = m_Run - m_TokenPos; return m_Line.Substring(m_TokenPos, len);*/
                return m_Token;
            }
        }

        public void LexMe(string value)
        {
            Line = value;

            while (TokenId != YukonLexer.TokenKind.tkNull)
            {
                System.Console.Write(this.TokenId);
                System.Console.Write(": ");
                System.Console.WriteLine(this.Token);


                Next();
            }
        }


        private char GetChar(int idx)
        {
            return idx >= m_Line.Length ? '\x00' : m_Line[idx];
        }


        private void NullProc()
        {
            m_TokenId = YukonLexer.TokenKind.tkNull;
        }

        // ReSharper disable InconsistentNaming
        private void LFProc()
        {
            m_TokenId = YukonLexer.TokenKind.tkSpace;
            m_Run++;
        }

        private void CRProc()
        {
            m_TokenId = YukonLexer.TokenKind.tkSpace;
            m_Run++;
            if (GetChar(m_Run) == '\x0A') m_Run++;
        }
        // ReSharper restore InconsistentNaming

        private void AnsiCProc()
        {
            switch (GetChar(m_Run))
            {
                case '\x00':
                    NullProc();
                    break;
                case '\x0A':
                    LFProc();
                    break;
                case '\x0D':
                    CRProc();
                    break;

                default:
                {
                    m_TokenId = YukonLexer.TokenKind.tkComment;
                    char c;
                    do
                    {
                        if (GetChar(m_Run) == '*' && GetChar(m_Run + 1) == '/')
                        {
                            Range = SqlRange.rsUnknown;
                            m_Run += 2;
                            break;
                        }

                        m_Run++;
                        c = GetChar(m_Run);
                    } while (!(c == '\x00' || c == '\x0A' || c == '\x0D'));

                    break;
                }
            }
        }


        private void AsciiCharProc()
        {
            if (GetChar(m_Run) == '\x00')
            {
                NullProc();
            }
            else
            {
                m_TokenId = YukonLexer.TokenKind.tkString;
                if (m_Run > 0 || Range != SqlRange.rsString || GetChar(m_Run) != '\x27')
                {
                    Range = SqlRange.rsString;
                    char c;
                    do
                    {
                        m_Run++;
                        c = GetChar(m_Run);
                    } while (!(c == '\x00' || c == '\x0A' || c == '\x0D' || c == '\x27'));

                    if (GetChar(m_Run) == '\x27')
                    {
                        m_Run++;
                        Range = SqlRange.rsUnknown;
                    }
                }
            }
        }

        private void EqualProc()
        {
            m_TokenId = YukonLexer.TokenKind.tkSymbol;
            m_Run++;
            if (GetChar(m_Run) == '=' || GetChar(m_Run) == '>') m_Run++;
        }


        private void GreaterProc()
        {
            m_TokenId = YukonLexer.TokenKind.tkSymbol;
            m_Run++;
            if (GetChar(m_Run) == '=' || GetChar(m_Run) == '>') m_Run++;
        }


        private void LowerProc()
        {
            m_TokenId = YukonLexer.TokenKind.tkSymbol;
            m_Run++;
            switch (GetChar(m_Run))
            {
                case '=':
                    m_Run++;
                    break;
                case '<':
                    m_Run++;
                    if (GetChar(m_Run) == '=') m_Run++;
                    break;
            }
        }


        private void PlusProc()
        {
            m_TokenId = YukonLexer.TokenKind.tkSymbol;
            m_Run++;
            if (GetChar(m_Run) == '=' || GetChar(m_Run) == '=') m_Run++;
        }

        private void MinusProc()
        {
            m_Run++;
            if (GetChar(m_Run) == '-')
            {
                m_TokenId = YukonLexer.TokenKind.tkComment;
                char c;
                do
                {
                    m_Run++;
                    c = GetChar(m_Run);
                } while (!(c == '\x00' || c == '\x0A' || c == '\x0D'));
            }
            else
            {
                m_TokenId = YukonLexer.TokenKind.tkSymbol;
            }
        }


        private void SlashProc()
        {
            m_Run++;
            switch (GetChar(m_Run))
            {
                case '*':
                {
                    Range = SqlRange.rsComment;
                    m_TokenId = YukonLexer.TokenKind.tkComment;
                    do
                    {
                        m_Run++;
                        if (GetChar(m_Run) == '*' && GetChar(m_Run + 1) == '/')
                        {
                            Range = SqlRange.rsUnknown;
                            m_Run += 2;
                            break;
                        }
                    } while (!(GetChar(m_Run) == '\x00' || GetChar(m_Run) == '\x0D' || GetChar(m_Run) == '\x0A'));
                }
                    break;
                case '=':
                    m_Run++;
                    m_TokenId = YukonLexer.TokenKind.tkSymbol;
                    break;
                default:
                    m_TokenId = YukonLexer.TokenKind.tkSymbol;
                    break;
            }
        }


        private void AndSymbolProc()
        {
            m_TokenId = YukonLexer.TokenKind.tkSymbol;
            m_Run++;
            if (GetChar(m_Run) == '=' || GetChar(m_Run) == '&') m_Run++;
        }


        private void OrSymbolProc()
        {
            m_TokenId = YukonLexer.TokenKind.tkSymbol;
            m_Run++;
            if (GetChar(m_Run) == '=' || GetChar(m_Run) == '|') m_Run++;
        }


        private void QuoteProc()
        {
            m_TokenId = YukonLexer.TokenKind.tkIdentifier;
            m_Run++;
            while (!(GetChar(m_Run) == '\x00' || GetChar(m_Run) == '\x0A' || GetChar(m_Run) == '\x0D'))
            {
                if (GetChar(m_Run) == '\x22')
                {
                    m_Run++;
                    break;
                }

                m_Run++;
            }
        }


        private void BracketProc()
        {
            m_TokenId = YukonLexer.TokenKind.tkIdentifier;
            m_Run++;
            while (!(GetChar(m_Run) == '\x00' || GetChar(m_Run) == '\x0A' || GetChar(m_Run) == '\x0D'))
            {
                if (GetChar(m_Run) == ']')
                {
                    m_Run++;
                    break;
                }

                m_Run++;
            }
        }


        private void VariableProc()
        {
            if (GetChar(m_Run) == '@' && GetChar(m_Run + 1) == '@')
            {
                m_Run += 2;
                IdentProc();
            }
            else
            {
                m_TokenId = YukonLexer.TokenKind.tkVariable;
                int i = m_Run;
                do
                {
                    i++;
                } while (!(IdentifierStr.IndexOf(GetChar(i)) == -1));

                m_Run = i;
            }
        }


        private void SymbolProc()
        {
            m_Run++;
            m_TokenId = YukonLexer.TokenKind.tkSymbol;
        }


        private void SymbolAssignProc()
        {
            m_TokenId = YukonLexer.TokenKind.tkSymbol;
            m_Run++;
            if (GetChar(m_Run) == '=') m_Run++;
        }


        const string IdentifierStr = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890_#$";

        private readonly char[] m_IdentifiersArray = IdentifierStr.ToCharArray();

        private void KeyHash(int pos)
        {
            m_StringLen = 0;
            while (System.Array.BinarySearch(m_IdentifiersArray, GetChar(pos)) >= 0)
            {
                m_StringLen++;
                pos++;
            }

            return;
        }

        private YukonLexer.TokenKind IdentKind()
        {
            KeyHash(m_Run);
            return m_Tokens[m_Line.Substring(m_TokenPos, m_Run + m_StringLen - m_TokenPos)];
        }


        private void IdentProc()
        {
            m_TokenId = IdentKind();
            m_Run += m_StringLen;
            if (m_TokenId == YukonLexer.TokenKind.tkComment)
            {
                while (!(GetChar(m_Run) == '\x00' || GetChar(m_Run) == '\x0A' || GetChar(m_Run) == '\x0D'))
                {
                    m_Run++;
                }
            }
            else
            {
                while (IdentifierStr.IndexOf(GetChar(m_Run)) != -1) m_Run++;
            }
        }

        const string HexDigits = "1234567890abcdefABCDEF";
        const string NumberStr = "1234567890.-";

        private void NumberProc()
        {
            m_TokenId = YukonLexer.TokenKind.tkNumber;
            if (GetChar(m_Run) == '0' && (GetChar(m_Run + 1) == 'X' || GetChar(m_Run + 1) == 'x'))
            {
                m_Run += 2;
                while (HexDigits.IndexOf(GetChar(m_Run)) != -1) m_Run++;
                return;
            }

            m_Run++;
            m_TokenId = YukonLexer.TokenKind.tkNumber;
            while (NumberStr.IndexOf(GetChar(m_Run)) != -1)
            {
                if (GetChar(m_Run) == '.' && GetChar(m_Run + 1) == '.') break;
                m_Run++;
            }
        }


        private void SpaceProc()
        {
            m_TokenId = YukonLexer.TokenKind.tkSpace;
            char c;
            do
            {
                m_Run++;
                c = GetChar(m_Run);
            } while (!(c > '\x20' || c == '\x00' || c == '\x0A' || c == '\x0D'));
        }


        private void UnknownProc()
        {
            m_Run++;
            m_TokenId = YukonLexer.TokenKind.tkUnknown;
        }


        private void DoInsideProc(char chr)
        {
            if ((chr >= 'A' && chr <= 'Z') || (chr >= 'a' && chr <= 'z') || (chr == '_') || (chr == '#'))
            {
                IdentProc();
                return;
            }

            if (chr >= '0' && chr <= '9')
            {
                NumberProc();
                return;
            }

            if ((chr >= '\x00' && chr <= '\x09') || (chr >= '\x0B' && chr <= '\x0C') ||
                (chr >= '\x0E' && chr <= '\x20'))
            {
                SpaceProc();
                return;
            }

            UnknownProc();
        }

        private void DoProcTable(char chr)
        {
            switch (chr)
            {
                case '\x00':
                    NullProc();
                    break;
                case '\x0A':
                    LFProc();
                    break;
                case '\x0D':
                    CRProc();
                    break;
                case '\x27':
                    AsciiCharProc();
                    break;

                case '=':
                    EqualProc();
                    break;
                case '>':
                    GreaterProc();
                    break;
                case '<':
                    LowerProc();
                    break;
                case '-':
                    MinusProc();
                    break;
                case '|':
                    OrSymbolProc();
                    break;
                case '+':
                    PlusProc();
                    break;
                case '/':
                    SlashProc();
                    break;
                case '&':
                    AndSymbolProc();
                    break;
                case '\x22':
                    QuoteProc();
                    break;
                case ':':
                case '@':
                    VariableProc();
                    break;
                case '^':
                case '%':
                case '*':
                case '!':
                    SymbolAssignProc();
                    break;
                case '{':
                case '}':
                case '.':
                case ',':
                case ';':
                case '?':
                case '(':
                case ')':
                case ']':
                case '~':
                    SymbolProc();
                    break;
                case '[':
                    BracketProc();
                    break;
                default:
                    DoInsideProc(chr);
                    break;
            }
        }


        private void Next()
        {
            m_TokenPos = m_Run;
            switch (Range)
            {
                case SqlRange.rsComment:
                    AnsiCProc();
                    break;
                case SqlRange.rsString:
                    AsciiCharProc();
                    break;
                default:
                    DoProcTable(GetChar(m_Run));
                    break;
            }

            m_Token = m_Line.Substring(m_TokenPos, m_Run - m_TokenPos);
        }


    }


}