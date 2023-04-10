using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomZedGraphControlApp
{
    public partial class SettingView : Form
    {
        public SettingView(int[] ini)
        {
            InitializeComponent();

            checkBox6.Checked = Convert.ToBoolean(ini[0] & 0x1);
            checkBox7.Checked = Convert.ToBoolean(ini[0] & 0x2);

            checkBox8.Checked = Convert.ToBoolean(ini[1] & 0x1);
            checkBox9.Checked = Convert.ToBoolean(ini[1] & 0x2);

            checkBox10.Checked = Convert.ToBoolean(ini[2] & 0x1);
            checkBox11.Checked = Convert.ToBoolean(ini[2] & 0x2);

            checkBox12.Checked = Convert.ToBoolean(ini[3] & 0x1);
            checkBox13.Checked = Convert.ToBoolean(ini[3] & 0x2);

            checkBox14.Checked = Convert.ToBoolean(ini[4] & 0x1);
            checkBox15.Checked = Convert.ToBoolean(ini[4] & 0x2);
        }

        public int[] setting = new int[5];

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            setting[0] = (Convert.ToInt32(checkBox7.Checked) << 1) | Convert.ToInt32(checkBox6.Checked);
            setting[1] = (Convert.ToInt32(checkBox9.Checked) << 1) | Convert.ToInt32(checkBox8.Checked);
            setting[2] = (Convert.ToInt32(checkBox11.Checked) << 1) | Convert.ToInt32(checkBox10.Checked);
            setting[3] = (Convert.ToInt32(checkBox13.Checked) << 1) | Convert.ToInt32(checkBox12.Checked);
            setting[4] = (Convert.ToInt32(checkBox15.Checked) << 1) | Convert.ToInt32(checkBox14.Checked);

            DialogResult = DialogResult.OK;
        }
    }
}
