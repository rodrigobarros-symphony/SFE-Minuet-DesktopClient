﻿//Licensed to the Apache Software Foundation(ASF) under one
//or more contributor license agreements.See the NOTICE file
//distributed with this work for additional information
//regarding copyright ownership.The ASF licenses this file
//to you under the Apache License, Version 2.0 (the
//"License"); you may not use this file except in compliance
//with the License.  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing,
//software distributed under the License is distributed on an
//"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//KIND, either express or implied.  See the License for the
//specific language governing permissions and limitations
//under the License.

using System;
using Paragon.Runtime.Plugins;
using Xilium.CefGlue;

namespace Paragon.Runtime
{
    internal sealed class CefWebClient : CefClient, IDisposable
    {
        private CefContextMenuHandler _contextMenuHandler;
        private ICefWebBrowserInternal _core;
        private CefDisplayHandler _displayHandler;
        private CefDownloadHandler _downloadHandler;
        private CefJSDialogHandler _jsDialogHandler;
        private CefKeyboardHandler _keyboardHandler;
        private CefLifeSpanHandler _lifeSpanHandler;
        private CefLoadHandler _loadHandler;
        private CefRequestHandler _requestHandler;

        public CefWebClient(ICefWebBrowserInternal core)
        {
            _core = core;
            _lifeSpanHandler = new CefWebLifeSpanHandler(_core);
            _displayHandler = new CefWebDisplayHandler(_core);
            _loadHandler = new CefWebLoadHandler(_core);
            _requestHandler = new CefWebRequestHandler(_core);
            _contextMenuHandler = new CefWebContextMenuHandler(_core);
            _downloadHandler = new CefWebDownloadHandler(_core);
            _jsDialogHandler = new CefWebJSDialogHandler(_core);
            _keyboardHandler = new CefWebKeyboardHandler(_core);
        }

        public void Dispose()
        {
            _core = null;

            var dispose = new Action<object>(o =>
            {
                var disposable = o as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            });

            dispose(_lifeSpanHandler);
            _lifeSpanHandler = null;

            dispose(_displayHandler);
            _displayHandler = null;

            dispose(_loadHandler);
            _loadHandler = null;

            dispose(_requestHandler);
            _requestHandler = null;

            dispose(_contextMenuHandler);
            _contextMenuHandler = null;

            dispose(_downloadHandler);
            _downloadHandler = null;

            dispose(_jsDialogHandler);
            _jsDialogHandler = null;

            dispose(_keyboardHandler);
            _keyboardHandler = null;
        }

        protected override CefLifeSpanHandler GetLifeSpanHandler()
        {
            return _lifeSpanHandler;
        }

        protected override CefDisplayHandler GetDisplayHandler()
        {
            return _displayHandler;
        }

        protected override CefLoadHandler GetLoadHandler()
        {
            return _loadHandler;
        }

        protected override CefRequestHandler GetRequestHandler()
        {
            return _requestHandler;
        }

        protected override CefContextMenuHandler GetContextMenuHandler()
        {
            return _contextMenuHandler;
        }

        protected override CefDownloadHandler GetDownloadHandler()
        {
            return _downloadHandler;
        }

        protected override CefJSDialogHandler GetJSDialogHandler()
        {
            return _jsDialogHandler;
        }

        protected override CefKeyboardHandler GetKeyboardHandler()
        {
            return _keyboardHandler;
        }

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            if (browser != null)
            {
                // Clone the browser as CEFGlue will dispose the reference passed to this function when it returns
                browser = browser.Clone();
            }
            // Use local variable to guard against client being disposed on different thread (assignment is atomic)
            var core = _core;
            return core != null && core.OnProcessMessageReceived(browser, message);
        }
    }
}