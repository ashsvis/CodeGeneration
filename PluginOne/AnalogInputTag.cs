namespace PluginOne
{
    public class AnalogInputTag : DigitalInputTag
    {
        public AnalogInputTag()
        {
            FuncDesc = "Тег аналогового входа";
            FuncName = "TAG_AI";
            Width = 48 * 3;
            Inputs = [];
            Outputs = [new() { }];
            AllowedPinInverted = false;
            AllowedShowBorder = false;
            AllowedShowFuncName = false;
            AllowedShowLabelNumber = false;
            CalculateHeight();
        }
    }
}
