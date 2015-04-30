int Amount1=1;	//[1,20]Size: use with Pixelate x3


void Render(Surface dst, Surface src, Rectangle rect)
{

    int amo1=Amount1*3;
    int cy,cx,y1,x1;
    ColorBgra CP1;
    for(int y = rect.Top; y < rect.Bottom; y=y+amo1)
    {
        for (int x = rect.Left; x < rect.Right; x=x+amo1)
        {
            CP1=src[x,y];
            CP1.G=0;
            CP1.B=0;
            for(cy=0;cy<amo1;cy++)
            {
            for(cx=0;cx<(amo1/3);cx++)
            {
            x1=x+cx;
            y1=y+cy;
            if(x1<rect.Right)
            {
            if(y1<rect.Bottom)
            {            
            dst[x1,y1] = CP1;
            }
            }
            }        
            }
            CP1.G=(byte)src[x,y];
            CP1.R=0;
            for(cy=0;cy<amo1;cy++)
            {
            for(cx=(amo1/3);cx<(2*(amo1/3));cx++)
            {
            x1=x+cx;
            y1=y+cy;
            if(x1<rect.Right)
            {
            if(y1<rect.Bottom)
            {            
            dst[x1,y1] = CP1;
            }
            }
            }
            }  
            CP1.B=(byte)src[x,y];
            CP1.G=0;
            for(cy=0;cy<amo1;cy++)
            {
            for(cx=(2*(amo1/3));cx<amo1;cx++)
            {
            x1=x+cx;
            y1=y+cy;
            if(x1<rect.Right)
            {
            if(y1<rect.Bottom)
            {            
            dst[x1,y1] = CP1;
            }
            }
            }
            }       
        }
     
    }
}
