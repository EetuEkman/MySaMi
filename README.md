# MySaMi
MySaMi provides visualized results from [Savonia Measurements](https://sami.savonia.fi/) service. Savonia Measurements provides access to sensors and related measurement data resources.

To consume these resources, user needs to provide a key. With MySaMi, user can save these keys to make access less tedious. MySaMi remembers the users last three queries as a reminder.

## Installation
MySaMi targets .NET Core 3.0.

MySaMi uses Sqlite and Entity Framework Core with code-first approach. After building the project, update the project with Entity Framework Core using command `dotnet ef database update` in the command line at the MySaMi project folder. This creates the database file MySaMi.db at the project folder.

To change the database file or connection, connection string for the database file can be changed in the appsettings.json
