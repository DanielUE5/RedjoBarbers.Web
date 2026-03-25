using Microsoft.EntityFrameworkCore;
using RedjoBarbers.Web.Data;
using RedjoBarbers.Web.Data.Models;
using RedjoBarbers.Web.Services.Contracts;

namespace RedjoBarbers.Web.Services
{
    public class BarberServiceService : IBarberServiceService
    {
        private readonly RedjoBarbersDbContext dbContext;

        public BarberServiceService(RedjoBarbersDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<BarberService>> GetAllAsync()
        {
            return await dbContext.BarberServices
                .AsNoTracking()
                .Include(bs => bs.Reviews)
                .AsSplitQuery()
                .OrderBy(bs => bs.Id)
                .ToListAsync();
        }
    }
}