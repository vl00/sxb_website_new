using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.Recommend.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            IEnumerable<SchoolMap> list1 = new List<SchoolMap> {
                new SchoolMap(Guid.NewGuid(), Guid.NewGuid(),10,"list1-10"),
                new SchoolMap(Guid.NewGuid(), Guid.NewGuid(),9,"list1-9"),
                new SchoolMap(Guid.NewGuid(), Guid.NewGuid(),6,"list1-6"),
                new SchoolMap(Guid.NewGuid(), Guid.NewGuid(),3,"list1-3"),
                new SchoolMap(Guid.NewGuid(), Guid.NewGuid(),2,"list1-2"),
            };
            IEnumerable<SchoolMap> list2 = new List<SchoolMap> {
                new SchoolMap(Guid.NewGuid(), Guid.NewGuid(),10,"list2-10"),
                new SchoolMap(Guid.NewGuid(), Guid.NewGuid(),9,"list2-9"),
                new SchoolMap(Guid.NewGuid(), Guid.NewGuid(),6,"list2-6"),
                new SchoolMap(Guid.NewGuid(), Guid.NewGuid(),3,"list2-3"),
                new SchoolMap(Guid.NewGuid(), Guid.NewGuid(),2,"list2-2"),
            };
            var sortswap = SortSwap(list1, list2).ToArray();
            var theRight= list1.Concat(list2).OrderByDescending(s => s.Score).Skip(0).Take(list1.Count()).ToArray();
            for (int i = 0; i < theRight.Length; i++)
            {
                Assert.IsTrue(sortswap[i].Score == theRight[i].Score);
            }


        }


        IEnumerable<SchoolMap> SortSwap(IEnumerable<SchoolMap> list1, IEnumerable<SchoolMap> list2)
        {
            //O(log(n*m))
            var temp = list2.Where(s => s.Score >= list1.Min(s1 => s1.Score) && s.Score <= list1.Max(s1 => s1.Score));
            return list1.Concat(temp).OrderByDescending(s => s.Score).Skip(0).Take(list1.Count());

        }

    }
}
