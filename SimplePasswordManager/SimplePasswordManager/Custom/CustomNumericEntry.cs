using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace SimplePasswordManager.Custom
{
    public class ValidValueChangedEvent : EventArgs
    {
        public float Value { get; set; }
    }

    public class CustomNumericEntry : Entry
    {
        public class NumericValidationBehaviour : Behavior<CustomNumericEntry>
        {
            protected override void OnAttachedTo(CustomNumericEntry entry)
            {
                entry.TextChanged += OnEntryTextChanged;
                entry.Unfocused += OnEntryUnFocused;
                base.OnAttachedTo(entry);
            }

            protected override void OnDetachingFrom(CustomNumericEntry entry)
            {
                entry.TextChanged -= OnEntryTextChanged;
                entry.Unfocused -= OnEntryUnFocused;
                base.OnDetachingFrom(entry);
            }

            private void OnEntryTextChanged(object s, TextChangedEventArgs e)
            {
                var entry = (CustomNumericEntry)s;
                if(!string.IsNullOrWhiteSpace(e.NewTextValue))
                {
                    var is_valid = e.NewTextValue.ToCharArray().All(x => char.IsDigit(x) || x == '-' || x == '.');
                    try
                    {
                        var value = float.Parse(e.NewTextValue);
                        is_valid = entry.InclusiveBounds ? 
                                   is_valid && value >= entry.LowerBound && value <= entry.UpperBound :
                                   is_valid && value > entry.LowerBound && value < entry.UpperBound;
                    } 
                    catch(Exception) {}
                    entry.Text = is_valid ? e.NewTextValue : e.OldTextValue;
                }
            }

            private void OnEntryUnFocused(object s, EventArgs e)
            {
                var entry = (CustomNumericEntry)s;
                var text_value = entry.Text;

                if(text_value != null && text_value.Length > 0)
                {
                    if(text_value[0] == '-')
                    {
                        if(entry.LowerBound >= 0)
                        {
                            entry.Text = "0";
                        }
                        else
                        {
                            if(entry.Text.Length == 1)
                            {
                                entry.Text = "-0";
                            }
                        }
                    }
                    else if(text_value[0] == '.')
                    {
                        if(entry.FloatingPoint)
                        {
                            if(text_value.Length == 1)
                            {
                                entry.Text = "0.0";
                            }
                            else
                            {
                                entry.Text = "0" + entry.Text;
                            }
                        }
                        else
                        {
                            entry.Text = entry.Text.Substring(1, entry.Text.Length - 1);
                        }
                    }
                    else if(!entry.FloatingPoint)
                    {
                        var period_index = entry.Text.IndexOf('.');
                        if(period_index != -1)
                        {
                            entry.Text = entry.Text.Substring(0, period_index);
                        }
                    }
                }
            }
        }

        public CustomNumericEntry()
        {
            Keyboard = Keyboard.Numeric;
            Behaviors.Add(m_behavior);
            //TextChanged += Entry_TextChanged;
        }

        public event EventHandler<ValidValueChangedEvent> ValidValueChanged
        {
            add
            {
                m_validValueEvents.Add(value);
            }

            remove
            {
                m_validValueEvents.Remove(value);
            }
        }

        public int LowerBound { get; set; } = Int32.MinValue;
        public int UpperBound { get; set; } = Int32.MaxValue;
        public bool InclusiveBounds { get; set; } = false;
        public bool FloatingPoint { get; set; } = false;

        private List<EventHandler<ValidValueChangedEvent>>  m_validValueEvents = new List<EventHandler<ValidValueChangedEvent>>();
        private NumericValidationBehaviour                  m_behavior = new NumericValidationBehaviour();
    }
}
