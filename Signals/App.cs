using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Signals
{
    public class App
    {

        private IView activeView;

        private List<Document> documents = new List<Document>();

        private MainForm mainForm;

        private static App theApp;

        public static App Instance
        {
            get { return theApp; }
        }

        public static void Initialize(MainForm form)
        {
            theApp = new App();
            theApp.mainForm = form;
        }

        public MainForm MainForm
        {
            get { return mainForm; }
        }

        public Document ActiveDocument
        {
            get 
            {
                if (activeView == null)
                    return null;
                    
                return activeView.GetDocument();
            }
        }
        
        public void CloseActiveView()
        {
            if (mainForm.TabControl.TabPages.Count == 0)
                return;

            Document docToClose = ActiveDocument;

            docToClose.DetachView(activeView);
            mainForm.TabControl.TabPages.Remove(GetTabPageForView(activeView));

            if (!docToClose.HasAnyView())
                documents.Remove(docToClose);
        }

        public void NewDocumentWithView()
        {
            NewDocForm form = new NewDocForm();
            if (form.ShowDialog() != DialogResult.OK)
                return;

            SignalDocument doc = new SignalDocument(form.DocName); 
            documents.Add(doc);
            CreateView(doc, true);
        }

        public void UpdateActiveView()
        {
            if (mainForm.TabControl.TabPages.Count == 0)
                activeView = null;
            else
                activeView = (IView)mainForm.TabControl.SelectedTab.Tag;
        }


        public void CreateViewForActiveDocument()
        {
            Document doc = ActiveDocument;
            if (doc == null)
                return;
            CreateView(doc, true);
        }

        public void OpenDocumentWithView()
        {
            string path = "";

            using(OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if(openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    path = openFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }

            string docName = System.IO.Path.GetFileName(path);            
            SignalDocument openedDoc = new SignalDocument(docName);

            documents.Add(openedDoc);
            CreateView(openedDoc, true);

            openedDoc.LoadDocument(path);
            
        }

        public void SaveActiveDocument()
        {
            if (ActiveDocument == null)
                return;

            SaveFileDialog saveDialog = new SaveFileDialog();

            saveDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveDialog.FilterIndex = 2;
            saveDialog.RestoreDirectory = true;

            if (saveDialog.ShowDialog() != DialogResult.OK)
                return;

            ActiveDocument.SaveDocument(saveDialog.FileName);
        }

        IView CreateView(Document doc, bool activateView)
        {
            // Új tab felvétele: az első paraméter egy kulcs, a második a tab felirata
            //mainForm.TabControl.TabPages.Add(form.DocName, form.DocName);
            TabPage tp = new TabPage(doc.Name);
            mainForm.TabControl.TabPages.Add(tp);

            //Feladat 3.1. DemoView helyett GraphicsSignalView létrehozása
            GraphicsSignalView view = new GraphicsSignalView((SignalDocument)doc);

            //TabPage tp = mainForm.TabControl.TabPages[form.DocName];
            tp.Controls.Add(view);
            tp.Tag = view;
            view.Dock = DockStyle.Fill;

            // A View beregisztrálása a dokumentumnál, hogy értesüljön majd a dokumentum változásairól.
            doc.AttachView(view);

            // Ha az új nézet nem a dokumentum első nézete, akkor a tabfülön a nézet sorszámát is
            // megjelenítjük.
            if (view.ViewNumber > 1)
                tp.Text = tp.Text + ":" + view.ViewNumber;

            // Az új tab legyen a kiválasztott. 
            if (activateView)
            {
                mainForm.TabControl.SelectTab(tp); // Ennek hatására elsül a tab SelectedIndexChanged eseménykezelője, ami meg beállítja az activeView tagváltozót
                activeView = view;
            }
            return view;
        }

        TabPage GetTabPageForView(IView view)
        {
            foreach (TabPage page in mainForm.TabControl.TabPages)
               if (page.Tag == activeView)
                   return page;
            throw new Exception("Page for view not found.");
        }

    }
}
