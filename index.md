### **Lens Configuraiton Manager**
#### Allow users to seelected model number and retrieved all configuraiton information from database to send it to external device
![UI Design](/GUI.PNG)

### **CS-499-02 Informal Code Review**
#### Review of implemented code and outline of artifacts that require modifications
<iframe width="640" height="415" src="https://www.youtube.com/embed/MBTTOdIVU_U" frameborder="1" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>


### **List of Artifacts**
1. Application memory usage.
2. Use of proper data structures for quick lookups.
3. Continue to function without connection to database.
4. Connection checking to database.

### **Artifacts Breakdown**
<div style="width:641px; height:76px; overflow:hidden">
  <img src="https://miro.medium.com/max/700/0*__5nhm_2qHSrTVoZ" width="640" height="120">
</div>

### **Application Memory Usage**
------------
#### Using C# API SQL query was constructed to retrieve all data from selected database table and place it inside `DataTable` class on stack. Due to this implementation as database grows, loading entire table content into the memory could lead to stack overflow error. 
#### Example for SQL query being used:
##### ```SELECT * FROM dbo.table```

#### In order to fix this issue new function were create that would accept additional arguments that include id column name and id number. This change would only retreive configuraiton information from database for selected model id.
#### Example of new SQL query to fix the issue:
##### ```SELECT * FROM dbo.table WHERE col_id_name = id```
------------

