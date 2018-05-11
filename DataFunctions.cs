using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using TopazLib;

namespace KH2Suite_ItemTable
{
    public partial class MainShit
    {
        // All of the creators are here //////////////////////////////////////////////
                                                                                    //
        // Basic Parameters                                                         //
        Int32 EntryCount_Item;                                                      //
        Int32 EntryCount_Prop;                                                      //
                                                                                    //
        // Params of Item                                                           //
        Int16 ItemID;                                                               //
        Int16 ItemType;  // Hard-Coded.                                             //
        Int16 ItemFlag;  // Varies, changes linked to ItemType                      //
        Byte Breakdown1; // For the Gem Sub-Type                                    //
        Byte Breakdown2; // For the Gem Sub-Type                                    //
        Int16 PropLink;  // Linked to Sub-Table                                     //
        Int16 NameStr;   // Linked to msg/jp/sys.bar                                //
        Int16 DescStr;   // Linked to msg/jp/sys.bar                                //
        Int32 NoEffect;  // Literally, changing this on any item, did nothing.      //  
        Int16 CmmdLink;  // Linked to 03system.bin/cmd[0x02].bin                    //
        Int16 SlotNum;                                                              //
        Int16 PictID;    // Linked to itempic/item-xxx.imd                          //
        Byte Unused0;                                                               //
        Byte IconID;     // Linked to msg/jp/fontimage.bin                          //  
                                                                                    //
        // Params of Prop                                                           //
        Int16 EntryID;   // Linked To Sub-Table - PropLink                          //
        Int16 AbilityID; // Linked to Sub-Table - ItemID                            //
        Byte AttackValue;                                                           //
        Byte MagicValue;                                                            //
        Byte DefenseValue;                                                          //
        Byte APValue;                                                               //  
        Byte Unused1;                                                               //
        Byte FireVulnerability;   // Is % Based, 100% Resist at 0% and vice-versa.  //
        Byte IceVulnerability;    // Is % Based, 100% Resist at 0% and vice-versa.  //
        Byte LightVulnerability;  // Is % Based, 100% Resist at 0% and vice-versa.  //
        Byte DarkVulnerability;   // Is % Based, 100% Resist at 0% and vice-versa.  //
        Byte Unused2;                                                               //
                                                                                    //
        //////////////////////////////////////////////////////////////////////////////

