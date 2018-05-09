using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;

namespace KH2Suite_ItemTable
{
    public partial class MainShit
    {
        // Basic Parameters
        Byte[] EntryCount_Item; // Little Endian
        Byte[] EntryCount_Prop; // Little Endian

        // Params of Item
        Byte[] ItemID;
        Byte[] ItemType; // Hard-Coded.
        Byte[] ItemFlag; // Varies, changes linked to ItemType
        Byte[] Breakdown1; // For the Gem Sub-Type
        Byte[] Breakdown2; // For the Gem Sub-Type
        Byte[] PropLink; // Linked to Sub-Table
        Byte[] NameStr; // Linked to msg/jp/sys.bar
        Byte[] DescStr; // Linked to msg/jp/sys.bar
        Byte[] NoEffect; // Literally, changing this on any item, did nothing.
        Byte[] CmmdLink; // Linked to 03system.bin/cmd[0x02].bin
        Byte[] SlotNum;
        Byte[] PictID; // Linked to itempic/item-xxx.imd
        Byte PictID2; // Do not, EVER, use this.
        Byte Unused0;
        Byte[] IconID; // Linked to msg/jp/fontimage.bin

        // Params of Prop
        Byte[] EntryID; // Linked To Sub-Table - PropLink
        Byte[] AbilityID; // Linked to Sub-Table - ItemID
        Byte[] AttackValue;
        Byte[] MagicValue;
        Byte[] DefenseValue;
        Byte[] APValue;
        Byte Unused1;
        Byte[] FireVulnerability; // Is % Based -- 100% Resist at 0%, 0% Resist at 100%.
        Byte[] IceVulnerability; // Is % Based -- 100% Resist at 0%, 0% Resist at 100%.
        Byte[] LightningVulnerability; // Is % Based -- 100% Resist at 0%, 0% Resist at 100%.
        Byte[] DarkVulnerability; // Is % Based -- 100% Resist at 0%, 0% Resist at 100%.
        Byte[] Unused2;

