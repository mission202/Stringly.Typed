# Stringly.Typed

Making it easier to convert to/from .NET types to strings.

## The problem

`String` is everywhere - often being a top-level input to most pieces of code.

- User input.
- JSON data.
- QueryString values.
- Database records/documents.

We then take this `string`, and "pass it on" to service layers, which then do their own type-checking/validation, but (for some reason) end up passing the `string` on to other systems. The "validation" logic is duplicated, refactoring is hard, bugs and "WTF's" ensue.

This is referred to as being "[Stringly Typed](https://blog.codinghorror.com/new-programming-jargon/)":

> A riff on strongly typed. Used to describe an implementation that needlessly relies on strings when programmer & refactor friendly options are available.
> 
> For example:
>
> * Method parameters that take strings when other more appropriate types should be used.
> * On the occasion that a string is required in a method call (e.g. network service), the string is then passed and used throughout the rest of the call graph without first converting it to a more suitable internal representation (e.g. parse it and create an enum, then you have strong typing throughout the rest of your codebase).
> * Message passing without using typed messages etc.

For example:

```cs
public Data GetData(string recordId) {
    var result = Guid.Empty;
    if (!Guid.TryParse(recordId, out result))
        throw new ArgumentException(nameof(recordId));

    // Now we have a GUID that our Repository actually needs..
}
```

It turns out here that our "record ID" is actually a `Guid`, but we were lazy while putting together our interfaces.

**It's really just a specific form of ["Primitive Obsession"](https://sourcemaking.com/refactoring/smells/primitive-obsession)**, which over the years I've found to be a _prolific_ problem. **Literally _every_ codebase I've worked on has had this smell.**

We should make it _easy_ to "do the right thing".

Enter `Stringly.Typed`.

****

## The solution

> **If we can easily define what a "valid" `string` for our datatype looks like, and quickly convert to the "concrete", then we can really clean up our codebase.**

`Stringly.Typed` made of a few parts:

1. A generic type (`Stringly<T>`) that implements `implicit` operators.
2. Base classes that throw `Regex` matching into the mix.
3. A fallback mechanism to use `TryParse` (via reflection) to work with primitives (or anything) out-of-the-box.
4. A non-generic `Stringly` base class to make it easy to define "just a string that conforms to a specific format".

## Getting Started

> **Note:** You can see NUnit tests demonstrating features in the "[Samples](https://github.com/mission202/Stringly.Typed/blob/proto/tests/StringlyTyped.Tests/Samples.cs)".

### Simple Primitive Conversion

Let's say we have a service method that actually wants a `Guid`:

```cs
public void Method(Stringly<Guid> guid) {
    // No validation/guard clauses required.
    // Repository expects (and gets) a GUID.
    var data = respository.Get(guid);
}

// Meanwhile...
// This works!
service.Method("71c03645-08c6-4c12-8336-a5784a286fac");

// Naturally, so does this!
service.Method(new Guid("71c03645-08c6-4c12-8336-a5784a286fac"));

// But this doesn't! (throws ArgumentOutOfRangeException)
service.Method("im-not-a-guid");
```

### Simple Regular Expression Matching

What if your database actually stores customer identifiers in the format "`AA1234`"? No problem!

We simply inherit from the `Stringly` base class and provide our regular expression. When a `string` is passed in, they will automagically get validated for you!

```cs
public class CustomerId : Stringly
{
    protected override Regex Regex => new Regex(@"^[a-zA-Z]{2}\d{4}$");
}

// Let's define method that wants to work with "Stringly" values:
public void Method(Stringly<CustomerId> Id){ /* ... */ }


service.Method("AA1234"); // Works
service.Method(new Stringly<RecordId>("AA1234")) // Works
service.Method("1234AA"); // ArgumentOutOfRangeException
```

### Complex Types

What if our database uses a composite key (such as [Table Storage](https://azure.microsoft.com/en-gb/services/storage/tables/))? Rather than litter our codebase with `string partitionKey, string rowKey` (I've seen this!) - we can quickly define a type using `Stringly.Typed` and the ease of regular expression matching:

```cs

// Define a type to be built from a regular expression.
public class TableStorageKey : StringlyPattern<TableStorageKey>
{
    public string PartitionKey { get; protected set; }
    public string RowKey { get; protected set; }

    protected override Regex Regex => new Regex(@"^(?<partitionKey>\w+):(?<rowKey>\w+)$");

    protected override TableStorageKey ParseFromRegex(Match match)
    {
        // Regular expression has already been matched for us.
        // The string data is "good to go" here.
        return new TableStorageKey
        {
            PartitionKey = match.Groups["partitionKey"].Value,
            RowKey = match.Groups["rowKey"].Value
        };
    }
}

// As before, but I don't want to think about Stringly values.
// My caller (e.g. Controller, Worker Role etc) should give me
// what I need!
public void Method(TableStorageKey key) {
    // Do stuff with the key..
    DoALookup(key.PartitionKey, key.RowKey);
}

// No problem! I can do that!
service.Method(new Stringly<TableStorageKey>("pk:rk")); // Works
service.Method(new Stringly<TableStorageKey>("usa:totalSearches")); // Works
service.Method(new Stringly<TableStorageKey>("not-a-key")); // ArgumentOutOfRangeException
```

****

There you have it! Now do the right thing! Define custom types and only use the _concrete_ types in future, you've no excuse! :smile:
