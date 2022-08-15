using API.Contexts;

namespace API.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly DapperContext _context;
        public BaseRepository(DapperContext context)
        {
            _context = context;
        }
    }
}
