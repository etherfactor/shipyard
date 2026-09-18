using System.Text.Json;

namespace EtherGizmos.Shipyard;

public static class JsonShapeAssert
{
    public static void HasOnlyProps(JsonElement obj, params string[] expected)
    {
        var actual = obj.EnumerateObject().Select(p => p.Name).Where(x => x != "@odata.context").OrderBy(x => x).ToArray();
        var exp = expected.OrderBy(x => x).ToArray();
        Assert.That(actual, Is.EquivalentTo(exp));
    }

    public static void HasProps(JsonElement obj, params string[] expected)
    {
        foreach (var name in expected)
            Assert.That(obj.TryGetProperty(name, out _), Is.True, $"Missing property '{name}'.");
    }

    public static void HasNoProps(JsonElement obj, params string[] notExpected)
    {
        foreach (var name in notExpected)
            Assert.That(obj.TryGetProperty(name, out _), Is.False, $"Unexpected property '{name}'.");
    }
}
