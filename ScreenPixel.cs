// Name: Screen Pixel
// Submenu: Distort
// Author: Bleek II & toe_head2001
// Title:
// Version: 1.1
// Desc: Breaks images into their RGB components
// Keywords: subpixel|rgb|screen
// URL: https://forums.getpaint.net/index.php?showtopic=31570
// Help:
#region UICode
IntSliderControl Amount1 = 1; // [1,33] Size
IntSliderControl Amount2 = 100; // [0,255] Opacity
CheckboxControl Amount3 = true; // [0,1] Use Pixelation
#endregion

readonly BinaryPixelOp normalOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Normal);
readonly PixelateEffect pixelateEffect = new PixelateEffect();
PropertyCollection pixelateProps;

void PreRender(Surface dst, Surface src)
{
    if (Amount3)
    {
        int cellAmount = Amount1 * 3;
        pixelateProps = pixelateEffect.CreatePropertyCollection();
        PropertyBasedEffectConfigToken pixelateParameters = new PropertyBasedEffectConfigToken(pixelateProps);
        pixelateParameters.SetPropertyValue(PixelateEffect.PropertyNames.CellSize, cellAmount);
        pixelateEffect.SetRenderInfo(pixelateParameters, new RenderArgs(dst), new RenderArgs(src));
    }
}

void Render(Surface dst, Surface src, Rectangle rect)
{
    if (Amount3)
    {
        pixelateEffect.Render(new Rectangle[1] { rect }, 0, 1);
    }

    int size = Amount1 * 3;
    int cy, cx, y1, x1;
    ColorBgra OriginalImage;
    ColorBgra CurrentPixel;
    for (int y = rect.Top; y < rect.Bottom; y = y + size)
    {
        if (IsCancelRequested) return;
        for (int x = rect.Left; x < rect.Right; x = x + size)
        {
            OriginalImage = (Amount3) ? dst[x, y] : src[x, y];
            CurrentPixel = (Amount3) ? dst[x, y] : src[x, y];

            CurrentPixel.A = (byte)Amount2;

            CurrentPixel.G = 0;
            CurrentPixel.B = 0;
            for (cy = 0; cy < size; cy++)
            {
                for (cx = 0; cx < (size / 3); cx++)
                {
                    x1 = x + cx;
                    y1 = y + cy;
                    if (x1 < rect.Right)
                    {
                        if (y1 < rect.Bottom)
                        {
                            dst[x1, y1] = normalOp.Apply(OriginalImage, CurrentPixel);
                        }
                    }
                }
            }

            CurrentPixel.G = (byte)src[x, y];
            CurrentPixel.R = 0;
            for (cy = 0; cy < size; cy++)
            {
                for (cx = (size / 3); cx < (2 * (size / 3)); cx++)
                {
                    x1 = x + cx;
                    y1 = y + cy;
                    if (x1 < rect.Right)
                    {
                        if (y1 < rect.Bottom)
                        {
                            dst[x1, y1] = normalOp.Apply(OriginalImage, CurrentPixel);
                        }
                    }
                }
            }

            CurrentPixel.B = (byte)src[x, y];
            CurrentPixel.G = 0;
            for (cy = 0; cy < size; cy++)
            {
                for (cx = (2 * (size / 3)); cx < size; cx++)
                {
                    x1 = x + cx;
                    y1 = y + cy;
                    if (x1 < rect.Right)
                    {
                        if (y1 < rect.Bottom)
                        {
                            dst[x1, y1] = normalOp.Apply(OriginalImage, CurrentPixel);
                        }
                    }
                }
            }
        }

    }
}
