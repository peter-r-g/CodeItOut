using Sandbox;
using Sandbox.UI;

namespace CodeItOut.UI;

[UseTemplate]
public class Editor : Panel
{
	public Button VisibilityButton { get; set; }
	public Button SubmitButton { get; set; }
	
	public CodeEntry CodeEntry { get; set; }

	[Event.BuildInput]
	protected void BuildInput( InputBuilder input )
	{
		if ( input.Pressed( InputButton.Score ) )
			ToggleVisibility();
	}

	public void ToggleVisibility()
	{
		var nowHidden = !HasClass( "hidden" );
		VisibilityButton.Text = nowHidden ? "⬆️" : "⬇️";
		SetClass( "hidden", nowHidden );
		CodeEntry.SetClass( "hidden", nowHidden );
		SubmitButton.SetClass( "hidden", nowHidden );
	}
	
	public void Submit()
	{
		ToggleVisibility();
		PuzzleGame.SubmitSolution( CodeEntry.Text );
	}
}
