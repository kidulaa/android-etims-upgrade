using SkiaSharp;
using System;
using System.Text;

namespace EBM2x.UI.Draw
{
    public class DrawTools
    {
        public static void PaintSurface(SKCanvas canvas, float width, float height, string text, float fillRate, string textAlign, SKColor color)
        {
            // clear the surface
            canvas.Clear(SKColors.Transparent);
            PaintSurfaceAppend(canvas, width, height, text, fillRate, textAlign, color);
        }
        public static void PaintSurfaceAppend(SKCanvas canvas, float width, float height, string text, float fillRate, string textAlign, SKColor color)
        {
            using (var paint = new SKPaint())
            {
                paint.TextSize = height * fillRate;
                paint.Color = color;

                float xOffset = 0;
                float yOffset = (height / 2) + (paint.TextSize / 2) - (paint.TextSize * 0.15f);

                paint.IsAntialias = true;
                if (textAlign != null && textAlign.ToUpper().Equals("CENTER"))
                {
                    paint.TextAlign = SKTextAlign.Center;
                    xOffset = width / 2;
                }
                else if (textAlign != null && textAlign.ToUpper().Equals("RIGHT"))
                {
                    paint.TextAlign = SKTextAlign.Right;
                    xOffset = width - (paint.TextSize / 3);
                }
                else
                {
                    paint.TextAlign = SKTextAlign.Left;
                    xOffset = 0;
                }

                if (text != null)
                {
                    string[] result = text.Split('|');
                    if (result.Length == 1)
                    {
                        // 기본 출력 함수
                        canvas.DrawText(text, xOffset, yOffset, paint);
                    }
                    else
                    {
                        yOffset = (height / 2) - ((result.Length * paint.TextSize) / 2) + paint.TextSize - (paint.TextSize * 0.15f); ;

                        foreach (string s in result)
                        {
                            // 기본 출력 함수
                            canvas.DrawText(s, xOffset, yOffset, paint);

                            yOffset += paint.TextSize;
                        }
                    }
                }
            }
        }
        public static void PaintSurfaceKr(SKCanvas canvas, float width, float height, string text, float fillRate, string textAlign, SKColor color)
        {
            // clear the surface
            canvas.Clear(SKColors.Transparent);

            using (var paint = new SKPaint())
            {
                paint.TextSize = height * (fillRate - 0.1f);
                paint.Color = color;

                float xOffset = 0;
                float yOffset = 0;

                paint.IsAntialias = true;
                if (textAlign != null && textAlign.ToUpper().Equals("CENTER"))
                {
                    paint.TextAlign = SKTextAlign.Center;
                    xOffset = width / 2;
                }
                else if (textAlign != null && textAlign.ToUpper().Equals("RIGHT"))
                {
                    paint.TextAlign = SKTextAlign.Right;
                    xOffset = width - (paint.TextSize / 3);
                }
                else
                {
                    paint.TextAlign = SKTextAlign.Left;
                    xOffset = 0;
                }

                if (text != null)
                {
                    string[] result = text.Split('|');
                    if (result.Length == 1)
                    {
                        // 한글 출력 함수
                        DrawTools.DrawString(canvas, text, (int)paint.TextSize, SKColors.Black, 0, yOffset);
                    }
                    else
                    {
                        foreach (string s in result)
                        {
                            // 한글 출력 함수
                            DrawTools.DrawString(canvas, s, (int)paint.TextSize, SKColors.Black, 0, yOffset);

                            yOffset += paint.TextSize;
                        }
                    }
                }
            }
        }

        public static void DrawLine(SKCanvas canvas, SKColor color, float x1, float y1, float x2, float y2)
        {
            SKPaint sKPaint = new SKPaint();
            sKPaint.Color = color;

            canvas.DrawLine(x1, y1, x2, y2, sKPaint);
        }
        public static void DrawLine(SKCanvas canvas, SKColor color, SKPoint pt1, SKPoint pt2)
        {
            DrawLine(canvas, color, (float)pt1.X, (float)pt1.Y, (float)pt2.X, (float)pt2.Y);
        }

        public static void DrawLine(SKCanvas canvas, SKColor color, int x1, int y1, int x2, int y2)
        {
            DrawLine(canvas, color, (float)x1, (float)y1, (float)x2, (float)y2);
        }
        public static void DrawLines(SKCanvas canvas, SKColor color, SKPoint[] points)
        {
            if (points.Length < 2)
                return;
            SKPoint point1 = points[0];
            SKPoint point2;
            for (int i = 1; i < points.Length; i++)
            {
                point2 = points[i];
                DrawLine(canvas, color, point1, point2);
                point1 = point2;
            }
        }

