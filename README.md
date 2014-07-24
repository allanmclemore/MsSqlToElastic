
A command-line utility for piping MS SQL data to Elastic Search.
Saves you from writing custom code for simple data pipelines.
(See Implementation Notes Below)

Syntax
------
MsSqlToElastic -dbserver [value] -database [value] -sql [value] -elasticurl [value] -index [value] -append [optional value] -pagesize [optional value] -type [optional value] -id [optional value]

Compatibility
--------------
Supports Microsoft SQL Server 2012, 2014 or later.
Not regression tested against extremely complex SQL SELECT statements.  
So complex, nested SQL statements may have issues.

Example
-------
MsSqlToElastic -dbserver localhost -database MyDb -sql "Select id, firstname, lastname, phone from customers order by id" -elasticurl "http://localhost:9200" -index customers -pagesize 10000

Required         Description                                                     
--------         -----------                                                     
-dbserver        MS SQL Server host name or computer name           

-database        MS SQL Server database name

-sql             TSQL SELECT statement

-elasticurl      URL (including port number) of elastic search instance

-index           elastic search index name (must be all lowercase)

Optional
--------
-append           Elastic search write method (true/false)
                  true appends to an existing index 
                  false overrites the index with new data
                  Default Value: true

-id               field name use for _id in Elastic Search
                  Default Value: blank
                  Blank value indicates no id, so Elastic
                  writes allow duplicate _ids for the same type.
                  If -id is specified, all Elastic writes will be upserts.

-sqlpagesize      sql server pagesize (positive integer)
                  Default Value: 1000

-type             specifies the type name written elastic documents.
                  Maps to the "_type" field in each elastic document.
                  Default Value: "object"

Help
------------
MsSqlToElastic -help
(Basic help)  

MsSqlToElastic -readme  
(Basic help and more detailed information on command usage.)

Implementation Notes
--------------------
Uses standard TSQL to pipe data from MS SQL Server into Elastic Search. 
Single command uses convention over configuration approach.
Has naive implementations for paging, index creation, and elastic upserts.
Basically just a pass-through filter that dumps SQL results to Elastic Search.
Internally uses uses Dapper, so the code has no internal data processing logic.

To keep things simple, the following 3 implementation conventions are followed:

1.  Field mapping is implicit.
The tool leverages Dapper to read from SQL.
So SQL SELECT statement fields map directly to Elastic document fields.

2.  MS SQL Server Security Authentication and Rights
Only Integrated Security is supported at this time.
User context running the SQL command needs datareader rights to the database.
 
3.  SQL Paging
Naive implementation leverages OFFSET/FETCH TSQL (MS SQL 2012 or higher)
So, "-sql" parameter must include an ORDER BY clause in the SQL SELECT.  
OFFSET/FETCH TSQL should NOT be passed in in the "-sql" switch.
The application internally appends OFFSET/FETCH TSQL to the SQL statement.

Currently the SQL read and Elastic write buffers use the same pagesize setting.
For BulkIndexing, Elastic Search documentation recommends 1000-5000 pagesize.

TODO: Implement a separate pagesize switch for elastic search write buffering, 
so read buffers and write buffers can be tuned seperately.

