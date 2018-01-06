using Autofac;

namespace BizDbAccess
{

    public class BizDbAccessModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            //Autowire the classes with interfaces
            builder.RegisterAssemblyTypes(GetType().Assembly).AsImplementedInterfaces();
        }
    }

}
