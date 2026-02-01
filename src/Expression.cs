
using Raylib_cs;

interface IExpression
{
    void Draw(ILayout layout);
    string ToC();
    string Type();
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
