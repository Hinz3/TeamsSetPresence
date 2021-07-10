namespace TeamsSetPresence.Core.DTOs
{
    public class GraphTeamsStatusDTO
    {
        /// <summary>
        /// Client id
        /// </summary>
        public string SessionId { get; set; }
        /// <summary>
        /// Which availability should be set on user
        /// </summary>
        /// <example>
        /// examplsdsa
        /// </example>
        public string Availability { get; set; }
        public string Activity { get; set; }
    }
}
