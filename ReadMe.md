#Operation Extensions (OpExtension)

This Library defines an Abstraction for [Defensive Programming](http://en.wikipedia.org/wiki/Defensive_programming). 
Its based on the [Monad](http://en.wikipedia.org/wiki/Monad_%28functional_programming%29) pattern.
Using the Operation Monad helps ensure that you applications can fail gracefully even in unforeseen circumstances.

##Operation and Operation&lt;T&gt;
At the Heart of the libaray are two types. They are `Operation` and `Operation<T>`. 
An Operation represents the output of a piece of computation. It has two states: Successful or Failed
It also contains a helpful message that states why the piece of computation failed. `Operation<T>` 
also posesses a `Result` property that contains the Result of that Computation


##Installation 
You can install OpExtensions via Nuget:

<code>install-package OpExtensions</code>