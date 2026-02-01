# DatReaderWriter.Extensions

DatReaderWriter.Extensions

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Basic Usage](#basic-usage)
- [Contributing](#contributing)
- [License](#license)

## Features
- Targets `net8.0`

## Installation

Install the `Chorizite.DatReaderWriter.Extensions` package from NuGet:

```bash
dotnet add package Chorizite.DatReaderWriter.Extensions
```

## DatEasyWriter

### Initializing DatEasyWriter

You can initialize `DatEasyWriter` pointing to a directory containing your portal/cell/local.dat files, or use an existing `DatCollection`.

```csharp
using DatReaderWriter.Extensions;
using DatReaderWriter.Options;

// From directory
using var writer = new DatEasyWriter("C:\\Turbine\\Asheron's Call");

// With options (e.g. auto-increment iterations)
var options = new DatEasyWriterOptions { IncreaseIterations = true };
using var writerWithOptions = new DatEasyWriter("C:\\Turbine\\Asheron's Call", options);
```

### Managing Titles

`DatEasyWriter` provides high-level helpers for managing character titles (adding, updating, removing). These methods handle the underlying `EnumMapper` and `StringTable` updates for you.

```csharp
// Add a new title (Enum ID is auto-generated)
var newTitleIdResult = writer.AddTitle("My Legendary Title");
if (newTitleIdResult.Success) {
    Console.WriteLine($"Added title with ID: {newTitleIdResult.Value}");
}

// Add a title with a specific Enum ID
writer.AddTitle("Another Title", "ID_CharacterTitle_AnotherTitle");

// Update a title by ID
writer.UpdateTitle(newTitleIdResult.Value, "My Updated Legendary Title");

// Update a title by string match
writer.UpdateTitle("My Updated Legendary Title", "My Final Title");

// Remove a title by ID
writer.RemoveTitle(newTitleIdResult.Value);

// Remove a title by string
writer.RemoveTitle("My Final Title");
```

### Generic Database Operations

You can easily get and save `DBObj` files using the generic helpers.

```csharp
// Get a file (e.g. a LandBlock 0xFFFF0000)
var result = writer.Get<LandBlock>(0xFFFF0000);
if (result.Success) {
    var landBlock = result.Value;
    // Modify landBlock...
    
    // Save it back. If IncreaseIterations is true, the iteration count will update.
    writer.Save(landBlock);
}
```


### Accessing Mappers and String Tables

- **`DatEasyWriter.GetEnumMapper(EnumMapperType)`**: Helper to fetch a specific EnumMapper.
- **`DatEasyWriter.GetStringTable(StringTableType)`**: Helper to fetch a specific StringTable.

## Extensions

### RenderSurface Extensions

Extensions for `RenderSurface` (textures) to easily replace or export image data.

```csharp
using DatReaderWriter.Extensions.DBObjs;

// Replace a texture with a PNG/BMP/etc on disk
var renderSurface = writer.Get<RenderSurface>(0x06001234).Value;

// Replace and optionally resize to match original dimensions
renderSurface.ReplaceWith("path/to/new_texture.png", shouldResize: true);
writer.Save(renderSurface);

// Export a texture to a file
renderSurface.SaveToImageFile("path/to/extracted_texture.png", writer);

// Get raw RGBA bytes
byte[] rgbaBytes = renderSurface.ToRgba8(writer);
```

### Other Extensions

- **`DBObj.GetDatFileType()`**: Returns the `DatFileType` (Portal, Cell, Local) a generic `DBObj` belongs to.
- **`string.ComputeHash()`**: Computes the Asheron's Call specific hash of a string (useful for StringTables).


## Contributing

We welcome contributions from the community! If you would like to contribute to DatReaderWriter, please follow these steps:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature-branch`).
3. Make your changes.
4. Commit your changes (`git commit -am 'Add some feature'`).
5. Push to the branch (`git push origin feature-branch`).
6. Create a new Pull Request.

## License

This project is licensed under the MIT License. See the LICENSE.txt file for details.
