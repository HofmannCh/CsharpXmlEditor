using System.Data;
using System.Xml;

namespace XmlEditor
{
    public class XmlEditorForm
    {
        public Form Form { get; private set; }

        public string XmlFile { get; init; }
        public string XmlSearch { get; init; }
        public (string Key, string Path)[] XmlMappings { get; init; }

        public XmlEditorForm()
        {
            Form = new Form
            {
                Size = new Size(600, 500),
                Text = "Xml Editor",
                FormBorderStyle = FormBorderStyle.FixedSingle
            };
        }

        protected TextBox ElXmlFile;
        protected Button ElXmlFileSelectButton;
        protected TextBox ElXmlSearch;
        protected Button ElXmlSearchButton;
        protected DataGridView ElConfigTable;
        protected Button ElXmlLoadButton;
        protected Button ElXmlShowButton;
        protected DataGridView ElDataTable;
        protected Button ElXmlExportButton;

        public Form Init()
        {
            Form.Controls.Clear();

            // Path row
            new Label()
            {
                Location = new Point(10, 10),
                Size = new Size(60, 25),
                Text = "Path",
                Parent = Form,
            };

            ElXmlFile = new TextBox()
            {
                Location = new Point(80, 10),
                Size = new Size(430, 25),
                Text = XmlFile,
                Parent = Form
            };
            ElXmlFile.KeyPress += ElXmlFileText_KeyPress;

            ElXmlFileSelectButton = new Button()
            {
                Location = new Point(520, 10),
                Size = new Size(50, 25),
                Text = "Open XML",
                Parent = Form,
            };
            ElXmlFileSelectButton.Click += ElXmlFileSelectButton_Click;

            // Search row
            new Label()
            {
                Location = new Point(10, 50),
                Size = new Size(60, 25),
                Text = "Search",
                Parent = Form,
            };

            ElXmlSearch = new TextBox()
            {
                Location = new Point(80, 50),
                Size = new Size(430, 25),
                Text = XmlSearch,
                PlaceholderText = "Search Element name or XPath",
                Parent = Form,
            };
            ElXmlSearch.KeyPress += ElXmlSearch_KeyPress;

            ElXmlSearchButton = new Button()
            {
                Location = new Point(520, 50),
                Size = new Size(50, 25),
                Text = "Search",
                Parent = Form,
            };
            ElXmlSearchButton.Click += ElXmlSearchButton_Click;

            // Config table
            new Label()
            {
                Location = new Point(10, 90),
                Size = new Size(60, 25),
                Text = "Config",
                Parent = Form,
            };

            ElConfigTable = new DataGridView()
            {
                Location = new Point(80, 90),
                Size = new Size(430, 100),
                Parent = Form,

                ColumnCount = 2,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                ColumnHeadersHeight = 20,
                RowHeadersVisible = true,
                RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing,
                AllowUserToResizeColumns = true,
                AllowUserToResizeRows = false,
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = true,
                AllowUserToOrderColumns = false,
            };
            ElConfigTable.RowTemplate.Height = 20;

            ElConfigTable.Columns[0].Name = "Field";
            ElConfigTable.Columns[0].Width = 150;
            ElConfigTable.Columns[0].ReadOnly = false;
            ElConfigTable.Columns[1].Name = "Column";
            ElConfigTable.Columns[1].Width = 150;
            ElConfigTable.Columns[1].ReadOnly = false;

            if (XmlMappings?.Any() == true)
                foreach (var m in XmlMappings)
                    ElConfigTable.Rows.Add([m.Key, m.Path]);

            ElXmlLoadButton = new Button()
            {
                Location = new Point(520, 90),
                Size = new Size(50, 25),
                Text = "Load",
                Parent = Form,
            };
            ElXmlLoadButton.Click += ElXmlLoadButton_Click;

            ElXmlShowButton = new Button()
            {
                Location = new Point(520, 165),
                Size = new Size(50, 25),
                Text = "Show",
                Parent = Form,
            };
            ElXmlShowButton.Click += ElXmlShowButton_Click;

            // Data grid
            new Label()
            {
                Location = new Point(10, 200),
                Size = new Size(60, 25),
                Text = "Data",
                Parent = Form,
            };

            ElDataTable = new DataGridView()
            {
                Location = new Point(80, 200),
                Size = new Size(490, 200),
                Parent = Form,

                //ColumnCount = 2,
                //ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                //ColumnHeadersHeight = 20,
                RowHeadersVisible = true,
                RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing,
                AllowUserToResizeColumns = true,
                AllowUserToResizeRows = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToOrderColumns = false,
                EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2,
            };
            ElDataTable.RowTemplate.Height = 20;

            ElXmlExportButton = new Button()
            {
                Location = new Point(520, 410),
                Size = new Size(50, 25),
                Text = "Export",
                Parent = Form,
            };
            ElXmlExportButton.Click += ElXmlExportButton_Click;


            ElXmlSearch.Enabled = false;
            ElXmlSearchButton.Enabled = false;
            ElDataTable.Enabled = false;
            LoadData();

            return Form;
        }

        private void ElXmlFileText_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                LoadData();
        }

