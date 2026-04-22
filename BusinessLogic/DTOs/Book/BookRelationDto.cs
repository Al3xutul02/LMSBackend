using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Book
{
    /// <summary>
    /// Represents a relation DTO for book entities.
    /// </summary>
    public record BookRelationDto(
        int ISBN = 0,
        int Count = 0
        );
}
