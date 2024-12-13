using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace u3WindowsManager
{
    public class WindowsSelectionList : Form
    {
        private ListView listWindows;
        private Button btnSave;
        private Button btnCancel;

		
        public WindowsSelectionList()
        {
            InitializeComponent();
            listWindows.Items.Clear();
        }
		
        private void InitializeComponent()
        {
            ListViewItem listViewItem1 = new ListViewItem("window 1");
            ListViewItem listViewItem2 = new ListViewItem("window 2");
            ListViewItem listViewItem3 = new ListViewItem("window 3");
            listWindows = new ListView();
            btnSave = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // listWindows
            // 
            listWindows.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listWindows.CheckBoxes = true;
            listViewItem1.StateImageIndex = 0;
            listViewItem2.Checked = true;
            listViewItem2.StateImageIndex = 1;
            listViewItem3.StateImageIndex = 0;
            listWindows.Items.AddRange(new ListViewItem[] { listViewItem1, listViewItem2, listViewItem3 });
            listWindows.Location = new Point(12, 12);
            listWindows.Name = "listWindows";
            listWindows.Size = new Size(458, 252);
            listWindows.TabIndex = 0;
            listWindows.UseCompatibleStateImageBehavior = false;
            listWindows.View = View.List;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSave.DialogResult = DialogResult.OK;
            btnSave.Location = new Point(476, 12);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(158, 29);
            btnSave.TabIndex = 1;
            btnSave.Text = "Save selected";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(476, 47);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(158, 29);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // WindowsSelectionList
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(640, 277);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(listWindows);
            Name = "WindowsSelectionList";
            Text = "WindowsSelectionList";
            ResumeLayout(false);
        }

        public void AddItem(string item_name)
        {
            listWindows.Items.Add(item_name);
        }

        public List<string> SelectedItems()
        {
            List<string> selected = new List<string>();
            foreach (ListViewItem item in listWindows.Items)
            {
                if (item.Checked)
                    selected.Add(item.Text);
            }
            return selected;
        }

    }
}
