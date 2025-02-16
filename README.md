Stephen’s answer ▲ does a great job using a `Style`, as the OP requested. To build on that, this answer drills a little deeper into an alternative approach: a custom control for better reusability, maintainability, and extensibility across projects. Since the OP wants a control that "_has no checkmark or other icon_" (essentially a `Button`), what if we create `OneHotButton` and give it a property that binds in XAML markup—e.g., `GroupName`, whose members behave one-hot, just like a `RadioButton` group? While we're at it, we can add bindable properties for `SelectedTextColor`, `SelectedBackgroundColor`, `UnselectedTextColor`, and `UnselectedBackgroundColor`, making it easier to customize the appearance without modifying multiple styles.



```
<local:OneHotButton Text="Apple" SelectedTextColor="Salmon"  GroupName="OptionsGroup"  IsChecked="true"/>
```

and

```
<Style TargetType="local:OneHotButton" >
    <Setter Property="SelectedBackgroundColor" Value="CornflowerBlue"/>
</Style>
```

___

**Example One Hot Implementation**

~~~csharp
[DebuggerDisplay("{Text}")]
public partial class OneHotButton : Button
{
    public OneHotButton()
    {
        Clicked += OnButtonClicked;
        UpdateColors();
    }
    private static readonly Dictionary<string, List<OneHotButton>> buttonGroups = new();

    public static readonly BindableProperty GroupNameProperty =
        BindableProperty.Create(
            nameof(GroupName),
            typeof(string),
            typeof(OneHotButton),
            default(string),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is OneHotButton button)
                {
                    if (oldValue is string oldGroup)
                    {
                        buttonGroups[oldGroup].Remove(button);
                        if (buttonGroups[oldGroup].Count == 0)
                            buttonGroups.Remove(oldGroup);

                    }
                    if (newValue is string newGroup && !string.IsNullOrWhiteSpace(newGroup))
                    {
                        if (!buttonGroups.ContainsKey(newGroup))
                            buttonGroups[newGroup] = new List<OneHotButton>();
                        buttonGroups[newGroup].Add(button);
                    }
                }
            });

    public static readonly BindableProperty IsCheckedProperty =
        BindableProperty.Create(
            nameof(IsChecked),
            typeof(bool),
            typeof(OneHotButton),
            false,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is OneHotButton button &&
                    Equals(newValue, true) &&
                    !string.IsNullOrWhiteSpace(button.GroupName) &&
                    buttonGroups.TryGetValue(button.GroupName, out var buttonList))
                {
                    button.UpdateColors();
                    buttonList
                    .Where(_ => !ReferenceEquals(_, button))
                    .ToList()
                    .ForEach(_ =>
                    {
                        _.IsChecked = false;
                        _.UpdateColors();
                    });
                }
            });
    ...
}
~~~

___

**Example Bindable Color**

This snippet shows how to expose a property to be used in XAML markup.

```csharp
public Color SelectedTextColor
{
    get => (Color)GetValue(SelectedTextColorProperty);
    set => SetValue(SelectedTextColorProperty, value);
}

public static readonly BindableProperty SelectedBackgroundColorProperty =
    BindableProperty.Create(
        propertyName: nameof(OneHotButton.SelectedBackgroundColor),
        returnType: typeof(Color),
        declaringType: typeof(OneHotButton),
        defaultValue: Colors.CornflowerBlue,
        defaultBindingMode: BindingMode.OneWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is OneHotButton @this)
            {
                @this.UpdateColors();
            }
        });

private void UpdateColors()
{
    if(IsChecked)
    {
        TextColor = SelectedTextColor;
        BackgroundColor = SelectedBackgroundColor;
    }
    else
    {
        TextColor = UnselectedTextColor;
        BackgroundColor = UnselectedBackgroundColor;
    }
}
... Other bindable colors
```
___

**XAML Example**

```xaml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:style_radio_group"
             x:Class="style_radio_group.MainPage">

    <ContentPage.Resources>
        <Style TargetType="local:OneHotButton" >
            <Setter Property="SelectedBackgroundColor" Value="CornflowerBlue"/>
            <Setter Property="Margin" Value="25,0"/>
        </Style>
    </ContentPage.Resources>
    <ScrollView>
        <VerticalStackLayout Padding="30,0" Spacing="25">
            <Image 
                Source="dotnet_bot.png"
                HeightRequest="185"
                Aspect="AspectFit"
                SemanticProperties.Description="dot net bot in a race car number eight" />
            <Border Padding="0,0,0,10" StrokeShape="RoundRectangle 10">
                <VerticalStackLayout Spacing="10">
                    <Label
                        Text="Choose One Fruit"
                        Padding="5"
                        BackgroundColor="LightGray"
                        FontSize="16"
                        FontAttributes="Bold"
                        HorizontalOptions="Fill"/>
                    <local:OneHotButton 
                        Text="Apple"
                        SelectedTextColor="Salmon"     
                        GroupName="OptionsGroup"
                        IsChecked="true"/>
                    <local:OneHotButton    
                        Text="Banana"
                        SelectedTextColor="Yellow"
                        GroupName="OptionsGroup"/>
                    <local:OneHotButton   
                        Text="Grape"
                        SelectedTextColor="LightGreen"
                        GroupName="OptionsGroup"/>
                </VerticalStackLayout>
            </Border>
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```
[![net9.0 on android][1]][1]


  [1]: https://i.sstatic.net/jtkXzVqF.png