        private void ElXmlFileSelectButton_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                //openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.CheckFileExists = true;
                openFileDialog.Multiselect = false;
                //openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ElXmlFile.Text = openFileDialog.FileName;
                    LoadData();
                }
            }
        }

        private void ElXmlSearch_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                LoadData();
        }

        private void ElXmlSearchButton_Click(object? sender, EventArgs e)
        {
            //SearchData(ElXmlSearch.Text);
        }
        private void LoadData()
        {
            var file = ElXmlFile.Text;

            if (string.IsNullOrWhiteSpace(file))
                return;

            if (!File.Exists(file))
            {
                MessageBox.Show($"File {file} doesn't exists", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ElXmlSearch.Enabled = true;
            ElXmlSearchButton.Enabled = true;

            XmlDocument = new XmlDocument();
            XmlDocument.Load(file);
        }

        private void ElXmlLoadButton_Click(object? sender, EventArgs e)
        {
            if (XmlDocument == null)
                LoadData();

            var form = new Form()
            {
                Size = new Size(300, 400),
                Text = "Select Fields",
                FormBorderStyle = FormBorderStyle.FixedDialog,
            };

            var list = new ListView()
            {
                Location = new Point(10, 10),
                Size = new Size(260, 300),
                Parent = form,
                View = View.Details,
            };
            list.Columns.Add("Field").Width = 100;
            list.Columns.Add("Path").Width = 120;

            var checkbox = new CheckBox()
            {
                Location = new Point(10, 320),
                Size = new Size(190, 25),
                Text = "Remove existing",
                Parent = form,
            };

            var button = new Button()
            {
                Location = new Point(210, 320),
                Size = new Size(60, 25),
                Text = "Submit selected",
                Parent = form,
            };
            button.Click += (object? sender, EventArgs e) =>
            {
                if (checkbox.Checked)
                    ElConfigTable.Rows.Clear();

                for (int i = 0; i < list.SelectedItems.Count; i++)
                {
                    var item = list.SelectedItems[i];
                    ElConfigTable.Rows.Add([item.Text, item.SubItems[1].Text]);
                }

                form.Close();
            };

            var nodes = GetNodesBySearch();

            var allKeys = new HashSet<string>(StringComparer.Ordinal);
            foreach (var node in ToIter(nodes))
                foreach (var child in ToIter(node.ChildNodes))
                    allKeys.Add(child.Name);

            foreach (var key in allKeys)
                list.Items.Add(new ListViewItem([key, "./" + key]));

            form.ShowDialog();
        }

        private void ElXmlShowButton_Click(object? sender, EventArgs e)
        {
            var conf = GetConf();

            ElDataTable.Rows.Clear();
            ElDataTable.Columns.Clear();

            var nodes = GetNodesBySearch();

            foreach (var item in conf)
            {
                var colI = ElDataTable.Columns.Add(null, null);
                ElDataTable.Columns[colI].Name = item.Key;
                ElDataTable.Columns[colI].Width = 120;
                ElDataTable.Columns[colI].ReadOnly = false;
                ElDataTable.Columns[colI].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            ElDataTable.Enabled = true;

            var values = new string[conf.Length];
            foreach (var node in ToIter(nodes))
            {
                foreach (var (index, _, path) in conf)
                {
                    values[index] = node.SelectSingleNode(path)?.InnerXml ?? string.Empty;
                }
                ElDataTable.Rows.Add(values);
            }
        }

        private void ElXmlExportButton_Click(object? sender, EventArgs e)
        {
            string exportFileName;
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                //openFileDialog.InitialDirectory = "c:\\";
                saveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                //openFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    exportFileName = saveFileDialog.FileName;
                else
                    return;
            }

            var conf = GetConf();
            var nodes = GetNodesBySearch();

            for (int rowI = 0; rowI < ElDataTable.Rows.Count; rowI++)
            {
                var row = ElDataTable.Rows[rowI];
                var node = nodes[rowI]!;
                foreach (var (index, _, path) in conf)
                {
                    node.SelectSingleNode(path)?.InnerXml = row.Cells[index].Value?.ToString() ?? string.Empty;
                }
            }

            XmlDocument.PreserveWhitespace = true;
            XmlDocument.Save(exportFileName);
        }

        private XmlNodeList GetNodesBySearch()
        {
            if (string.IsNullOrWhiteSpace(ElXmlSearch.Text))
                return XmlDocument.ChildNodes;

            try
            {
                return XmlDocument.SelectNodes(ElXmlSearch.Text)!;
            }
            catch (Exception)
            {
                MessageBox.Show($"Invalid xPath: {ElXmlSearch.Text}", "Invalid xPath", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return XmlDocument.ChildNodes;
            }

        }

        private (int index, string Key, string Path)[] GetConf()
        {
            var conf = new (int idex, string Key, string Path)[ElConfigTable.Rows.Count - 1];

            for (int rowI = 0; rowI < ElConfigTable.Rows.Count - 1; rowI++)
            {
                var row = ElConfigTable.Rows[rowI];
                conf[rowI] = (rowI, row.Cells[0].Value?.ToString() ?? string.Empty, row.Cells[1].Value?.ToString() ?? string.Empty);
            }

            return conf;
        }


        private XmlDocument XmlDocument;

        private IEnumerable<XmlNode> ToIter(XmlNodeList src)
        {
            for (var i = 0; i < src?.Count; i++)
            {
                yield return src[i]!;
            }
        }
    }
}
