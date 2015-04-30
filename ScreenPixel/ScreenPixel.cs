using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Drawing;
using Microsoft.Win32;
using System.Reflection;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using PaintDotNet;
using PaintDotNet.Data;
using PaintDotNet.Effects;
using PaintDotNet.AppModel;
using PaintDotNet.IndirectUI;
using PaintDotNet.Collections;
using PaintDotNet.PropertySystem;

[assembly: AssemblyTitle("Screen Pixel Plugin for Paint.NET")]
[assembly: AssemblyDescription("Takes a image and breaks it into RGB components")]
[assembly: AssemblyConfiguration("subpixel|rgb|screen")]
[assembly: AssemblyCompany("Bleek II & toe_head2001")]
[assembly: AssemblyProduct("Screen Pixel")]
[assembly: AssemblyCopyright("Copyright © Bleek II & toe_head2001")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.1.*")]

namespace ScreenPixelEffect
{
    public class PluginSupportInfo : IPluginSupportInfo
    {
        public string Author
        {
            get
            {
                return ((AssemblyCopyrightAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]).Copyright;
            }
        }
        public string Copyright
        {
            get
            {
                return ((AssemblyDescriptionAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0]).Description;
            }
        }

        public string DisplayName
        {
            get
            {
                return ((AssemblyProductAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0]).Product;
            }
        }

        public Version Version
        {
            get
            {
                return base.GetType().Assembly.GetName().Version;
            }
        }

        public Uri WebsiteUri
        {
            get
            {
                return new Uri("http://www.getpaint.net/redirect/plugins.html");
            }
        }
    }

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "Screen Pixel")]

    public class ScreenPixelEffectPlugin : PropertyBasedEffect
    {
        public static string StaticName
        {
            get
            {
                return "Screen Pixel";
            }
        }

        public static Image StaticIcon
        {
            get
            {
                return new Bitmap(typeof(ScreenPixelEffectPlugin), "ScreenPixel.png");
            }
        }

        public static string SubmenuName
        {
            get
            {
                return SubmenuNames.Distort;  // Programmer's chosen default
            }
        }

        public ScreenPixelEffectPlugin()
            : base(StaticName, StaticIcon, SubmenuName, EffectFlags.Configurable)
        {
        }

        public enum PropertyNames
        {
            Amount1,
            Amount2,
            Amount3
        }


        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            props.Add(new Int32Property(PropertyNames.Amount1, 1, 1, 33));
            props.Add(new Int32Property(PropertyNames.Amount2, 100, 0, 255));
            props.Add(new BooleanProperty(PropertyNames.Amount3, true));

            return new PropertyCollection(props);
        }

        protected override ControlInfo OnCreateConfigUI(PropertyCollection props)
        {
            ControlInfo configUI = CreateDefaultConfigUI(props);

            configUI.SetPropertyControlValue(PropertyNames.Amount1, ControlInfoPropertyNames.DisplayName, "Size");
            configUI.SetPropertyControlValue(PropertyNames.Amount2, ControlInfoPropertyNames.DisplayName, "Opacity");
            configUI.SetPropertyControlValue(PropertyNames.Amount3, ControlInfoPropertyNames.DisplayName, string.Empty);
            configUI.SetPropertyControlValue(PropertyNames.Amount3, ControlInfoPropertyNames.Description, "Use Pixelation");

            return configUI;
        }

        protected override void OnSetRenderInfo(PropertyBasedEffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            this.Amount1 = newToken.GetProperty<Int32Property>(PropertyNames.Amount1).Value;
            this.Amount2 = newToken.GetProperty<Int32Property>(PropertyNames.Amount2).Value;
            this.Amount3 = newToken.GetProperty<BooleanProperty>(PropertyNames.Amount3).Value;

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);

            if (cellSurface == null)
            {
                cellSurface = new Surface(dstArgs.Size);
            }
        }

        protected override unsafe void OnRender(Rectangle[] rois, int startIndex, int length)
        {
            if (length == 0) return;
            for (int i = startIndex; i < startIndex + length; ++i)
            {
                Render(DstArgs.Surface, SrcArgs.Surface, rois[i]);
            }
        }

        #region User Entered Code
        #region UICode
        int Amount1 = 1; // [1,33] Size
        int Amount2 = 100; // [0,255] Opacity
        bool Amount3 = true; // [0,1] Use Pixelation
        #endregion

        private Surface cellSurface = null;

        private BinaryPixelOp normalOp = LayerBlendModeUtil.CreateCompositionOp(LayerBlendMode.Normal);

        unsafe void Render(Surface dst, Surface src, Rectangle rect)
        {
            if (Amount3 == true)
            {
                int cellAmount = Amount1 * 3;
                PixelateEffect cellEffect = new PixelateEffect();
                PropertyCollection cellProps = cellEffect.CreatePropertyCollection();
                PropertyBasedEffectConfigToken cellParameters = new PropertyBasedEffectConfigToken(cellProps);
                cellParameters.SetPropertyValue(PixelateEffect.PropertyNames.CellSize, cellAmount);
                cellEffect.SetRenderInfo(cellParameters, new RenderArgs(cellSurface), new RenderArgs(src));
                cellEffect.Render(new Rectangle[1] { rect }, 0, 1);

                src = cellSurface;
            }



            int amo1 = Amount1 * 3;
            int cy, cx, y1, x1;
            ColorBgra CP1;
            ColorBgra CP2;
            for (int y = rect.Top; y < rect.Bottom; y = y + amo1)
            {
                for (int x = rect.Left; x < rect.Right; x = x + amo1)
                {
                    CP2 = src[x, y];
                    CP1 = src[x, y];
                    CP1.A = (byte)Amount2;

                    CP1.G = 0;
                    CP1.B = 0;
                    for (cy = 0; cy < amo1; cy++)
                    {
                        for (cx = 0; cx < (amo1 / 3); cx++)
                        {
                            x1 = x + cx;
                            y1 = y + cy;
                            if (x1 < rect.Right)
                            {
                                if (y1 < rect.Bottom)
                                {
                                    dst[x1, y1] = normalOp.Apply(CP2, CP1);
                                }
                            }
                        }
                    }

                    CP1.G = (byte)src[x, y];
                    CP1.R = 0;
                    for (cy = 0; cy < amo1; cy++)
                    {
                        for (cx = (amo1 / 3); cx < (2 * (amo1 / 3)); cx++)
                        {
                            x1 = x + cx;
                            y1 = y + cy;
                            if (x1 < rect.Right)
                            {
                                if (y1 < rect.Bottom)
                                {
                                    dst[x1, y1] = normalOp.Apply(CP2, CP1);
                                }
                            }
                        }
                    }

                    CP1.B = (byte)src[x, y];
                    CP1.G = 0;
                    for (cy = 0; cy < amo1; cy++)
                    {
                        for (cx = (2 * (amo1 / 3)); cx < amo1; cx++)
                        {
                            x1 = x + cx;
                            y1 = y + cy;
                            if (x1 < rect.Right)
                            {
                                if (y1 < rect.Bottom)
                                {
                                    dst[x1, y1] = normalOp.Apply(CP2, CP1);
                                }
                            }
                        }
                    }
                }

            }
        }

        #endregion
    }
}
