Norm - (Yet Another) .Net object-relational mapper
==================================================

Overview
-----------

Norm is an opinionated object-relational mapper (ORM). It relies on a specific set of [naming conventions](#naming-conventions) for your objects. Yes, strict adherence to an arbitrary set of naming conventions limits the range of use cases that Norm can handle, but I was scratching my own itch, and wanted a simple ORM for smaller projects that does only what I need it to, and doesn't require a bunch of setup to get started.

Methods
----------

Norm includes 4 extension methods on the `IDbConnection` class: Select<T>, Insert, Update, Delete.

### Select<T>

The `Select` method returns a mapped list of `T` objects matching the given `parameters`.

```csharp
public static IEnumerable<T> Select<T>(this IDbConnection connection, object parameters = null)
```

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

// select all
IEnumerable<Person> people = connection.Select<Person>();

// select one
Person person = connection.Select<Person>(new { Id = 1 }).SingleOrDefault();
```

### Insert

The `Insert` method inserts the object `obj`, and returns the `Id` of the newly inserted object.

```csharp
public static int Insert(this IDbConnection connection, object obj)
```

Example usage:

```csharp
Person person = new Person();
person.FirstName = "John";
person.MiddleInitial = 'Q';
person.LastName = "Smith";
person.Gender = 'M';

person.Id = connection.Insert(p);
```

### Update

The `Update` method updates an existing object `obj`, and returns `true` if the object was updated successfully, otherwise `false`.

```csharp
public static bool Update(this IDbConnection connection, object obj)
```

Example usage:

```csharp
Person person = connection.Select<Person>(new { Id = 1 }).SingleOrDefault();
person.FirstName = "Jane";
person.MiddleInitial = null;
person.LastName = "Doe";
person.Gender = 'F';

connection.Update(person);
```

### Delete

The `Delete` method deletes an existing object `obj`, and returns `true` if the object was deleted successfully, otherwise `false`.

```csharp
public static bool Delete(this IDbConnection connection, object obj)
```

Example usage:

```csharp
Person person = connection.Select<Person>(new { Id = 1 }).SingleOrDefault();
connection.Delete(person);
```

<a name="naming-conventions"></a>Assumptions & Naming Conventions
--------------------------------

- Norm assumes that the `IDbConnection` is already open.
- Norm assumes that your object and property names map directly to your table and column names.
- Norm uses the following naming conventions _(note that name-matching is **not** case sensitive)_:
  - The primary key field (required) is either `Id` or `<TypeName>Id`.
  - The created timestamp (optional) is either `created`, `createdate`, or `createdon`. This field will automatically be set when a record is inserted.
  - The updated timestamp (optional) is either `updated`, `updatedate`, or `updatedon`. This field will automatically be set when a record is updated.
