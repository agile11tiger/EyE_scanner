using Xamarin.Forms;

namespace Scanner.Controls
{
    /// <summary>
    /// https://trailheadtechnology.com/gradient-label-control-in-xamarin-forms/
    /// </summary>
    public class GradientLabel : Label
    {
        public static readonly BindableProperty TextColorFromProperty = BindableProperty.Create(
            propertyName: nameof(TextColorFrom),
            returnType: typeof(Color),
            declaringType: typeof(GradientLabel),
            defaultValue: Color.Red);

        public Color TextColorFrom
        {
            get => (Color)GetValue(TextColorFromProperty);
            set => SetValue(TextColorFromProperty, value);
        }

        public static readonly BindableProperty TextColorToProperty = BindableProperty.Create(
            propertyName: nameof(TextColorTo),
            returnType: typeof(Color),
            declaringType: typeof(GradientLabel),
            defaultValue: Color.Black);

        public Color TextColorTo
        {
            get => (Color)GetValue(TextColorToProperty);
            set => SetValue(TextColorToProperty, value);
        }
    }
}
