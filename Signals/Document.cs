using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Signals
{
    public class Document
    {
        private string name;

        List<IView> views = new List<IView>();

        private int viewCount;

        public string Name
        {
            get { return name; }
        }

        public int ViewCount
        {
            get { return viewCount; }
        }


        public Document(string name)
        {
            this.name = name;
        }

        public void AttachView(IView v)
        {
            views.Add(v);
            viewCount++;
            v.ViewNumber = viewCount;
            v.Update();
        }

        public void DetachView(IView v)
        {
            views.Remove(v);
            viewCount--;
        }

        public bool HasAnyView()
        {
            return views.Count > 0;
        }
        
        protected void UpdateAllViews()
        {
            foreach (IView view in views)
                view.Update();
        }

        public virtual void LoadDocument(string filePath)
        { }

        public virtual void SaveDocument(string filePath)
        { }

        public virtual void SwitchLiveDataSource()
        { }
    }
}
