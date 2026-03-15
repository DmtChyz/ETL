30,000 rows in source file. 29,889 ended up in DB, 111 were duplicates.
22 rows have pickup time equal to dropoff time . 49 with null passengersCount
Any rows with missing fields will be inserted with default values (you can uncomment part of the code and it will be inserted with NULLs). All columns are nullable for this reason.
The task description did not specify strict requirements for handling missing values, duplicate resolution strategy, or exact data constraints (e.g., which columns should be NOT NULL). Because of that, several implementation decisions were made based on reasonable engineering assumptions rather than explicit instructions.
So if i worked within a team i'd ask someone should i keep it null or default, or even delete "unmanaganed data".

Two rows sharing identical pickup/dropoff times but both having NULL passenger_count are treated as duplicates. Can't distinguish them, so safer to treat as one.
For 10GB files - CSV is already streamed row by row and inserted in batches of 5000 so memory stays flat. The only real problem at that scale is the in-memory HashSet for duplicate detection - it would eat too much RAM. I'd replace it with a staging table in SQL and handle duplicates on the DB side instead.

Database is created automatically via EnsureCreated() on first run - no manual setup needed.
In production, migrations (e.g. EF Core migrations) would be preferred over EnsureCreated() 
since they allow schema versioning and safe incremental updates without dropping data.

To run the program, place sample-cab-data.csv into /bin/Debug/net8.0/ before running.

Also about bulk insertion, there also could be used this pipeline, which is perfect case for insertion 10GB data:
CsvReader -> IDataReader -> SqlBulkCopy
