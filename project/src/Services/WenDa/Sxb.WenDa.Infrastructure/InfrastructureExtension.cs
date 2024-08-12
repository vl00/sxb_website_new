using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Infrastructure
{
    public class InfrastructureExtension
    {
        public static IEnumerable<Type> RegisterTypes()
        {
            return
                //typeof(InfrastructureExtension).Assembly
                Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(s => s.IsClass && !s.IsAbstract && !s.IsInterface)
                .Where(s => s.Name.EndsWith("Repository"))
                ;
        }
    }
}
