## SimpleMongoRepo
### A Simple and Generic Repository For MongoDB

### usage:

Assuming you've a mongo collection like this:

USER:

name
surename
address
age
sex
created_at
updated_at

and you created a correspondent class:

```
public class User: : IMongoEntity{
  public string Name{get;set;}
  public string Address{get;set;}
  ...
  ...
}
```

### connect to the database

```
var ConnectionString = "My mongoconnectionstring";
IMongoDatabase _db = DatabaseFactory.Create(ConnectionString);

//Get the repository of the collection you're interested in
var repo = RepositoryFactory<User>.GetRepository(_db, "User"); // the second parameter is the name of the collection and may differ from the name of the class User
var anyoneIsNamedJohn = repo.Any(c=>c.Name.Equals("John"))
var listOfAddress = repo.GetAllValuesByPropertyName("Address"); //let me have a list of any records with such a property with values filled
var firstJohnWhatever = repo.FindBy(c => c.Name.Contains("John")).First();
var anotherWayToGetFirst = repo.First(c => c.Id == "53be7a8d3bdfab1ab4efb62c");
var countMePLease = repo.CountBy(c => c.Name == "danieleTest");
var retrievedList =repo.GetListBy(c => c.Name.Contains("john"));
```

### An Async CRUD Operation

```
var repo = RepositoryFactory<User>.GetRepository(_db, "User");
var id = ObjectId.GenerateNewId();
var user = new User {Id =id.ToString(), Name = "danieleTest",.....};
await repo.AsyncInsert(user);
var findMe = repo.FindBy(c => c.Id == id.ToString()).First();

var isUpdated = await repo.AsyncUpdateField(id.ToString(), "Name", "Daniele"); // Update
var isDeleted = await repo.AsyncDeleteById(id.ToString());  //delete
```
