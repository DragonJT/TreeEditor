


using System.Runtime.Serialization;

class Variable(VarInitializationStmt varInitializationStmt, object value)
{
    public VarInitializationStmt varInitializationStmt = varInitializationStmt;
    public object value = value;
}


static class ConnectVariables
{
    static void ConnectExpr(Tree tree, IExpression expression)
    {
        if(expression is IdentifierExpr identifierExpr)
        {
            foreach(var c in tree.Parent.children)
            {
                if(c.LineTree is VarInitializationStmt varInitializationStmt)
                {
                    if(varInitializationStmt.name == identifierExpr.value)
                    {
                        identifierExpr.variable = varInitializationStmt.variable;
                    }
                }
            }
        }
    }

    static void ConnectArgs(Tree tree, Arguments arguments)
    {
        foreach(var a in arguments.args)
        {
            ConnectExpr(tree, a);
        }
    }

    static void ConnectTree(Tree tree)
    {
        if(tree.LineTree is Invocation invocation)
        {
            ConnectArgs(tree, invocation.args);
        }
    }

    public static void Connect(Tree tree)
    {
        if(tree.LineTree is VarInitializationStmt)
        {
            foreach(var c in tree.Parent.children)
            {
                ConnectTree(c);
            }
        }
        else
        {
            ConnectTree(tree);
        }
    }
}