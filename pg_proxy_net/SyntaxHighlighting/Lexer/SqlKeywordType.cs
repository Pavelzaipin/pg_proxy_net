
namespace pg_proxy_net.SyntaxHighlighting.Lexer
{


    public enum SqlKeywordType : int
    {
        Reserved = 0,
        Function = 1,
        Operator = 2,
        From = 3,
        Identifier = 4,
        String = 5,
        Number = 6,
        Separator = 7,
        Comment = 8,
        OpenBracket = 9,
        CloseBracket = 10,
        Unknown = 11
    }


}
