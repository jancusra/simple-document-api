# Simple API with different data sources and different response types

### Assignment: Create production-ready ASP.NET Core service app that provides API for storage and retrive documents in different formats

1. The documents are send as a payload of POST request in JSON format and could be modified via PUT verb. \
**3 API methods added: GET/POST/PUT for document operations**

2. The service is able to return the stored documents in different format, such as XML, MessagePack, etc. \
**The service is able to return documents in (by Accept header): JSON (application/json), XML (application/xml) and MessagePack (application/x-msgpack)**

3. It must be easy to add support for new formats. \
**New formats can easily be added to the services in the Startup.cs file (using the library or a custom formatter)**

4. It must be easy to add different underlying storage, like cloud, HDD, InMemory, etc. \
**It´s easy to switch between Memory and SQL Server storage in the appsettings.json file ("memory" or "sqlserver")**

5. Assume that the load of this service will be very high (mostly reading). \
**All operations are used as async methods**

6. Demonstrate ability to write unit tests. \
**Added 2 unit tests for cache memory operations, also added 3 integration tests to test all response formats JSON/XML/MessagePack**
