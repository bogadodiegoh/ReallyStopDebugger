﻿// Copyright (c) Luis Gómez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the src\ReallyStopDebugger directory for full license information.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ReallyStopDebugger.Native
{
    public static class ImageExtensions
    {
        /// <summary>
        /// Converts a <see cref="Icon"/> into a WPF <see cref="BitmapSource"/>
        /// </summary>
        public static BitmapSource ToBitmapSource(this Icon source)
        {
            BitmapSource bitmapSource;

            using (var bitmap = new Bitmap(source.ToBitmap()))
            {
                bitmapSource = bitmap.ToBitmapSource();
                bitmapSource.Freeze();
            }

            return bitmapSource;
        }

        /// <summary>
        /// Converts a <see cref="Bitmap"/> into a WPF <see cref="BitmapSource"/>
        /// </summary>
        public static BitmapSource ToBitmapSource(this Bitmap source)
        {
            BitmapSource bitmapSource;
            var hBitmap = source.GetHbitmap();

            using (new SafeHandles.SafeGDIObjectHandle(hBitmap))
            {
                try
                {
                    bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                        hBitmap,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

                    bitmapSource.Freeze();
                }
                catch (Win32Exception)
                {
                    bitmapSource = null;
                }
            }

            return bitmapSource;
        }
    }
}