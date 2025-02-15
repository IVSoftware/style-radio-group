using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace style_radio_group
{
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

        public static readonly BindableProperty SelectedTextColorProperty =
        BindableProperty.Create(
            propertyName: nameof(OneHotButton.SelectedTextColor),
            returnType: typeof(Color),
            declaringType: typeof(OneHotButton),
            defaultValue: Colors.White,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is OneHotButton @this)
                {
                    @this.UpdateColors();
                }
            });

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

        public Color SelectedBackgroundColor
        {
            get => (Color)GetValue(SelectedBackgroundColorProperty);
            set => SetValue(SelectedBackgroundColorProperty, value);
        }


        public static readonly BindableProperty UnselectedTextColorProperty =
            BindableProperty.Create(
                propertyName: nameof(OneHotButton.UnselectedTextColor),
                returnType: typeof(Color),
                declaringType: typeof(OneHotButton),
                defaultValue: Colors.Black,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    if (bindable is OneHotButton @this)
                    {
                        @this.UpdateColors();
                    }
                });

        public Color UnselectedTextColor
        {
            get => (Color)GetValue(UnselectedTextColorProperty);
            set => SetValue(UnselectedTextColorProperty, value);
        }

        public static readonly BindableProperty UnselectedBackgroundColorProperty =
            BindableProperty.Create(
                propertyName: nameof(OneHotButton.UnselectedBackgroundColor),
                returnType: typeof(Color),
                declaringType: typeof(OneHotButton),
                defaultValue: Colors.White,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    if (bindable is OneHotButton @this)
                    {
                        @this.UpdateColors();
                    }
                });

        public Color UnselectedBackgroundColor
        {
            get => (Color)GetValue(UnselectedBackgroundColorProperty);
            set => SetValue(UnselectedBackgroundColorProperty, value);
        }


        public string GroupName
        {
            get => (string)GetValue(GroupNameProperty);
            set => SetValue(GroupNameProperty, value);
        }

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        private void OnButtonClicked(object? sender, EventArgs e) =>
            IsChecked = true;
    }
}
