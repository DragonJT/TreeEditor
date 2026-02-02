

class Invalid(string text) : IExpression, IParameter
{
    public string text = text;

    public void Draw(ILayout layout)
    {
        layout.DrawInvalidText(text);
    }
    public string GetCode()
    {
        return text;
    }

    public string Type()
    {
        throw new Exception();
    }
}

class InvalidLine(string text) : ILineTree
{
    public string text = text;

    public void Draw(ILayout layout)
    {
        layout.DrawInvalidText(text);
    }
    public string GetCode()
    {
        return text;
    }
}

class Tokens
{
    readonly Token[] allTokens;
    readonly Token[] tokens;

    public int Count => tokens.Length;

    public Tokens(Token[] allTokens)
    {
        this.allTokens = allTokens;
        tokens = [.. allTokens.Where(t=>t.kind != TokenKind.Whitespace)];
    }

    public string GetCode()
    {
        string code = "";
        foreach(var t in allTokens)
        {
            code += t.value;
        }
        return code;
    }

    public Token this[int index]
    {
        get => tokens[index];
    } 

    public Tokens[] Split(TokenKind kind)
    {
        List<Token> splitTokens = [];
        List<Tokens> result = [];
        foreach(var t in allTokens)
        {
            if(t.kind == kind)
            {
                result.Add(new Tokens([.. splitTokens]));
                splitTokens.Clear();
            }
            else
            {
                splitTokens.Add(t);
            }
        }
        result.Add(new Tokens([.. splitTokens]));
        return [..result];
    }

    public Tokens GetTokensAfter(int id)
    {
        var token = tokens[id];
        var startIndex = allTokens.IndexOf(token);
        return new Tokens(allTokens[(startIndex+1)..]);
    }

    public bool IsEmpty()
    {
        return tokens.Length == 0;
    }
}

class Parser(Tokens tokens)
{
    readonly Tokens tokens = tokens;
    int index = 0;

    bool IsKeyword(string keyword)
    {
        if(index < tokens.Count && tokens[index].kind == TokenKind.Keyword && tokens[index].value == keyword)
        {
            index++;
            return true;
        }
        return false;
    }

    bool AtEnd()
    {
        return index >= tokens.Count;
    }

    bool IsEmpty()
    {
        return index >= tokens.Count;
    }

    bool IsParentheses(out string value)
    {
        if(index < tokens.Count && tokens[index].kind == TokenKind.Parentheses)
        {
            value = tokens[index].value;
            index++;
            return true;
        }
        value = null;
        return false;
    }

    bool IsIdentifier(out string value)
    {
        if(index < tokens.Count && tokens[index].kind == TokenKind.Identifier)
        {
            value = tokens[index].value;
            index++;
            return true;
        }
        value = null;
        return false;
    }

    public ILineTree Parse()
    {
        if (IsEmpty())
        {
            return new Empty();
        }
        else if (IsKeyword("class"))
        {
            if(IsIdentifier(out string name) && AtEnd())
            {
                return new ClassDecl(name);
            }
            return new InvalidLine(tokens.GetCode());
        }
        else if (IsKeyword("singleton"))
        {
            if(IsIdentifier(out string name) && AtEnd())
            {
                return new SingletonDecl(name);
            }
            return new InvalidLine(tokens.GetCode());
        }
        else
        {
            return new InvalidLine(tokens.GetCode());
        }
    }

    public IParameter ParseParameter()
    {
        if(IsIdentifier(out string type) && IsIdentifier(out string name) && AtEnd())
        {
            return new Parameter(type, name);
        }
        return new Invalid(tokens.GetCode());
    }

    public ILineTree ParseClassMember()
    {
        if (IsEmpty())
        {
            return new Empty();
        }
        else if(IsIdentifier(out string type))
        {
            if(IsIdentifier(out string name))
            {
                if (AtEnd())
                {
                    return new FieldDecl(type, name);
                }
                else if (IsParentheses(out string parameterCode) && AtEnd())
                {
                    var splitTokens = new Tokens([.. new Tokenizer(parameterCode[1..^1]).GetTokens()])
                        .Split(TokenKind.Comma);
                    if(splitTokens.Length == 1 && splitTokens[0].IsEmpty())
                    {
                        return new MethodDecl(type, name, new Parameters([]));
                    }
                    var parameters = splitTokens.Select(t => new Parser(t).ParseParameter()).ToArray();
                    return new MethodDecl(type, name, new Parameters(parameters));
                }
                else
                {
                    return new InvalidLine(tokens.GetCode());
                }
            }
            return new InvalidLine(tokens.GetCode());
        }
        else
        {
            return new InvalidLine(tokens.GetCode());
        }
    }

    static Invocation ParseInvocation(string name, string argsCode)
    {
        var splitTokens = new Tokens([.. new Tokenizer(argsCode[1..^1]).GetTokens()])
                .Split(TokenKind.Comma);
        if(splitTokens.Length == 1 && splitTokens[0].IsEmpty())
        {
            return new Invocation(name, new Arguments([]));
        }
        var args = splitTokens.Select(t => new Parser(t).ParseExpression()).ToArray();
        return new Invocation(name, new Arguments(args));
    }

    IExpression ParseExpression()
    {
        if(tokens.Count == 1)
        {
            var t = tokens[index];
            if(t.kind == TokenKind.Number)
            {
                return new NumberExpr(tokens[index].value);
            }
            else if(t.kind == TokenKind.String)
            {
                return new StringExpr(tokens[index].value);
            }
            else if(t.kind == TokenKind.Keyword && t.value == "true")
            {
                return new BoolExpr("true");
            }
            else if(t.kind == TokenKind.Keyword && t.value == "false")
            {
                return new BoolExpr("false");
            }
            else if(t.kind == TokenKind.Identifier)
            {
                return new IdentifierExpr(t.value);
            }
        }
        if(tokens.Count == 2)
        {
            if(tokens[0].kind == TokenKind.Identifier && tokens[1].kind == TokenKind.Parentheses)
            {
                return ParseInvocation(tokens[0].value, tokens[1].value);
            }
        }
        if(tokens.Count > 1)
        {
            if(tokens[0].kind == TokenKind.Operator && tokens[0].value == "!")
            {
                return new UnaryExpr(tokens[0].value, new Parser(tokens.GetTokensAfter(0)).ParseExpression());
            }
        }        
        return new Invalid(tokens.GetCode());
    }

    public ILineTree ParseStatement()
    {
        if (IsEmpty())
        {
            return new Empty();
        }
        else if(IsIdentifier(out string name))
        {
            if (IsParentheses(out string argsCode) && AtEnd())
            {          
                return ParseInvocation(name, argsCode);
            }
            return new InvalidLine(tokens.GetCode());
        }
        else if (IsKeyword("while"))
        {
            var condition = new Parser(tokens.GetTokensAfter(0)).ParseExpression();
            return new WhileStmt(condition);
        }
        else
        {
            return new InvalidLine(tokens.GetCode());
        }
    }

}