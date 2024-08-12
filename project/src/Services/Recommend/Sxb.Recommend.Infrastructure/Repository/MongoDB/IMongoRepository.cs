using Sxb.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Infrastructure.Repository.MongoDB
{
    public interface IMongoRepository<TEntity> where TEntity:Entity 
    {
        void InitCollection();

    }
}
