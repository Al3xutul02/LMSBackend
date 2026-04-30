using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.DTOs.Loan
{
    public record RequestedBookDto(
        string Title = "",
        string Author = "",
        int AvailableCopies = 0,
        int RequestedQuantity = 1,
        string Status = "Pending"
    );
}
