// Copyright © 2010-2017 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using CefSharp.Event;

namespace CefSharp
{
    /// <summary>
    /// Javascript object repository, object are registered for binding
    /// One repository per ChromiumWebBrowser instance
    /// </summary>
    public interface IJavascriptObjectRepository : IDisposable
    {
        /// <summary>
        /// Register an object for binding in Javascript. You can either
        /// register an object in advance or as part of the <see cref="ResolveObject"/>
        /// event that will be called if no object matching object is found in the registry.
        /// Objects binding is now initiated in Javascript through the CefSharp.BindObjectAsync
        /// function (returns a Promose).
        /// For more detailed examples see https://github.com/cefsharp/CefSharp/issues/2246
        /// The equivilient to RegisterJsObject is isAsync = false
        /// The equivilient RegisterAsyncJsObject is isAsync = true
        /// </summary>
        /// <param name="name">object name</param>
        /// <param name="objectToBind">the object that will be bound in javascript</param>
        /// <param name="isAsync">
        /// if true the object will be registered for async communication,
        /// only methods will be exposed and when called from javascript will return a Promise to be awaited. 
        /// This method is newer and recommended for everyone starting out as it is faster and more reliable.
        /// If false then methods and properties will be registered, this method relies on a WCF service to communicate.
        /// </param>
        /// <param name="options">binding options, by default method/property names are camelCased, you can control this
        /// and other advanced options though this class.</param>
        void Register(string name, object objectToBind, bool isAsync = false, BindingOptions options = null);
        /// <summary>
        /// Has bound objects
        /// </summary>
        bool HasBoundObjects { get; }
        /// <summary>
        /// Is object bound
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>true if object with matching name bound</returns>
        bool IsBound(string name);
        /// <summary>
        /// Event handler is called when an object with a given name is requested for binding and is not yet
        /// registered with the repository. Use <see cref="JavascriptBindingEventArgs.ObjectRepository"/>
        /// to register objects (using 
        /// </summary>
        event EventHandler<JavascriptBindingEventArgs> ResolveObject;
        /// <summary>
        /// Event handler is triggered when a object has been successfully bound on javascript
        /// </summary>
        event EventHandler<JavascriptBindingEventArgs> ObjectBoundInJavascript;
    }
}
