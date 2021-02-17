# Dazzler - a simple object mapper for .Net

## Features
Dazzler is a NuGet data access library that extends IDbConnection interface.

- lightweight and high performance. :rocket: 
- mapping a query result :scroll: to **`strongly-typed`** object.
- 2-way binding :link: a class property to **`input`** and **`output`** parameters.



## Parameterized Query
A **Strongly-Typed**, **Anonymous** and **ExpandoObject** object can be passed as query parameters
and a property name should match with query parameter name in order to bind it. 
After a query is executed, **Output** and **Function Return** parameter will automatically take 
the value that is returned by a query. You don't have to do any extra operation. :+1:

There are 2 methods to specify a direction of the query parameter.

- **`DirectionAttribute`** attribute class.
- special **`suffixes`** for the property name.

For the **strongly-typed** class type, both methods can be used.
But, Anonymous class type does not allow any attribute implementation, 
therefore, you will have to use **suffixes** in order to specify a direction.



### Property Name Suffixes
A suffix is a special notation that is specified at the end of the property name
and it must use the format **PropertyName`[__in|out|ret[size]]`**.

A suffix consists of the following components:
| Component | Description |
| --- | --- |
|`__`| an identifiers of the suffix. (double Low Line '0x5F')
|`in`| specifies as **input** parameter.
|`out`| specifies as **output** parameter.
|`inout`| specifies as **input** and **output** parameter.
|`ret`| specifies as **return** parameter. Used to call database funciton.
|`size`| specifies as value size of the parameter. For example: `__out50`, `__ret200`, etc.



### Input/Output Parameters
Default direction is always **input** and no need to specify, but you could.

Using Anonymous class type:
```C#
var args = new
{
   value1 = 999, // same as value1__in = 999
   value2__out = 0
};

var result = connection.NonQuery(CommandType.Text, $"set @value2=@value1", args);
Assert.AreEqual(args.value1, args.value2__out, "Invalid output value.");
```

Using Strongly-Typed class type with suffixes:
```C#
public class ModelClass
{
   public int value1 { get; set; }
   public int value2__out { get; set; }
};
```

```C#
ModelClass args = new ModelClass()
{
   value1 = 999,
   value2__out = 0
};

var result = connection.NonQuery(CommandType.Text, $"set @value2=@value1", args);
Assert.AreEqual(args.value1, args.value2__out, "Invalid output value.");
```

Using Strongly-Typed class type with attribute:
```C#
public class ModelClass
{
   public int value1 { get; set; }

   [Direction(Direction.Output)]
   public int value2 { get; set; }
};
```
```C#
ModelClass args = new ModelClass()
{
   value1 = 999,
   value2 = 0
};

var result = connection.NonQuery(CommandType.Text, $"set @value2=@value1", args);
Assert.AreEqual(args.value1, args.value2, "Invalid output value.");
```


## Execute Commands
You can pass input parameters and fetch records and bring back all output parameter values.

```TSQL
CREATE OR ALTER PROCEDURE MyStoredProcedure
   @Name varchar(50),
   @Age int,
   @Value int
AS
BEGIN
   -- any output parameters.
   set @Value = 99

   -- any returning query records.
   select @Name Name, @Age Age
END
```

```C#
var args = new
{
   Name = "John",
   Age = 25,
   Value__out = 0
};

var result = connection.Query<ModelClass>(CommandType.StoredProcedure, "MyStoredProcedure", args);
Assert.AreEqual(1, result.Count, "Invalid record count.");
Assert.AreEqual(25, result[0].Age, "Fetched wrong record.");
Assert.AreEqual(99, args.Value__out, "Invalid output value.");
```


## Paging
It allows to read a some count of records from given offset position.

```C#
string sql = "select Age from ( values (1),(2),(3),(4),(5),(6),(7) ) as tmp (Age)";

var result = connection.Query<ModelClass>(CommandType.Text, sql, offset: 2, limit: 2);

Assert.AreEqual(2, result.Count, "Invalid output record count.");
Assert.AreEqual(3, result[0].Age, "Fetched wrong record.");
Assert.AreEqual(4, result[1].Age, "Fetched wrong record.");
```


## Execution Events
Some application needs to monitor, log and control database actions globally without writing an extra code.
Using the following **pre** and **post** events, it allows to implement such needs.

- ExecutingEvent(CommandEventArgs args)
- ExecutedEvent(CommandEventArgs args, ResultInfo result)


The use cases can be as follows:

- to monitor/report all Update/Delete/Insert operations.
- to monitor/report top Nth long running queries.
- to accept/reject a query execution in centralized code base.



## DB providers can be used
It works across all .NET ADO providers including SQL Server, MySQL, Firebird, PostgreSQL and Oracle.



## Examples
You can see and learn from the test project [Dazzler.Test](https://github.com/suntorch/dazzler.test).



## Installation
Please use the following command in the NuGet Package Manager Console to install the library.
```
Install-Package Dazzler
```