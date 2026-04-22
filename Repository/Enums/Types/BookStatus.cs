namespace Repository.Enums.Types
{
    /// <summary>
    /// Enum type for possible book statuses
    /// </summary>
    public enum BookStatus
    {
        /// <summary>
        /// The book is in stock and can be loaned
        /// </summary>
        InStock,

        /// <summary>
        /// The book is out of stock and is unavailable
        /// </summary>
        OutOfStock,

        /// <summary>
        /// The book has been discontinued for one reason on another, unavailable for loans
        /// </summary>
        Discontinued,

        /// <summary>
        /// The book has a problem and should not be loaned at the moment
        /// </summary>
        TemporarilyUnavailable
    }
}
