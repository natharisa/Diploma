using System.Windows.Forms;
using Services;

namespace UI
{
    public static class TranslationApplier
    {
        public static void Apply(Control root)
        {
            if (root == null)
            {
                return;
            }

            ApplyControl(root);

            foreach (Control child in root.Controls)
            {
                Apply(child);
            }
        }

        public static void ApplyMenu(ToolStrip toolStrip)
        {
            if (toolStrip == null)
            {
                return;
            }

            foreach (ToolStripItem item in toolStrip.Items)
            {
                ApplyToolStripItem(item);
            }
        }

        private static void ApplyControl(Control control)
        {
            string key = control.Tag as string;
            if (!string.IsNullOrWhiteSpace(key))
            {
                control.Text = LanguageManager.Instance.Translate(key);
            }

            DataGridView dataGridView = control as DataGridView;
            if (dataGridView != null)
            {
                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    string columnKey = column.Tag as string;
                    if (!string.IsNullOrWhiteSpace(columnKey))
                    {
                        column.HeaderText = LanguageManager.Instance.Translate(columnKey);
                    }
                }
            }

            ListView listView = control as ListView;
            if (listView != null)
            {
                foreach (ColumnHeader column in listView.Columns)
                {
                    string columnKey = column.Tag as string;
                    if (!string.IsNullOrWhiteSpace(columnKey))
                    {
                        column.Text = LanguageManager.Instance.Translate(columnKey);
                    }
                }
            }
        }

        private static void ApplyToolStripItem(ToolStripItem item)
        {
            string key = item.Tag as string;
            if (!string.IsNullOrWhiteSpace(key))
            {
                item.Text = LanguageManager.Instance.Translate(key);
            }

            ToolStripDropDownItem dropDownItem = item as ToolStripDropDownItem;
            if (dropDownItem == null)
            {
                return;
            }

            foreach (ToolStripItem child in dropDownItem.DropDownItems)
            {
                ApplyToolStripItem(child);
            }
        }
    }
}
