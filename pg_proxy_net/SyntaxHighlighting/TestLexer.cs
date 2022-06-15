
namespace pg_proxy_net.SyntaxHighlighting
{

    using ExpressProfiler;


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
        } // End Sub DoProcTable 


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
        } // End Sub Next 


    } // End Class TestLexer 


} // End Namespace 
