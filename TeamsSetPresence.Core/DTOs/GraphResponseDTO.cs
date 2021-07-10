using System.Collections.Generic;

namespace TeamsSetPresence.Core.DTOs
{
    public class GraphResponseDTO<T>
    {
        public List<T> Value { get; set; }
    }
}
