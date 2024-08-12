using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Infrastructure.IRepository
{
    public interface ISchoolFileRepository
    {

        bool Exists();
        void Init(IEnumerable<School> schools);

        IEnumerable<School> Query(Func<School,bool> where);
        void Append(School school);

        void Update(School school);

        void SaveChange();
    }
}
