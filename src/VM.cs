
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

static class VM
{
    static object GetExpression(IExpression expression)
    {
        if(expression is StringExpr stringExpr)
        {
            var s = stringExpr.value;
            if(s.Length >= 2)
            {
                return stringExpr.value[1..^1];
            }
            return null;
        }
        else if(expression is IdentifierExpr identifierExpr)
        {
            if(identifierExpr.variable == null)
            {
                return null;
            }
            return identifierExpr.variable.value;
        }
        else if(expression is FloatExpr floatExpr)
        {
            return floatExpr.value;
        }
        else if(expression is IntExpr intExpr)
        {
            return intExpr.value;
        }
        else if(expression is Invalid)
        {
            return null;
        }
        else
        {
            throw new Exception(expression.GetType().Name);
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
                    var value = GetExpression(invocation.args.args[0]);
                    if(value == null)
                    {
                        JConsole.WriteLine("null");
                    }
                    else
                    {
                        JConsole.WriteLine(value.ToString());
                    }
                }
            }
            else if(c.LineTree is VarInitializationStmt varInitializationStmt)
            {
                varInitializationStmt.variable.value = GetExpression(varInitializationStmt.expression);
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