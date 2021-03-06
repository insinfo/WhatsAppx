// Copyright © 2010-2017 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;

namespace CefSharp
{
    /// <summary>
    /// Event arguments to the OnPaint event handler.
    /// </summary>
    public class OnPaintEventArgs : EventArgs
    {
        public bool Handled { get; set; }
        public bool IsPopup { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public IntPtr BufferHandle { get; private set; }
        public int BytesPerPixel { get; private set; }
        public int NumberOfBytes { get; set; }
        public Rect DirtyRect { get; private set; }

        /// <summary>
        /// Creates a new OnPaint event arg
        /// </summary>
        /// <param name="isPopup">is popup</param>
        /// <param name="dirtyRect">dirty rect if enabled</param>
        /// <param name="bufferHandle">buffer handle (back buffer)</param>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        /// <param name="bytesPerPixel">bytes per pixel</param>
        /// <param name="numberOfBytes">number of bytes in the buffer</param>
        public OnPaintEventArgs(bool isPopup, Rect dirtyRect, IntPtr bufferHandle, int width, int height, int bytesPerPixel, int numberOfBytes)
        {
            IsPopup = isPopup;
            DirtyRect = dirtyRect;
            BufferHandle = bufferHandle;
            Width = width;
            Height = height;
            BytesPerPixel = bytesPerPixel;
            NumberOfBytes = numberOfBytes;
        }
    }
}

