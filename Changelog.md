#Version 1.0.0
- Moved to 1.0 Since this library is now used in production environments and the API surface is not going to change

#Version 0.1.7
- Added Change Log :smile:
- Moved Extensions namespace from `System.Extensions` to `System.OperationExtensions` to prevent namespace collision
- Added `Operation.Unwrap()` method instead of `Operation.Throw()`. Throw will always be available but Unwrap is clearer
