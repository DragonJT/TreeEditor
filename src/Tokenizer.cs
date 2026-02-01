
enum TokenKind
{
    Identifier,
    Keyword,
    Number,
    Operator,
    Parentheses,
    Squares,
    String,
    Comma,
    Error,
}

class Token(TokenKind kind, string value)
{
    public TokenKind kind = kind;
    public string value = value;
}

class Tokenizer(string code)
{
    readonly string code = code;
    int index = 0;
    readonly HashSet<string> operators = ["+", "-", "&", "|", "=", "<", ">", "<=", ">=", "&&", "||", "==", "++", "--"];
    readonly HashSet<string> keywords = ["static", "class", "singleton", "if", "this", "var", "return"];

    static bool IsDigit(char c)
    {
        return c>='0' && c<='9';
    }

    static bool IsCharacter(char c)
    {
        return (c>='a' && c<='z') || (c>='A' && c<='Z') || c=='_';
    }

    static bool IsAlphaNumeric(char c)
    {
        return IsCharacter(c) || IsDigit(c);
    }

    Token ReadOpenClose(char open, char close, TokenKind kind)
    {
        List<char> chars = [];
        index++;
        int depth = 1;
        while (true)
        {
            if(index >= code.Length)
            {
                return new Token(TokenKind.Error, new string([..chars]));
            }
            var c = code[index];
            if(c == open)
            {
                depth++;
            }
            else if(c == close)
            {
                depth--;
                if(depth <= 0)
                {
                    index++;
                    return new Token(kind, new string([..chars]));
                }
            }
            index++;
            chars.Add(c);
        }
    }

    public List<Token> GetTokens()
    {
        List<Token> tokens = [];
        while (true)
        {
            if(index >= code.Length)
            {
                return tokens;
            }
            var c = code[index];
            if (IsCharacter(c))
            {
                List<char> chars = [c];
                index++;
                while (true)
                {
                    if(index >= code.Length)
                    {
                        break;
                    }
                    c = code[index];
                    if (IsAlphaNumeric(c))
                    {
                        chars.Add(c);
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }
                var identifier = new string([..chars]);
                if (keywords.Contains(identifier))
                {
                    tokens.Add(new(TokenKind.Keyword, identifier));
                }
                else
                {
                    tokens.Add(new (TokenKind.Identifier, identifier));
                }
            }
            else if (c == ' ')
            {
                index++;
            }
            else if(IsDigit(c))
            {
                List<char> chars = [c];
                index++;
                while (true)
                {
                    if(index >= code.Length)
                    {
                        break;
                    }
                    c = code[index];
                    if(float.TryParse(new string([..chars, c]), out float result))
                    {
                        chars.Add(c);
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }
                tokens.Add(new (TokenKind.Number, new string([..chars])));
            }
            else if(c == '"')
            {
                List<char> chars = [c];
                index++;
                while (true)
                {
                    if(index >= code.Length)
                    {
                        break;
                    }
                    c = code[index];
                    if(c == '"' && code[index-1] != '\\')
                    {
                        chars.Add(c);
                        index++;
                        break;
                    }
                    else
                    {
                        chars.Add(c);
                        index++;
                    }
                }
                tokens.Add(new(TokenKind.String, new string([..chars])));
            }
            else if(operators.Contains(c.ToString()))
            {
                List<char> chars = [c];
                index++;
                while (true)
                {
                    if(index >= code.Length)
                    {
                        break;
                    }
                    c = code[index];
                    if(operators.Contains(new string([..chars, c])))
                    {
                        chars.Add(c);
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }
                tokens.Add(new (TokenKind.Operator, new string([..chars])));
            }
            else if (c == ',')
            {
                tokens.Add(new (TokenKind.Comma, ","));
                index++;
            }
            else if(c == '(')
            {
                tokens.Add(ReadOpenClose('(', ')', TokenKind.Parentheses));
            }
            else if(c == '[')
            {
                tokens.Add(ReadOpenClose('[', ']', TokenKind.Squares));
            }
            else
            {
                tokens.Add(new(TokenKind.Error, new string(c.ToString())));
                index++;
            }
        }
    }

}