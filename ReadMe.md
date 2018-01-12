# Operation Extension
	
[![Windows](https://ci.appveyor.com/api/projects/status/m4xei8kvod9fguqk/branch/master?svg=true)](https://ci.appveyor.com/project/odytrice/operation/branch/master)
[![NuGet Operation](https://img.shields.io/nuget/v/Operation.svg?style=flat)](https://www.nuget.org/packages/Operation)

This Library provides a way for doing [Railway Oriented Programming](http://fsharpforfunandprofit.com/rop/) in C#. Which is simply a way of encoding Errors into the Type System.  The `Operation` class is a contract
that tells the calling method that it will not throw an exception rather, any exeptions generated will be available in the returned `Operation` object.

Using `Operation` helps ensure that your applications can fail gracefully even in unforeseen circumstances. It's typically used at the boundries 
between domains/layers in your application.eg. Between Calls from WebApi to Business Layer or between calls from your Business Layer to your DataAccess Layer

## The Operation Class

At the Heart of the library are two types. They are `Operation` and `Operation<T>`. 
An Operation represents the output of a piece of computation. It has two states: Succeeded or Failed. 
To represent this, a boolean flag `Succeeded` tells you whether the computation succeeded or failed.
It also contains a `GetException()` Method that returns the original Exception including Stack trace and all.
It also contains a helpful message that states why the piece of computation failed. `Operation<T>` 
includes a `Result` property that contains the Result of that Computation.

## Installation 
You can install Operation library via Nuget:

`install-package Operation`

## Features
### 1. Fault Tolerance

```csharp
public Operation ErrorProneFunction()
{
    return Operation.Create(() => {
    	//Doing Some Dangerous Stuff
    	throw new Exception("Halt and Catch Fire");
    });
}

var operation = ErrorProneFunction()
var suceeded = operation.Succeeded //False
var message = operation.Message    //Halt and Catch Fire
```
There are also helper functions for creating Operation Objects
```csharp
//Returns a Successful Operation<int>
Operation<int> succeededOperation = Operation.Success(20);	

//Returns a Failed Operation with the Message "Error Here"
Operation failedOperation = Operation.Fail("Error Here");
```

### 2. Operation Chaining 

- `Map` allows you to take an Operation and transform it into another Operation. It allows you to chain more operations together

```csharp
int GetNumber(int x){
    return 10;
}

string AddCups(int x){
    return x + " Cups";
}

string ToUpper(string s){
    return s.ToUpper();
}

void Main(){
    var compoundOp = Operation.Create(GetNumber)
                              .Map(AddCups) //Adds "Cups"
                              .Map(ToUpper); //Makes it UpperCase

	//Only Returns True if all 3 operations Succeeded
    var succeeded = compoundOp.Succeeded;
}
```

- `Bind` is just like map however It used to chaining multiple functions that return Operation.

```csharp
//Get Random UserId from Database
Operation<int> GetUserId(){
    return Operation.Success(15);
}

//Get User Name from the Database
Operation<string>> getNameById(int i){
    return Operation.Success("John Doe");
}

void Main()
{
    //Get User Id and Then Get User's Name
    var getUserName = GetUserId().Bind(i => GetNameById(i));
}
```

There are also helper functions that allow you to create Failed and Successful Operations easily

```csharp
Operation<int> failedOp = Operation.Fail<int>("Error Message");
Operation<int> successOp = Operation.Success(10);
```

### 3. Linq Syntax

You can also chain operations using Linq query syntax. This makes the flow of execution clearer. 
    It also allows you to easily combine the interim results of operations as shown below

```csharp
var operation1 = Operation.Success(10);
var operation2 = Operation.Success(12);
var operation3 = Operation.Success(3);

var operation = from res1 in operation1
                from res2 in operation2
                select res1 + res2 into temp
                from res3 in operation3
                select temp - res3;

var result = operation.Unwrap(); // 19 or throws an Exception if any of the Operations failed
```
The above code gets the result of `operation1` and `operation2`, adds them and then subtracts
the result of `operation3`. And of course the final operation would only be successful
if the entire computation worked without a hitch. Otherwise the Error and Message for the 
faulty function would be returned.
This should simplify chaining combining the results across operations.

*NOTE* - Using the LINQ Syntax with `Operation` returns `null`. 
Trying to access its properties will produce an null reference exception that won't be caught

### 4. Batch Operations
The linq syntax is also extended to allow you to mix Linq Queries with Operation Queries. 
This is useful for doing batch operations in a safe way

```csharp
var list = new []{ 1 , 2, 3 };

Operation<string> op = Operation.Success("Resulting String");

var query = from x in list
	    from y in op
	    select x + " " + y;

var result = query.Select(r => r.Result).ToArray(); //["1 Resulting String", "2 Resulting String", "3 Resulting String"]
```

Operations can also seemlessly transition into Linq Queries

```csharp
var list = new []{ 1 , 2, 3 };

Operation<string> op1 = Operation.Success("First");
Operation<string> op2 = Operation.Success("Second")

var query = from x in op1
	    from y in op2
	    from z in list
	    select x + " " + y + " " + z;

var result = query.ToArray(); //["1 First Second", "2 First Second", "3 First Second"]
```

The `Fold()` can also be used to collapse a sequence of operations by "folding" them into a single Operation

```csharp
var op1 = Operation.Success("Hello");
var op2 = Operation.Success("World");

var all = new[] { op1, op2 }.Fold((a, s) => a + " " + s);

var succeeded = all.Succeeded;	//true 
var result = all.Result;		//"Hello World"
```
### 5. Chaining Error Cases
In addition to Handling Success Cases, you can add error handling code for already failed exceptions

```csharp
var failed = Operation.Fail("Error Occured");
failed.Catch(o => Console.WriteLine(o.Message));
```

### 6. Operation Dependency

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
    var dependentOp = DependentOp();  	//Returns Operation<int>
    int result = dependedOp.Unwrap();
    return result + 2;			//Runs only if dependedOp was successful.
});
```

If your create Method returns a Operation you should use `CreateBind` instead

```csharp
var operation = Operation.CreateBind(() => {
	var dependentOp = DependentOp();
	return dependentOp;
});
```

### 7. Async Support
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

### 8. Conversion
Its also easy to Convert Operations to Tasks for APIs that Require Tasks

```csharp
var op = Operation.Create(() => doStuff());
Task<int> t = op.AsTask();
```

Note that this is not the same as `Operation.Run` Execution will still block the running Thread

Documentation available here
[Github Readme](https://github.com/odytrice/Operation/blob/master/ReadMe.md)