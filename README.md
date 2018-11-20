# Stringly.Typed

> Quick, painless conversion of .NET Types <> Strings.

## The Problem

`String` is everywhere - often being a top-level input to most pieces of code.

- User input.
- JSON data (e.g. configuration files).
- QueryString values.
- Database records/documents.
- Console input.

We then take this `string`, and "pass it on" to service layers, which then do their own type-checking/validation, but (for some reason) end up passing the `string` on to other systems. The "validation" logic is duplicated, refactoring is hard, bugs and "WTF's" ensue.

**This is referred to as being "[Stringly Typed](https://blog.codinghorror.com/new-programming-jargon/)"**:

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
    int result = -1;
    if (!int.TryParse(recordId, out result))
        throw new ArgumentException(nameof(recordId));

    // Now we can get to code the business cares about...
}
```

Our lives would have been so much easier if we had just written:

```cs
public Data GetData(int recordId) {
    // Living the good life! :)
}
```

We see this ALL the time, particularly with things like `int`, `Guid`, `MailAddress`, Postal/ZIP Codes, anything-that-is-a-thing-that-can't-be-empty, and so on.

**It's really just a specific form of ["Primitive Obsession"](https://sourcemaking.com/refactoring/smells/primitive-obsession).**

**You're losing the power (and safety) of a typed system. Validation logic is duplicated, and you get FUGLY code.**

Enter `Stringly.Typed`.

****

## The Solution

> If we can _easily_ define what a "valid" `string` for our type, we can really clean up our codebase.

`Stringly.Typed` made of a few parts:

1. Support for `Stringly<T>`, working with anything that has a `TryParse` method (e.g. primitives, `Guid`, `DateTime`, `IPAddress` - [knock yourself out](https://www.google.co.uk/search?q=.net+tryparse+site%3Ahttps%3A%2F%2Fmsdn.microsoft.com%2Fen-us)).
1. Support for `Stringly<Uri>`, parsing absolute `Uri`'s (e.g. from configuration files).
1. A generic type (`Stringly<T>`) that implements `implicit` operators so you can seamlessly move to/from `String`.
1. A base class that enables quick `Regex` matching.
1. A non-generic `Stringly` base class to make it easy to define "just a string that conforms to a specific format".

## Getting Started

> **Note:** You can see NUnit tests demonstrating features in the "[Samples](https://github.com/mission202/Stringly.Typed/blob/master/tests/StringlyTyped.Tests/Samples.cs)".

### Simple Conversion via `TryParse`

Let's say we have a service method that actually wants a `Int32`:

```cs
public void Method(Stringly<int> id) {
    // No validation/guard clauses required.
    // Repository expects (and gets) a Int32.
    var data = respository.Get(id);
}

// Meanwhile...
service.Method("123"); // Works
service.Method(123); // Works
service.Method("im-not-a-int"); // throws ArgumentOutOfRangeException
```

### (Absolute) URI Parsing

```cs
public void OutputHost(Stringly<Uri> uri) {
    Console.WriteLine(uri.Result.Host); // 'Result' is a Uri
}

// Both of these naturally work - outputting 'nyan.cat'
OutputHost("http://nyan.cat");
OutputHost(new Uri("http://nyan.cat"));

OutputHost("nyan"); // ArgumentOutOfRangeException - Not an absolute URI.
```

### Simple Regular Expression Matching

Your company has customer identifiers in the format "`AA1234`"? No problem!

Inherit from the `Stringly` base class and provide our regular expression. When a `string` is passed in, they will automagically get validated for you!

```cs
public class CustomerIdentifier : Stringly
{
    protected override Regex Regex => new Regex(@"^[a-zA-Z]{2}\d{4}$");
}

public void Method(Stringly<CustomerIdentifier> Id){ /* ... */ }

x.Method("AA1234"); // Works
x.Method(new Stringly<CustomerIdentifier>("AA1234")) // Works
x.Method("1234"); // ArgumentOutOfRangeException
```

### Complex Types

What if our database uses a composite key (such as [Table Storage](https://azure.microsoft.com/en-gb/services/storage/tables/))?
Rather than litter our codebase with `string partitionKey, string rowKey` (I've seen this!) - we can quickly define a complex type matched by regular expression:

```cs
public class TableStorageKey : StringlyPattern<TableStorageKey>
{
    public string PartitionKey { get; protected set; }
    public string RowKey { get; protected set; }

    protected override Regex Regex => new Regex(@"^(?<partitionKey>\w+):(?<rowKey>\w+)$");

    protected override TableStorageKey ParseFromRegex(Match match)
    {
        // Regular expression has already been matched for us.
        return new TableStorageKey
        {
            PartitionKey = match.Groups["partitionKey"].Value,
            RowKey = match.Groups["rowKey"].Value
        };
    }

    // Enable implicit conversion
    public static implicit operator TableStorageKey(string value)
    {
        // Helper method from the StringlyPattern base class.
        return Parse(value);
    }
}

// My caller (e.g. Controller, Worker Role etc) now doesn't have to work hard to parse strings.
public void Method(TableStorageKey key) {
    // Do stuff with the key..
    DoALookup(key.PartitionKey, key.RowKey);
}

// No problem! I can do that!
x.Method("pk:rk"); // Works
x.Method("usa:totalSearches"); // Works
x.Method("not-a-key"); // ArgumentOutOfRangeException
```

There you have it! Now do the right thing! Define custom types and only use the _concrete_ types in future, you've no excuse! :smile: