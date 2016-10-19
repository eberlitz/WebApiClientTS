using System.Collections.Generic;

namespace WebApiSample.Models
{
    /// <summary>
    /// Represents a page with respective records.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class PagedResult<T>
    {
        /// <summary>
        /// Records in a one page.
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Total records at query.
        /// </summary>
        public int Count { get; set; }
    }
}