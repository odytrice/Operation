#Operation Extension

This Library defines an Abstraction for [Defensive Programming](http://en.wikipedia.org/wiki/Defensive_programming). 
Its based on the [Monad](http://en.wikipedia.org/wiki/Monad_%28functional_programming%29) pattern.
Using the Operation Monad helps ensure that you applications can fail gracefully even in unforeseen circumstances.
Defensive Programming is not about hiding failures, Its about recognizing and embracing them. It's typically used at the boundries 
between domains/layers in your application.eg. Between Calls from WebApi to Business Layer or between calls from your Business Layer to your DataAccess Layer

For more information about the `Monad` pattern, Checkout this Talk by Ben Albahari about Programming with Purity

[![Programming with Purity](http://img.youtube.com/vi/aZCzG2I8Hds/mqdefault.jpg)](http://www.youtube.com/watch?v=aZCzG2I8Hds)<br/>
Programming with Purity

##Operation and Operation&lt;T&gt;
At the Heart of the library are two types. They are `Operation` and `Operation<T>`. 
An Operation represents the output of a piece of computation. It has two states: Success or Failure. To represent this, a boolean flag `Succeeded` tells you wether the computation succeeded or failed.
It also contains an Error field that contains the complete Exception including the Stack trace and all.
It also contains a helpful message that states why the piece of computation failed. `Operation<T>` 
also possesses a `Result` property that contains the Result of that Computation.

##Installation 
You can install Operation library via Nuget:

<code>install-package Operation</code>

##Features
###1. Fault Tolerance

```csharp
public void ErrorProneFunction()
{
	//Doing Some Stuff
	throw new Exception("Halt and Catch Fire");
}

var operation = Operation.Create(ErrorProneFunction);
var suceeded = operation.Succeeded //False
var message = operation.Message    //Halt and Catch Fire
```

###2. Operation Chaining
You can chain multiple Operations together to produce a compound Operation

```csharp
var compoundOp = Operation.Create(ErrorFunction1)
						  .Next(ErrorFunction2)
						  .Next(ErrorFunction3);
							  
var suceeded = compoundOp.Succeeded //Only Returns True if all 3 operations Succeeded
```

The Return values for Operations are passed on to the Next Functions if They accept parameters

```csharp
var compoundOp = Operation.Create(() => ErrorFunction1())
						  .Next(r1 => ErrorFunction2(r1))
						  .Next(r2 => ErrorFunction3(r2));
						  							  
var suceeded = compoundOp.Succeeded //Only Returns True if all 3 operations Succeeded
```
`r1` and `r2` are the return values of `ErrorFunction1` and `ErrorFunction2` respectively.

###3. Linq Syntax

The `Operation<T>` also allows you chain operations using Linq query syntax. This makes the flow of execution clearer. 
    It also allows you to easily combine the interim results of operations as shown below

```csharp
var operation = from res1 in operation1
				from res2 in operation2
				select res1 + res2 into temp
				from res3 in operation3
				select temp - res3;
```
The above code gets the result of `operation1` and `operation2`, adds them and then subtracts
the result of `operation3`. And of course the final operation would only be successful
if the entire computation worked without a hitch. Otherwise the Error and Message for the 
faulty function would be returned.
This should simplify chaining combining the results across operations.

###4. Operation Dependency

If an Operation depends on another operation, you simply call the `Unwrap()` method on the dependent Operation
and it halts exectuion and raises the error for the main operation to catch.

```csharp
var operation = Operation.Create(() => {
	var dependedOp = DependentOp();	//Returns an Operation
		
	dependedOp.Unwrap(); //Throws an Exception up if the the Operation did not succeed
	dependedOp.Unwrap("Simpler Error Message");
	dependedOp.Unwrap(e => "Simpler Error Message: " + e);
});
```
The `Unwrap()` method also serves to unwrap the `Operation` object into its constituent 
result and throws an Exception if the operation failed.

NOTE: Throwing with a custom message replaces the original Exception. 
This is useful if you do not want clients to see the original exception 
but should instead see a more friendly error message.

The `Unwrap()` method can also be used to make operations depend on other operations in a
clean way

```csharp
var operation = Operation.Create(() => {
	var dependentOp = DependentOp();  //Returns Operation<int>
	int result = dependedOp.Unwrap();
	return result + 2;				//Runs only if dependedOp was successful.
});
```

###5. Async Support
```csharp
Task<Operation<T>> asyncOp = Operation.Run(async () => {
	var result = await SomeLongRunningProcess();
	//Do Other Stuff
	return result;
});
	

var success = asyncOp.Result.Success	//Returns True if SomeLongRunningProcess() succeeds
var message = asyncOp.Result.Message	//Returns the message of the
var result = asyncOp.Result.Result		//Result of SomeLongRunningProcess() 
```

###6. Conversion
Its also easy to Convert Operations to Tasks for APIs that Require Tasks

```csharp
var op = Operation.Create(() => doStuff());
Task<int> t = op.AsTask();
```

Note that this is not the same as `Operation.Run` Execution will still block the running Thread