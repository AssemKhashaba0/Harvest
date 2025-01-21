using DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;

namespace DataAccess.Repository
{
    public class CategoriesRepository : Repository<Category>, CategoriesIRepository
    {
        private readonly ApplicationDbContext dbContext;

        public CategoriesRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }


    }
}
