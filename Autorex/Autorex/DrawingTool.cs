namespace Autorex {
    /// <summary>
    /// All drawing tools and actions that can be done by user
    /// </summary>
    public enum DrawingTool: byte {
        Circle,
        Curve,
        Ellipse,
        Line,
        Pen,
        Rectangle,

		// actions
		Scale,
		Select,
        Rotate
    }
}
