using System;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Essentials;
using Microsoft.Maui.Graphics;
using UIKit;

namespace Microsoft.Maui.Handlers
{
	public partial class EditorHandler : ViewHandler<IEditor, MauiTextView>
	{
		static readonly int BaseHeight = 30;

		protected override MauiTextView CreateNativeView()
		{
		var nativeEditor =	new MauiTextView();

			if (DeviceInfo.Idiom == DeviceIdiom.Phone)
			{
				// iPhone does not have a dismiss keyboard button
				var keyboardWidth = UIScreen.MainScreen.Bounds.Width;
				var accessoryView = new UIToolbar(new CGRect(0, 0, keyboardWidth, 44)) { BarStyle = UIBarStyle.Default, Translucent = true };

				var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
				var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) =>
				{
					nativeEditor.ResignFirstResponder();
					VirtualView?.Completed();
				});

				accessoryView.SetItems(new[] { spacer, doneButton }, false);
				nativeEditor.InputAccessoryView = accessoryView;
			}

			return nativeEditor;
		}

		protected override void ConnectHandler(MauiTextView nativeView)
		{
			nativeView.ShouldChangeText += OnShouldChangeText;
			nativeView.Started += OnStarted;
			nativeView.Ended += OnEnded;
			nativeView.TextSetOrChanged += OnTextPropertySet;
		}

		protected override void DisconnectHandler(MauiTextView nativeView)
		{
			nativeView.ShouldChangeText -= OnShouldChangeText;
			nativeView.Started -= OnStarted;
			nativeView.Ended -= OnEnded;
			nativeView.TextSetOrChanged -= OnTextPropertySet;
			nativeView.FrameChanged -= OnFrameChanged;
		}

		public override Size GetDesiredSize(double widthConstraint, double heightConstraint) =>
			new SizeRequest(new Size(widthConstraint, BaseHeight));

		public static void MapAutoSize(EditorHandler handler, IEditor editor)
		{
			handler.UpdateAutoSizeOption();
		}

		protected internal virtual void UpdateAutoSizeOption()
		{
			if (VirtualView == null || NativeView == null)
				return;

			NativeView.FrameChanged -= OnFrameChanged;

			if (VirtualView.AutoSize == EditorAutoSizeOption.TextChanges)
				NativeView.FrameChanged += OnFrameChanged;
		}

		public static void MapText(EditorHandler handler, IEditor editor)
		{
			handler.NativeView?.UpdateText(editor);

			// Any text update requires that we update any attributed string formatting
			MapFormatting(handler, editor);
		}

		public static void MapTextColor(EditorHandler handler, IEditor editor) =>
			handler.NativeView?.UpdateTextColor(editor);

		public static void MapPlaceholder(EditorHandler handler, IEditor editor) =>
			handler.NativeView?.UpdatePlaceholder(editor);

		public static void MapPlaceholderColor(EditorHandler handler, IEditor editor) =>
			handler.NativeView?.UpdatePlaceholderColor(editor);

		public static void MapCharacterSpacing(EditorHandler handler, IEditor editor) =>
			handler.NativeView?.UpdateCharacterSpacing(editor);

		public static void MapMaxLength(EditorHandler handler, IEditor editor) =>
			handler.NativeView?.UpdateMaxLength(editor);

		public static void MapIsReadOnly(EditorHandler handler, IEditor editor) =>
			handler.NativeView?.UpdateIsReadOnly(editor);

		public static void MapIsTextPredictionEnabled(EditorHandler handler, IEditor editor) =>
			handler.NativeView?.UpdateIsTextPredictionEnabled(editor);

		public static void MapFont(EditorHandler handler, IEditor editor) =>
			handler.NativeView?.UpdateFont(editor, handler.GetRequiredService<IFontManager>());

		public static void MapHorizontalTextAlignment(EditorHandler handler, IEditor editor) =>
			handler.NativeView?.UpdateHorizontalTextAlignment(editor);

		[MissingMapper]
		public static void MapVerticalTextAlignment(EditorHandler handler, IEditor editor)
		{
		}

		public static void MapCursorPosition(EditorHandler handler, IEditor editor) =>
			handler.NativeView?.UpdateCursorPosition(editor);

		public static void MapSelectionLength(EditorHandler handler, IEditor editor) =>
			handler.NativeView?.UpdateSelectionLength(editor);

		public static void MapKeyboard(EditorHandler handler, IEditor editor) =>
			handler.NativeView?.UpdateKeyboard(editor);

		public static void MapFormatting(EditorHandler handler, IEditor editor)
		{
			handler.NativeView?.UpdateMaxLength(editor);

			// Update all of the attributed text formatting properties
			handler.NativeView?.UpdateCharacterSpacing(editor);
		}

		bool OnShouldChangeText(UITextView textView, NSRange range, string replacementString) =>
			VirtualView.TextWithinMaxLength(textView.Text, range, replacementString);

		void OnStarted(object? sender, EventArgs eventArgs)
		{
			// TODO: Update IsFocused property
		}

		void OnEnded(object? sender, EventArgs eventArgs)
		{
			// TODO: Update IsFocused property
			VirtualView.Completed();
		}

		void OnTextPropertySet(object? sender, EventArgs e) =>
			VirtualView.UpdateText(NativeView.Text); 
		
		void OnFrameChanged(object? sender, EventArgs e)
		{
			// When a new line is added to the UITextView the resize happens after the view has already scrolled
			// This causes the view to reposition without the scroll. If TextChanges is enabled then the Frame
			// will resize until it can't anymore and thus it should never be scrolled until the Frame can't increase in size
			if (VirtualView.AutoSize == EditorAutoSizeOption.TextChanges)
			{
				NativeView.ScrollRangeToVisible(new NSRange(0, 0));
			}
		}
	}
}