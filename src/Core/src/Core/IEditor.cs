namespace Microsoft.Maui
{
	/// <summary>
	/// Represents a View used to accept multi-line input.
	/// </summary>
	public interface IEditor : IView, ITextInput, ITextStyle, ITextAlignment
	{
		/// <summary>
		/// Gets a value that controls whether the editor will change size to accommodate input as the user enters it.
		/// </summary>
		EditorAutoSizeOption AutoSize { get; }

		/// <summary>
		/// Occurs when the user finalizes the text in an editor with the return key.
		/// </summary>
		void Completed();
	}
}