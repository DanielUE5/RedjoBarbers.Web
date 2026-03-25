using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.ViewModels;

namespace RedjoBarbers.Web.Services.Contracts
{
    public interface IHomeService
    {
        Task<HomeIndexViewModel> GetHomePageDataAsync();

        Task<Barber?> GetOwnerAsync();

        Task<IEnumerable<Barber>> GetContactsAsync();
    }
}