        public static void DrawRectangle(SKCanvas canvas, SKColor color, float x, float y, float width, float height)
        {
            SKPaint sKPaint = new SKPaint();
            sKPaint.Color = color;
            sKPaint.Style = SKPaintStyle.Stroke;
            sKPaint.StrokeWidth = 1;

            canvas.DrawRect(x, y, width, height, sKPaint);
        }

        public static void DrawRectangle(SKCanvas canvas, SKColor color, SKRect rect)
        {
            DrawRectangle(canvas, color, rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public static void DrawRectangle(SKCanvas canvas, SKColor color, int x, int y, int width, int height)
        {
            DrawRectangle(canvas, color, (float)x, (float)y, (float)width, (float)height);
        }

        public static void DrawEllipse(SKCanvas canvas, SKColor color, float x, float y, float width, float height)
        {
            SKPaint sKPaint = new SKPaint();
            sKPaint.Color = color;
            sKPaint.Style = SKPaintStyle.Stroke;
            sKPaint.StrokeWidth = 1;

            GetHalfSize(ref x, ref y, ref width, ref height);
            canvas.DrawOval(x, y, width, height, sKPaint);
        }
        public static void DrawEllipse(SKCanvas canvas, SKColor color, SKRect rect)
        {
            DrawEllipse(canvas, color, (float)rect.Left, (float)rect.Top, (float)rect.Width, (float)rect.Height);
        }
        public static void DrawEllipse(SKCanvas canvas, SKColor color, int x, int y, int width, int height)
        {
            DrawEllipse(canvas, color, (float)x, (float)y, (float)width, (float)height);
        }

        public static void DrawPolygon(SKCanvas canvas, SKColor color, SKPoint[] points)
        {
            SKPaint sKPaint = new SKPaint();
            sKPaint.Color = color;

            canvas.DrawPoints(SKPointMode.Polygon, points, sKPaint);
        }

        public static void FillRectangle(SKCanvas canvas, SKColor color, float x, float y, float width, float height)
        {
            SKPaint sKPaint = new SKPaint();
            sKPaint.Color = color;

            canvas.DrawRect(x, y, width, height, sKPaint);
        }

        public static void FillRectangle(SKCanvas canvas, SKColor color, SKRect rect)
        {
            FillRectangle(canvas, color, rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public static void FillRectangle(SKCanvas canvas, SKColor color, int x, int y, int width, int height)
        {
            FillRectangle(canvas, color, (float)x, (float)y, (float)width, (float)height);
        }

        public static void FillRectangle(SKCanvas canvas, SKColor color, SKRect[] rects)
        {
            foreach (SKRect rect in rects)
                FillRectangle(canvas, color, rect);
        }

        static void GetHalfSize(ref float x, ref float y, ref float width, ref float height)
        {
            x = x + width / 2;      //Center
            width /= 2;
            y = y + height / 2;     //Center
            height /= 2;
        }
        public static void FillEllipse(SKCanvas canvas, SKColor color, float x, float y, float width, float height)
        {
            SKPaint sKPaint = new SKPaint();
            sKPaint.Color = color;

            GetHalfSize(ref x, ref y, ref width, ref height);
            canvas.DrawOval(x, y, width, height, sKPaint);
        }
        public static void FillEllipse(SKCanvas canvas, SKColor color, int x, int y, int width, int height)
        {
            FillEllipse(canvas, color, (float)x, (float)y, (float)width, (float)height);
        }

        public static void FillEllipse(SKCanvas canvas, SKColor color, SKRect rect)
        {
            FillEllipse(canvas, color, (float)rect.Left, (float)rect.Top, (float)rect.Width, (float)rect.Height);
        }

        static void DrawPath(SKCanvas canvas, SKPoint[] skPoints, SKPaint sKPaint)
        {
            SKPath skPath;
            skPath = new SKPath();
            skPath.AddPoly(skPoints);
            canvas.DrawPath(skPath, sKPaint);
        }

        public static void FillPolygon(SKCanvas canvas, SKColor color, SKPoint[] points)
        {
            SKPaint sKPaint = new SKPaint();
            sKPaint.Color = color;

            DrawPath(canvas, points, sKPaint);
        }

        public static void DrawPopBox(SKCanvas canvas, SKColor color, SKRect rectPos)
        {
            SKColor colorBound = new SKColor(0, 0, 0);
            SKColor colorLight = new SKColor(255, 255, 255);
            SKColor colorShadow = new SKColor(128, 128, 128);

            FillRectangle(canvas, color, rectPos);

            DrawRectangle(canvas, colorBound, rectPos);

            DrawLine(canvas, colorLight, rectPos.Left + 1, rectPos.Top + 1, rectPos.Left + 1, rectPos.Bottom - 1);
            DrawLine(canvas, colorLight, rectPos.Left + 1, rectPos.Top + 1, rectPos.Right - 1, rectPos.Top + 1);

            DrawLine(canvas, colorShadow, rectPos.Right - 1, rectPos.Top + 1, rectPos.Right - 1, rectPos.Bottom - 1);
            DrawLine(canvas, colorShadow, rectPos.Left + 1, rectPos.Bottom - 1, rectPos.Right - 1, rectPos.Bottom - 1);
        }


        public static void DrawPushBox(SKCanvas canvas, SKColor color, SKRect rectPos)
        {
            SKColor colorBound = new SKColor(0, 0, 0);
            SKColor colorLight = new SKColor(255, 255, 255);
            SKColor colorShadow = new SKColor(128, 128, 128);

            FillRectangle(canvas, color, rectPos);
            DrawLine(canvas, colorShadow, rectPos.Left, rectPos.Top, rectPos.Left, rectPos.Bottom);
            DrawLine(canvas, colorShadow, rectPos.Left, rectPos.Top, rectPos.Right, rectPos.Top);

            DrawLine(canvas, colorLight, rectPos.Right, rectPos.Top, rectPos.Right, rectPos.Bottom);
            DrawLine(canvas, colorLight, rectPos.Left, rectPos.Bottom, rectPos.Right, rectPos.Bottom);

            rectPos.Inflate(-1, -1);
            DrawRectangle(canvas, colorBound, rectPos);
        }

        static float GetDrawStringXPos(SKRect layoutRectangle, SKTextAlign textAlign)
        {
            if (textAlign == SKTextAlign.Left)
                return layoutRectangle.Left;
            else if (textAlign == SKTextAlign.Right)
                return layoutRectangle.Right;
            return layoutRectangle.Left + layoutRectangle.Width / 2;
        }

        static float GetDrawStringYPos(int nTextSize, SKRect layoutRectangle, SKTextAlign textAlign)
        {
            if (textAlign == SKTextAlign.Left)
                return layoutRectangle.Top + nTextSize / 3;
            else if (textAlign == SKTextAlign.Right)
                return layoutRectangle.Bottom - nTextSize;
            return layoutRectangle.Top + layoutRectangle.Height / 2 - nTextSize / 2;
        }

        public static void DrawString(SKCanvas canvas, string s, int nTextSize, SKColor color, SKPoint point)
        {
            DrawString(canvas, s, nTextSize, color, point.X, point.Y);
        }
        public static void DrawString(SKCanvas canvas, string s, int nTextSize, SKColor color, SKRect layoutRectangle)
        {
            canvas.Save();
            canvas.ClipRect(layoutRectangle);
            float fTop = GetDrawStringYPos(nTextSize, layoutRectangle, SKTextAlign.Center);
            DrawString(canvas, s, nTextSize, color, layoutRectangle.Left, fTop);
            canvas.Restore();
        }

        static float DrawTextKorea(SKCanvas canvas, string strText, int nTextSize, SKColor color, float x, float y, bool bOnlyMeasure)
        {
            var tf = SKFontManager.Default.MatchCharacter('헬');
            SKPaint sKPaint = new SKPaint { Typeface = tf };
            sKPaint.TextSize = nTextSize;
            if (bOnlyMeasure == false)
            {
                sKPaint.Color = color;
                canvas.DrawText(strText, x, y, sKPaint);
            }
            return sKPaint.MeasureText(strText);
        }


        static float DrawTextEnglish(SKCanvas canvas, string strText, int nTextSize, SKColor color, float x, float y, bool bOnlyMeasure)
        {
            SKPaint sKPaint = new SKPaint();
            sKPaint.TextSize = nTextSize;
            if (bOnlyMeasure == false)
            {
                sKPaint.Color = color;
                canvas.DrawText(strText, x, y, sKPaint);
            }
            return sKPaint.MeasureText(strText);
        }

        static float MesaureText(SKCanvas canvas, string strText, int nTextSize)
        {
            return DrawTextCommon(canvas, strText, nTextSize, new SKColor(0, 0, 0), 0, 0, true);         //마지막 인자 true일 경우 길이만 구한다
        }

        static float DrawTextCommon(SKCanvas canvas, string strText, int nTextSize, SKColor color, float x, float y, bool bOnlyMeasure = false)
        {
            try
            {
                //                EncodingInfo[] fonts = Encoding.GetEncodings();
                byte[] pText = Encoding.GetEncoding("ks_c_5601-1987").GetBytes(strText);
                int nPos = 0;
                int nCount = 0;
                bool bStart = true;
                LanguageKind languageKind = LanguageKind.IDLE;
                LanguageKind languageKindBuff;
                float fWidth = 0;
                string strTextBuff;
                for (int i = 0; i < pText.Length; i++)
                {
                    if (pText[i] >= 0x80)
                    {
                        languageKindBuff = LanguageKind.KOREA;
                        i++;
                    }
                    else
                        languageKindBuff = LanguageKind.ENGLISH;
                    if (bStart == true)
                    {
                        languageKind = languageKindBuff;
                        nCount++;
                        bStart = false;
                        continue;
                    }
                    if (languageKind == languageKindBuff)
                    {
                        nCount++;
                        continue;
                    }

                    strTextBuff = strText.Substring(nPos, nCount);
                    if (languageKind == LanguageKind.KOREA)
                        fWidth += DrawTextKorea(canvas, strTextBuff, nTextSize, color, x + fWidth, y, bOnlyMeasure);
                    else
                        fWidth += DrawTextEnglish(canvas, strTextBuff, nTextSize, color, x + fWidth, y, bOnlyMeasure);

                    languageKind = languageKindBuff;
                    nPos = nPos + nCount;
                    nCount = 1;
                }
                if (nCount > 0)
                {
                    strTextBuff = strText.Substring(nPos, nCount);
                    if (languageKind == LanguageKind.KOREA)
                        fWidth += DrawTextKorea(canvas, strTextBuff, nTextSize, color, x + fWidth, y, bOnlyMeasure);
                    else
                        fWidth += DrawTextEnglish(canvas, strTextBuff, nTextSize, color, x + fWidth, y, bOnlyMeasure);
                }
                return fWidth;
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return 0;
        }

        public static void DrawString(SKCanvas canvas, string s, int nTextSize, SKColor color, float x, float y)
        {
            //Modified 2018/05/30
            y = y + nTextSize * 4 / 3;
            //y = y + font.Size / 3;
            DrawTextCommon(canvas, s, nTextSize, color, x, y);
        }
        public void DrawString(SKCanvas canvas, string s, int nTextSize, SKColor color, int x, int y)
        {
            DrawString(canvas, s, nTextSize, color, (float)x, (float)y);
        }
        public static void DrawString(SKCanvas canvas, string s, int nTextSize, SKColor color, SKPoint point, SKTextAlign textAlign)
        {
            DrawString(canvas, s, nTextSize, color, point.X, point.Y, textAlign);
        }
        public static void DrawString(SKCanvas canvas, string s, int nTextSize, SKColor color, SKRect layoutRectangle, SKTextAlign textAlign, SKTextAlign lineAlign)
        {
            canvas.Save();
            canvas.ClipRect(layoutRectangle);
            float posX = GetDrawStringXPos(layoutRectangle, textAlign);
            float posY = GetDrawStringYPos(nTextSize, layoutRectangle, lineAlign);
            //Modified 2018/05/21
            DrawString(canvas, s, nTextSize, color, posX, posY - nTextSize / 3, textAlign);
            //DrawString(s, font, brush, posX, posY - font.Size *2 / 3, format);
            canvas.Restore();
        }
        public static void DrawString(SKCanvas canvas, string s, int nTextSize, SKColor color, float x, float y, SKTextAlign textAlign)
        {
            //Modified 2018/05/24
            y = y + nTextSize * 4 / 3;
            //y = y + font.Size;
            if (textAlign == SKTextAlign.Center)
                x -= (MesaureText(canvas, s, nTextSize) / 2);
            else if (textAlign == SKTextAlign.Right)
                x -= (MesaureText(canvas, s, nTextSize));
            DrawTextCommon(canvas, s, nTextSize, color, x, y);
        }
    }
    public enum LanguageKind
    {
        KOREA,
        ENGLISH,
        IDLE
    }
}
