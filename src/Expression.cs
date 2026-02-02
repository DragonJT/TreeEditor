
using Raylib_cs;

interface IExpression
{
    void Draw(ILayout layout);
    string Type();
}

class IdentifierExpr(string value) : IExpression
{
    public readonly string value = value;

    public void Draw(ILayout layout)
    {
        layout.DrawText(value, Color.Lime);
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

    public string Type()
    {
        return "bool";
    }
}