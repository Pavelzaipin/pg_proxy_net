﻿
// Traceutils assembly
// writen by Locky, 2009. 


namespace ExpressProfiler
{


    class Sqltokens
    {

        private const string Keywords = "ADD,ALTER,AS,ASC,AUTHORIZATION,BACKUP," +
                                        "BEGIN,BREAK,BROWSE,BULK,BY,CASCADE,CASE," +
                                        "CHECK,CHECKPOINT,CLOSE,CLUSTERED,COLLATE," +
                                        "COLUMN,COMMIT,COMPUTE,CONSTRAINT,CONTAINS,CONTAINSTABLE," +
                                        "CONTINUE,CREATE,CURRENT,CURSOR,DATABASE," +
                                        "DBCC,DEALLOCATE,DECLARE,DEFAULT,DELETE,DENY,DESC,DISK," +
                                        "DISTINCT,DISTRIBUTED,DOUBLE,DROP,DUMMY,DUMP,ELSE,END," +
                                        "ERRLVL,ESCAPE,EXCEPT,EXEC,EXECUTE,EXIT,FETCH,FILE," +
                                        "FILLFACTOR,FOR,FOREIGN,FORMSOF,FREETEXT,FREETEXTTABLE,FROM,FULL," +
                                        "FUNCTION,GOTO,GRANT,GROUP,HAVING,HOLDLOCK,IDENTITY," +
                                        "IDENTITYCOL,IDENTITY_INSERT,IF,INFLECTIONAL,INDEX,INNER,INSERT," +
                                        "INTERSECT,INTO,IS,ISABOUT,KEY,KILL,LIMIT,LINENO,LOAD," +
                                        "NATIONAL,NOCHECK,NONCLUSTERED,OF,OFF,OFFSET" +
                                        "OFFSETS,ON,OPEN,OPENDATASOURCE,OPENQUERY,OPENROWSET,OPENXML," +
                                        "OPTION,ORDER,OVER,PERCENT,PLAN,PRECISION," +
                                        "PRIMARY,PRINT,PROC,PROCEDURE,PUBLIC,RAISERROR,READ," +
                                        "READTEXT,RECONFIGURE,REFERENCES,REPLICATION,RESTORE," +
                                        "RESTRICT,RETURN,REVOKE,ROLLBACK,ROWCOUNT,ROWGUIDCOL," +
                                        "RULE,SAVE,SCHEMA,SELECT,SET,SETUSER,SHUTDOWN," +
                                        "STATISTICS,TABLE,TEXTSIZE,THEN,TO,TOP,TRAN,TRANSACTION," +
                                        "TRIGGER,TRUNCATE,TSEQUAL,UNION,UNIQUE,UPDATE,UPDATETEXT," +
                                        "USE,VALUES,VARYING,VIEW,WAITFOR,WEIGHT,WHEN,WHERE,WHILE," +
                                        "WITH,WRITETEXT,CURRENT_DATE,CURRENT_TIME" +
                                        ",OUT,NEXT,PRIOR,RETURNS,ABSOLUTE,ACTION,PARTIAL,FALSE" +
                                        ",PREPARE,FIRST,PRIVILEGES,AT,GLOBAL,RELATIVE,ROWS,HOUR,MIN,MAX" +
                                        ",SCROLL,SECOND,SECTION,SIZE,INSENSITIVE,CONNECT,CONNECTION" +
                                        ",ISOLATION,LEVEL,LOCAL,DATE,MINUTE,TRANSLATION" +
                                        ",TRUE,NO,ONLY,WORK,OUTPUT" +
                                        ",ABSOLUTE,ACTION,FREE,PRIOR,PRIVILEGES,AFTER,GLOBAL" +
                                        ",HOUR,RELATIVE,IGNORE,AT,RETURNS,ROLLUP,ROWS,SCROLL" +
                                        ",ISOLATION,SECOND,SECTION,SEQUENCE,LAST,SIZE,LEVEL" +
                                        ",CONNECT,CONNECTION,LOCAL,CUBE,MINUTE,MODIFY,STATIC" +
                                        ",DATE,TEMPORARY,TIME,NEXT,NO,TRANSLATION,TRUE,ONLY" +
                                        ",OUT,DYNAMIC,OUTPUT,PARTIAL,WORK,FALSE,FIRST,PREPARE,GROUPING,FORMAT,INIT,STATS" +
                                        "FORMAT,INIT,STATS,NOCOUNT,FORWARD_ONLY,KEEPFIXED,FORCE,KEEP,MERGE,HASH,LOOP,maxdop,nolock" +
                                        ",updlock,tablock,tablockx,paglock,readcommitted,readpast,readuncommitted,repeatableread,rowlock,serializable,xlock"
                                        + ",delay";

        
        private const string Functions = "@@CONNECTIONS,@@CPU_BUSY,@@CURSOR_ROWS,@@DATEFIRST,@@DBTS,@@ERROR," +
                                         "@@FETCH_STATUS,@@IDENTITY,@@IDLE,@@IO_BUSY,@@LANGID,@@LANGUAGE," +
                                         "@@LOCK_TIMEOUT,@@MAX_CONNECTIONS,@@MAX_PRECISION,@@NESTLEVEL,@@OPTIONS," +
                                         "@@PACKET_ERRORS,@@PACK_RECEIVED,@@PACK_SENT,@@PROCID,@@REMSERVER," +
                                         "@@ROWCOUNT,@@SERVERNAME,@@SERVICENAME,@@SPID,@@TEXTSIZE,@@TIMETICKS," +
                                         "@@TOTAL_ERRORS,@@TOTAL_READ,@@TOTAL_WRITE,@@TRANCOUNT,@@VERSION," +
                                         "ABS,ACOS,APP_NAME,ASCII,ASIN,ATAN,ATN2,AVG,BINARY_CHECKSUM,CAST," +
                                         "CEILING,CHARINDEX,CHECKSUM,CHECKSUM_AGG,COLLATIONPROPERTY," +
                                         "COLUMNPROPERTY,COL_LENGTH,COL_NAME,COS,COT,COUNT," +
                                         "COUNT_BIG," +
                                         "CURSOR_STATUS,DATABASEPROPERTY,DATABASEPROPERTYEX," +
                                         "DATALENGTH,DATEADD,DATEDIFF,DATENAME,DATEPART,DAY,DB_ID,DB_NAME,DEGREES," +
                                         "DIFFERENCE,EXP,FILEGROUPPROPERTY,FILEGROUP_ID,FILEGROUP_NAME,FILEPROPERTY," +
                                         "FILE_ID,FILE_NAME,FLOOR" +
                                         "" +
                                         "FORMATMESSAGE,FULLTEXTCATALOGPROPERTY,FULLTEXTSERVICEPROPERTY," +
                                         "GETANSINULL,GETDATE,GETUTCDATE,HAS_DBACCESS,HOST_ID,HOST_NAME," +
                                         "IDENT_CURRENT,IDENT_INCR,IDENT_SEED,INDEXKEY_PROPERTY,INDEXPROPERTY," +
                                         "INDEX_COL,ISDATE,ISNULL,ISNUMERIC,IS_MEMBER,IS_SRVROLEMEMBER,LEN,LOG," +
                                         "LOG10,LOWER,LTRIM,MONTH,NEWID,OBJECTPROPERTY,OBJECT_ID," +
                                         "OBJECT_NAME,PARSENAME,PATINDEX," +
                                         "PERMISSIONS,PI,POWER,QUOTENAME,RADIANS,RAND,REPLACE,REPLICATE,REVERSE," +
                                         "ROUND,ROWCOUNT_BIG,RTRIM,SCOPE_IDENTITY,SERVERPROPERTY,SESSIONPROPERTY," +
                                         "SIGN,SIN,SOUNDEX,SPACE,SQL_VARIANT_PROPERTY,SQRT,SQUARE," +
                                         "STATS_DATE,STDEV,STDEVP,STR,STUFF,SUBSTRING,SUM,SUSER_SID,SUSER_SNAME," +
                                         "TAN,TEXTPTR,TEXTVALID,TYPEPROPERTY,UNICODE,UPPER," +
                                         "USER_ID,USER_NAME,VAR,VARP,YEAR," +
                                         "SUBSTR,SUBSTRING,SPLIT_PART,UNNEST,ARRAY,ARRAY_AGG,ARRAY_TO_STRING," +
                                         "ROW_NUMBER,RANK,DENSE_RANK,NTILE," +
                                         "CURRENT_SETTING,FORMAT_TYPE,HAS_DATABASE_PRIVILEGE,OBJ_DESCRIPTION,SHOBJ_DESCRIPTION," +
                                         "PG_AVAILABLE_EXTENSIONS,PG_GET_CONSTRAINTDEF,PG_GET_EXPR,PG_GET_FUNCTION_RESULT,PG_GET_INDEXDEF,PG_GET_RULEDEF,PG_GET_TRIGGERDEF,PG_GET_USERBYID," +
                                         "PG_IS_IN_RECOVERY,PG_CONF_LOAD_TIME,PG_ENCODING_TO_CHAR," +
                                         "PG_IS_XLOG_REPLAY_PAUSED,PG_LAST_XACT_REPLAY_TIMESTAMP,PG_LAST_XLOG_REPLAY_LOCATION,PG_LAST_XLOG_RECEIVE_LOCATION,PG_POSTMASTER_START_TIME,PG_TABLESPACE_LOCATION,"+
                                         "REGEXP_REPLACE,TS_TOKEN_TYPE";


