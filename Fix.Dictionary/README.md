# Fix.Dictionary



```csharp
// Enumerate The FIX Versions Supported By The Dictionary
foreach (var version in Versions)
{
    Console.WriteLine($"BeginString = {version.BeginString}");
    Console.WriteLine($"Messages = {version.Messages.Length}");
    Console.WriteLine($"Maximum Field Tag = {version.Fields.MaxTag}");
    Console.WriteLine($"DataTypes = {version.DataTypes.Count}");
}
```

```csharp
// Reference a specific FIX vrsion directly
var version = Versions.FIX_5_0SP2;
Console.WriteLine($"BeginString = {version.BeginString}");
Console.WriteLine($"Messages = {version.Messages.Length}");
Console.WriteLine($"Maximum Field Tag = {version.Fields.MaxTag}");
Console.WriteLine($"DataTypes = {version.DataTypes.Count}");
```

```csharp
// Lookup A Specific FIX Version By BeginString
if (Versions["FIX.5.0SP2"] is Fix.Dictionary.Version version)
{
    Console.WriteLine($"BeginString = {version.BeginString}");
    Console.WriteLine($"Messages = {version.Messages.Length}");
    Console.WriteLine($"Maximum Field Tag = {version.Fields.MaxTag}");
    Console.WriteLine($"DataTypes = {version.DataTypes.Count}");
}
```

```csharp
// Enumerate The Messages In A FIX Version
foreach (var message in Versions.FIX_5_0SP2.Messages)
{
    Console.WriteLine($"MsgType = {message.MsgType}");
    Console.WriteLine($"Name = {message.Name}");
    Console.WriteLine($"Pedigree = ({message.Pedigree})");
    Console.WriteLine($"Description = {message.Description}");
}
```

```csharp
// Lookup A Message By MsgType
if (Versions.FIX_5_0SP2.Messages["D"] is Message message)
{
    Console.WriteLine($"MsgType = {message.MsgType}");
    Console.WriteLine($"Name = {message.Name}");
    Console.WriteLine($"Pedigree = ({message.Pedigree})");
    Console.WriteLine($"Description = {message.Description}");
}
```

```csharp
// Enumerate The Fields In A FIX Version
foreach (var field in Versions.FIX_5_0SP2.Fields.Where(field => field.IsValid))
{
    Console.WriteLine($"Tag = {field.Tag}");
    Console.WriteLine($"Name = {field.Name}");
    Console.WriteLine($"DataType = {field.DataType}");
    Console.WriteLine($"Pedigree = ({field.Pedigree})");
    Console.WriteLine($"Description = {field.Description}");
}
```
```csharp
// Lookup A Field By Tag
if (Versions.FIX_5_0SP2.Fields[38] is VersionField field && field.IsValid)
{
    Console.WriteLine($"Tag = {field.Tag}");
    Console.WriteLine($"Name = {field.Name}");
    Console.WriteLine($"DataType = {field.DataType}");
    Console.WriteLine($"Pedigree = ({field.Pedigree})");
    Console.WriteLine($"Description = {field.Description}");
}
```
```csharp
// Enumerate The DataTypes In A FIX Version
foreach (var dataType in Versions.FIX_5_0SP2.DataTypes)
{
    Console.WriteLine($"Name = {dataType.Name}");
    Console.WriteLine($"Pedigree = {dataType.Pedigree}");
    Console.WriteLine($"Description = {dataType.Description}");
}
```
```csharp
// Enumerate The Fields In A Message
if (Versions.FIX_5_0SP2.Messages["D"] is Message message)
{
    foreach (var field in message.Fields)
    {
        Console.WriteLine($"Tag = {field.Tag}");
        Console.WriteLine($"Name = {field.Name}");
        Console.WriteLine($"Required = {field.Required}");
        Console.WriteLine($"Depth = {field.Depth}");
        Console.WriteLine($"Description = {field.Description}");
    }
}
```
```csharp 
// Enumerate The Values Of A Field
if (Versions.FIX_5_0SP2.Fields[54] is VersionField side)
{
    foreach (var value in side.Values.Values)
    {
        Console.WriteLine($"Value = {value.Value}");
        Console.WriteLine($"Name = {value.Name}");
        Console.WriteLine($"Pedigree = ({value.Pedigree})");
        Console.WriteLine($"Description = ({value.Description})");
    }
}
```
```csharp
// Lookup A Field Value
if (Versions.FIX_5_0SP2.Fields[54] is VersionField side)
{
    if (side.Values.TryGetValue("2", out var sell))
    {
        Console.WriteLine($"Value = {sell.Value}");
        Console.WriteLine($"Name = {sell.Name}");
        Console.WriteLine($"Pedigree = ({sell.Pedigree})");
        Console.WriteLine($"Description = ({sell.Description})");
    }
}
```
```csharp
// Directly Reference A Field Value
var sell = FIX_5_0SP2.Side.Sell;
Console.WriteLine($"Value = {sell.Value}");
Console.WriteLine($"Name = {sell.Name}");
Console.WriteLine($"Pedigree = ({sell.Pedigree})");
Console.WriteLine($"Description = ({sell.Description})");
```