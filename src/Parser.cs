
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

    public void Draw(ILayout layout)
    {
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
                var args = new Arguments([..exprcode.Split(',').Select(c=>new Parser(c).ParseExpression())]);
                return new InvocationStmt(name, args);
            }
            return new InvalidLine(code);
        }
        else if (IsKeyword("while"))
        {
            var condition = new Parser(code[5..]).ParseExpression();
            return new WhileStmt(condition);
        }
        else
        {
            return new InvalidLine(code);
        }
    }

}