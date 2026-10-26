namespace CoroutineInteractions;

public class InteractorCoroutines
{
    const int Eof = -1;

    string value = string.Empty;
    public readonly List<Token> tokens = [];

    public static IEnumerable<int> Decompressor(TextReader input)
    {
        int c, len;

        while (true)
        {
            c = input.Read();
            if (c == Eof)
            {
                break;
            }

            if (c == 0xFF)
            {
                len = input.Read();
                c = input.Read();
                while (len-- > 0)
                {
                    yield return c;
                }
            }
            else
            {
                yield return c;
            }
        }

        yield return c;
    }

    public IEnumerable<object?> Parser()
    {
        var args = Coroutine.Args(new { c = Eof });
        int c = Eof;

        while (true)
        {
            // first char already in c
            if (Coroutine.Args(ref args)?.c == Eof)
            {
                break;
            }

            if (Coroutine.Args(ref args)?.c is int i && char.IsAsciiLetter((char)(c = i)))
            {
                do
                {
                    AddToToken(c);
                    yield return null;
                }
                while (Coroutine.Args(ref args)?.c is int j && char.IsAsciiLetter((char)(c = j)));
                GotToken(TokenType.Word);
            }

            AddToToken(c);
            GotToken(TokenType.Punct);
            yield return null;
        }
    }

    private void AddToToken(int c)
    {
        value += (char)c;
    }

    private void GotToken(TokenType type)
    {
        tokens.Add(new(type, value));
        value = string.Empty;
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
