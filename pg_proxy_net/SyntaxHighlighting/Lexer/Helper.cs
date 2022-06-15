
namespace pg_proxy_net.SyntaxHighlighting.Lexer
{

    public class Helper
    {

        public static bool IsNewLine(char c)
        {
            if (c == '\r' || c == '\n')
                return true;

            return false;
        }


        public static bool IsWhiteSpace(char c)
        {
            if (c == ' ')
                return true;

            if (c == '\t')
                return true;

            if (c == '\r')
                return true;

            if (c == '\n')
                return true;

            if (c == '\v')
                return true;

            if (c == '\f')
                return true;

            if (c == '\u00A0') // ASCII 0xA0 (160: non-breaking space)
                return true;

            if (c == '\uFEFF') // Unicode Character 'ZERO WIDTH NO-BREAK SPACE' (U+FEFF)
                return true;

            return char.IsWhiteSpace(c);
        }


    }


}
