
using Raylib_cs;

interface ILineTree
{
    void Draw(ILayout layout);
    string ToC(Tree tree);
}

interface IParameter
{
    void Draw(ILayout layout);
    string ToC();
}

interface IParser
{
    ILineTree Parse(Tokens tokens);
}


class Arguments(IExpression[] args)
{
    public readonly IExpression[] args = args;

    public void Draw(ILayout layout)
    {
        layout.DrawText("(", Color.White);
        for(var i = 0; i < args.Length; i++)
        {
            args[i].Draw(layout);
            if(i < args.Length - 1)
            {
                layout.DrawText(",", Color.White);
                layout.DrawSpace();
            }
        }
        layout.DrawText(")", Color.White);
    }

    public string ToC()
    {
        return "("+string.Join(", ", args.Select(a=>a.ToC()))+")";
    }
}

class Invocation(string name, Arguments args) : ILineTree, IExpression
{
    public readonly string name = name;
    public readonly Arguments args = args;

    public void Draw(ILayout layout)
    {
        layout.DrawText(name, Color.Blue);
        args.Draw(layout);
    }

    public string ToC()
    {
        if(name == "Print")
        {
            if(args.args.Length != 1)
            {
                throw new Exception();
            }
            var arg = args.args[0];
            var type = arg.Type();
            if(type == "string")
            {
                return $"printf(\"%s\\n\", {arg.ToC()});\n";
            }
            else if(type == "int")
            {
                return $"printf(\"%i\\n\", {arg.ToC()});\n";
            }
            else if(type == "float")
            {
                return $"printf(\"%f\\n\", {arg.ToC()});\n";
            }
            else
            {
                throw new Exception();
            }
        }
        else
        {
            return $"{name}{args.ToC()}";   
        }
    }

    public string ToC(Tree tree)
    {
        return ToC()+";\n";
    }

    public string Type()
    {
        return "NOT IMPLEMENTED YET";
    }
}

class WhileStmt(IExpression condition) : ILineTree, IParser
{
    public readonly IExpression condition = condition;

    public void Draw(ILayout layout)
    {
        layout.DrawText("while", Color.Magenta);
        layout.DrawSpace();
        condition.Draw(layout);
    }

    public ILineTree Parse(Tokens tokens)
    {
        return new Parser(tokens).ParseStatement();
    }

    public string ToC(Tree tree)
    {
        return $"while({condition.ToC()}){{\n{tree.ToC()}}}\n";
    }
}

class Parameter(string type, string name) : IParameter
{
    public readonly string type = type;
    public readonly string name = name;

    public void Draw(ILayout layout)
    {
        layout.DrawText(type, Color.Magenta);
        layout.DrawSpace();
        layout.DrawText(name, Color.Blue);
    }

    public string ToC()
    {
        return $"{type} {name}";
    }
}

class Parameters(IParameter[] parameters)
{
    public readonly IParameter[] parameters = parameters;

    public void Draw(ILayout layout)
    {
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

    public string ToC()
    {
        return "("+string.Join(',', parameters.Select(p=>p.ToC()))+")";
    }
}

class MethodDecl(string type, string name, Parameters parameters) : ILineTree, IParser
{
    public readonly string type = type;
    public readonly string name = name;
    public readonly Parameters parameters = parameters;

    public void Draw(ILayout layout)
    {
        layout.DrawText(type, Color.Magenta);
        layout.DrawSpace();
        layout.DrawText(name, Color.Blue);
        parameters.Draw(layout);
    }

    public ILineTree Parse(Tokens tokens)
    {
        return new Parser(tokens).ParseStatement();
    }

    public string ToC(Tree tree)
    {
        return $"{type} {name}{parameters.ToC()}{{\n{tree.ToC()}}}\n";
    }
}

class FieldDecl(string type, string name) : ILineTree
{
    public readonly string type = type;
    public readonly string name = name;

    public void Draw(ILayout layout)
    {
        layout.DrawText(type, Color.Magenta);
        layout.DrawSpace();
        layout.DrawText(name, Color.Blue);
    }

    public string ToC(Tree tree)
    {
        return $"{type} {name};\n";
    }
}

class ClassDecl(string name) : ILineTree, IParser
{
    public readonly string name = name;

    public void Draw(ILayout layout)
    {
        layout.DrawText("class", Color.Magenta);
        layout.DrawSpace();
        layout.DrawText(name, Color.Blue);
    }

    public ILineTree Parse(Tokens tokens)
    {
        return new Parser(tokens).ParseClassMember();
    }

    public string ToC(Tree tree)
    {
        throw new Exception();
    }
}

class SingletonDecl(string name) : ILineTree, IParser
{
    public readonly string name = name;

    public void Draw(ILayout layout)
    {
        layout.DrawText("singleton", Color.Magenta);
        layout.DrawSpace();
        layout.DrawText(name, Color.Blue);
    }

    public ILineTree Parse(Tokens tokens)
    {
        return new Parser(tokens).ParseClassMember();
    }

    public string ToC(Tree tree)
    {
        return tree.ToC();
    }
}

class Root : ILineTree, IParser
{
    public void Draw(ILayout layout){}

    public ILineTree Parse(Tokens tokens)
    {
        return new Parser(tokens).Parse();
    }

    public string ToC(Tree tree)
    {
        throw new Exception();
    }
}

class Empty : ILineTree
{
    public void Draw(ILayout layout){}

    public string ToC(Tree tree)
    {
        return "\n";
    }
}
