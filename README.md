Norm 
==================================================

(Yet Another) .Net object-relational mapper

Overview
-----------

Norm is an opinionated object-relational mapper (ORM). It relies on a specific set of [naming conventions](#naming-conventions) for your objects. Yes, strict adherence to an arbitrary set of naming conventions limits the range of use cases that Norm can handle, but I was scratching my own itch, and wanted a simple ORM for smaller projects that does only what I need it to, and doesn't require a bunch of setup to get started.

Norm defines a single class for interacting with your database, aptly named `Database`. You have 2 options for initializing a `Database` object:

```csharp
// Pass in the connection string. Norm will open a connection to the database, and close it when it's done.
public Database(string connectionString)
```

```csharp
// Pass in the connection. Norm will assume the connection is already open, and leave it open when it's done.
public Database(IDbConnection connection)
```

Methods
----------

The `Database` class defines 4 methods for performing your typical CRUD database operations: [Select<T>](#select), [Insert](#insert), [Update](#update), [Delete](#delete).

### <a name="select">Select</a>

The `Select` method builds a query using a fluent-interface. You can then return a single object using `SingleOrDefault()`, or a list of matching objects using `ToList()`. The query can be fine-tuned using a number of fluent methods: `OrderBy()`, `OrderByDesc()`, and `Limit()`.

Example usage:

```csharp
public class Person
{
    public int Id { get; set; }
    public string FirstName { get; set; }
	public char MiddleInitial { get; set; }
	public string LastName { get; set; }
	public char Gender { get; set; }
	public DateTime CreateDate { get; set; }
	public DateTime? UpdateDate { get; set; }
}

var db = new Norm.Database(connString);

// select all
IEnumerable<Person> people = db.Select<Person>().ToList();

// select one
Person person = db.Select<Person>(p => p.Id == 1).SingleOrDefault();

// use of OrderBy and Limit
IEnumerable<Person> people = db.Select<Person>(p => p.LastName == "Smith")
                               .OrderBy(p => p.FirstName)
                               .Limit(5)
                               .ToList();
```

### <a name="insert">Insert</a>

The `Insert` method inserts the object `obj`, and returns the `Id` of the newly inserted object.

Example usage:

```csharp
Person person = new Person();
person.FirstName = "John";
person.MiddleInitial = 'Q';
person.LastName = "Smith";
person.Gender = 'M';

person.Id = db.Insert(p);
```

### <a name="update">Update</a>

The `Update` method updates an existing object `obj`, and returns `true` if the object was updated successfully, otherwise `false`.

Example usage:

```csharp
Person person = db.Select<Person>(p => p.Id == 1).SingleOrDefault();
person.FirstName = "Jane";
person.MiddleInitial = null;
person.LastName = "Doe";
person.Gender = 'F';

db.Update(person);
```

### <a name="delete">Delete</a>

The `Delete` method deletes an existing object `obj`, and returns `true` if the object was deleted successfully, otherwise `false`.

Example usage:

```csharp
Person person = db.Select<Person>(p => p.Id == 1).SingleOrDefault();
db.Delete(person);
```

<a name="naming-conventions"></a>Assumptions & Naming Conventions
--------------------------------

- Norm assumes that your object and property names map directly to your table and column names.
- Norm uses the following naming conventions _(note that name-matching is **not** case sensitive)_:
  - The primary key field (required) is either `Id` or `<TypeName>Id`.
  - The created timestamp (optional) is either `Created`, `CreateDate`, or `CreatedOn`. This field will automatically be set when a record is inserted.
  - The updated timestamp (optional) is either `Updated`, `UpdateDate`, or `UpdatedOn`. This field will automatically be set when a record is updated.
