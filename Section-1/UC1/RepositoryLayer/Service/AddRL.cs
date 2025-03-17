using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Context;

namespace RepositoryLayer.Service
{
    public class AddRL
    {
        private readonly AppDbContext _context;
        public AddRL(AppDbContext context)
        {
            _context = context;
        }
    }
}
