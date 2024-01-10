## Using ADO.NET

This library provides many convenience methods to access data with ADO.NET such as:

- Execute an SQL statement,
- Run SELECT query to return a DataTable or DataReader
- Setup a DataAdapter to manage CRUD operations of a DataTable
 
**Most importantly, it will manage the openning and closing of the Connection object to prevent orphan connections that might exhaust the connection pool. If the connection pool is exhausted, no application can connect to the database until some connections has timeout.**

## Working with types in System.Data.Common

You should try your best to use only types from the System.Data.Common namespace on all your ADO.NET public methods and interfaces. This design will provide the flexibility if you need to switch to a different data provider or library with minimal impact to your existing codebase. In this library, the primary public data access interface is the _IDatabase_ interface. This interface will hide the implementations that are specific to each data provider.

## Managing the database connection

The type _IDatabase_ and _DataReaderResult_ manage a database connection internally and therefore it should be _Dispose()_ when no longer needed to prevent orphan database connection. Connection should not be saved. Connection should be opened and closed just long enough to perform a unit of work. Connection should not be passed outside of the current method or outside of the current object if possible. 