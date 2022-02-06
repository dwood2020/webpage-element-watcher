# Webpage Element Watcher

A simple .NET 6 console application which periodically watches specific HTML elements on the web.  
Only works for static websites which rely on server-side page rendering.   
This project serves as a small introduction to .NET 6.  

---
### Getting started - User   

#### Runtime   

This project runs on .NET 6. If `dotnet --version` returns a version number < 6, install the .NET 6 Runtime from [here](https://dotnet.microsoft.com/en-us/download).  
(May also work on older versions of .NET / .NET Core but this is not tested.)  

#### Configuration   

The entire application is constructed from the TOML configuration file `app.cfg` (See TOML language specs [here](https://toml.io/en/) if desired).   
Each webpage and its HTML element that shall be watched is referred to as a `Job`.
Jobs are continuously executed in periodic intervals.
[**XPath Syntax**](https://www.w3schools.com/xml/xpath_syntax.asp) is used to identify the HTML page element whos content shall be watched.   

Configuration parameters are briefly explained below:   
(Note that small lettering and snake_case is used throughout the entire config file)    

- `interval_seconds`: The time interval in which the jobs are executed in seconds (see below). Min. 60.
- `database_path`: TBD 
**user**: 
- `name`: User name
- `mail`: User email address
**logger**:
- `verbosity`: Log verbosity. Set to 1,2, or 3 (1 = only Errors are logged, 2 = Warnings, 3 = Info/All). 
- `show_xpath_query_result`: Set to `true` or `false` - `true` if the jobs shall output their XPath query results. Use this for testing the provided XPath (see below)  
**database**: 
- `path`: File path to where the database shall be stored. Best keep this local to the executable.
**jobs** (Multiple):  
- `name`: Descriptive name of the job solely used for debugging/logging
- `url`: URL to the webpage which shall be watched
- `xpath`: XPath Syntax to the page element
- `result_type`: Set to `"number"` or `"string"`. Each job can either yield a string content (this is the default) or try to parse a number (double) from the content. 


---
### Getting started - Developer  

This is a .NET 6 project (Runtime can be downloaded [here](https://dotnet.microsoft.com/en-us/download)).   
Build and run the project on the CLI with `dotnet run`.   

TBC.

---
### Dependencies   

- [Tomlyn](https://github.com/xoofx/Tomlyn) by Alexandre Mutel (BSD-2)  
- [HtmlAgilityPack](https://github.com/zzzprojects/html-agility-pack) by ZZZ Projects (MIT)  
- [Microsoft.Data.Sqlite](https://docs.microsoft.com/de-de/dotnet/standard/data/sqlite/?tabs=netcore-cli) by Microsoft  (MIT)
