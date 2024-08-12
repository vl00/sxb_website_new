using Autofac;
using Microsoft.Extensions.Configuration;
using Sxb.WeWorkFinance.API.Application.ES;
using Sxb.WeWorkFinance.API.Application.Queries;
using Sxb.WeWorkFinance.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sxb.WeWorkFinance.API.Extensions
{
    public class AutofacExtension : Autofac.Module
    {
        public string QueriesConnectionString { get; }
        public string UserConnectionString { get; }

        public AutofacExtension(IConfiguration configuration)
        {
            QueriesConnectionString = configuration.GetSection("ConnectionString").GetValue<string>("Master");
            UserConnectionString = configuration.GetSection("ConnectionString").GetValue<string>("User");
        }
        protected override void Load(ContainerBuilder builder)
        {
            //获取当前应用程序加载程序集
            //var assemblys = AppDomain.CurrentDomain.GetAssemblies().ToArray();
            //builder.RegisterAssemblyTypes(assembly)
            //    .InNamespaceOf<MyTalentQueries>()
            //    .InstancePerLifetimeScope();
            //var assemblys = AppDomain.CurrentDomain.GetAssemblies().ToArray();
            //var perRequestType = typeof(MyTalentQueries);
            //builder.RegisterAssemblyTypes(assemblys)
            //    .Where(t => perRequestType.IsAssignableFrom(t) && t != perRequestType)
            //    .PropertiesAutowired()
            //    .AsImplementedInterfaces()
            //    .InstancePerLifetimeScope();

            builder.RegisterType<GroupChatRepository>().As<IGroupChatRepository>();
            builder.RegisterType<GroupMemberRepository>().As<IGroupMemberRepository>();
            builder.RegisterType<GroupUserRepository>().As<IGroupUserRepository>();

            builder.RegisterType<ContactRepository>().As<IContactRepository>();
            builder.RegisterType<CustomerQrCodeRepository>().As<ICustomerQrCodeRepository>();

            builder.RegisterType<LogCloseOrderRepository>().As<ILogCloseOrderRepository>();
            builder.RegisterType<LogCorrectPointRepository>().As<ILogCorrectPointRepository>();

            builder.RegisterType<ChatDataES>().As<IChatDataES>();

            builder.Register(c => new WeixinQueries(QueriesConnectionString))
                .As<IWeixinQueries>()
                .InstancePerLifetimeScope();

            builder.Register(c => new UserQueries(UserConnectionString))
                .As<IUserQueries>()
                .InstancePerLifetimeScope();
        }
    }
}
