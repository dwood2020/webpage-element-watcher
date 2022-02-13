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

`app.cfg` is not contained in this repository for security reasons.   
To get started, copy the provided `demo-app.cfg`file and rename it to `app.cfg`. 
The demo file is well commented and intended to be used as reference.   

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
