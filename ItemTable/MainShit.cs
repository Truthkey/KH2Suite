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
                        int ToRefer = BitConverter.ToInt32(File.ReadAllBytes(OpenFilez.FileName).Skip(0x68).Take(4).ToArray(), 0);
                        int ToTake = BitConverter.ToInt32(File.ReadAllBytes(OpenFilez.FileName).Skip(0x6C).Take(4).ToArray(), 0);
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
            byte[] New01 = File.ReadAllBytes(OpenFilez.FileName).Take(8 + 24 * BitConverter.ToInt32(EntryCount_Item, 0)).ToArray();
            File.WriteAllBytes(Path.GetTempPath() + "/File001.topaz", New01);

            byte[] New02 = File.ReadAllBytes(OpenFilez.FileName).Skip(8 + 24 * BitConverter.ToInt32(EntryCount_Item, 0)).ToArray();
            File.WriteAllBytes(Path.GetTempPath() + "/File002.topaz", New02);

            int Baboosh = BitConverter.ToInt32(EntryCount_Item, 0);
            byte[] NewByteArray = new byte[24];
            Baboosh = Baboosh + 1;
            EntryItemCB.Items.Add(Baboosh);
            EntryCount_Item = BitConverter.GetBytes(Baboosh);


            using (Stream stream = new FileStream(Path.GetTempPath() + "/File001.topaz", FileMode.Append, FileAccess.Write))
            {
                stream.Write(NewByteArray, 0, 24);
            }

            using (Stream stream = new FileStream(Path.GetTempPath() + "/File001.topaz", FileMode.OpenOrCreate))
            {
                stream.Seek(4, SeekOrigin.Begin);
                stream.Write(EntryCount_Item, 0, 4);
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
            int Baboosh = BitConverter.ToInt32(EntryCount_Item, 0);
            Baboosh = Baboosh - 1;
            EntryCount_Item = BitConverter.GetBytes(Baboosh);
            Array.Reverse(EntryCount_Item);

            using (Stream stream = new FileStream(OpenFilez.FileName, FileMode.OpenOrCreate))
            {
                stream.Seek(4, SeekOrigin.Begin);
                stream.Write(EntryCount_Item, 0, 4);

                stream.Seek(8 + 24 * (EntryItemCB.Items.Count - 1), SeekOrigin.Begin);
                stream.Write(null, 0, 24);
            }

            EntryItemCB.Items.Remove(Baboosh + 1);
            MessageBox.Show("Items Decreased Successfully!", "Success #5", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Butt_IncProp_Click(object sender, EventArgs e)
        {
            DialogResult Topaz = MessageBox.Show("You have chosen to add a new Property Entry, would you like to define the said Entry ID?", "Question #1", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            byte[] NewByteArray = new byte[16];

            if (Topaz == DialogResult.Yes)
            {
                int Baboosh = 0;
                String Topaz2 = Interaction.InputBox("Please input the ID of your wish below.", "Interaction Box #1", "ID Here, A max of 4 Characters (2 Bytes) are allowed.");
                EntryPropCB.Items.Add(Topaz2);
                Baboosh = Baboosh + 1;
                EntryCount_Prop = BitConverter.GetBytes(Baboosh);
                Array.Reverse(EntryCount_Prop);
                EntryPropCB.SelectedItem = Topaz2;
            }

            else
            {
                byte[] Roger = new byte[] { 0 };
                int Baboosh = BitConverter.ToInt32(EntryCount_Prop, 0);
                Baboosh = Baboosh + 1;
                Roger = BitConverter.GetBytes(Baboosh);
                EntryPropCB.Items.Add(BitConverter.ToString(Roger).Replace("-", ""));
                EntryCount_Prop = BitConverter.GetBytes(Baboosh);
                Array.Reverse(EntryCount_Prop);
            }

            using (Stream stream = new FileStream(OpenFilez.FileName, FileMode.OpenOrCreate))
            {
                stream.Seek(8 + 4 + 24 * EntryItemCB.SelectedIndex, SeekOrigin.Begin);
                stream.Write(EntryCount_Item, 0, 4);

                stream.Seek(0, SeekOrigin.End);
                stream.Write(NewByteArray, 0, 16);
            }

            MessageBox.Show("Property Added Successfully!", "Success #2", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Butt_DecProp_Click(object sender, EventArgs e)
        {
            String Topaz2 = Interaction.InputBox("Please input the ID of your wish below.", "Interaction Box #1", "ID Here, A max of 4 Characters (2 Bytes) are allowed.");
            EntryPropCB.SelectedItem = Topaz2;

            int Baboosh = BitConverter.ToInt32(EntryCount_Prop, 0);
            Baboosh = Baboosh - 1;
            EntryCount_Prop = BitConverter.GetBytes(Baboosh);
            Array.Reverse(EntryCount_Prop);

            using (Stream stream = new FileStream(OpenFilez.FileName, FileMode.OpenOrCreate))
            {
                stream.Seek(8 + 4 + 24 * EntryItemCB.SelectedIndex, SeekOrigin.Begin);
                stream.Write(EntryCount_Item, 0, 4);

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
                File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + OpenFilez.SafeFileName + ".backup", CopyPapa);
                TextConverterOut(OpenFilez.FileName);
                MessageBox.Show("Operation Successful!", "Success #1", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        ItemType = new byte[] { 00, 00 };
                        break;
                    case 1:
                        ItemType = new byte[] { 0x00, 0x01 };
                        break;
                    case 2:
                        ItemType = new byte[] { 0x02, 0x00 };
                        break;
                    case 3:
                        ItemType = new byte[] { 0x02, 0x01 };
                        break;
                    case 4:
                        ItemType = new byte[] { 0x00, 0x02 };
                        break;
                    case 5:
                        ItemType = new byte[] { 0x00, 0x03 };
                        break;
                    case 6:
                        ItemType = new byte[] { 0x00, 0x04 };
                        break;
                    case 7:
                        ItemType = new byte[] { 0x00, 0x05 };
                        break;
                    case 8:
                        ItemType = new byte[] { 0x00, 0x06 };
                        break;
                    case 9:
                        ItemType = new byte[] { 0x00, 0x07 };
                        break;
                    case 10:
                        ItemType = new byte[] { 0x00, 0x08 };
                        break;
                    case 11:
                        ItemType = new byte[] { 0x00, 0x09 };
                        break;
                    case 12:
                        ItemType = new byte[] { 0x00, 0x0A };
                        break;
                    case 13:
                        ItemType = new byte[] { 0x00, 0x0B };
                        break;
                    case 14:
                        ItemType = new byte[] { 0x00, 0x0C };
                        break;
                    case 15:
                        ItemType = new byte[] { 0x00, 0x0D };
                        break;
                    case 16:
                        ItemType = new byte[] { 0x00, 0x0E };
                        break;
                    case 17:
                        ItemType = new byte[] { 0x00, 0x0F };
                        break;
                    case 18:
                        ItemType = new byte[] { 0x00, 0x10 };
                        break;
                    case 19:
                        ItemType = new byte[] { 0x00, 0x11 };
                        break;
                    case 20:
                        ItemType = new byte[] { 0x00, 0x12 };
                        break;
                    case 21:
                        ItemType = new byte[] { 0x00, 0x13 };
                        break;
                    case 22:
                        ItemType = new byte[] { 0x01, 0x14 };
                        break;
                    case 23:
                        ItemType = new byte[] { 0x01, 0x15 };
                        break;
                    case 24:
                        ItemType = new byte[] { 0x01, 0x16 };
                        break;
                    case 25:
                        ItemType = new byte[] { 0x01, 0x17 };
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