        public void ParseFile(String InputFile, Int32 FileID, Boolean is03System)
        {
            if (is03System == false)
            {
                if (FileID == 1)
                {

                    // Parse Items ////////////////////////////////////////////////////////////////////
                                                                                                     // 
                    // The if statement adds all the entries to EntryItemCB if the combo box is null //
                    if (EntryCount_Item == 0)                                                        //
                    {                                                                                //
                        EntryCount_Item = GeneralFunctions.readBytetoInt32(InputFile, 0x04, 0x04);   //
                        GeneralFunctions.entryLoop(EntryItemCB, EntryCount_Item);                    //
                                                                                                     //
                        // This line sets the progress bar value to the Entry Count.                 // 
                        PBar_ItemCap.Value = EntryItemCB.Items.Count;                                //
                        EntryItemCB.SelectedIndex = 0;                                               // 
                    }                                                                                //
                                                                                                     // 
                    // This part parses the said item.                                               // 
                    int skipBytes = 8 + 24 * EntryItemCB.SelectedIndex;                              // 
                    ItemID     = GeneralFunctions.readBytetoInt16(InputFile, skipBytes + 00, 0x02);  //
                    ItemType   = GeneralFunctions.readBytetoInt16(InputFile, skipBytes + 02, 0x02);  //
                    ItemFlag   = GeneralFunctions.readBytetoInt16(InputFile, skipBytes + 04, 0x02);  //
                    Breakdown1 = GeneralFunctions.readByte(   InputFile    , skipBytes + 04);        //
                    Breakdown2 = GeneralFunctions.readByte(   InputFile    , skipBytes + 05);        //
                    PropLink   = GeneralFunctions.readBytetoInt16(InputFile, skipBytes + 06, 0x02);  //
                    NameStr    = GeneralFunctions.readBytetoInt16(InputFile, skipBytes + 08, 0x02);  //
                    DescStr    = GeneralFunctions.readBytetoInt16(InputFile, skipBytes + 10, 0x02);  //
                    CmmdLink   = GeneralFunctions.readBytetoInt16(InputFile, skipBytes + 16, 0x02);  //
                    SlotNum    = GeneralFunctions.readBytetoInt16(InputFile, skipBytes + 18, 0x02);  //
                    PictID     = GeneralFunctions.readBytetoInt16(InputFile, skipBytes + 20, 0x02);  //
                    IconID     = GeneralFunctions.readByte(   InputFile    , skipBytes + 23);        //
                                                                                                     //
                    ///////////////////////////////////////////////////////////////////////////////////


                    // Parse Props ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                                                                                                                             // 
                    // This int represents the general skipping value.                                                                       //
                    int skipBytes2 = 8 + 4 + 24 * EntryItemCB.Items.Count;                                                                   //
                    int skipBytes3 = 8 + 8 + 24 * EntryItemCB.Items.Count;                                                                   //
                                                                                                                                             //
                    // The if statement adds all the entries to EntryItemCB if the combo box is null.                                        //
                    if (EntryCount_Prop == 0)                                                                                                //
                    {                                                                                                                        //
                        EntryCount_Prop = GeneralFunctions.readBytetoInt32(InputFile, skipBytes2, 0x04);                                     //
                        GeneralFunctions.entryLoop(EntryPropCB, EntryCount_Prop, InputFile, skipBytes3, 16, 0x02);                           //
                                                                                                                                             //
                        // This line sets the progress bar value to the Entry Count.                                                         // 
                        PBar_PropCap.Value = EntryPropCB.Items.Count;                                                                        //
                        EntryPropCB.SelectedIndex = 0;                                                                                       //
                    }                                                                                                                        //
                                                                                                                                             //
                    EntryID            = GeneralFunctions.readBytetoInt16(InputFile, skipBytes3 + 16 * EntryPropCB.SelectedIndex + 00, 02);  //
                    AbilityID          = GeneralFunctions.readBytetoInt16(InputFile, skipBytes3 + 16 * EntryPropCB.SelectedIndex + 02, 02);  //
                    AttackValue        = GeneralFunctions.readByte(   InputFile   ,  skipBytes3 + 16 * EntryPropCB.SelectedIndex + 04);      //
                    MagicValue         = GeneralFunctions.readByte(   InputFile   ,  skipBytes3 + 16 * EntryPropCB.SelectedIndex + 05);      //
                    DefenseValue       = GeneralFunctions.readByte(   InputFile   ,  skipBytes3 + 16 * EntryPropCB.SelectedIndex + 06);      //
                    APValue            = GeneralFunctions.readByte(   InputFile   ,  skipBytes3 + 16 * EntryPropCB.SelectedIndex + 07);      //
                    FireVulnerability  = GeneralFunctions.readByte(   InputFile   ,  skipBytes3 + 16 * EntryPropCB.SelectedIndex + 09);      //
                    IceVulnerability   = GeneralFunctions.readByte(   InputFile   ,  skipBytes3 + 16 * EntryPropCB.SelectedIndex + 10);      //
                    LightVulnerability = GeneralFunctions.readByte(   InputFile   ,  skipBytes3 + 16 * EntryPropCB.SelectedIndex + 11);      //
                    DarkVulnerability  = GeneralFunctions.readByte(   InputFile   ,  skipBytes3 + 16 * EntryPropCB.SelectedIndex + 12);      //
                                                                                                                                             //
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                }

                else if (FileID == 2)
                {
                    // Parse Props ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                                                                                                                             // 
                    // This int represents the general skipping value.                                                                       //
                    int skipBytes2 = 8 + 4 + 24 * EntryItemCB.Items.Count;                                                                   //
                    int skipBytes3 = 8 + 8 + 24 * EntryItemCB.Items.Count;                                                                   //
                                                                                                                                             //
                    // The if statement adds all the entries to EntryItemCB if the combo box is null.                                        //
                    if (EntryCount_Prop == 0)                                                                                                //
                    {                                                                                                                        //
                        EntryCount_Prop = GeneralFunctions.readBytetoInt32(InputFile, skipBytes2, 0x04);                                     //
                        GeneralFunctions.entryLoop(EntryPropCB, EntryCount_Prop, InputFile, skipBytes3, 16, 0x02);                           //
                                                                                                                                             //
                        // This line sets the progress bar value to the Entry Count.                                                         // 
                        PBar_PropCap.Value = EntryPropCB.Items.Count;                                                                        //
                        EntryPropCB.SelectedIndex = 0;                                                                                       //
                    }                                                                                                                        //
                                                                                                                                             //
                    EntryID            = GeneralFunctions.readBytetoInt16(InputFile, skipBytes3 + 16 * EntryPropCB.SelectedIndex + 00, 02);  //
                    AbilityID          = GeneralFunctions.readBytetoInt16(InputFile, skipBytes3 + 16 * EntryPropCB.SelectedIndex + 02, 02);  //
                    AttackValue        = GeneralFunctions.readByte(   InputFile   ,  skipBytes3 + 16 * EntryPropCB.SelectedIndex + 04);      //
                    MagicValue         = GeneralFunctions.readByte(   InputFile   ,  skipBytes3 + 16 * EntryPropCB.SelectedIndex + 05);      //
                    DefenseValue       = GeneralFunctions.readByte(   InputFile   ,  skipBytes3 + 16 * EntryPropCB.SelectedIndex + 06);      //
                    APValue            = GeneralFunctions.readByte(   InputFile   ,  skipBytes3 + 16 * EntryPropCB.SelectedIndex + 07);      //
                    FireVulnerability  = GeneralFunctions.readByte(   InputFile   ,  skipBytes3 + 16 * EntryPropCB.SelectedIndex + 09);      //
                    IceVulnerability   = GeneralFunctions.readByte(   InputFile   ,  skipBytes3 + 16 * EntryPropCB.SelectedIndex + 10);      //
                    LightVulnerability = GeneralFunctions.readByte(   InputFile   ,  skipBytes3 + 16 * EntryPropCB.SelectedIndex + 11);      //
                    DarkVulnerability  = GeneralFunctions.readByte(   InputFile   ,  skipBytes3 + 16 * EntryPropCB.SelectedIndex + 12);      //
                                                                                                                                             //
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////            
                }
            }
        }

