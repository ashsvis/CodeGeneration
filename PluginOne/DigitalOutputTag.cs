namespace PluginOne
{
    public class DigitalOutputTag : DigitalInputTag
    {
        public DigitalOutputTag()
        {
            FuncDesc = "Тег дискретного выхода";
            FuncName = "TAG_DO";
            Width = 48 * 3;
            Inputs = [new() { }];
            Outputs = [];
            AllowedPinInverted = false;
            AllowedShowBorder = false;
            AllowedShowFuncName = false;
            AllowedShowLabelNumber = false;
            CalculateHeight();
        }
    }
    public class AnalogOutputTag : DigitalInputTag
    {
        public AnalogOutputTag()
        {
            FuncDesc = "Тег аналогового выхода";
            FuncName = "TAG_AO";
            Width = 48 * 3;
            Inputs = [new() { }];
            Outputs = [];
            AllowedPinInverted = false;
            AllowedShowBorder = false;
            AllowedShowFuncName = false;
            AllowedShowLabelNumber = false;
            CalculateHeight();
        }
    }
}
