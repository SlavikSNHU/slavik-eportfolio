## **Lens Configuration Manager** 
[![Action Status](https://github.com/ruby/ruby/workflows/Windows/badge.svg)](https://github.com/SlavikSNHU/slavik-eportfolio/tree/master)
#### Allow users to selected model number and retrieved all configuration information from database to send it to external device
![UI Design](/GUI.PNG)
## **Self-Assessment**
While working on any project the most difficult part is to predict what could go wrong. Especially when you are doing it by yourself. With further experience the skill gets developed that allows you to spot more problems and address it ahead of time. But still nothing compared to work that could be accomplished while working with a team. By conducting code reviews and discussions on features and architecture, range of bugs, vulnerabilities and other issues could be prevented. 
In my ePortfolio I am hoping to outline my understanding of practices that is being used in professional setting. Below I would like to list them and give a brief description on how and why it benefited me.

1.	As I started to work on updating my application, I wanted to save the progress and history of changes that I was making. With variety of different source controls available I have chosen to use GitHub since it was free and easy to use. ePortfolio was also had to be hosted using GitPages so it made decision process easier. In my previous experience source safe have saved me multiple times, especially when Visual Studio designer went ballistic and decided to remove all UI elements without having ability to go back. Quick checkout of designer files has fixed it in no time. Git has range of other useful features that makes collaboration easier. Having ability to have multiple branches is a great feature since it provides more flexibility in path that project could take. With this I could show that I understand the importance of source control in any project.

2.	Code review is essential in finding bugs, security vulnerabilities and critical errors. It also helps to start group conversation on path forward and provide additional solutions to the problem. For me, code reviews have been a great opportunity to learn from more experienced software engineers. It also taught me to be more open minded while receiving criticism.

3.	Following standard practices while writing code is a key for making sure that others could understand it with spending least amount of time. It also helps developer since they might have to go back to add a new feature later. Adding new features should also be as simple as possible. While working on my project I make sure that my code could be scalable when required without breaking bunch of functions. Since I cannot predict everything that may be required, having ability to add new functions is the key. For example, when I had to update data structure that I was using from linked list to dictionary it was a minor change that did not break any other pieces of the project. 

4.	With the project being written with C# I did not had to worry about garbage collection and memory allocation as much as I did in the work projects on embedded platforms. Experience of working with embedded devices have made me better developer when working with high-level programming languages. Since now I understand how commonly used data structures work on the lower levels. Even though computers nowadays have insane amount of resources I still try to write clean and fast code.

As I got to the end of developing lens configuration manager, I have started to consider all potential problems that it could have once it goes live. With the main issue was amount of memory that was going to be allocated when data being pulled from database. Since I am only at the beginning of using this tool and there are not too many configurations available, amount of data that is being pulled is not a problem. But at some point, in the future its going to reach massive sizes and transferring entire table each time into application would result in critical errors. This is something I did not considered a year ago when I made initial C# SQL database control API. After going trough few data structure and database management classes I was much more prepared for handling databases. In my CS360 Mobile Architecture and Programming class I had to create an app to track weight loss progress (https://github.com/SlavikSNHU/CS360/tree/master). In it I had to use SQLite database to store user credentials and weight information. To handle credential checking all I had to do is construct SQL query that would check if username and password exists. In the past I might have used looping mechanism. As I worked through each artifact, I was able to improve overall performance, readability, and scalability of the lens configuration manager application. Below you will see assessment of all artifacts and lessons learned.

### **List of Artifacts**
1. Application memory usage.
2. Use of proper data structures for quick lookups.
3. Continue to function without connection to database on a network.

## **CS-499-02 Informal Code Review**
#### Review of implemented code and outline of artifacts that require modifications
<iframe width="640" height="415" src="https://www.youtube.com/embed/MBTTOdIVU_U" frameborder="1" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>

##  __Artifacts Breakdown__
------------
### **Application Memory Usage**
------------
Using C# API SQL query was constructed to retrieve all data from selected database table and place it inside `DataTable` class on stack. Due to this implementation as database grows, loading entire table content into the memory could lead to stack overflow error. 
#### Example for SQL query being used:
##### ```SELECT * FROM dbo.table```

In order to fix this issue new function were create that would accept additional arguments that include id column name and id number. This change would only retrieve configuration information from database for selected model id.
#### Example of new SQL query to fix the issue:
##### ```SELECT * FROM dbo.table WHERE col_id_name = id```
------------
#### *While working on this artifact I have learned how to better manage memory that is being used by application. I have also learned how to prevent potential critical errors that arise from ignoring how memory is being used and how much of it is being allocated. Now when looking at a problem I am more likely to pick a better solution.*

------------
### **Use of Proper Data Structures**
------------
To handle configuration values that consisted of number of different data types I have created a class that would handle parsing of incoming and outcoming data. For each configuration I would create a single class and place it inside linked list. When I needed to update different configurations, I would loop trough the list and check configuration name. Once I find the configuration, I am looking for I will then return its object.

The problem is with a larger amount of configuration values the task of searching for proper configuration could take a while.

Since my application is using less memory due to improvements in previous artifact I could use a hash table in form of C# `Dictionary<T,T>`. By changing from list to dictionary I was able to save huge amount of time when selecting different configurations.

#### When previously for i had to use a loop to go through the list to find right configuraiton, now I could just use `CustomConfig myConfig = configDictionary[configName];`

------------
#### *While working on this artifact I was able to better organize various configuration data inside custom class. By doing so it made my code clean and easy to understand. It also helped during debugging stages since I had to make changes only in one area. It also allowed me to handle various data types and connecting of UI elements to configuration data.*

------------
### **Continue to Function Without Connection to Database on a Network**
------------
To save configuration data for selected model I would store it inside SQL database on a server. This allowed user to choose model number they want to configure, and all configuration data would be pulled into configuration data structure mentioned in previous artifact. It is a must have feature since it saves bunch of time when trying to configure different external devices. It also prevents potential user errors of setting wrong configuration and making device to go unfunctional.

The problem with having all configurations for different models on a SQL server is that it requires connection to local network. The scenarios could occur where device may need to be reconfigured without access to local network. It could be in field upgrade or during demo outside of facility location.

The solution to this problem has required to have a local database that contained all configurations that is stored on a server during local device connection to a local network. I have selected to use SQLite database since it uses same SQL queries so I would not have to change the method in which my queries are made. With this improvement I limited dependency on connection to a server when using application.

Additional database improvement included connection checking and switching between local and external databases.

------------
#### *While working on this artifact I have learned how to setup SQLite database for the first time. It was very convenient since I had to use it on other projects. I have also had to investigate other alternatives for storing data locally that include JSON and XML file formats.*
------------



