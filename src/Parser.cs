
class Invalid(string text) : IExpression, IParameter
{
    public string text = text;

    public void Draw(Layout layout)
    {
        layout.DrawInvalidText(text);
    }
    public string GetCode()
    {
        return text;
    }

    public string ToC()
    {
        throw new Exception();
    }

    public string Type()
    {
        throw new Exception();
    }
}

class InvalidLine(string text) : ILineTree
{
    public string text = text;

    public void Draw(Layout layout, int indent, bool selected)
    {
        layout.DrawIndent(indent, selected);
        layout.DrawInvalidText(text);
    }
    public string GetCode()
    {
        return text;
    }

    public string ToC(Tree tree)
    {
        throw new Exception();
    }
}

class Parser
{
    string code;
    readonly List<Token> tokens;
    int index = 0;

    public Parser(string code)
    {
        this.code = code;
        tokens = new Tokenizer(code).GetTokens();
    }

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

    bool IsOther(string other)
    {
        if(index < tokens.Count && tokens[index].kind == TokenKind.Error && tokens[index].value == other)
        {
            index++;
            return true;
        }
        return false;
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
            return new InvalidLine(code);
        }
        else if (IsKeyword("singleton"))
        {
            if(IsIdentifier(out string name) && AtEnd())
            {
                return new SingletonDecl(name);
            }
            return new InvalidLine(code);
        }
        else
        {
            return new InvalidLine(code);
        }
    }

    public IParameter ParseParameter()
    {
        if(IsIdentifier(out string type) && IsIdentifier(out string name) && AtEnd())
        {
            return new Parameter(type, name);
        }
        return new Invalid(code);
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
                    var parameters = parameterCode.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(c => new Parser(c).ParseParameter()).ToArray();
                    return new MethodDecl(type, name, parameters);
                }
                else
                {
                    return new InvalidLine(code);
                }
            }
            return new InvalidLine(code);
        }
        else
        {
            return new InvalidLine(code);
        }
    }

    IExpression ParseExpression()
    {
        if(tokens.Count == 1)
        {
            if(tokens[index].kind == TokenKind.Number)
            {
                return new NumberExpr(tokens[index].value);
            }
            else if(tokens[index].kind == TokenKind.String)
            {
                return new StringExpr(tokens[index].value);
            }
        }
        return new Invalid(code);
    }

    public ILineTree ParseStatement()
    {
        if (IsEmpty())
        {
            return new Empty();
        }
        else if(IsIdentifier(out string name))
        {
            if (IsParentheses(out string exprcode) && AtEnd())
            {          
                var value = new Parser(exprcode).ParseExpression();
                return new InvocationStmt(name, value);
            }
            return new InvalidLine(code);
        }
        else
        {
            return new InvalidLine(code);
        }
    }

}