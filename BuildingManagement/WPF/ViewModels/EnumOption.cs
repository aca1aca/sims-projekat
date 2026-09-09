namespace Sims.BuildingManagement.WPF.ViewModels;

public class EnumOption<T>
{
    public EnumOption(T value, string label)
    {
        Value = value;
        Label = label;
    }

    public T Value { get; }

    public string Label { get; }
}
