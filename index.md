## **Lens Configuraiton Manager**
#### Allow users to seelected model number and retrieved all configuraiton information from database to send it to external device
![UI Design](/GUI.PNG)

## **CS-499-02 Informal Code Review**
#### Review of implemented code and outline of artifacts that require modifications
<iframe width="640" height="415" src="https://www.youtube.com/embed/MBTTOdIVU_U" frameborder="1" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>


### **List of Artifacts**
1. Application memory usage.
2. Use of proper data structures for quick lookups.
3. Continue to function without connection to database.
4. Connection checking to database.

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
#### Example for SQL query being used:

