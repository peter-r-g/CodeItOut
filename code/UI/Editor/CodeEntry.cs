using Sandbox.UI;

namespace CodeItOut.UI;

public class CodeEntry : TextEntry
{
	public CodeEntry()
	{
		Multiline = true;
	}

	public override void OnButtonEvent( ButtonEvent e )
	{
		if ( e.Pressed && e.Button == "tab" )
		{
			(Parent as Editor).ToggleVisibility();
			return;
		}
		
		base.OnButtonEvent( e );
	}
}
