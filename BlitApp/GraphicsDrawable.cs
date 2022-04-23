namespace BlitApp;

public abstract class DrawableBase : IDrawable
{
    protected readonly ICollection<IDrawable> drawables = new HashSet<IDrawable>();
    protected RectF? dirtyRect;
    private int frameCount = 0;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        this.dirtyRect = dirtyRect;

        canvas.ClearBackground(dirtyRect);

        // WORKS
        canvas.DrawFromList(drawables);

        // DOESNT
        //drawables.ToArray().AsParallel().ForAll(drawable => drawable.Draw(canvas, dirtyRect));

        canvas.DrawBigTextWithShadow(dirtyRect, $"No Skia Here!....({++frameCount})");
    }
}

public class RandomLinesDrawable : DrawableBase
{
    const int maxLines = 1000;
    const int linesPerTick = 1;

    public void NextStep()
    {

        if (drawables.Count + linesPerTick > maxLines)
        {
            drawables.Clear();
        }

        for (var i = 0; i < linesPerTick; i++)
        {
            if (dirtyRect.HasValue)
            {
                drawables.Add(new Line() { Point1 = dirtyRect.Value.RandomPoint(), Point2 = dirtyRect.Value.RandomPoint(), StrokeColor = CanvasTools.NextColor, StrokeSize = CanvasTools.NextStrokeSize });
            }
        }

    }
}

public class Ball : IDrawable
{
    public Color FillColor;
    public PointF Position;
    public float Radius;
    public SizeF Velocity;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = FillColor;
        canvas.FillCircle(Position, Radius);
    }
}

public class Line : IDrawable
{
    public PointF Point1;
    public PointF Point2;
    public Color StrokeColor;
    public float StrokeSize;

    public void Draw(ICanvas canvas, RectF dirtyRect) => canvas.DrawStrokedLine(StrokeSize, StrokeColor, Point1, Point2);

    public static Line Random(RectF bounds) => new() { Point1 = bounds.RandomPoint(), Point2 = bounds.RandomPoint(), StrokeColor = CanvasTools.NextColor, StrokeSize = CanvasTools.NextStrokeSize };
}

public static class CanvasTools
{
    readonly static float bigTextFontSize = 36;
    readonly static Color bigTextFontColor = Colors.White;
    readonly static PointF bigTextPosition = new(20, 50);
    readonly static float bigTextShadowBlur = 1;
    readonly static Color bigTextShadowColor = Colors.Black;
    readonly static SizeF bigTextShadowOffset = new(2, 2);

    readonly static float randomLineAlpha = .2f;
    readonly static float randomLineStrokeBaseSize = 10f;

    readonly static float solidCornerRadius = 12;
    readonly static Color solidFillColor = Color.FromArgb("#003366");
    readonly static SolidPaint solidPaint = new(Colors.Silver);

    readonly static float shadowBlur = 10;
    readonly static Color shadowColor = Colors.Gray;
    readonly static SizeF shadowSize = new(10, 10);

    readonly static Random random = Random.Shared;

    public static Color NextColor => new(
                red: NextFloat,
                green: NextFloat,
                blue: NextFloat,
                alpha: randomLineAlpha);

    public static float NextFloat => random.NextSingle();

    public static float NextStrokeSize => NextFloat * randomLineStrokeBaseSize;

    public static void ClearBackground(this ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = solidFillColor;

        canvas.FillRectangle(dirtyRect);
    }

    public static void DrawBigTextWithShadow(this ICanvas canvas, RectF dirtyRect, string text)
    {
        canvas.FontSize = bigTextFontSize;
        canvas.FontColor = bigTextFontColor;

        canvas.SetShadow(offset: bigTextShadowOffset, blur: bigTextShadowBlur, color: bigTextShadowColor);
        canvas.DrawString(text, Math.Min(bigTextPosition.X, dirtyRect.Width), Math.Min(bigTextPosition.Y, dirtyRect.Height), HorizontalAlignment.Left);
    }

    public static void DrawGraySolid(this ICanvas canvas, RectF dirtyRect)
    {
        canvas.SetFillPaint(solidPaint, dirtyRect);
        canvas.SetShadow(shadowSize, shadowBlur, shadowColor);
        canvas.FillRoundedRectangle(dirtyRect, solidCornerRadius);
    }

    public static void DrawFromList(this ICanvas canvas, IEnumerable<IDrawable> lines)
    {
        //RectF newRect = new();
        //lines.ToArray().AsParallel().WithDegreeOfParallelism(16).ForAll(thing => thing.Draw(canvas, newRect));
        foreach (var line in lines.ToArray())
        {
            line.Draw(canvas, new());
        }
    }

    public static void DrawRandomLine(this ICanvas canvas, RectF dirtyRect) => canvas.DrawStrokedLine(NextStrokeSize, NextColor, dirtyRect.RandomPoint(), dirtyRect.RandomPoint());

    public static void DrawRandomLines(this ICanvas canvas, RectF dirtyRect, int count = 1000)
    {
        for (var i = 0; i < count; i++)
        {
            canvas.DrawRandomLine(dirtyRect);
        }
    }

    public static void DrawStrokedLine(this ICanvas canvas, float strokeSize, Color strokeColor, PointF point1, PointF point2)
    {
        canvas.StrokeColor = strokeColor;
        canvas.StrokeSize = strokeSize;

        canvas.DrawLine(point1, point2);
    }

    public static PointF RandomPoint(this RectF rect) => new(rect.Size.Width * NextFloat, rect.Size.Height * NextFloat);
}
