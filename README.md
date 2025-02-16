Stephen has answered ▲ the question in exactly the way it was asked. This is to offer the perspective that writing this as a custom control offers some advantages in terms of reusability, especially in other projects. The OP's requirement is for a control that _"has no checkmark or other icon"_. That sounds like a `Button` so what if we make `OneHotButton` and give it a property that binds in XAML markup, that is a `GroupName` whose members behave one-hot like a `RadioButton` group would. While we're at it, make bindable properties for `SelectedTextColor`, `SelectedBackgroundColor`, `UnselectedTextColor`, and `UnselectedBackgroundColor`.
    }
}

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
                    Text="Select a Fruit:"
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

[![screenshots][1]][1]


  [1]: https://i.sstatic.net/TU5S4OJj.png