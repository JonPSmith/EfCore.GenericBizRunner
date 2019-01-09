# Release Notes

## planned changes

- New feature: Once AutoMapper with `MapTo` attribute out, then change DI and NoDi code to support that style of DTO.

## 3.0.0

- BREAKING CHANGE: Changed the way in which AutoMapper mappings are set up. Please read the [Upgrade Guide]() as this change may break your startup and unit testing code.
- Breaking change: removed AutoFac DI setup - now only provides Net Core setup (you can still use AutoFac, but GenericBizRunner should be registered to normal Net Core DI).
- New Feature: Added `IGenericStatus BeforeSaveChanges(DbContext)` to configuration. This allows you to inject code that is called just before SaveChanges/SaveChangesAsync is run. This allows you to add some validation, logging etc.
- Improvement: Previously the Sql error handler was only used if validation was turned on. Now, if the SaveChangesExceptionHandler property is not null, then that method is called, i.e. it is not longer dependant on the state of the ...ValidateOnSave flag in the config. 

## 2.0.1

- Bug Fix: Fixed a error in StatusGenericHandler where `CombineErrors` didn't copy over the `Message` properly

## 2.0.0

- Breaking Change: The SaveChangesWithValidation/Async interface has changed to accommodate the new SaveChangesExceptionHandler.
- New Feature: Added SqlErrorHandler to configuration and called in SaveChangesWithValidation/Async. Allows you to intercept an exception in SaveChanges and do things like capture SQL errors and turn them into user-friendly error messages.
- New Feature: A version of the GenericBizRunner dependency injection registration that works with NET Core DI provider.
- Minor change: added GetAllErrors method to IBizActionStatus to get all errors in one go. Mainly useful for unit testing.

## 1.1.1 (unlisted as replaced by 2.0.0)

- New Feature: Added SqlErrorHandler to configuration and called in SaveChangesWithValidation/Async.
Allows you to intercept `DbUpdateException` and turn SQL errors into user-friendly error messages.
- New Feature: A version of the GenericBizRunner dependency injection registration that works with NET Core DI provider 
- Minor change: added GetAllErrors method to IBizActionStatus to get all errors in one go.
Mainly useful for unit testing. 

## 1.1.0

- Package: Updated to NET Core 2.1

## 1.0.0 

- First release