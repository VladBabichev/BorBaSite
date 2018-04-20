using Microsoft.EntityFrameworkCore;

namespace BorBaNetCore
{
    internal class GenericRepository<TType, TId> where TType : class
    {
        private DbContext _context;

        public GenericRepository(DbContext _context)
        {
            this._context = _context;
        }
    }
}