namespace NetProxy
{
    /// <summary>
    /// Model of queries
    /// </summary>
    public class Query
    {
        /// <summary>
        /// Query identifier 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Short sql text to display in the list
        /// </summary>
        public string ShortSqlText { get; set; } = "";

        /// <summary>
        /// Full sql text
        /// </summary>
        public string SqlText { get; set; } = "";

        /// <summary>
        /// Identifier creation constructor 
        /// </summary>
        /// <param name="num"></param>
        public Query(ref int num)
        {
            Id = ++num;
        }
    }
}