        public void TextConverterIn(String FileName, Int16 Calltype)
        {
            // Parse the values to text and combo entries ////////////////////////////////
                                                                                        //
            //                                                                          //
            BTxt_ItemID.Text      = GeneralFunctions.getInt16String(ItemID);            //
            BTxt_ItemType.Text    = GeneralFunctions.getInt16String(ItemType);          //
            BTxt_ItemFlag.Text    = GeneralFunctions.getInt16String(ItemFlag);          //
            BTxt_PropLink.Text    = GeneralFunctions.getInt16String(PropLink);          //
            BTxt_NameStr.Text     = GeneralFunctions.getInt16String(NameStr);           //
            BTxt_DescStr.Text     = GeneralFunctions.getInt16String(DescStr);           //
            BTxt_CmmdLink.Text    = GeneralFunctions.getInt16String(CmmdLink);          //
            BTxt_SlotNum.Text     = GeneralFunctions.getInt16String(SlotNum);           //
            BTxt_PictID.Text      = GeneralFunctions.getInt16String(PictID);            //
            BTxt_IconID.Text      = GeneralFunctions.getByteString(IconID);             //
                                                                                        //
            BTxt_PropID.Text      = GeneralFunctions.getInt16String(EntryID);           //
            BTxt_AbilityLink.Text = GeneralFunctions.getInt16String(AbilityID);         //
            BTxt_ATK.Text         = GeneralFunctions.getByteString(AttackValue);        //
            BTxt_MAG.Text         = GeneralFunctions.getByteString(MagicValue);         //
            BTxt_DEF.Text         = GeneralFunctions.getByteString(DefenseValue);       //
            BTxt_AP.Text          = GeneralFunctions.getByteString(APValue);            //
            BTxt_FireVul.Text     = GeneralFunctions.getByteString(FireVulnerability);  //
            BTxt_IceVul.Text      = GeneralFunctions.getByteString(IceVulnerability);   //
            BTxt_ThunderVul.Text  = GeneralFunctions.getByteString(LightVulnerability); //
            BTxt_DarkVul.Text     = GeneralFunctions.getByteString(DarkVulnerability);  //
                                                                                        //
            // If it's the initial type call, get the following to apply.               //
            if (Calltype == 0)                                                          //
            {                                                                           //
                                                                                        //
            // Initial Values                                                           //
                EntryPropCB.Enabled = true;                                             //
                BTxt_PropLink.Enabled = true;                                           //
                BTxt_GemClass.Enabled = false;                                          //
                BTxt_GemType.Enabled = false;                                           //
                BTxt_ItemFlag.Enabled = true;                                           //
                TXT_FlagVary.Text = "Item Flag (May Vary):";                            //
                TXT_PropVary.Text = "Prop Link (May Vary):";                            //
                                                                                        //
                switch (ItemType)                                                       //
                {                                                                       //
                    case 0x0000: // Equipable Item                                      //
                        CBox_Types.SelectedIndex = 0;                                   //
                        EntryPropCB.Enabled = false;                                    //
                        EntryPropCB.SelectedIndex = 0;                                  //
                        BTxt_PropLink.Enabled = true;                                   //
                        TXT_FlagVary.Text = "Item Recover Value:";                      //
                        TXT_PropVary.Text = "Item Type:";                               //
                        break;                                                          //
                                                                                        //
                    case 0x0001: // Menu Item                                           //
                        CBox_Types.SelectedIndex = 1;                                   //
                        EntryPropCB.Enabled = false;                                    //
                        EntryPropCB.SelectedIndex = 0;                                  //
                        BTxt_PropLink.Enabled = true;                                   //
                        TXT_FlagVary.Text = "Item Recover Value:";                      //
                        TXT_PropVary.Text = "Item Type:";                               //
                        break;                                                          //
                                                                                        //
                    case 0x0200: // Equippable Mega Item                                //
                        CBox_Types.SelectedIndex = 2;                                   //
                        EntryPropCB.Enabled = false;                                    //
                        EntryPropCB.SelectedIndex = 0;                                  //
                        BTxt_PropLink.Enabled = true;                                   //
                        TXT_FlagVary.Text = "Item Recover Value:";                      //
                        TXT_PropVary.Text = "Item Type:";                               //
                        break;                                                          //
                                                                                        //...
                    case 0x0201: // Mega Menu Item
                        CBox_Types.SelectedIndex = 3;
                        EntryPropCB.Enabled = false;
                        EntryPropCB.SelectedIndex = 0;
                        BTxt_PropLink.Enabled = true;
                        TXT_FlagVary.Text = "Item Recover Value:";
                        TXT_PropVary.Text = "Item Type:";
                        break;
                                                                                        
                    case 0x0002: // Keyblade
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
                                                                                        
                    case 0x0003: // Staff
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

                    case 0x0004: // Shield
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

                    case 0x0005:
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

                    case 0x0006:
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

                    case 0x0007:
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

                    case 0x0008:
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

                    case 0x0009:
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

                    case 0x000A:
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

                    case 0x000B:
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

                    case 0x000C:
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

                    case 0x000D:
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

                    case 0x000E: // Armor
                        CBox_Types.SelectedIndex = 16;
                        EntryPropCB.Enabled = true;
                        EntryPropCB.SelectedItem = BTxt_ItemFlag.Text;
                        BTxt_PropLink.Enabled = false;
                        BTxt_PropLink.Text = null;
                        TXT_FlagVary.Text = "Prop Link (May Vary):";
                        TXT_PropVary.Text = "Unused Value:";
                        break;

                    case 0x000F: // Accessory
                        CBox_Types.SelectedIndex = 17;
                        EntryPropCB.Enabled = true;
                        EntryPropCB.SelectedItem = BTxt_ItemFlag.Text;
                        BTxt_PropLink.Enabled = false;
                        BTxt_PropLink.Text = null;
                        TXT_FlagVary.Text = "Prop Link (May Vary):";
                        TXT_PropVary.Text = "Unused Value:";
                        break;

                    case 0x0010: // Gem Type ID
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

                    case 0x0011: // Misc. Item
                        CBox_Types.SelectedIndex = 19;
                        EntryPropCB.Enabled = false;
                        EntryPropCB.SelectedIndex = 0;
                        BTxt_PropLink.Enabled = false;
                        BTxt_ItemFlag.Text = null;
                        BTxt_PropLink.Text = null;
                        TXT_FlagVary.Text = "Unused Value:";
                        TXT_PropVary.Text = "Unused Value";
                        break;


                    case 0x0012: // Magic 
                        CBox_Types.SelectedIndex = 20;
                        EntryPropCB.Enabled = false;
                        EntryPropCB.SelectedIndex = 0;
                        BTxt_PropLink.Enabled = false;
                        BTxt_PropLink.Text = null;
                        TXT_FlagVary.Text = ".Mag File ID:";
                        TXT_PropVary.Text = "Unused Value:";
                        break;

                    case 0x0013: // Ability
                        CBox_Types.SelectedIndex = 21;
                        EntryPropCB.Enabled = false;
                        EntryPropCB.SelectedIndex = 0;
                        BTxt_PropLink.Enabled = true;
                        TXT_FlagVary.Text = "Ability ID:";
                        TXT_PropVary.Text = "Ability Type:";
                        break;

                    case 0x0114: // Summon Charm
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

                    case 0x0115: // Form
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

                    case 0x0116: // Map
                        CBox_Types.SelectedIndex = 24;
                        EntryPropCB.Enabled = false;
                        EntryPropCB.SelectedIndex = 0;
                        BTxt_PropLink.Enabled = false;
                        BTxt_PropLink.Text = null;
                        TXT_FlagVary.Text = "World ID:";
                        TXT_PropVary.Text = "Unused Value:";
                        break;

                    case 0x0117: // Report
                        CBox_Types.SelectedIndex = 25;
                        EntryPropCB.Enabled = false;
                        EntryPropCB.SelectedIndex = 0;
                        BTxt_PropLink.Enabled = false;
                        BTxt_PropLink.Text = null;
                        TXT_FlagVary.Text = "World ID:";
                        TXT_PropVary.Text = "Unused Value:";
                        break;                                                          //...                    
                }                                                                       //
                                                                                        //
                // Determine if it's a gem, if it is, do the following                  //
                if (BTxt_GemClass.Enabled == true && BTxt_GemType.Enabled == true)      //
                {                                                                       //
                    BTxt_ItemFlag.Text = null;                                          //
                    BTxt_GemClass.Text = GeneralFunctions.getByteString(Breakdown1);    //
                    BTxt_GemType.Text = GeneralFunctions.getByteString(Breakdown2);     //
                }                                                                       //
                                                                                        //
                else                                                                    //
                {                                                                       //
                    BTxt_ItemFlag.Text = GeneralFunctions.getInt16String(ItemFlag);     //
                    BTxt_GemClass.Text = null;                                          //
                    BTxt_GemType.Text = null;                                           //
                }                                                                       //
            }                                                                           //
                                                                                        //
            TextChanging = false;                                                       //
                                                                                        //
            //////////////////////////////////////////////////////////////////////////////
        }                                   

