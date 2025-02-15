using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace style_radio_group
{
    public class OneHotButton : Button
    {
        public OneHotButton() => Clicked += OnButtonClicked;

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
                        buttonList
                        .Where(_=> !ReferenceEquals(_, button))
                        .ToList()
                        .ForEach(_ => _.IsChecked = false);
                    }
                });

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
