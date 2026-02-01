using DatReaderWriter;
using DatReaderWriter.DBObjs;
using DatReaderWriter.Enums;
using DatReaderWriter.Extensions.Tests.Helpers;
using DatReaderWriter.Options;
using DatReaderWriter.Types;

namespace DatReaderWriter.Extensions.Tests;

[TestClass]
public class DatEasyWriterTitlesTests {
    [TestMethod]
    public void CanAddTitle() {
        using var tempDat = new TempDatDirectory();
        using var writer = new DatEasyWriter(tempDat.DirectoryPath);

        // First, we need to set up the required files for title management
        // This includes EnumMapper and StringTable
        SetupTitlePrerequisites(writer);

        var result = writer.AddTitle("Test Title");

        Assert.IsTrue(result.Success, result.Error ?? "");
        Assert.IsTrue(result.Value > 0);
    }

    [TestMethod]
    public void CanAddAndUpdateTitle() {
        using var tempDat = new TempDatDirectory();
        using var writer = new DatEasyWriter(tempDat.DirectoryPath);

        SetupTitlePrerequisites(writer);

        var addResult = writer.AddTitle("Original Title");
        Assert.IsTrue(addResult.Success, addResult.Error ?? "");

        var updateResult = writer.UpdateTitle("Original Title", "Updated Title");
        Assert.IsTrue(updateResult.Success, updateResult.Error ?? "");
    }

    [TestMethod]
    public void CanAddAndRemoveTitle() {
        using var tempDat = new TempDatDirectory();
        using var writer = new DatEasyWriter(tempDat.DirectoryPath);

        SetupTitlePrerequisites(writer);

        var addResult = writer.AddTitle("Title To Remove");
        Assert.IsTrue(addResult.Success, addResult.Error ?? "");

        var removeResult = writer.RemoveTitle("Title To Remove");
        Assert.IsTrue(removeResult.Success, removeResult.Error ?? "");
    }

    /// <summary>
    /// Sets up the prerequisite files for title operations.
    /// Title operations require EnumMapper and StringTable files.
    /// </summary>
    private void SetupTitlePrerequisites(DatEasyWriter writer) {
        // Create the CharacterTitle EnumMapper (0x22000041)
        var enumMapper = new EnumMapper { Id = 0x22000041, BaseEnumMap = 0, IdToStringMap = [] };
        writer.Dats.TryWriteFile(enumMapper);

        // Create the StringTable for titles (0x2300000E)
        var stringTable = new StringTable { Id = 0x2300000E, Language = 1, Strings = [] };
        writer.Dats.TryWriteFile(stringTable);
    }
}
