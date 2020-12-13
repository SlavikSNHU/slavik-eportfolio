## **Lens Configuraiton Manager**
#### Allow users to seelected model number and retrieved all configuraiton information from database to send it to external device
![UI Design](/GUI.PNG)

## **CS-499-02 Informal Code Review**
#### Review of implemented code and outline of artifacts that require modifications
<iframe width="640" height="415" src="https://www.youtube.com/embed/MBTTOdIVU_U" frameborder="1" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>


### **List of Artifacts**
1. Application memory usage.
2. Use of proper data structures for quick lookups.
3. Continue to function without connection to database on a network.

##  __Artifacts Breakdown__

### **Application Memory Usage**
------------
#### Using C# API SQL query was constructed to retrieve all data from selected database table and place it inside `DataTable` class on stack. Due to this implementation as database grows, loading entire table content into the memory could lead to stack overflow error. 
#### Example for SQL query being used:
##### ```SELECT * FROM dbo.table```

#### In order to fix this issue new function were create that would accept additional arguments that include id column name and id number. This change would only retreive configuraiton information from database for selected model id.
#### Example of new SQL query to fix the issue:
##### ```SELECT * FROM dbo.table WHERE col_id_name = id```
------------
### **Use of Proper Data Structures**
------------
#### To handle configuration values that consisted of number of different data types i have created a class that would handle parsing of incoming and outcoming data. For each configuraiton i would create a single class and place it inside linked list. When i needed to update different configurations i would loop trough the list and check configuraiton name. Once i find the configuraiton i am looking for i will then return its object.
#### The problem is with a larger amount of configuraiton values the task of searching for proper configuraiton could take awhile.
#### Since my applicaiton is using less memory due to improvements in previous artifact i could use a hash table in form of C# `Dictionary<T,T>`. By changing from list to dictionary i was able to save huge amount of time when selecting different configurations.
#### When previous for exmaple i had to use a loop to go trough the list, now i could just use `CustomConfig myConfig = configDictionary[configName];`
------------
### **Continue to Function Without Connection to Database on a Network**
------------
#### To save configuraiton data for selected model i would store it inside SQL database on a server. This allowed user to choose model number they want to configure and all configuraiton data would be pulled into configuraiton data strucutre mentioned in previous artifact. It's a must have feature since it saves bunch of time when trying to configure different external devices. It also prevents potential user errors of setting wrong configuration and making device to go unfunctional.
#### The problem with having all configuraitons for different models on a SQL server is that it requires connection to local network. The scenarious could occur where device may need to be reconfigured without access to local network. It could be in field upgrade or during demo outside of facility location.
#### The solution to this problem has required to have a local database that contained all configuraitons that is stored on a server during local device connection to a local network. I have selected to use SQLite database since it uses same SQL queries so i would not have to change the method in which my queries is made. With this improvement i limited dependancy on connection to a server when using applicaiton.
#### Additional database improvement included connection checking and switching between local and external databases.


