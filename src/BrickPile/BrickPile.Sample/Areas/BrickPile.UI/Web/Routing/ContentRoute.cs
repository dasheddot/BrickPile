/* Copyright (C) 2011 by Marcus Lindblom

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BrickPile.Domain.Models;
using BrickPile.UI.Common;

namespace BrickPile.UI.Web.Routing {

    public class ContentRoute : RouteBase, IRouteWithArea {
        private readonly IPathResolver _pathResolver;
        private readonly IVirtualPathResolver _virtualPathResolver;
        private readonly IRouteHandler _routeHandler;
        private readonly Route _innerRoute;
        public const string ControllerKey = "controller";
        /// <summary>
        /// Gets the name of the area to associate the route with.
        /// </summary>
        /// <returns>The name of the area to associate the route with.</returns>
        public string Area {
            get { return "Dashboard"; }
        }
        /// <summary>
        /// Gets the model key.
        /// </summary>
        public static string ModelKey {
            get { return "model"; }
        }
        /// <summary>
        /// Gets the action key.
        /// </summary>
        public static string ActionKey {
            get { return "action"; }
        }
        /// <summary>
        /// Gets the default action.
        /// </summary>
        public static string DefaultAction {
            get { return "index"; }
        }
        /// <summary>
        /// Gets the default controller.
        /// </summary>
        /// <value>
        /// The default name of the controller.
        /// </value>
        public static string DefaultControllerName {    
            get { return "Content"; }
        }
        /// <summary>
        /// When overridden in a derived class, returns route information about the request.
        /// </summary>
        /// <param name="httpContext">An object that encapsulates information about the HTTP request.</param>
        /// <returns>
        /// An object that contains the values from the route definition if the route matches the current request, or null if the route does not match the request.
        /// </returns>
        public override RouteData GetRouteData(System.Web.HttpContextBase httpContext) {

            // get the virtual path of the request
            var virtualPath = httpContext.Request.CurrentExecutionFilePath;
            
            // abort if the virtual path does not contain dashboard
            if (!IsDashboardRoute(virtualPath)) {
                return null;
            }

            // try to resolve the current item
            
            var pathData = _pathResolver.ResolvePath(virtualPath.Replace("/dashboard/content", "").TrimStart(new[] {'/'}));

            var routeData = new RouteData(this, _routeHandler);

            foreach (var defaultPair in _innerRoute.Defaults)
                routeData.Values[defaultPair.Key] = defaultPair.Value;
            foreach (var tokenPair in _innerRoute.DataTokens)
                routeData.DataTokens[tokenPair.Key] = tokenPair.Value;

            // Abort and proceed to other routes in the route table
            if (pathData == null) {
                return null;
            }
            
            routeData.ApplyCurrentModel(pathData.Controller, pathData.Action, pathData.CurrentPageModel);

            return routeData;
        }
        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values) {

            if (!IsDashboardRoute(requestContext.HttpContext.Request.CurrentExecutionFilePath)) {
                return null;
            }
            var item = values[ModelKey] as IPageModel;
            if (item == null) {
                return null;
            }

            var vpd = _innerRoute.GetVirtualPath(requestContext, values);

            if (vpd == null)
                return null;

            vpd.Route = this;

            

            vpd.VirtualPath = string.Format("dashboard/content/{0}", _virtualPathResolver.ResolveVirtualPath(item, values));

            return vpd;
        }
        private static bool IsDashboardRoute(string virtualPath) {
            return virtualPath.StartsWith("/dashboard/content");
        }
        public ContentRoute(IPathResolver pathResolver, IVirtualPathResolver virtualPathResolver, Route innerRoute) {
            _pathResolver = pathResolver;
            _virtualPathResolver = virtualPathResolver;
            _routeHandler = _routeHandler ?? new MvcRouteHandler();
            _innerRoute = innerRoute ?? new Route("dashboard/{controller}/{action}",
                                                        new RouteValueDictionary(new { controller = "content", action = "index" }),
                                                        new RouteValueDictionary(),
                                                        new RouteValueDictionary(new { area = "Dashboard" }),
                                                        new MvcRouteHandler());
        }
    }
}