using System.Text;
using System.Threading.Channels;

namespace CoroutineInteractions.Tests;

public class InteractorTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("text", "Word:text,Punct:\uffff")]
    [InlineData("text ", "Word:text,Punct: ")]
    [InlineData("b\u00ff\u0002ef ", "Word:beef,Punct: ")]
    [InlineData("b\u00ff\u0002ef f\u00ff\u0002od gud", "Word:beef,Punct: ,Word:food,Punct: ,Word:gud,Punct:\uffff")]
    public async Task DecompressorConnectsToParserAsync(
        string inputText, string expectedTokens)
    {
        var byteArray = Encoding.UTF8.GetBytes(inputText);
        var inputStream = new MemoryStream(byteArray);
        using var input = new StreamReader(inputStream);
        var requestChannel = Channel.CreateUnbounded<InteractorInstruction>();
        Task? decompressor = null;
        var writer = new InteractorInstructionWriter(requestChannel.Writer);
        var interactor = new CoroutineInteractor(writer);
        var parser = Task.Run(async () =>
        {
            await InteractorCoroutines.ParserAsync(interactor);
            requestChannel.Writer.Complete();
        });
        ChannelWriter<int>? charWriter = null;
        List<InteractorCoroutines.Token> tokens = [];
        var sut = requestChannel.Reader.ReadAllAsync().GetAsyncEnumerator();

        await foreach (var instruction in requestChannel.Reader.ReadAllAsync())
        {
            instruction.Switch(
                yieldDecompressor =>
                {
                    charWriter?.TryWrite(yieldDecompressor.Request);
                    charWriter?.Complete();
                    charWriter = null;
                },
                yieldParser =>
                {
                    decompressor ??= InteractorCoroutines.DecompressorAsync(
                        interactor);
                    charWriter = yieldParser.Responses.Writer;
                    if (decompressor.IsCompleted)
                    {
                        charWriter.TryWrite(InteractorCoroutines.Eof);
                        charWriter.Complete();
                        charWriter = null;
                    }
                },
                read =>
                {
                    var response = input.Read();
                    var writer = read.Responses.Writer;
                    writer.TryWrite(response);
                    writer.Complete();
                },
                write =>
                {
                    tokens.Add(write.Request);
                });
        }

        if (decompressor is not null)
        {
            await decompressor;
        }

        await parser;

        var expected = expectedTokens
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t =>
            {
                var typeAndValue = t.Split(":");
                return new InteractorCoroutines.Token(
                    Enum.Parse<InteractorCoroutines.TokenType>(
                        typeAndValue[0]),
                    typeAndValue[1]);
            });
        Assert.Equal(expected, tokens);
    }
}