        private const string Types = "bigint,binary,bit,char,character,datetime," +
                                     "dec,decimal,float,image,int," +
                                     "integer,money,nchar,ntext,nvarchar,real," +
                                     "rowversion,smalldatetime,smallint,smallmoney," +
                                     "sql_variant,sysname,text,timestamp,tinyint,uniqueidentifier," +
                                     "varbinary,varchar,NUMERIC";
       

        private const string Greykeywords = "AND,EXISTS,ALL,ANY,BETWEEN,IN,SOME,LEFT,RIGHT,JOIN,CROSS,OR,NULL,OUTER,NOT";
        private const string Fukeywords = "LIKE,COALESCE,SESSION_USER,CONVERT,SYSTEM_USER,CURRENT_TIMESTAMP,CURRENT_USER,NULLIF,USER";

        private readonly System.Collections.Generic.Dictionary<string, YukonLexer.TokenKind>
            m_Words = new System.Collections.Generic
                .Dictionary<string, YukonLexer.TokenKind>();

        public YukonLexer.TokenKind this[string token] { get { token = token.ToLower(); return m_Words.ContainsKey(token) ? m_Words[token] : YukonLexer.TokenKind.tkUnknown; } }


        private void AddTokens(string tokens, YukonLexer.TokenKind tokenkind)
        {
            System.Text.StringBuilder curtoken = new System.Text.StringBuilder();
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] == ',')
                {
                    string s = curtoken.ToString().ToLower();
                    if (!m_Words.ContainsKey(s))
                        m_Words.Add(s, tokenkind);
                    curtoken = new System.Text.StringBuilder();
                }
                else
                {
                    curtoken.Append(tokens[i]);
                }
            }
            if (curtoken.Length != 0) m_Words.Add(curtoken.ToString(), tokenkind);
        }


        public Sqltokens()
        {
            AddTokens(Keywords, YukonLexer.TokenKind.tkKey);
            AddTokens(Functions, YukonLexer.TokenKind.tkFunction);
            AddTokens(Types, YukonLexer.TokenKind.tkDatatype);
            AddTokens(Greykeywords, YukonLexer.TokenKind.tkGreyKeyword);
            AddTokens(Fukeywords, YukonLexer.TokenKind.tkFuKeyword);
        }


    }


}