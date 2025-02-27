﻿using System;

namespace OpenMacroBoard.SDK
{
    /// <summary>
    /// Wraps an <see cref="IMacroBoard"/> and allows for hooks implement features.
    /// </summary>
    public abstract class MacroBoardAdapter : IMacroBoard
    {
        private readonly IMacroBoard macroBoard;
        private readonly bool leaveOpen;

        private bool disposed = false;

        /// <summary>
        /// Creates a new instance of <see cref="MacroBoardAdapter"/>.
        /// </summary>
        /// <remarks>
        /// When this instance is disposed, the underlying board is disposed aswell.
        /// </remarks>
        /// <param name="macroBoard">The macroBoard that is wrapped.</param>
        public MacroBoardAdapter(IMacroBoard macroBoard)
            : this(macroBoard, false)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="MacroBoardAdapter"/>.
        /// </summary>
        /// <param name="macroBoard">The macroBoard that is wrapped.</param>
        /// <param name="leaveOpen">When true, the underlying macroBoard will not be disposed with this instance.</param>
        public MacroBoardAdapter(IMacroBoard macroBoard, bool leaveOpen)
        {
            this.macroBoard = macroBoard ?? throw new ArgumentNullException(nameof(macroBoard));
            this.leaveOpen = leaveOpen;

            macroBoard.ConnectionStateChanged += OnConnectionStateChanged;
            macroBoard.KeyStateChanged += OnKeyStateChanged;
        }

        /// <summary>
        /// Virtual KeyStateChanged event handler.
        /// </summary>
        /// <param name="sender">The sender of the original event.</param>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnKeyStateChanged(object sender, KeyEventArgs e)
        {
            KeyStateChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Virtual ConnectionStateChanged event handler.
        /// </summary>
        /// <param name="sender">The sender of the original event.</param>
        /// <param name="e">The event arguments.</param>
        protected void OnConnectionStateChanged(object sender, ConnectionEventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, e);
        }

        /// <inheritdoc/>
        public virtual IKeyPositionCollection Keys => macroBoard.Keys;

        /// <inheritdoc/>
        public virtual bool IsConnected => macroBoard.IsConnected;

        /// <inheritdoc/>
        public virtual event EventHandler<KeyEventArgs> KeyStateChanged;

        /// <inheritdoc/>
        public virtual event EventHandler<ConnectionEventArgs> ConnectionStateChanged;

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~MacroBoardAdapter()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public virtual void SetBrightness(byte percent)
        {
            macroBoard.SetBrightness(percent);
        }

        /// <inheritdoc/>
        public virtual void SetKeyBitmap(int keyId, KeyBitmap bitmapData)
        {
            macroBoard.SetKeyBitmap(keyId, bitmapData);
        }

        /// <inheritdoc/>
        public virtual void ShowLogo()
        {
            macroBoard.ShowLogo();
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">True when called from <see cref="Dispose()"/> and false when called from the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (!leaveOpen)
                {
                    macroBoard?.Dispose();
                }
            }

            disposed = true;
        }
    }
}