        public void TextConverterOut(String FileName)
        {
            // Parse the text into Ints and Bytes and write them /////////////////////////////////////////////////////////////////////////
                                                                                                                                        //
            ItemID             = GeneralFunctions.hexToInteger(BTxt_ItemID.Text);                                                       //
            ItemType           = GeneralFunctions.hexToInteger(BTxt_ItemType.Text);                                                     //
            ItemFlag           = GeneralFunctions.hexToInteger(BTxt_ItemFlag.Text);                                                     //
            PropLink           = GeneralFunctions.hexToInteger(BTxt_PropLink.Text);                                                     //
            NameStr            = GeneralFunctions.hexToInteger(BTxt_NameStr.Text);                                                      //    
            DescStr            = GeneralFunctions.hexToInteger(BTxt_DescStr.Text);                                                      //    
            CmmdLink           = GeneralFunctions.hexToInteger(BTxt_CmmdLink.Text);                                                     //
            SlotNum            = GeneralFunctions.hexToInteger(BTxt_SlotNum.Text);                                                      //
            PictID             = GeneralFunctions.hexToInteger(BTxt_PictID.Text);                                                       //
            IconID             = GeneralFunctions.hexToByte(BTxt_IconID.Text);                                                          //
                                                                                                                                        //
            EntryID            = GeneralFunctions.hexToInteger(BTxt_PropID.Text);                                                       //
            AbilityID          = GeneralFunctions.hexToInteger(BTxt_AbilityLink.Text);                                                  //
            AttackValue        = GeneralFunctions.hexToByte(BTxt_ATK.Text);                                                             //
            MagicValue         = GeneralFunctions.hexToByte(BTxt_MAG.Text);                                                             //
            DefenseValue       = GeneralFunctions.hexToByte(BTxt_DEF.Text);                                                             //
            APValue            = GeneralFunctions.hexToByte(BTxt_AP.Text);                                                              //
            FireVulnerability  = GeneralFunctions.hexToByte(BTxt_FireVul.Text);                                                         //
            IceVulnerability   = GeneralFunctions.hexToByte(BTxt_IceVul.Text);                                                          //
            LightVulnerability = GeneralFunctions.hexToByte(BTxt_ThunderVul.Text);                                                      //
            DarkVulnerability  = GeneralFunctions.hexToByte(BTxt_DarkVul.Text);                                                         //
                                                                                                                                        //
            // If the item is a gem, do the following:                                                                                  //
            if (BTxt_GemClass.Enabled == true && BTxt_GemType.Enabled == true)                                                          //
            {                                                                                                                           //
                ItemFlag = BitConverter.ToInt16(new byte[] { Byte.Parse(BTxt_GemType.Text), Byte.Parse(BTxt_GemClass.Text) }, 0);       //
            }                                                                                                                           //
                                                                                                                                        //
            // Write everything (Tried to downgrade this to just 10 lines, didn't work)                                                 //
            using (Stream stream = new FileStream(FileName, FileMode.OpenOrCreate))                                                     //
            {                                                                                                                           //
            // Write the header values                                                                                                  //
                stream.Seek(4, SeekOrigin.Begin);                                                                                       //
                stream.Write(BitConverter.GetBytes(EntryCount_Item), 0, 4);                                                             //
                stream.Seek(8 + 4 + 24 * EntryItemCB.SelectedIndex, SeekOrigin.Begin);                                                  //
                stream.Write(BitConverter.GetBytes(EntryCount_Item), 0, 4);                                                             //
                                                                                                                                        //
            // Write the said item entry                                                                                                //
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex, SeekOrigin.Begin);                                                      //
                stream.Write(GeneralFunctions.intToByteArray(ItemID), 0, 2);                                                            //
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 2, SeekOrigin.Begin);                                                  //
                stream.Write(GeneralFunctions.intToByteArray(ItemType), 0, 2);                                                          //
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 4, SeekOrigin.Begin);                                                  //
                stream.Write(GeneralFunctions.intToByteArray(ItemFlag), 0, 2);                                                          //
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 6, SeekOrigin.Begin);                                                  //
                stream.Write(GeneralFunctions.intToByteArray(PropLink), 0, 2);                                                          //
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 8, SeekOrigin.Begin);                                                  //
                stream.Write(GeneralFunctions.intToByteArray(NameStr), 0, 2);                                                           //
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 10, SeekOrigin.Begin);                                                 //    
                stream.Write(GeneralFunctions.intToByteArray(DescStr), 0, 2);                                                           //
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 12, SeekOrigin.Begin);                                                 //                                            
                stream.Write(new byte[4], 0, 4);                                                                                        //
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 16, SeekOrigin.Begin);                                                 //
                stream.Write(GeneralFunctions.intToByteArray(CmmdLink), 0, 2);                                                          //
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 18, SeekOrigin.Begin);                                                 //
                stream.Write(GeneralFunctions.intToByteArray(SlotNum), 0, 2);                                                           //
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 20, SeekOrigin.Begin);                                                 //
                stream.Write(GeneralFunctions.intToByteArray(PictID), 0, 2);                                                            //
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 22, SeekOrigin.Begin);                                                 //
                stream.Write(new byte[1], 0, 1);                                                                                        //
                stream.Seek(8 + 24 * EntryItemCB.SelectedIndex + 23, SeekOrigin.Begin);                                                 //
                stream.WriteByte(IconID);                                                                                               //
                                                                                                                                        //
            // Write the said property entry                                                                                            //
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex, SeekOrigin.Begin);                   //
                stream.Write(GeneralFunctions.intToByteArray(EntryID), 0, 2);                                                           //
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 2, SeekOrigin.Begin);               //
                stream.Write(GeneralFunctions.intToByteArray(AbilityID), 0, 2);                                                         //
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 4, SeekOrigin.Begin);               //
                stream.WriteByte(AttackValue);                                                                                          //
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 5, SeekOrigin.Begin);               //
                stream.WriteByte(MagicValue);                                                                                           //
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 6, SeekOrigin.Begin);               //  
                stream.WriteByte(DefenseValue);                                                                                         //
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 7, SeekOrigin.Begin);               //
                stream.WriteByte(APValue);                                                                                              //
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 8, SeekOrigin.Begin);               //
                stream.Write(new byte[1], 0, 1);                                                                                        //
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 9, SeekOrigin.Begin);               //
                stream.WriteByte(FireVulnerability);                                                                                    //
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 10, SeekOrigin.Begin);              //
                stream.WriteByte(IceVulnerability);                                                                                     //
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 11, SeekOrigin.Begin);              //
                stream.WriteByte(LightVulnerability);                                                                                   //
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 12, SeekOrigin.Begin);              //
                stream.WriteByte(DarkVulnerability);                                                                                    //
                stream.Seek(8 + 8 + 24 * EntryItemCB.Items.Count + 16 * EntryPropCB.SelectedIndex + 13, SeekOrigin.Begin);              //
                stream.Write(new byte[] { 0x64, 0x64, 0x00 }, 0, 3);                                                                    //
                                                                                                                                        //
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }
        }
    }
}
