MsSqlToElastic
==============

Command line utility that pipes Microsoft SQL Server query results to elastic search

Syntax:
MsSqlToElastic -dbserver [server] -database [database] -sql [sql select statement] -elasticurl [url] -index [index] -pagesize 10000

Example:
MsSqlToElastic -dbserver localhost -database MyDb -sql "Select id, firstname, lastname, phone from customers order by id" -elasticurl "http://localhost:9200" -index customers -pagesize 10000