using RedjoBarbers.Web.Data.Models;

namespace RedjoBarbers.Web.Services.Contracts
{
    /// <summary>
    /// Service for managing barber services.
    /// </summary>
    public interface IBarberServiceService
    {
        Task<IEnumerable<BarberService>> GetAllAsync();
    }
}