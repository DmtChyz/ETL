30,000 rows in source file. 29,889 ended up in DB, 111 were duplicates.
22 rows have pickup time equal to dropoff time . I kept them in the DB, if i worked in team i'd ask someone should i keep it or not, but i wasn't able to.
Any rows with missing fields will be inserted with NULLs. All columns are nullable for this reason.
Two rows sharing identical pickup/dropoff times but both having NULL passenger_count are treated as duplicates. Can't distinguish them, so safer to treat as one.
For 10GB files — CSV is already streamed row by row and inserted in batches of 5000 so memory stays flat. The only real problem at that scale is the in-memory HashSet for duplicate detection — it would eat too much RAM. I'd replace it with a staging table in SQL and handle duplicates on the DB side instead.

To run the program, place sample-cab-data.csv into /bin/Debug/net8.0/ before running.
