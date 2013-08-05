﻿#region The MIT License - Copyright (C) 2012-2013 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions Inc. - Hex-Grid Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2013 Pieter Geerkens (email: pgeerkens@hotmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, 
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:
//     The above copyright notice and this permission notice shall be 
//     included in all copies or substantial portions of the Software.
// 
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//     EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//     OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
//     NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
//     HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
//     WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//     FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//     OTHER DEALINGS IN THE SOFTWARE.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Drawing;
using System.Threading.Tasks;

using PGNapoleonics.HexUtilities;
using PGNapoleonics.HexUtilities.Common;
using PGNapoleonics.HexUtilities.Pathfinding;

namespace PGNapoleonics.HexUtilities.FieldOfView {
  /// <summary>Interface required to make use of ShadowCasting Field-of-View calculation.</summary>
  public interface IFovBoard<out THex> where THex : IHex {
    /// <summary>Distance in hexes out to which Field-of-View is to be calculated.</summary>
    int      FovRadius             { get; set; }

    /// <summary>The rectangular extent of the board's hexagonal grid, in hexes.</summary>
    Size     MapSizeHexes             { get; }

    /// <summary>Returns the <c>IHex</c> at location <c>coords</c>.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
    THex     this[HexCoords  coords] { get; }

    /// <summary>Returns whether the hex at location <c>coords</c>is "on board".</summary>
    bool     IsOnboard(HexCoords coords);

    /// <summary>Returns whether the hex at location <c>coords</c> is passable.</summary>
    /// <param name="coords"></param>
    bool     IsPassable(HexCoords coords);
  }

  /// <summary>Extension methods for interface IFovBoard&lt;IHex&gt;.</summary>
  public static class FovBoardExtensions {
    /// <summary>Returns the field-of-view on <c>board</c> from the hex specified by coordinates <c>coords</c>.</summary>
    [Obsolete("Use GetFieldOfView(HexCoords) instead.")]
    public static IFov GetFov(this IFovBoard<IHex> @this, HexCoords origin) {
      return @this.GetFieldOfView(origin);
    }

    /// <summary>Gets a Field-of-View for this board asynchronously.</summary>
    public static Task<IFov> GetFieldOfViewAsync(this IFovBoard<IHex> @this, HexCoords origin) {
      return @this.GetFieldOfViewAsync(origin, FovTargetMode.EqualHeights);
    }

    /// <summary>Gets a Field-of-View for this board asynchronously.</summary>
    public static Task<IFov> GetFieldOfViewAsync(this IFovBoard<IHex> @this, HexCoords origin, FovTargetMode targetMode) {
      return Task.Run<IFov>(
        () => @this.GetFieldOfView(origin, targetMode)
      );
    }
    
    /// <summary>Gets a Field-of-View for this board synchronously.</summary>
    public static IFov GetFieldOfView(this IFovBoard<IHex> @this, HexCoords origin) {
      if (@this==null) throw new ArgumentNullException("this");
      return @this.GetFieldOfView(origin, FovTargetMode.EqualHeights);
    }

    /// <summary>Gets a Field-of-View for this board synchronously.</summary>
    public static IFov GetFieldOfView(this IFovBoard<IHex> @this, HexCoords origin, FovTargetMode targetMode) {
      TraceFlags.FieldOfView.Trace("GetFieldOfView");
      var fov = new ArrayFieldOfView(@this);
      if (@this.IsPassable(origin))
        ShadowCasting.ComputeFieldOfView(origin, @this, targetMode, coords => fov[coords] = true);

      return fov;
    } 
  }

  /// <summary>TODO</summary>
  [Obsolete("Use Extension methods instead.")]
  public static class FovFactory {
    /// <summary>TODO</summary>
  [Obsolete("Use Extension methods instead.")]
    public static IFov GetFieldOfView(IFovBoard<IHex> board, HexCoords origin) {
      if (board==null) throw new ArgumentNullException("board");
      return board.GetFieldOfView(origin);
    }
    /// <summary>TODO</summary>
  [Obsolete("Use Extension methods instead.")]
    public static IFov GetFieldOfView(IFovBoard<IHex> board, HexCoords origin, FovTargetMode targetMode) {
      return board.GetFieldOfView(origin, targetMode);
    }
  }
}