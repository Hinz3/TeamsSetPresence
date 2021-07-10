using System.Collections.Generic;
using System.Threading.Tasks;
using TeamsSetPresence.Core.DTOs;

namespace TeamsSetPresence.Core.Interfaces
{
    public interface ITeamsService
    {
        Task<string> GetAccessToken();
        Task<bool> ChangeStatus(string accessToken, string userId, string availability, string activity);
        Task<List<UserDTO>> GetUsers(string accessToken);
    }
}
