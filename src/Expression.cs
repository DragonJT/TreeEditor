
using Raylib_cs;

interface IExpression
{
    void Draw(ILayout layout);
    string Type();
}

class IdentifierExpr(string value) : IExpression
{
    public string value = value;
    public Variable variable;

    public void Draw(ILayout layout)
    {
        if(variable == null)
        {
            layout.DrawText(value, Color.Green, "Cant find variable");
        }
        else
        {
            layout.DrawText(value, Color.Green);
        }
    }

    public string Type()
    {
        if (variable == null)
        {
            return "error";
        }
        return variable.varInitializationStmt.expression.Type();
    }
}

class StringExpr(string value) : IExpression
{
    public string value = value;

    public void Draw(ILayout layout)
    {
        layout.DrawText(value, Color.Orange);
    }

    public string Type()
    {
        return "string";
    }
}


class IntExpr : IExpression
{
    public string text;
    bool error;
    public int value;

    public IntExpr(string text)
    {
        this.text = text;
        error = !int.TryParse(text, out value);
    }

    public void Draw(ILayout layout)
    {
        if (error)
        {
            layout.DrawText(text, new Color(0.7f, 1f, 0.3f), "Cant parse int");
        }
        else
        {
            layout.DrawText(text, new Color(0.7f, 1f, 0.3f));
        }
        
    }

    public string Type() => "int";
}

class FloatExpr : IExpression
{
    public string text;
    bool error;
    public float value;

    public FloatExpr(string text)
    {
        this.text = text;
        error = !float.TryParse(text, out value);
    }

    public void Draw(ILayout layout)
    {
        if (error)
        {
            layout.DrawText(text, new Color(0.7f, 1f, 0.3f), "Cant parse float");
        }
        else
        {
            layout.DrawText(text, new Color(0.7f, 1f, 0.3f));
        }
    }

    public string Type() => "float";
}

class BoolExpr(string value) : IExpression
{
    public string value = value;

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
    public string op = op;
    public IExpression expression = expression;

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