
using Raylib_cs;

interface IExpression
{
    void Draw(ILayout layout);
    string ToC();
    string Type();
}

class IdentifierExpr(string value) : IExpression
{
    public readonly string value = value;

    public void Draw(ILayout layout)
    {
        layout.DrawText(value, Color.Lime);
    }

    public string ToC()
    {
        return value;
    }

    public string Type()
    {
        return "NOT IMPLEMENTED YET";
    }
}

class StringExpr(string value) : IExpression
{
    public readonly string value = value;

    public void Draw(ILayout layout)
    {
        layout.DrawText(value, Color.Orange);
    }

    public string ToC()
    {
        return value;
    }

    public string Type()
    {
        return "string";
    }
}


class NumberExpr(string value) : IExpression
{
    public readonly string value = value;
    
    public void Draw(ILayout layout)
    {
        layout.DrawText(value, new Color(0.7f, 1f, 0.3f));
    }

    public string ToC()
    {
        return value;
    }

    public string Type()
    {
        return value.Contains('.') ? "float" : "int";
    }
}

class BoolExpr(string value) : IExpression
{
    public readonly string value = value;

    public void Draw(ILayout layout)
    {
        layout.DrawText(value, new Color(0.7f, 1f, 0.3f));
    }

    public string ToC()
    {
        return value == "true" ? "1" : "0";
    }

    public string Type()
    {
        return "bool";
    }
}


class UnaryExpr(string op, IExpression expression) : IExpression
{
    public readonly string op = op;
    public readonly IExpression expression = expression;

    public void Draw(ILayout layout)
    {
        layout.DrawText(op, new Color(0.3f, 0.7f, 1f));
        expression.Draw(layout);
    }

    public string ToC()
    {
        return op+expression.ToC();
    }

    public string Type()
    {
        return "bool";
    }
}