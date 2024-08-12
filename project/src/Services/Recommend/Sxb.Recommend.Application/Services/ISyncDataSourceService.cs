using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Application.Services
{
    public interface ISyncDataSourceService
    {
        void SyncSchoolsCopies();
        void SyncSchoolsCopies(string synclogFilePath);
        void SyncArticleCopies();
        void SyncArticleCopies(string synclogFilePath);
    }
}
