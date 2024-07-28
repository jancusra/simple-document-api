# Simple API with different data sources and different response types

**Assignment: Create production-ready ASP.NET Core service app that provides API for storage and retrive documents in different formats**

1. The documents are send as a payload of POST request in JSON format and could be modified via PUT verb. \
**3 API methods added: GET/POST/PUT for document operations**

2. The service is able to return the stored documents in different format, such as XML, MessagePack, etc. \
**The service is able to return documents in (by Accept header): JSON (application/json), XML (application/xml) and MessagePack (application/x-msgpack)**