        public void ParseFile(String InputFile, Int32 FileID, Boolean is03System)
        {
            if (is03System == false)
            {
                if (FileID == 1)
                {
                    byte[] Entry = File.ReadAllBytes(InputFile).Skip(4).Take(4).ToArray();

                    if (EntryCount_Item == null)
                        EntryCount_Item = Entry;

                    int Baboozal = 0;
                    if (EntryItemCB.Items.Count == 0)
                    {
                        while (Baboozal < BitConverter.ToInt32(Entry, 0))
                        {
                            EntryItemCB.Items.Add(Baboozal);
                            Baboozal++;
                        }
                    }

                    PBar_ItemCap.Value = EntryItemCB.Items.Count;

                    ItemID = File.ReadAllBytes(InputFile).Skip(8 + 24 * EntryItemCB.SelectedIndex).Take(2).Reverse().ToArray();
                    ItemType = File.ReadAllBytes(InputFile).Skip(8 + 24 * EntryItemCB.SelectedIndex + 2).Take(2).Reverse().ToArray();
                    ItemFlag = File.ReadAllBytes(InputFile).Skip(8 + 24 * EntryItemCB.SelectedIndex + 4).Take(2).Reverse().ToArray();
                    Breakdown1 = File.ReadAllBytes(InputFile).Skip(8 + 24 * EntryItemCB.SelectedIndex + 4).Take(1).ToArray();
                    Breakdown2 = File.ReadAllBytes(InputFile).Skip(8 + 24 * EntryItemCB.SelectedIndex + 5).Take(1).ToArray();
                    PropLink = File.ReadAllBytes(InputFile).Skip(8 + 24 * EntryItemCB.SelectedIndex + 6).Take(2).Reverse().ToArray();
                    NameStr = File.ReadAllBytes(InputFile).Skip(8 + 24 * EntryItemCB.SelectedIndex + 8).Take(2).Reverse().ToArray();
                    DescStr = File.ReadAllBytes(InputFile).Skip(8 + 24 * EntryItemCB.SelectedIndex + 10).Take(2).Reverse().ToArray();
                    CmmdLink = File.ReadAllBytes(InputFile).Skip(8 + 24 * EntryItemCB.SelectedIndex + 16).Take(2).Reverse().ToArray();
                    SlotNum = File.ReadAllBytes(InputFile).Skip(8 + 24 * EntryItemCB.SelectedIndex + 18).Take(2).Reverse().ToArray();
                    PictID = File.ReadAllBytes(InputFile).Skip(8 + 24 * EntryItemCB.SelectedIndex + 20).Take(1).ToArray();
                    IconID = File.ReadAllBytes(InputFile).Skip(8 + 24 * EntryItemCB.SelectedIndex + 23).Take(1).ToArray();


                    if (EntryCount_Prop == null)
                        EntryCount_Prop = File.ReadAllBytes(InputFile).Skip(8 + 4 + 24 * EntryItemCB.Items.Count).Take(2).ToArray();
                    int ToSkip = 8 + 8 + 24 * EntryItemCB.Items.Count;


                    int Baboozal2 = 0;
                    byte[] Roger = new byte[] {0};
                    if (EntryPropCB.Items.Count == 0)
                    {
                        while (Baboozal2 < BitConverter.ToInt16(EntryCount_Prop, 0))
                        {
                            Roger = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.Items.Count).Take(2).Reverse().ToArray();
                            EntryPropCB.Items.Add(BitConverter.ToString(Roger).Replace("-", ""));
                            Baboozal2 = Baboozal2 + 0x01;
                        }
                    }

                    PBar_PropCap.Value = EntryPropCB.Items.Count;

                    EntryID = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex).Take(2).Reverse().ToArray();
                    AbilityID = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex + 2).Take(2).Reverse().ToArray();
                    AttackValue = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex + 4).Take(1).ToArray();
                    MagicValue = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex + 5).Take(1).ToArray();
                    DefenseValue = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex + 6).Take(1).ToArray();
                    APValue = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex + 7).Take(1).ToArray();
                    FireVulnerability = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex + 9).Take(1).ToArray();
                    IceVulnerability = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex + 10).Take(1).ToArray();
                    LightningVulnerability = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex + 11).Take(1).ToArray();
                    DarkVulnerability = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex + 12).Take(1).ToArray();

                }

                else if (FileID == 2)
                {

                    if (EntryCount_Prop == null)
                        EntryCount_Prop = File.ReadAllBytes(InputFile).Skip(8 + 4 + 24 * EntryItemCB.Items.Count).Take(2).ToArray(); 
                    int ToSkip = 8 + 8 + 24 * EntryItemCB.Items.Count;


                    int Baboozal2 = 0x00;
                    if (EntryPropCB.Items.Count == 0)
                    {
                        while (Baboozal2 < BitConverter.ToInt32(EntryCount_Prop, 0))
                        {
                            EntryPropCB.Items.Add(Baboozal2.ToString("X"));
                            Baboozal2 = Baboozal2 + 0x01;
                        }

                    }

                    EntryID = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex).Take(2).Reverse().ToArray();
                    AbilityID = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex + 2).Take(2).Reverse().ToArray();
                    AttackValue = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex + 4).Take(1).ToArray();
                    MagicValue = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex + 5).Take(1).ToArray();
                    DefenseValue = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex + 6).Take(1).ToArray();
                    APValue = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex + 7).Take(1).ToArray();
                    FireVulnerability = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex + 9).Take(1).ToArray();
                    IceVulnerability = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex + 10).Take(1).ToArray();
                    LightningVulnerability = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex + 11).Take(1).ToArray();
                    DarkVulnerability = File.ReadAllBytes(InputFile).Skip(ToSkip + 16 * EntryPropCB.SelectedIndex + 12).Take(1).ToArray();

                }
            }
        }

        public void TextConverterIn(String FileName, Int16 Calltype)
        {
            // Values to the appropriate boxes.
            BTxt_ItemID.Text = BitConverter.ToString(ItemID, 0).Replace("-", "");
            BTxt_ItemType.Text = BitConverter.ToString(ItemType, 0).Replace("-", "");
            BTxt_ItemFlag.Text = BitConverter.ToString(ItemFlag, 0).Replace("-", "");
            BTxt_PropLink.Text = BitConverter.ToString(PropLink, 0).Replace("-", "");
            BTxt_NameStr.Text = BitConverter.ToString(NameStr, 0).Replace("-", "");
            BTxt_DescStr.Text = BitConverter.ToString(DescStr, 0).Replace("-", "");
            BTxt_CmmdLink.Text = BitConverter.ToString(CmmdLink, 0).Replace("-", "");
            BTxt_SlotNum.Text = BitConverter.ToString(SlotNum, 0).Replace("-", "");
            BTxt_PictID.Text = BitConverter.ToString(PictID, 0).Replace("-", "");
            BTxt_IconID.Text = BitConverter.ToString(IconID, 0).Replace("-", "");

            BTxt_PropID.Text = BitConverter.ToString(EntryID, 0).Replace("-", "");
            BTxt_AbilityLink.Text = BitConverter.ToString(AbilityID, 0).Replace("-", "");
            BTxt_ATK.Text = BitConverter.ToString(AttackValue, 0).Replace("-", "");
            BTxt_MAG.Text = BitConverter.ToString(MagicValue, 0).Replace("-", "");
            BTxt_DEF.Text = BitConverter.ToString(DefenseValue, 0).Replace("-", "");
            BTxt_AP.Text = BitConverter.ToString(APValue, 0).Replace("-", "");
            BTxt_FireVul.Text = BitConverter.ToString(FireVulnerability, 0).Replace("-", "");
            BTxt_IceVul.Text = BitConverter.ToString(IceVulnerability, 0).Replace("-", "");
            BTxt_ThunderVul.Text = BitConverter.ToString(LightningVulnerability, 0).Replace("-", "");
            BTxt_DarkVul.Text = BitConverter.ToString(DarkVulnerability, 0).Replace("-", "");

            if (Calltype == 0)
            {
                EntryPropCB.Enabled = true;
                BTxt_PropLink.Enabled = true;
                BTxt_GemClass.Enabled = false;
                BTxt_GemType.Enabled = false;
                BTxt_ItemFlag.Enabled = true;
                TXT_FlagVary.Text = "Item Flag (May Vary):";
                TXT_PropVary.Text = "Prop Link (May Vary):";

                Byte[] Alakazam2 = ItemType;

                switch (BitConverter.ToInt16(Alakazam2, 0))
                {
                    case 0x0000: // Equipable Item
                        CBox_Types.SelectedIndex = 0;
                        EntryPropCB.Enabled = false;
                        EntryPropCB.SelectedIndex = 0;
                        BTxt_PropLink.Enabled = true;
                        TXT_FlagVary.Text = "Item Recover Value:";
                        TXT_PropVary.Text = "Item Type:";
                        break;

                    case 0x0100: // Menu Item
                        CBox_Types.SelectedIndex = 1;
                        EntryPropCB.Enabled = false;
                        EntryPropCB.SelectedIndex = 0;
                        BTxt_PropLink.Enabled = true;
                        TXT_FlagVary.Text = "Item Recover Value:";
                        TXT_PropVary.Text = "Item Type:";
                        break;

                    case 0x0002: // Equippable Mega Item
                        CBox_Types.SelectedIndex = 2;
                        EntryPropCB.Enabled = false;
                        EntryPropCB.SelectedIndex = 0;
                        BTxt_PropLink.Enabled = true;
                        TXT_FlagVary.Text = "Item Recover Value:";
                        TXT_PropVary.Text = "Item Type:";
                        break;

                    case 0x0102: // Mega Menu Item
                        CBox_Types.SelectedIndex = 3;
                        EntryPropCB.Enabled = false;
                        EntryPropCB.SelectedIndex = 0;
                        BTxt_PropLink.Enabled = true;
                        TXT_FlagVary.Text = "Item Recover Value:";
                        TXT_PropVary.Text = "Item Type:";
                        break;

                    case 0x0200: // Keyblade
                        CBox_Types.SelectedIndex = 4;
                        EntryPropCB.Enabled = true;
                        EntryPropCB.SelectedItem = BTxt_PropLink.Text;
                        BTxt_PropLink.Enabled = true;
                        BTxt_GemClass.Enabled = false;
                        BTxt_GemType.Enabled = false;
                        BTxt_ItemFlag.Enabled = true;
                        TXT_FlagVary.Text = "Model ID:";
                        TXT_PropVary.Text = "Prop Link (May Vary):";
                        break;

                    case 0x0300: // Staff
                        CBox_Types.SelectedIndex = 5;
                        EntryPropCB.Enabled = true;
                        EntryPropCB.SelectedItem = BTxt_PropLink.Text;
                        BTxt_PropLink.Enabled = true;
                        BTxt_GemClass.Enabled = false;
                        BTxt_GemType.Enabled = false;
                        BTxt_ItemFlag.Enabled = true;
                        TXT_FlagVary.Text = "Model ID:";
                        TXT_PropVary.Text = "Prop Link (May Vary):";
                        break;

                    case 0x0400: // Shield
                        CBox_Types.SelectedIndex = 6;
                        EntryPropCB.Enabled = true;
                        EntryPropCB.SelectedItem = BTxt_PropLink.Text;
                        BTxt_PropLink.Enabled = true;
                        BTxt_GemClass.Enabled = false;
                        BTxt_GemType.Enabled = false;
                        BTxt_ItemFlag.Enabled = true;
                        TXT_FlagVary.Text = "Model ID:";
                        TXT_PropVary.Text = "Prop Link (May Vary):";
                        break;

                    case 0x0500:
                        CBox_Types.SelectedIndex = 7;
                        EntryPropCB.Enabled = true;
                        EntryPropCB.SelectedItem = BTxt_PropLink.Text;
                        BTxt_PropLink.Enabled = true;
                        BTxt_GemClass.Enabled = false;
                        BTxt_GemType.Enabled = false;
                        BTxt_ItemFlag.Enabled = true;
                        TXT_FlagVary.Text = "Model ID:";
                        TXT_PropVary.Text = "Prop Link (May Vary):";
                        break;

                    case 0x0600:
                        CBox_Types.SelectedIndex = 8;
                        EntryPropCB.Enabled = true;
                        EntryPropCB.SelectedItem = BTxt_PropLink.Text;
                        BTxt_PropLink.Enabled = true;
                        BTxt_GemClass.Enabled = false;
                        BTxt_GemType.Enabled = false;
                        BTxt_ItemFlag.Enabled = true;
                        TXT_FlagVary.Text = "Model ID:";
                        TXT_PropVary.Text = "Prop Link (May Vary):";
                        break;

                    case 0x0700:
                        CBox_Types.SelectedIndex = 9;
                        EntryPropCB.Enabled = true;
                        EntryPropCB.SelectedItem = BTxt_PropLink.Text;
                        BTxt_PropLink.Enabled = true;
                        BTxt_GemClass.Enabled = false;
                        BTxt_GemType.Enabled = false;
                        BTxt_ItemFlag.Enabled = true;
                        TXT_FlagVary.Text = "Model ID:";
                        TXT_PropVary.Text = "Prop Link (May Vary):";
                        break;

                    case 0x0800:
                        CBox_Types.SelectedIndex = 10;
                        EntryPropCB.Enabled = true;
                        EntryPropCB.SelectedItem = BTxt_PropLink.Text;
                        BTxt_PropLink.Enabled = true;
                        BTxt_GemClass.Enabled = false;
                        BTxt_GemType.Enabled = false;
                        BTxt_ItemFlag.Enabled = true;
                        TXT_FlagVary.Text = "Model ID:";
                        TXT_PropVary.Text = "Prop Link (May Vary):";
                        break;

                    case 0x0900:
                        CBox_Types.SelectedIndex = 11;
                        EntryPropCB.Enabled = true;
                        EntryPropCB.SelectedItem = BTxt_PropLink.Text;
                        BTxt_PropLink.Enabled = true;
                        BTxt_GemClass.Enabled = false;
                        BTxt_GemType.Enabled = false;
                        BTxt_ItemFlag.Enabled = true;
                        TXT_FlagVary.Text = "Model ID:";
                        TXT_PropVary.Text = "Prop Link (May Vary):";
                        break;

                    case 0x0A00:
                        CBox_Types.SelectedIndex = 12;
                        EntryPropCB.Enabled = true;
                        EntryPropCB.SelectedItem = BTxt_PropLink.Text;
                        BTxt_PropLink.Enabled = true;
                        BTxt_GemClass.Enabled = false;
                        BTxt_GemType.Enabled = false;
                        BTxt_ItemFlag.Enabled = true;
                        TXT_FlagVary.Text = "Model ID:";
                        TXT_PropVary.Text = "Prop Link (May Vary):";
                        break;

                    case 0x0B00:
                        CBox_Types.SelectedIndex = 13;
                        EntryPropCB.Enabled = true;
                        EntryPropCB.SelectedItem = BTxt_PropLink.Text;
                        BTxt_PropLink.Enabled = true;
                        BTxt_GemClass.Enabled = false;
                        BTxt_GemType.Enabled = false;
                        BTxt_ItemFlag.Enabled = true;
                        TXT_FlagVary.Text = "Model ID:";
                        TXT_PropVary.Text = "Prop Link (May Vary):";
                        break;

                    case 0x0C00:
                        CBox_Types.SelectedIndex = 14;
                        EntryPropCB.Enabled = true;
                        EntryPropCB.SelectedItem = BTxt_PropLink.Text;
                        BTxt_PropLink.Enabled = true;
                        BTxt_GemClass.Enabled = false;
                        BTxt_GemType.Enabled = false;
                        BTxt_ItemFlag.Enabled = true;
                        TXT_FlagVary.Text = "Model ID:";
                        TXT_PropVary.Text = "Prop Link (May Vary):";
                        break;

                    case 0x0D00:
                        CBox_Types.SelectedIndex = 15;
                        EntryPropCB.Enabled = true;
                        EntryPropCB.SelectedItem = BTxt_PropLink.Text;
                        BTxt_PropLink.Enabled = true;
                        BTxt_GemClass.Enabled = false;
                        BTxt_GemType.Enabled = false;
                        BTxt_ItemFlag.Enabled = true;
                        TXT_FlagVary.Text = "Model ID:";
                        TXT_PropVary.Text = "Prop Link (May Vary):";
                        break;

                    case 0x0E00: // Armor
                        CBox_Types.SelectedIndex = 16;
                        EntryPropCB.Enabled = true;
                        EntryPropCB.SelectedItem = BTxt_ItemFlag.Text;
                        BTxt_PropLink.Enabled = false;
                        BTxt_PropLink.Text = null;
                        TXT_FlagVary.Text = "Prop Link (May Vary):";
                        TXT_PropVary.Text = "Unused Value:";
                        break;

                    case 0x0F00: // Accessory
                        CBox_Types.SelectedIndex = 17;
                        EntryPropCB.Enabled = true;
                        EntryPropCB.SelectedItem = BTxt_ItemFlag.Text;
                        BTxt_PropLink.Enabled = false;
                        BTxt_PropLink.Text = null;
                        TXT_FlagVary.Text = "Prop Link (May Vary):";
                        TXT_PropVary.Text = "Unused Value:";
                        break;

                    case 0x1000: // Gem Type ID
                        CBox_Types.SelectedIndex = 18;
                        EntryPropCB.Enabled = false;
                        EntryPropCB.SelectedIndex = 0;
                        BTxt_PropLink.Enabled = false;
                        BTxt_ItemFlag.Enabled = false;
                        BTxt_GemClass.Enabled = true;
                        BTxt_GemType.Enabled = true;
                        BTxt_PropLink.Text = null;
                        TXT_PropVary.Text = "Unused Value:";
                        break;

                    case 0x1100: // Misc. Item
                        CBox_Types.SelectedIndex = 19;
                        EntryPropCB.Enabled = false;
                        EntryPropCB.SelectedIndex = 0;
                        BTxt_PropLink.Enabled = false;
                        BTxt_ItemFlag.Text = null;
                        BTxt_PropLink.Text = null;
                        TXT_FlagVary.Text = "Unused Value:";
                        TXT_PropVary.Text = "Unused Value";
                        break;


                    case 0x1200: // Magic 
                        CBox_Types.SelectedIndex = 20;
                        EntryPropCB.Enabled = false;
                        EntryPropCB.SelectedIndex = 0;
                        BTxt_PropLink.Enabled = false;
                        BTxt_PropLink.Text = null;
                        TXT_FlagVary.Text = ".Mag File ID:";
                        TXT_PropVary.Text = "Unused Value:";
                        break;

                    case 0x1300: // Ability
                        CBox_Types.SelectedIndex = 21;
                        EntryPropCB.Enabled = false;
                        EntryPropCB.SelectedIndex = 0;
                        BTxt_PropLink.Enabled = true;
                        TXT_FlagVary.Text = "Ability ID:";
                        TXT_PropVary.Text = "Ability Type:";
                        break;

                    case 0x1401: // Summon Charm
                        CBox_Types.SelectedIndex = 22;
                        EntryPropCB.Enabled = false;
                        EntryPropCB.SelectedIndex = 0;
                        BTxt_PropLink.Enabled = false;
                        BTxt_ItemFlag.Enabled = false;
                        BTxt_PropLink.Text = null;
                        BTxt_ItemFlag.Text = null;
                        TXT_FlagVary.Text = "Unused Value:";
                        TXT_PropVary.Text = "Unused Value:";
                        break;

                    case 0x1501: // Form
                        CBox_Types.SelectedIndex = 23;
                        EntryPropCB.Enabled = false;
                        EntryPropCB.SelectedIndex = 0;
                        BTxt_PropLink.Enabled = false;
                        BTxt_ItemFlag.Enabled = false;
                        BTxt_PropLink.Text = null;
                        BTxt_ItemFlag.Text = null;
                        TXT_FlagVary.Text = "Unused Value:";
                        TXT_PropVary.Text = "Unused Value:";
                        break;

                    case 0x1601: // Map
                        CBox_Types.SelectedIndex = 24;
                        EntryPropCB.Enabled = false;
                        EntryPropCB.SelectedIndex = 0;
                        BTxt_PropLink.Enabled = false;
                        BTxt_PropLink.Text = null;
                        TXT_FlagVary.Text = "World ID:";
                        TXT_PropVary.Text = "Unused Value:";
                        break;

                    case 0x1701: // Report
                        CBox_Types.SelectedIndex = 25;
                        EntryPropCB.Enabled = false;
                        EntryPropCB.SelectedIndex = 0;
                        BTxt_PropLink.Enabled = false;
                        BTxt_PropLink.Text = null;
                        TXT_FlagVary.Text = "World ID:";
                        TXT_PropVary.Text = "Unused Value:";
                        break;
                }

                if (BTxt_GemClass.Enabled == true && BTxt_GemType.Enabled == true)
                {
                    BTxt_ItemFlag.Text = null;
                    BTxt_GemClass.Text = BitConverter.ToString(Breakdown1, 0);
                    BTxt_GemType.Text = BitConverter.ToString(Breakdown2, 0);
                }

                else
                {
                    BTxt_ItemFlag.Text = BitConverter.ToString(ItemFlag, 0).Replace("-", "");
                    BTxt_GemClass.Text = null;
                    BTxt_GemType.Text = null;
                }
            }

            TextChanging = false;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public void TextConverterOut(String FileName)
        {
            // Appropriate boxes to the values.
            ItemID = StringToByteArray(BTxt_ItemID.Text);
            ItemType = StringToByteArray(BTxt_ItemType.Text);
            ItemFlag = StringToByteArray(BTxt_ItemFlag.Text);
            PropLink = StringToByteArray(BTxt_PropLink.Text);
            NameStr = StringToByteArray(BTxt_NameStr.Text);
            DescStr = StringToByteArray(BTxt_DescStr.Text);
            CmmdLink = StringToByteArray(BTxt_CmmdLink.Text);
            SlotNum = StringToByteArray(BTxt_SlotNum.Text);
            PictID = new Byte[] { Byte.Parse(BTxt_PictID.Text, NumberStyles.HexNumber) };
            IconID = new Byte[] { Byte.Parse(BTxt_IconID.Text, NumberStyles.HexNumber) };

            EntryID = StringToByteArray(BTxt_PropID.Text);
            AbilityID = StringToByteArray(BTxt_AbilityLink.Text);
            AttackValue = new Byte[] { Byte.Parse(BTxt_ATK.Text, NumberStyles.HexNumber) };
            MagicValue = new Byte[] { Byte.Parse(BTxt_MAG.Text, NumberStyles.HexNumber) };
            DefenseValue = new Byte[] { Byte.Parse(BTxt_DEF.Text, NumberStyles.HexNumber) };
            APValue = new Byte[] { Byte.Parse(BTxt_AP.Text, NumberStyles.HexNumber) };
            FireVulnerability = new Byte[] { Byte.Parse(BTxt_FireVul.Text, NumberStyles.HexNumber) };
            IceVulnerability = new Byte[] { Byte.Parse(BTxt_IceVul.Text, NumberStyles.HexNumber) };
            LightningVulnerability = new Byte[] { Byte.Parse(BTxt_ThunderVul.Text, NumberStyles.HexNumber) };
            DarkVulnerability = new Byte[] { Byte.Parse(BTxt_DarkVul.Text, NumberStyles.HexNumber) };

            Array.Reverse(ItemID);
            Array.Reverse(ItemType);
            Array.Reverse(ItemFlag);
            Array.Reverse(PropLink);
            Array.Reverse(NameStr);
            Array.Reverse(DescStr);
            Array.Reverse(CmmdLink);
            Array.Reverse(SlotNum);
            Array.Reverse(PictID);
            Array.Reverse(IconID);

            Array.Reverse(EntryID);
            Array.Reverse(AbilityID);

            if (BTxt_GemClass.Enabled == true && BTxt_GemType.Enabled == true)
            {
                ItemFlag = new Byte[] { Byte.Parse(BTxt_GemClass.Text, NumberStyles.HexNumber), Byte.Parse(BTxt_GemType.Text, NumberStyles.HexNumber) };
            }
            using (Stream stream = new FileStream(FileName, FileMode.OpenOrCreate))
            {
                stream.Seek(4, SeekOrigin.Begin);
                stream.Write(EntryCount_Item, 0, 4);
                stream.Seek(8 + 4 + 24 * EntryItemCB.SelectedIndex, SeekOrigin.Begin);
                stream.Write(EntryCount_Item, 0, 4);

                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex, SeekOrigin.Begin);
                stream.Write(ItemID, 0, 2);
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 2, SeekOrigin.Begin);
                stream.Write(ItemType, 0, 2);
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 4, SeekOrigin.Begin);
                stream.Write(ItemFlag, 0, 2);
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 6, SeekOrigin.Begin);
                stream.Write(PropLink, 0, 2);
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 8, SeekOrigin.Begin);
                stream.Write(NameStr, 0, 2);
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 10, SeekOrigin.Begin);
                stream.Write(DescStr, 0, 2);
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 16, SeekOrigin.Begin);
                stream.Write(CmmdLink, 0, 2);
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 18, SeekOrigin.Begin);
                stream.Write(SlotNum, 0, 2);
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 20, SeekOrigin.Begin);
                stream.Write(PictID, 0, 1);
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 23, SeekOrigin.Begin);
                stream.Write(IconID, 0, 1);

                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex, SeekOrigin.Begin);
                stream.Write(EntryID, 0, 2);
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 2, SeekOrigin.Begin);
                stream.Write(AbilityID, 0, 2);
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 4, SeekOrigin.Begin);
                stream.Write(AttackValue, 0, 1);
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 5, SeekOrigin.Begin);
                stream.Write(MagicValue, 0, 1);
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 6, SeekOrigin.Begin);
                stream.Write(DefenseValue, 0, 1);
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 7, SeekOrigin.Begin);
                stream.Write(APValue, 0, 1);
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 9, SeekOrigin.Begin);
                stream.Write(FireVulnerability, 0, 1);
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 10, SeekOrigin.Begin);
                stream.Write(IceVulnerability, 0, 1);
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 11, SeekOrigin.Begin);
                stream.Write(LightningVulnerability, 0, 1);
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 12, SeekOrigin.Begin);
                stream.Write(DarkVulnerability, 0, 1);
            }
        }
    }
}
