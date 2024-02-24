# Transactional Data Web Application

Prototype of a web banking applications providing several functionalities - balances, statements, payments. 
While transactional data can be submitted and reported, every interaction with the data is tracked 
ensuring they can be checked and verified. 

This web app is founded on .NET Core MVC and data persistence is achieved by virtue of a MS SQL database.
REST API have been written to enable CRUD operations on data to be performed. Clients consume data 
using AJAX, contacting the web server asynchronously to update selected areas on screen. 
Client and server side validation have been added to ensure robustness and safety. 
Lastly, SignalR is employed to made to web app interactive and responsive. 
