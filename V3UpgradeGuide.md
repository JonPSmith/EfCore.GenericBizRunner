# Upgrade guide - GenericBizRunner version 3

Version 3 (V3) of [EfCore.GenericBizRunner](https://github.com/JonPSmith/EfCore.GenericBizRunner) changes (improves) the way it configures [AutoMapper](https://automapper.org/), plus adds a number of useful improvements. The problem is this could well break any code you wrote to work with version 2 of the library. This page tells you what has changed and tells you what to do if you upgrade and get compile errors.

## Motivation for these changes

EfCore.GenericBizRunner uses AutoMapper to map inputs and outputs to the business logic *(you can find out why from the [Anti corruption feature](https://github.com/JonPSmith/EfCore.GenericBizRunner/wiki/Anti-corruption-feature) page in the Wiki)*. I made a mistake around how to configure AutoMapper mappings, which added extra properties to the the DTO. That's a small problem with ASP.NET Core Razor pages, but its a big pain with ASP.NET Core Web API.

Version 3 of EfCore.GenericBizRunner fixes this and also adds some useful features found in [EfCore.GenericServices](https://github.com/JonPSmith/EfCore.GenericServices). See the [Release Notes](https://github.com/JonPSmith/EfCore.GenericServices/blob/master/ReleaseNotes.md) for information on all the changes.

## How the AutoMapper changes affect your code

In versions before V3 I placed AutoMapper's `Profile` class inside the DTOs and used `AddAutoMapper()` to set up the mapping. In V3 I removed the `Profile` class from the DTO definitions and handle the setup of the AutoMapper mappings within the library. **If you used BizRunner's DTOs, e.g. `GenericActionFromBizDto<TBizOut, TDtoOut>`, then you will need to make the following changes to your code.**

### ASP.NET Core Startup Class

Somewhere in the `ConfigureServices` in the `Startup` class you will have some code to set up AutoMapper and GenericBizRunner, possibly via AutoFac bundles or .NET Core DI. This needs to be replaced by new code that needs a list of all the assemblies that GenericBizRunner DTOs exist. It needs this to set up the AutoMapper mappings, e.g.

```csharp
services.RegisterGenericBizRunnerBasic<EfCoreContext>(
    Assembly.GetAssembly(typeof(WebChangeDeliveryDto)),
    Assembly.GetAssembly(typeof(DtoInAnotherAssembly)));
```

*NOTE: there are versions that allow you to add a `GenericBizRunnerConfig` and another form, `RegisterGenericBizRunnerMultiDbContext` for when you are using multiple DBContexts [see Using multiple DbContexts](https://github.com/JonPSmith/EfCore.GenericBizRunner/wiki/Using-multiple-DbContexts) in Wiki.*

PS. You don't need `services.AddAutoMapper();` anymore unless you are using AutoMapper yourself. GenericBizRunner.

## 