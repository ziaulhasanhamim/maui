﻿using System;
using Foundation;
using Microsoft.Maui.Graphics;
using ObjCRuntime;
using UIKit;

namespace Microsoft.Maui.Platform
{
	public static class TextFieldExtensions
	{
		public static void UpdateText(this UITextField textField, IEntry entry)
		{
			textField.Text = entry.Text;
		}

		public static void UpdateTextColor(this UITextField textField, ITextStyle textStyle, UIColor? defaultTextColor = null)
		{
			// Default value of color documented to be black in iOS docs

			var textColor = textStyle.TextColor;
			textField.TextColor = textColor.ToPlatform(defaultTextColor ?? ColorExtensions.LabelColor);
		}

		public static void UpdateIsPassword(this UITextField textField, IEntry entry)
		{
			if (entry.IsPassword && textField.IsFirstResponder)
			{
				textField.Enabled = false;
				textField.SecureTextEntry = true;
				textField.Enabled = entry.IsEnabled;
				textField.BecomeFirstResponder();
			}
			else
				textField.SecureTextEntry = entry.IsPassword;
		}

		public static void UpdateHorizontalTextAlignment(this UITextField textField, ITextAlignment textAlignment)
		{
			bool isLtr;

			if (textAlignment is IView v && v.FlowDirection == FlowDirection.LeftToRight)
				isLtr = true;
			else
				isLtr = false;

			textField.TextAlignment = textAlignment.HorizontalTextAlignment.ToPlatform(isLtr);
		}

		public static void UpdateVerticalTextAlignment(this UITextField textField, ITextAlignment textAlignment)
		{
			textField.VerticalAlignment = textAlignment.VerticalTextAlignment.ToPlatform();
		}

		public static void UpdateIsTextPredictionEnabled(this UITextField textField, IEntry entry)
		{
			if (entry.IsTextPredictionEnabled)
				textField.AutocorrectionType = UITextAutocorrectionType.Yes;
			else
				textField.AutocorrectionType = UITextAutocorrectionType.No;
		}

		public static void UpdateMaxLength(this UITextField textField, IEntry entry)
		{
			var newText = textField.AttributedText.TrimToMaxLength(entry.MaxLength);
			if (newText != null && textField.AttributedText != newText)
				textField.AttributedText = newText;
		}

		public static void UpdatePlaceholder(this UITextField textField, IEntry entry, Color? defaultPlaceholderColor = null)
		{
			var placeholder = entry.Placeholder;
			if (placeholder == null)
			{
				textField.AttributedPlaceholder = null;
				return;
			}

			var placeholderColor = entry.PlaceholderColor;
			var foregroundColor = placeholderColor ?? defaultPlaceholderColor;

			textField.AttributedPlaceholder = foregroundColor == null
 				? new NSAttributedString(placeholder)
 				: new NSAttributedString(str: placeholder, foregroundColor: foregroundColor.ToPlatform());

			textField.AttributedPlaceholder.WithCharacterSpacing(entry.CharacterSpacing);
		}

		public static void UpdateIsReadOnly(this UITextField textField, IEntry entry)
		{
			textField.UserInteractionEnabled = !(entry.IsReadOnly || entry.InputTransparent);
		}

		public static void UpdateInputTransparent(this UITextField textField, IViewHandler handler, IEntry entry)
		{
			textField.UserInteractionEnabled = !(entry.IsReadOnly || entry.InputTransparent);
		}

		public static void UpdateFont(this UITextField textField, ITextStyle textStyle, IFontManager fontManager)
		{
			var uiFont = fontManager.GetFont(textStyle.Font, UIFont.LabelFontSize);
			textField.Font = uiFont;
		}

		public static void UpdateReturnType(this UITextField textField, IEntry entry)
		{
			textField.ReturnKeyType = entry.ReturnType.ToPlatform();
		}

		public static void UpdateCharacterSpacing(this UITextField textField, ITextStyle textStyle)
		{
			var textAttr = textField.AttributedText?.WithCharacterSpacing(textStyle.CharacterSpacing);
			if (textAttr != null)
				textField.AttributedText = textAttr;

			textAttr = textField.AttributedPlaceholder?.WithCharacterSpacing(textStyle.CharacterSpacing);
			if (textAttr != null)
				textField.AttributedPlaceholder = textAttr;
		}

		public static void UpdateKeyboard(this UITextField textField, IEntry entry)
		{
			var keyboard = entry.Keyboard;

			textField.ApplyKeyboard(keyboard);

			if (keyboard is not CustomKeyboard)
				textField.UpdateIsTextPredictionEnabled(entry);

			textField.ReloadInputViews();
		}

		[PortHandler]
		public static void UpdateCursorPosition(this UITextField textField, IEntry entry)
		{
			var selectedTextRange = textField.SelectedTextRange;
			if (selectedTextRange == null)
				return;
			if (textField.GetOffsetFromPosition(textField.BeginningOfDocument, selectedTextRange.Start) != entry.CursorPosition)
				UpdateCursorSelection(textField, entry);
		}

		[PortHandler]
		public static void UpdateSelectionLength(this UITextField textField, IEntry entry)
		{
			var selectedTextRange = textField.SelectedTextRange;
			if (selectedTextRange == null)
				return;
			if (textField.GetOffsetFromPosition(selectedTextRange.Start, selectedTextRange.End) != entry.SelectionLength)
				UpdateCursorSelection(textField, entry);
		}

		/* Updates both the IEntry.CursorPosition and IEntry.SelectionLength properties. */
		static void UpdateCursorSelection(this UITextField textField, IEntry entry)
		{
			if (!entry.IsReadOnly)
			{
				if (!textField.IsFirstResponder)
					textField.BecomeFirstResponder();
				UITextPosition start = GetSelectionStart(textField, entry, out int startOffset);
				UITextPosition end = GetSelectionEnd(textField, entry, start, startOffset);

				textField.SelectedTextRange = textField.GetTextRange(start, end);
			}
		}

		static UITextPosition GetSelectionStart(UITextField textField, IEntry entry, out int startOffset)
		{
			int cursorPosition = entry.CursorPosition;

			UITextPosition start = textField.GetPosition(textField.BeginningOfDocument, cursorPosition) ?? textField.EndOfDocument;
			startOffset = Math.Max(0, (int)textField.GetOffsetFromPosition(textField.BeginningOfDocument, start));

			if (startOffset != cursorPosition)
				entry.CursorPosition = startOffset;

			return start;
		}

		static UITextPosition GetSelectionEnd(UITextField textField, IEntry entry, UITextPosition start, int startOffset)
		{
			int selectionLength = entry.SelectionLength;
			int textFieldLength = textField.Text == null ? 0 : textField.Text.Length;
			// Get the desired range in respect to the actual length of the text we are working with
			UITextPosition end = textField.GetPosition(start, Math.Min(textFieldLength - entry.CursorPosition, selectionLength)) ?? start;
			int endOffset = Math.Max(startOffset, (int)textField.GetOffsetFromPosition(textField.BeginningOfDocument, end));

			int newSelectionLength = Math.Max(0, endOffset - startOffset);
			if (newSelectionLength != selectionLength)
				entry.SelectionLength = newSelectionLength;

			return end;
		}

		public static void UpdateClearButtonVisibility(this UITextField textField, IEntry entry)
		{
			textField.ClearButtonMode = entry.ClearButtonVisibility == ClearButtonVisibility.WhileEditing ? UITextFieldViewMode.WhileEditing : UITextFieldViewMode.Never;
		}
	}
}
