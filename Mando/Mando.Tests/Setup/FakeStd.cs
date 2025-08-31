namespace Mando.Tests.Setup;

public sealed class FakeStd : IStd
{
    public List<string> Out { get; } = [];

    public void Write(string value) => Out.Add(value);
}