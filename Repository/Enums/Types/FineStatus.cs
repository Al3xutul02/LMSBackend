namespace Repository.Enums.Types
{
    /// <summary>
    /// Enum type for possible fine statuses
    /// </summary>
    public enum FineStatus
    {
        /// <summary>
        /// The user has paid their fine
        /// </summary>
        Paid,

        /// <summary>
        /// The user has <b>NOT</b> paid their fine
        /// </summary>
        Unpaid,

        /// <summary>
        /// The fine has been waived by a librarian or administrator
        /// and does not need to be paid.
        /// </summary>
        Waived
    }
}
