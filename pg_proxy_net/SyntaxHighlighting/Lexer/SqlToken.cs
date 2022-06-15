
namespace pg_proxy_net.SyntaxHighlighting.Lexer
{

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


}
