using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Signals
{
    public interface IView
    {
        void Update();

        Document GetDocument();

        int ViewNumber { get; set; }
    }
}
