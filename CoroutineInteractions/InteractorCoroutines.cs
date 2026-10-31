using System.Threading.Channels;

namespace CoroutineInteractions;

public class InteractorCoroutines
{
    public const int Eof = -1;

    public static async Task DecompressorAsync(
        IInteractor interactor, ChannelWriter<int> output)
    {
        int c, len;

        while (true)
        {
            c = await interactor.ReadAsync();
            if (c == Eof)
            {
                break;
            }

            if (c == 0xFF)
            {
                len = await interactor.ReadAsync();
                c = await interactor.ReadAsync();
                while (len-- > 0)
                {
                    await output.WriteAsync(c);
                }
            }
            else
            {
                await output.WriteAsync(c);
            }
        }

        await output.WriteAsync(c);
    }

    public static async Task ParserAsync(
        IInteractor interactor, ChannelReader<int> input)
    {
        string value = string.Empty;
        List<Token> tokens = [];

        var c = await input.ReadAsync();

        while (true)
        {
            // first char already in c
            if (c == Eof)
            {
                break;
            }

            if (char.IsAsciiLetter((char)c))
            {
                do
                {
                    AddToToken(c);
                    c = await input.ReadAsync();
                }
                while (char.IsAsciiLetter((char)c));
                await GotTokenAsync(TokenType.Word);
            }

            AddToToken(c);
            await GotTokenAsync(TokenType.Punct);
            c = await input.ReadAsync();
        }

        void AddToToken(int c)
        {
            value += (char)c;
        }

        async Task GotTokenAsync(TokenType type)
        {
            await interactor.WriteAsync(new(type, value));
            value = string.Empty;
        }
    }

    public record class Token(
        TokenType Type,
        string Value);

    public enum TokenType
    {
        Word,
        Punct,
    }
}
