## **Lens Configuration Manager** 
[![Action Status](https://github.com/ruby/ruby/workflows/Windows/badge.svg)](https://github.com/SlavikSNHU/slavik-eportfolio/tree/master)
#### Allow users to selected model number and retrieved all configuration information from database to send it to external device
![UI Design](/GUI.PNG)

## **CS-499-02 Informal Code Review**
#### Review of implemented code and outline of artifacts that require modifications
<iframe width="640" height="415" src="https://www.youtube.com/embed/MBTTOdIVU_U" frameborder="1" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>


### **List of Artifacts**
1. Application memory usage.
2. Use of proper data structures for quick lookups.
3. Continue to function without connection to database on a network.

##  __Artifacts Breakdown__
------------
### **Application Memory Usage**
------------
#### Using C# API SQL query was constructed to retrieve all data from selected database table and place it inside `DataTable` class on stack. Due to this implementation as database grows, loading entire table content into the memory could lead to stack overflow error. 
#### Example for SQL query being used:
##### ```SELECT * FROM dbo.table```

#### In order to fix this issue new function were create that would accept additional arguments that include id column name and id number. This change would only retrieve configuration information from database for selected model id.
#### Example of new SQL query to fix the issue:
##### ```SELECT * FROM dbo.table WHERE col_id_name = id```
------------
#### *While working on this artifact I have learned how to better manage memory that is being used by application. I have also learned how to prevent potential critical errors that arise from ignoring how memory is being used and how much of it is being allocated. Now when looking at a problem I am more likely to pick a better solution. *
------------
### **Use of Proper Data Structures**
------------
#### To handle configuration values that consisted of number of different data types I have created a class that would handle parsing of incoming and outcoming data. For each configuration I would create a single class and place it inside linked list. When I needed to update different configurations, I would loop trough the list and check configuration name. Once I find the configuration, I am looking for I will then return its object.
#### The problem is with a larger amount of configuration values the task of searching for proper configuration could take a while.
#### Since my application is using less memory due to improvements in previous artifact I could use a hash table in form of C# `Dictionary<T,T>`. By changing from list to dictionary I was able to save huge amount of time when selecting different configurations.
#### When previous for example i had to use a loop to go through the list, now I could just use `CustomConfig myConfig = configDictionary[configName];`
------------
#### *While working on this artifact I was able to better organize various configuration data inside custom class. By doing so it made my code clean and easy to understand. It also helped during debugging stages since I had to make changes only in one area. It also allowed me to handle various data types and connecting of UI elements to configuration data. *
------------
### **Continue to Function Without Connection to Database on a Network**
------------
#### To save configuration data for selected model I would store it inside SQL database on a server. This allowed user to choose model number they want to configure, and all configuration data would be pulled into configuration data structure mentioned in previous artifact. It is a must have feature since it saves bunch of time when trying to configure different external devices. It also prevents potential user errors of setting wrong configuration and making device to go unfunctional.
#### The problem with having all configurations for different models on a SQL server is that it requires connection to local network. The scenarios could occur where device may need to be reconfigured without access to local network. It could be in field upgrade or during demo outside of facility location.
#### The solution to this problem has required to have a local database that contained all configurations that is stored on a server during local device connection to a local network. I have selected to use SQLite database since it uses same SQL queries so I would not have to change the method in which my queries are made. With this improvement I limited dependency on connection to a server when using application.
#### Additional database improvement included connection checking and switching between local and external databases.
------------
#### *While working on this artifact I have learned how to setup SQLite database for the first time. It was very convenient since I had to use it on other projects. I have also had to investigate other alternatives for storing data locally that include JSON and XML file formats. *
------------



