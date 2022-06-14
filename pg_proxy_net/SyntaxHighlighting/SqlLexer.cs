
namespace AnySQL
{



    public enum SqlSyntaxTokenType
    {
        String,
        DashComment,
        SlashComment,
        StatementSeparator,
        QuotedIdentifier,
        SingleArgumentOperator,
        DoubleArgumentOperator,
        Undefined
    }

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


    [System.Diagnostics.DebuggerDisplay("Text = {m_text}, SyntaxTokenType = {SyntaxTokenType} KeywordType = {KeywordType}")]
    public class SqlToken
    {
        public SqlSyntaxTokenType SyntaxTokenType;
        public SqlKeywordType KeywordType;


        protected string m_text = "";


        public SqlToken? Previous;
        public SqlToken? Next;

        public bool IsJoinStatement;
        public bool IsPotentialJoinStatement;
        public bool NeedsPostProcessing;








        public SqlToken(string text, SqlSyntaxTokenType syntaxTokenType, SqlKeywordType keywordType)
        {
            this.NeedsPostProcessing = false;
            this.IsJoinStatement = false;
            this.IsPotentialJoinStatement = false;
            this.SyntaxTokenType = syntaxTokenType;
            this.KeywordType = keywordType;
            this.Text = text;
        }


        public SqlToken(string text)
            : this(text, SqlSyntaxTokenType.Undefined, SqlKeywordType.Unknown)
        { }


        public SqlToken()
            : this("")
        { }








        public string HtmlText
        {

            get
            {
                return System.Web.HttpUtility.HtmlEncode(m_text).Replace(" ", "&nbsp;").Replace("\r\n", "\n").Replace("\n", "<br />");
            }
        }

