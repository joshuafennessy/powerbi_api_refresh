# powerbi_api_refresh
Attempt at building incremental refresh using the PowerBI API.

This application connectes to a database, builds a JSON set of rows and pushes them 10000 rows at at time to the Power BI API.

Currently, the API does not allow row additions to datasets created with a database connection. The dataset must be created with the Power BI API to use the AddRows functionality.

As it stands today, this code is not suitable for incremental refresh of a database connected Power BI table -- but it is still useful for building a managed application to load Power BI dataset programatically.
