using MyShop.CORE.RepositoriyInterfaces;
using MyShop.INFRASTRUCTURE.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.INFRASTRUCTURE.Repositories
{
    public class AttributeRepository : BaseRepository<CORE.Entities.Attribute>, IAttributeRepository
    {
        public AttributeRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
