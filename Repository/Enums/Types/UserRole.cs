namespace Repository.Enums.Types
{
    /// <summary>
    /// Enum type for possible user roles
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// The typical user of the website that wants to search and loan books to read
        /// </summary>
        Reader,

        /// <summary>
        /// Employee of the library that manages and oversees books and loans
        /// </summary>
        Librarian,

        /// <summary>
        /// Company administrators that can modify and oversee data around books, branches,
        /// users and librarians
        /// </summary>
        Administrator
    }
}
