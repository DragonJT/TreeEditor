
using Raylib_cs;

interface ILineTree
{
    void Draw(ILayout layout);
}

interface IParameter
{
    void Draw(ILayout layout);
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
}

class Empty : ILineTree
{
    public void Draw(ILayout layout){}
}
