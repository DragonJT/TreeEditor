
using Raylib_cs;

interface IExpression
{
    void Draw(Layout layout);
    string GetCode();
    string ToC();
    string Type();
}

class StringExpr(string value) : IExpression
{
    public readonly string value = value;

    public void Draw(Layout layout)
    {
        layout.DrawText(value, Color.Orange);
    }

    public string GetCode()
    {
        return value;
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
    
    public void Draw(Layout layout)
    {
        layout.DrawText(value, new Color(0.7f, 1f, 0.3f));
    }

    public string GetCode()
    {
        return value;
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
