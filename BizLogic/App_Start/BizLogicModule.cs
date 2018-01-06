using Autofac;

namespace BizLogic
{

    public class BizLogicModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            //Autowire the classes with interfaces
            builder.RegisterAssemblyTypes(GetType().Assembly).AsImplementedInterfaces();
        }
    }

}
