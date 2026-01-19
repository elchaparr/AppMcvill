using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMcvill.Behaviors
{
    public class NumericValidationBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += OnTextChanged;
            base.OnAttachedTo(bindable);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not Entry entry)
                return;

            var text = entry.Text;

            if (string.IsNullOrEmpty(text))
                entry.Text = "0";
            else if (text == ".")
                entry.Text = "0.";
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= OnTextChanged;
            base.OnDetachingFrom(bindable);
        }
    }
}
