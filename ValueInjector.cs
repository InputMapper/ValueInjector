using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ODIF;

namespace ValueInjector
{
    [PluginInfo(
        PluginName = "Value Injector",
        PluginDescription = "",
        PluginID = 0,
        PluginAuthorName = "InputMapper",
        PluginAuthorEmail = "jhebbel@gmail.com",
        PluginAuthorURL = "http://inputmapper.com",
        PluginIconPath = @"pack://application:,,,/SimpleLogic;component/Resources/share-icon.png"
    )]
    [CompatibleTypes(
        InputTypes = mappingIOTypes.Bool,
        OutputTypes = mappingIOTypes.Bool| mappingIOTypes.Double
    )]
    public class ValueInjector : InputModificationPlugin
    {

        Setting ValType, FixedValue, ChannelValue, PluginValue;
        dynamic cachedValue;
        bool inVal;
        public ValueInjector(Guid guid) : base(guid)
        {
            Setting ValType, FixedValue, ChannelValue, PluginValue;

            ValType = new Setting("Value Type", "Type of value that will be used", SettingControl.Dropdown, SettingType.Text, "fixed", true, true);
            ValType.configuration.Add("options", new List<string>() { "fixed", "input channel", "existing plugin value" });
            ValType.PropertyChanged += ValType_PropertyChanged; ;
            
            FixedValue = new Setting("Fixed Value", "value that will used", SettingControl.Numeric, SettingType.Decimal, 0d, true, true);
            FixedValue.configuration["interval"] = .1d;

            ChannelValue = new Setting("Input Channel", "", SettingControl.InputChannelSelector, SettingType.Text, new DeviceChannel(), false, true);

            settings.settings.Add(ValType);
            settings.settings.Add(FixedValue);
            settings.settings.Add(ChannelValue);
            //trueChannelValue.configuration.Add("options", new List<string>() { "fixed", "input channel", "existing plugin value" });

            ChannelValue.PropertyChanged += ChannelValue_PropertyChanged;

            SetValue(false);
        }

        private void ChannelValue_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ChannelValue.settingValue != null)
            {
                (ChannelValue.settingValue as DeviceChannel).PropertyChanged += (s, ev) => { SetValue(cachedValue); };
            }
        }

        private void ValType_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ValType.settingValue == "fixed")
            {
                FixedValue.visible = true;
                ChannelValue.visible = false;
            }
            if (ValType.settingValue == "input channel")
            {
                FixedValue.visible = false;
                ChannelValue.visible = true;
            }
            if (ValType.settingValue == "existing plugin value")
            {
                FixedValue.visible = false;
                ChannelValue.visible = false;
            }
        }

        public override void SetValue(dynamic inValue)
        {
            if (inValue == inVal) return;

            inVal = inValue;

            if (inValue == true)
            {
                if (ValType.settingValue == "fixed")
                {
                    this.Value = FixedValue.settingValue;
                }
                if (ValType.settingValue == "input channel")
                {
                    this.Value = ChannelValue.settingValue.Value;
                }
                if (ValType.settingValue == "existing plugin value")
                {

                }
            }
        }
    }
}
