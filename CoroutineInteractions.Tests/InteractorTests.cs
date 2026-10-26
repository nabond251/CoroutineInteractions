using System.Text;
using static CoroutineInteractions.InteractorCoroutines;

namespace CoroutineInteractions.Tests;

public class InteractorTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("text", "Word:text,Punct:\uffff")]
    [InlineData("text ", "Word:text,Punct: ")]
    [InlineData("b\u00ff\u0002ef ", "Word:beef,Punct: ")]
    [InlineData("b\u00ff\u0002ef f\u00ff\u0002od gud", "Word:beef,Punct: ,Word:food,Punct: ,Word:gud,Punct:\uffff")]
    public void CanConnectDecompressorToParser(
        string inputText, string expectedTokens)
    {
        var sut = new InteractorCoroutines();

        var byteArray = Encoding.UTF8.GetBytes(inputText);
        var inputStream = new MemoryStream(byteArray);
        using var input = new StreamReader(inputStream);
        using var decompressor = Coroutine.Create(Decompressor(input));
        using var parser = Coroutine.Create(sut.Parser());

        while (decompressor.Resume())
        {
            parser.Resume(new { c = decompressor.Result.ReturnValue });
        }

        var expected = expectedTokens
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t =>
            {
                var typeAndValue = t.Split(":");
                return new Token(
                    Enum.Parse<TokenType>(typeAndValue[0]),
                    typeAndValue[1]);
            });
        Assert.Equal(expected, sut.tokens);
    }
}
