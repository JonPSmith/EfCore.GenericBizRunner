# Upgrade guide - GenericBizRunner version 3

Version 3 (V3) of [EfCore.GenericBizRunner](https://github.com/JonPSmith/EfCore.GenericBizRunner) changes (improves) the way it configures [AutoMapper](https://automapper.org/), plus adds a number of other useful improvements. The problem is this could break any code you wrote to work with version 2 (V2) of the library. This page tells you what has changed and tells you what to do if you upgrade and get compile errors.

**NOTE: If you change over from V2 to V3 and get the code compile, then your code should work as it did before. There isn't any changes to what it does, just how it is implemented inside.**

## Motivation for these changes

GenericBizRunner uses AutoMapper to map inputs and outputs to the business logic *(you can find out why from the [Anti corruption feature](https://github.com/JonPSmith/EfCore.GenericBizRunner/wiki/Anti-corruption-feature) page in the Wiki)*. I made a mistake around how to configure AutoMapper mappings, which added extra properties to the the DTO. That's causes a problem with scaffolding ASP.NET Core Razor pages, but you can edit the code by hand to remove the extra properties. The real pain-point comes with ASP.NET Core Web API.

Web APIs are all about sending back the exact data that the front-end/service requires, but GenericBizRunner V2 had these extra properties which made using it with Web APIs really difficult. I worked around this problem in one project, but now I have the time I have fixed this in V3. GenericBizRunner V3 now handles Web APIs really well.

GenericBizRunner V3 also has some useful features I recently added to the[EfCore.GenericServices](https://github.com/JonPSmith/EfCore.GenericServices) library which are nice to have. See the [Release Notes](https://github.com/JonPSmith/EfCore.GenericServices/blob/master/ReleaseNotes.md) for information on all the changes, plus notes at the end.

## How the AutoMapper changes affect your code

In GenericBizRunner before V3 I made my GenericBizRunner DTO templates inherit AutoMapper's `Profile` class and used `AddAutoMapper()` to set up the mapping (It was the `Profile` class that added the extra public properties). In V3 I removed the `Profile` class from the DTO definitions and handle the setup of the AutoMapper mappings within the library.

### 1. Changes ASP.NET Core Startup Class

Somewhere in the `ConfigureServices` in the `Startup` class you will have some code to set up AutoMapper and GenericBizRunner, possibly via AutoFac bundles or .NET Core DI. This needs to be replaced by new code that needs a list of all the assemblies that GenericBizRunner DTOs exist. It needs this to set up the AutoMapper mappings, e.g.

```csharp
services.RegisterGenericBizRunnerBasic<EfCoreContext>(
    Assembly.GetAssembly(typeof(WebChangeDeliveryDto)),
    Assembly.GetAssembly(typeof(DtoInAnotherAssembly)),
    //... as many assemblies as you need);
```

*NOTE: there are versions that allow you to add a `GenericBizRunnerConfig` and another form, `RegisterGenericBizRunnerMultiDbContext`, for when you are using multiple DBContexts [see Using multiple DbContexts](https://github.com/JonPSmith/EfCore.GenericBizRunner/wiki/Using-multiple-DbContexts) in Wiki.*

PS. You don't need `services.AddAutoMapper();` anymore unless you are using AutoMapper yourself. GenericBizRunner now handles setting up the mappings.

### 2. Changes to your GenericBizRunner DTOs

If you were using the default mapping then there is no change. But if you provided a AutoMapper configuration then they change. Here is the old (V2) version and the new (V3) version

#### ORIGINAL (V2) version

```csharp
public class ServiceLayerBizOutWithMappingDto 
    : GenericActionFromBizDto<BizDataOut, ServiceLayerBizOutWithMappingDto>
{
    public string Output { get; set; }
    public string MappedOutput { get; set; }

    protected override void AutoMapperSetup()
    {
        CreateMap<BizDataOut, ServiceLayerBizOutWithMappingDto>()
            .ForMember(p => p.MappedOutput, opt => opt.MapFrom(x => x.Output + " with suffix."));
    }
    //... other code left out
}
```

#### NEW (V3) version

```csharp
public class ServiceLayerBizOutWithMappingDto 
    : GenericActionFromBizDto<BizDataOut, ServiceLayerBizOutWithMappingDto>
{
    public string Output { get; set; }
    public string MappedOutput { get; set; }

    protected internal override
        Action<IMappingExpression<BizDataOut, ServiceLayerBizOutWithMappingDto>>
            AlterDtoMapping
    {
        get
        {
            return cfg => cfg
                .ForMember(p => p.MappedOutput, opt => opt.MapFrom(x => x.Output + " with suffix."));
        }
    }
    //... other code left out
}
```

### 3. Unit tests

You most likely don't need to change your unit tests of your business logic as you can call it directly. The only real case I can think of is if you want to do integration tests of ASP.NET Controllers, e.g.  Web API controllers.

In that specific type of test then you need create a valid version of GenericBizRunner's `ActionService` and you will need to change your code. The level of change depends what you are doing so here are some examples.

The examples below works for direct, i.e. no DTO mapping.

#### Old code (V2)

```csharp
public void TestActionServiceValueInOutDirectOk()
{
    //SETUP 
    var noCachingConfig = new GenericBizRunnerConfig { TurnOffCaching = true };
    var mapper = SetupHelpers.CreateEmptyMapper();  //CHANGE!!!
    var bizInstance = new YourBizAction(yourDbContext);
    var runner = new ActionService<IYourBizAction>(
        yourDbContext, bizInstance, _mapper, noCachingConfig);   //CHANGE!!!
    int input = 1;

    //ATTEMPT
    var data = runner.RunBizAction<string>(input);

    //VERIFY
    bizInstance.HasErrors.ShouldEqual(hasErrors);
    //... other tests left out
}
```

#### New code (V3)

```csharp
public void TestActionServiceValueInOutDirectOk()
{
    //SETUP
    var noCachingConfig = new GenericBizRunnerConfig { TurnOffCaching = true };
    var utData = new NonDiBizSetup(noCachingConfig);   //CHANGE!!!
    var bizInstance = new YourBizAction(yourDbContext);
    var runner = new ActionService<IYourBizAction>(
        yourDbContext, bizInstance, utData.WrappedConfig);   //CHANGE!!!
    int input = 1;

    //ATTEMPT
    var data = runner.RunBizAction<string>(input);

    //VERIFY
    bizInstance.HasErrors.ShouldEqual(hasErrors);
    //... other tests left out
}
```

In cases where you use DTOs with your business logic you need to set up the mappings. In V2 you would have had something like the code below to set up the AutoMapper mappings.

```csharp
//V2 code
var mapper = SetupHelpers
    .CreateMapper<ServiceLayerBizInDto, ServiceLayerBizOutDto>();
```

But in V3 code there is a set of NonDi helper code for setting up the internal mappings. Here is the same code, but in V3 format.

```csharp
var config = new GenericBizRunnerConfig { TurnOffCaching = true };
var utData = NonDiBizSetup.SetupDtoMapping<ServiceLayerBizInDto>(config);
utData.AddDtoMapping<ServiceLayerBizOutDto>();
```

### Other V3 changes

1. I have added a `IGenericStatus BeforeSaveChanges(DbContext)` method to the config that, if present, is called before SaveChanges/SaveChangesAsync. This allows you to provide your own validation approach, such as [fluentvalidation](https://fluentvalidation.net/), or other features such as logging etc. before a call to SaveChanges. The method returns a status, and if that status has errors then it won't call SaveChanges.
2. Better control over validation, `BeforeSaveChanges` and `SaveChangesExceptionHandler`. Each one is used if provided, while in V2 you had to have standard validation turned on for `SaveChangesExceptionHandler` to work.
3. I have removed the `AutoFac` setup, as there is now a direct NET Core version. It's quicker and with the changes I only wanted one version to support. You can still use AutoFac - you just have to register GenericBizRunner before converting over the AutoFac.

[End]