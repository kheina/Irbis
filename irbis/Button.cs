﻿using Irbis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Button
{
    public Rectangle bounds;
    Direction alignSide;
    public string highlightStatement;
    public string originalStatement;
    public string buttonStatement;
    public string data;
    Color buttonBorderColor;
    public Print text;

    bool drawBorder;

    Texture2D borderTex;

    //MouseState prevMouseState;

    public Point buttonLocation;

    float depth;

    public Button(Rectangle buttonBounds, Direction align, string buttonText, string highlightText, Color borderColor, Texture2D borderTexture, Font font, Color fillColor, bool AlignRectangle, float drawDepth)
	{
        //if (Irbis.Irbis.debug > 4) { Irbis.Irbis.methodLogger.AppendLine("Irbis.Button"); }
        if (AlignRectangle)
        {
            if (align == Direction.left)
            {
                bounds = buttonBounds;
            }
            else if (align == Direction.right)
            {
                bounds = new Rectangle(new Point(buttonBounds.X - buttonBounds.Width, buttonBounds.Y), buttonBounds.Size);
            }
            else
            {
                bounds = new Rectangle(new Point(buttonBounds.X - (buttonBounds.Width / 2), buttonBounds.Y), buttonBounds.Size);
            }
        }
        else
        {
            bounds = buttonBounds;
        }

        alignSide = align;
        data = originalStatement = buttonStatement = buttonText;
        buttonBorderColor = borderColor;
        highlightStatement = highlightText;
        depth = drawDepth;

        borderTex = borderTexture;
        buttonLocation = bounds.Center;


        text = new Print(bounds.Width, font, Color.White, false, buttonLocation, Direction.forward, depth);
        text.Update(originalStatement);

        drawBorder = true;
    }

    public Button(Rectangle buttonBounds, Direction align, string buttonText, string highlightText, Color borderColor, Texture2D borderTexture, Font font, Color fillColor, bool AlignRectangle, bool dBorder, float drawDepth)
    {
        //if (Irbis.Irbis.debug > 4) { Irbis.Irbis.methodLogger.AppendLine("Irbis.Button"); }

        if (AlignRectangle)
        {
            if (align == Direction.left)
            {
                bounds = buttonBounds;
            }
            else if (align == Direction.right)
            {
                bounds = new Rectangle(new Point(buttonBounds.X - buttonBounds.Width, buttonBounds.Y), buttonBounds.Size);
            }
            else
            {
                bounds = new Rectangle(new Point(buttonBounds.X - (buttonBounds.Width / 2), buttonBounds.Y), buttonBounds.Size);
            }
        }
        else
        {
            bounds = buttonBounds;
        }

        alignSide = align;
        data = originalStatement = buttonStatement = buttonText;
        highlightStatement = highlightText;

        buttonBorderColor = borderColor;
        depth = drawDepth;

        borderTex = borderTexture;
        buttonLocation = bounds.Center;


        text = new Print(bounds.Width, font, Color.White, false, buttonLocation, align, depth);
        text.Update(buttonStatement);
        drawBorder = dBorder;
    }

    public Button(Rectangle buttonBounds, Direction align, Side side, string buttonText, string highlightText, Color borderColor, Texture2D borderTexture, Font font, Color fillColor, bool AlignRectangle, bool dBorder, float drawDepth)
    {
        //if (Irbis.Irbis.debug > 4) { Irbis.Irbis.methodLogger.AppendLine("Button.Button"); }
        if (AlignRectangle)
        {
            if (align == Direction.left)
            {
                bounds = buttonBounds;
            }
            else if (align == Direction.right)
            {
                bounds = new Rectangle(new Point(buttonBounds.X - buttonBounds.Width, buttonBounds.Y), buttonBounds.Size);
            }
            else
            {
                bounds = new Rectangle(new Point(buttonBounds.X - (buttonBounds.Width / 2), buttonBounds.Y), buttonBounds.Size);
            }
        }
        else
        {
            bounds = buttonBounds;
        }

        alignSide = align;
        data = originalStatement = buttonStatement = buttonText;
        highlightStatement = highlightText;

        buttonBorderColor = borderColor;
        depth = drawDepth;

        borderTex = borderTexture;
        buttonLocation = bounds.Center;

        if (side == Side.left)
        {
            text = new Print(bounds.Width, font, Color.White, false, new Point(bounds.Left, bounds.Center.Y), align, depth);
        }
        else if (side == Side.right)
        {
            text = new Print(bounds.Width, font, Color.White, false, new Point(bounds.Right, bounds.Center.Y), align, depth);
        }
        else if(side == Side.top)
        {
            text = new Print(bounds.Width, font, Color.White, false, new Point(bounds.Center.X, bounds.Top), align, depth);
        }
        else if (side == Side.bottom)
        {
            text = new Print(bounds.Width, font, Color.White, false, new Point(bounds.Center.X, bounds.Bottom), align, depth);
        }

        text.Update(buttonStatement);
        drawBorder = dBorder;
    }

    public bool Pressed(MouseState mouseState)
    {
        //if (Irbis.Irbis.debug > 4) { Irbis.Irbis.methodLogger.AppendLine("Button.Pressed"); }
        if (bounds.Contains(mouseState.Position.X, mouseState.Position.Y) && mouseState.LeftButton == ButtonState.Pressed)
        {
            return true;
        }
        return false;
    }

    public bool Pressed(MouseState mouseState, MouseState previousMouseState)
    {
        //if (Irbis.Irbis.debug > 4) { Irbis.Irbis.methodLogger.AppendLine("Button.Pressed"); }
        if (bounds.Contains(mouseState.Position.X, mouseState.Position.Y) && mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton != ButtonState.Pressed)
        {
            return true;
        }
        return false;
    }

    public void Update(string statement)
    {
        //if (Irbis.Irbis.debug > 4) { Irbis.Irbis.methodLogger.AppendLine("Button.Update"); }
        buttonStatement = statement;
        text.Update(statement, true);
    }

    public void Update(string statement, bool clear)
    {
        //if (Irbis.Irbis.debug > 4) { Irbis.Irbis.methodLogger.AppendLine("Button.Update"); }
        if (clear)
        {
            buttonStatement = statement;
        }
        else
        {
            buttonStatement += statement;
        }
        text.Update(buttonStatement, true);
    }

    public bool Contains(MouseState mouseState)
    {
        //if (Irbis.Irbis.debug > 4) { Irbis.Irbis.methodLogger.AppendLine("Button.Contains"); }
        return bounds.Contains(mouseState.Position.X, mouseState.Position.Y);
    }

    public void Draw(SpriteBatch sb)
    {
        //if (Irbis.Irbis.debug > 4) { Irbis.Irbis.methodLogger.AppendLine("Button.Draw"); }
        text.Draw(sb);
        if (drawBorder) { RectangleBorder.Draw(sb, bounds, buttonBorderColor, false); }
    }
}
