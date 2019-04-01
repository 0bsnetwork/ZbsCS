# ZbsCS
A C# library for interacting with the 0bsNetwork blockchain

Supports node interaction, offline transaction signing, Matcher orders, and creating addresses and keys.


## Getting Started
Add a Reference
```
using ZbsCS;
```

For installation NuGet package from VS Package Manager Console you should use:
```
PM> Install-Package ZbsNetwork.ZbsCS -Version 1.1.0
```

For installation via UI Package Manager use this [instruction](https://docs.microsoft.com/en-us/nuget/tools/package-manager-ui).

Target framework .NET Framework 4.5.1
## Documentation

The library utilizes classes to represent various Zbs data structures and encoding and serialization methods:

- ZbsCS.Node
- ZbsCS.Order
- ZbsCS.OrderBook
- ZbsCS.PrivateKeyAccount
- ZbsCS.Transaction
- ZbsCS.AddressEncoding
- ZbsCS.Base58
- ZbsCS.Utils


#### Code Example
Code examples are in [ZbsCSTests](https://github.com/0bsnetwork/ZbsCS/tree/master/ZbsCSTests) project and [Examples](Examples.md) page.

### Source code
[ZbsCS Github repository](https://github.com/0bsnetwork/ZbsCS
