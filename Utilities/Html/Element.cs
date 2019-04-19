using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Html
{
    public class Element
    {
        public Element()
        {
            Parent = null;
            Children = new List<Element>();
            Content = "";
        }

        public Element Parent;
        public List<Element> Children;
        public string Content;
    }
}
