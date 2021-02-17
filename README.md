# Dazzler - a simple object mapper for .Net

## Features
Dazzler is a NuGet data access library that extends IDbConnection interface.

- :+1: lightweight and high performance. :rocket: 
- :+1: mapping a query result :scroll: to **`strongly-typed`** object.
- :+1: 2-way binding :link: a class property to **`input`** and **`output`** parameters.
- :+1: *



## Parameterized Query
A Strongly-Typed, Anonymous and ExpandoObject object can be passed as query parameters
and a property name should match with query parameter name in order to bind it.

There are 2 methods to specify a direction of the query parameter.

- **`DirectionAttribute`** attribute class.
- special **`suffixes`** on the property name.

If you use Anonymous class type object, it does not allow any attribute and you will have to use
**suffixes** in order to specify a direction.

### Property Name Suffixes
Suffix format is **PropertyName`[__in|out|ret[size]]`**.

Here:
| Component | Description |
| --- | --- |
|`__`| an identifiers of the suffix. (double Low Line '0x5F')
|`in`| specifies as **input** parameter.
|`out`| specifies as **output** parameter.
|`inout`| specifies as **input** and **output** parameter.
|`ret`| specifies as **return** parameter. Used to call database funciton.



### Input/Output Parameters
Default direction is always **input** and no need to specify, but you could.

Using Anonymous class type:
```csharp
var args = new
{
   value1 = 999, // same as value1__in = 999
   value2__out = 0
};

var result = connection.NonQuery(CommandType.Text, $"set @value2=@value1", args);
Assert.AreEqual(args.value1, args.value2__out, "Invalid output value.");
```

Using Strongly-Typed class type with suffixes:
```csharp
public class ModelClass
{
   public int value1 { get; set; }
   public int value2__out { get; set; }
};
```

```csharp
ModelClass args = new ModelClass()
{
   value1 = 999,
   value2__out = 0
};

var result = connection.NonQuery(CommandType.Text, $"set @value2=@value1", args);
Assert.AreEqual(args.value1, args.value2__out, "Invalid output value.");
```

Using Strongly-Typed class type with attribute:
```csharp
public class ModelClass
{
   public int value1 { get; set; }

   [Direction(Direction.Output)]
   public int value2 { get; set; }
};
```
```csharp
ModelClass args = new ModelClass()
{
   value1 = 999,
   value2 = 0
};

var result = connection.NonQuery(CommandType.Text, $"set @value2=@value1", args);
Assert.AreEqual(args.value1, args.value2, "Invalid output value.");
```




## Execution Events
It has the following **pre** and **post** events that enable to control and monitor any database operations globally.

- ExecutingEvent(CommandEventArgs args)
- ExecutedEvent(CommandEventArgs args, ResultInfo result)


The use cases can be as follows:

- to log all database operations.
- to accept/cancel database operations in centralized code base.


## DB providers can be used
It works across all .NET ADO providers including SQL Server, MySQL, Firebird, PostgreSQL and Oracle.


## Examples
You can see and learn from the test project [Dazzler.Test](https://github.com/suntorch/dazzler.test).



## Installation
Please use the following command in the NuGet Package Manager Console to install the library.
```
Install-Package Dazzler
```