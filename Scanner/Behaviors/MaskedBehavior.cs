﻿using System.Collections.Generic;
using Xamarin.Forms;

namespace Scanner.Behaviors
{
    /// <summary>
    /// https://xamarinhelp.com/masked-entry-in-xamarin-forms/
    /// http://steinebel.ru/?go=all/phone-numbers/
    /// </summary>
    public class MaskedBehavior : Behavior<Entry>
    {
        private string mask = "";
        private IDictionary<int, char> positions;

        public string Mask
        {
            get => mask;
            set
            {
                mask = value;
                SetPositions();
            }
        }

        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            var entry = sender as Entry;
            var text = entry.Text;

            if (string.IsNullOrWhiteSpace(text) || positions == null)
                return;

            if (text.Length > mask.Length)
            {
                entry.Text = text.Remove(text.Length - 1);
                return;
            }

            foreach (var position in positions)
            {
                if (text.Length >= position.Key + 1)
                {
                    var value = position.Value.ToString();

                    if (text.Substring(position.Key, 1) != value)
                        text = text.Insert(position.Key, value);
                }
            }

            if (entry.Text != text)
                entry.Text = text;
        }

        private void SetPositions()
        {
            if (string.IsNullOrEmpty(Mask))
            {
                positions = null;
                return;
            }

            var list = new Dictionary<int, char>();

            for (var i = 0; i < Mask.Length; i++)
            {
                if (Mask[i] != 'X')
                    list.Add(i, Mask[i]);
            }

            positions = list;
        }
    }
}