        public string Text
        {

            get { return m_text; }
            set
            {
                this.m_text = value;

                if ("(".Equals(m_text))
                {
                    this.KeywordType = SqlKeywordType.OpenBracket;
                }
                else if (")".Equals(m_text))
                {
                    this.KeywordType = SqlKeywordType.CloseBracket;

                }
                else if ("LEFT".Equals(this.m_text, System.StringComparison.InvariantCultureIgnoreCase))
                    this.NeedsPostProcessing = true;
                else if ("RIGHT".Equals(this.m_text, System.StringComparison.InvariantCultureIgnoreCase))
                    this.NeedsPostProcessing = true;
                else if ("INNER".Equals(this.m_text, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    this.IsPotentialJoinStatement = true;
                    this.NeedsPostProcessing = true;
                }
                else if ("OUTER".Equals(this.m_text, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    this.IsPotentialJoinStatement = true;
                    this.NeedsPostProcessing = true;
                }

                else if ("FULL".Equals(this.m_text, System.StringComparison.InvariantCultureIgnoreCase))
                    this.NeedsPostProcessing = true;
                else if ("CROSS".Equals(this.m_text, System.StringComparison.InvariantCultureIgnoreCase))
                    this.NeedsPostProcessing = true;
                else if ("NATURAL".Equals(this.m_text, System.StringComparison.InvariantCultureIgnoreCase))
                    this.NeedsPostProcessing = true;
                else if ("JOIN".Equals(this.m_text, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    this.IsJoinStatement = true;
                    this.KeywordType = SqlKeywordType.From;
                }
                else if ("APPLY".Equals(this.m_text, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    this.IsJoinStatement = true;
                    this.NeedsPostProcessing = true;
                }
                else if ("LATERAL".Equals(this.m_text, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    this.IsJoinStatement = true;
                    this.KeywordType = SqlKeywordType.From;
                }
                else if ("NOT".Equals(this.m_text, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    this.KeywordType = SqlKeywordType.Operator;
                    this.SyntaxTokenType = SqlSyntaxTokenType.SingleArgumentOperator;
                }
                else if ("AND".Equals(this.m_text, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    this.KeywordType = SqlKeywordType.Operator;
                    this.SyntaxTokenType = SqlSyntaxTokenType.DoubleArgumentOperator;
                }
                else if ("OR".Equals(this.m_text, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    this.KeywordType = SqlKeywordType.Operator;
                    this.SyntaxTokenType = SqlSyntaxTokenType.DoubleArgumentOperator;
                }
                else if ("ANY".Equals(this.m_text, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    this.KeywordType = SqlKeywordType.Operator;
                    this.SyntaxTokenType = SqlSyntaxTokenType.DoubleArgumentOperator;
                }
                else if ("ALL".Equals(this.m_text, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    this.KeywordType = SqlKeywordType.Operator;
                    this.SyntaxTokenType = SqlSyntaxTokenType.DoubleArgumentOperator;
                    this.NeedsPostProcessing = true;
                }
                else if ("LIKE".Equals(this.m_text, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    this.KeywordType = SqlKeywordType.Operator;
                    this.SyntaxTokenType = SqlSyntaxTokenType.DoubleArgumentOperator;
                }



            }
        }




        public SqlToken? NextNonSeparatorIncludingBracket()
        {
            SqlToken? foo = this;

            while ((foo = foo.Next) != null)
            {
                if (
                       (
                        (foo.SyntaxTokenType != SqlSyntaxTokenType.StatementSeparator)
                        || (foo.KeywordType != SqlKeywordType.OpenBracket)
                        || (foo.KeywordType != SqlKeywordType.CloseBracket)
                       )
                    && (foo.SyntaxTokenType != SqlSyntaxTokenType.DashComment)
                    && (foo.SyntaxTokenType != SqlSyntaxTokenType.SlashComment)
                )
                    return foo;
            }

            return foo;
        }



        public SqlToken? NextNonSeparator()
        {
            SqlToken? foo = this;

            while ((foo = foo.Next) != null)
            {
                if (
                       (foo.SyntaxTokenType != SqlSyntaxTokenType.StatementSeparator)
                    && (foo.SyntaxTokenType != SqlSyntaxTokenType.DashComment)
                    && (foo.SyntaxTokenType != SqlSyntaxTokenType.SlashComment)
                )
                    return foo;
            }

            return foo;
        }


        public SqlToken? PreviousNonSeparator()
        {
            SqlToken? foo = this;

            while ((foo = foo.Previous) != null)
            {
                if (
                       (foo.SyntaxTokenType != SqlSyntaxTokenType.StatementSeparator)
                    && (foo.SyntaxTokenType != SqlSyntaxTokenType.DashComment)
                    && (foo.SyntaxTokenType != SqlSyntaxTokenType.SlashComment)
                )
                    return foo;
            }

            return foo;
        }


    }


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



    public class SqlStringReader
    {

        protected char[] m_charArray;

        protected long m_currentPosition;
        protected long m_length;


        public static string Substitute(string text)
        {
            return text
                   .Replace("''", "\b") // BS
                   .Replace("\"\"", "\u0005") // ENQ
                   .Replace("]]", "\u0006") // ACK
            ;
        }


        public static string BackSubstitute(string text)
        {
            return text
                   .Replace("\b", "''")  // BS
                   .Replace("\u0005", "\"\"")// ENQ
                   .Replace("\u0006", "]]")// ACK
            ;
        }



        public static System.Text.StringBuilder Substitute(System.Text.StringBuilder text)
        {
            return text
                   .Replace("''", "\b") // BS
                   .Replace("\"\"", "\u0005") // ENQ
                   .Replace("]]", "\u0006") // ACK
            ;
        }


        public static System.Text.StringBuilder BackSubstitute(System.Text.StringBuilder text)
        {
            return text
                   .Replace("\b", "''")  // BS
                   .Replace("\u0005", "\"\"")// ENQ
                   .Replace("\u0006", "]]")// ACK
            ;
        }



        public SqlStringReader(string sql)
        {
            sql = Substitute(sql);

            this.m_charArray = sql.ToCharArray();
            this.m_length = this.m_charArray.Length;
            this.m_currentPosition = -1;
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
            long pos = this.m_currentPosition + 1;
            if (pos < 0 || pos >= this.m_length)
                return false;

            this.m_currentPosition = pos;

            return true;
        }


        public bool GoToPrevious()
        {
            long pos = this.m_currentPosition - 1;
            if (pos < 0 || pos >= this.m_length)
                return false;

            this.m_currentPosition = pos;

            return true;
        }


        public bool IsBeginSlashStarComment
        {
            get
            {
                return this.Current == '/' && this.Next == '*';
            }
        }

        public bool IsEndSlashStarComment
        {
            get
            {
                return this.Current == '*' && this.Next == '/';
            }
        }

        public bool IsBeginDashDashComment
        {
            get { return this.Current == '-' && this.Next == '-'; }
        }


        public bool IsEndOfLine
        {
            get { return this.Current == '\n'; }
        }


        public bool IsQuote
        {
            get { return this.Current == '\''; }
        }


        public bool IsDoubleQuote
        {
            get { return this.Current == '"'; }
        }

        public bool IsOpenSquareBracket
        {
            get { return this.Current == '['; }
        }

        public bool IsCloseSquareBracket
        {
            get { return this.Current == ']'; }
        }


        public bool IsBackspace
        {
            get { return this.Current == '\b'; }
        }


        public bool IsEnquiry
        {
            get { return this.Current == '\u0005'; }// ENQ
        }

        public bool IsAcknowledge
        {
            get { return this.Current == '\u0006'; } // ACK
        }


        public bool IsComma
        {
            get { return this.Current == ','; }
        }

        public bool IsSemicolon
        {
            get { return this.Current == ';'; }
        }

        public bool IsBracket
        {
            get { return (this.Current == '(' || this.Current == ')'); }
        }


        public bool IsOperator
        {
            get
            {
                return (
                  this.Current == '!' ||
                  this.Current == '<' ||
                  this.Current == '>' ||
                  this.Current == '+' ||
                  this.Current == '-' ||
                  this.Current == '*' ||
                  this.Current == '/' ||
                  this.Current == '%' ||
                  this.Current == '~' ||
                  this.Current == '^' ||
                  this.Current == '&' ||
                  this.Current == '|'
                  );
            }
        }


        public bool IsDoubleOperator
        {
            get
            {
                return (
                    (this.Current == '&' && this.Next == '&') ||
                    (this.Current == '|' && this.Next == '|') ||
                    (this.Current == '!' && this.Next == '=') ||
                    (this.Current == '~' && this.Next == '=') ||
                    (this.Current == '=' && this.Next == '=') ||
                    (this.Current == '<' && this.Next == '=') ||
                    (this.Current == '>' && this.Next == '=') ||
                    (this.Current == '<' && this.Next == '>') ||
                    (this.Current == '+' && this.Next == '=') ||
                    (this.Current == '-' && this.Next == '=') ||
                    (this.Current == '*' && this.Next == '=') ||
                    (this.Current == '/' && this.Next == '=') ||
                    (this.Current == '%' && this.Next == '=') ||
                    (this.Current == '&' && this.Next == '=') ||
                    (this.Current == '|' && this.Next == '=') ||
                    (this.Current == '^' && this.Next == '=')
                    );
            }
        }


        public bool IsWhiteSpace
        {
            get
            {
                char? c = this.Current;

                if (c.HasValue)
                    return Helper.IsWhiteSpace(c.Value);

                return true;
            }
        }


        public bool NextIsWhiteSpace
        {
            get
            {
                char? c = this.Next;

                if (c.HasValue)
                    return Helper.IsWhiteSpace(c.Value);

                return true;
            }
        }

        public bool PreviousIsWhiteSpace
        {
            get
            {
                char? c = this.Previous;

                if (c.HasValue)
                    return Helper.IsWhiteSpace(c.Value);

                return true;
            }
        }


        // SELECT 123 AS hel--lo
        // SELECT '123'AS hel--lo
        // SELECT '12/*3'AS hel--lo
        public bool IsStatementSeparator
        {
            get
            {
                char? c = this.Current;

                if (c.HasValue)
                {
                    if (Helper.IsWhiteSpace(c.Value))
                        return true;

                    if (c.Value == '\b')
                        return true;

                    if (c.Value == ',')
                        return true;

                    if (c.Value == ';')
                        return true;

                    if (c.Value == '(')
                        return true;

                    if (c.Value == ')')
                        return true;

                    if (this.IsBeginDashDashComment)
                        return true;

                    if (this.IsBeginSlashStarComment)
                        return true;

                    if (this.IsQuote)
                        return true;
                }

                return false;
            }
        }

        public string ReadQuotedString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(this.Current);

            while (this.GoToNext())
            {
                sb.Append(this.Current);

                if (this.IsQuote)
                {
                    return BackSubstitute(sb).ToString();
                }
            }

            return BackSubstitute(sb).ToString();
        }


        public string ReadDoubleQuotedString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(this.Current);

            while (this.GoToNext())
            {
                sb.Append(this.Current);

                if (this.IsDoubleQuote)
                {
                    return BackSubstitute(sb).ToString();
                }
            }

            return BackSubstitute(sb).ToString();
        }


        public string ReadSquareBracketString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(this.Current);

            while (this.GoToNext())
            {
                sb.Append(this.Current);

                if (this.IsCloseSquareBracket)
                {
                    return BackSubstitute(sb).ToString();
                }
            }

            return BackSubstitute(sb).ToString();
        }


        public string ReadDashDashComment()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append(this.Current);

            while (this.GoToNext())
            {
                sb.Append(this.Current);

                if (this.Next == '\r' || this.Next == '\n')
                    break;
            }

            return BackSubstitute(sb).ToString();
        }


        public string ReadStatementSeparator()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append(this.Current);

            while (this.GoToNext())
            {
                if (this.IsStatementSeparator && !this.IsBracket && !this.IsBeginDashDashComment && !this.IsBeginSlashStarComment && !this.IsBackspace && !this.IsEndSlashStarComment && !this.IsQuote)
                    sb.Append(this.Current);
                else
                {
                    this.GoToPrevious();
                    break;
                }
            }

            return sb.ToString();
        }



        // /* SELECT /*'SELECT 123 AS hello, 346 AS world' */  */
        // SELECT /*'SELECT ''123'' AS hello, ''Jean le Rond d''''Alembert'' AS world' */ 
        // SELECT '' as abc 
        // SELECT 'SELECT ''123'' AS hello, ''Jean le Rond d''''Alembert''' AS world
        public string ReadSlashStarCommentWithResult()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(this.Current);

            while (this.GoToNext())
            {
                if (this.IsBeginSlashStarComment)
                {
                    string innerComment = ReadSlashStarCommentWithResult();
                    sb.Append(innerComment);
                    continue;
                }

                sb.Append(this.Current);

                if (this.IsEndSlashStarComment)
                {
                    this.GoToNext();
                    sb.Append(this.Current);

                    return BackSubstitute(sb).ToString();
                }
            }

            // return false;
            return BackSubstitute(sb).ToString();
        }


    }



    public class SqlLexer
    {

        public static void Test()
        {

            SqlStringReader? x = new SqlStringReader("SELECT * FROM T_Benutzer AS foo");
            x = new SqlStringReader("SELECT 123 AS abc FROM T_Benutzer AS foo");
            x = new SqlStringReader("SELECT '123' AS abc FROM T_Benutzer AS foo");
            x = new SqlStringReader("SELECT '' AS abc FROM T_Benutzer AS foo");
            x = new SqlStringReader("SELECT 'd''Alambert' AS abc FROM T_Benutzer AS foo");

            x = new SqlStringReader("SELECT '/*d''Alambert*/' AS abc FROM T_Benutzer AS foo");
            x = new SqlStringReader("SELECT /*'d''Alambert'*/ AS abc FROM T_Benutzer AS foo");
            x = new SqlStringReader("--SELECT /*'d''Alambert'*/ AS abc FROM T_Benutzer AS foo");
            x = new SqlStringReader("--SELECT /*'d''Alambert'*/ AS abc FROM T_Benutzer AS foo\r\nSELECT 123 AS test");
            x = new SqlStringReader("SELECT /*'d''Alambert'*/AS abc FROM T_Benutzer AS foo");
            x = new SqlStringReader("SELECT 'd''Alambert'AS abc FROM T_Benutzer AS foo");
            x = new SqlStringReader("SELECT 'd''Alambert'AS abc FROM T_Benutzer AS foo LEFT JOIN T_Benutzergruppen ON 1=2");

            x = new SqlStringReader("SELECT 'd''Alambert'AS abc, LEFT(abc, 5) FROM T_Benutzer AS foo LEFT JOIN T_Benutzergruppen ON 1=2");

            x = new SqlStringReader("SELECT ''as abc 'd''Alambert'AS abc, LTRIM(RTRIM(abc)) FROM T_Benutzer AS foo LEFT JOIN T_Benutzergruppen ON 1=2");

            x = new SqlStringReader(@"
SELECT 
 123*758.13/13%5+1-7 AS expr  
,''as abc 'd''Alambert'AS ""abc def, []ghi"" 
, 123 AS [hello[]] world, now]
,CASE @in_sprache 
   WHEN 'DE' THEN xxx_DE 
   WHEN 'FR' THEN xxx_FR 
   WHEN 'IT' THEN xxx_IT 
   ELSE xxx_en 
 END AS xxx 
, LTRIM(RTRIM(abc)) 

FROM T_Benutzer AS foo 

LEFT JOIN T_Benutzergruppen 
    ON 1=2 ");



            x = new SqlStringReader(@"


IF 1=2 
BEGIN
    PRINT 'FOO'; 
END

SELECT current_timestamp, current_user, varchar(MAX), pretzelkoenig('test') 


;WITH RECURSIVE CTE AS 
( 
    SELECT 
        1 AS i 
       ,array[1] AS paths 

    UNION ALL 

    SELECT 
         CTE.i+1 AS i 
        ,CTE.paths || (CTE.i + 1) AS paths
    FROM CTE
    WHERE CTE.i < 10000
    AND CTE.i = ANY(CTE.paths) 
    -- AND CTE.i <> ALL(CTE.paths) 
)
SELECT i FROM CTE 



SELECT * 
, apply(5,3) AS ppl 
-- ,LEFT(abc, 5) AS lll, RIGHT('abc', 2) AS rrrr, FULL(123,5) AS fff 
FROM T_Benutzer 
LEFT OUTER JOIN T_Benutzergruppen AS t1 ON 1=2 
-- LEFT INNER JOIN T_Benutzergruppen AS t2 ON 1=2 
RIGHT JOIN T_Benutzergruppen AS t2 ON 1=2 
NATURAL JOIN T_Benutzergruppen AS t2 ON 1=2 



CROSS JOIN T_Benutzergruppen AS t3 

-- FULL OUTER JOIN T_Benutzergruppen AS t4 ON 1=2 
-- FULL JOIN T_Benutzergruppen AS t4 ON 1=2 

OUTER APPLY (SELECT 123 AS outerApplied) AS t5

CROSS APPLY (SELECT 123 AS crossApplied) AS t6 


LEFT JOIN LATERAL (SELECT 123 AS crossApplied) AS t7 
INNER JOIN LATERAL (SELECT 123 AS crossApplied) AS t8 
CROSS JOIN LATERAL (SELECT 123 AS crossApplied) AS t9 



");

            System.Collections.Generic.List<SqlToken> ls = new System.Collections.Generic.List<SqlToken>();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            while (x.GoToNext())
            {
                if (x.IsQuote)
                {
                    string quotedString = x.ReadQuotedString();

                    ls.Add(new SqlToken(quotedString, SqlSyntaxTokenType.String, SqlKeywordType.String));
                    sb.Clear();

                    if (!x.NextIsWhiteSpace)
                        ls.Add(new SqlToken("", SqlSyntaxTokenType.StatementSeparator, SqlKeywordType.Separator));


                    continue;
                }

                if (x.IsBeginSlashStarComment)
                {
                    string slashComment = x.ReadSlashStarCommentWithResult();
                    ls.Add(new SqlToken(slashComment, SqlSyntaxTokenType.SlashComment, SqlKeywordType.Comment));
                    sb.Clear();

                    if (!x.NextIsWhiteSpace)
                        ls.Add(new SqlToken("", SqlSyntaxTokenType.StatementSeparator, SqlKeywordType.Separator));

                    continue;
                }

                if (x.IsBeginDashDashComment)
                {
                    string dashComment = x.ReadDashDashComment();
                    ls.Add(new SqlToken(dashComment, SqlSyntaxTokenType.DashComment, SqlKeywordType.Comment));
                    sb.Clear();

                    if (!x.NextIsWhiteSpace)
                        ls.Add(new SqlToken("", SqlSyntaxTokenType.StatementSeparator, SqlKeywordType.Separator));

                    continue;
                }


                if (x.IsDoubleQuote)
                {
                    string quotedString = x.ReadDoubleQuotedString();
                    ls.Add(new SqlToken(quotedString, SqlSyntaxTokenType.QuotedIdentifier, SqlKeywordType.Identifier));
                    sb.Clear();

                    if (!x.NextIsWhiteSpace)
                        ls.Add(new SqlToken("", SqlSyntaxTokenType.StatementSeparator, SqlKeywordType.Separator));

                    continue;
                }


                if (x.IsOpenSquareBracket)
                {
                    string quotedString = x.ReadSquareBracketString();
                    ls.Add(new SqlToken(quotedString, SqlSyntaxTokenType.QuotedIdentifier, SqlKeywordType.Identifier));
                    sb.Clear();

                    if (!x.NextIsWhiteSpace)
                        ls.Add(new SqlToken("", SqlSyntaxTokenType.StatementSeparator, SqlKeywordType.Separator));

                    continue;
                }

                if (x.IsBackspace)
                {
                    string foo = sb.ToString();
                    if (!string.IsNullOrEmpty(foo))
                        throw new System.InvalidProgramException("damn");

                    sb.Clear();

                    ls.Add(new SqlToken("''", SqlSyntaxTokenType.String, SqlKeywordType.String));

                    if (!x.NextIsWhiteSpace)
                        ls.Add(new SqlToken("", SqlSyntaxTokenType.StatementSeparator, SqlKeywordType.Separator));

                    continue;
                }

                if (x.IsEnquiry)
                {
                    string foo = sb.ToString();
                    if (!string.IsNullOrEmpty(foo))
                        throw new System.InvalidProgramException("damn");

                    sb.Clear();


                    ls.Add(new SqlToken("\"\"", SqlSyntaxTokenType.String, SqlKeywordType.String));

                    if (!x.NextIsWhiteSpace)
                        ls.Add(new SqlToken("", SqlSyntaxTokenType.StatementSeparator, SqlKeywordType.Separator));

                    continue;
                }

                if (x.IsAcknowledge)
                {
                    string foo = sb.ToString();
                    if (!string.IsNullOrEmpty(foo))
                        throw new System.InvalidProgramException("damn");

                    sb.Clear();

                    ls.Add(new SqlToken("]]", SqlSyntaxTokenType.String, SqlKeywordType.String));

                    if (!x.NextIsWhiteSpace)
                        ls.Add(new SqlToken("", SqlSyntaxTokenType.StatementSeparator, SqlKeywordType.Separator));

                    continue;
                }


                if (x.IsDoubleOperator)
                {
                    string foo = sb.ToString();
                    if (!string.IsNullOrEmpty(foo))
                        ls.Add(new SqlToken(foo, SqlSyntaxTokenType.Undefined, SqlKeywordType.Identifier));

                    sb.Clear();

                    string doubleOperator = x.Current.ToString() + x.Next.ToString();
                    ls.Add(new SqlToken(doubleOperator, SqlSyntaxTokenType.DoubleArgumentOperator, SqlKeywordType.Operator));

                    x.GoToNext();

                    continue;
                }


                if (x.IsOperator)
                {
                    string foo = sb.ToString();
                    if (!string.IsNullOrEmpty(foo))
                        ls.Add(new SqlToken(foo, SqlSyntaxTokenType.Undefined, SqlKeywordType.Identifier));

                    sb.Clear();

                    ls.Add(new SqlToken(x.Current!.Value.ToString(), SqlSyntaxTokenType.SingleArgumentOperator, SqlKeywordType.Operator));

                    continue;
                }


                if (x.IsStatementSeparator)
                {
                    string statement = sb.ToString();
                    if (!string.IsNullOrEmpty(statement))
                        ls.Add(new SqlToken(statement, SqlSyntaxTokenType.Undefined, SqlKeywordType.Identifier));


                    sb.Clear();

                    statement = x.ReadStatementSeparator();
                    ls.Add(new SqlToken(statement, SqlSyntaxTokenType.StatementSeparator, SqlKeywordType.Separator));

                    continue;
                }

                sb.Append(x.Current!);
            }

            if (sb.Length != 0)
                ls.Add(new SqlToken(sb.ToString(), SqlSyntaxTokenType.Undefined, SqlKeywordType.Identifier));


            for (int i = 0; i < ls.Count; ++i)
            {
                if (i != 0)
                    ls[i].Previous = ls[i - 1];


                if (i != ls.Count - 1)
                    ls[i].Next = ls[i + 1];
            }


            for (int i = 0; i < ls.Count; ++i)
            {
                if (ls[i].NeedsPostProcessing)
                {
                    SqlToken? potentialToken = ls[i].NextNonSeparator();
                    SqlToken? realToken = potentialToken;


                    // Here are the LEFT/RIGHT OUTER|INNER JOIN/APPLY LATERAL 
                    if (potentialToken != null && potentialToken.IsPotentialJoinStatement)
                        realToken = potentialToken.NextNonSeparator();

                    if (realToken != null && realToken.IsJoinStatement)
                    {
                        potentialToken!.KeywordType = SqlKeywordType.From;
                        realToken.KeywordType = SqlKeywordType.From;
                        ls[i].KeywordType = SqlKeywordType.From;

                        potentialToken.NeedsPostProcessing = false;
                        realToken.NeedsPostProcessing = false;
                        ls[i].NeedsPostProcessing = false;
                    }


                    if ("ALL".Equals(ls[i].Text, System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        SqlToken? previousToken = ls[i].PreviousNonSeparator();
                        if (previousToken != null && "UNION".Equals(previousToken.Text, System.StringComparison.InvariantCultureIgnoreCase))
                        {
                            ls[i].SyntaxTokenType = SqlSyntaxTokenType.Undefined;
                            ls[i].KeywordType = SqlKeywordType.Reserved;
                        }
                    }
                    
                    


                }
            }


            string[] list_of_reserved_words = new string[] {
                  "BIT", "BOOLEAN", "CHAR", "NCHAR", "VARCHAR", "NVARCHAR"
                , "NATIONAL", "CHARACTER", "VARYING"
                , "CURSOR",  "OPEN", "CLOSE", "WHILE", "FETCH", "NEXT", "DEALLOCATE"
                , "TRAN", "TRANSACTION", "COMMIT", "ROLLBACK"
                , "BACKUP", "RESTORE" , "CHECKPOINT"
                , "DATE", "TIME", "DATETIME", "TIMESTAMP","UNIQUEIDENTIFIER"
                , "INT", "INTEGER", "ARRAY", "FLOAT", "REAL","DECIMAL"
                , "TABLE","VIEW", "COLUMN", "TRIGGER", "FUNCTION", "PROCEDURE", "INDEX", "DOMAIN", "CONSTRAINT", "UNIQUE", "RULE" 
                , "DECLARE", "SET", "PRINT", "READONLY","OUTPUT"
                , "WITH", "RECURSIVE", "AS", "CONNECT", "NOCYCLE", "PRIOR", "OPTION", "MAXRECURSION", "WITHIN"
                , "CREATE", "REPLACE", "ALTER", "DROP", "SELECT", "INSERT", "INTO", "VALUES", "MERGE", "UPDATE", "DELETE", "TRUNCATE"
                , "EXEC", "EXECUTE", "EXISTS", "RETURNS", "BEGIN", "END", "IF", "BREAK", "CONTINUE", "GOTO", "EXIT" 
                , "FROM", "UNION", "EXCEPT", "INTERSECT", "DISTINCT", "GROUP", "BY", "DISTRIBUTED" 
                , "ORDER", "ASC", "DESC", "COLLATE", "LIMIT", "OFFSET", "TOP", "PERCENT" 
                , "CASE", "WHEN", "ELSE", "ESCAPE", "OVER"
                , "TRUE", "FALSE", "ON", "OFF", "OF" 
                , "GRANT", "REVOKE", "DENY" ,"STATISTICS", "WAITFOR"
                , "FOREIGN", "KEY", "REFERENCES", "NOCHECK", "CASCADE", "DEFAULT", "PERSIST" 
                , "OPENROWSET","OPENQUERY","OPENDATASOURCE"
                , "FILLFACTOR", "FREETEXT", "FREETEXTTABLE", "READTEXT", "WRITETEXT", "TEXTSIZE"
                , "SEMANTICKEYPHRASETABLE", "SEMANTICSIMILARITYDETAILSTABLE", "SEMANTICSIMILARITYTABLE"
                , "CONTAINSTABLE", "EXPLAIN", "PLAN", "TABLESAMPLE"
                , "GO", "START" , "UNPIVOT", "NONCLUSTERED", "DBCC", "RECONFIGURE", "DUMP" 
            };

            string[] list_of_functions = new string[] {
                  "CURRENT_DATE", "CURRENT_TIME", "CURRENT_TIMESTAMP", "CURRENT_USER" , "SESSION_USER"
                , "ERROR_MESSAGE", "ERROR_SEVERITY", "ERROR_STATE", "ERRLVL", "LINENO"
                , "NANOSECOND", "MICROSECOND", "MILLISECOND", "SECOND", "MINUTE", "HOUR", "DAY"
                , "DAYOFYEAR", "WEEKDAY", "WEEK", "ISO_WEEK", "MONTH", "QUARTER", "YEAR"
                , "MAX","MIN", "LEAST", "GREATEST", "CONVERT", "TRY_CONVERT", "CAST", "TRY_CAST"
                , "ROW_NUMBER", "DENSE_RANK", "RANK", "NTILE", "SUM", "COUNT", "NULLIF", "ISNULL", "COALESCE"
                , "RAISERROR"
            };

            string[] list_of_joins = new string[] {
                "LEFT", "RIGHT","INNER","OUTER", "FULL", "NATURAL", "JOIN", "APPLY"
            };

            System.Array.Sort(list_of_reserved_words);
            System.Array.Sort(list_of_functions);
            System.Array.Sort(list_of_joins);



            System.Collections.Generic.HashSet<string> reserved_words =
                new System.Collections.Generic.HashSet<string>(list_of_reserved_words, System.StringComparer.InvariantCultureIgnoreCase);

            System.Collections.Generic.HashSet<string> functions =
                new System.Collections.Generic.HashSet<string>(list_of_functions, System.StringComparer.InvariantCultureIgnoreCase);

            System.Collections.Generic.HashSet<string> joins =
                new System.Collections.Generic.HashSet<string>(list_of_joins, System.StringComparer.InvariantCultureIgnoreCase);



            for (int i = 0; i < ls.Count; ++i)
            {
                // SET Functions
                SqlToken? potentialBracket = ls[i].NextNonSeparatorIncludingBracket();

                if (potentialBracket != null && potentialBracket.KeywordType == SqlKeywordType.OpenBracket)
                {
                    // LEFT/RIGHT/INNER/FULL JOIN is not a function 
                    if (ls[i].KeywordType != SqlKeywordType.From
                        && ls[i].KeywordType != SqlKeywordType.Operator)
                    {
                        if(reserved_words.Contains(ls[i].Text))
                            ls[i].KeywordType = SqlKeywordType.Reserved;
                        else
                            ls[i].KeywordType = SqlKeywordType.Function;
                    }

                }

            }



            for (int i = 0; i < ls.Count; ++i)
            {
                if (ls[i].KeywordType == SqlKeywordType.Function)
                    continue;

                string txt = ls[i].Text;

                if (reserved_words.Contains(txt))
                {
                    ls[i].KeywordType = SqlKeywordType.Reserved;
                }

                if (functions.Contains(txt))
                {
                    ls[i].KeywordType = SqlKeywordType.Function;
                }

                if (joins.Contains(txt))
                {
                    ls[i].KeywordType = SqlKeywordType.From;
                }

            }




            string[] colors = new string[]
            {
                "blue", // Reserved = 0,
                "hotpink", // Function = 1,
                "gray", // Operator = 2,
                "#A0A0A0", // From = 3,
                "black", // Identifier = 4,
                "red", // String = 5,
                "black", // Number = 6,
                "black", // Separator = 7,
                "green", // Comment = 8,
                "black", // OpenBracket = 9,
                "black", // CloseBracket = 10,
                "black", // Unknown = 11
            };



            System.Text.StringBuilder sbb = new System.Text.StringBuilder();
            sbb.AppendLine(@"
<html>
<head></head>
<body>
");

            for (int i = 0; i < ls.Count; ++i)
            {
                SqlToken? mytok = ls[i];

                // if (mytok.KeywordType == SqlKeywordType.Comment) continue;

                string html = mytok.HtmlText;
                string color = colors[(int)mytok.KeywordType];

                sbb.Append(@"<span style=""color: ");
                sbb.Append(color);
                sbb.Append(@";"">");
                sbb.Append(html);
                sbb.Append("</span>");

            }
            sbb.AppendLine(@"</body>
</html>");

            System.IO.File.WriteAllText(@"D:\mysyntax.html", sbb.ToString(), System.Text.Encoding.UTF8);

            System.Console.WriteLine(ls);
        }

    }
}
