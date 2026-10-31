namespace CoroutineInteractions;

public class InteractorCoroutines
{
    public const int Eof = -1;

    public static async Task DecompressorAsync(IInteractor interactor)
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
                    await interactor.YieldDecompressorAsync(c);
                }
            }
            else
            {
                await interactor.YieldDecompressorAsync(c);
            }
        }

        await interactor.YieldDecompressorAsync(c);
    }

    public static async Task ParserAsync(IInteractor interactor)
    {
        string value = string.Empty;
        List<Token> tokens = [];

        var c = await interactor.YieldParserAsync();

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
                    c = await interactor.YieldParserAsync();
                }
                while (char.IsAsciiLetter((char)c));
                await GotTokenAsync(TokenType.Word);
            }

            AddToToken(c);
            await GotTokenAsync(TokenType.Punct);
            c = await interactor.YieldParserAsync();
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
