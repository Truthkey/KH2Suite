using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace KH2Suite_IMDCreator
{
    public partial class MainShit : Form
    {
        public MainShit()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] HeaderChars = new byte[4]
            {
                0x49, 
                0x4D, 
                0x47, 
                0x44
            };

            int Size = int.Parse(TBoxWidth.Text) * int.Parse(TBoxHeight.Text);
            int OffsetPalette = Size + 0x40;

            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/NewImage.imd");
            using (FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/NewImage.imd", FileMode.Append, FileAccess.Write))
            {
                fs.Write(HeaderChars, 0, 4);
                fs.Write(new byte[] { 0x00, 0x01, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, }, 0, 8);
                fs.Write(BitConverter.GetBytes(Size), 0, 4);
                fs.Write(BitConverter.GetBytes(OffsetPalette), 0, 4);
                fs.Write(new byte[] { 0x00, 0x04, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF }, 0, 8);
                fs.Write(BitConverter.GetBytes(short.Parse(TBoxWidth.Text)), 0, 2);
                fs.Write(BitConverter.GetBytes(short.Parse(TBoxHeight.Text)), 0, 2);

                fs.Write(new byte[] {0x07, 0x00, 0x07, 0x00, 0x02, 0x00, 0x13, 0x00, 
                                     0xFF, 0xFF, 0xFF, 0xFF, 0x10, 0x00, 0x10, 0x00, 
                                     0x01, 0x00, 0x00, 0x00, 0x05, 0x00, 0x03, 0x00, 
                                     0x00, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00}, 0, 32);

                fs.Write(new byte[Size + 0x400], 0, Size + 0x400);

            }

            MessageBox.Show("Done!", "Success #1", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
