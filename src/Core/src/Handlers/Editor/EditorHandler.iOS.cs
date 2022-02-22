using System;
using Foundation;
using Microsoft.Maui.Graphics;
using UIKit;

namespace Microsoft.Maui.Handlers
{
	public partial class EditorHandler : ViewHandler<IEditor, MauiTextView>
	{
		static readonly int BaseHeight = 30;

		protected override MauiTextView CreatePlatformView() => new MauiTextView();

		protected override void ConnectHandler(MauiTextView platformView)
		{
			platformView.ShouldChangeText += OnShouldChangeText;
			platformView.Started += OnStarted;
			platformView.Ended += OnEnded;
			platformView.TextSetOrChanged += OnTextPropertySet;
		}

		protected override void DisconnectHandler(MauiTextView platformView)
		{
			platformView.ShouldChangeText -= OnShouldChangeText;
			platformView.Started -= OnStarted;
			platformView.Ended -= OnEnded;
			platformView.TextSetOrChanged -= OnTextPropertySet;

			if (platformView is IMauiTextView mauiUITextView)
				mauiUITextView.FrameChanged -= OnFrameChanged;
		}

		public override Size GetDesiredSize(double widthConstraint, double heightConstraint) =>
			new SizeRequest(new Size(widthConstraint, BaseHeight));

		protected internal virtual void UpdateAutoSizeOption()
		{
			if (VirtualView == null || PlatformView == null)
				return;

			PlatformView.FrameChanged -= OnFrameChanged;

			if (VirtualView.AutoSize == EditorAutoSizeOption.TextChanges)
				PlatformView.FrameChanged += OnFrameChanged;
		}

		public static void MapAutoSize(IEditorHandler handler, IEditor editor)
		{
			(handler as EditorHandler)?.UpdateAutoSizeOption();
		}

		public static void MapText(IEditorHandler handler, IEditor editor)
		{
			handler.PlatformView?.UpdateText(editor);

			// Any text update requires that we update any attributed string formatting
			MapFormatting(handler, editor);
		}

		public static void MapTextColor(IEditorHandler handler, IEditor editor) =>
			handler.PlatformView?.UpdateTextColor(editor);

		public static void MapPlaceholder(IEditorHandler handler, IEditor editor) =>
			handler.PlatformView?.UpdatePlaceholder(editor);

		public static void MapPlaceholderColor(IEditorHandler handler, IEditor editor) =>
			handler.PlatformView?.UpdatePlaceholderColor(editor);

		public static void MapCharacterSpacing(IEditorHandler handler, IEditor editor) =>
			handler.PlatformView?.UpdateCharacterSpacing(editor);

		public static void MapMaxLength(IEditorHandler handler, IEditor editor) =>
			handler.PlatformView?.UpdateMaxLength(editor);

		public static void MapIsReadOnly(IEditorHandler handler, IEditor editor) =>
			handler.PlatformView?.UpdateIsReadOnly(editor);

		public static void MapIsTextPredictionEnabled(IEditorHandler handler, IEditor editor) =>
			handler.PlatformView?.UpdateIsTextPredictionEnabled(editor);

		public static void MapFont(IEditorHandler handler, IEditor editor) =>
			handler.PlatformView?.UpdateFont(editor, handler.GetRequiredService<IFontManager>());

		public static void MapHorizontalTextAlignment(IEditorHandler handler, IEditor editor) =>
			handler.PlatformView?.UpdateHorizontalTextAlignment(editor);

		[MissingMapper]
		public static void MapVerticalTextAlignment(IEditorHandler handler, IEditor editor)
		{
		}

		public static void MapCursorPosition(IEditorHandler handler, IEditor editor) =>
			handler.PlatformView?.UpdateCursorPosition(editor);

		public static void MapSelectionLength(IEditorHandler handler, IEditor editor) =>
			handler.PlatformView?.UpdateSelectionLength(editor);

		public static void MapKeyboard(IEditorHandler handler, IEditor editor) =>
			handler.PlatformView?.UpdateKeyboard(editor);

		public static void MapFormatting(IEditorHandler handler, IEditor editor)
		{
			handler.PlatformView?.UpdateMaxLength(editor);

			// Update all of the attributed text formatting properties
			handler.PlatformView?.UpdateCharacterSpacing(editor);
		}

		bool OnShouldChangeText(UITextView textView, NSRange range, string replacementString) =>
			VirtualView.TextWithinMaxLength(textView.Text, range, replacementString);

		void OnStarted(object? sender, EventArgs eventArgs)
		{
			if (VirtualView != null)
				VirtualView.IsFocused = true;
		}

		void OnEnded(object? sender, EventArgs eventArgs)
		{
			if (VirtualView != null)
			{
				VirtualView.IsFocused = false;

				VirtualView.Completed();
			}
		}

		void OnTextPropertySet(object? sender, EventArgs e) =>
			VirtualView.UpdateText(PlatformView.Text); 
		
		void OnFrameChanged(object? sender, EventArgs e)
		{
			// When a new line is added to the UITextView the resize happens after the view has already scrolled
			// This causes the view to reposition without the scroll. If TextChanges is enabled then the Frame
			// will resize until it can't anymore and thus it should never be scrolled until the Frame can't increase in size
			if (VirtualView.AutoSize == EditorAutoSizeOption.TextChanges)
			{
				PlatformView?.ScrollRangeToVisible(new NSRange(0, 0));
			}
		}
	}
}