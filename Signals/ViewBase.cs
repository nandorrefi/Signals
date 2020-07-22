using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Signals
{
    public class ViewBase : UserControl, IView
    {
        protected int viewNumber;

        protected Document document;

        public int ViewNumber
        {
            get { return viewNumber; }
            set { viewNumber = value; }
        }

        public void Update()
        {
            Invalidate();
        }

        public Document GetDocument()
        {
            return document;
        }
    }
}
