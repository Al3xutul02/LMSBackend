namespace Repository.Enums.Types
{
    /// <summary>
    /// Enum type for possible loan statuses
    /// </summary>
    public enum LoanStatus
    {
        /// <summary>
        /// The loan is still in effect
        /// </summary>
        Active,

        /// <summary>
        /// The user has not returned the books yet and the due date has passed
        /// </summary>
        Overdue,

        /// <summary>
        /// The books have been retured, either on time or after the due date
        /// </summary>
        Returned,

        /// <summary>
        /// The loan is waiting to be approved by a librarian
        /// </summary>
        Pending
    }
}
