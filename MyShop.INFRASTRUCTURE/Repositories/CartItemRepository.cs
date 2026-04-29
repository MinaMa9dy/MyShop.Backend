using MyShop.CORE.Entities;
using MyShop.CORE.RepositoriyInterfaces;
using MyShop.INFRASTRUCTURE.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.INFRASTRUCTURE.Repositories
{
    public class CartItemRepository : BaseRepository<CartItem>,ICartItemRepository
    {
        private readonly AppDbContext _context;
        public CartItemRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _context = appDbContext;
        }

    }
}
