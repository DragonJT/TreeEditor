
using Raylib_cs;

static class JConsole
{
    static List<string> lines = [];

    public static void Clear()
    {
        lines.Clear();
    }

    public static void WriteLine(string line)
    {
        lines.Add(line);
    }

    public static void Draw()
    {
        var x = Raylib.GetScreenWidth() - 400;
        var y = 20;
        foreach(var l in lines)
        {
            Raylib.DrawText(l, x, y, 35, Color.White);
            y += 45;
        }
    }
}

static class Executer
{
    static object GetExpression(IExpression expression)
    {
        if(expression is StringExpr stringExpr)
        {
            return stringExpr.value[1..^1];
        }
        else
        {
            throw new Exception();
        }
    }

    static void ExecuteMethod(Tree method)
    {
        foreach(var c in method.children)
        {
            if(c.LineTree is Invocation invocation)
            {
                if(invocation.name == "WriteLine")
                {
                    var text = (string)GetExpression(invocation.args.args[0]);
                    JConsole.WriteLine(text);
                }
            }
        }
    }

    public static void Execute(Tree root)
    {
        JConsole.Clear();
        if(root.LineTree is SingletonDecl singletonDecl)
        {
            var update = root.children
                .FirstOrDefault(c=>c.LineTree is MethodDecl methodDecl && methodDecl.name == "Update");
            if(update != null)
            {
                ExecuteMethod(update);
            }
        }
        JConsole.Draw();
    }
}