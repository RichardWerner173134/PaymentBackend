namespace Masterarbeit.OrderService
{
    public interface IFunctionSettingsResolver
    {
        string GetStringValue(string settingsKey);
        int GetIntegerValue(string settingsKey);
    }

    public class FunctionSettingsResolver : IFunctionSettingsResolver
    {
        public virtual string GetStringValue(string settingsKey)
        {
            return Environment.GetEnvironmentVariable(settingsKey) ?? string.Empty;
        }

        public virtual int GetIntegerValue(string settingsKey)
        {
            return int.Parse(Environment.GetEnvironmentVariable(settingsKey) ?? string.Empty);
        }
    }
}
