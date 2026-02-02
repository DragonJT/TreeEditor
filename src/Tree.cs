using Raylib_cs;

interface ILayout
{
    void DrawSpace();
    void DrawText(string text, Color color);
    void DrawInvalidText(string text);
}

class TextLayout : ILayout
{
    public string text = "";

    public void DrawInvalidText(string text)
    {
        this.text += text;
    }

    public void DrawSpace()
    {
        text += ' ';
    }

    public void DrawText(string text, Color color)
    {
        this.text += text;
    }
}

class DrawLayout : ILayout
{
    readonly int spacing = 6;
    readonly int fontSize = 35;
    readonly int lineSize = 50;
    readonly int border = 20;
    readonly int indentSize = 60;
    readonly int spaceSize = 18;
    int x;
    int y;

    public DrawLayout()
    {
        x = border;
        y = border;
    }

    public void NewLine()
    {
        y += lineSize;
        x = border;
    }

    public void DrawSpace()
    {
        x += spaceSize;
    }
    
    public void DrawText(string text, Color color)
    {
        Raylib.DrawText(text, x, y, fontSize, color);
        x += Raylib.MeasureText(text, fontSize) + spacing;
    }

    public void DrawInvalidText(string text)
    {
        Raylib.DrawText(text, x, y, fontSize, Color.White);
        var length = Raylib.MeasureText(text, fontSize) + spacing;
        Raylib.DrawLineEx(new (x, y+fontSize), new (x+length, y+fontSize), 3, Color.Red);
        x += length;
    }

    public void DrawIndent(int indent, bool selected)
    {
        if (selected)
        {
            Raylib.DrawRectangle(0, y, Raylib.GetScreenWidth(), fontSize, new Color(0.2f,0.2f,0.2f));
        }
        for(var i = 0; i < indent; i++)
        {
            Raylib.DrawRectangle(x, y, 4, fontSize, new Color(0,0,0,0.3f));
            x += indentSize;
        }
        Raylib.DrawRectangle(x, y, 4, fontSize, new Color(0,0,0,0.3f));
    }
}

class Tree
{
    public Tree Parent { get; private set; }
    ILineTree lineTree;
    readonly List<Tree> children = [];

    public bool IsRoot => Parent == null;

    public Tree(bool isRoot = false)
    {
        if (isRoot)
        {
            lineTree = new Root();
        }
        else
        {
            lineTree = new Empty();
        }
    }

    string GetCode()
    {
        var layout = new TextLayout();
        lineTree.Draw(layout);
        return layout.text;
    }

    public bool Backspace()
    {
        var code = GetCode();
        if(code.Length > 0)
        {
            code = code[..^1];
            if(Parent.lineTree is IParser parser)
            {
                var tokens = new Tokens([..new Tokenizer(code).GetTokens()]);
                lineTree = parser.Parse(tokens);
            }
            return true;
        }
        return false;
    }

    public void InsertCode(string text)
    {
        var code = GetCode();
        code += text;
        if(Parent.lineTree is IParser parser)
        {
            var tokens = new Tokens([..new Tokenizer(code).GetTokens()]);
            lineTree = parser.Parse(tokens);
        }
    }

    void Draw(DrawLayout layout, int indent)
    {
        layout.DrawIndent(indent, Program.selected == this);
        lineTree.Draw(layout);
        layout.NewLine();
        foreach(var c in children)
        {
            c.Draw(layout, indent + 1);
        }
    }

    public void Draw()
    {
        DrawLayout layout = new();
        foreach(var c in children)
        {
            c.Draw(layout, 0);
        }
    }

    public Tree AddChild()
    {
        var tree = new Tree();
        AddChild(tree, 0);
        return tree;
    }

    public void AddChild(Tree child, int index)
    {
        children.Insert(index, child);
        child.Parent = this;
    }

    public void SetParentAfter(Tree newParent, Tree after)
    {
        Parent.children.Remove(this);
        newParent.AddChild(this, after.Index() + 1);
    }

    public int Index()
    {
        if(Parent == null)
        {
            return 0;
        }
        return Parent.children.IndexOf(this);
    }

    public Tree GetChild(int index)
    {
        return children[index];
    }

    public void Delete()
    {
        if (Parent != null)
        {
            Parent.children.Remove(this);
        }
    }

    public Tree LastChild()
    {
        if(children.Count > 0)
        {
            return children[^1].LastChild();
        }
        return this;
    }

    public Tree Next(bool goIntoChildren = true)
    {
        if(goIntoChildren && children.Count > 0)
        {
            return children[0];
        }
        else if(Parent != null)
        {
            var id = Index() + 1;
            if(id < Parent.children.Count)
            {
                return Parent.children[id];
            }
            else
            {
                return Parent.Next(false);
            }
        }
        else
        {
            return null;
        }
    }

    public Tree AddNextChild()
    {
        var tree = new Tree();
        if(lineTree is IParser)
        {
            AddChild(tree, 0);
            return tree;
        }
        else
        {
            Parent.AddChild(tree, Index()+1);
            return tree;
        }
    }
}