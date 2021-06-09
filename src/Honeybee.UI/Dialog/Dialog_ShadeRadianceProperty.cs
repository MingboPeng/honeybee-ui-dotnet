﻿using Eto.Drawing;
using Eto.Forms;
using System.Linq;
using System;
using HoneybeeSchema;

namespace Honeybee.UI
{
    [Obsolete("This is deprecated", true)]
    public class Dialog_ShadeRadianceProperty : Dialog<ShadeRadiancePropertiesAbridged>
    {
        private ModelRadianceProperties ModelRadianceProperties { get; set; }
        public Dialog_ShadeRadianceProperty(ModelRadianceProperties libSource, ShadeRadiancePropertiesAbridged ShadeRadianceProperties, bool updateChangesOnly = false)
        {
            try
            {
                this.ModelRadianceProperties = libSource;
                var prop = ShadeRadianceProperties ?? new ShadeRadiancePropertiesAbridged();

                if (updateChangesOnly)
                    prop = new ShadeRadiancePropertiesAbridged("No Changes");


                Padding = new Padding(15);
                Title = $"Shade Radiance Properties - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                Width = 450;
                this.Icon = DialogHelper.HoneybeeIcon;

                //Get Modifier
                var mSets = this.ModelRadianceProperties.Modifiers
                    .OfType<IDdRadianceBaseModel>()
                    .ToList();

                if (updateChangesOnly)
                    mSets.Insert(0, new Plastic("No Changes"));

                var modifierDP = DialogHelper.MakeDropDown(prop.Modifier, (v) => prop.Modifier = v?.Identifier,
                    mSets, "Default Modifier");

                var modifierBlkDP = DialogHelper.MakeDropDown(prop.ModifierBlk, (v) => prop.ModifierBlk = v?.Identifier,
                    mSets, "Default Modifier");

                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => 
                {
                    Close(prop); 
                };

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

           
                var layout = new DynamicLayout();
                //layout.DefaultPadding = new Padding(10);
                layout.DefaultSpacing = new Size(5, 5);

                layout.AddRow("Shade Modifier:");
                layout.AddRow(modifierDP);
                layout.AddRow("Shade Modifier Blk:");
                layout.AddRow(modifierBlkDP);
                layout.AddSeparateRow(null, this.DefaultButton, this.AbortButton, null);
                layout.AddRow(null);
                //Create layout
                Content = layout;
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Failed to open ShadeRadianceProperty dialog:\n{e.Message}");
            }
            
            
        }
    
    
    }
}
