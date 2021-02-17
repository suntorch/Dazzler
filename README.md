# Dazzler - a simple object mapper for .Net

## Features
Dazzler is a NuGet data access library that extends IDbConnection interface.

:+1: lightweigth and high performance.

:+1: mapping a query result to **strongly-typed** object.

:+1: 2-way binding a class property to **input** and **output* parameters.

:+1: *




## Parameterized Query
Query parameter can be given by class type such as Strongly-Typed, Anonymous or ExpandoObject.
A property name of the parameter object should match with query parameter name in order to bind it.

There are 2 different methods to specify a direction of the query parameter.

- **DirectionAttribute** attribute class.
- special **suffixes** on the property name.

If you use Anonymous class object, it does not allow any attribute and you will have to use
**suffixes** in order to specify a direction.


### Input Parameters
Default parameter direction will be **input** if it is not specified.

### Output/Return Parameters


## Execution Events
It has the following **pre** and **post** events that can globally control and monitor any execution.
The use cases can be as follows:

- to log all database operations.
- to accept/cancel database operations in centralized code base.


## DB providers can be used
It works across all .NET ADO providers including SQL Server, MySQL, Firebird, PostgreSQL and Oracle.


## Do you have a comprehensive list of examples?
You can see and learn from [test project](https://github.com/suntorch/dazzler.test).



## Installation
Please use the following command in the NuGet Package Manager Console.
```
Install-Package Dazzler
```