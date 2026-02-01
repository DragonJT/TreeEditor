
using Raylib_cs;

interface ILineTree
{
    void Draw(Layout layout, int indent, bool selected);
    string GetCode();
    string ToC(Tree tree);
}

interface IParameter
{
    void Draw(Layout layout);
    string GetCode();
    string ToC();
}

interface IParser
{
    ILineTree Parse(string code);
}

class InvocationStmt(string name, IExpression value) : ILineTree
{
    public readonly string name = name;
    public readonly IExpression value = value;

    public void Draw(Layout layout, int indent, bool selected)
    {
        layout.DrawIndent(indent, selected);
        layout.DrawText(name, Color.Blue);
        layout.DrawText("(", Color.White);
        value.Draw(layout);
        layout.DrawText(")", Color.White);
    }

    public string GetCode()
    {
        return $"{name}({value})";
    }

    public string ToC(Tree tree)
    {
        if(name == "Print")
        {
            var type = value.Type();
            if(type == "string")
            {
                return $"printf(\"%s\\n\", {value.ToC()});\n";
            }
            else if(type == "int")
            {
                return $"printf(\"%i\\n\", {value.ToC()});\n";
            }
            else if(type == "float")
            {
                return $"printf(\"%f\\n\", {value.ToC()});\n";
            }
            else
            {
                throw new Exception();
            }
        }
        else
        {
            return $"{name}({value});\n";   
        }
    }
}

class Parameter(string type, string name) : IParameter
{
    public readonly string type = type;
    public readonly string name = name;

    public void Draw(Layout layout)
    {
        layout.DrawText(type, Color.Magenta);
        layout.DrawSpace();
        layout.DrawText(name, Color.Blue);
    }

    public string GetCode()
    {
        return $"{type} {name}";
    }

    public string ToC()
    {
        return $"{type} {name}";
    }
}

class MethodDecl(string type, string name, IParameter[] parameters) : ILineTree, IParser
{
    public readonly string type = type;
    public readonly string name = name;
    public readonly IParameter[] parameters = parameters;

    public void Draw(Layout layout, int indent, bool selected)
    {
        layout.DrawIndent(indent, selected);
        layout.DrawText(type, Color.Magenta);
        layout.DrawSpace();
        layout.DrawText(name, Color.Blue);
        layout.DrawText("(", Color.White);
        for(var i = 0; i < parameters.Length; i++)
        {
            parameters[i].Draw(layout);
            if(i < parameters.Length - 1)
            {
                layout.DrawText(",", Color.White);
                layout.DrawSpace();
            }
        }
        layout.DrawText(")", Color.White);
    }

    public string GetCode()
    {
        var parametersCode = string.Join(',', parameters.Select(p=>p.ToC()));
        return $"{type} {name}({parametersCode})";
    }

    public ILineTree Parse(string code)
    {
        return new Parser(code).ParseStatement();
    }

    public string ToC(Tree tree)
    {
        var parametersC = string.Join(',', parameters.Select(p=>p.ToC()));
        return $"{type} {name}({parametersC}){{\n{tree.ToC()}}}\n";
    }
}

class FieldDecl(string type, string name) : ILineTree
{
    public readonly string type = type;
    public readonly string name = name;

    public void Draw(Layout layout, int indent, bool selected)
    {
        layout.DrawIndent(indent, selected);
        layout.DrawText(type, Color.Magenta);
        layout.DrawSpace();
        layout.DrawText(name, Color.Blue);
    }

    public string GetCode()
    {
        return $"{type} {name}";
    }

    public string ToC(Tree tree)
    {
        return $"{type} {name};\n";
    }
}

class ClassDecl(string name) : ILineTree, IParser
{
    public readonly string name = name;

    public void Draw(Layout layout, int indent, bool selected)
    {
        layout.DrawIndent(indent, selected);
        layout.DrawText("class", Color.Magenta);
        layout.DrawSpace();
        layout.DrawText(name, Color.Blue);
    }
    public string GetCode()
    {
        return $"class {name}";
    }

    public ILineTree Parse(string code)
    {
        return new Parser(code).ParseClassMember();
    }

    public string ToC(Tree tree)
    {
        throw new Exception();
    }
}

class SingletonDecl(string name) : ILineTree, IParser
{
    public readonly string name = name;

    public void Draw(Layout layout, int indent, bool selected)
    {
        layout.DrawIndent(indent, selected);
        layout.DrawText("singleton", Color.Magenta);
        layout.DrawSpace();
        layout.DrawText(name, Color.Blue);
    }
    public string GetCode()
    {
        return $"singleton {name}";
    }

    public ILineTree Parse(string code)
    {
        return new Parser(code).ParseClassMember();
    }

    public string ToC(Tree tree)
    {
        return tree.ToC();
    }
}

class Root : ILineTree, IParser
{
    public void Draw(Layout layout, int indent, bool selected)
    {
        layout.DrawIndent(indent, selected);
    }

    public string GetCode() => "";

    public ILineTree Parse(string code)
    {
        return new Parser(code).Parse();
    }

    public string ToC(Tree tree)
    {
        throw new Exception();
    }
}

class Empty : ILineTree
{
    public void Draw(Layout layout, int indent, bool selected)
    {
        layout.DrawIndent(indent, selected);
    }

    public string GetCode() => "";

    public string ToC(Tree tree)
    {
        return "\n";
    }
}
