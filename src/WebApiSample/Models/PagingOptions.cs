namespace WebApiSample.Models
{
    /// <summary>
    /// Options to pagination.
    /// </summary>
    public sealed class PagingOptions
    {
        /// <summary>
        /// Page number to take.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Number of records on each page.
        /// </summary>
        public int PageSize { get; set; }
    }
}