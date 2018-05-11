using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using TopazLib;
using System.IO;

namespace KH2Suite_ItemTable
{
    public partial class MainShit : Form
    {
        OpenFileDialog OpenFilez = new OpenFileDialog();
        Boolean TextChanging = false;

        public MainShit()
        {
            InitializeComponent();
        }

        private void GB_Item_Enter(object sender, EventArgs e)
        {

        }

        private void OpenShit_Click(object sender, EventArgs e)
        {
            OpenFilez.Filter = "General ITEM Table|*.item;*.bin|03system.bin|03system.bin";

            if (OpenFilez.ShowDialog() == DialogResult.OK)
            {
                if (OpenFilez.SafeFileName == "03system.bin")
                {
                    DialogResult Topaz3 = MessageBox.Show("This is a complete Kingdom Hearts 2 Final Mix system file. The item table will be extracted to your desktop from this file. Continue?", "Question #2", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (Topaz3 == DialogResult.Yes)
                    {
                        int ToReferOffset = 0x68;
                        int ToTakeOffset = 0x6C;

                        //We have to figure out if this is a PS2 file or a PS4 file
                        var IsPS2File = Encoding.UTF8.GetString(File.ReadAllBytes(OpenFilez.FileName).Take(4).ToArray()).Replace("\u0001", string.Empty) == "BAR" ? true : false;

                        if (!IsPS2File)
                        {
                            var IsPS4File = Encoding.UTF8.GetString(File.ReadAllBytes(OpenFilez.FileName).Skip(16).Take(4).ToArray()).Replace("\u0001", string.Empty) == "BAR" ? true : false;
                            if (IsPS4File)
                            {
                                ToReferOffset += 32;
                                ToTakeOffset += 16;
                            }
                        }

                        int ToRefer = BitConverter.ToInt32(File.ReadAllBytes(OpenFilez.FileName).Skip(ToReferOffset).Take(4).ToArray(), 0);
                        int ToTake = BitConverter.ToInt32(File.ReadAllBytes(OpenFilez.FileName).Skip(ToTakeOffset).Take(4).ToArray(), 0);
                       
                        byte[] GetFile = File.ReadAllBytes(OpenFilez.FileName).Skip(ToRefer).Take(ToTake).ToArray();

                        File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/ItemTableExtract" + ".bin", GetFile);
                        OpenFilez.FileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/ItemTableExtract" + ".bin";
                        MessageBox.Show("Operation Successful! Parsing the file...", "Success #0", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (EntryItemCB.Items.Count != 0 && EntryPropCB.Items.Count != 0)
                        {
                            EntryItemCB.Items.Clear();
                            EntryPropCB.Items.Clear();
                        }

                        ParseFile(OpenFilez.FileName, 1, false);
                        TextConverterIn(OpenFilez.FileName, 0);
                    }

                    else
                    {
                        MessageBox.Show("Operation Aborted by User!", "Failure #1", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                else
                {
                    if (EntryItemCB.Items.Count != 0 && EntryPropCB.Items.Count != 0)
                    {
                        EntryItemCB.Items.Clear();
                        EntryPropCB.Items.Clear();
                    }

                    ParseFile(OpenFilez.FileName, 1, false);
                    TextConverterIn(OpenFilez.FileName, 0);
                }

                foreach (Control lbl in this.Controls)
                {
                    lbl.Enabled = true;
                }
            }
        }

        private void EntryItemCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            ParseFile(OpenFilez.FileName, 1, false);
            TextConverterIn(OpenFilez.FileName, 0);
        }

        private void EntryPropCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            ParseFile(OpenFilez.FileName, 2, false);
            TextConverterIn(OpenFilez.FileName, 1);
        }

        private void Butt_IncItem_Click(object sender, EventArgs e)
        {
            byte[] New01 = File.ReadAllBytes(OpenFilez.FileName).Take(8 + 24 * EntryCount_Item).ToArray();
            File.WriteAllBytes(Path.GetTempPath() + "/File001.topaz", New01);

            byte[] New02 = File.ReadAllBytes(OpenFilez.FileName).Skip(8 + 24 * EntryCount_Item).ToArray();
            File.WriteAllBytes(Path.GetTempPath() + "/File002.topaz", New02);

            int Baboosh = EntryCount_Item;
            byte[] NewByteArray = new byte[24];
            Baboosh = Baboosh + 1;
            EntryItemCB.Items.Add(Baboosh);


            using (Stream stream = new FileStream(Path.GetTempPath() + "/File001.topaz", FileMode.Append, FileAccess.Write))
            {
                stream.Write(NewByteArray, 0, 24);
            }

            using (Stream stream = new FileStream(Path.GetTempPath() + "/File001.topaz", FileMode.OpenOrCreate))
            {
                stream.Seek(4, SeekOrigin.Begin);
                stream.Write(BitConverter.GetBytes(EntryCount_Item), 0, 4);
            }

            File.WriteAllBytes(OpenFilez.FileName, new Byte[] { 0 });

            using (Stream stream = new FileStream(OpenFilez.FileName, FileMode.OpenOrCreate))
            {
                byte[] Sup1 = File.ReadAllBytes(Path.GetTempPath() + "/File001.topaz");
                stream.Seek(0, SeekOrigin.Begin);
                stream.Write(Sup1, 0, Sup1.Length);

                byte[] Sup2 = File.ReadAllBytes(Path.GetTempPath() + "/File002.topaz");
                stream.Seek(0, SeekOrigin.End);
                stream.Write(Sup2, 0, Sup2.Length);
            }

            MessageBox.Show("Item Added Successfully!", "Success #4", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Butt_DecItem_Click(object sender, EventArgs e)
        {
            int Baboosh = EntryCount_Item;
            Baboosh = Baboosh - 1;
            EntryCount_Item = Baboosh;

            using (Stream stream = new FileStream(OpenFilez.FileName, FileMode.OpenOrCreate))
            {
                stream.Seek(4, SeekOrigin.Begin);
                stream.Write(BitConverter.GetBytes(EntryCount_Item), 0, 4);

                stream.Seek(8 + 24 * (EntryItemCB.Items.Count - 1), SeekOrigin.Begin);
                stream.Write(null, 0, 24);
            }

            EntryItemCB.Items.Remove(Baboosh + 1);
            MessageBox.Show("Items Decreased Successfully!", "Success #5", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Butt_IncProp_Click(object sender, EventArgs e)
        {
            String Topaz2 = null;
            DialogResult Topaz = MessageBox.Show("You have chosen to add a new Property Entry, would you like to define the said Entry ID?", "Question #1", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            byte[] NewByteArray = new byte[16];

            EntryCount_Prop = GeneralFunctions.readBytetoInt32(OpenFilez.FileName, 8 + 8 + 24 * EntryItemCB.Items.Count + 16, 0x04);      
            int Baboosh = EntryCount_Prop;

            if (Topaz == DialogResult.Yes)
            {
                Topaz2 = Interaction.InputBox("Please input the ID of your wish below.", "Interaction Box #1", "ID Here, A max of 4 Characters (2 Bytes) are allowed.");
                EntryPropCB.Items.Add(Topaz2);
            }

            else
            {
                byte[] Roger = new byte[] { 0 };
                EntryPropCB.Items.Add(BitConverter.ToString(Roger).Replace("-", ""));
            }

            using (Stream stream = new FileStream(OpenFilez.FileName, FileMode.OpenOrCreate))
            {
                stream.Seek(8 + 4 + 24 * EntryItemCB.Items.Count, SeekOrigin.Begin);
                stream.Write(BitConverter.GetBytes(Baboosh + 1), 0, 2);
                EntryCount_Prop = Baboosh;

                stream.Seek(0, SeekOrigin.End);
                stream.Write(NewByteArray, 0, 16);
            }

            EntryPropCB.SelectedItem = Topaz2;

            MessageBox.Show("Property Added Successfully!", "Success #2", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Butt_DecProp_Click(object sender, EventArgs e)
        {
            String Topaz2 = Interaction.InputBox("Please input the ID of your wish below.", "Interaction Box #1", "ID Here, A max of 4 Characters (2 Bytes) are allowed.");
            EntryPropCB.SelectedItem = Topaz2;

            int Baboosh = EntryCount_Prop;
            Baboosh = Baboosh - 1;
            EntryCount_Prop = Baboosh;;

            using (Stream stream = new FileStream(OpenFilez.FileName, FileMode.OpenOrCreate))
            {
                stream.Seek(8 + 4 + 24 * EntryItemCB.SelectedIndex, SeekOrigin.Begin);
                stream.Write(BitConverter.GetBytes(EntryCount_Item), 0, 4);

                stream.Seek(8 + 8 + 24 * EntryItemCB.SelectedIndex + 16 * EntryPropCB.SelectedIndex, SeekOrigin.Begin);
                stream.Write(null, 0, 16);
            }

            EntryPropCB.Items.Remove(Topaz2);
            MessageBox.Show("Property Deleted Successfully!", "Success #3", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Butt_InjectShit_Click(object sender, EventArgs e)
        {
            DialogResult Topaz3 = MessageBox.Show("The Injection of the item will be made on the file that is open. A backup of the said file will be made on your desktop beforehand. Continue?", "Question #2", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (Topaz3 == DialogResult.Yes)
            {
                Byte[] CopyPapa = File.ReadAllBytes(OpenFilez.FileName);
                File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + OpenFilez.SafeFileName + ".backup", CopyPapa);
                TextConverterOut(OpenFilez.FileName);

                if (PictID > 359)
                {
                    MessageBox.Show("Unknown Picture ID Detected! Must create a blank .IMD file... Click OK to create a file...", "Information #1", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    using (Stream stream3 = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/item-" + PictID.ToString() + ".imd", FileMode.OpenOrCreate))
                    {
                        stream3.Seek(0, SeekOrigin.Begin);
                        stream3.Write(new byte[] { 0x49, 0x4D, 0x47, 0x44, 0x00, 0x01, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x40, 0x40, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x80, 0x00, 0x80, 0x00, 0x08, 0x00, 0x08, 0x00, 0x00, 0x00, 0x13, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x10, 0x00, 0x10, 0x00, 0x01, 0x00, 0x00, 0x00, 0x05, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00 }, 0, 0x40);                         
                        stream3.Seek(0, SeekOrigin.End);
                        stream3.Write(new byte[0x4004], 0, 0x4004);
                        stream3.Seek(0, SeekOrigin.End);
                        stream3.Write(new byte[] {0x01, 0x01, 0x01, 0x01, 0x02, 0x02, 0x02, 0x02, 0x03, 0x03, 0x03, 0x03, 0x04, 0x04, 0x04, 0x04, 0x05, 0x05, 0x05, 0x05, 0x06, 0x06, 0x06, 0x06, 0x07, 0x07, 0x07, 0x07, 0x08, 0x08, 0x08, 0x08, 0x09, 0x09, 0x09, 0x09, 0x0A, 0x0A, 0x0A, 0x0A, 0x0B, 0x0B, 0x0B, 0x0B, 0x0C, 0x0C, 0x0C, 0x0C, 0x0D, 0x0D, 0x0D, 0x0D, 0x0E, 0x0E, 0x0E, 0x0E, 0x0F, 0x0F, 0x0F, 0x0F, 0x10, 0x10, 0x10, 0x10, 0x11, 0x11, 0x11, 0x11, 0x12, 0x12, 0x12, 0x12, 0x13, 0x13, 0x13, 0x13, 0x14, 0x14, 0x14, 0x14, 0x15, 0x15, 0x15, 0x15, 0x16, 0x16, 0x16, 0x16, 0x17, 0x17, 0x17, 0x17, 0x18, 0x18, 0x18, 0x18, 0x19, 0x19, 0x19, 0x19, 0x1A, 0x1A, 0x1A, 0x1A, 0x1B, 0x1B, 0x1B, 0x1B, 0x1C, 0x1C, 0x1C, 0x1C, 0x1D, 0x1D, 0x1D, 0x1D, 0x1E, 0x1E, 0x1E, 0x1E, 0x1F, 0x1F, 0x1F, 0x1F, 0x20, 0x20, 0x20, 0x20, 0x21, 0x21, 0x21, 0x21, 0x22, 0x22, 0x22, 0x22, 0x23, 0x23, 0x23, 0x23, 0x24, 0x24, 0x24, 0x24, 0x25, 0x25, 0x25, 0x25, 0x26, 0x26, 0x26, 0x26, 0x27, 0x27, 0x27, 0x27, 0x28, 0x28, 0x28, 0x28, 0x29, 0x29, 0x29, 0x29, 0x2A, 0x2A, 0x2A, 0x2A, 0x2B, 0x2B, 0x2B, 0x2B, 0x2C, 0x2C, 0x2C, 0x2C, 0x2D, 0x2D, 0x2D, 0x2D, 0x2E, 0x2E, 0x2E, 0x2E, 0x2F, 0x2F, 0x2F, 0x2F, 0x30, 0x30, 0x30, 0x30, 0x31, 0x31, 0x31, 0x31, 0x32, 0x32, 0x32, 0x32, 0x33, 0x33, 0x33, 0x33, 0x34, 0x34, 0x34, 0x34, 0x35, 0x35, 0x35, 0x35, 0x36, 0x36, 0x36, 0x36, 0x37, 0x37, 0x37, 0x37, 0x38, 0x38, 0x38, 0x38, 0x39, 0x39, 0x39, 0x39, 0x3A, 0x3A, 0x3A, 0x3A, 0x3B, 0x3B, 0x3B, 0x3B, 0x3C, 0x3C, 0x3C, 0x3C, 0x3D, 0x3D, 0x3D, 0x3D, 0x3E, 0x3E, 0x3E, 0x3E, 0x3F, 0x3F, 0x3F, 0x3F, 0x40, 0x40, 0x40, 0x40, 0x41, 0x41, 0x41, 0x41, 0x42, 0x42, 0x42, 0x42, 0x43, 0x43, 0x43, 0x43, 0x44, 0x44, 0x44, 0x44, 0x45, 0x45, 0x45, 0x45, 0x46, 0x46, 0x46, 0x46, 0x47, 0x47, 0x47, 0x47, 0x48, 0x48, 0x48, 0x48, 0x49, 0x49, 0x49, 0x49, 0x4A, 0x4A, 0x4A, 0x4A, 0x4B, 0x4B, 0x4B, 0x4B, 0x4C, 0x4C, 0x4C, 0x4C, 0x4D, 0x4D, 0x4D, 0x4D, 0x4E, 0x4E, 0x4E, 0x4E, 0x4F, 0x4F, 0x4F, 0x4F, 0x50, 0x50, 0x50, 0x50, 0x51, 0x51, 0x51, 0x51, 0x52, 0x52, 0x52, 0x52, 0x53, 0x53, 0x53, 0x53, 0x54, 0x54, 0x54, 0x54, 0x55, 0x55, 0x55, 0x55, 0x56, 0x56, 0x56, 0x56, 0x57, 0x57, 0x57, 0x57, 0x58, 0x58, 0x58, 0x58, 0x59, 0x59, 0x59, 0x59, 0x5A, 0x5A, 0x5A, 0x5A, 0x5B, 0x5B, 0x5B, 0x5B, 0x5C, 0x5C, 0x5C, 0x5C, 0x5D, 0x5D, 0x5D, 0x5D, 0x5E, 0x5E, 0x5E, 0x5E, 0x5F, 0x5F, 0x5F, 0x5F, 0x60, 0x60, 0x60, 0x60,
                            0x61, 0x61, 0x61, 0x61, 0x62, 0x62, 0x62, 0x62, 0x63, 0x63, 0x63, 0x63, 0x64, 0x64, 0x64, 0x64, 0x65, 0x65, 0x65, 0x65, 0x66, 0x66, 0x66, 0x66, 0x67, 0x67, 0x67, 0x67, 0x68, 0x68, 0x68, 0x68, 0x69, 0x69, 0x69, 0x69, 0x6A, 0x6A, 0x6A, 0x6A, 0x6B, 0x6B, 0x6B, 0x6B, 0x6C, 0x6C, 0x6C, 0x6C, 0x6D, 0x6D, 0x6D, 0x6D, 0x6E, 0x6E, 0x6E, 0x6E, 0x6F, 0x6F, 0x6F, 0x6F, 0x70, 0x70, 0x70, 0x70, 0x71, 0x71, 0x71, 0x71, 0x72, 0x72, 0x72, 0x72, 0x73, 0x73, 0x73, 0x73, 0x74, 0x74, 0x74, 0x74, 0x75, 0x75, 0x75, 0x75, 0x76, 0x76, 0x76, 0x76, 0x77, 0x77, 0x77, 0x77, 0x78, 0x78, 0x78, 0x78, 0x79, 0x79, 0x79, 0x79, 0x7A, 0x7A, 0x7A, 0x7A, 0x7B, 0x7B, 0x7B, 0x7B, 0x7C, 0x7C, 0x7C, 0x7C, 0x7D, 0x7D, 0x7D, 0x7D, 0x7E, 0x7E, 0x7E, 0x7E, 0x7F, 0x7F, 0x7F, 0x7F, 0x80, 0x80, 0x80, 0x80, 0x81, 0x81, 0x81, 0x81, 0x82, 0x82, 0x82, 0x82, 0x83, 0x83, 0x83, 0x83, 0x84, 0x84, 0x84, 0x84, 0x85, 0x85, 0x85, 0x85, 0x86, 0x86, 0x86, 0x86, 0x87, 0x87, 0x87, 0x87, 0x88, 0x88, 0x88, 0x88, 0x89, 0x89, 0x89, 0x89, 0x8A, 0x8A, 0x8A, 0x8A, 0x8B, 0x8B, 0x8B, 0x8B, 0x8C, 0x8C, 0x8C, 0x8C, 0x8D, 0x8D, 0x8D, 0x8D, 0x8E, 0x8E, 0x8E, 0x8E, 0x8F, 0x8F, 0x8F, 0x8F, 0x90, 0x90, 0x90, 0x90, 0x91, 0x91, 0x91, 0x91, 0x92, 0x92, 0x92, 0x92, 0x93, 0x93, 0x93, 0x93, 0x94, 0x94, 0x94, 0x94, 0x95, 0x95, 0x95, 0x95, 0x96, 0x96, 0x96, 0x96, 0x97, 0x97, 0x97, 0x97, 0x98, 0x98, 0x98, 0x98, 0x99, 0x99, 0x99, 0x99, 0x9A, 0x9A, 0x9A, 0x9A, 0x9B, 0x9B, 0x9B, 0x9B, 0x9C, 0x9C, 0x9C, 0x9C, 0x9D, 0x9D, 0x9D, 0x9D, 0x9E, 0x9E, 0x9E, 0x9E, 0x9F, 0x9F, 0x9F, 0x9F, 0xA0, 0xA0, 0xA0, 0xA0, 0xA1, 0xA1, 0xA1, 0xA1, 0xA2, 0xA2, 0xA2, 0xA2, 0xA3, 0xA3, 0xA3, 0xA3, 0xA4, 0xA4, 0xA4, 0xA4, 0xA5, 0xA5, 0xA5, 0xA5, 0xA6, 0xA6, 0xA6, 0xA6, 0xA7, 0xA7, 0xA7, 0xA7, 0xA8, 0xA8, 0xA8, 0xA8, 0xA9, 0xA9, 0xA9, 0xA9, 0xAA, 0xAA, 0xAA, 0xAA, 0xAB, 0xAB, 0xAB, 0xAB, 0xAC, 0xAC, 0xAC, 0xAC, 0xAD, 0xAD, 0xAD, 0xAD, 0xAE, 0xAE, 0xAE, 0xAE, 0xAF, 0xAF, 0xAF, 0xAF, 0xB0, 0xB0, 0xB0, 0xB0, 0xB1, 0xB1, 0xB1, 0xB1, 0xB2, 0xB2, 0xB2, 0xB2, 0xB3, 0xB3, 0xB3, 0xB3, 0xB4, 0xB4, 0xB4, 0xB4, 0xB5, 0xB5, 0xB5, 0xB5, 0xB6, 0xB6, 0xB6, 0xB6, 0xB7, 0xB7, 0xB7, 0xB7, 0xB8, 0xB8, 0xB8, 0xB8, 0xB9, 0xB9, 0xB9, 0xB9, 0xBA, 0xBA, 0xBA, 0xBA, 0xBB, 0xBB, 0xBB, 0xBB, 0xBC, 0xBC, 0xBC, 0xBC, 0xBD, 0xBD, 0xBD, 0xBD, 0xBE, 0xBE, 0xBE, 0xBE, 0xBF, 0xBF, 0xBF, 0xBF, 0xC0, 0xC0, 0xC0, 0xC0, 0xC1, 0xC1, 0xC1, 0xC1, 
                            0xC2, 0xC2, 0xC2, 0xC2, 0xC3, 0xC3, 0xC3, 0xC3, 0xC4, 0xC4, 0xC4, 0xC4, 0xC5, 0xC5, 0xC5, 0xC5, 0xC6, 0xC6, 0xC6, 0xC6, 0xC7, 0xC7, 0xC7, 0xC7, 0xC8, 0xC8, 0xC8, 0xC8, 0xC9, 0xC9, 0xC9, 0xC9, 0xCA, 0xCA, 0xCA, 0xCA, 0xCB, 0xCB, 0xCB, 0xCB, 0xCC, 0xCC, 0xCC, 0xCC, 0xCD, 0xCD, 0xCD, 0xCD, 0xCE, 0xCE, 0xCE, 0xCE, 0xCF, 0xCF, 0xCF, 0xCF, 0xD0, 0xD0, 0xD0, 0xD0, 0xD1, 0xD1, 0xD1, 0xD1, 0xD2, 0xD2, 0xD2, 0xD2, 0xD3, 0xD3, 0xD3, 0xD3, 0xD4, 0xD4, 0xD4, 0xD4, 0xD5, 0xD5, 0xD5, 0xD5, 0xD6, 0xD6, 0xD6, 0xD6, 0xD7, 0xD7, 0xD7, 0xD7, 0xD8, 0xD8, 0xD8, 0xD8, 0xD9, 0xD9, 0xD9, 0xD9, 0xDA, 0xDA, 0xDA, 0xDA, 0xDB, 0xDB, 0xDB, 0xDB, 0xDC, 0xDC, 0xDC, 0xDC, 0xDD, 0xDD, 0xDD, 0xDD, 0xDE, 0xDE, 0xDE, 0xDE, 0xDF, 0xDF, 0xDF, 0xDF, 0xE0, 0xE0, 0xE0, 0xE0, 0xE1, 0xE1, 0xE1, 0xE1, 0xE2, 0xE2, 0xE2, 0xE2, 0xE3, 0xE3, 0xE3, 0xE3, 0xE4, 0xE4, 0xE4, 0xE4, 0xE5, 0xE5, 0xE5, 0xE5, 0xE6, 0xE6, 0xE6, 0xE6, 0xE7, 0xE7, 0xE7, 0xE7, 0xE8, 0xE8, 0xE8, 0xE8, 0xE9, 0xE9, 0xE9, 0xE9, 0xEA, 0xEA, 0xEA, 0xEA, 0xEB, 0xEB, 0xEB, 0xEB, 0xEC, 0xEC, 0xEC, 0xEC, 0xED, 0xED, 0xED, 0xED, 0xEE, 0xEE, 0xEE, 0xEE, 0xEF, 0xEF, 0xEF, 0xEF, 0xF0, 0xF0, 0xF0, 0xF0, 0xF1, 0xF1, 0xF1, 0xF1, 0xF2, 0xF2, 0xF2, 0xF2, 0xF3, 0xF3, 0xF3, 0xF3, 0xF4, 0xF4, 0xF4, 0xF4, 0xF5, 0xF5, 0xF5, 0xF5, 0xF6, 0xF6, 0xF6, 0xF6, 0xF7, 0xF7, 0xF7, 0xF7, 0xF8, 0xF8, 0xF8, 0xF8, 0xF9, 0xF9, 0xF9, 0xF9, 0xFA, 0xFA, 0xFA, 0xFA, 0xFB, 0xFB, 0xFB, 0xFB, 0xFC, 0xFC, 0xFC, 0xFC, 0xFD, 0xFD, 0xFD, 0xFD, 0xFE, 0xFE, 0xFE, 0xFE, 0xFF, 0xFF, 0xFF, 0xFF,}, 0, 0x3FC);
                        stream3.Dispose();
                    }
                }

                if (NameStr > 0x5476 || 
                    DescStr > 0x5476 || 
                    NameStr < 0x01BA ||
                    DescStr < 0x01BA)
                {
                    DialogResult Topaz6 = MessageBox.Show("It seems that you have also declared a new Name and Description ID for your item, would you like to implement the said strings to a 'sys.bar' file?", "Question #3", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (Topaz6 == DialogResult.Yes)
                    {
                        OpenFileDialog OpenFilez1 = new OpenFileDialog();

                        OpenFilez1.Filter = "General String Table|sys.bar";

                        if (OpenFilez1.ShowDialog() == DialogResult.OK)
                        {
                            byte[] EntryCount_Str = File.ReadAllBytes(OpenFilez1.FileName).Skip(0x34).Take(0x02).ToArray();
                            int Kaboosh = BitConverter.ToInt16(EntryCount_Str, 0) + 2;
                            EntryCount_Str = BitConverter.GetBytes(Kaboosh);

                            byte[] New01 = File.ReadAllBytes(OpenFilez1.FileName).Skip(0x30).Take(8 * (BitConverter.ToInt32(EntryCount_Str, 0) - 1)).ToArray();
                            File.WriteAllBytes(Path.GetTempPath() + "/File003.topaz", New01);

                            byte[] New02 = File.ReadAllBytes(OpenFilez1.FileName).Skip(0x30 + 8 * (BitConverter.ToInt32(EntryCount_Str, 0) - 1)).ToArray();
                            File.WriteAllBytes(Path.GetTempPath() + "/File004.topaz", New02);

                            int Kabloosh = Kaboosh - 2;
                            int ToSkipShit = 4 + 8 * Kabloosh;
                            byte[] LastOffset = File.ReadAllBytes(Path.GetTempPath() + "/File003.topaz").Skip(ToSkipShit - 0x04).Take(0x04).ToArray();
                            int s = 0;

                            int CountWrite = BitConverter.ToInt32(File.ReadAllBytes(Path.GetTempPath() + "/File003.topaz").Skip(0x04).Take(0x04).ToArray(), 0) + 2;
                            using (Stream stream5 = new FileStream(Path.GetTempPath() + "/File003.topaz", FileMode.OpenOrCreate))
                            {
                                stream5.Seek(4, SeekOrigin.Begin);
                                stream5.Write(BitConverter.GetBytes(CountWrite), 0, 0x04);
                            }


                            for (int i = 0; i < ToSkipShit - 6; )
                            {
                                int Kaboomsh = BitConverter.ToInt32(File.ReadAllBytes(Path.GetTempPath() + "/File003.topaz").Skip(4 + 8 + i).Take(0x04).ToArray(), 0) + 16;

                                byte[] WriteShit = BitConverter.GetBytes(Kaboomsh);
                                using (Stream stream = new FileStream(Path.GetTempPath() + "/File003.topaz", FileMode.OpenOrCreate))
                                {
                                    stream.Seek(12 + i, SeekOrigin.Begin);
                                    stream.Write(WriteShit, 0, WriteShit.Length);
                                    i = i + 8;
                                    s++;
                                    stream.Dispose();
                                }
                            }


                                byte[] Longevity1 = File.ReadAllBytes(Path.GetTempPath() + "/File003.topaz");
                                byte[] Longevity2 = File.ReadAllBytes(Path.GetTempPath() + "/File004.topaz");
                                using (Stream stream2 = new FileStream(Path.GetTempPath() + "/File003.topaz", FileMode.Append, FileAccess.Write))
                                {
                                    int Kaboomsh = BitConverter.ToInt32(LastOffset, 0) + 8;
                                    stream2.Write(BitConverter.GetBytes(NameStr), 0, 2);
                                    stream2.Write(new byte[2], 0, 2);
                                    stream2.Write(BitConverter.GetBytes(Longevity1.Length + Longevity2.Length + 0xF), 0, 4);
                                    Kaboomsh = Kaboomsh + 8;
                                    stream2.Write(BitConverter.GetBytes(DescStr), 0, 2);
                                    stream2.Write(new byte[2], 0, 2);
                                    stream2.Write(BitConverter.GetBytes(Longevity1.Length + Longevity2.Length + 0xF), 0, 4);
                                    stream2.Dispose();
                                }

                                Longevity1 = File.ReadAllBytes(Path.GetTempPath() + "/File003.topaz");
                                Longevity2 = File.ReadAllBytes(Path.GetTempPath() + "/File004.topaz");
                                byte[] Final1 = File.ReadAllBytes(Path.GetTempPath() + "/File003.topaz");
                                byte[] Final2 = File.ReadAllBytes(Path.GetTempPath() + "/File004.topaz");
                                File.Delete(Path.GetTempPath() + "/DemiSys.bar");
                                using (Stream stream3 = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/DemiSys.bar", FileMode.OpenOrCreate))
                                {
                                    stream3.Seek(0, SeekOrigin.Begin);
                                    stream3.Write(new byte[] { 0x42, 0x41, 0x52, 0x01, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                                               0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x73, 0x79,
                                                               0x73, 0x00, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x18, 
                                                               0x00, 0x00, 0x00, 0x6D, 0x64, 0x5F, 0x6D, 0x00, 0x00, 0x00, 0x00, 
                                                               0x00, 0x00, 0x00, 0x00 }, 0, 0x30);
                                    stream3.Seek(0x1C, SeekOrigin.Begin);
                                    stream3.Write(BitConverter.GetBytes(Longevity1.Length + Longevity2.Length), 0, 0x04);
                                    stream3.Seek(0, SeekOrigin.End);
                                    stream3.Write(Final1, 0, Final1.Length);
                                    stream3.Seek(0, SeekOrigin.End);
                                    stream3.Write(Final2, 0, Final2.Length);
                                    stream3.Seek(0x28, SeekOrigin.Begin);
                                    stream3.Write(BitConverter.GetBytes(Longevity1.Length + Longevity2.Length + 0x30), 0, 0x04);
                                }
                        }
                        MessageBox.Show("Operation Success! The item has been imported/changed/edited, and the new sys.bar had been made on your Desktop with the name 'DemiSys.bar'", "Failure #1", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }

                    else
                    {
                        MessageBox.Show("Operation Success!", "Failure #1", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                }
            }

            else
            {
                MessageBox.Show("Operation Aborted by User!", "Failure #1", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BTxt_ItemType_TextChanged(object sender, EventArgs e)
        {
            TextChanging = true;
        }

        private void CBox_Types_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TextChanging == false)
            {
                switch (CBox_Types.SelectedIndex)
                {
                    case 0:
                        ItemType = 0000;
                        break;
                    case 1:
                        ItemType = 0x0001;
                        break;
                    case 2:
                        ItemType = 0x0200;
                        break;
                    case 3:
                        ItemType = 0x0201;
                        break;
                    case 4:
                        ItemType = 0x0002;
                        break;
                    case 5:
                        ItemType = 0x0003;
                        break;
                    case 6:
                        ItemType = 0x0004;
                        break;
                    case 7:
                        ItemType = 0x0005;
                        break;
                    case 8:
                        ItemType = 0x0006;
                        break;
                    case 9:
                        ItemType = 0x0007;
                        break;
                    case 10:
                        ItemType = 0x0008;
                        break;
                    case 11:
                        ItemType = 0x0009;
                        break;
                    case 12:
                        ItemType = 0x000A;
                        break;
                    case 13:
                        ItemType = 0x000B;
                        break;
                    case 14:
                        ItemType = 0x000C;
                        break;
                    case 15:
                        ItemType = 0x000D;
                        break;
                    case 16:
                        ItemType = 0x000E;
                        break;
                    case 17:
                        ItemType = 0x000F;
                        break;
                    case 18:
                        ItemType = 0x0010;
                        break;
                    case 19:
                        ItemType = 0x0011;
                        break;
                    case 20:
                        ItemType = 0x0012;
                        break;
                    case 21:
                        ItemType = 0x0013;
                        break;
                    case 22:
                        ItemType = 0x0114;
                        break;
                    case 23:
                        ItemType = 0x0115;
                        break;
                    case 24:
                        ItemType = 0x0116;
                        break;
                    case 25:
                        ItemType = 0x0117;
                        break;
                }

                TextConverterIn(OpenFilez.FileName, 0);
            }
        }

        private void Butt_SwapEnd_Click(object sender, EventArgs e)
        {
            DialogResult Topaz5 = MessageBox.Show("This will completely shift the Endian values of your file to make it 2.5(PS3) Compatible. No backups will be taken, this process is irreversable. Continue?", "Question #3", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (Topaz5 == DialogResult.Yes)
            {
                byte[] DemFile = File.ReadAllBytes(OpenFilez.FileName);

                for (int i = 0; i < DemFile.Length; )
                {
                    byte[] WriteEndian = File.ReadAllBytes(OpenFilez.FileName).Skip(i).Take(0x04).Reverse().ToArray();
                    using (Stream stream = new FileStream(OpenFilez.FileName, FileMode.OpenOrCreate))
                    {
                        stream.Seek(i, SeekOrigin.Begin);
                        stream.Write(WriteEndian, 0, WriteEndian.Length);
                        i = i + 4;
                    }
                }
                MessageBox.Show("Success! Now your file is Big Endian!", "Success #4", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
    }
}
